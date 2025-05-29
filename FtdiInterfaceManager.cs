using System;
using System.Collections.Generic;
using System.Linq;
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

        private SpiDriver _spi;
        private i2cDriver _i2c;

        private readonly uint _locId;

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
            if (status == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                throw new InvalidOperationException($"Failed to open FTDI device{status}.");

            _spi = new SpiDriver(_locId, Ft4222Native.FT4222_SPI_Mode.SPI_MODE0, Ft4222Native.FT4222_CLK.CLK_DIV_1, Ft4222Native.FT4222_SPICPOL.CPOL_LOW, Ft4222Native.FT4222_SPICPHA.CPHA_FIRST, 0);
            _currentMode = BusMode.SPI;
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
