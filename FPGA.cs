using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BringUp_Control
{
    internal class FPGA : IDisposable
    {

        private SpiDriver _ft;
        private FtdiInterfaceManager _interfaceManager;
        private const uint BASE_ADDRESS = 0x00001000;

        DataTable dtFPGA = new DataTable();

        private const int BC = 12; // Bit count for I/Q components
        private const int MaxRetries = 3;

        public static readonly Dictionary<string, uint> RegisterMapTOP_Container = new Dictionary<string, uint>()
        {
            { "container_version", 0x0 },
            { "dsp_cfg_ul_i0_nco_delta_phase", 0x4 },
            { "dsp_cfg_ul_i0_gain", 0x8 },
            { "dsp_cfg_ul_i1_nco_delta_phase", 0xC },
            { "dsp_cfg_ul_i1_gain", 0x10 },
            { "dsp_cfg_ul_i2_nco_delta_phase", 0x14 },
            { "dsp_cfg_ul_i2_gain", 0x18 },
            { "dsp_cfg_ul_i3_nco_delta_phase", 0x1C },
            { "dsp_cfg_ul_i3_gain", 0x20 },
            { "dsp_cfg_dl_i0_nco_delta_phase", 0x24 },
            { "dsp_cfg_dl_i0_gain", 0x28 },
            { "dsp_cfg_dl_i1_nco_delta_phase", 0x2C },
            { "dsp_cfg_dl_i1_gain", 0x30 },
            { "dsp_cfg_dl_i2_nco_delta_phase", 0x34 },
            { "dsp_cfg_dl_i2_gain", 0x38 },
            { "dsp_cfg_dl_i3_nco_delta_phase", 0x3C },
            { "dsp_cfg_dl_i3_gain", 0x40 },
            { "dsp_ul_pwr_measure_ctrl_start", 0x44 },
            { "dsp_ul_pwr_measure_ctrl_log2_sample_count_added_to_min", 0x48},
            { "dsp_ul_pwr_measure_ctrl_power_sat_msb", 0x4C},
            { "dsp_ul_pwr_measure_status_i0_valid", 0x50},
            { "dsp_ul_pwr_measure_status_i0_average_power", 0x54},
            { "dsp_ul_pwr_measure_status_i0_power_sat_count", 0x58},
            { "dsp_ul_pwr_measure_status_i1_valid", 0x5C},
            { "dsp_ul_pwr_measure_status_i1_average_power", 0x60},
            { "dsp_ul_pwr_measure_status_i1_power_sat_count", 0x64},
            { "dsp_ul_pwr_measure_status_i2_valid", 0x68 },
            { "dsp_ul_pwr_measure_status_i2_average_power", 0x6C},
            { "dsp_ul_pwr_measure_status_i2_power_sat_count", 0x70},
            { "dsp_ul_pwr_measure_status_i3_valid", 0x74},
            { "dsp_ul_pwr_measure_status_i3_average_power", 0x78},
            { "dsp_ul_pwr_measure_status_i3_power_sat_count", 0x7C},
            { "dsp_dl_pwr_measure_ctrl_start", 0x80},
            { "dsp_dl_pwr_measure_ctrl_log2_sample_count_added_to_min", 0x84},
            { "dsp_dl_pwr_measure_ctrl_power_sat_msb", 0x88},
            { "dsp_dl_pwr_measure_status_i0_valid", 0x8C},
            { "dsp_dl_pwr_measure_status_i0_average_power", 0x90 },
            { "dsp_dl_pwr_measure_status_i0_power_sat_count", 0x94},
            { "dsp_dl_pwr_measure_status_i1_valid", 0x98},
            { "dsp_dl_pwr_measure_status_i1_average_power", 0x9C},
            { "dsp_dl_pwr_measure_status_i1_power_sat_count", 0xA0},
            { "dsp_dl_pwr_measure_status_i2_valid", 0xA4},
            { "dsp_dl_pwr_measure_status_i2_average_power", 0xA8},
            { "dsp_dl_pwr_measure_status_i2_power_sat_count", 0xAC},
            { "dsp_dl_pwr_measure_status_i3_valid", 0xB0},
            { "dsp_dl_pwr_measure_status_i3_average_power", 0xB4},
            { "dsp_dl_pwr_measure_status_i3_power_sat_count", 0xB8},
            { "debugger_i0_hw_channel_count", 0xBC},
            { "debugger_i0_hw_samples_per_channel",0xC0},
            { "debugger_i0_hw_memory_depth", 0xC4},
            { "debugger_i0_rgf_set_active_channel", 0xC8},
            { "debugger_i0_rgf_set_wr_address", 0xCC},
            { "debugger_i0_rgf_set_rd_address", 0xD0},
            { "debugger_i0_rgf_get_wr_address", 0xD4},
            { "debugger_i0_rgf_get_rd_address", 0xD8},
            { "debugger_i0_rgf_get_wr_pointer", 0xDC},
            { "debugger_i0_rgf_get_rd_pointer", 0xE0},
            { "debugger_i0_rgf_wr_sample", 0xE4 },
            { "debugger_i0_rgf_rd_sample", 0xE8},
            { "debugger_i0_rgf_start_recording", 0xEC},
            { "debugger_i0_rgf_activate_player", 0xF0},
            { "debugger_i0_rgf_player_count", 0xF4},
            { "debugger_i0_rgf_mode_status", 0xF8},
            { "debugger_i1_hw_channel_count", 0xFC},
            { "debugger_i1_hw_samples_per_channel", 0x100},
            { "debugger_i1_hw_memory_depth", 0x104},
            { "debugger_i1_rgf_set_active_channel", 0x108},
            { "debugger_i1_rgf_set_wr_address", 0x10C},
            { "debugger_i1_rgf_set_rd_address", 0x110},
            { "debugger_i1_rgf_get_wr_address", 0x114},
            { "debugger_i1_rgf_get_rd_address", 0x118},
            { "debugger_i1_rgf_get_wr_pointer", 0x11C},
            { "debugger_i1_rgf_get_rd_pointer", 0x120},
            { "debugger_i1_rgf_wr_sample", 0x124},
            { "debugger_i1_rgf_rd_sample", 0x128},
            { "debugger_i1_rgf_start_recording", 0x12C},
            { "debugger_i1_rgf_activate_player", 0x130},
            { "debugger_i1_rgf_player_count", 0x134},
            { "debugger_i1_rgf_mode_status", 0x138},
            { "debugger_i2_hw_channel_count", 0x13C},
            { "debugger_i2_hw_samples_per_channel", 0x140},
            { "debugger_i2_hw_memory_depth", 0x144},
            { "debugger_i2_rgf_set_active_channel", 0x148},
            { "debugger_i2_rgf_set_wr_address", 0x14C},
            { "debugger_i2_rgf_set_rd_address", 0x150},
            { "debugger_i2_rgf_get_wr_address", 0x154},
            { "debugger_i2_rgf_get_rd_address", 0x158},
            { "debugger_i2_rgf_get_wr_pointer", 0x15C},
            { "debugger_i2_rgf_get_rd_pointer", 0x160},
            { "debugger_i2_rgf_wr_sample", 0x164},
            { "debugger_i2_rgf_rd_sample", 0x168},
            { "debugger_i2_rgf_start_recording", 0x16C},
            { "debugger_i2_rgf_activate_player", 0x170},
            { "debugger_i2_rgf_player_count", 0x174},
            { "debugger_i2_rgf_mode_status", 0x178},
            { "debugger_i3_hw_channel_count", 0x17C},
            { "debugger_i3_hw_samples_per_channel", 0x180},
            { "debugger_i3_hw_memory_depth", 0x184},
            { "debugger_i3_rgf_set_active_channel", 0x188},
            { "debugger_i3_rgf_set_wr_address", 0x18C},
            { "debugger_i3_rgf_set_rd_address", 0x190},
            { "debugger_i3_rgf_get_wr_address", 0x194},
            { "debugger_i3_rgf_get_rd_address", 0x198},
            { "debugger_i3_rgf_get_wr_pointer", 0x19C},
            { "debugger_i3_rgf_get_rd_pointer", 0x1A0},
            { "debugger_i3_rgf_wr_sample", 0x1A4},
            { "debugger_i3_rgf_rd_sample", 0x1A8},
            { "debugger_i3_rgf_start_recording", 0x1AC},
            { "debugger_i3_rgf_activate_player", 0x1B0},
            { "debugger_i3_rgf_player_count", 0x1B4},
            { "debugger_i3_rgf_mode_status", 0x1B8},
            { "enable_data_path", 0x1BC},
            { "activate_loopback", 0x1C0 },
            { "end", 0x1C4 }
        };
        /*public static readonly Dictionary<string, uint> RegisterMapCoin_Digital = new Dictionary<string, uint>()
        {
            { "container_version", 0x0 },
            { "dsp_cfg_ul_i0_nco_delta_phase", 0x4 },
            { "dsp_cfg_ul_i0_gain", 0x8 },
            { "dsp_cfg_ul_i1_nco_delta_phase", 0xC },
            { "dsp_cfg_ul_i1_gain", 0x10 },
            { "dsp_cfg_ul_i2_nco_delta_phase", 0x14 },
            { "dsp_cfg_ul_i2_gain", 0x18 },
            { "dsp_cfg_ul_i3_nco_delta_phase", 0x1C },
            { "dsp_cfg_ul_i3_gain", 0x20 },
            { "dsp_cfg_dl_i0_nco_delta_phase", 0x24 },
            { "dsp_cfg_dl_i0_gain", 0x28 },
            { "dsp_cfg_dl_i1_nco_delta_phase", 0x2C },
            { "dsp_cfg_dl_i1_gain", 0x30 },
            { "dsp_cfg_dl_i2_nco_delta_phase", 0x34 },
            { "dsp_cfg_dl_i2_gain", 0x38 },
            { "dsp_cfg_dl_i3_nco_delta_phase", 0x3C },
            { "dsp_cfg_dl_i3_gain", 0x40 },
            { "dsp_ul_pwr_measure_ctrl_start", 0x44 },
            { "dsp_ul_pwr_measure_ctrl_log2_sample_count_added_to_min", 0x48},
            { "dsp_ul_pwr_measure_ctrl_power_sat_msb", 0x4C},
            { "dsp_ul_pwr_measure_status_i0_valid", 0x50},
            { "dsp_ul_pwr_measure_status_i0_average_power", 0x54},
            { "dsp_ul_pwr_measure_status_i0_power_sat_count", 0x58},
            { "dsp_ul_pwr_measure_status_i1_valid", 0x5C},
            { "dsp_ul_pwr_measure_status_i1_average_power", 0x60},
            { "dsp_ul_pwr_measure_status_i1_power_sat_count", 0x64},
            { "dsp_ul_pwr_measure_status_i2_valid", 0x68 },
            { "dsp_ul_pwr_measure_status_i2_average_power", 0x6C},
            { "dsp_ul_pwr_measure_status_i2_power_sat_count", 0x70},
            { "dsp_ul_pwr_measure_status_i3_valid", 0x74},
            { "dsp_ul_pwr_measure_status_i3_average_power", 0x78},
            { "dsp_ul_pwr_measure_status_i3_power_sat_count", 0x7C},
            { "dsp_dl_pwr_measure_ctrl_start", 0x80},
            { "dsp_dl_pwr_measure_ctrl_log2_sample_count_added_to_min", 0x84},
            { "dsp_dl_pwr_measure_ctrl_power_sat_msb", 0x88},
            { "dsp_dl_pwr_measure_status_i0_valid", 0x8C},
            { "dsp_dl_pwr_measure_status_i0_average_power", 0x90 },
            { "dsp_dl_pwr_measure_status_i0_power_sat_count", 0x94},
            { "dsp_dl_pwr_measure_status_i1_valid", 0x98},
            { "dsp_dl_pwr_measure_status_i1_average_power", 0x9C},
            { "dsp_dl_pwr_measure_status_i1_power_sat_count", 0xA0},
            { "dsp_dl_pwr_measure_status_i2_valid", 0xA4},
            { "dsp_dl_pwr_measure_status_i2_average_power", 0xA8},
            { "dsp_dl_pwr_measure_status_i2_power_sat_count", 0xAC},
            { "dsp_dl_pwr_measure_status_i3_valid", 0xB0},
            { "dsp_dl_pwr_measure_status_i3_average_power", 0xB4},
            { "dsp_dl_pwr_measure_status_i3_power_sat_count", 0xB8},
            { "debugger_i0_hw_channel_count", 0xBC},
            { "debugger_i0_hw_samples_per_channel",0xC0},
            { "debugger_i0_hw_memory_depth", 0xC4},
            { "debugger_i0_rgf_set_active_channel", 0xC8},
            { "debugger_i0_rgf_set_wr_address", 0xCC},
            { "debugger_i0_rgf_set_rd_address", 0xD0},
            { "debugger_i0_rgf_get_wr_address", 0xD4},
            { "debugger_i0_rgf_get_rd_address", 0xD8},
            { "debugger_i0_rgf_get_wr_pointer", 0xDC},
            { "debugger_i0_rgf_get_rd_pointer", 0xE0},
            { "debugger_i0_rgf_wr_sample", 0xE4 },
            { "debugger_i0_rgf_rd_sample", 0xE8},
            { "debugger_i0_rgf_start_recording", 0xEC},
            { "debugger_i0_rgf_activate_player", 0xF0},
            { "debugger_i0_rgf_player_count", 0xF4},
            { "debugger_i0_rgf_mode_status", 0xF8},
            { "debugger_i1_hw_channel_count", 0xFC},
            { "debugger_i1_hw_samples_per_channel", 0x100},
            { "debugger_i1_hw_memory_depth", 0x104},
            { "debugger_i1_rgf_set_active_channel", 0x108},
            { "debugger_i1_rgf_set_wr_address", 0x10C},
            { "debugger_i1_rgf_set_rd_address", 0x110},
            { "debugger_i1_rgf_get_wr_address", 0x114},
            { "debugger_i1_rgf_get_rd_address", 0x118},
            { "debugger_i1_rgf_get_wr_pointer", 0x11C},
            { "debugger_i1_rgf_get_rd_pointer", 0x120},
            { "debugger_i1_rgf_wr_sample", 0x124},
            { "debugger_i1_rgf_rd_sample", 0x128},
            { "debugger_i1_rgf_start_recording", 0x12C},
            { "debugger_i1_rgf_activate_player", 0x130},
            { "debugger_i1_rgf_player_count", 0x134},
            { "debugger_i1_rgf_mode_status", 0x138},
            { "debugger_i2_hw_channel_count", 0x13C},
            { "debugger_i2_hw_samples_per_channel", 0x140},
            { "debugger_i2_hw_memory_depth", 0x144},
            { "debugger_i2_rgf_set_active_channel", 0x148},
            { "debugger_i2_rgf_set_wr_address", 0x14C},
            { "debugger_i2_rgf_set_rd_address", 0x150},
            { "debugger_i2_rgf_get_wr_address", 0x154},
            { "debugger_i2_rgf_get_rd_address", 0x158},
            { "debugger_i2_rgf_get_wr_pointer", 0x15C},
            { "debugger_i2_rgf_get_rd_pointer", 0x160},
            { "debugger_i2_rgf_wr_sample", 0x164},
            { "debugger_i2_rgf_rd_sample", 0x168},
            { "debugger_i2_rgf_start_recording", 0x16C},
            { "debugger_i2_rgf_activate_player", 0x170},
            { "debugger_i2_rgf_player_count", 0x174},
            { "debugger_i2_rgf_mode_status", 0x178},
            { "debugger_i3_hw_channel_count", 0x17C},
            { "debugger_i3_hw_samples_per_channel", 0x180},
            { "debugger_i3_hw_memory_depth", 0x184},
            { "debugger_i3_rgf_set_active_channel", 0x188},
            { "debugger_i3_rgf_set_wr_address", 0x18C},
            { "debugger_i3_rgf_set_rd_address", 0x190},
            { "debugger_i3_rgf_get_wr_address", 0x194},
            { "debugger_i3_rgf_get_rd_address", 0x198},
            { "debugger_i3_rgf_get_wr_pointer", 0x19C},
            { "debugger_i3_rgf_get_rd_pointer", 0x1A0},
            { "debugger_i3_rgf_wr_sample", 0x1A4},
            { "debugger_i3_rgf_rd_sample", 0x1A8},
            { "debugger_i3_rgf_start_recording", 0x1AC},
            { "debugger_i3_rgf_activate_player", 0x1B0},
            { "debugger_i3_rgf_player_count", 0x1B4},
            { "debugger_i3_rgf_mode_status", 0x1B8},
            { "enable_data_path", 0x1BC},
            { "activate_loopback", 0x1C0 },
            { "end", 0x1C4 }
        };*/

        public Dictionary<string, DebuggerInstance> Debuggers = new Dictionary<string, DebuggerInstance>
        {
            ["uplink_modem"] = new DebuggerInstance("debugger_i0_", 4, 8, 1024),
            ["uplink_DAC"] = new DebuggerInstance("debugger_i1_", 2, 8, 1024),
            ["downlink_modem"] = new DebuggerInstance("debugger_i2_", 4, 8, 1024),
            ["downlink_ADC"] = new DebuggerInstance("debugger_i3_", 1, 20, 1024)
        };

        

        public void Init(SpiDriver ft, FtdiInterfaceManager interfaceManager)
        {
            _ft = ft ?? throw new ArgumentNullException(nameof(ft));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));

            _ft = _interfaceManager.GetSpi(); // Get current SPI interface
        }

        /// <summary>
        /// Sends a 32-bit write command over SPI.
        /// </summary>
        public void SpiWrite(uint address, uint data)
        {
            byte[] txBuffer = new byte[11];

            txBuffer[0] = 0x00; // Write command

            // Address: MSB first
            txBuffer[1] = (byte)((address >> 24) & 0xFF);
            txBuffer[2] = (byte)((address >> 16) & 0xFF);
            txBuffer[3] = (byte)((address >> 8) & 0xFF);
            txBuffer[4] = (byte)(address & 0xFF);

            // Data: MSB first
            txBuffer[5] = (byte)((data >> 24) & 0xFF);
            txBuffer[6] = (byte)((data >> 16) & 0xFF);
            txBuffer[7] = (byte)((data >> 8) & 0xFF);
            txBuffer[8] = (byte)(data & 0xFF);

            // Unused bytes
            txBuffer[9] = 0x00;
            txBuffer[10] = 0x00;

            _ft.Write(txBuffer);
        }

        /// <summary>
        /// Sends a 32-bit read command over SPI and returns the 32-bit register value from FPGA.
        /// </summary>
        public uint SpiRead(uint address)
        {
            byte[] txBuffer = new byte[11];
            byte[] rxBuffer = new byte[11];

            txBuffer[0] = 0x01; // Read command

            // Address: MSB first
            txBuffer[1] = (byte)((address >> 24) & 0xFF);
            txBuffer[2] = (byte)((address >> 16) & 0xFF);
            txBuffer[3] = (byte)((address >> 8) & 0xFF);
            txBuffer[4] = (byte)(address & 0xFF);

            // Bytes 5–10 are dummy (0x00)
            for (int i = 5; i < txBuffer.Length; i++)
              txBuffer[i] = 0x00;            

            _ft.TransferFullDuplex(txBuffer, rxBuffer); // Perform full-duplex SPI transfer

            // Extract 32-bit result from bytes 6–9 (MSB first)
            uint result = (uint)(
                (rxBuffer[6] << 24) |
                (rxBuffer[7] << 16) |
                (rxBuffer[8] << 8) |
                rxBuffer[9]);

            return result;
        }




        //new test function to read register from FPGA
        public uint SpiReadByName(string regName)
        {
            if (!RegisterMapTOP_Container.TryGetValue(regName, out uint offset))
                throw new ArgumentException($"Register '{regName}' not found.");

            uint address = BASE_ADDRESS + offset;

            return SpiRead(address);
        }
        //new test function to write register to FPGA
        public void SpiWriteByName(string regName, uint value)
        {
            if (!RegisterMapTOP_Container.TryGetValue(regName, out uint offset))
                throw new ArgumentException($"Register '{regName}' not found.");

            uint address = BASE_ADDRESS + offset;
            SpiWrite(address, value);
        }
        public DataTable InitDataTableFPGA()
        {
            dtFPGA.Columns.Add("Module", typeof(string));
            dtFPGA.Columns.Add("RegisterName", typeof(string));
            dtFPGA.Columns.Add("Address", typeof(string));
            dtFPGA.Columns.Add("AccessType", typeof(string));
            dtFPGA.Columns.Add("Value", typeof(string));


            return dtFPGA;
        }

        //Load JES204C register data file from a CSV file
        public void LoadRegisterFile(string filePath)
        {

            dtFPGA.Clear();
            try 
            {
                foreach (var line in File.ReadLines(filePath))
                {
                    var parts = line.Split(',').Select(p => p.Trim()).ToArray();
                    if (parts.Length == 5)
                    {
                        dtFPGA.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading register file: " + ex.Message);
                return;
            }             
        }

        //for loading the file from the FPGA
        public void WriteReadFPGA()
        {
            foreach (DataRow row in dtFPGA.Rows)
            {
                string access = row["AccessType"].ToString();
                string addressStr = row["Address"].ToString();
                string valueStr = row["Value"].ToString();

                uint address = Convert.ToUInt32(addressStr.Replace("0x", ""), 16);

                if (access == "R")
                {
                    uint readVal = SpiRead(address);
                    row["Value"] = $"0x{readVal:X8}";  // update value column
                }
                else if (access == "W")
                {
                    uint writeVal = Convert.ToUInt32(valueStr.Replace("0x", ""), 16);
                    SpiWrite(address, writeVal);
                }
            }
        }

        

        //convert from uint to hex string
        public string UIntToHexValue(uint value)
        {
            return "0x" + value.ToString("X8");
        }

        //convert from uint to byte array
        public static byte[] UIntToByteArray(uint value)
        {
            return new byte[]
            {
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }
        public uint ReadRegisterWithIntegrity(uint address)
        {
            var list = new List<uint>();
            for (int i = 0; i < MaxRetries; i++)
                list.Add(SpiRead(address));

            return FindMajority(list);
        }

        private uint FindMajority(List<uint> values)
        {
            return values.GroupBy(x => x)
                         .OrderByDescending(g => g.Count())
                         .First().Key;
        }

        public int ConvertFromComplex(Complex a)
        {
            int real = (int)Math.Round(a.Real);
            int imag = (int)Math.Round(a.Imaginary);
            int i = (real + (1 << BC)) % (1 << BC);
            int q = (imag + (1 << BC)) % (1 << BC);
            return (q << BC) | i;
        }

        public Complex ConvertToComplex(uint packed)
        {
            int i = (int)(packed & 0xFFF);
            if (i > 2047) i -= 4096;
            int q = (int)(packed >> BC);
            if (q > 2047) q -= 4096;
            return new Complex(i, q);
        }

        public Complex[] LoadVector(string path)
        {
            return File.ReadAllLines(path)
                       .Select(line => line.Trim().Replace("j", "i"))
                       .Where(line => !string.IsNullOrWhiteSpace(line))
                       .Select(s => ParseComplex(s))
                       .ToArray();
        }

        // Helper method to parse a string into a Complex number
        private static Complex ParseComplex(string s)
        {
            // Handles formats like "1.0+2.0i" or "1.0-2.0i"
            s = s.Replace(" ", "");
            int iIndex = s.LastIndexOf('i');
            if (iIndex < 0)
                throw new FormatException("Invalid complex format: missing 'i'.");

            string withoutI = s.Substring(0, iIndex);
            int plusIndex = withoutI.LastIndexOf('+');
            int minusIndex = withoutI.LastIndexOf('-');

            int splitIndex = Math.Max(plusIndex, minusIndex);
            if (splitIndex <= 0)
                throw new FormatException("Invalid complex format: missing '+' or '-' between real and imaginary.");

            string realPart = withoutI.Substring(0, splitIndex);
            string imagPart = withoutI.Substring(splitIndex);

            double real = double.Parse(realPart, System.Globalization.CultureInfo.InvariantCulture);
            double imag = double.Parse(imagPart, System.Globalization.CultureInfo.InvariantCulture);

            return new Complex(real, imag);
        }

        public void WriteToPlayerMemory(string debuggerKey, int stream, Complex[] samples)
        {
            if (!Debuggers.TryGetValue(debuggerKey, out DebuggerInstance dbg))
            {
                MessageBox.Show("Invalid debugger key: " + debuggerKey);
                return;
            }

            int count = samples.Length;

            int PlayerCount = count / dbg.Parallel;

            if (count % dbg.Parallel != 0 || count / dbg.Parallel > dbg.MemLen)
            {
                MessageBox.Show("Invalid sample count: must be divisible by " + dbg.Parallel);
                return;
            }

            string prefix = dbg.Prefix;
            SpiWriteByName(prefix+ "rgf_set_active_channel", (uint)stream);
            SpiWriteByName(prefix+ "rgf_player_count", (uint)PlayerCount);
            SpiWriteByName(prefix+ "rgf_set_wr_address", 0);

            

            foreach (var c in samples)
            {
                int packed = ConvertFromComplex(c);
                //SpiWrite(StringToAddress(prefix + "rgf_wr_sample"), (uint)packed);
            }
        }

        public void ActivatePlayer(string debuggerKey, bool play)
        {
            if (!Debuggers.TryGetValue(debuggerKey, out DebuggerInstance dbg))
            {
                MessageBox.Show("Invalid debugger key: " + debuggerKey);
                return;
            }
            string preffix = dbg.Prefix;
            SpiWriteByName(preffix + "rgf_activate_player", play ? 1u : 0u);
        }

        public void StopPlayer(string debuggerKey)
        {
            ActivatePlayer(debuggerKey, false);
        }

        
        public class DebuggerInstance
        {
            public string Prefix { get; set; }
            public int StreamCount { get; set; }
            public int Parallel { get; set; }
            public int MemLen { get; set; }

            public DebuggerInstance(string prefix, int streams, int par, int memlen)
            {
                Prefix = prefix;
                StreamCount = streams;
                Parallel = par;
                MemLen = memlen;
            }
        }


        public string LoadVectorDataCsv()
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

                        filepath = ftfile.FileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning");

                }

                return filepath;
            }
        }


        public void LoadVectorFile(string vectorfile)
        {
            if (!string.IsNullOrEmpty(vectorfile))
            {

            }
            else
            {
                   
            }
        }

        public void Dispose()
        {
            //_ft?.Dispose();
            _ft = null; 
        }

        

    }
}
