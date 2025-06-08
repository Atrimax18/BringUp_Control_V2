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
        private SpiDriver _ft;

        public void Init(SpiDriver ft)
        {
            _ft = ft ?? throw new ArgumentNullException(nameof(ft));
        }

        /// <summary>
        /// Writes the attenuation value to the specified channel.
        /// </summary>        /// 
        /// <param name="attenuationValue">The attenuation value to write.</param>
        public void WriteAttenuation(byte attenuationValue)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("HMC1119 is not initialized. Call Init() before using this method.");
            }

            // Example SPI write logic for HMC1119
            byte[] command = BuildAttenuationCommand(attenuationValue);
            _ft.Write(command);
        }

        private byte[] BuildAttenuationCommand(byte attenuationValue)
        {
            // Construct the SPI command based on the channel and attenuation value.
            // This is a placeholder implementation. Replace with actual command logic.
            return new byte[] { attenuationValue };
        }

        public void Dispose()
        {
            // Dispose resources if necessary
        }
    }
}
