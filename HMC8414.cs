using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class HMC8414 : IDisposable
    {
        public enum ChipIndex
        {
            HMC8414_CHIP1 = 0,
            HMC8414_CHIP2 = 1
        }

        private i2cDriver _i2c;
        private PCAL6416A _ioExp;
        private FtdiInterfaceManager _interfaceManager;
        private PCAL6416A.PinIndex _vctrlPin;

        public void Init(i2cDriver i2c, PCAL6416A ioExp, FtdiInterfaceManager interfaceManager)
        {
            _i2c = i2c ?? throw new ArgumentNullException(nameof(i2c));
            _ioExp = ioExp ?? throw new ArgumentNullException(nameof(ioExp));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));
        }

        public void SetBypass(ChipIndex idx, bool enable)
        {
            SetAmplifier(idx, !enable); // Invert the enable state for bypass mode
        }

        public void SetAmplifier(ChipIndex idx, bool enable)
        {
            if (_i2c == null || _ioExp == null || _interfaceManager == null)
            {
                throw new InvalidOperationException("HMC8414 not initialized. Call Init() first.");
            }

            if(idx == ChipIndex.HMC8414_CHIP1)
            {
                _vctrlPin = PCAL6416A.PinIndex.CTRL_HMC8414_VCTRL1;
            }
            else if (idx == ChipIndex.HMC8414_CHIP2)
            {
                _vctrlPin = PCAL6416A.PinIndex.CTRL_HMC8414_VCTRL2;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(idx), "Invalid chip index.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // Set the IO Expander CTRL_SPI_EN_1V8 to high to enable the FTDI CS
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, true);

            // Now set the VCTRL pin value for the selected chip
            // Setting VCTRL to 0 V enables bypass mode. Setting VCTRL to 3 V enables amplifier mode
            _ioExp.SetPinStateFromIndex(_vctrlPin, enable);
        }


        public void Dispose() 
        {
            _i2c = null; // Release the FTDI device
        }   
    }
}
