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

        private IntPtr _gpiohandle;
        

        public GpioDriver(uint interfaceIndex = 1,
                                byte outputMask = 0b_1000,   // GPIO3 out, the rest in
                                bool driveHigh = false)
        {
            // ── open interface-B by Location-ID ───────────────────────────────
            var native = new Ft4222Native();
            uint locId = native.GetDeviceLocId(interfaceIndex);               // :contentReference[oaicite:0]{index=0}&#8203;:contentReference[oaicite:1]{index=1}

            var ftStatus = Ft4222Native.FT_OpenEx(locId, Ft4222Native.FtOpenType.OpenByLocation, out _gpiohandle);               // :contentReference[oaicite:2]{index=2}&#8203;:contentReference[oaicite:3]{index=3}
            if (ftStatus != FTDI.FT_STATUS.FT_OK) // Updated to compare with FTDI.FT_STATUS.FT_OK
                throw new InvalidOperationException($"FT_OpenEx failed: {ftStatus}");

            Check(Ft4222Native.FT4222_SetSuspendOut(_gpiohandle, false), "TEST");
            
            // ----------------------------------------------------------------------------------
            Check(Ft4222Native.FT4222_SetWakeUpInterrupt(_gpiohandle, false), "TEST");

            // ── set directions (0 = output, 1 = input) ──────────────────────
            byte[] dir = new byte[4];                                         // L0-L3
            for (int i = 0; i < 4; i++)
                dir[i] = (byte)(((outputMask >> i) & 0x01) == 1
                                 ? Ft4222Native.GPIO_Dir.GPIO_OUTPUT
                                 : Ft4222Native.GPIO_Dir.GPIO_INPUT);

            Check(Ft4222Native.FT4222_GPIO_Init(_gpiohandle, dir),                // :contentReference[oaicite:4]{index=4}&#8203;:contentReference[oaicite:5]{index=5}
                  "FT4222_GPIO_Init");

            // ── default level for outputs ───────────────────────────────────
            for (byte pin = 0; pin < 4; pin++)
                if (((outputMask >> pin) & 0x01) == 1)
                    Ft4222Native.FT4222_GPIO_Write(_gpiohandle, pin,
                                                   (byte)(driveHigh ? 1 : 0)); // :contentReference[oaicite:6]{index=6}&#8203;:contentReference[oaicite:7]{index=7}
        }

        // ─────────────── public helpers ─────────────────────────────────────
        public void SetDir(byte pin, Ft4222Native.GPIO_Dir dir) =>
            Check(Ft4222Native.FT4222_GPIO_SetDir(_gpiohandle, pin, dir), "SetDir"); // :contentReference[oaicite:8]{index=8}&#8203;:contentReference[oaicite:9]{index=9}

        public void Write(byte pin, bool high) =>
            Check(Ft4222Native.FT4222_GPIO_Write(_gpiohandle, pin, (byte)(high ? 1 : 0)), "Write"); // :contentReference[oaicite:10]{index=10}&#8203;:contentReference[oaicite:11]{index=11}

        public bool Read(byte pin)
        {
            byte v = 0;
            Check(Ft4222Native.FT4222_GPIO_Read(_gpiohandle, pin, out v), "Read");  // :contentReference[oaicite:12]{index=12}&#8203;:contentReference[oaicite:13]{index=13}
            return v != 0;
        }

        private void Gpio3Write(bool high)
        {
            if (_gpiohandle != IntPtr.Zero) return;
            Ft4222Native.FT4222_GPIO_Write(_gpiohandle, (byte)Ft4222Native.GPIO.GPIO3, (byte)(high ? 1 : 0));
        }

        // ─────────────── cleanup ────────────────────────────────────────────
        public void Dispose()
        {
            if (_gpiohandle != IntPtr.Zero) // Replace IsInvalid check with IntPtr.Zero comparison  
            {
                Ft4222Native.FT4222_UnInitialize(_gpiohandle);
                Ft4222Native.FT_Close(_gpiohandle);
                _gpiohandle = IntPtr.Zero; // Reset the handle to IntPtr.Zero after closing  
            }
        }

        // ─────────────── internal check ─────────────────────────────────────
        private static void Check(Ft4222Native.FT4222_STATUS st, string api)
        {
            if (st != Ft4222Native.FT4222_STATUS.FT4222_OK)
                throw new InvalidOperationException($"{api} failed → {st}");
        }


    }
}
