using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BringUp_Control.PCAL6416A;


namespace BringUp_Control
{

    
    internal sealed class AD9175_DAC : IDisposable
    {
        private SpiDriver _ft;
        private i2cDriver _i2c;
        private PCAL6416A _ioExp;
        private FtdiInterfaceManager _interfaceManager;

        private double DAC_freq;
        

        byte[] DAC_Data;
        

        public enum OperationType
        {
            Read,
            Write,
            Sleep,
            Skip
        }

        public enum FunctionGroup
        {
            POWER_UP,
            DAC_PLL,
            DLL_CONFIG,
            CALIBRATION,
            JESD204,
            CHANNEL_DATAPATH,
            MAINDAC_DATAPATH_DAC0,
            MAINDAC_DATAPATH_DDCM_DAC0,
            MAINDAC_DATAPATH_DAC1,
            MAINDAC_DATAPATH_DDCM_DAC1,
            JESD204_SERDES,
            TRANSPORT_LAYER,
            CLEANUP
            // … add any others you have
        }

        public class Command
        {
            public ushort Address { get; set; }
            public byte Data { get; set; }
            public OperationType OpType { get; set; }
            public FunctionGroup Group { get; set; }
        }        

        private string _csvPath;
        private const byte ErrorValue = 0xFF; // error value from readregister function ???? check it !!!!

        
        DataTable dtAD9175 = new DataTable();

        public void Init(SpiDriver ft, i2cDriver i2c, PCAL6416A ioExp, FtdiInterfaceManager interfaceManager)
        {
            _ft = ft ?? throw new ArgumentNullException(nameof(ft));
            _i2c = i2c ?? throw new ArgumentNullException(nameof(i2c));
            _ioExp = ioExp ?? throw new ArgumentNullException(nameof(ioExp));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));

            // Move this code below to any DAC usage method as needed
            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // Set the IO Expander CTRL_SPI_EN_1V8 to high to enable the FTDI CS
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, true);
            // Set the IO Expander TMUX1104 address pins to 0x00 to allow the FTDI CS to reach the AD9175
            

            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_CSN_SEL0, false);
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_CSN_SEL1, false);

            

            // Now direct CS from FTDI to the AD9175 is enabled and ready for SPI communication

            _ft = _interfaceManager.GetSpi(); // Get current SPI interface
            //... do some SPI communication with the AD7091
        }

        public void SetHwResetState(bool state)
        {
            if (_ioExp == null || _i2c == null || _interfaceManager == null)
            {
                throw new InvalidOperationException("AD9175_DAC not initialized. Call Init() first.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // Set the DAC reset pin state
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_RSTn, state);
            

        }

        public void IO_DAC_IO_Reset()
        {

            if (_ioExp == null || _i2c == null || _interfaceManager == null)
            {
                throw new InvalidOperationException("AD9175_DAC not initialized. Call Init() first.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device

           // _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_RSTn, false);

            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_RSTn, false);
            Thread.Sleep(10);
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_RSTn, true);
            Thread.Sleep(10);

            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_TXEN0, true);
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_TXEN1, true);

            //return to ftdi handle
            _ft = _interfaceManager.GetSpi(); // Get current SPI interface

        }

        public void SetTxEnable0(bool tx0State)
        {
            if (_ioExp == null || _i2c == null || _interfaceManager == null)
            {
                throw new InvalidOperationException("AD9175_DAC not initialized. Call Init() first.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // Set the DAC TX enable pin state
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_TXEN0, tx0State);
        }

        public void SetTxEnable1(bool tx1State)
        {
            if (_ioExp == null || _i2c == null || _interfaceManager == null)
            {
                throw new InvalidOperationException("AD9175_DAC not initialized. Call Init() first.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // Set the DAC TX enable pin state
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_TXEN1, tx1State);
        }

        public void SetTxEnableBoth(bool tx0State, bool tx1State)
        {
            if (_ioExp == null || _i2c == null || _interfaceManager == null)
            {
                throw new InvalidOperationException("AD9175_DAC not initialized. Call Init() first.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            // Set both DAC TX enable pins state
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_TXEN0, tx0State);
            _ioExp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_TXEN1, tx1State);
        }

        public bool GetIrq0State()
        {
            if (_ioExp == null || _i2c == null || _interfaceManager == null)
            {
                throw new InvalidOperationException("AD9175_DAC not initialized. Call Init() first.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            bool irq0 = _ioExp.GetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_IRQn0);

            return irq0; // Return the state of the IRQ0 pin
        }

        public bool GetIrq1State()
        {
            if (_ioExp == null || _i2c == null || _interfaceManager == null)
            {
                throw new InvalidOperationException("AD9175_DAC not initialized. Call Init() first.");
            }

            _i2c = _interfaceManager.GetI2c(); // Get current I2C interface
            _ioExp.Init(_i2c); // Re-initialize IO Expander with the current I2C device
            bool irq1 = _ioExp.GetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_DAC_IRQn1);

            return irq1; // Return the state of the IRQ1 pin
        }

        public void DAC9175_InitEngine(string csvPath)
        {
            _csvPath = csvPath;
        }


        public int RUN_CSV()
        {
            if (!File.Exists(_csvPath))
                throw new FileNotFoundException($"Initialization file not found: {_csvPath}");


            var commands = LoadCommands(_csvPath);

            foreach (var cmd in commands)
            {
                switch (cmd.Group)
                {

                    case FunctionGroup.POWER_UP:
                        if (ProcessPowerUp(cmd) != 0) return -1; // If PowerUp fails, return -1
                        break;
                    case FunctionGroup.DAC_PLL:
                        if (ProcessDacPll(cmd) != 0) return -1; // If DAC_PLL fails, return -1
                        break;
                    case FunctionGroup.DLL_CONFIG:
                        if (ProcessDllConfig(cmd) != 0) return -1; // If DLL_CONFIG fails, return -1
                        break;
                    case FunctionGroup.CALIBRATION:
                        if (ProcessCalibration(cmd) != 0) return -1; // If CALIBRATION fails, return -1 
                        break;
                    case FunctionGroup.JESD204:
                        if (ProcessJESD204(cmd) != 0) return -1; // If JESD204 fails, return -1
                        break;
                    case FunctionGroup.CHANNEL_DATAPATH:
                        break;
                    case FunctionGroup.MAINDAC_DATAPATH_DAC0:
                        if (ProcessMainDacDatapathDac0(cmd) != 0) return -1; // If MAINDAC_DATAPATH_DAC0 fails, return -1
                        break;
                    case FunctionGroup.MAINDAC_DATAPATH_DDCM_DAC0:
                        if (ProcessMainDac_DDCM_DAC0(cmd) != 0) return -1; // If MAINDAC_DATAPATH_DDCM_DAC0  fails, return -1
                        break;
                    case FunctionGroup.MAINDAC_DATAPATH_DAC1:
                        if (ProcessMainDacDatapathDac1(cmd) != 0) return -1; // If MAINDAC_DATAPATH_DAC1 fails, return -1
                        break;
                    case FunctionGroup.MAINDAC_DATAPATH_DDCM_DAC1:
                        if (ProcessMainDac_DDCM_DAC1(cmd) != 0) return -1; // If MAINDAC_DATAPATH_DDCM_DAC1  fails, return -1
                        break;
                    case FunctionGroup.JESD204_SERDES:
                        if (ProcessJESD204Serdes(cmd) != 0) return -1;
                        break;
                    case FunctionGroup.TRANSPORT_LAYER:
                        if (ProcessTransportLayer(cmd) != 0) return -1; // If TRANSPORT_LAYER fails, return -1
                        break;
                    case FunctionGroup.CLEANUP:
                        if (ProcessCleanup(cmd) != 0) return -1; // If CLEANUP fails, return -1
                        break;
                    default:
                        //throw new InvalidOperationException($"Unknown function group: {cmd.Group}");
                        return -99;

                }
            }
            return 0; // Return 0 if all commands processed successfully

        }

        private List<Command> LoadCommands(string path)
        {
            var list = new List<Command>();
            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < 4) continue;

                var addrHex = parts[0].Trim();
                var dataHex = parts[1].Trim();
                var opCode = parts[2].Trim().ToUpperInvariant();
                var groupName = parts[3].Trim();

                if (!Enum.TryParse<FunctionGroup>(groupName, out var group))
                    throw new InvalidDataException($"Unknown function group '{groupName}'");

                OperationType opType;
                switch (opCode)
                {
                    case "W":
                        opType = OperationType.Write;
                        break;
                    case "R":
                        opType = OperationType.Read;
                        break;
                    case "SLEEP":
                        opType = OperationType.Sleep;
                        break;
                    case "SKIP":
                        opType = OperationType.Skip;
                        break;
                    default:
                        throw new InvalidDataException($"Unknown operation '{opCode}'");
                }

                var cmd = new Command
                {
                    Address = HexStringToUshort(addrHex),
                    Data = byte.Parse(dataHex.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? dataHex.Substring(2) : dataHex, NumberStyles.HexNumber),
                    OpType = opType,
                    Group = group
                };
                list.Add(cmd);
            }
            return list;
        }

        #region Group Processors

        //POWER_UP
        private int ProcessPowerUp(Command cmd) => ProcessGeneric(cmd);
        //DAC_PLL
        private int ProcessDacPll(Command cmd) => ProcessGeneric(cmd);
        //DLL_CONFIG
        private int ProcessDllConfig(Command cmd) => ProcessGeneric(cmd);
        //CALLIBRATION
        private int ProcessCalibration(Command cmd) => ProcessGeneric(cmd);
        //JESD204
        private int ProcessJESD204(Command cmd) => ProcessGeneric(cmd);
        //MAINDATAPATH_DAC0
        private int ProcessMainDacDatapathDac0(Command cmd) => ProcessGeneric(cmd);
        //MAINDATAPATH_DAC1 
        private int ProcessMainDacDatapathDac1(Command cmd) => ProcessGeneric(cmd);
        //MAINDAC_DATAPATH_DDCM_DAC0
        private int ProcessMainDac_DDCM_DAC0(Command cmd) => ProcessSpecific(cmd);
        //MAINDAC_DATAPATH_DDCM_DAC1
        private int ProcessMainDac_DDCM_DAC1(Command cmd) => ProcessSpecific(cmd);
        //TRANSPORT_LAYER
        private int ProcessTransportLayer(Command cmd) => ProcessGeneric(cmd);
        //JESD204_SERDES
        private int ProcessJESD204Serdes(Command cmd) => ProcessGeneric(cmd);
        //CLEANUP
        private int ProcessCleanup(Command cmd) => ProcessGeneric(cmd);
        private int ProcessGeneric(Command cmd)
        {
            switch (cmd.OpType)
            {
                case OperationType.Write:
                    WriteRegister(cmd.Address, cmd.Data);
                    break;

                case OperationType.Read:
                    //Thread.Sleep(5000); // Delay to ensure the device is ready for reading
                    var result = ReadRegister(cmd.Address);
                    result &= 0x01;
                    if (result != cmd.Data)
                    {
                        Console.Error.WriteLine(
                            $"Error: Read value 0x{result:X2} at address 0x{cmd.Address:X4} triggers stop.");

                        MainForm.Instance.LogStatus($"DAC INIT FAILED!");
                        return -1;
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Read value 0x{result:X2} at address 0x{cmd.Address:X4} - Read Function.");
                    }
                    break;

                case OperationType.Sleep:
                    int timeval = cmd.Data * 100; // LSB = 100 mSec
                    Console.WriteLine($"Sleeping for {timeval} millisecond(s)...");
                    Thread.Sleep(timeval); //milliseconds
                    break;

                case OperationType.Skip:
                    // do nothing
                    break;
            }
            return 0;
        }

        private int ProcessSpecific(Command cmd)
        {
            if ((int)cmd.Data == 0) return -1;

            DAC_freq = (int)cmd.Data * 1e9; 
            DAC_Data = GetBytes48BitBigEndian(CalculateDdsmFtw(DAC_freq));   //2  or 3 GHz
            
            switch (cmd.OpType)
            {
                case OperationType.Write:
                    MainDAC_DDCM_Setup(DAC_Data);
                    break;

                case OperationType.Skip:
                    // do nothing
                    break;
            }
            return 0;            
        }

        #endregion  

        //Table 50 : Power Up registee writing     

        // Table 51: DAC and PLL configuration sequence        
        
        // Table 52: Delay lock loop configuration sequence        

        // parse bit position value: value - byte of data, bitIndex - position of the bit that must be tested
        public bool GetBit(byte value, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 7)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), "Bit index must be between 0 and 7");

            return (value & (1 << bitIndex)) != 0;
        }

        // Table 53: Calibration sequence        
        
        // Table 54: JESD204B configuration sequence        

        // Table 55: Channel datapath configuration sequence        

        // Table 56: Main datapath and Main NCO configuration sequence - TEST IT!!!
        public void MainDAC_DDCM_Setup(byte[] DDCM_DAC0)
        {
            for (ushort i = 0x0114; i <= 0x0119; i++)
            {
                WriteRegister(i, DDCM_DAC0[0x0119-i]);               
                
            }           
        }

        // Table 57 : JESD204B SERDES configuration sequence        

        //Table 58: Transport layer configuration sequence        

        // Table 59: Register cleanup sequence        

        public List<string> LoadComboRegister9175()
        {
            List<string> regaddresslist9175 = new List<string>();
            for (int i = 0x0000; i <= 0x07B5; i++)
            {
                if (i == 0x0007 || i == 0x0009 || i == 0x0092 || i == 0x0093)
                    continue;

                if (i >= 0x000B && i <= 0x000F)
                    continue;

                if (i >= 0x0014 && i <= 0x0019)
                    continue;

                if (i >= 0x002D && i <= 0x0035)
                    continue;

                if (i >= 0x0037 && i <= 0x0038)
                    continue;

                if (i >= 0x003C && i <= 0x003E)
                    continue;

                if (i >= 0x0040 && i <= 0x004F)
                    continue;

                if (i >= 0x0053 && i <= 0x0059)
                    continue;

                if (i >= 0x005B && i <= 0x0060)
                    continue;

                if (i >= 0x0062 && i <= 0x0080)
                    continue;

                if (i == 0x0082 || i == 0x008E || i == 0x00C2 || i == 0x00C4 || i == 0x011A || i == 0x011B || i == 0x014A || i == 0x014F || i == 0x0150)
                    continue;

                if (i >= 0x0086 && i <= 0x008C)
                    continue;

                if (i >= 0x0096 && i <= 0x0099)
                    continue;

                if (i >= 0x009B && i <= 0x00BF)
                    continue;

                if (i >= 0x00C8 && i <= 0x00CB)
                    continue;

                if (i >= 0x00CE && i <= 0x00DA)
                    continue;

                if (i >= 0x00DC && i <= 0x00FE)
                    continue;

                if (i >= 0x0101 && i <= 0x010F)
                    continue;

                if (i >= 0x011F && i <= 0x0123)
                    continue;

                if (i >= 0x0152 && i <= 0x01DD)
                    continue;

                if (i >= 0x01DF && i <= 0x01E1)
                    continue;

                if (i >= 0x01E8 && i <= 0x01FF)
                    continue;

                if (i == 0x0202 || i == 0x0204 || i ==0x0205 || i == 0x0211 || i == 0x0214 || i == 0x0215 || i == 0x0252 || i == 0x0301)

                if (i >= 0x0217 && i <= 0x0233)
                    continue;

                if (i >= 0x0235 && i <= 0x023F)
                    continue;

                if (i >= 0x024C && i <= 0x027F)
                    continue;

                if (i >= 0x0282 && i <= 0x02FF)
                    continue;

                if (i >= 0x030E && i <= 0x0310)
                    continue;

                if (i >= 0x0313 && i <= 0x0314)
                    continue;

                if (i >= 0x0324 && i <= 0x032B)
                    continue;

                if (i >= 0x0330 && i <= 0x0333)
                    continue;

                if (i >= 0x0335 && i <= 0x03FF)
                    continue;

                if (i >= 0x040F && i <= 0x0411)
                    continue;

                if (i == 0x0413 || i == 0x0414 || i == 0x041B || i == 0x041C || i == 0x0423 || i == 0x0424 || i == 0x0433 || i == 0x0434 || i == 0x043B || i == 0x043C)
                    continue;

                if (i >= 0x0427 && i <= 0x0429)
                    continue;

                if (i >= 0x042B && i <= 0x042C)
                    continue;

                if (i >= 0x042F && i <= 0x0431)
                    continue;

                if (i >= 0x0437 && i <= 0x0439)
                    continue;

                if (i >= 0x043F && i <= 0x0441)
                    continue;

                if (i == 0x0443 || i == 0x0444 || i == 0x07A1)
                    continue;

                if (i >= 0x0447 && i <= 0x044F)
                    continue;

                if (i >= 0x045E && i <= 0x046B)
                    continue;

                if (i >= 0x047E && i <= 0x0479)
                    continue;

                if (i >= 0x0498 && i <= 0x04A9)
                    continue;

                if (i >= 0x04BD && i <= 0x057F)
                    continue;

                if (i >= 0x058E && i <= 0x0595)
                    continue;

                if (i >= 0x059A && i <= 0x0704)
                    continue;

                if (i >= 0x0706 && i <= 0x078F)
                    continue;

                if (i >= 0x07A3 && i <= 0x07B4)
                    continue;
                
                regaddresslist9175.Add($"0x{i:X4}"); // Format as hexadecimal with 4 digits
            }

            return regaddresslist9175;
        }

        public void Read_Dump(List<string> registers, string filepath)
        {
            //create datatable with 2 columns register, value
            DataTable regDump = new DataTable();
            regDump.Columns.Add("Register", typeof(string));
            regDump.Columns.Add("Value", typeof(string));
            regDump.Columns.Add("Value byte", typeof(byte));

            //read all registers from the list
            foreach (var reg in registers)
            {
                if (reg.StartsWith("0x") &&
                    ushort.TryParse(reg.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out ushort regAddr))
                {
                    byte val = ReadRegister(regAddr);  // <-- Replace this with your actual SPI read method
                    regDump.Rows.Add(reg, $"0x{val:X2}", val);
                    
                }
                //Thread.Sleep(10); // Delay to ensure the device is ready for next reading
            }

            var sb = new StringBuilder();

            // Write header
            string[] columnNames = regDump.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            // Write rows
            foreach (DataRow row in regDump.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filepath, sb.ToString());
        }


        // DAC_Full scale measurement and output value calculation
        public int DAC_FullScale(int dac_index, float IOUTFS_mA)
        {
            int FSC_Ctrl = 0;
            if (IOUTFS_mA > 15.625 && IOUTFS_mA < 25.977)
            {
                uint DAC_FS_Value = (uint)(1 << (6 + dac_index));
                WriteRegister(0x0008, (byte)DAC_FS_Value);

                FSC_Ctrl = Convert.ToInt16((IOUTFS_mA - 15.625) * 256 / 25);

                WriteRegister(0x005A, (byte)FSC_Ctrl);
            }
            else
            {                
                FSC_Ctrl = -1; // Invalid value
                MainForm.Instance?.LogStatus("IOUTFS_mA must be between 15.625 and 25.977 mA");
            }
            
            return FSC_Ctrl;
        }
        
        public void DAC_DT_Clear()        {
            
            dtAD9175.Clear();
        }

        public DataTable InitDataTableDAC()
        {
            
            dtAD9175.Columns.Add("Index", typeof(int));
            dtAD9175.Columns.Add("Register", typeof(string));
            dtAD9175.Columns.Add("Value", typeof(string));
            dtAD9175.Columns.Add("Value byte", typeof(byte));

            return dtAD9175;
        }

        public void ReadAllRegisters()
        {
            for (int i = 0; i < dtAD9175.Rows.Count; i++)
            {
                ReadRegisterAndUpdate(i);
            }
        }

        public void ReadRegisterAndUpdate(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dtAD9175.Rows.Count)
                return;

            var row = dtAD9175.Rows[rowIndex];
            string regHex = row["Register"].ToString();

            if (regHex.StartsWith("0x") &&
                ushort.TryParse(regHex.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out ushort reg))
            {
                byte val = ReadRegister(reg);  // <-- Replace this with your actual SPI read method
                row["Value"] = $"0x{val:X2}";
                row["Value byte"] = val;
            }
        }



        public void LoadRegisterFile(string filePath)
        {
            dtAD9175.Clear(); // Optional: clear if reloading

            var lines = File.ReadAllLines(filePath);
            int index = 0;
            try
            {
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2 &&
                        ushort.TryParse(parts[0].Trim().Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out ushort reg) &&
                        byte.TryParse(parts[1].Trim(), out byte val))
                    {
                        dtAD9175.Rows.Add(index++, $"0x{reg:X4}", $"0x{val:X2}", val);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }


        public void WriteRegister(ushort address, byte data)
        {
            if (_ft == null) return;
            byte[] buffer = new byte[]
            {
                (byte)(address >> 8),
                (byte)(address & 0xFF),
                data
            };
            _ft.Write(buffer);
            
        }
        
        public byte ReadRegister(ushort address)
        {
            if (_ft == null) return 0;
            
            ushort cmd = (ushort)(address | 0x8000);
            byte[] tx = new byte[]
            {
                (byte)(cmd >> 8),
                (byte)(cmd & 0xFF),
                0x00
            };
            byte[] rx = new byte[3];
            _ft.TransferFullDuplex(tx, rx);
            return rx[2];
        }
        // Method to calculate the DDSM FTW value based on the given frequency ratio
        public ulong CalculateDdsmFtw(double numeratorHz)
        {
            double fraction = numeratorHz / 11.7e9;
            double result = fraction * Math.Pow(2, 48);
            return (ulong)Math.Round(result); // Round to nearest integer
        }

        // Method to convert a 48-bit unsigned integer to a  6-byte array in big-endian format
        public byte[] GetBytes48BitBigEndian(ulong value)
        {
            byte[] full = BitConverter.GetBytes(value); // Little-endian on most PCs
            Array.Reverse(full); // Make it big-endian
            return full.Skip(2).ToArray(); // Take the last 6 bytes
        }
        public void SaveDataTableToCsv(DataTable table)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = Directory.GetCurrentDirectory();
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.Title = "Save CSV File";
                saveDialog.FileName = "register_dump.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder csvContent = new StringBuilder();

                        // Optional: Add header row
                        // csvContent.AppendLine("Address,Value,Label");

                        foreach (DataRow row in table.Rows)
                        {
                            string regStr = row["Register"].ToString(); // e.g. "0x0023"
                            string valStr = row["Value"].ToString();    // e.g. "0x0F"

                            int regInt = Convert.ToUInt16(regStr.Substring(2), 16); // Convert "0x0023" to int
                            string regHexFull = $"0x{regInt:X8}"; // Format to 8-digit hex

                            // You can dynamically name or hardcode the label
                            string label = "RegMap1";

                            csvContent.AppendLine($"{regHexFull},{valStr},{label}");
                        }

                        File.WriteAllText(saveDialog.FileName, csvContent.ToString());
                        MessageBox.Show("CSV exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving CSV: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public string LoadDataTableToCsv()
        {
            string filepath = string.Empty;
            using (OpenFileDialog ftfile = new OpenFileDialog())
            {
                try
                {
                    ftfile.InitialDirectory = Directory.GetCurrentDirectory();
                    ftfile.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*";
                    ftfile.FilterIndex = 0;

                    if (ftfile.ShowDialog() == DialogResult.OK)
                    {
                        if (dtAD9175.Rows.Count != 0)
                            dtAD9175.Clear();

                        filepath = ftfile.FileName;

                        ParsingFile(filepath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning");

                }

                return filepath;
            }
        }

        public void ParsingFile(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    int index = 1;
                    foreach (var line in File.ReadLines(file))
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] parts = line.Split(',');

                        int value = Convert.ToInt32(parts[0].Trim().Substring(2), 16);

                        if (parts.Length == 3)
                        {
                            dtAD9175.Rows.Add(index++.ToString(), $"0x{value:X4}", parts[1].Trim(), Convert.ToByte(parts[1].Trim(), 16));
                        }
                    }
                    MessageBox.Show("DAC9175 Register Memory Map Loaded!", "Info");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning");
                }
            }
        }

        // TODO - QA tests
        public void PRBS_Test(string PRBS_Type)
        {
            byte prbs_byte;
            if (PRBS_Type.Equals("PRBS7"))
                prbs_byte = 0x00;
            else if (PRBS_Type.Equals("PRBS15"))
                prbs_byte = 0x01;
            else
                prbs_byte = 0x02;


            double PRBS_LaneRate = 14.625e9;
            float PRBS_TestTime = 1.0f; //1 second
            byte PRBS_Lanes_Bits = 0xFF; // All lanes enabled

            WriteRegister(0x0315, PRBS_Lanes_Bits); // select all lines to be tested  (each bit = lane (0 to 7))
            
            byte PRBS_Err_Threshold = 0x00; // Error threshold for PRBS test
            
            WriteRegister(0x317, (byte)((PRBS_Err_Threshold >> 0) & 0xFF));
            WriteRegister(0x318, (byte)((PRBS_Err_Threshold >> 8) & 0xFF));
            WriteRegister(0x319, (byte)((PRBS_Err_Threshold >> 16) & 0xFF));

            

            byte val_0x316 = (byte)(prbs_byte << 2); // Set PRBS type and enable PRBS test

            WriteRegister(0x0316, (byte)(val_0x316 | 0x0));
            WriteRegister(0x0316, (byte)(val_0x316 | 0x1)); 
            WriteRegister(0x0316, (byte)(val_0x316 | 0x0)); 

            WriteRegister(0x0316, (byte)(val_0x316 | 0x2)); // Start PRBS test

            Thread.Sleep((int)(PRBS_TestTime * 1000)); // Wait for the test duration 1 seconf

            WriteRegister(0x0316, (byte)(val_0x316 | 0x0)); // Stop PRBS test Bit 1 must 0.

            byte prbs_status = ReadRegister(0x031D); // Read PRBS status register

            MainForm.Instance?.LogStatus($"PRBS Test Status: 0x{prbs_status:X2}");


            for(int i = 0; i < 8; i++)
            {
                WriteRegister(0x0316, (byte)(val_0x316 | i << 4)); // Enable each lane for PRBS test
                int prbs_error_count = 0;
                ushort start_address = 0x031A;
                for (int j = 0; j < 3; j++)
                {                   
                    prbs_error_count |= (ReadRegister(start_address) & 0xFF) << (8*j); // Read error count for each lane
                    start_address = (ushort)(start_address + j);
                }

                double ber = prbs_error_count / PRBS_LaneRate / PRBS_TestTime; // Calculate Bit Error Rate (BER)

                MainForm.Instance?.LogStatus($"Lane = {i}, ErrCount = {prbs_error_count}, BER = {ber:E}");  //BE is bps
            }
        }

        // TODO - QA tests
        public void Calibration_NCO(int dac_index, float NCO_Freq, int ToneAmp_PR)
        {

            // MAIN DAC PAGE for DAC0 or DAC1
            WriteRegister(0x0008, (byte)(1<<(6+dac_index))); // Optimized calibration setting register

            int DDSM_CAL_FTW = (int)(1.0*(NCO_Freq / 11700) * Math.Pow(2, 32)); // Enabling Calibration NCO accumulator by setting Bit 2 to 1

            
            WriteRegister(0x01E6, 1 << 2); // 

            for(int i = 0; i < 4; i++)
            {
                WriteRegister((ushort)(0x01E2 + i), (byte)((DDSM_CAL_FTW >> (8 * i)) & 0xFF)); // Write Registers
            }
            
            
            WriteRegister(0x0113, 0x00); // Toggling register to update NCO phase and FTW words
            WriteRegister(0x0113, 0x01); // Toggling register to update NCO phase and FTW words

            WriteRegister(0x01E6, 0x07); // Disabling Calibration NCO accumulator by setting Bit 1 to 1

            WriteRegister(0x0008, (byte)(1 << (0 + dac_index))); // Disabling Calibration NCO accumulator by setting Bit 1 to 0

            int ToneValue = (int)(0x50FF *1.0 * ToneAmp_PR / 100.0); // Calculate the tone value based on the percentage
            for (int i = 0; i < 2; i++)
            {
                WriteRegister((ushort)(0x0148 + i), (byte)((ToneValue >> (8 * i)) & 0xFF)); // Write Tone Value to DDSM_ACC_DELTA[7:0] to DDSM_ACC_DELTA[15:8]
            }

            MainForm.Instance?.LogStatus($"NCO Calib DDSM_CAL:{DDSM_CAL_FTW}, Tone Value: {ToneValue} ");
        }

        public void Dispose()
        {
            //_ft?.Dispose();
            _ft = null; // Release the FTDI device
        }

        public static ushort HexStringToUshort(string hex)
        {
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);

            return Convert.ToUInt16(hex, 16);
        }
    }
}
