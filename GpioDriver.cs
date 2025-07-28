using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;

namespace BringUp_Control
{
    public sealed class GpioDriver : IDisposable
    {

        private readonly IntPtr _gpioHandle;

        // Expose the handle as a public property for external access
        public IntPtr Handle => _gpioHandle; 

        private readonly bool _ownsHandle;
        private bool _disposed;
        private readonly object _sync = new object(); // Replace target-typed object creation with explicit type instantiation

        // Open interface‑B by Location‑ID and take ownership of the handle
        public GpioDriver(uint locId,
                          byte outputMask = 0b_1100,
                          bool driveHigh = false)
        {
            var st = Ft4222Native.FT_OpenEx(locId,
                                             Ft4222Native.FtOpenType.OpenByLocation,
                                             out _gpioHandle);
            if (st != FTDI.FT_STATUS.FT_OK)
                throw new IOException($"FT_OpenEx failed: {st}");

            _ownsHandle = true;
            Init(outputMask, driveHigh);
        }

        // Re‑use an already‑opened handle 
        public GpioDriver(IntPtr sharedHandle,
                          byte outputMask = 0b_1100,
                          bool driveHigh = false)
        {
            if (sharedHandle == IntPtr.Zero)
                throw new ArgumentException(nameof(sharedHandle));

            _gpioHandle = sharedHandle;
            _ownsHandle = false;
            Init(outputMask, driveHigh);
        }

        private void Init(byte outputMask, bool driveHigh)
        {    

            // one byte per GPIO pin: 0 = out, 1 = in
            byte[] dir = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                // Skip GPIO0 and GPIO1 (reserved for I2C)
                if (i < 2) continue;

                dir[i] = (byte)(((outputMask >> i) & 0x01) == 1
                                  ? Ft4222Native.GPIO_Dir.GPIO_OUTPUT
                                  : Ft4222Native.GPIO_Dir.GPIO_INPUT);
            }
                

            Check(Ft4222Native.FT4222_GPIO_Init(_gpioHandle, dir));

            // default output level
            for (byte pin = 2; pin < 4; pin++) //Start from GPIO2
                if (((outputMask >> pin) & 0x01) == 1)
                    Ft4222Native.FT4222_GPIO_Write(_gpioHandle, pin,
                                                   (byte)(driveHigh ? 1 : 0));
            Check(Ft4222Native.FT4222_SetSuspendOut(_gpioHandle, false));
            Check(Ft4222Native.FT4222_SetWakeUpInterrupt(_gpioHandle, false));
        }

        public void SetDir(byte pin, Ft4222Native.GPIO_Dir dir)
        {
            lock (_sync)
                Check(Ft4222Native.FT4222_GPIO_SetDir(_gpioHandle, pin, dir));
        }

        public void Write(byte pin, bool high)
        {
            lock (_sync)
                Check(Ft4222Native.FT4222_GPIO_Write(_gpioHandle, pin, (byte)(high ? 1 : 0)));
        }

        public bool Read(byte pin)
        {
            byte v = 0;
            lock (_sync)
                Check(Ft4222Native.FT4222_GPIO_Read(_gpioHandle, pin, out v));
            return v != 0;
        }

        private static void Check(Ft4222Native.FT4222_STATUS st)
        {
            if (st != Ft4222Native.FT4222_STATUS.FT4222_OK)
                throw new IOException($"FT4222 GPIO error: {st}");
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (_ownsHandle && _gpioHandle != IntPtr.Zero)
            {
                Ft4222Native.FT4222_UnInitialize(_gpioHandle);
                Ft4222Native.FT_Close(_gpioHandle);
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        
    }
}
