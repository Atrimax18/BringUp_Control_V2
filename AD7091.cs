using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class AD7091 : IDisposable
    {
        private readonly SpiDriver _ft;


        public void Dispose()
        {
            _ft?.Dispose();
        }
    }
}
