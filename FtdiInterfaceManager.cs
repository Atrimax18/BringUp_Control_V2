using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class FtdiInterfaceManager : IDisposable
    {

        public enum BusMode
        {
            None,
            SPI,
            I2C
        }
        private IntPtr _ftHandle = IntPtr.Zero;
        private BusMode _currentMode = BusMode.None;

        public event EventHandler<string> BusModeChanged;

        private SpiDriver _spi;
        private i2cDriver _i2c;

        private uint _locId;

        public FtdiInterfaceManager(uint locId)
        {
            _locId = locId;
        }

        public SpiDriver GetSpi()
        {
            if (_currentMode != BusMode.SPI)
            {
                SwitchToSpi();
            }
            return _spi;
        }

        public i2cDriver GetI2c()
        {
            if (_currentMode != BusMode.I2C)
            {
                SwitchToI2c();                                
            }
            return _i2c;
        }

        private void SwitchToSpi()
        {
            Dispose(); // Dispose previous resources if any            
            
            var status = Ft4222Native.FT_OpenEx(_locId, Ft4222Native.FtOpenType.OpenByLocation, out _ftHandle);


            if (status != FTDI.FT_STATUS.FT_OK || _ftHandle == IntPtr.Zero)
                throw new IOException($"FT_OpenEx failed: {status}, Handle: {_ftHandle}");
            

            _spi = new SpiDriver(_ftHandle, Ft4222Native.FT4222_SPI_Mode.SPI_IO_SINGLE, Ft4222Native.FT4222_CLK.CLK_DIV_16, Ft4222Native.FT4222_SPICPOL.CLK_IDLE_LOW, Ft4222Native.FT4222_SPICPHA.CLK_LEADING, 0x01, false);
            
            _currentMode = BusMode.SPI;
            BusModeChanged?.Invoke(this, "SPI Driver initialized"); 
        }

        private void SwitchToI2c()
        {
            Dispose(); // Dispose previous resources if any

            var status = Ft4222Native.FT_OpenEx(_locId, Ft4222Native.FtOpenType.OpenByLocation, out _ftHandle);
            if (status != FTDI.FT_STATUS.FT_OK || _ftHandle == IntPtr.Zero)
                throw new IOException($"FT_OpenEx failed: {status}, Handle: {_ftHandle}");

            
            _i2c = new i2cDriver(_ftHandle, 400, true);
            _currentMode = BusMode.I2C;
            BusModeChanged?.Invoke(this, "I2C Driver initialized");
        }


        public void Dispose()
        {
            _spi?.Dispose(); _spi = null;
            _i2c?.Dispose(); _i2c = null;

            if (_ftHandle != IntPtr.Zero)
            {
                Ft4222Native.FT4222_UnInitialize(_ftHandle);
                Ft4222Native.FT_Close(_ftHandle);
                _ftHandle = IntPtr.Zero;
            }

            _currentMode = BusMode.None;
        }
    }
}
