using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BringUp_Control
{
    internal class HMC7044
    {

        List<string> regaddresslist = new List<string>();
        DataTable dtHMC7044 = new DataTable();

        public DataTable InitDataTable()
        {
            //DataTable dtAD4368 = new DataTable();
            dtHMC7044.Columns.Add("Index", typeof(int));
            dtHMC7044.Columns.Add("Register", typeof(string));
            dtHMC7044.Columns.Add("Value", typeof(string));
            dtHMC7044.Columns.Add("Value byte", typeof(byte));

            return dtHMC7044;
        }

        

        

        public List<string> LoadComboRegistersHMC()
        {
            for (int i = 0x0000; i <= 0x0153; i++)
            {
                
                if (i >= 0x000F && i <= 0x0013)
                    continue;

                if (i >= 0x0023 && i <= 0x0025)
                    continue;

                if (i >= 0x002B && i <= 0x0030)
                    continue;

                if (i >= 0x003D && i <= 0x0045)
                    continue;

                if (i >= 0x004A && i <= 0x004F)
                    continue;

                if (i >= 0x0055 && i <= 0x0059)
                    continue;

                if (i >= 0x005F && i <= 0x0063)
                    continue;

                if (i >= 0x0066 && i <= 0x006F)
                    continue;

                if (i >= 0x0072 && i <= 0x0077)
                    continue;

                if (i == 0x0080 || i == 0x0081 || i ==0x00AA || i == 0x00B4)
                    continue;

                if (i >= 0x0088 && i <= 0x008B)
                    continue;

                if (i >= 0x0092 && i <= 0x0095)
                    continue;

                if (i >= 0x00B9 && i <= 0x00C7)
                    continue;

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
                            string label = "Control_Register_Map";

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
            using (OpenFileDialog ft = new OpenFileDialog())
            {
                try
                {
                    ft.InitialDirectory = Directory.GetCurrentDirectory();
                    ft.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*";
                    ft.FilterIndex = 0;

                    if (ft.ShowDialog() == DialogResult.OK)
                    {
                        if (dtHMC7044.Rows.Count != 0)
                            dtHMC7044.Clear();

                        filepath = ft.FileName;

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
                            dtHMC7044.Rows.Add(index++.ToString(), $"0x{value:X4}", parts[1].Trim(), Convert.ToByte(parts[1].Trim(), 16));
                        }
                    }
                    MessageBox.Show("HMC7044 Register Memory Map Loaded!", "Info");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning");
                }
            }
        }
    }
}



// Control_Register_Map