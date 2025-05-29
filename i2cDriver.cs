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
        private IntPtr _i2cHandle;
        private bool _ownsHandle;
        private ushort _clockKHz;

        public IntPtr Handle => _i2cHandle;
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

        public i2cDriver(IntPtr ftHandle, ushort clockKHz = 400, bool ownsHandle = false)
        {
            if (ftHandle == IntPtr.Zero)
                throw new ArgumentException("Handle cannot be zero.", nameof(ftHandle));
            
            _i2cHandle = ftHandle;
            _clockKHz = clockKHz;

            _ownsHandle = ownsHandle; // will close it in Dispose() if true
            Init(_clockKHz);
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
                _ownsHandle = false;
                Ft4222Native.FT4222_UnInitialize(_i2cHandle);
                Ft4222Native.FT_Close(_i2cHandle);
                _i2cHandle = IntPtr.Zero; // Reset the handle to IntPtr.Zero after closing
            }
        }
    }
}
