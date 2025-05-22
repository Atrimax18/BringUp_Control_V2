using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BringUp_Control
{
    internal sealed class AD9175_DAC : IDisposable
    {
        private SpiDriver _ft;


        public double DAC0_freq { get; set; }
        public double DAC1_freq { get; set; }

        byte[] DAC0;
        byte[] DAC1;

        List<string> regaddresslist9175 = new List<string>();
        DataTable dtAD9175 = new DataTable();

        public void Init(SpiDriver ft)
        {
            _ft = ft;

            // Iinit of the DAC
            PowerUp();
            DAC_PLL_Config();

            if (DelayLockLoop() == 0x01)
            {
                // TODO : Add a message box or log to indicate that the DLL is locked
            }
            else
            {
                // TODO : Add a message box or log to indicate that the DLL is not locked
            }
            Calibration();

            JESD204B_Setup();

            //TODO : TEST IT 
            DAC0 = GetBytes48BitBigEndian(CalculateDdsmFtw(DAC0_freq));   //2 GHz
            DAC1 = GetBytes48BitBigEndian(CalculateDdsmFtw(DAC1_freq));   //3 Ghz
            MainDAC_Datapath_Setup(DAC0, DAC1);

            JESD204B_SERDES_Setup();
            TransportLayer_Setup();
            CleanUpRegisterList();
        }

        //Table 50 : Power Up registee writing - DONE
        public void PowerUp()
        {
            WriteRegister(0x0000, 0x81); // Soft reset
            WriteRegister(0x0000, 0x3C); // Release RESET and configure 4-wire SPI protocol
            WriteRegister(0x0091, 0x00); // Power up clock receiver
            WriteRegister(0x0206, 0x01); // Take PHY out of reset
            WriteRegister(0x0705, 0x01); // Enable Bootloader
            WriteRegister(0x0090, 0x00); // Power on DAC and bias circuity
        }

        // Table 51: DAC and PLL configuration sequence - DONE
        public void DAC_PLL_Config()
        {
            WriteRegister(0x0095, 0x01); // Bypass internal PLL
            WriteRegister(0x0790, 0xFF); // Bypass internal PLL
            WriteRegister(0x0791, 0xFF); // Bypass internal PLL            

        }
        
        // Table 52: Delay lock loop configuration sequence - DONE
        public byte DelayLockLoop()
        {
            WriteRegister(0x00C0, 0x00); // Power up delay line
            WriteRegister(0x00DB, 0x00); // 
            WriteRegister(0x00DB, 0x01); // Update DLL settings to circuityBypass internal PLL
            WriteRegister(0x00DB, 0x00); // 
            WriteRegister(0x00C1, 0x68); // Set DLL search mode Fdac > 4.5GHz
            WriteRegister(0x00C1, 0x69); // Set DLL search mode Fdac > 4.5GHz
            WriteRegister(0x00C7, 0x01); // Enable DLL read status
            return ReadRegister(0x00C3); // Ensure DLL is locked by reading value 1 for Bit0 of this register

        }
        
        // Table 53: Calibration sequence - DONE
        public void Calibration()
        {
            WriteRegister(0x0050, 0x2A); // Optimized calibration setting register
            WriteRegister(0x0061, 0x68); // Required calibration control register
            WriteRegister(0x0051, 0x82); // Optimized calibration setting register
            WriteRegister(0x0051, 0x83); // Required calibration control register 
            WriteRegister(0x0081, 0x03); // Required calibration control register
        }
        
        // Table 54: JESD204B configuration sequence - DONE
        public void JESD204B_Setup()
        {
            WriteRegister(0x0100, 0x00); // Power up digital datapath clocks
            WriteRegister(0x0110, 0x37); // dual link and mode 23
            WriteRegister(0x0111, 0x61); // Main datapath configuration
            WriteRegister(0x0084, 0x01); // SYSREF singla input
            WriteRegister(0x0312, 0x00); // ?????????????????????? NO DATA
            
            //run LINK = 0
            WriteRegister(0x0300, 0x08); // Corresponds to the mode selection made in register 0x110
            
            WriteRegister(0x0475, 0x09); // Soft reset JESD2024B quad byte deframer
            WriteRegister(0x0453, 0x03); // Set scrambling option for SERDES data
            WriteRegister(0x0458, 0x0B); // L value to JESD_MODE            
            WriteRegister(0x0475, 0x01); // Bring the JESD204B quad byte deframer out of reset
            //run LINK = 1
            WriteRegister(0x0300, 0x0C); // Corresponds to the mode selection made in register 0x110
            
            WriteRegister(0x0475, 0x09); // Soft reset JESD2024B quad byte deframer
            WriteRegister(0x0453, 0x03); // Set scrambling option for SERDES data
            WriteRegister(0x0458, 0x0B); // L value to JESD_MODE
            WriteRegister(0x0475, 0x01); // Bring the JESD204B quad byte deframer out of reset
        }

        // Table 55: Channel datapath configuration sequence - CAN BE SKIPPED
        public void ChannelDatapath_Setup()
        {
            
        }

        // Table 56: Main datapath and Main NCO configuration sequence - TEST IT!!!
        public void MainDAC_Datapath_Setup(byte[] DDCM_DAC0, byte[] DDCM_DAC1)
        {

            //DAC 0
            WriteRegister(0x0008, 0x40); // SELECT DAC0
            WriteRegister(0x0112, 0x38); // Enable NCO for selected channels in paging Register 0x008

            //WriteRegister(0x0114, 0xBC); // Write DDSM_FTW[7:0]                     2B C2 BC 2B C2 BC 
            //WriteRegister(0x0115, 0xC2); // Write DDSM_FTW[15:8]                    41 A4 1A 41 A4 1A
            //WriteRegister(0x0116, ); // Write DDSM_FTW[23:16]
            //WriteRegister(0x0117, 0xBC); // Write DDSM_FTW[31:24]
            //WriteRegister(0x0118, 0xC2); // Write DDSM_FTW[39:32]
            //WriteRegister(0x0119, 0x2B); // Write DDSM_FTW[47:40]

            for (ushort i = 0x0114; i <= 0x0119; i++)
            {
                WriteRegister(i, DDCM_DAC0[0x0119-i]);
            }
            
            WriteRegister(0x011C, 0x00); // Write DDSM_NCO_PHASE_OFFSET[7:0]
            WriteRegister(0x011D, 0x00); // Write DDSM_NCO_PHASE_OFFSET[15:8]
            
            WriteRegister(0x0124, 0x00); // Write DDSM_ACC_MODULUS[7:0]
            WriteRegister(0x0125, 0x00); // Write DDSM_ACC_MODULUS[15:8]
            WriteRegister(0x0126, 0x00); // Write DDSM_ACC_MODULUS[23:16]
            WriteRegister(0x0127, 0x00); // Write DDSM_ACC_MODULUS[31:24]
            WriteRegister(0x0128, 0x00); // Write DDSM_ACC_MODULUS[39:32]
            WriteRegister(0x0129, 0x00); // Write DDSM_ACC_MODULUS[47:40]
            
            WriteRegister(0x012A, 0x00); // Write DDSM_ACC_DELTA[7:0]
            WriteRegister(0x012B, 0x00); // Write DDSM_ACC_DELTA[15:8]
            WriteRegister(0x012C, 0x00); // Write DDSM_ACC_DELTA[23:16]
            WriteRegister(0x012D, 0x00); // Write DDSM_ACC_DELTA[31:24]
            WriteRegister(0x012E, 0x00); // Write DDSM_ACC_DELTA[39:32]
            WriteRegister(0x012F, 0x00); // Write DDSM_ACC_DELTA[47:40]
            WriteRegister(0x0113, 0x01); // Update all NCO phase and FTW words

            //DAC 1
            WriteRegister(0x0008, 0x80); // SELECT DAC1
            WriteRegister(0x0112, 0x38); // Enable NCO for selected channels in paging Register 0x008
            //WriteRegister(0x0114, 0x1A); // Write DDSM_FTW[7:0]
            //WriteRegister(0x0115, 0xA4); // Write DDSM_FTW[15:8]
            //WriteRegister(0x0116, 0x41); // Write DDSM_FTW[23:16]
            //WriteRegister(0x0117, 0x1A); // Write DDSM_FTW[31:24]
            //WriteRegister(0x0118, 0xA4); // Write DDSM_FTW[39:32]
            //WriteRegister(0x0119, 0x41); // Write DDSM_FTW[47:40]

            for (ushort i = 0x0114; i <= 0x0119; i++)
            {
                WriteRegister(i, DDCM_DAC1[0x0119 - i]);
            }

            WriteRegister(0x011C, 0x00); // Write DDSM_NCO_PHASE_OFFSET[7:0]
            WriteRegister(0x011D, 0x00); // Write DDSM_NCO_PHASE_OFFSET[15:8]

            WriteRegister(0x0124, 0x00); // Write DDSM_ACC_MODULUS[7:0]
            WriteRegister(0x0125, 0x00); // Write DDSM_ACC_MODULUS[15:8]
            WriteRegister(0x0126, 0x00); // Write DDSM_ACC_MODULUS[23:16]
            WriteRegister(0x0127, 0x00); // Write DDSM_ACC_MODULUS[31:24]
            WriteRegister(0x0128, 0x00); // Write DDSM_ACC_MODULUS[39:32]
            WriteRegister(0x0129, 0x00); // Write DDSM_ACC_MODULUS[47:40]

            WriteRegister(0x012A, 0x00); // Write DDSM_ACC_DELTA[7:0]
            WriteRegister(0x012B, 0x00); // Write DDSM_ACC_DELTA[15:8]
            WriteRegister(0x012C, 0x00); // Write DDSM_ACC_DELTA[23:16]
            WriteRegister(0x012D, 0x00); // Write DDSM_ACC_DELTA[31:24]
            WriteRegister(0x012E, 0x00); // Write DDSM_ACC_DELTA[39:32]
            WriteRegister(0x012F, 0x00); // Write DDSM_ACC_DELTA[47:40]
            WriteRegister(0x0113, 0x01); // Update all NCO phase and FTW words
        }

        // Table 57 : JESD204B SERDES configuration sequence
        public void JESD204B_SERDES_Setup()
        {
            WriteRegister(0x0240, 0xAA); // EQ settings IN_Loss < 11dB value 0xAA
            WriteRegister(0x0241, 0xAA); // EQ settings IN_Loss < 11dB value 0xAA
            WriteRegister(0x0242, 0x55); // EQ settings IN_Loss < 11dB value 0x55
            WriteRegister(0x0243, 0x55); // EQ settings IN_Loss < 11dB value 0x55
            WriteRegister(0x0244, 0x1F); // EQ settings
            WriteRegister(0x0245, 0x1F); // EQ settings
            WriteRegister(0x0246, 0x1F); // EQ settings
            WriteRegister(0x0247, 0x1F); // EQ settings
            WriteRegister(0x0248, 0x1F); // EQ settings
            WriteRegister(0x0249, 0x1F); // EQ settings
            WriteRegister(0x024A, 0x1F); // EQ settings
            WriteRegister(0x024B, 0x1F); // EQ settings
           
            WriteRegister(0x0201, 0x00); // Power down unused PHYs - - 0x00 = all PHYs powered up
            WriteRegister(0x0203, 0x00); // Powe up SYNCOUT0/1 - 0x00 = all SYNCOUT powered up
            WriteRegister(0x0253, 0x01); // Set SYNCOUT0' to be LVDS
            WriteRegister(0x0254, 0x01); // Set SYNCOUT1' to be LVDS
            WriteRegister(0x0210, 0x16); // SERDES required register write value
            WriteRegister(0x0216, 0x05); // SERDES required register write value
            WriteRegister(0x0212, 0xFF); // SERDES required register write value
            WriteRegister(0x0212, 0x00); // SERDES required register write value
            WriteRegister(0x0210, 0x87); // SERDES required register write value
            WriteRegister(0x0216, 0x11); // SERDES required register write value
            WriteRegister(0x0213, 0x01); // SERDES required register write value
            WriteRegister(0x0213, 0x00); // SERDES required register write value
            WriteRegister(0x0200, 0x00); // Powe up SERDES circuittry blocks
            Thread.Sleep(100); // delay 100ms
            
            WriteRegister(0x0210, 0x86); // EQ settings
            WriteRegister(0x0216, 0x40); // EQ settings
            WriteRegister(0x0213, 0x01); // EQ settings
            WriteRegister(0x0213, 0x00); // EQ settings

            WriteRegister(0x0210, 0x86); // EQ settings
            WriteRegister(0x0216, 0x00); // EQ settings
            WriteRegister(0x0213, 0x01); // EQ settings
            WriteRegister(0x0213, 0x00); // EQ settings
            
            WriteRegister(0x0210, 0x87); // EQ settings
            WriteRegister(0x0216, 0x01); // EQ settings
            WriteRegister(0x0213, 0x01); // EQ settings
            WriteRegister(0x0213, 0x00); // EQ settings
            
            WriteRegister(0x0280, 0x05); // EQ settings
            WriteRegister(0x0280, 0x01); // EQ settings

            byte bit0 = ReadRegister(0x0281); //Ensure Bit0 reads back 1 to indicate SERDES PLL is locked


        }

        //Table 58: Transport layer configuration sequence - DONE
        public void TransportLayer_Setup()
        {
            WriteRegister(0x0308, 0x08); // Crosbar setup , program the physical lane value
            WriteRegister(0x0309, 0x1A); // logical lines 3 and 2
            WriteRegister(0x030A, 0x2C); // logical lines 5 and 4
            WriteRegister(0x030B, 0x3E); // logical lines 7 and 6
            
            WriteRegister(0x003B, 0xF1); // Enable the sync logic , rotation mode
            WriteRegister(0x003A, 0x02); // Setup sync for one-shot sync mode
            WriteRegister(0x0300, 0x0B); // Link modes duallink
            
        }

        // Table 59: Register cleanup sequence - DONE
        public void CleanUpRegisterList()
        {
            WriteRegister(0x0085, 0x13); // Set the default register value
            WriteRegister(0x01DE, 0x03); // Disable analog SPI
            WriteRegister(0x0008, 0xC0); // Page all main DACs fro TXEN control update
            WriteRegister(0x0596, 0x0C); // SPU turn on TXEnx feature
        }

        public List<string> LoadComboRegister9175()
        {
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


        // DAC_FS measurement and output value calculation
        public int DAC_FS(int dac_index, float IOUTFS_mA)
        {
            uint DAC_FS_Value = 0;
            if (IOUTFS_mA < 15.625 || IOUTFS_mA > 25.977)
                DAC_FS_Value = 0;


            DAC_FS_Value = (uint)(1 << (6 + dac_index));
            WriteRegister(0x0008, (byte)DAC_FS_Value);

            int FSC_Ctrl = Convert.ToInt16((IOUTFS_mA-15.625)*256/25);

            return FSC_Ctrl;
        }
        

        public DataTable InitDataTableDAC()
        {
            
            dtAD9175.Columns.Add("Index", typeof(int));
            dtAD9175.Columns.Add("Register", typeof(string));
            dtAD9175.Columns.Add("Value", typeof(string));
            dtAD9175.Columns.Add("Value byte", typeof(byte));

            return dtAD9175;
        }

        
        public byte[] DDSM_Calculation()
        {
            byte[] DDSM = new byte[8];
            DDSM[0] = 0x00;
            DDSM[1] = 0x00;
            DDSM[2] = 0x00;
            DDSM[3] = 0x00;
            DDSM[4] = 0x00;
            DDSM[5] = 0x00;
            DDSM[6] = 0x00;
            DDSM[7] = 0x00;



            return DDSM;
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
        public static ulong CalculateDdsmFtw(double numeratorHz)
        {
            double fraction = numeratorHz / 11.7e9;
            double result = fraction * Math.Pow(2, 48);
            return (ulong)Math.Round(result); // Round to nearest integer
        }

        // Method to convert a 48-bit unsigned integer to a  6-byte array in big-endian format
        public static byte[] GetBytes48BitBigEndian(ulong value)
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

        public void Dispose()
        {
            _ft?.Dispose();
        }
    }
}
