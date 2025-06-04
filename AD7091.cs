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
        
        const double Vdd  = 3.3; // 2.5 ????Supply voltage


        public void Init(SpiDriver ft)
        {
            _ft = ft;
            // Additional initialization if needed
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
