using System;
using System.Collections.Generic;
using System.Data;

using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace BringUp_Control
{
    internal sealed class AD4368_PLL : IDisposable
    {

        private SpiDriver _ft;
        //private readonly byte _cs;   // CS pin on FT4222H (0‑3)

        List<string> regaddresslist = new List<string>();
        DataTable dtAD4368 = new DataTable();

        

        public void Init(SpiDriver ft)
        {
            _ft = ft;
            WriteRegister(0x0000, 0x18); // 4-wire SPI mode
        }

        public void WriteRegister(ushort reg, byte data)
        {
            if (_ft == null) return;

            byte[] buffer = new byte[3]
            {
            (byte)(reg >> 8),
            (byte)(reg & 0xFF),
            data
            };

            _ft.Write(buffer);
            
        }

        public byte ReadRegister(ushort reg)
        {
            if (_ft == null) return 0;
            ushort readCmd = (ushort)(reg | 0x8000);
            byte[] tx = new byte[3]
            {
            (byte)(readCmd >> 8),
            (byte)(readCmd & 0xFF),
            0x00
            };

            byte[] rx = new byte[3];

            _ft.TransferFullDuplex(tx, rx);
            return rx[2];
        }

        

        public void Dispose()
        {
            _ft = null; // Release the SpiDriver reference
        }

        public DataTable InitDataTable()
        {
            //DataTable dtAD4368 = new DataTable();
            dtAD4368.Columns.Add("Index", typeof(int));
            dtAD4368.Columns.Add("Register", typeof(string));
            dtAD4368.Columns.Add("Value", typeof(string));
            dtAD4368.Columns.Add("Value byte", typeof(byte));            

            return dtAD4368;
        }        

        public List<string> LoadComboRegisters()
        {
            for (int i = 0x0000; i <= 0x0063; i++)
            {
                if (i >= 0x0007 && i <= 0x0009)
                    continue; // Skip values 0x0007 to 0x0009
                regaddresslist.Add($"0x{i:X4}"); // Format as hexadecimal with 4 digits
            }

            return regaddresslist;            
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
                        if (dtAD4368.Rows.Count != 0)
                            dtAD4368.Clear();

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
                            dtAD4368.Rows.Add(index++.ToString(), $"0x{value:X4}", parts[1].Trim(), Convert.ToByte(parts[1].Trim(), 16));
                        }
                    }
                    MessageBox.Show("AD4368 Register Memory Map Loaded!", "Info");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning");
                }
            }
        }
        
    }
}
