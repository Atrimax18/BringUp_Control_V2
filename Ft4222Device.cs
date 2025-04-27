using System;
using System.Buffers;
using System.Collections.Generic;

using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FTD2XX_NET;


namespace BringUp_Control
{
    public class Ft4222Device : IDisposable
    {
        private IntPtr _handle = IntPtr.Zero;
        //private IntPtr _gpioHandle = IntPtr.Zero;
        private SafeHandle _ftHandle;

        private readonly Ft4222Native.FT4222_SPI_Mode _spiMode;
        private readonly Ft4222Native.FT4222_CLK _clkDiv;
        private readonly Ft4222Native.FT4222_SPICPOL _cpol;
        private readonly Ft4222Native.FT4222_SPICPHA _cpha;
        

        public bool IsOpen => _handle != IntPtr.Zero;
        //public bool IsGpioOpen => _gpioHandle != IntPtr.Zero;

        #region ‑‑ ctor / open / close ‑‑       
        public Ft4222Device(uint locId,
                            Ft4222Native.FT4222_SPI_Mode spiMode = Ft4222Native.FT4222_SPI_Mode.SPI_IO_SINGLE,
                            Ft4222Native.FT4222_CLK clkDiv = Ft4222Native.FT4222_CLK.CLK_DIV_16,
                            Ft4222Native.FT4222_SPICPOL cpol = Ft4222Native.FT4222_SPICPOL.CLK_IDLE_LOW,
                            Ft4222Native.FT4222_SPICPHA cpha = Ft4222Native.FT4222_SPICPHA.CLK_LEADING,
                            byte csActiveHigh = 0x01)
        {
            _spiMode = spiMode;
            _cpol = cpol;
            _cpha = cpha;
            _clkDiv = clkDiv;

            // open by USB‑location ID
            var ftStatus = Ft4222Native.FT_OpenEx(locId, 4 /*FT_OPEN_BY_LOCATION*/, out _handle);
            //if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //    throw new InvalidOperationException($"FT_OpenEx failed: {ftStatus}");

            //byte csPol = 0x01;// (byte)(csActiveHigh ? 1 : 0);
            
           // Check(Ft4222Native.FT4222_SPIMaster_Init(_handle, _spiMode, _clkDiv,_cpol, _cpha, csPol));
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                Ft4222Native.FT4222_UnInitialize(_handle);
                Ft4222Native.FT_Close(_handle);
                _handle = IntPtr.Zero;
            }

            
        }
        #endregion

        #region ‑‑ SPI helpers ‑‑
        //public void SpiSelect(byte csPin) => Check(Ft4222Native.FT4222_SPIMaster_SlaveSelect(_handle, csPin));

        public void SpiWrite(ReadOnlySpan<byte> buffer, bool endTxn = true)
        {
            
            ushort txed = 0;
            
            Check(Ft4222Native.FT4222_SPIMaster_SingleWrite(
                _handle, buffer.ToArray(), (ushort)buffer.Length, ref txed, endTxn));
        }

        public void SpiRead(Span<byte> rx, bool endTxn = true)
        {
            ushort rxed = 0;
            Check(Ft4222Native.FT4222_SPIMaster_SingleRead(
                _handle, rx.ToArray(), (ushort)rx.Length, ref rxed, endTxn));
        }

        public void SpiReadWrite(ReadOnlySpan<byte> writebuf, Span<byte> readbuff, bool transmit)
        {
            if (writebuf.Length != readbuff.Length)
                throw new ArgumentException("write buffer and readbuffer must be the same lenght");


            ushort readbytes = 0;


            Check(Ft4222Native.FT4222_SPIMaster_SingleReadWrite(_handle, readbuff.ToArray(), writebuf.ToArray(), (ushort)writebuf.Length, ref readbytes, transmit));
        }

        

        public byte SpiReadReg16(ushort addr)
        {
            Span<byte> buf = stackalloc byte[3] {
                (byte)((addr | 0x8000) >> 8), (byte)(addr & 0xFF), 0x00 };
            Span<byte> rx = stackalloc byte[3];
            SpiWrite(buf, false);
            SpiRead(rx, true);
            return rx[2];
        }

        

        public void SpiWriteReg16(ushort addr, byte data)
        {
            Span<byte> buf = stackalloc byte[3] {
                (byte)(addr >> 8), (byte)(addr & 0xFF), data };
            SpiWrite(buf);
        }
        #endregion

        #region ‑‑ I²C helpers (100 kHz‑1 MHz) ‑‑
        public void I2cInit(uint kbps = 400) =>
            Check(Ft4222Native.FT4222_I2CMaster_Init(_handle, kbps));

        public void I2cWrite(byte devAddr, ReadOnlySpan<byte> data)
        {
            ushort written = 0;
            Check(Ft4222Native.FT4222_I2CMaster_Write(
                _handle, devAddr, data.ToArray(), (ushort)data.Length, ref written));
        }

        public void I2cRead(byte devAddr, Span<byte> data)
        {
            ushort read = 0;
            Check(Ft4222Native.FT4222_I2CMaster_Read(
                _handle, devAddr, data.ToArray(), (ushort)data.Length, ref read));
        }
        #endregion
        


        

        private static void Check(Ft4222Native.FT4222_STATUS st)
        {
            if (st != Ft4222Native.FT4222_STATUS.FT4222_OK)
                throw new InvalidOperationException($"LibFT4222 error {st}");
        }

    }
}
