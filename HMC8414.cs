using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class HMC8414 : IDisposable
    {
        private SpiDriver _ft;

        public void Init(SpiDriver ft)
        {
            _ft = ft;
        }


        public void Dispose() 
        {
            _ft = null; // Release the FTDI device
        }   
    }
}
