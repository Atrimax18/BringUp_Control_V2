using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class HMC1119 : IDisposable
    {
        public enum ChipIndex
        {
            HMC1119_CHIP1 = 0,
            HMC1119_CHIP2 = 1,
            HMC1119_CHIP3 = 2
        }

        private SpiDriver _ft;

        public void Init(SpiDriver spi, i2cDriver i2c, PCAL6416A ioExp, FtdiInterfaceManager interfaceManager)
        {
            _spi = spi ?? throw new ArgumentNullException(nameof(spi));
            _i2c = i2c ?? throw new ArgumentNullException(nameof(i2c));
            _ioExp = ioExp ?? throw new ArgumentNullException(nameof(ioExp));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));
        }

        public void SetAttenuation(ChipIndex idx, float atten)
        {
            if ((atten < 0) || (atten > 31.75f))
            {
                throw new ArgumentOutOfRangeException(nameof(atten));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(idx), "Invalid chip index.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // First, make sure the IO Expander CTRL_SPI_EN_1V8 is low to enable SPI communication to the HMC1119!
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, false);
            // Select the chip to communicate with
            _ioExp.SetPinStateFromIndex(_chipSelectPin, false);

            byte txdata = (byte)Math.Floor(atten * 4 + 0.5);
            WriteByte(txdata);
        }

        public void WriteByte(byte data)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("HMC1119 is not initialized. Call Init() before using this method.");
            }


            byte[] command = { data };

            _ft.Write(command);
        }

        public void Dispose()
        {
            // Dispose resources if necessary
        }
    }
}
