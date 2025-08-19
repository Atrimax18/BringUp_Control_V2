
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private const byte CMD_DEVICE_INFO = 0x08;
        private const byte CMD_TEMPERATURE_READOUT = 0x19;
        private const byte CMD_RESTART = 0xF0;
        private const byte CTS_REPLY = 0x80;

        private SpiDriver _spi;
        private i2cDriver _i2c;
        private PCAL6416A _ioExp;
        private FtdiInterfaceManager _interfaceManager;

        public enum ResiltCode
        {
            OK = 0,
            Error = 1,
            Timeout = 2,
            InvalidData = 3,
            InvalidResponse = 4,
        }

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
            //_ioExp.SetMuxSpiPin(PCAL6416A.MuxSpiIndex.MUX_SPI_CSn_SKY_PLL);



            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_CSN_SEL0, true);
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_CSN_SEL1, true);
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
            Span<byte> buffer = stackalloc byte[2 + data.Length];
            buffer[0] = 0xC0;
            buffer[1] = command;
            data.CopyTo(buffer.Slice(2));

            _spi.Write(buffer);
            Console.WriteLine($"Command 0x{command:X2} sent with data: {BitConverter.ToString(buffer.ToArray())}");
        }

        private ResiltCode SendReceiveWaitCts(byte command, ReadOnlySpan<byte> data, Span<byte> r_buffer, int tout_ms)
        {
            Span<byte> cmd_buffer = stackalloc byte[2 + data.Length];
            cmd_buffer[0] = 0xC0;
            cmd_buffer[1] = command;
            data.CopyTo(cmd_buffer.Slice(2));

            Span<byte> reply_w_buffer = stackalloc byte[r_buffer.Length];
            reply_w_buffer[0] = 0xD0;
            reply_w_buffer.Slice(1).Clear();

            _spi.Write(cmd_buffer);

            int startTime = Environment.TickCount;

            while (true)
            {
                _spi.TransferFullDuplex(reply_w_buffer, r_buffer);
                if ((r_buffer[1] & 0xF0) == CTS_REPLY)
                {
                    // CTS=1 with no Other Error bits
                    break;
                }

                if (tout_ms > 0 && (Environment.TickCount - startTime) >= tout_ms)
                {
                    Console.WriteLine("Timeout waiting for CTS expired!");
                    //throw new TimeoutException("Timeout waiting for CTS expired!");
                    return ResiltCode.Timeout;
                }
            }

            //Console.WriteLine($"Command 0x{command:X2} sent with data: {BitConverter.ToString(cmd_buffer.ToArray())}");
            //Console.WriteLine($"Reply: {BitConverter.ToString(r_buffer.ToArray())}");

            return ResiltCode.OK;
        }

        private ResiltCode LoadConfig(ReadOnlySpan<byte> data, bool toNvm)
        {
            ResiltCode result;
            Span<byte> rd_buffer = stackalloc byte[2];
            byte command = toNvm ? CMD_NVM_LOAD_DATA : CMD_HOST_LOAD;
            int chunkSize = 500;
            Span<byte> chunk = stackalloc byte[chunkSize];
            int offset = 0;

            while (offset < data.Length)
            {
                int size = Math.Min(chunkSize, data.Length - offset);
                data.Slice(offset, size).CopyTo(chunk);
                result = SendReceiveWaitCts(command, chunk.Slice(0, size), rd_buffer, 1000);

                if (result != ResiltCode.OK)
                {
                    Console.WriteLine($"Error sending command 0x{command:X2} at offset {offset}: {result}");
                    return result;
                }

                offset += size;
            }

            Console.WriteLine($"Configuration loaded to {(toNvm ? "NVM" : "RAM")}.");
            return ResiltCode.OK;
        }

        private ResiltCode VerifyNvmBurn()
        {
            ResiltCode result;
            Span<byte> buffer = stackalloc byte[6];
            Span<byte> parse_buffer = buffer.Slice(1);
            Span<byte> args = stackalloc byte[2] { 0xDE, 0xC0 };

            result = SendReceiveWaitCts(CMD_NVM_BURN_VERIFY, args, buffer, 15000);

            if (result != ResiltCode.OK)
            {
                Console.WriteLine($"SendReceive error: {result}");
                return result;
            }

            bool success = (parse_buffer[1] & 0x01) == 0x01 && parse_buffer[4] == 0x00;
            Console.WriteLine($"NVM Burn Verify: {(success ? "Success" : "Failure")}");

            if (!success)
            {
                return ResiltCode.Error;
            }

            return ResiltCode.OK;
        }

        public ResiltCode CheckReferenceStatus(out bool ref_status)
        {
            ResiltCode result;
            Span<byte> rd_buffer = stackalloc byte[5];
            Span<byte> parse_buffer = rd_buffer.Slice(1);
            byte[] emptyData = new byte[0];

            result = SendReceiveWaitCts(CMD_REFERENCE_STATUS, emptyData, rd_buffer, 500);

            if (result != ResiltCode.OK)
            {
                Console.WriteLine($"SendReceive error: {result}");
                ref_status = false;
                return result;
            }

            bool clkValid = (parse_buffer[1] & 0x03) == 0x00;
            bool lossOfSignal = (parse_buffer[2] & 0x01) == 0x00;
            ref_status = clkValid && lossOfSignal;
            Console.WriteLine($"Reference Status: {(ref_status ? "Valid" : "Invalid")}");

            return ResiltCode.OK;
        }

        public ResiltCode ReadTemperature(out double temperature)
        {
            ResiltCode result;
            Span<byte> rd_buffer = stackalloc byte[7];
            Span<byte> parse_buffer = rd_buffer.Slice(1);
            Span<byte> buffer = rd_buffer.Slice(2);
            byte[] emptyData = new byte[0];
            string device_info = string.Empty;

            result = SendReceiveWaitCts(CMD_TEMPERATURE_READOUT, emptyData, rd_buffer, 500);

            if (result != ResiltCode.OK)
            {
                Console.WriteLine($"SendReceive error: {result}");
                temperature = -100.0f; // Invalid temperature
                return result;
            }

            int rawTemp = (parse_buffer[4] << 23) | (parse_buffer[3] << 16) | (parse_buffer[2] << 8) | parse_buffer[1];
            temperature = rawTemp / (float)(1 << 23);

            Console.WriteLine($"Temperature: {temperature:F2} °C");
            return ResiltCode.OK;
        }

        public ResiltCode ReadInfo(out string device_info)
        {
            ResiltCode result;
            Span<byte> rd_buffer = stackalloc byte[8];
            Span<byte> parse_buffer = rd_buffer.Slice(1);
            Span<byte> info_buffer = rd_buffer.Slice(2);
            byte[] emptyData = new byte[0];

            result = SendReceiveWaitCts(CMD_DEVICE_INFO, emptyData, rd_buffer, 500);

            if (result != ResiltCode.OK)
            {
                Console.WriteLine($"SendReceive error: {result}");
                device_info = string.Empty;
                return result;
            }

            device_info = BitConverter.ToString(info_buffer.ToArray());
            Console.WriteLine($"DevInfo: {device_info}");
            return ResiltCode.OK;
        }

        // Public Methods

        public ResiltCode InitPllSynth(string firmwarePath, string configPath)
        {
            ResiltCode result;
            byte[] firmware = ParseHexFile(firmwarePath);
            byte[] config = ParseHexFile(configPath);

            Console.WriteLine($"Firmware length: {firmware.Length}");
            /*
            Console.WriteLine("Firmware content:");
            foreach (var b in firmware)
            {
                Console.WriteLine($"0x{b:X2}");
            }
            */

            Console.WriteLine($"Config length: {config.Length}");
            /*
            Console.WriteLine("Config content:");
            foreach (var b in config)
            {
                Console.WriteLine($"0x{b:X2}");
            }
            */

            //return; // Early return for testing purposes

            if ((result = SioTest()) != ResiltCode.OK) return result;
            if ((result = Restart()) != ResiltCode.OK) return result;
            if ((result = LoadConfig(firmware, toNvm: false)) != ResiltCode.OK) return result;
            if ((result = LoadConfig(config, toNvm: false)) != ResiltCode.OK) return result;
            if ((result = Boot()) != ResiltCode.OK) return result;

            return ResiltCode.OK;
        }

        public ResiltCode BurnNvmPllSynth(string nvBootPath, string firmwarePath, string configPath)
        {
            ResiltCode result;
            byte[] nvBoot = ParseHexFile(nvBootPath);
            byte[] firmware = ParseHexFile(firmwarePath);
            byte[] config = ParseHexFile(configPath);

            Console.WriteLine($"NV Boot length: {nvBoot.Length}");
            /*
            Console.WriteLine("NV Boot content:");
            foreach (var b in nvBoot)
            {
                Console.WriteLine($"0x{b:X2}");
            }
            */

            Console.WriteLine($"Firmware length: {firmware.Length}");
            /*
            Console.WriteLine("Firmware content:");
            foreach (var b in firmware)
            {
                Console.WriteLine($"0x{b:X2}");
            }
            */

            Console.WriteLine($"Config length: {config.Length}");
            /*
            Console.WriteLine("Config content:");
            foreach (var b in config)
            {
                Console.WriteLine($"0x{b:X2}");
            }
            */

            //return; // Early return for testing purposes

            if ((result = SioTest()) != ResiltCode.OK) return result;
            if ((result = Restart()) != ResiltCode.OK) return result;
            if ((result = LoadConfig(nvBoot, toNvm: false)) != ResiltCode.OK) return result;
            if ((result = Boot()) != ResiltCode.OK) return result;
            if ((result = LoadConfig(firmware, toNvm: true)) != ResiltCode.OK) return result;
            if ((result = LoadConfig(config, toNvm: true)) != ResiltCode.OK) return result;
            if ((result = VerifyNvmBurn()) != ResiltCode.OK) return result;

            return ResiltCode.OK;
        }

        public ResiltCode SioTest()
        {
            ResiltCode result;
            Span<byte> rd_buffer = stackalloc byte[5];
            Span<byte> parse_buffer = rd_buffer.Slice(1);
            Span<byte> testData = stackalloc byte[2] { 0xAB, 0xCD };

            result = SendReceiveWaitCts(CMD_SIO_TEST, testData, rd_buffer, 500);

            if (result != ResiltCode.OK)
            {
                Console.WriteLine($"SendReceive error: {result}");
                return result;
            }

            if ((parse_buffer[0] != CTS_REPLY) || (parse_buffer[1] != CMD_SIO_TEST) || (parse_buffer[2] != testData[0]) || (parse_buffer[3] != testData[1]))
            {
                Console.WriteLine("SIO Test failed!");
                return ResiltCode.InvalidResponse;
            }

            Console.WriteLine("SIO Test passed!");
            return ResiltCode.OK;
        }

        public ResiltCode Restart()
        {
            ResiltCode result;
            Span<byte> rd_buffer = stackalloc byte[2];
            Span<byte> testData = stackalloc byte[1] { 0x00 };

            result = SendReceiveWaitCts(CMD_RESTART, testData, rd_buffer, 5000);

            if (result != ResiltCode.OK)
            {
                Console.WriteLine($"SendReceive error: {result}");
                return result;
            }

            Console.WriteLine("Device restarted.");
            return ResiltCode.OK;
        }

        public ResiltCode Boot()
        {
            ResiltCode result;
            Span<byte> rd_buffer = stackalloc byte[2];
            byte[] emptyData = new byte[0];

            result = SendReceiveWaitCts(CMD_BOOT, emptyData, rd_buffer, 1000);

            if (result != ResiltCode.OK)
            {
                Console.WriteLine($"SendReceive error: {result}");
                return result;
            }

            Console.WriteLine("Device boot command sent.");
            return ResiltCode.OK;
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



                    if (line.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        line = line.Substring(2);

                    // Convert to byte
                    byte result = Convert.ToByte(line, 16);
                    memoryStream.WriteByte(result);
                }

                return memoryStream.ToArray();
            }
        }
    }
}

