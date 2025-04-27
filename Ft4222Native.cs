using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FTD2XX_NET;


namespace BringUp_Control
{
    public class Ft4222Native
    {

#if WIN64
        const string DLL="LibFT4222-64.dll";
#else
        const string DLL = "LibFT4222.dll";
#endif
        
        // ============================== SPI ===================================================
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_SPIMaster_Init(IntPtr ftHandle, FT4222_SPI_Mode ioMode, FT4222_CLK clockDiv, FT4222_SPICPOL sclkPolarity, FT4222_SPICPHA sclkPhase, byte ssoMap);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]        
        public static extern FT4222_STATUS FT4222_SPIMaster_SingleRead(IntPtr ftHandle, in byte buffer, ushort bufferSize, out ushort sizeOfRead, bool isEndTransaction);
        
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]        
        public static extern FT4222_STATUS FT4222_SPIMaster_SingleWrite(IntPtr ftHandle, in byte buffer, ushort bufferSize, out ushort sizeTransferred, bool isEndTransaction);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_SPIMaster_SingleReadWrite(IntPtr ftHandle, in byte readBuffer, in byte writeBuffer, ushort bufferSize, out ushort sizeTransferred, bool isEndTransaction);
        

        // ============================= I2C ===================================================
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_I2CMaster_Init(IntPtr ftHandle, uint kbps);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_I2CMaster_Read(IntPtr ftHandle, byte deviceAddress, byte[] buffer, ushort bytesToRead, ref ushort bytesRead);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_I2CMaster_Write(IntPtr ftHandle, byte deviceAddress, byte[] buffer, ushort bytesToWrite, ref ushort bytesWritten);

        // ============================ GPIO =======================================================
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_GPIO_Init(IntPtr ftHandlee, byte[] dir);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_SetSuspendOut(IntPtr ftHandle, bool enable);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_SetWakeUpInterrupt(IntPtr ftHandle, bool enable);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_GPIO_SetDir(IntPtr ftHandle, byte gpioPort, GPIO_Dir direction);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_GPIO_Read(IntPtr ftHandle, byte gpioPort, out byte value);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_GPIO_Write(IntPtr ftHandle, byte gpioPort, byte value);

        // ======================== Misc ===========================================================
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FT4222_STATUS FT4222_UnInitialize(IntPtr ftHandle);

        //***********************************************************************************************************************************
        //                                                                                                                                  *
        // FUNCTION IMPORTS FROM FTD2XX DLL                                                                                                 *
        //                                                                                                                                  *
        //***********************************************************************************************************************************

        [DllImport("ftd2xx.dll")]
        public static extern FTDI.FT_STATUS FT_CreateDeviceInfoList(out UInt32 numdevs);

        [DllImport("ftd2xx.dll")]
        public static extern FTDI.FT_STATUS FT_GetDeviceInfoDetail(UInt32 index, ref UInt32 flags, ref FTDI.FT_DEVICE chiptype, ref UInt32 id, ref UInt32 locid, byte[] serialnumber, byte[] description, ref IntPtr ftHandle);
        ;

        [DllImport("ftd2xx.dll")]
        public static extern FTDI.FT_STATUS FT_OpenEx(uint pvArg1, FtOpenType dwFlags, out IntPtr ftHandle);
        

        [DllImport("ftd2xx.dll")]        
        public static extern FTDI.FT_STATUS FT_Close(IntPtr ftHandle);

        public enum FtDeviceType
        {
            //
            // Summary:
            //     FT232B or FT245B device
            Ft232BOrFt245B,
            //
            // Summary:
            //     FT8U232AM or FT8U245AM device
            Ft8U232AmOrFTtU245Am,
            //
            // Summary:
            //     FT8U100AX device
            Ft8U100Ax,
            //
            // Summary:
            //     Unknown device
            UnknownDevice,
            //
            // Summary:
            //     FT2232 device
            Ft2232,
            //
            // Summary:
            //     FT232R or FT245R device
            Ft232ROrFt245R,
            //
            // Summary:
            //     FT2232H device
            Ft2232H,
            //
            // Summary:
            //     FT4232H device
            Ft4232H,
            //
            // Summary:
            //     FT232H device
            Ft232H,
            //
            // Summary:
            //     FT X-Series device
            FtXSeries,
            //
            // Summary:
            //     FT4222 hi-speed device Mode 0 - 2 interfaces
            Ft4222HMode0or2With2Interfaces,
            //
            // Summary:
            //     FT4222 hi-speed device Mode 1 or 2 - 4 interfaces
            Ft4222HMode1or2With4Interfaces,
            //
            // Summary:
            //     FT4222 hi-speed device Mode 3 - 1 interface
            Ft4222HMode3With1Interface,
            //
            // Summary:
            //     OTP programmer board for the FT4222.
            Ft4222OtpProgrammerBoard,
            //
            // Summary:
            //     FT900 Device
            Ft900,
            //
            // Summary:
            //     FT930 Device
            Ft930,
            //
            // Summary:
            //     FTUMFTPD3A Device
            FtUmftpd3A,
            //
            // Summary:
            //     FT2233HP Device
            Ft2233HP,
            //
            // Summary:
            //     FT4233HP Device
            Ft4233HP,
            //
            // Summary:
            //     FT2232HP Device
            Ft2232HP,
            //
            // Summary:
            //     FT4232HP Device
            Ft4232HP,
            //
            // Summary:
            //     FT233HP Device
            Ft233HP,
            //
            // Summary:
            //     FT232HP Device
            Ft232HP,
            //
            // Summary:
            //     FT2232HA Device
            Ft2232HA,
            //
            // Summary:
            //     FT4232HA Device
            Ft4232HA
        }
        public enum FT4222_STATUS : uint
        {
            FT4222_OK = 0,
            FT4222_INVALID_HANDLE,
            FT4222_DEVICE_NOT_FOUND,
            FT4222_DEVICE_NOT_OPENED,
            FT4222_IO_ERROR,
            FT4222_INSUFFICIENT_RESOURCES,
            FT4222_INVALID_PARAMETER,
            FT4222_INVALID_BAUD_RATE,
            FT4222_DEVICE_NOT_SUPPORTED,
            FT4222_OTHER_ERROR
        }

        public enum FT4222_SPI_Mode
        {
            SPI_IO_NONE = 0,
            SPI_IO_SINGLE = 1,
            SPI_IO_DUAL = 2,
            SPI_IO_QUAD = 3,
        }

        public enum FT_STATUS : uint
        {
            FT_OK = 0,
            FT_INVALID_HANDLE,
            FT_DEVICE_NOT_FOUND,
            FT_DEVICE_NOT_OPENED,
            FT_IO_ERROR,
            FT_INSUFFICIENT_RESOURCES,
            FT_INVALID_PARAMETER,
            FT_INVALID_BAUD_RATE,
            FT_DEVICE_NOT_OPENED_FOR_ERASE,
            FT_DEVICE_NOT_OPENED_FOR_WRITE,
            FT_FAILED_TO_WRITE_DEVICE,
            FT_EEPROM_READ_FAILED,
            FT_EEPROM_WRITE_FAILED,
            FT_EEPROM_ERASE_FAILED,
            FT_EEPROM_NOT_PRESENT,
            FT_EEPROM_NOT_PROGRAMMED,
            FT_INVALID_ARGS,
            FT_NOT_SUPPORTED,
            FT_OTHER_ERROR
        }

        public enum FT4222_CLK
        {
            CLK_DIV_2 = 0,
            CLK_DIV_4,
            CLK_DIV_8,
            CLK_DIV_16,
            CLK_DIV_32,
            CLK_DIV_64,
            CLK_DIV_128,
            CLK_DIV_256
        }

        public enum GPIO_Dir : byte
        {
            GPIO_OUTPUT = 0,
            GPIO_INPUT = 1
        }

        public enum FtOpenType
        {
            OpenBySerialNumber = 1,
            OpenByDescription = 2,
            OpenByLocation = 4
        }

        public enum GPIO
        {
            GPIO0 = 0,
            GPIO1 = 1,
            GPIO2 = 2,
            GPIO3 = 3,
        }

        public enum FT4222_SPICPHA
        {
            CLK_LEADING = 0,
            CLK_TRAILING = 1,
        };

        public enum SPI_DrivingStrength
        {
            DS_4MA = 0,
            DS_8MA,
            DS_12MA,
            DS_16MA,
        };
        public enum FT4222_SPICPOL
        {
            CLK_IDLE_LOW = 0,
            CLK_IDLE_HIGH = 1,
        };

        public uint NumDevices()
        {
            UInt32 numofDevices = 0;
            _ = FT_CreateDeviceInfoList(out numofDevices); // Changed 'ref' to 'out' to fix CS1620  

            return numofDevices;
        }  
        

        public bool DeviceFlag(uint devices)
        {
            if (devices == 0) return false;
            else return true;
        }

        static FTDI.FT_DEVICE_INFO_NODE deviceA;

        public void GetInfo(uint numdevices, ref List<string> datdevice)
        {
            deviceA = new FTDI.FT_DEVICE_INFO_NODE();

            for (uint i = 0; i < numdevices; i++)
            {
                FTDI.FT_DEVICE_INFO_NODE devInfo = new FTDI.FT_DEVICE_INFO_NODE();
                byte[] sernum = new byte[16];
                byte[] desc = new byte[64];

                FT_GetDeviceInfoDetail(i, ref devInfo.Flags, ref devInfo.Type, ref devInfo.ID, ref devInfo.LocId, sernum, desc, ref devInfo.ftHandle);
                
                datdevice.Add(i.ToString());
                datdevice.Add(devInfo.Flags.ToString());
                datdevice.Add(devInfo.Type.ToString());
                datdevice.Add(devInfo.ID.ToString());
                datdevice.Add(devInfo.LocId.ToString());
                datdevice.Add(sernum.ToString());
                datdevice.Add(desc.ToString());
                datdevice.Add(devInfo.ftHandle.ToString());
            }

        }

        public static uint FindSpiInterfaceLocId()
        {
            uint devCount = 0;
            FT_CreateDeviceInfoList(out devCount);
            if (devCount == 0)
                throw new InvalidOperationException("No FT4222 devices found.");

            for (uint i = 0; i < devCount; i++)
            {
                uint flags = 0, id = 0, locId = 0;
                FTDI.FT_DEVICE type = 0;
                byte[] sn = new byte[16], desc = new byte[64];
                IntPtr h = IntPtr.Zero;

                FT_GetDeviceInfoDetail(i, ref flags, ref type, ref id, ref locId, sn, desc, ref h);

                if (type == FTDI.FT_DEVICE.FT_DEVICE_4222H_0 ||
                    type == FTDI.FT_DEVICE.FT_DEVICE_4222H_1_2)     // Interface A or B
                    return locId;
            }

            throw new InvalidOperationException("No SPI‑capable FT4222 interface found.");
        }

        public uint GetDeviceLocId(uint dev_id)
        {
            uint devCount = 0;
            Ft4222Native.FT_CreateDeviceInfoList(out devCount);
            if (devCount == 0)
                throw new InvalidOperationException("No FT4222 devices detected.");

            uint flags = 0, id = 0, locId = 0;
            FTDI.FT_DEVICE chip = 0;
            byte[] sn = new byte[16];
            byte[] desc = new byte[64];
            IntPtr dummy = IntPtr.Zero;

            Ft4222Native.FT_GetDeviceInfoDetail(dev_id, ref flags, ref chip, ref id, ref locId, sn, desc, ref dummy);

            return locId;                 // Location‑ID of the first FT4222H bridge
        }

        public string GetFtdiDriverVersion()
        {
            FTDI ftdi = new FTDI();
            FTDI.FT_STATUS status = ftdi.OpenByIndex(0); // or OpenBySerialNumber()

            if (status != FTDI.FT_STATUS.FT_OK)
                return "Failed to open FTDI device";

            uint version = 0;
            status = ftdi.GetDriverVersion(ref version);

            ftdi.Close();

            if (status == FTDI.FT_STATUS.FT_OK)
            {
                byte major = (byte)((version >> 24) & 0xFF);
                byte minor = (byte)((version >> 16) & 0xFF);
                byte build = (byte)((version >> 8) & 0xFF);

                return $"FTD2XX.dll Version: {major}.{minor}.{build}";
            }

            return "Failed to retrieve driver version";
        }

    }    
}
