using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

// BringUp application contains full list of RF part to control them for R&D tests

namespace BringUp_Control
{
    public partial class MainForm : Form
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVNODES_CHANGED = 0x0007;

        const int SPI_WRITE_CMD = 0x0000; // spi write code
        const int SPI_READ_CMD = 0x8000;  // spi read code

        public const int GPIO3 = 3;

        string filepath = string.Empty;  // csv file path
        int usbcounter = 0;

        private bool _usbInitInProgress = false;
        public System.Timers.Timer _usbDebounceTimer;


        Ft4222Native FTDriver = new Ft4222Native();            

        DataTable DT4368 = new DataTable();
        DataTable DT9175 = new DataTable();
        List<string> deviceInfo = new List<string>();

        TabPage selectedTab = new TabPage();
        
        bool driverflag = false;    // SPI Driver FLAG, FTDI init
        bool usbflag = false;       // USB CONNECTED FLAG

        string datavalue = string.Empty;

        // FTDI FT4222H devices
        Ft4222Device ftDev;
        AD4368_PLL ad4368;
        AD9175_DAC ad9175;
        public MainForm()
        {
            InitializeComponent();            

            // Get Version of application and show it Form Title
            string title = Assembly.GetEntryAssembly().GetName().Version.ToString();
            this.Text = $"BringUP SW App ver: {title}";            

            dataGridViewAD4368.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            dataGridViewAD4368.AllowUserToAddRows = false;
            dataGridViewAD9175.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            dataGridViewAD9175.AllowUserToAddRows = false;            

            string inifilecheck = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "init_config.ini");            

            if(!File.Exists(inifilecheck))
            {
                LogStatus("INI file not detected");
                SetControlsEnabled(false);
            }
            else
            {
                try
                {
                    var configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddIniFile(@"init_config.ini").Build();


                    string testValue = configuration["PLL4368:CLOCKFREQ"];
                    if (string.IsNullOrWhiteSpace(testValue))
                    {                        
                        LogStatus("INI file loaded but missing or invalid values.");
                    }
                    else
                    {                        
                        LogStatus("INI file loaded successfully.");
                    }                    
                }
                catch (Exception ex)
                {
                    LogStatus("Failed to load INI file.");
                    MessageBox.Show(ex.Message, "Info");
                }
            }  
        }

        

        private void CheckFTDIConnection()
        {
            string dllstring = string.Empty;
            try
            {
                bool isConnected = FTDriver.DeviceFlag(FTDriver.NumDevices());

                uint num = 0;
                Ft4222Native.FT_CreateDeviceInfoList(ref num);

                FTDriver.GetInfo(num, ref deviceInfo);

                // Avoid invoke if form handle not created yet
                if (!label1.IsHandleCreated)
                    return;

                // FTDI label status update
                if (isConnected )
                {
                    dllstring = "FTDI STATUS: " + $"Detected 2 FT4222H device(s): and {FTDriver.GetFtdiDriverVersion()}";
                    label1.ForeColor = Color.Green;
                    
                }
                else
                {
                    dllstring = "FTDI device disconnected";
                    label1.ForeColor = Color.Red;
                }

                label1.Invoke((MethodInvoker)(() =>
                {
                    label1.Text = dllstring;
                    LogStatus(dllstring);
                }));

                if (!isConnected)
                {
                    // Dispose resources if FTDI is disconnected                   

                    ad4368?.Dispose();
                    ad9175?.Dispose();
                    ftDev?.Dispose();

                    SetControlsEnabled(false);
                    usbflag = isConnected;
                    driverflag = isConnected;
                    return;
                }
                else
                {
                    // FTDI reconnected — reinitialize
                    uint locnumber = FTDriver.GetDeviceLocId(0); //0 - is Device A interface
                    //uint locnumber = Ft4222Native.FindSpiInterfaceLocId();
                    ftDev?.Dispose();

                    ftDev = new Ft4222Device(locnumber, Ft4222Native.FT4222_SPI_Mode.SPI_IO_SINGLE, Ft4222Native.FT4222_CLK.CLK_DIV_16, Ft4222Native.FT4222_SPICPOL.CLK_IDLE_LOW, Ft4222Native.FT4222_SPICPHA.CLK_LEADING, 0x01);    // open first bridge
                   
                    ad4368 = new AD4368_PLL(ftDev, 0);   // second paramemter is 0 not neeed  will be removed
                                                 
                    
                    DT4368 = ad4368.InitDataTable();
                    dataGridViewAD4368.DataSource = DT4368;
                    comboRegAddress.DataSource = ad4368.LoadComboRegisters();
                    LogStatus("AD4368 reinitialized on SPI CS1");                    

                    SetControlsEnabled(isConnected);
                    usbflag = isConnected;
                    driverflag = isConnected;
                }

                    
            }
            catch (Exception ex)
            {
                string errorText = "Error: " + ex.Message;
                label1.Text = errorText;
                label1.ForeColor = Color.OrangeRed;
                LogStatus(errorText);
                SetControlsEnabled(false);
            }
            
        }

        private void LogStatus(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            textLog.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
        }
               

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_DEVICECHANGE)
            {
                int eventType = m.WParam.ToInt32();
                Console.WriteLine($"WM_DEVICECHANGE fired: {eventType}");

                // Debounce — only trigger one init
                if (_usbInitInProgress) return;

                if (eventType == DBT_DEVICEARRIVAL ||
                    eventType == DBT_DEVICEREMOVECOMPLETE ||
                    eventType == DBT_DEVNODES_CHANGED)
                {
                    
                    // Delay check to allow USB stack to settle
                    _usbDebounceTimer?.Dispose();
                    _usbDebounceTimer = new System.Timers.Timer();

                    _usbInitInProgress = true;                    
                    
                    _usbDebounceTimer.Interval = 1000;         
                                        
                    
                    _usbDebounceTimer.Elapsed += (s, e) =>
                    {
                        _usbDebounceTimer.Stop();
                        _usbDebounceTimer.Dispose();
                        _usbDebounceTimer = null;

                        this.Invoke((MethodInvoker)(() =>
                        {
                            _usbInitInProgress = false;
                            CheckFTDIConnection(); // your existing method
                        }));
                    };
                    _usbDebounceTimer.AutoReset = false;
                    _usbDebounceTimer.Start();
                }
            }
        }

        private void Cmd_Exit_Click(object sender, EventArgs e)
        {
            ftDev?.Dispose();
            ad4368?.Dispose();
            ad9175?.Dispose();


            Application.ExitThread();
        }     

        
        // Validation of correct HEX value
        private bool IsHexString(string input)
        {
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                input = input.Substring(2); // Remove "0x" prefix

            return int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        }

        private void comboRegAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRegisterAddress = comboRegAddress.SelectedItem.ToString();
            int selectedHex = Convert.ToInt32(selectedRegisterAddress.Substring(2), 16); // Convert hex string to int

            // Disable the button for values 0x0002 to 0x000D
            if (driverflag)
            {
                Cmd_WriteReg_ADF4368.Enabled = !((selectedHex >= 0x0002 && selectedHex <= 0x000D) || (selectedHex >= 0x0054 && selectedHex <= 0x0063));
                textAD4368_Value.Enabled = !((selectedHex >= 0x0002 && selectedHex <= 0x000D) || (selectedHex >= 0x0054 && selectedHex <= 0x0063));
            }
            
            
            if (driverflag && usbflag)
            {
                byte valbyte = ad4368.ReadRegister((ushort)selectedHex);
                textAD4368_Value.Text = $"0x{valbyte:X2}";
            }

            textAD4368_Value.Focus();
        }

        private void Cmd_WriteReg_ADF4368_Click(object sender, EventArgs e)
        {
            string regaddress = comboRegAddress.SelectedItem?.ToString(); // Get selected value as string
                                                                    // 
            byte poweraddress = Convert.ToByte(regaddress.Replace("0x", ""), 16);
            ushort regValue = Convert.ToUInt16(regaddress.Replace("0x", ""), 16);
            byte databyte = Convert.ToByte(datavalue.Replace("0x", ""), 16);

            ad4368.WriteRegister(regValue, databyte);

            if (poweraddress == 0x002B)
            {
                CheckPowerRegister(poweraddress);
            }
        }        

        private void Cmd_ADF4368_INIT_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            selectedTab = tabControl1.SelectedTab;

            CheckFTDIConnection(); // Initial check on load            
        }

        private void comboMUXOUT_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            byte data = 0b0000000;
            byte registerval = 0x002E;

            switch (comboMUXOUT.SelectedIndex)
            {
                case 0:
                    data = 0b00000000;
                    break;
                case 1:
                    data = 0b00010000;
                    break;
                case 2:
                    data = 0b00100000;
                    break;
                case 3:
                    data = 0b00110000;
                    break;
                case 4:
                    data = 0b01000000;
                    break;
                case 5:
                    data = 0b01010000;
                    break;
            }

            ad4368.WriteRegister((ushort)registerval, data);
            Thread.Sleep(100);
            
            byte checkbyte = ad4368.ReadRegister((ushort)registerval);

            if (checkbyte == data)
                MessageBox.Show("MUXOUT Updated Successfully!", "Information");
            else
                MessageBox.Show("MUXOUT Not Updated!", "Information");
        }
              

        private void CheckPowerRegister(byte address)
        {
            byte powerreturn = ad4368.ReadRegister(address);

            if (powerreturn == 0x00)
            {
                Cmd_PowerONOFF.Text = "RF POWER OFF";
                radioRF_POWER_Status.Checked = true;
                //timer1.Start();
            }
            else
            {
                Cmd_PowerONOFF.Text = "RF POWER ON";
                radioRF_POWER_Status.Checked = false;
                //timer1.Stop();
            }
        }

        private void Cmd_PowerONOFF_Click(object sender, EventArgs e)
        {
            byte poweraddress = 0x002B;
            if (Cmd_PowerONOFF.Text.Equals("RF POWER ON"))
            {
                ad4368.WriteRegister(poweraddress, 0x00);
                Thread.Sleep(100);                
                CheckPowerRegister(poweraddress);
            }
            else
            {
                ad4368.WriteRegister(poweraddress, 0x83);
                Thread.Sleep(100);
                CheckPowerRegister(poweraddress);
            }
        }

        private void Cmd_ReadAll_AD4368_Click(object sender, EventArgs e)
        {
            int index = 1; byte paddress = 0;

            if (ftDev == null)
            {
                MessageBox.Show("SPI interface not initialized. Please reconnect the FTDI device.");
                return;
            }

            if (usbflag && driverflag)
            {
                if (dataGridViewAD4368.Rows.Count > 0)
                {
                    dataGridViewAD4368.DataSource = null;
                    DT4368.Clear();
                    dataGridViewAD4368.DataSource = DT4368;
                }

                foreach (var indstring in comboRegAddress.Items)
                {

                    string getcombo = indstring.ToString();
                    ushort regValue = Convert.ToUInt16(getcombo.Replace("0x", ""), 16);// Convert.ToInt32(getcombo.Substring(2), 16);
                    paddress = Convert.ToByte(getcombo.Replace("0x", ""), 16);
                    byte valbyte = ad4368.ReadRegister(regValue);

                    DataRow row = DT4368.NewRow();
                    row["Index"] = index++.ToString();
                    row["Register"] = getcombo;
                    row["Value"] = $"0x{valbyte:X2}";
                    row["Value byte"] = valbyte;
                    DT4368.Rows.Add(row);

                    if (paddress == 0x002B)
                    {
                        CheckPowerRegister(paddress);
                    }
                }

                if (DT4368.Rows.Count != 0)
                {
                    Cmd_WriteAll_AD4368.Enabled = true;
                }
            }
            
        }

        private int RFLockSampling(byte address, int monitoredBitIndex)
        {
            int bitValue = 0;

            /*if (spiDriver != null && usbflag)
            {
                byte lockdata = AD4368_driver.ReadRegister(address);
                bitValue = (lockdata >> monitoredBitIndex) & 0x01;
            }
            else
            {
                bitValue = 0;
            }*/
            
            return bitValue;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (RFLockSampling(0x0058, 0) == 1)
            {
                checkRFLOCK.Checked = true;
                checkRFLOCK.Text = "PLL LOCK";
            }
            else
            {
                checkRFLOCK.Checked = false;
                checkRFLOCK.Text = "PLL UNLOCKED";
            }
        }

        private void Cmd_Import9175_file_Click(object sender, EventArgs e)
        {
            labelFilePath9175.Text = $"DAC File Path: {ad9175.LoadDataTableToCsv()}";
        }

        private void Cmd_Export9175_file_Click(object sender, EventArgs e)
        {
            ad9175.SaveDataTableToCsv(DT9175);
        }               

        // Set Control Enabled via USB connection status
        private void SetControlsEnabled(bool enabled)
        {
            if (!enabled)
            {
                tabControl1.Enabled = false;
                Cmd_Init_All.Enabled = false;
                Cmd_FT_Temp_Read.Enabled = false;
                Cmd_RF_Temp_Read.Enabled=false;
            }
            else
            {
                tabControl1.Enabled = true;
                Cmd_Init_All.Enabled = true;
                Cmd_FT_Temp_Read.Enabled = true;
                Cmd_RF_Temp_Read.Enabled = true;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTab  = tabControl1.SelectedTab;
        }
        // textBox for PLL4368 specific register value update, after pressing Enter focus will change to Write Register button
        private void textAD4368_Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                // Validate Hex value entered in this field
                if (IsHexString(textAD4368_Value.Text))
                {
                    datavalue = textAD4368_Value.Text.ToUpper();
                    Cmd_WriteReg_ADF4368.Focus();
                }
                else
                {
                    textAD4368_Value.Clear();
                    datavalue = string.Empty;
                    textAD4368_Value.Focus();
                }
            }
        }

        private void Cmd_Import_AD4368_File_Click(object sender, EventArgs e)
        {
            labelFilePathAD4368.Text = $"File Path: {ad4368.LoadDataTableToCsv()}";
            Cmd_WriteAll_AD4368.Enabled = true;
        }

        private void Cmd_Export_AD4368_File_Click(object sender, EventArgs e)
        {
            ad4368.SaveDataTableToCsv(DT4368);
        }

        private void Cmd_WriteAll_AD4368_Click(object sender, EventArgs e)
        {
            var filteredRows = DT4368.AsEnumerable().Where(row =>
            {
                string hexStr = row["Register"].ToString();      // e.g. "0x0053"
                int reg = Convert.ToInt32(hexStr.Substring(2), 16); // Convert to int
                return reg >= 0x10 && reg <= 0x53;
            }).OrderByDescending(row => Convert.ToInt32(row["Register"].ToString().Substring(2), 16) // Sort descending
            );

            foreach (var row in filteredRows)
            {

                byte paddress = Convert.ToByte(row["Register"].ToString().Replace("0x", ""), 16);
                ushort regValue = Convert.ToUInt16(row["Register"].ToString().Replace("0x", ""), 16);
                byte databyte = Convert.ToByte(row["Value"].ToString().Replace("0x", ""), 16);

                ad4368.WriteRegister(regValue, databyte);

                if (paddress == 0x002B)
                {
                    CheckPowerRegister(paddress);
                }
            }
        }

        private void Cmd_Init_All_Click(object sender, EventArgs e)
        {

        }
    }
}


/***************
 * 
 * 
 * 
 * 
 * 
 * 
 * try
            {
                //SPI configuration 
                var spiSettings = new SpiConnectionSettings(0, 1)
                {
                    ClockFrequency = 3750000,//ClockDiv16, // 7500000 ClockDiv8, 1875000 ClockDiv32// SPI Clock: System Clock / 16
                    Mode = SpiMode.Mode0, // SPI Mode 0 (CPOL = 0, CPHA = 0)
                    DataBitLength = 8, // 8-bit data per transaction

                };

                // Setup SPI communication FTDI A
                spiDriver = new Ft4222Spi(spiSettings);
                driverflag = true;

                //Configuration of 0x0000 address with MSB bit and SDO - 4 wire SPI protocol
                byte nulladress = 0x0000;
                AD4368_driver.WriteRegister(spiDriver, nulladress, 0x18);

                byte IDC = AD4368_driver.ReadRegister(spiDriver, 0x000C);
                byte IDD = AD4368_driver.ReadRegister(spiDriver, 0x000D);

                /*if (IDC == 86 && IDD == 4)
                    check4368.Checked = true;
                else
                    check4368.Checked = false;*/
       /*     }
            catch (Exception ex)
            {
                driverflag = false;
            }*/
 


/*
 * 
 * private void INIT_FTDI4222()
        {
            //devices = FtCommon.GetDevices();
            
            if (devices.Count == 0)
            {
                
                comboRegAddress.Enabled = false;
                comboMUXOUT.Enabled = false;
                initflag = false;
                Cmd_WriteReg_ADF4368.Enabled = false;
                Cmd_WriteReg9175.Enabled = false;
                

                Cmd_Import_AD4368_File.Enabled = false;
                Cmd_Export_AD4368_File.Enabled = false;
                textAD4368_Value.Enabled = false;
                textDAC9175_Value.Enabled = false;
                
                Cmd_ReadAll_AD4368.Enabled = false;
                Cmd_WriteAll_AD4368.Enabled = false;

                Cmd_PowerONOFF.Enabled = false;
                Cmd_Init_All.Enabled = false;
                
                

                
                label1.ForeColor = Color.Red;
                label1.Text = "FTDI STATUS: " + $"FTDI Driver NOT Detected!";
                Cmd_WriteReg_ADF4368.Enabled = false;
                
                return;
            }
            else
            {
                //var (chip, dll) = Ft4222Common.GetVersions();
                label1.ForeColor = Color.Green;
                //label1.Text = "FTDI STATUS: " + $"Detected {devices.Count} FT4222H device(s): Chip Version {chip}, Dll version {dll}";



                
                try
                {
                        //SPI configuration 
                   
                        driverflag = true;

                        //Configuration of 0x0000 address with MSB bit and SDO - 4 wire SPI protocol
                        byte nulladress = 0x0000;
                        //AD4368_driver.WriteRegister(spiDriver, nulladress, 0x18);

                        //byte IDC = AD4368_driver.ReadRegister(spiDriver, 0x000C);
                        //byte IDD = AD4368_driver.ReadRegister(spiDriver, 0x000D);

                    usbflag = true;
                    initflag = true;
                    SetControlsEnabled(true);

                    /*if (IDC == 86 && IDD == 4)
                        check4368.Checked = true;
                    else
                        check4368.Checked = false;*/
            
       /*         }
                catch (Exception ex)
                {
                    driverflag = false;
usbflag = false;

label1.ForeColor = Color.Red;
label1.Text = "FTDI STATUS: SPI Init failed";
                }
                
    
                comboRegAddress.Enabled = true;
comboMUXOUT.Enabled = true;
initflag = true;

Cmd_WriteReg_ADF4368.Enabled = true;
Cmd_WriteReg9175.Enabled = true;

Cmd_Import_AD4368_File.Enabled = true;
Cmd_Export_AD4368_File.Enabled = false;
textAD4368_Value.Enabled = true;
textDAC9175_Value.Enabled = true;


Cmd_ReadAll_AD4368.Enabled = true;
Cmd_WriteAll_AD4368.Enabled = false;

Cmd_PowerONOFF.Enabled = false;              
        
            }

                        
        }
 * 
 * 
 * 
 * */