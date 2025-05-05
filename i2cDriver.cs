using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTD2XX_NET;
using System.Runtime.InteropServices;
using System.IO;

namespace BringUp_Control
{
    internal sealed class i2cDriver : IDisposable
    {
        private readonly IntPtr _i2cHandle;
        private readonly bool _ownsHandle;
        public enum I2C_MasterFlag : byte
        {
            NONE = 0x80,
            START = 0x02,
            Repeated_START = 0x03,     // Repeated_START will not send master code in HS mode
            STOP = 0x04,
            START_AND_STOP = 0x06,      // START condition followed by SEND and STOP condition
        };

        // Open Handle , the frequency is set to 400KHz - will be dinamic in next update
        public i2cDriver(IntPtr sharedHandle, uint kbps = 400)
        {
            if (sharedHandle == IntPtr.Zero)
                throw new ArgumentException("Handle cannot be zero.", nameof(sharedHandle));

            _i2cHandle = sharedHandle;
            _ownsHandle = false;               // don’t close it in Dispose()
            Init(kbps);
        }

        // Open a fresh interface by Location-ID
        public i2cDriver(uint locId, uint kbps = 400)
        {
            var st = Ft4222Native.FT_OpenEx(locId, Ft4222Native.FtOpenType.OpenByLocation, out _i2cHandle);
            if (st != FTDI.FT_STATUS.FT_OK)
                throw new IOException($"FT_OpenEx failed: {st}");

            _ownsHandle = true;   // close it in Dispose()
            Init(kbps);
        }

        

        private void Init(uint kbps)
        {
            Check(Ft4222Native.FT4222_I2CMaster_Init(_i2cHandle, kbps));
        }

        private static void Check(Ft4222Native.FT4222_STATUS st)
        {
            if (st != Ft4222Native.FT4222_STATUS.FT4222_OK)
                throw new InvalidOperationException($"FT4222 I²C error → {st}");
        }

        public void Read(byte devAddr, Span<byte> buffer)
        {
            ushort read = 0;
            //Check(Ft4222Native.FT4222_I2CMaster_Read(_i2cHandle, devAddr, buffer.ToArray(), (ushort)buffer.Length, ref read));
            Check(Ft4222Native.FT4222_I2CMaster_Read(_i2cHandle, devAddr, in MemoryMarshal.GetReference(buffer), (ushort)buffer.Length, ref read));
        }

        public void Write(byte devAddr, ReadOnlySpan<byte> buffer)
        {
            ushort written = 0;
            //Check(Ft4222Native.FT4222_I2CMaster_Write(_i2cHandle, devAddr, buffer.ToArray(), (ushort)buffer.Length, ref written));
            Check(Ft4222Native.FT4222_I2CMaster_Write(_i2cHandle, devAddr, in MemoryMarshal.GetReference(buffer), (ushort)buffer.Length, ref written));
        }
        
        public void Dispose()
        {
            if (_ownsHandle && _i2cHandle != IntPtr.Zero)
            {
                Ft4222Native.FT4222_UnInitialize(_i2cHandle);
                Ft4222Native.FT_Close(_i2cHandle);
            }
        }
    }
}
