using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class AD7091 : IDisposable
    {
        private SpiDriver _ft;
        private i2cDriver _i2c;
        private PCAL6416A _ioExp;
        private FtdiInterfaceManager _interfaceManager;

        const double Vdd  = 3.3; // 2.5 ????Supply voltage


        public void Init(SpiDriver ft, i2cDriver i2c, PCAL6416A ioExp, FtdiInterfaceManager interfaceManager)
        {
            _ft = ft ?? throw new ArgumentNullException(nameof(ft));
            _i2c = i2c ?? throw new ArgumentNullException(nameof(i2c));
            _ioExp = ioExp ?? throw new ArgumentNullException(nameof(ioExp));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));
            // Additional initialization if needed

            // Move this code below to any read/write method as needed
            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // Set the IO Expander CTRL_SPI_EN_1V8 to high to enable the FTDI CS
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, true);
            // Set the IO Expander TMUX1104 address pins to 0x02 to allow the FTDI CS to reach the AD7091
            _ioExp.SetMuxSpiPin(PCAL6416A.MuxSpiIndex.MUX_SPI_CSn_ADC);
            // Now direct CS from FTDI to the AD7091 is enabled and ready for SPI communication

            //Example of toggling the ADC_CONVST pin
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_ADC_CONVST, false); // Set low to start conversion
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_ADC_CONVST, true); // Set high to end conversion

            _ft = _interfaceManager.GetSpi(); // Get current SPI interface
            //... do some SPI communication with the AD7091
        }

        public void Dispose()
        {
            _ft = null;
        }

        public static ushort ConvertVoltageToAdcCode(double voltage)
        {
            // Clamp voltage to be between 0 and VDD
            if (voltage < 0.0)
                voltage = 0.0;
            if (voltage > Vdd)
                voltage = Vdd;

            // Calculate raw ADC code
            double rawCode = voltage * 4095.0 / Vdd; // 12-bit ADC, so max code is 4095

            // Round to nearest integer
            ushort adcCode = (ushort)Math.Round(rawCode);

            // Ensure max 12-bit
            if (adcCode > 0x0FFF)
                adcCode = 0x0FFF;

            return adcCode;
        }

        public static double ConvertAdcCodeToVoltage(ushort rawAdcCode)
        {
            // Sanity check: raw code must be 0 .. 4095
            if (rawAdcCode > 0x0FFF)
                rawAdcCode = 0x0FFF;

            // Convert ADC code to voltage
            double voltage = (rawAdcCode * Vdd) / 4095.0;

            return voltage;
        }
    }
}
