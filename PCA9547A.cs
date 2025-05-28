using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class PCA9547A : IDisposable
    {
        private i2cDriver _ft;

        private const byte PCAL9547A_I2C_MUX_ADDRESS0 = 0x70;
        private const byte PCAL9547A_I2C_MUX_ADDRESS1 = 0x71;

        public void Init(i2cDriver ft)
        {
            _ft = ft;
        }

        private void WriteByteSimple(byte i2C_address, byte regAddr, in byte data)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("I2C driver is not initialized. Call Init() first.");
            }

            if (data == 0x00)
            {
                ReadOnlySpan<byte> buff_wr = stackalloc byte[1] { regAddr };
                _ft.Write(i2C_address, buff_wr);
            }
            else
            {
                ReadOnlySpan<byte> buff_wr = stackalloc byte[2] { regAddr, data };
                _ft.Write(i2C_address, buff_wr);
            }
        }

        private void ReadByteSimple(byte i2c_address, byte regAddr, out byte data)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("I2C driver is not initialized. Call Init() first.");
            }

            ReadOnlySpan<byte> registerAddress = stackalloc byte[1] { regAddr };
            Span<byte> buff_rd = stackalloc byte[1];

            _ft.Write(i2c_address, registerAddress);
            _ft.Read(i2c_address, buff_rd);

            data = buff_rd[0];
        }
        public void Set_Mux_Channel(int i2c, int mux)
        {
            byte val;
            byte i2caddress;
            if (i2c == 0)
                i2caddress = PCAL9547A_I2C_MUX_ADDRESS0;
            else
                i2caddress = PCAL9547A_I2C_MUX_ADDRESS1;

            if (mux >= 0 && mux <= 7)
            {
                // IMPLEMENTATION PCA9548A MUX Channel Selection
                // Set the mux channel to the specified value (0-7) 
                // Each channel corresponds to a bit in the byte, so we shift 1 left by the mux value
                // Example: mux = 0 -> 0b00000001, mux = 1 -> 0b00000010, ..., mux = 7 -> 0b10000000
                //val = (byte)(1 << mux);

                // For PCA9547, the channel selection is only 3 LSB bits, bit 4 is value 1 always.
                val = (byte)(0x08 + (mux & 0x07));

                // Write to PCA9547B (I2C Addr 0x70) Control Register
                // No data field - data goes in the 'register address' field
                WriteByteSimple(i2caddress, val, 0);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(mux), "Mux channel must be between 0 and 7.");
            }


        }

        public void Dispose()
        {
            _ft?.Dispose();
        }
    }
}
