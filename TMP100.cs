using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BringUp_Control
{
    internal class TMP100 : IDisposable
    {
        private static readonly byte[] TMP100_I2C_ADDRESSES = { 0x48, 0x49 };//{ 0x48 - FT sensor, 0x49 - RF sensor };
        private const byte TMP100_TEMP_REGISTER = 0x00; // Temperature register address
        private const byte TMP100_CONFIG_REGISTER = 0x01; // Configuration register address
        private const byte TMP100_THIGH_REGISTER = 0x03; // T-high register address
        private const byte TMP100_MAGIC_NUMBER = 0x6D; // Magix number for self check
        private static readonly string[] TMP100_DEVICE_NAMES = { "TMP100_FTDI_CHIP", "TMP100_RF_CHIP" };

        public enum AddressIndex
        {
            TMP100_FTDI_CHIP = 0, // Corresponds to 0x48
            TMP100_RF_CHIP = 1  // Corresponds to 0x49
        }

        private i2cDriver _ft;
        private FtdiInterfaceManager _interfaceManager;

        public void Init(i2cDriver ft, FtdiInterfaceManager interfaceManager)
        {
            _ft = ft ?? throw new ArgumentNullException(nameof(ft));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));
        }

        public void Config(AddressIndex addressIndex)
        {
            byte configValue = 0x60; // 12-bit resolution, 1-shot mode

            _ft = _interfaceManager.GetI2c(); // Get current I2C interface

            // Write the desired configuration value to the configuration register
            WriteByte(addressIndex, TMP100_CONFIG_REGISTER, configValue);
        }
        public bool BultInTest(AddressIndex addressIndex)
        {
            _ft = _interfaceManager.GetI2c(); // Get current I2C interface

            //Read the current content of the T-high register address
            ReadByte(addressIndex, TMP100_THIGH_REGISTER, out byte restoreValue);
            // Write the magic number into the T-high register address of the device
            WriteByte(addressIndex, TMP100_THIGH_REGISTER, TMP100_MAGIC_NUMBER);
            //Read the current content of the T-high register address
            ReadByte(addressIndex, TMP100_THIGH_REGISTER, out byte verifyVal);
            // Restore the original value into the T-high register address of the device
            WriteByte(addressIndex, TMP100_THIGH_REGISTER, restoreValue);

            if (verifyVal == TMP100_MAGIC_NUMBER)
            {
                Console.WriteLine($"{TMP100_DEVICE_NAMES[(int)addressIndex]} self-test passed.");
                return true;
            }
            else
            {
                Console.WriteLine($"{TMP100_DEVICE_NAMES[(int)addressIndex]} self-test failed.");
                return false;
            }
        }

        public double ReadTemperature(AddressIndex addressIndex)
        {
            _ft = _interfaceManager.GetI2c(); // Get current I2C interface

            ReadWord(addressIndex, TMP100_TEMP_REGISTER, out ushort rawTemperature);
            // TMP100 temperature is in the upper 12 bits, shift right by 4
            rawTemperature >>= 4;
            // Convert to Celsius (TMP100 uses 0.0625°C per LSB)
            double temperatureCelsius = rawTemperature * 0.0625;
            Console.WriteLine($"{TMP100_DEVICE_NAMES[(int)addressIndex]} temperature is {temperatureCelsius:F2} °C.");
            return temperatureCelsius;
        }

        private void WriteByte(AddressIndex addressIndex, byte regAddr, in byte data)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("I2C driver is not initialized. Call Init() first.");
            }

            byte deviceAddress = TMP100_I2C_ADDRESSES[(int)addressIndex];

            // Buffer to hold the register address and data to be written
            ReadOnlySpan<byte> buff_wr = stackalloc byte[2] { regAddr, data };
            // Write the register address and data to the device
            _ft.Write(deviceAddress, buff_wr);
        }

        private void ReadByte(AddressIndex addressIndex, byte regAddr, out byte data)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("I2C driver is not initialized. Call Init() first.");
            }

            byte deviceAddress = TMP100_I2C_ADDRESSES[(int)addressIndex];

            // Buffer to hold the register address to the device
            ReadOnlySpan<byte> registerAddress = stackalloc byte[1] { regAddr };
            // Buffer to hold 1 byte data
            Span<byte> buff_rd = stackalloc byte[1];
            // Write the register address to the device
            _ft.Write(deviceAddress, registerAddress);
            // Read 1 byte from the requested register
            _ft.Read(deviceAddress, buff_rd);
            data = buff_rd[0];
        }

        private void ReadWord(AddressIndex addressIndex, byte regAddr, out ushort data)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("I2C driver is not initialized. Call Init() first.");
            }

            byte deviceAddress = TMP100_I2C_ADDRESSES[(int)addressIndex];

            // Buffer to hold the register address to the device
            ReadOnlySpan<byte> registerAddress = stackalloc byte[1] { regAddr };
            // Buffer to hold 2 bytes of data
            Span<byte> buff_rd = stackalloc byte[2];
            // Write the register address to the device
            _ft.Write(deviceAddress, registerAddress);
            // Read 2 bytes from the requested register
            _ft.Read(deviceAddress, buff_rd);
            // Combine the two bytes into a single value
            data = (ushort)(((buff_rd[0] << 8) & 0xFF00) | (buff_rd[1] & 0x00FF));
        }

        public void Dispose()
        {
            
                _ft = null; // Release the I2C driver reference
            
        }
    }
}
