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
    internal class SpiDriver : IDisposable
    {

        private IntPtr _ftHandle;

        private readonly object _sync = new object();

        private readonly Ft4222Native.FT4222_SPI_Mode _spiMode;
        private readonly Ft4222Native.FT4222_CLK _clkDiv;
        private readonly Ft4222Native.FT4222_SPICPOL _cpol;
        private readonly Ft4222Native.FT4222_SPICPHA _cpha;
        public SpiDriver(uint locId,
                            Ft4222Native.FT4222_SPI_Mode spiMode,
                            Ft4222Native.FT4222_CLK clkDiv,
                            Ft4222Native.FT4222_SPICPOL cpol,
                            Ft4222Native.FT4222_SPICPHA cpha,
                            byte csActiveHigh)
        {
            _spiMode = spiMode;
            _cpol = cpol;
            _cpha = cpha;
            _clkDiv = clkDiv;

            // open by USB‑location ID
            
            var ftStatus = Ft4222Native.FT_OpenEx(locId, Ft4222Native.FtOpenType.OpenByLocation, out _ftHandle);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) // Updated to compare with FTDI.FT_STATUS.FT_OK
                throw new InvalidOperationException($"FT_OpenEx failed: {ftStatus}");
            

            byte csPol = csActiveHigh;// (byte)(csActiveHigh ? 1 : 0);

            Check(Ft4222Native.FT4222_SPIMaster_Init(_ftHandle, _spiMode, _clkDiv, _cpol, _cpha, csPol));
        }
        
         public void Dispose()
        {
            if (_ftHandle != IntPtr.Zero) // Check if the handle is not null
            {
                Ft4222Native.FT4222_UnInitialize(_ftHandle);
                Ft4222Native.FT_Close(_ftHandle);
                _ftHandle = IntPtr.Zero; // Reset the handle to IntPtr.Zero after closing
            }
        }



        public void Read(ReadOnlySpan<byte> buffer)
        {
            ushort readBytes;
            var ftStatus = Ft4222Native.FT4222_SPIMaster_SingleRead(_ftHandle, in MemoryMarshal.GetReference(buffer),
                (ushort)buffer.Length, out readBytes, true);
            if (ftStatus != Ft4222Native.FT4222_STATUS.FT4222_OK)
            {
                throw new IOException($"{nameof(Read)} failed to read, error: {ftStatus}");
            }
        }
        

        public void TransferFullDuplex(ReadOnlySpan<byte> writeBuffer, Span<byte> readBuffer)
        {
            ushort readBytes;
            var ftStatus = Ft4222Native.FT4222_SPIMaster_SingleReadWrite(_ftHandle,
                in MemoryMarshal.GetReference(readBuffer), in MemoryMarshal.GetReference(writeBuffer),
                (ushort)writeBuffer.Length, out readBytes, true);
            if (ftStatus != Ft4222Native.FT4222_STATUS.FT4222_OK)
            {
                throw new IOException($"{nameof(TransferFullDuplex)} failed to do a full duplex transfer, error: {ftStatus}");
            }
        }
        
        public void Write(ReadOnlySpan<byte> buffer)
        {
            ushort bytesWritten;

            lock (_sync)
            { 
                if (buffer.IsEmpty) return; // Check if the buffer is empty

                var ftStatus = Ft4222Native.FT4222_SPIMaster_SingleWrite(_ftHandle, in MemoryMarshal.GetReference(buffer),
                (ushort)buffer.Length, out bytesWritten, true);
                if (ftStatus != Ft4222Native.FT4222_STATUS.FT4222_OK)
                {
                    throw new IOException($"{nameof(Write)} failed to write, error: {ftStatus}");
                }
            }
            
        }
        /*
        public void Write(ReadOnlySpan<byte> buffer)
        {
            lock (_sync) // Global gate
            {
                if (buffer.IsEmpty) return;

                // Convert ReadOnlySpan<byte> to byte[] for compatibility
                byte[] bufferArray = buffer.ToArray();

                ushort written;
                var st = Ft4222Native.FT4222_SPIMaster_SingleWrite(_ftHandle, bufferArray, (ushort)bufferArray.Length, out written, true);

                if (st != Ft4222Native.FT4222_STATUS.FT4222_OK)
                    throw new IOException($"Write() failed → {st}");
            }
        }*/

        private static void Check(Ft4222Native.FT4222_STATUS st)
        {
            if (st != Ft4222Native.FT4222_STATUS.FT4222_OK)
                throw new InvalidOperationException($"LibFT4222 error {st}");
        }
    }
}
