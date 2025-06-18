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

        public void Init(SpiDriver ft)
        {
            _ft = ft ?? throw new ArgumentNullException(nameof(ft));
        }

        public void SetAttenuation(ChipIndex idx, float atten)
        {
            if ((atten < 0) || (atten > 31.75f))
            {
                throw new ArgumentOutOfRangeException(nameof(atten));
            }

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
