﻿using System;
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
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using FTD2XX_NET;


// BringUp application contains full list of RF part to control them for R&D tests

namespace BringUp_Control
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;

        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVNODES_CHANGED = 0x0007;
         

        public const int GPIO3 = 3;

        string filepath = string.Empty;  // csv file path
        //int usbcounter = 0;

        private bool _usbInitInProgress = false;
        public System.Timers.Timer _usbDebounceTimer;


        Ft4222Native FTDriver = new Ft4222Native(); 
        FtdiInterfaceManager InterfaceManager; // FTDI SPI interface manager



        DataTable DT4368 = new DataTable();
        DataTable DT9175 = new DataTable();
        List<string> deviceInfo = new List<string>();

        TabPage selectedTab = new TabPage();
        
        bool driverflag = false;    // SPI Driver FLAG, FTDI init
        bool usbflag = false;       // USB CONNECTED FLAG
        bool PLL_Init_Flag = false; // PLL Init Flag
        bool TXline_flag = false;   // TX line up values upload flag
        private bool _disposed;


        private struct TX_Line
        {
            public bool bypass1;
            public bool bypass2;
            public float att1;
            public float att2;
            public float att3;
            public double nco_dac0;
            public double nco_dac1;
        }

        //private byte att1_value = 0x00;
        //private byte att2_value = 0x00;
        //private byte att3_value = 0x00;
        private float att1_value = 0.00f;
        private float att2_value = 0.00f;
        private float att3_value = 0.00f;


        

        private string fpga_address = string.Empty; // FPGA address for register access (string)
        private string fpga_data = string.Empty;    // FPGA data for register access (string)

        private static readonly Regex HexBytePattern = new Regex(@"^0x[0-9A-Fa-f]{2}$");
        private static readonly Regex HexU16Pattern = new Regex(@"^0x[0-9A-Fa-f]{4}$");
        private static readonly Regex HexU32Pattern = new Regex(@"^0x[0-9A-Fa-f]{8}$");

        private uint _spiLocId = UInt32.MaxValue;   // interface-A  (SPI)
        private uint _gpioLocId = UInt32.MaxValue;   // interface-B  (GPIO / future I²C)

        SpiDriver ftDev;
        AD4368_PLL ad4368;
        AD9175_DAC ad9175;
        FPGA fpga;
        GpioDriver gpio_control;
        i2cDriver i2cBus;
        TMP100 tmp100;
        PCAL6416A IO_Exp;
        HMC1119 hmc1119; // RF TX Line Up Attenuators
        HMC8414 hmc8414; // RF TX Line Up Amplifiers

        PCA9547A MUX; // I2C MUX SNOW EVB Board
        AD7091 ad7091; // ADC for RF Power measurement

        string dac_ini = string.Empty; // DAC 9175 INI file path

        TX_Line txLineData = new TX_Line(); 
        public MainForm()
        {
            InitializeComponent();
            Instance = this;

            numericATT1.Minimum = 0.00M;
            numericATT1.Maximum = 31.75M;

            numericATT2.Minimum = 0.00M;
            numericATT2.Maximum = 31.75M;

            numericATT3.Minimum = 0.00M;
            numericATT3.Maximum = 31.75M;

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


                    string testValue = configuration["PLL4368:POWER_SW"];
                    if (string.IsNullOrWhiteSpace(testValue))
                    {                        
                        LogStatus("INI file loaded but missing or invalid values.");
                    }
                    else
                    {                        
                        LogStatus("INI file loaded successfully.");

                        // Read the INI file and set the values

                        if (configuration["HMC8414:BYPASS_MODE1"] == "0")   //Bypass ON = false -> (AMP ON)
                            txLineData.bypass1 = false;                        
                        else
                            txLineData.bypass1 = true;                      //Bypass ON = true  -> (AMP OFF) 

                        if (configuration["HMC8414:BYPASS_MODE2"] == "0")
                            txLineData.bypass2 = false;
                        else
                            txLineData.bypass2 = true;
                            
                        txLineData.att1 = float.Parse(configuration["HMC1119:ATT1"]);
                        txLineData.att2 = float.Parse(configuration["HMC1119:ATT2"]);
                        txLineData.att3 = float.Parse(configuration["HMC1119:ATT3"]);

                        //txLineData.nco_dac0 = double.Parse(configuration["DAC9175:NCO_DAC0_GHz"]);
                        //txLineData.nco_dac1 = double.Parse(configuration["DAC9175:NCO_DAC1_GHz"]);

                        dac_ini = configuration["DAC9175:INIT_FILE"];

                        if (File.Exists(dac_ini))
                        {
                            LogStatus($"DAC 9175 INI file loaded!");
                        }
                        else
                        {
                            LogStatus("DAC 9175 INI file not found or invalid path.");
                            dac_ini = string.Empty; // Reset if not valid
                        }
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
                Ft4222Native.FT_CreateDeviceInfoList(out num);

                FTDriver.GetInfo(num, ref deviceInfo);

                // Avoid invoke if form handle not created yet
                if (!label1.IsHandleCreated)
                    return;

                // FTDI label status update
                if (isConnected)
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
                    i2cBus?.Dispose();
                    gpio_control?.Dispose();
                    fpga?.Dispose();
                    hmc1119?.Dispose();
                    ad7091?.Dispose();

                    usbflag = false;
                    driverflag = false;
                    SetControlsEnabled(false);
                    
                    return;
                }
                else
                {
                    // FTDI reconnected — reinitialize
                    uint locfirst = FTDriver.GetDeviceInterfaceSPI("FT4222 A");   //Device A interface for SPI
                    uint locsecond = FTDriver.GetDeviceInterfaceSPI("FT4222 B");  //Device B interface for GPIO and I2C

                    // ── Guard: if nothing changed we’re already initialised ────────────────────
                    bool locationsUnchanged = (ftDev != null) &&
                                              (gpio_control != null) &&
                                              (_spiLocId == locfirst) &&
                                              (_gpioLocId == locsecond) &&
                                              usbflag && driverflag;

                    if (locationsUnchanged)
                        return;   // avoid handle churn

                    // ── Either first time or the device list really changed ───────────────────
                    _spiLocId = locfirst;    //INTERFACE FT4222 A
                    _gpioLocId = locsecond;  //INTERFACE FT4222 B
                    InterfaceManager = new FtdiInterfaceManager(_spiLocId); // Initialize FTDI interface manager
                    InterfaceManager.BusModeChanged += OnBusModeChanged;

                    // ************************* Dispose if already has init ******************************************
                    
                    fpga?.Dispose();
                    ad9175?.Dispose();
                    ad4368?.Dispose();
                    MUX?.Dispose();
                    IO_Exp?.Dispose();
                    gpio_control?.Dispose();
                    ad7091?.Dispose();
                    hmc1119?.Dispose();
                    tmp100?.Dispose();
                                        
                    //  *************************** GPIO INIT DRIVER 2 ***********************************************
                    gpio_control = new GpioDriver(_gpioLocId);                    
                    gpio_control.Write(GPIO3, false);  // GPIO3 is false by default for EVB AD4368 it must be True to enable SPI interface                    

                    i2cBus = InterfaceManager.GetI2c();
                    

                    // flag status change 
                    usbflag = true;
                    driverflag = true;

                    // GUI elements enabled/disabled
                    SetControlsEnabled(true);                    
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
        private void OnBusModeChanged(object sender, string message)
        {
            /*if (InvokeRequired)
            {
                Invoke(new Action(() => logStatus.AppendText($"{message}{Environment.NewLine}")));
            }
            else
            {
                logStatus.AppendText($"{message}{Environment.NewLine}");
            }*/
            LogStatus(message);
        }


        public void LogStatus(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string fullMessage = $"[{timestamp}] {message}{Environment.NewLine}";
            

            if (textLog.InvokeRequired)
            {
                textLog.BeginInvoke(new Action(() =>
                {
                    textLog.AppendText(fullMessage);
                }));
            }
            else
            {
                textLog.AppendText(fullMessage);
            }
        }

        private void SafeShutdown()
        {
            if (_disposed) return;                 // already done once
            _disposed = true;

            // 1) stop timers / background work
            _usbDebounceTimer?.Stop();
            _usbDebounceTimer?.Dispose();

            // 2) ensure critical outputs are safe BEFORE we drop the handle
            try { gpio_control?.Write(GPIO3, false); }
            catch { /* ignore if USB just vanished */ }

            // 3) dispose high-level chip helpers (they only reference spi/i2c)
            ad4368?.Dispose();
            ad9175?.Dispose();
            fpga?.Dispose();
            IO_Exp?.Dispose(); 
            MUX?.Dispose(); //snow evb

            // 4) dispose the transports
            ftDev?.Dispose();          // SPI (interface-A)
            i2cBus?.Dispose();         // I2C (interface-A) 
            gpio_control?.Dispose();   // GPIO (interface-B)

            //InterfaceManager.Dispose();
            // 5) close the app – use Exit() so Application.Run() unwinds cleanly
            Application.Exit();
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

        private void Cmd_Exit_Click(object sender, EventArgs e) => SafeShutdown();        

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SafeShutdown();
            base.OnFormClosing(e);
        }

        // Validation of Hex register value, return ushort value
        private static bool TryParseHexU16(string input, out ushort value)
        {
            value = 0;

            if (!HexU16Pattern.IsMatch(input?.Trim() ?? string.Empty))
                return false;

            // Use Substring instead of the range operator
            string hexPart = input.Substring(4);
            return ushort.TryParse(hexPart, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            
        }
        
        // Validation of correct HEX value, return int value
        private bool IsHexString(string input)
        {
            if (!HexBytePattern.IsMatch(input?.Trim() ?? string.Empty))
                return false;
                        
            input = input.Substring(2); // Remove "0x" prefix
            return int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        }
        
        // Validation of correct HEX value, return byte value
        private static bool TryParseHexByte(string input, out byte value)
        {
            value = 0;

            // Replace the range operator with a substring method for compatibility with C# 7.3
            if (!HexBytePattern.IsMatch(input?.Trim() ?? string.Empty))
                return false;

            // Use Substring instead of the range operator
            string hexPart = input.Substring(2);
            return byte.TryParse(hexPart, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }
        private bool IsHexString4bytes(string input)
        {

            // Replace the range operator with a substring method for compatibility with C# 7.3
            if (!HexU32Pattern.IsMatch(input?.Trim() ?? string.Empty))
                return false;

            input = input.Substring(8); // Remove "0x" prefix
            return ulong.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        }

        private void comboRegAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRegisterAddress = comboRegAddress.SelectedItem.ToString();
            int selectedHex = Convert.ToInt32(selectedRegisterAddress.Substring(2), 16); // Convert hex string to int

            // Disable the button for values 0x0002 to 0x000D
            if (driverflag)
            {
                Cmd_WriteReg_AD4368.Enabled = !((selectedHex >= 0x0002 && selectedHex <= 0x000D) || (selectedHex >= 0x0054 && selectedHex <= 0x0063));
                textAD4368_Value.Enabled = !((selectedHex >= 0x0002 && selectedHex <= 0x000D) || (selectedHex >= 0x0054 && selectedHex <= 0x0063));
            }
            
            
            if (driverflag && usbflag)
            {
                byte valbyte = ad4368.ReadRegister((ushort)selectedHex);                
                textAD4368_Value.Text = $"0x{valbyte:X2}";
            }

            textAD4368_Value.Focus();
        }

        private void Cmd_WriteReg_AD4368_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD4368)
            {
                if (!string.IsNullOrWhiteSpace(textAD4368_Value.Text))
                {
                    string regaddress = comboRegAddress.SelectedItem?.ToString()?.Trim(); // Get selected value as string

                    string dataRaw = textAD4368_Value.Text.Trim();

                    if (!TryParseHexU16(regaddress, out ushort regValue))
                    {
                        MessageBox.Show("Register address must be in 0xXXXX format (e.g. 0x002B).",
                                        "Invalid address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        comboRegAddress.Focus();
                        return;
                    }

                    if (!TryParseHexByte(dataRaw, out byte dataByte))
                    {
                        MessageBox.Show("Data value must be in 0xXX format (00 - FF).",
                                        "Invalid data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textAD4368_Value.Focus();
                        textAD4368_Value.Clear();
                        return;
                    }

                    ad4368.WriteRegister(regValue, dataByte);

                    byte powerAddress = (byte)regValue;
                    // Check if the register address is the power register
                    if (powerAddress == 0x2B)
                        CheckPowerRegister(powerAddress);
                }
                else
                {
                    MessageBox.Show("The textbox value is empty or wrong!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textAD4368_Value.Focus();
                    textAD4368_Value.Clear();
                }
            }            
        }        

        private void Cmd_AD4368_INIT_Click(object sender, EventArgs e)
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
            //ushort paddrress = Convert.ToUInt16(address.ToString("X4"), 16);

            byte powerreturn = ad4368.ReadRegister((ushort)address);

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
            if (selectedTab == tabAD4368)
            {
                int index = 1;

                if (ftDev == null)
                {
                    MessageBox.Show("SPI interface not initialized. Please reconnect the FTDI device.", "No SPI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!(usbflag && driverflag)) return; // USB or driver not ready – silently exit


                if (dataGridViewAD4368.Rows.Count > 0)
                {
                    dataGridViewAD4368.DataSource = null;
                    DT4368.Clear();
                    dataGridViewAD4368.DataSource = DT4368;
                }
                else
                {
                    foreach (var item in comboRegAddress.Items)
                    {
                        string raw = item?.ToString()?.Trim() ?? string.Empty;

                        if (!TryParseHexU16(raw, out ushort regValue))
                        {
                            MessageBox.Show($"Register '{raw}' is not in 0xXXXX format (e.g. 0x002B). " +
                                            "Item skipped.", "Invalid address",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        byte dataByte;
                        try
                        {
                            dataByte = ad4368.ReadRegister(regValue);

                            // Add to DataTable
                            DataRow row = DT4368.NewRow();
                            row["Index"] = index++;
                            row["Register"] = raw;                   // already validated
                            row["Value"] = $"0x{dataByte:X2}";
                            row["Value byte"] = dataByte;
                            DT4368.Rows.Add(row);

                            // Extra handling for power register (low byte 0x2B)
                            if ((byte)regValue == 0x2B)
                                CheckPowerRegister(0x2B);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"ReadRegister(0x{regValue:X4}) failed:\n{ex.Message}",
                                            "Read error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                    }
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
            if (selectedTab == tabAD9175)
            {
                labelFilePath9175.Text = $"DAC File Path: {ad9175.LoadDataTableToCsv()}";
            }            
        }

        private void Cmd_Export9175_file_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                ad9175.SaveDataTableToCsv(DT9175);
            }            
        }               

        // Set Control Enabled via USB connection status
        private void SetControlsEnabled(bool enabled)
        {
            if (!enabled)
            {
                tabControl1.Enabled = false;

                //Cmd_Init_All.Enabled = false;
                Cmd_FT_Temp_Read.Enabled = false;
                Cmd_RF_Temp_Read.Enabled=false;
                //Cmd_AD4368_INIT.Enabled = false;
            }
            else
            {
                tabControl1.Enabled = true;
                Cmd_Init_All.Enabled = true;
                Cmd_FT_Temp_Read.Enabled = true;
                Cmd_RF_Temp_Read.Enabled = true;
                comboMUXOUT.Enabled = false;
                comboRegAddress.Enabled = false;
                Cmd_WriteReg_AD4368.Enabled = false;
                Cmd_Import_AD4368_File.Enabled = false;
                textAD4368_Value.Enabled = false;
                
            }
        }

        public void Control_Init_PLL(bool initflag)
        {
            if (!initflag)
            {
                textAD4368_Value.Enabled = false;
                comboRegAddress.Enabled = false;
                comboMUXOUT.Enabled = false;
                Cmd_Import_AD4368_File.Enabled = false;
            }
            else
            {
                textAD4368_Value.Enabled = true;
                comboRegAddress.Enabled = true;
                comboMUXOUT.Enabled = true;
                Cmd_Import_AD4368_File.Enabled = true;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTab = tabControl1.SelectedTab;

            if (selectedTab == tabRFLine)
            {
                if (!TXline_flag)
                {
                    i2cBus = InterfaceManager.GetI2c();
                    
                    checkAmp1.Checked = txLineData.bypass1; //bypass mode - false,   AMP mode - true
                    checkAmp2.Checked = txLineData.bypass2; //bypass mode - false,   AMP mode - true
                    numericATT1.Value = Convert.ToDecimal(txLineData.att1);
                    numericATT2.Value = Convert.ToDecimal(txLineData.att2);
                    numericATT3.Value = Convert.ToDecimal(txLineData.att3);
                }
            }
            else if (selectedTab == tabFPGA)
            {
                fpga = new FPGA();
                fpga.Init(ftDev); // Initialize FPGA with the current FTDI device
                textFPGA_Address.Focus();
            }
            else if (selectedTab == tabAD9175)
            {
                //todo: check if the DAC is initialized and if not - initialize it
                //i2cBus = InterfaceManager.GetI2c(); // Get current I²C interface
                //IO_Exp.SetPinState();
                ad9175 = new AD9175_DAC();
                ftDev = InterfaceManager.GetSpi();
                ad9175.Init(ftDev);
                //DAC0_freq = txLineData.nco_dac0 * 1e9;
                //DAC1_freq = txLineData.nco_dac1 * 1e9;
                ad9175.Init(ftDev);

                comboRegisters9175.Focus();
            }
            else if (selectedTab == tabAD4368)
            {
                PLL_Init_Flag = true; //TODO test QA - set this flag to true if you want to reinitialize AD4368 on tab change
                if (PLL_Init_Flag)
                {
                    Control_Init_PLL(PLL_Init_Flag);
                    ad4368 = new AD4368_PLL();
                    ftDev = InterfaceManager.GetSpi(); // Get current SPI interface
                    ad4368.Init(ftDev); // Initialize AD4368 with the current FTDI device


                    DT4368 = ad4368.InitDataTable();
                    dataGridViewAD4368.DataSource = DT4368;
                    comboRegAddress.DataSource = ad4368.LoadComboRegisters();
                    LogStatus("AD4368 reinitialized on SPI CS1");
                    Cmd_ReadAll_AD4368.Enabled = true;
                    PLL_Init_Flag = false; // Reset the flag after initialization
                }
                else
                {
                    ftDev = InterfaceManager.GetSpi(); // Get current SPI interface
                    ad4368.Init(ftDev); // Initialize AD4368 with the current FTDI device
                }


                /*
                comboRegAddress.Focus();
                ftDev = InterfaceManager.GetSpi(); // Get current SPI interface
                ad4368.Init(ftDev);*/
            }
            else if (selectedTab == tabMux)
            {

                i2cBus = InterfaceManager.GetI2c();

                IO_Exp = new PCAL6416A();
                IO_Exp.Init(i2cBus);

                MUX = new PCA9547A();
                MUX.Init(i2cBus); // Initialize MUX with the current I²C device

                MUX.Set_Mux_Channel(1, 5);

            }
            else if (selectedTab == tabSi5518)
            {
                // Initialize Si5518 if needed

            }
        }

        // textBox for PLL4368 specific register value update, after pressing Enter focus will change to Write Register button
        private void textAD4368_Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedTab == tabAD4368)
            {
                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textAD4368_Value.Text))
                {
                    // Validate Hex value entered in this field
                    if (IsHexString(textAD4368_Value.Text))
                    {
                        Cmd_WriteReg_AD4368.Focus();
                    }
                    else
                    {
                        textAD4368_Value.Clear();
                        textAD4368_Value.Focus();
                    }
                }
            }
            
        }

        private void Cmd_Import_AD4368_File_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD4368)
            {
                labelFilePathAD4368.Text = $"File Path: {ad4368.LoadDataTableToCsv()}";
                Cmd_WriteAll_AD4368.Enabled = true;
                Cmd_ReadAll_AD4368.Enabled = true;
            }            
        }

        private void Cmd_Export_AD4368_File_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD4368)
                ad4368.SaveDataTableToCsv(DT4368);
        }

        private void Cmd_WriteAll_AD4368_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD4368)
            {
                var filteredRows = DT4368.AsEnumerable().Where(row =>
                {
                    string hexStr = row["Register"].ToString();      // e.g. "0x0053"
                    ushort reg = Convert.ToUInt16(hexStr.Substring(2), 16); // Convert to int
                    return reg >= 0x10 && reg <= 0x53;
                }).OrderByDescending(row => Convert.ToUInt16(row["Register"].ToString().Substring(2), 16) // Sort descending
                );

                foreach (var row in filteredRows)
                {

                    byte paddress = Convert.ToByte(row["Register"].ToString().Replace("0x", ""), 16);
                    ushort regValue = Convert.ToUInt16(row["Register"].ToString().Replace("0x", ""), 16);
                    byte databyte = Convert.ToByte(row["Value"].ToString().Replace("0x", ""), 16);

                    ad4368.WriteRegister(regValue, databyte);

                    if (paddress == 0x2B)
                    {
                        CheckPowerRegister(paddress);
                    }
                }
            }            
        }

        private void Cmd_Init_All_Click(object sender, EventArgs e)
        {
            IO_Exp = new PCAL6416A();
            IO_Exp.Init(i2cBus); // Initialize IO Expander with the current I²C device
            MUX = new PCA9547A();
            MUX.Init(i2cBus); // Initialize MUX with the current I²C device
            MUX.Set_Mux_Channel(1, 5);


            tmp100 = new TMP100();
            tmp100.Init(i2cBus); // Initialize TMP100 with the current I²C device
            tmp100.BultInTest(TMP100.AddressIndex.TMP100_FTDI_CHIP); // Self test the FTDI TMP100
            tmp100.BultInTest(TMP100.AddressIndex.TMP100_RF_CHIP); // Self test the RF TMP100
            tmp100.Config(TMP100.AddressIndex.TMP100_FTDI_CHIP); // Configure the FTDI TMP100
            tmp100.Config(TMP100.AddressIndex.TMP100_RF_CHIP); // Configure the RF TMP100
            
        }
        

        private void Cmd_DAC_Init_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabAD9175)
            {
                if (ad9175 != null)
                {
                    //ad9175 = new AD9175_DAC();
                    //ad9175.Init(ftDev); // Initialize DAC9175 with the current FTDI device


                    DT9175 = ad9175.InitDataTableDAC();
                    dataGridViewAD4368.DataSource = DT9175;
                    comboRegisters9175.DataSource = ad9175.LoadComboRegister9175();
                    LogStatus("AD9175 reinitialized on SPI CS1");
                    Cmd_ReadDAC9175.Enabled = true;


                    // Fix for CS0815: Cannot assign void to an implicitly-typed variable
                    // The method `DAC9175_InitEngine` returns void, so it cannot be assigned to a variable.
                    // To fix this, simply call the method without attempting to assign its return value.

                    ad9175.DAC9175_InitEngine(dac_ini);

                    int code = -1;

                    try
                    {
                        code = ad9175.RUN_CSV(); // Run the CSV initialization for DAC9175
                    }
                    catch (Exception ex)
                    {
                        code = ex.HResult; // Get the error code from the exception
                        LogStatus($"DAC9175 initialization failed with error code: {code}");
                        MessageBox.Show($"DAC9175 initialization failed with error code: {code}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    /* Iinit of the DAC
                    ad9175.PowerUp();
                    ad9175.DAC_PLL_Config();

                    if (ad9175.GetBit(ad9175.DelayLockLoop(), 0))
                    {
                        // TODO : Add a message box or log to indicate that the DLL is locked
                        // True - DAC DLL is locked
                    }
                    else
                    {
                        // TODO : Add a message box or log to indicate that the DLL is not locked
                        // False - DAC DLL is not locked
                    }
                    ad9175.Calibration();

                    ad9175.JESD204B_Setup();

                    //TODO : TEST IT 
                    DAC0 = ad9175.GetBytes48BitBigEndian(ad9175.CalculateDdsmFtw(DAC0_freq));   //2 GHz
                    DAC1 = ad9175.GetBytes48BitBigEndian(ad9175.CalculateDdsmFtw(DAC1_freq));   //3 Ghz
                    ad9175.MainDAC_Datapath_Setup(DAC0, DAC1);

                    ad9175.JESD204B_SERDES_Setup();
                    ad9175.TransportLayer_Setup();
                    //when to implement it
                    ad9175.CleanUpRegisterList();
                }*/
                }
            }
        }

        private void comboRegisters9175_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRegisterAddress9175 = comboRegisters9175.SelectedItem.ToString();
            int selectedHex = Convert.ToInt32(selectedRegisterAddress9175.Substring(2), 16); // Convert hex string to int

            // Disable the button for values 0x0002 to 0x000D
            if (driverflag)
            {
                //Cmd_WriteReg9175.Enabled = !((selectedHex >= 0x0002 && selectedHex <= 0x000D) || (selectedHex >= 0x0054 && selectedHex <= 0x0063));
                //textDAC9175_Value.Enabled = !((selectedHex >= 0x0002 && selectedHex <= 0x000D) || (selectedHex >= 0x0054 && selectedHex <= 0x0063));
            }


            if (driverflag && usbflag)
            {
                byte valbyte = ad9175.ReadRegister((ushort)selectedHex);
                //byte valbyte = ad4368.ReadWrite((ushort)selectedHex);
                textDAC9175_Value.Text = $"0x{valbyte:X2}";
            }

            textDAC9175_Value.Focus();
        }

        private void Cmd_FT_Temp_Read_Click(object sender, EventArgs e)
        {
            if (i2cBus == null)
            {
                //MessageBox.Show("I2C bus is not initialized. Please check the FTDI connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
                i2cBus = InterfaceManager.GetI2c(); // Get current I²C interface
                if (tmp100 == null)
                {
                    tmp100 = new TMP100();
                    tmp100.Init(i2cBus); // Initialize TMP100 with the current I²C device
                    tmp100.Config(TMP100.AddressIndex.TMP100_FTDI_CHIP); // Configure the FTDI TMP100
                }

                try
                {
                    double tempval = tmp100.ReadTemperature(TMP100.AddressIndex.TMP100_FTDI_CHIP);
                    label3.Text = $"{tempval:F1} °C"; // Format the temperature
                    LogStatus("Temp FT: " + label3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to read temperature: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                i2cBus = InterfaceManager.GetI2c(); // Get current I²C interface
                if (tmp100 == null)
                {
                    tmp100 = new TMP100();
                    tmp100.Init(i2cBus); // Initialize TMP100 with the current I²C device
                    tmp100.Config(TMP100.AddressIndex.TMP100_FTDI_CHIP); // Configure the FTDI TMP100
                }

                try
                {
                    double tempval = tmp100.ReadTemperature(TMP100.AddressIndex.TMP100_FTDI_CHIP);
                    label3.Text = $"{tempval:F1} °C"; // Format the temperature
                    LogStatus("Temp FT: " + label3.Text);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to read temperature: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }           

        }

        private void Cmd_RF_Temp_Read_Click(object sender, EventArgs e)
        {
            if (i2cBus == null)
            {
                MessageBox.Show("I2C bus is not initialized. Please check the FTDI connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tmp100 == null)
            {
                tmp100 = new TMP100();
                tmp100.Init(i2cBus); // Initialize TMP100 with the current I²C device
                tmp100.Config(TMP100.AddressIndex.TMP100_RF_CHIP); // Configure the RF TMP100
            }

            try
            {
                double tempval = tmp100.ReadTemperature(TMP100.AddressIndex.TMP100_RF_CHIP);
                label5.Text = $"{tempval:F1} °C"; // Format the temperature
                LogStatus("Temp RF: "+ label5.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read temperature: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }       
             

        private void Cmd_Read_ADC_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                if (i2cBus == null)
                {
                    MessageBox.Show("I2C bus is not initialized. Please check the FTDI connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    i2cBus = InterfaceManager.GetI2c(); // Get current I²C interface
                    if (IO_Exp == null)
                    {
                        IO_Exp = new PCAL6416A();
                        IO_Exp.Init(i2cBus); // Initialize IO Expander with the current I²C device
                    }
                    else
                    {
                        IO_Exp.Init(i2cBus); // Re-initialize IO Expander with the current I²C device
                        IO_Exp.PCAL6416A_CONFIG_IO_EXP(6, 0);
                        //IO_Exp.ChipSelect_IO(3, false); // Enable chip select for PCAL6416A
                        IO_Exp.SetPinsFromValue(3, false);
                        
                        string hexval = string.Empty;
                        string voltage = string.Empty;

                        label7.Text = $"Hex value: 0x{hexval:X3} Decimal Voltage value: {voltage} V";

                        
                    }
                }

            }
            
        }
        
        public static byte ToByte(float valueDb)
        {
            const float step = 0.25f;   // ¼-dB resolution
            const float min = 0.0f;    // bottom of table
            const float max = 31.75f;  // top of table
            const int mask = 0x7F;   // keep only 7 bits

            // --- Clamp ----------------------------------------------------------
            if (valueDb < min) valueDb = min;
            else if (valueDb > max) valueDb = max;
            // --------------------------------------------------------------------

            // Convert dB → code units byte and round to nearest
            int code = (int)Math.Round(valueDb / step, MidpointRounding.AwayFromZero);

            return (byte)(code & mask);
        }
        
        public static string ToHex(float valueDb) => $"0x{ToByte(valueDb):X2}";
        
        private void Cmd_FPGA_Write_Click(object sender, EventArgs e)
        {
            
            if (selectedTab == tabFPGA)
            {
                try 
                {                    
                    fpga.SpiWrite(AlignmentRegister(HexStringToUInt(fpga_address)), HexStringToUInt(fpga_data));
                    LogStatus($"The register address {fpga_address} passed value {fpga_data} to FPGA");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to write to FPGA: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogStatus("Writing to FPGA Caused an ERROR!!!");
                }
               
            }            
        }

        private void Cmd_FPGA_Read_Click(object sender, EventArgs e)
        {
            
            if (selectedTab == tabFPGA)
            {
                try
                {
                    uint addr = HexStringToUInt(fpga_address);
                    uint retval = fpga.SpiRead(AlignmentRegister(HexStringToUInt(fpga_address)));                    
                    LogStatus($"The register address {fpga_address} gets value [0x{retval:X8}] from FPGA");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to write to FPGA: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogStatus("Writing to FPGA Caused an ERROR!!!");
                }
            }
        }

        public static uint HexStringToUInt(string hex)
        {
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);

            return Convert.ToUInt32(hex, 16);
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);

            return Enumerable.Range(0, hex.Length / 2)
                .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
                .ToArray();
        }
        
        public static byte[] BuildSpiWriteBuffer(byte[] addressBytes, byte[] valueBytes)
        {
            if (addressBytes.Length != 4 || valueBytes.Length != 4)
                throw new ArgumentException("Both address and value must be exactly 4 bytes.");

            byte[] buffer = new byte[11];

            buffer[0] = 0x00; // Write command

            Array.Copy(addressBytes, 0, buffer, 1, 4); // Copy address to buffer[1–4]
            Array.Copy(valueBytes, 0, buffer, 5, 4);   // Copy value to buffer[5–8]

            buffer[9] = 0x00; // Don't care
            buffer[10] = 0x00;

            return buffer;
        }

        public static byte[] BuildSpiReadBuffer(byte[] addressBytes)
        {
            if (addressBytes.Length != 4)
                throw new ArgumentException("Regiser address must be exactly 4 bytes.");

            byte[] buffer = new byte[11];
            
            buffer[0] = 0x01; // Read command
            buffer[5] = 0x00; // Don't care
            buffer[6] = 0x00; // Don't care   
            buffer[7] = 0x00; // Don't care
            buffer[8] = 0x00; // Don't care

            Array.Copy(addressBytes, 0, buffer, 1, 4); // Copy address to buffer[1–4]
            
            buffer[9] = 0x00; // Don't care
            buffer[10] = 0x00;

            return buffer;
        }

        private void textFPGA_Address_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedTab == tabFPGA)
            {
                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textFPGA_Address.Text))
                {
                    // Validate Hex value entered in this field
                    if (IsHexString4bytes(textFPGA_Address.Text))
                    {
                        fpga_address = textFPGA_Address.Text;
                        textFPGA_Value.Focus();
                    }
                    else
                    {
                        textFPGA_Address.Clear();
                        textFPGA_Address.Focus();
                        fpga_address = string.Empty;
                        MessageBox.Show("The register address is not correct!", "Warning");
                    }
                }
            }            
        }

        private void textFPGA_Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedTab == tabFPGA)
            {
                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textFPGA_Value.Text))
                {
                    if (IsHexString4bytes(textFPGA_Value.Text))
                    {
                        fpga_data = textFPGA_Value.Text;
                        Cmd_FPGA_Write.Focus();
                    }
                    else
                    {
                        textFPGA_Value.Clear();
                        textFPGA_Value.Focus();
                        fpga_data = string.Empty;
                        MessageBox.Show("The register value is not correct!", "Warning");                       
                        
                    }
                }                
            }
        }

        private void Cmd_WriteReg9175_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD9175)
            {

            }

        }

        private void Cmd_LoadCounter_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabFPGA)
            {

                string startaddress = "0x00001000";
                string stopaddress = "0x00001020";
                Load_FGPA_Register(HexStringToUInt(startaddress), HexStringToUInt(stopaddress));
            }            
        }

        private void LogStatusFPGA(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string fullMessage = $"[{timestamp}] {message}{Environment.NewLine}";
            //textFPGA_Output.AppendText($"[{timestamp}] {message}{Environment.NewLine}");

            if (textFPGA_Output.InvokeRequired)
            {
                textFPGA_Output.BeginInvoke(new Action(() =>
                {
                    textFPGA_Output.AppendText(fullMessage);
                }));
            }
            else
            {
                textFPGA_Output.AppendText(fullMessage);
            }
        }

        // Import FPGA file with relevant data
        private void Cmd_FPGA_Import_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabFPGA)
            {
                // TEST function now
                TestRegister();
            }
            
        }

        // Writes counter values to the FPGA registers in the specified range.
        public void Load_FGPA_Register(uint StartAddress, uint StopAddress)
        {
            if (StartAddress == 0 || StopAddress == 0)
                throw new ArgumentException("Start Address and Stop Address must be not equal to zero!!!");

            if (StartAddress > StopAddress)
                throw new ArgumentException("Start Address must be less or equal to Stop Address!!!");

            uint counternumber = 0x00000000;

            for (uint addr = StartAddress; addr < StopAddress; addr +=4)
            {
                
                fpga.SpiWrite(addr, counternumber);
                LogStatusFPGA($"The FPGA register address 0x{addr:X8} received value [0x{counternumber:X8}]");
                Thread.Sleep(10);
                uint returnvalue = fpga.SpiRead(addr);
                LogStatusFPGA($"The FPGA register address 0x{addr:X8} READ OK");
                counternumber++;
                Thread.Sleep(50);
                
            }
            counternumber = 0x00000000;
        }

        // Reads register values from the FPGA registers in the specified range.
        public void Read_FPGA_Registers(uint StartAddress, uint StopAddress)
        {
            if (StartAddress == 0 || StopAddress == 0)
                throw new ArgumentException("Start Address and Stop Address must be not equal to zero!!!");

            if (StartAddress > StopAddress)
                throw new ArgumentException("Start Address must be less or equal to Stop Address!!!");

            for (uint addr = StartAddress; addr < StopAddress; addr +=4)
            {
                uint receiveddata = fpga.SpiRead(addr);
                LogStatusFPGA($"The FPGA register address 0x{addr:X8} has value [0x{receiveddata:X8}]");
            }

        }

        private void Cmd_Read_Registers_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabFPGA)
            {
                string startaddress = "0x00001000";
                string stopaaddress = "0x00001020";
                Read_FPGA_Registers(HexStringToUInt(startaddress), HexStringToUInt(stopaaddress));
            }

        }

        //Alignment Register Map 32-bit word
        private uint AlignmentRegister(uint alignaddress)
        {
            return alignaddress & 0xFFFFFFFC;
        }

        

        // test fpga register logics
        private void TestRegister()
        {
            uint startaddress = 0x00001000;
            uint stopaaddress = 0x00001FFF;
            uint prevAlignAddress = 0xFFFFFFFF;

            uint counter = 0x0;
            for (uint addr = startaddress; addr < stopaaddress; addr++)
            {
                uint alignaddress = addr & 0xFFFFFFFC;
               
                if (alignaddress != prevAlignAddress)
                {
                    LogStatusFPGA($"The FPGA register address 0x{alignaddress:X8} received value [0x{counter++:X8}]");                    
                    //counter++;
                    prevAlignAddress = alignaddress;
                }
                else
                {
                    LogStatusFPGA($"FGPA Alignment register address [0x{alignaddress:X8}] of regular address 0x{addr:X8}");
                }                
            }            
        }

        private void Cmd_Led_Test_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabMux)
            {

                MUX.Set_Mux_Channel(1, 7); // Set MUX channel 1 to 7 (for example, you can change this as needed)
                MUX.Set_Mux_Channel(0, 7); // Set MUX channel 0 to 7 (for example, you can change this as needed)
                // Test the LED functionality of the PCAL6416A I/O expander
                IO_Exp.PCAL6416A_CONFIG_IO_EXP(6, 0);
                IO_Exp.Led_Test();
            }
        }

        private void Cmd_Led_ON_Click(object sender, EventArgs e)
        {
            IO_Exp.WriteByte(0x02, 0x00); //ALL LEDS ON            
        }

        private void Cmd_Led_OFF_Click(object sender, EventArgs e)
        {
            IO_Exp.WriteByte(0x02, 0xFF); //ALL LEDS OFF
        }

        private void comboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboDevice.SelectedItem.ToString() == "PCAL6416A")
            {
                MUX.Set_Mux_Channel(1, 7); // Set MUX channel 1 to 7 (for example, you can change this as needed)
                MUX.Set_Mux_Channel(0, 7); // Set MUX channel 0 to 7 (for example, you can change this as needed)
                // Test the LED functionality of the PCAL6416A I/O expander
                IO_Exp.PCAL6416A_CONFIG_IO_EXP(6, 0);
                //IO_Exp.ChipSelect_IO(3, false); // Enable chip select for PCAL6416A
                IO_Exp.SetPinsFromValue(3, false);
                //IO_Exp.ChipSelect_IO(3, true); // Disable chip select for PCAL6416A

                IO_Exp.SetPinsFromValue(34, false); 

                IO_Exp.SetPinsFromValue(255, true);
            }
        }

        private void Cmd_FPGA_Export_Click(object sender, EventArgs e)
        {

        }

        private void Cmd_UpdateTX_Values_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                att1_value = (float)numericATT1.Value;
                att2_value = (float)numericATT2.Value;
                att3_value = (float)numericATT3.Value;

                if (i2cBus != null)
                {
                    // Initialize IO Expander if not already initialized
                    i2cBus = InterfaceManager.GetI2c(); // Get current I2C interface
                    IO_Exp.Init(i2cBus); // Re-initialize IO Expander with the current I2C device
                    // First, make sure the IO Expander CTRL_SPI_EN_1V8 is low to enable SPI communication to the HMC1119!
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, false);
                    //IO_Exp.SetPinState(0x04, false);
                    // Enable chip select of HMC1119 chip 1
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE1, false);

                    // Instantiate HMC1119 object for further operations
                    hmc1119 = new HMC1119();

                    // Write values to SERIN lines of HMC1119
                    ftDev = InterfaceManager.GetSpi(); // Get current SPI interface
                    hmc1119.Init(ftDev); // Initialize HMC1119 with the current I2C device
                    hmc1119.SetAttenuation(HMC1119.ChipIndex.HMC1119_CHIP1, att1_value);

                    i2cBus = InterfaceManager.GetI2c(); // Get current I²C interface
                    IO_Exp.Init(i2cBus);
                    // Latch the attenuation value to the HMC1119 chip 1
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE1, true);

                    // Enable chip select of HMC1119 chip 2
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE2, false);

                    // Write values to SERIN lines of HMC1119                    
                    ftDev = InterfaceManager.GetSpi(); // Get current SPI interface
                    hmc1119.Init(ftDev); // Initialize HMC1119 with the current I2C device
                    hmc1119.SetAttenuation(HMC1119.ChipIndex.HMC1119_CHIP2, att2_value);

                    i2cBus = InterfaceManager.GetI2c(); // Get current I2C interface
                    IO_Exp.Init(i2cBus);
                    // Latch the attenuation value to the HMC1119 chip 2
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE2, true);

                    // Enable chip select of HMC1119 chip 3
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE3, false);

                    // Write values to SERIN lines of HMC1119                    
                    ftDev = InterfaceManager.GetSpi(); // Get current SPI interface
                    hmc1119.Init(ftDev); // Initialize HMC1119 with the current I2C device
                    hmc1119.SetAttenuation(HMC1119.ChipIndex.HMC1119_CHIP3, att3_value);

                    i2cBus = InterfaceManager.GetI2c(); // Get current I2C interface
                    IO_Exp.Init(i2cBus);
                    // Latch the attenuation value to the HMC1119 chip 3
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE3, true);
                }

            }
        }

        private void numericATT1_ValueChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                if (numericATT1.Value == numericATT1.Maximum)
                {
                    MessageBox.Show("Status: MAX VALUE REACHED!","Warning");                   
                }
                else
                {
                    //att1_value = ToByte((float)numericATT1.Value);
                    att1_value = (float)numericATT1.Value;
                }
            }
        }

        private void numericATT2_ValueChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                if (numericATT2.Value == numericATT2.Maximum)
                {
                    MessageBox.Show("Status: MAX VALUE REACHED!", "Warning");
                }
                else
                {
                    //att2_value = ToByte((float)numericATT2.Value);
                    att2_value = (float)numericATT2.Value;
                }
            }
        }

        private void numericATT3_ValueChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                if (numericATT3.Value == numericATT3.Maximum)
                {
                    MessageBox.Show("Status: MAX VALUE REACHED!", "Warning");
                }
                else
                {
                    //att1_value = ToByte((float)numericATT3.Value);
                    att3_value = (float)numericATT3.Value;
                }
            }
        }

        private void checkAmp1_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                //MUX.Set_Mux_Channel(1, 7); // Set MUX channel 1 to 7 (for example, you can change this as needed)
                //MUX.Set_Mux_Channel(0, 7); // Set MUX channel 0 to 7 (for example, you can change this as needed)
                
                if (IO_Exp == null)
                { 
                    IO_Exp = new PCAL6416A();
                    IO_Exp.Init(i2cBus); // Initialize IO Expander with the current I²C device
                    IO_Exp.PCAL6416A_CONFIG_IO_EXP(6, 0);
                }

                
                if (checkAmp1.CheckState == CheckState.Checked)
                {
                    txLineData.bypass1 = true; // BYPASS ON
                    checkAmp1.Text = "BYPASS MODE";
                    IO_Exp.SetPinState(6, false); 
                }
                else
                {
                    txLineData.bypass1 = false; // BYPASS OFF (AMP ON)
                    checkAmp1.Text = "AMP ON";
                    IO_Exp.SetPinState(6, true);
                }

                LogStatus($"Amplifier 1 Status: {checkAmp1.Text}");
            }
            
        }

        private void checkAmp2_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                //MUX.Set_Mux_Channel(1, 7); // Set MUX channel 1 to 7 (for example, you can change this as needed)
                // MUX.Set_Mux_Channel(0, 7);
                
                if (IO_Exp == null)
                {
                    IO_Exp = new PCAL6416A();
                    IO_Exp.Init(i2cBus); // Initialize IO Expander with the current I²C device
                    IO_Exp.PCAL6416A_CONFIG_IO_EXP(6, 0);
                }

                if (checkAmp2.CheckState == CheckState.Checked)
                {
                    txLineData.bypass2 = true; // BYPASS ON
                    checkAmp2.Text = "BYPASS MODE";
                    IO_Exp.SetPinState(7, false);
                }
                else
                {
                    txLineData.bypass2 = false; // BYPASS OFF (AMP ON)
                    checkAmp2.Text = "AMP ON";
                    IO_Exp.SetPinState(7, true);
                }

                LogStatus($"Amplifier 2 Status: { checkAmp2.Text}");
            }
            
        }

        private void numericATT1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                decimal rounded = RoundToStep(numericATT1.Value, numericATT1.Increment);
                if (rounded != numericATT1.Value)
                {
                    numericATT1.Value = rounded;

                    att1_value = ToByte((float)numericATT1.Value);  //integer value example 11.75 dB = 10 1111
                }
            }
        }

        private decimal RoundToStep(decimal value, decimal step)
        {
            return Math.Round(value / step, MidpointRounding.AwayFromZero) * numericATT1.Increment;
        }

        private void numericATT2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                decimal rounded = RoundToStep(numericATT2.Value, numericATT2.Increment);
                if (rounded != numericATT2.Value)
                {
                    numericATT2.Value = rounded;

                    att2_value = ToByte((float)numericATT2.Value);  //integer value example 11.75 dB = 10 1111
                }
            }
        }

        private void numericATT3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                decimal rounded = RoundToStep(numericATT3.Value, numericATT3.Increment);
                if (rounded != numericATT3.Value)
                {
                    numericATT3.Value = rounded;

                    att3_value = ToByte((float)numericATT3.Value);  //integer value example 11.75 dB = 10 1111
                }
            }
        }
    }
}



