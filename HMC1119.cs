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
        private SpiDriver _spi;
        private i2cDriver _i2c;
        private PCAL6416A _ioExp;
        private FtdiInterfaceManager _interfaceManager;
        public enum ChipIndex
        {
            HMC1119_CHIP1 = 0,
            HMC1119_CHIP2 = 1,
            HMC1119_CHIP3 = 2
        }

        

        public void Init(SpiDriver spi, i2cDriver i2c, PCAL6416A ioExp, FtdiInterfaceManager interfaceManager)
        {
            _spi = spi ?? throw new ArgumentNullException(nameof(spi));
            _i2c = i2c ?? throw new ArgumentNullException(nameof(i2c));
            _ioExp = ioExp ?? throw new ArgumentNullException(nameof(ioExp));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));

            //_ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, false);

        }

        public void SetAttenuation(ChipIndex idx, float atten)
        {
            if ((atten < 0) || (atten > 31.75f))
            {
                throw new ArgumentOutOfRangeException(nameof(atten));
            }
            else
            {
                _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
                _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
                                   // First, make sure the IO Expander CTRL_SPI_EN_1V8 is low to enable SPI communication to the HMC1119!
                _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, false);

                if (idx == ChipIndex.HMC1119_CHIP1)
                {
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE1, false);
                }
                else if (idx == ChipIndex.HMC1119_CHIP2)
                {
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE2, false);
                }
                else if (idx == ChipIndex.HMC1119_CHIP3)
                {
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE3, false);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(idx), "Invalid chip index.");
                }


                _spi = _interfaceManager.GetSpi();
                int code = (int)Math.Round(atten / 0.25, MidpointRounding.AwayFromZero);
                
                byte txdata = (byte)(code & 0x7F); // Ensure we only use the lower 6 bits                
                WriteByte(txdata);


                _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
                _ioExp.Init(_i2c);

                if (idx == ChipIndex.HMC1119_CHIP1)
                {
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE1, true);
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE1, false);
                }
                else if (idx == ChipIndex.HMC1119_CHIP2)
                {
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE2, true);
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE2, false);
                }
                else if (idx == ChipIndex.HMC1119_CHIP3)
                {
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE3, true);
                    _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE3, false);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(idx), "Invalid chip index.");
                }

            }
            
        }

        public void WriteByte(byte data)
        {
            if (_spi == null)
            {
                throw new InvalidOperationException("HMC1119 is not initialized. Call Init() before using this method.");
            }


            byte[] command = { data };

            _spi.Write(command);
        }

        public void Dispose()
        {
            // Dispose resources if necessary
        }
    }
}
