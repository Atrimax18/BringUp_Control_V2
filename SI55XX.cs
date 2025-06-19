using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class SI55XX : IDisposable
    {
        // SPI Commands
        private const byte CMD_READ_REPLY = 0x00;
        private const byte CMD_SIO_TEST = 0x01;
        private const byte CMD_HOST_LOAD = 0x05;
        private const byte CMD_BOOT = 0x07;
        private const byte CMD_NVM_LOAD_DATA = 0xF1;
        private const byte CMD_NVM_BURN_VERIFY = 0xF2;
        private const byte CMD_REFERENCE_STATUS = 0x16;
        private const byte CMD_TEMPERATURE_READOUT = 0x19;

        private SpiDriver _spi;
        private i2cDriver _i2c;
        private PCAL6416A _ioExp;
        private FtdiInterfaceManager _interfaceManager;

        public void Init(SpiDriver spi, i2cDriver i2c, PCAL6416A ioExp, FtdiInterfaceManager interfaceManager)
        {
            _spi = spi ?? throw new ArgumentNullException(nameof(spi), "SPI driver cannot be null.");
            _i2c = i2c ?? throw new ArgumentNullException(nameof(i2c));
            _ioExp = ioExp ?? throw new ArgumentNullException(nameof(ioExp));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // Set the IO Expander CTRL_SPI_EN_1V8 to high to enable the FTDI CS
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, true);
            // Set the IO Expander TMUX1104 address pins to 0x03 to allow the FTDI CS to reach the Si5518
            _ioExp.SetMuxSpiPin(PCAL6416A.MuxSpiIndex.MUX_SPI_CSn_PLL);
            // Now direct CS from FTDI to the Si5518 is enabled and ready for SPI communication
            _spi = _interfaceManager.GetSpi(); // Get current SPI interface
        }


        public void Dispose()
        {
            _spi?.Dispose();
            _spi = null;
        }
        // Private Methods

        private void SendCommand(byte command, ReadOnlySpan<byte> data = default)
        {
            Span<byte> buffer = stackalloc byte[1 + data.Length];
            buffer[0] = command;
            data.CopyTo(buffer.Slice(1));

            _spi.Write(buffer);
            Console.WriteLine($"Command 0x{command:X2} sent with data: {BitConverter.ToString(buffer.ToArray())}");
        }

        private void LoadConfig(ReadOnlySpan<byte> data, bool toNvm)
        {
            byte command = toNvm ? CMD_NVM_LOAD_DATA : CMD_HOST_LOAD;
            int chunkSize = 500;
            Span<byte> chunk = stackalloc byte[chunkSize];
            int offset = 0;

            while (offset < data.Length)
            {
                int size = Math.Min(chunkSize, data.Length - offset);
                data.Slice(offset, size).CopyTo(chunk);
                SendCommand(command, chunk.Slice(0, size));
                offset += size;
            }

            Console.WriteLine($"Configuration loaded to {(toNvm ? "NVM" : "RAM")}.");
        }

        private bool VerifyNvmBurn()
        {
            Span<byte> buffer = stackalloc byte[5];
            Span<byte> args = stackalloc byte[2] { 0xDE, 0xC0 };

            SendCommand(CMD_NVM_BURN_VERIFY, args);
            Read(CMD_NVM_BURN_VERIFY, buffer);

            bool success = (buffer[1] & 0x01) == 0x01 && buffer[4] == 0x00;
            Console.WriteLine($"NVM Burn Verify: {(success ? "Success" : "Failure")}");
            return success;
        }

        private bool CheckReferenceStatus()
        {
            Span<byte> buffer = stackalloc byte[5];
            Read(CMD_REFERENCE_STATUS, buffer);

            bool clkValid = (buffer[1] & 0x03) == 0x00;
            bool lossOfSignal = (buffer[2] & 0x01) == 0x00;

            bool status = clkValid && lossOfSignal;
            Console.WriteLine($"Reference Status: {(status ? "Valid" : "Invalid")}");
            return status;
        }

        private double ReadTemperature()
        {
            Span<byte> buffer = stackalloc byte[5];
            Read(CMD_TEMPERATURE_READOUT, buffer);

            int rawTemp = (buffer[4] << 23) | (buffer[3] << 16) | (buffer[2] << 8) | buffer[1];
            double temperature = rawTemp / (double)(1 << 23);

            Console.WriteLine($"Temperature: {temperature:F2} °C");
            return temperature;
        }

        private void Read(byte command, Span<byte> buffer)
        {
            Span<byte> writeBuffer = stackalloc byte[1];
            writeBuffer[0] = command;

            _spi.TransferFullDuplex(writeBuffer, buffer);
            Console.WriteLine($"Read from command 0x{command:X2}: {BitConverter.ToString(buffer.ToArray())}");
        }

        // Public Methods

        public void LoadPllConfig(string filePath, bool isBinary, bool toNvm)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Configuration file not found.", filePath);
            }

            byte[] data = isBinary ? File.ReadAllBytes(filePath) : ParseHexFile(filePath);
            LoadConfig(data, toNvm);
        }

        public void InitPllSynth(string firmwarePath, string configPath)
        {
            byte[] firmware = ParseHexFile(firmwarePath);
            byte[] config = ParseHexFile(configPath);

            Console.WriteLine($"Firmware length: {firmware.Length}");
            Console.WriteLine($"Config length: {config.Length}");

            SioTest();
            Restart();
            LoadConfig(firmware, toNvm: false);
            LoadConfig(config, toNvm: false);
            Boot();
        }

        public void BurnNvmPllSynth(string nvBootPath, string firmwarePath, string configPath)
        {
            byte[] nvBoot = ParseHexFile(nvBootPath);
            byte[] firmware = ParseHexFile(firmwarePath);
            byte[] config = ParseHexFile(configPath);

            Console.WriteLine($"NV Boot length: {nvBoot.Length}");
            Console.WriteLine($"Firmware length: {firmware.Length}");
            Console.WriteLine($"Config length: {config.Length}");

            SioTest();
            Restart();
            LoadConfig(nvBoot, toNvm: false);
            Boot();
            LoadConfig(firmware, toNvm: true);
            LoadConfig(config, toNvm: true);
            VerifyNvmBurn();
        }

        public void SioTest()
        {
            Span<byte> testData = stackalloc byte[2] { 0xAB, 0xCD };
            Span<byte> reply = stackalloc byte[4];

            SendCommand(CMD_SIO_TEST, testData);
            Read(CMD_SIO_TEST, reply);

            if (reply[1] != CMD_SIO_TEST || reply[2] != testData[0] || reply[3] != testData[1])
            {
                throw new InvalidOperationException("SIO Test failed: Echo response mismatch.");
            }

            Console.WriteLine("SIO Test passed.");
        }

        public void Restart()
        {
            SendCommand(CMD_BOOT);
            Console.WriteLine("Device restarted.");
        }

        public void Boot()
        {
            SendCommand(CMD_BOOT);
            Console.WriteLine("Device boot command sent.");
        }

        public bool VerifyNvm()
        {
            return VerifyNvmBurn();
        }

        public bool CheckRefStatus()
        {
            return CheckReferenceStatus();
        }

        public double GetTemperature()
        {
            return ReadTemperature();
        }

        // Helper Methods

        private byte[] ParseHexFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                MemoryStream memoryStream = new MemoryStream();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(":"))
                    {
                        line = line.Substring(1); // Remove the colon
                    }

                    for (int i = 0; i < line.Length; i += 2)
                    {
                        byte value = Convert.ToByte(line.Substring(i, 2), 16);
                        memoryStream.WriteByte(value);
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}
