using FTD2XX_NET;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BringUp_Control.AD9175_DAC;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;


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

        private const int RF_PLL_POWER_REG = 0x002B;
        private const int RF_PLL_LKDET_REG = 0x0058;


        // Define register addresses
        private const int REG_SHORT_TPL_TEST_0 = 0x032C; // SHORT_TPL_CHAN_SEL - bits [3:2] - channel, SHORT_TPL_SP_SEL - bits [7:4] - select sample, SHORT_TPL_TEST_EN -bit 0, SHORT_TPL_TEST_RESET - bit 1[0/1]
        private const int REG_SHORT_TPL_TEST_1 = 0x032D; // SHORT_TPL_REF_SP_LSB
        private const int REG_SHORT_TPL_TEST_2 = 0x032E; // SHORT_TPL_REF_SP_MSB
        private const int REG_SHORT_TPL_TEST_3 = 0x032F; // SHORT_TPL_LINK_SEL bit 7 DAC0/DAC1, SHORT_TPL_IQ_PATH_SEL - bit 6 IQ stream


        private CancellationTokenSource _playbackCancelToken;

        private CancellationTokenSource _stplCancelToken;

        public const int GPIO3 = 3;

        string filepath = string.Empty;  // csv file path
        //int usbcounter = 0;

        private bool _usbInitInProgress = false;
        public System.Timers.Timer _usbDebounceTimer;


        string linevaluetx3 = string.Empty;

        Ft4222Native FTDriver = new Ft4222Native();
        FtdiInterfaceManager InterfaceManager; // FTDI SPI interface manager
        FtdiInterfaceManager InterfaceManagerFPGA; // FTDI SPI interface manager for FPGA
                                                   // Regex to match 0x... hex values
        
        string stpl_data = string.Empty; // STPL data 


        DataTable DT4368 = new DataTable();
        DataTable DT9175 = new DataTable();
        DataTable DTFPGA = new DataTable();
        List<string> deviceInfo = new List<string>();

        TabPage selectedTab = new TabPage();

        bool driverflag = false;    // SPI Driver FLAG, FTDI init
        bool usbflag = false;       // USB CONNECTED FLAG

        bool TXline_flag = false;   // TX line up values upload flag
        private bool _disposed;


        float dac_fs_value = 0.0f; // DAC full scale value

        private struct TX_Line
        {
            public bool bypass1;
            public bool bypass2;
            public float att1;
            public float att2;
            public float att3;

        }
        private bool isAD4368GridBound = false;

        private float att1_value = 0.00f;
        private float att2_value = 0.00f;
        private float att3_value = 0.00f;

        private string fpga_address = string.Empty; // FPGA address for register access (string)
        private string fpga_data = string.Empty;    // FPGA data for register access (string)

        private string daq_address = string.Empty;
        private string daq_value = string.Empty;

        private string start_freq = string.Empty; // Start frequency for DAQ
        private string stop_freq = string.Empty;  // Stop frequency for DAQ
        private string step_freq = string.Empty;  // Step frequency for DAQ

        private static readonly Regex HexRegex = new Regex(@"^0x[0-9A-Fa-f]+$", RegexOptions.Compiled);
        private static readonly Regex HexBytePattern = new Regex(@"^0x[0-9A-Fa-f]{2}$");
        private static readonly Regex HexU16Pattern = new Regex(@"^0x[0-9A-Fa-f]{4}$");
        private static readonly Regex HexU32Pattern = new Regex(@"^0x[0-9A-Fa-f]{8}$");

        private uint _spiLocId = UInt32.MaxValue;   // interface-A  (SPI)
        private uint _gpioLocId = UInt32.MaxValue;   // interface-B  (GPIO / future I²C)

        private uint _spiLocId_FPGA = UInt32.MaxValue; // interface-A  (SPI FPGA)

        SpiDriver ftDev;
        SpiDriver ftFPGA; // SPI driver for FPGA
        AD4368_PLL ad4368; // Analog Devices 4368 RF PLL
        AD9175_DAC ad9175; // Analog Devices 9175 DAC
        FPGA fpga; // FPGA interface for register access
        GpioDriver gpio_control;
        i2cDriver i2cBus;
        TMP100 tmp100;   // temperature sensors
        PCAL6416A IO_Exp; // IO expander PCAL6416A
        HMC1119 hmc1119; // RF TX Line Up Attenuators
        HMC8414 hmc8414; // RF TX Line Up Amplifiers
        SI55XX si5518; // Si5518 SkyWorks PLL

        AD7091 ad7091; // ADC for RF Power measurement

        string ftdi_fpganame = string.Empty;
        string nvm_file_pll = string.Empty;
        string config_file_pll = string.Empty;
        string Prod_file_pll = string.Empty;

        string dac_ini_file = string.Empty; // DAC 9175 INI file path
        string daq_reg_file = string.Empty;

        string rf_pll_ini_file = string.Empty; // RF PLL 4368 INI file path

        string jesd_file = string.Empty; // JESD file path

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

            if (!File.Exists(inifilecheck))
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

                        if (configuration["HMC8414:BYPASS_MODE1"] == "0")   //Bypass ON = 0 | AMP ON = 1
                            txLineData.bypass1 = false;
                        else
                            txLineData.bypass1 = true;                      //Bypass ON = 0 | AMP ON = 1 

                        if (configuration["HMC8414:BYPASS_MODE2"] == "0")
                            txLineData.bypass2 = false;
                        else
                            txLineData.bypass2 = true;

                        txLineData.att1 = float.Parse(configuration["HMC1119:ATT1"]);
                        txLineData.att2 = float.Parse(configuration["HMC1119:ATT2"]);
                        txLineData.att3 = float.Parse(configuration["HMC1119:ATT3"]);


                        // set init files for RF PLL and DAC new adaptation
                        rf_pll_ini_file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration["PLL4368:INIT_FILE"]);
                        dac_ini_file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration["DAC9175:INIT_FILE"]);
                        daq_reg_file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration["DAC9175:REG_FILE"]);
                        jesd_file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration["FPGA:JESD204_FILE"]);
                        ftdi_fpganame = configuration["FPGA:FTDI_ID"];

                        if (File.Exists(dac_ini_file))
                        {
                            LogStatus($"DAC 9175 INI file loaded!");
                        }
                        else
                        {
                            LogStatus("DAC 9175 INI file not found or invalid path.");
                            dac_ini_file = string.Empty; // Reset if not valid
                        }

                        if (File.Exists(rf_pll_ini_file))
                        {
                            LogStatus($"RF PLL 4368 INI file loaded!");
                        }
                        else
                        {
                            LogStatus("RF PLL 4368 INI file not found or invalid path.");
                            rf_pll_ini_file = string.Empty; // Reset if not valid
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

                //uint num = 0;
                Ft4222Native.FT_CreateDeviceInfoList(out uint num);

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
                    hmc8414?.Dispose();
                    ad7091?.Dispose();
                    si5518?.Dispose();

                    usbflag = false;
                    driverflag = false;
                    SetControlsEnabled(false);

                    return;
                }
                else
                {
                    uint fpganum = 0;
                    if (num > 2)
                    {
                        fpganum = FTDriver.FindSpiInterfaceLocId(ftdi_fpganame);

                        _spiLocId_FPGA = fpganum; // INTERFACE FT4222 FPGA
                        InterfaceManagerFPGA = new FtdiInterfaceManager(_spiLocId_FPGA); // Initialize FTDI interface manager for FPGA
                        InterfaceManagerFPGA.BusModeChanged += OnBusModeChanged;
                        ftFPGA = InterfaceManagerFPGA.GetSpi();

                    }
                    // FTDI reconnected — reinitialize                  


                    uint locfirst = FTDriver.FindSpiInterfaceLocId("A");
                    uint locsecond = FTDriver.FindSpiInterfaceLocId("B");

                    //uint locfirst = FTDriver.GetDeviceInterfaceSPI("FT4222 A");   //Device A interface for SPI
                    //uint locsecond = FTDriver.GetDeviceInterfaceSPI("FT4222 B");  //Device B interface for GPIO and I2C



                    // ── Guard: if nothing changed we’re already initialised ────────────────────
                    bool locationsUnchanged = (ftDev != null) &&
                                              (gpio_control != null) &&
                                              (_spiLocId == locfirst) &&
                                              (_gpioLocId == locsecond) &&
                                              usbflag && driverflag;

                    if (locationsUnchanged)
                        return;   // avoid handle churn

                    // ── Either first time or the device list really changed ───────────────────
                    _spiLocId = locfirst;    //INTERFACE FT4222 A bringup board
                    _gpioLocId = locsecond;  //INTERFACE FT4222 B bringup board


                    InterfaceManager = new FtdiInterfaceManager(_spiLocId); // Initialize FTDI interface manager
                    InterfaceManager.BusModeChanged += OnBusModeChanged;




                    // ************************* Dispose if already has init ******************************************

                    fpga?.Dispose();
                    ad9175?.Dispose();
                    ad4368?.Dispose();

                    IO_Exp?.Dispose();
                    gpio_control?.Dispose();
                    ad7091?.Dispose();
                    hmc1119?.Dispose();
                    tmp100?.Dispose();
                    si5518?.Dispose();

                    //  *************************** GPIO INIT DRIVER 2 ***********************************************
                    gpio_control = new GpioDriver(_gpioLocId);
                    gpio_control.Write(GPIO3, false);  // GPIO3 is false by default for EVB AD4368 it must be True to enable SPI interface                    


                    ftDev = InterfaceManager.GetSpi();
                    i2cBus = InterfaceManager.GetI2c();


                    // flag status change 
                    usbflag = true;
                    driverflag = true;

                    tmp100 = new TMP100();
                    tmp100.Init(i2cBus, InterfaceManager); // Initialize TMP100 with the current I²C device
                    tmp100.Config(TMP100.AddressIndex.TMP100_FTDI_CHIP); // Configure the FTDI TMP100
                    tmp100.Config(TMP100.AddressIndex.TMP100_RF_CHIP); // Configure the RF TMP100

                    IO_Exp = new PCAL6416A();
                    IO_Exp.Init(i2cBus);
                    IO_Exp.ConfigurePort(PCAL6416A.CONFIG_PORT_0, 0x00); // 00000000b: pins 0-7 outputs
                    IO_Exp.ConfigurePort(PCAL6416A.CONFIG_PORT_1, 0x78); // 01111000b: pins 0-2 outputs, pins 3-6 inputs, pin 7 output


                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, false);

                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_SPI_EN, true);


                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE1, false);
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE2, false);
                    IO_Exp.SetPinStateFromIndex(PCAL6416A.PinIndex.CTRL_HMC1119_LE3, false);


                    // Instantiate HMC1119 object for further operations
                    hmc1119 = new HMC1119();
                    // // Instantiate HMC8414 object for further operations
                    hmc8414 = new HMC8414();
                    hmc8414.Init(i2cBus, IO_Exp, InterfaceManager);

                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP1, txLineData.bypass1);

                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP1, txLineData.bypass2);

                    ad4368 = new AD4368_PLL();
                    ad9175 = new AD9175_DAC();
                    ad7091 = new AD7091();
                    si5518 = new SI55XX();
                    fpga = new FPGA();

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

            si5518?.Dispose();

            // 4) dispose the transports
            ftDev?.Dispose();          // SPI (interface-A)
            i2cBus?.Dispose();         // I2C (interface-A) 
            gpio_control?.Dispose();   // GPIO (interface-B)

            //InterfaceManager.Dispose();
            // 5) close the app – use Exit() so Application.Run() unwinds cleanly
            System.Windows.Forms.Application.Exit();
        }
        /*
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
        }*/

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

        public static ushort HexStringToUShort(string hexString)
        {
            if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hexString = hexString.Substring(2); // remove "0x"

            return Convert.ToUInt16(hexString, 16); // base 16 = hex
        }

        private static bool TryParseHexU16_sec(string input, out ushort value)
        {
            value = 0;

            if (!HexU16Pattern.IsMatch(input?.Trim() ?? string.Empty))
                return false;

            // Use Substring instead of the range operator
            string hexPart = input.Substring(2);
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
        //checks correct format of hex value, return bool
        public bool IsValidHex(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string clean = input.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? input.Substring(2)
                : input;

            // Hex characters only
            return uint.TryParse(clean, System.Globalization.NumberStyles.HexNumber, null, out _);
        }
        private bool IsHexString4bytes(string input)
        {

            // Replace the range operator with a substring method for compatibility with C# 7.3
            if (!HexU32Pattern.IsMatch(input?.Trim() ?? string.Empty))
                return false;

            input = input.Substring(8); // Remove "0x" prefix
            return ulong.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        }

        private bool IsHexString4bytes_DAC(string input)
        {

            // Replace the range operator with a substring method for compatibility with C# 7.3
            if (!HexU16Pattern.IsMatch(input?.Trim() ?? string.Empty))
                return false;

            input = input.Substring(2); // Remove "0x" prefix
            return ulong.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        }

        private void ComboRegAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabAD4368)
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

        }

        private void Cmd_WriteReg_AD4368_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD4368)
            {
                if ((ftDev == null))
                {
                    LogStatus("Message SPI init neeeded");
                }
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



                    if ((byte)regValue == RF_PLL_POWER_REG || (byte)regValue == RF_PLL_LKDET_REG)
                    {
                        // If the register is the power register, check its status
                        CheckPowerRegister(RF_PLL_POWER_REG);
                        RFLockSampling(RF_PLL_LKDET_REG, 0);
                    }

                }
                else
                {
                    MessageBox.Show("The textbox value is empty or wrong!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textAD4368_Value.Focus();
                    textAD4368_Value.Clear();
                }
            }
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
            selectedTab = tabControl1.SelectedTab;

            CheckFTDIConnection(); // Initial check on load            
        }

        private void ComboMUXOUT_SelectedIndexChanged(object sender, EventArgs e)
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

                case 6:
                    data = 0b01100000;
                    break;
                case 7:
                    data = 0b01110000;
                    break;
                case 8:
                    data = 0b10000000;
                    break;

                case 9:
                    data = 0b10010000;
                    break;

                case 10:
                    data = 0b10100000;
                    break;
                case 11:
                    data = 0b10110000;
                    break;
                case 12:
                    data = 0b11000000;
                    break;
                case 13:
                    data = 0b11010000;
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
            if (selectedTab == tabAD4368)
            {

                byte powerreturn = ad4368.ReadRegister((ushort)address);

                if (powerreturn == 0x00)
                {
                    Cmd_PowerONOFF.Text = "RF POWER OFF";
                    radioRF_POWER_Status.Checked = true;


                }
                else
                {
                    Cmd_PowerONOFF.Text = "RF POWER ON";
                    radioRF_POWER_Status.Checked = false;

                }

                foreach (DataRow row in DT4368.Rows)
                {
                    if (row["Register"].ToString() == "0x002B")
                    {
                        row["Value"] = $"0x{powerreturn:X2}";
                        row["Value byte"] = powerreturn;
                        break;
                    }
                }
            }


        }

        private void Cmd_PowerONOFF_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD4368)
            {
                if (Cmd_PowerONOFF.Text.Equals("RF POWER ON"))
                {
                    ad4368.WriteRegister(RF_PLL_POWER_REG, 0x00);
                    Thread.Sleep(100);
                    CheckPowerRegister(RF_PLL_POWER_REG);
                    RFLockSampling(RF_PLL_LKDET_REG, 0);
                }
                else
                {
                    ad4368.WriteRegister(RF_PLL_POWER_REG, 0x83);
                    Thread.Sleep(100);
                    CheckPowerRegister(RF_PLL_POWER_REG);
                    RFLockSampling(RF_PLL_LKDET_REG, 0);
                }
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
                    DT4368.Clear();
                }

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

                        if ((byte)regValue == RF_PLL_POWER_REG)
                        {
                            // if the register is the power register, check its status
                            CheckPowerRegister(RF_PLL_POWER_REG);
                            //RFLockSampling(RF_PLL_LKDET_REG, 0);
                        }

                        if ((byte)regValue == RF_PLL_LKDET_REG)
                        {
                            // if the register is the Lock Detect bit[0] register, check its status
                            RFLockSampling(RF_PLL_LKDET_REG, 0);
                        }

                    }
                    catch (Exception ex)
                    {

                        LogStatus($"ReadRegister(0x{regValue:X4}) failed: {ex.Message}");
                        continue;
                    }
                }



            }
        }

        private int RFLockSampling(byte address, int monitoredBitIndex)
        {
            int bitValue = -1;
            byte lockdata = 0x00;
            if (selectedTab == tabAD4368)
            {
                if (ftDev != null)
                {
                    lockdata = ad4368.ReadRegister(address);
                    bitValue = (lockdata >> monitoredBitIndex) & 0x01;

                    if (bitValue == 1)
                    {
                        LogStatus("RF PLL LOCKED");
                        checkRFLOCK.Checked = true;
                    }
                    else
                    {
                        LogStatus("RF PLL UNLOCKED");
                        checkRFLOCK.Checked = false;
                    }

                }
                else
                {
                    LogStatus("SPI interface not initialized. Please reconnect the FTDI device.");
                    throw new InvalidOperationException("SPI interface not initialized. Please reconnect the FTDI device.");
                }

                foreach (DataRow row in DT4368.Rows)
                {
                    if (row["Register"].ToString() == "0x0058")
                    {
                        row["Value"] = $"0x{lockdata:X2}";

                        row["Value byte"] = lockdata;
                        break;
                    }
                }

            }
            return bitValue;
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


                Cmd_FT_Temp_Read.Enabled = false;
                Cmd_RF_Temp_Read.Enabled = false;

            }
            else
            {
                tabControl1.Enabled = true;

                Cmd_FT_Temp_Read.Enabled = true;
                Cmd_RF_Temp_Read.Enabled = true;
                comboMUXOUT.Enabled = true;
                comboRegAddress.Enabled = true;
                Cmd_WriteReg_AD4368.Enabled = true;
                Cmd_RFPLL_Init.Enabled = true;
                textAD4368_Value.Enabled = true;

            }
        }

        public void Control_Init_PLL(bool initflag)
        {
            if (!initflag)
            {
                textAD4368_Value.Enabled = false;
                comboRegAddress.Enabled = false;
                comboMUXOUT.Enabled = false;
                //Cmd_Import_AD4368_File.Enabled = false;
            }
            else
            {
                textAD4368_Value.Enabled = true;
                comboRegAddress.Enabled = true;
                comboMUXOUT.Enabled = true;
                //Cmd_Import_AD4368_File.Enabled = true;
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTab = tabControl1.SelectedTab;

            if (selectedTab == tabRFLine)
            {
                gpio_control.Write(GPIO3, false);
                if (!TXline_flag)
                {

                    checkAmp1.Checked = txLineData.bypass1; //bypass mode - false,   AMP mode - true
                    checkAmp2.Checked = txLineData.bypass2; //bypass mode - false,   AMP mode - true
                    numericATT1.Value = Convert.ToDecimal(txLineData.att1);
                    numericATT2.Value = Convert.ToDecimal(txLineData.att2);
                    numericATT3.Value = Convert.ToDecimal(txLineData.att3);


                    hmc8414.Init(i2cBus, IO_Exp, InterfaceManager);

                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP1, txLineData.bypass1);
                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP2, txLineData.bypass2);
                    TXline_flag = true; // Set flag to true after initialization
                }
                else
                {


                    // Reinitialize HMC8414 if already initialized
                    hmc8414.Init(i2cBus, IO_Exp, InterfaceManager);
                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP1, checkAmp1.Checked);
                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP2, checkAmp2.Checked);
                }
            }
            else if (selectedTab == tabFPGA)
            {
                gpio_control.Write(GPIO3, true);
                //fpga.Init(ftDev, InterfaceManager);
                fpga.Init(ftFPGA, InterfaceManagerFPGA); // Initialize FPGA with the current FTDI device

                if (DTFPGA.Rows.Count > 0)
                {

                }
                else if (DTFPGA.Rows.Count == 0)
                {
                    DTFPGA = fpga.InitDataTableFPGA();
                    dataGridFPGA.DataSource = DTFPGA;
                    fpga.LoadRegisterFile(jesd_file);
                    LogStatusFPGA($"JESD204B register file loaded: {jesd_file}");

                }

                textFPGA_Address.Focus();
            }
            else if (selectedTab == tabAD9175)
            {
                gpio_control.Write(GPIO3, false);
                NCO_Control(true);
                ComboDAC_index.SelectedIndex = 0; // Set default index to 0
                comboPRBS.SelectedIndex = 0; // Set default index to 0


                ftDev = InterfaceManager.GetSpi();
                i2cBus = InterfaceManager.GetI2c(); // Get current I²C interface
                ad9175.Init(ftDev, i2cBus, IO_Exp, InterfaceManager);

                //ad9175.IO_DAC_IO_Reset(); not relevant

                if (DT9175.Rows.Count > 0)
                {
                    ad9175.ReadAllRegisters();
                }
                else if (DT9175.Rows.Count == 0)
                {
                    DT9175 = ad9175.InitDataTableDAC();
                    dataGridViewAD9175.DataSource = DT9175;
                    ad9175.LoadRegisterFile(daq_reg_file);
                }


                comboRegisters9175.Focus();
            }
            else if (selectedTab == tabAD4368)
            {
                gpio_control.Write(GPIO3, false);
                if (tabControl1.SelectedTab == tabAD4368 && !isAD4368GridBound)
                {
                    DT4368 = ad4368.InitDataTable();
                    dataGridViewAD4368.DataSource = DT4368;
                    isAD4368GridBound = true;

                    Cmd_WriteAll_AD4368.Enabled = false;
                    Cmd_ReadAll_AD4368.Enabled = false;
                    Cmd_Export_AD4368_File.Enabled = false;
                }


                ftDev = InterfaceManager.GetSpi();
                i2cBus = InterfaceManager.GetI2c(); // Get current I²C interface
                ad4368.Init(ftDev, i2cBus, IO_Exp, InterfaceManager); // Initialize AD4368 with the current FTDI device


                comboRegAddress.DataSource = ad4368.LoadComboRegisters();
                LogStatus("AD4368 reinitialized on SPI CS1");
                //Cmd_ReadAll_AD4368.Enabled = true;
            }
            else if (selectedTab == tabSi5518)
            {

                gpio_control.Write(GPIO3, false);
                ftDev = InterfaceManager.GetSpi();
                i2cBus = InterfaceManager.GetI2c(); // Get current I²C interface
                si5518.Init(ftDev, i2cBus, IO_Exp, InterfaceManager); // Initialize Si5518 with the current FTDI device

            }
            else
            {
                //gpio_control.Write(GPIO3, false);
                //ftDev = InterfaceManager.GetSpi(); // Get current SPI interface
                ad4368.Init(ftDev, i2cBus, IO_Exp, InterfaceManager); // Initialize AD4368 with the current FTDI device
            }



        }

        // textBox for PLL4368 specific register value update, after pressing Enter focus will change to Write Register button
        private void TextAD4368_Value_KeyPress(object sender, KeyPressEventArgs e)
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



        private void Cmd_Export_AD4368_File_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD4368)
                ad4368.SaveDataTableToCsv(DT4368);
        }

        private void Cmd_WriteAll_AD4368_Click(object sender, EventArgs e)
        {

            if (selectedTab == tabAD4368)
                WriteToRFPLL();

        }


        private void WriteToRFPLL()
        {

            byte paddress = 0x00;
            var filteredRows = DT4368.AsEnumerable().Where(row =>
            {
                string hexStr = row["Register"].ToString();      // e.g. "0x0053"
                ushort reg = Convert.ToUInt16(hexStr.Substring(2), 16); // Convert to int
                return reg >= 0x10 && reg <= 0x53;
            }).OrderByDescending(row => Convert.ToUInt16(row["Register"].ToString().Substring(2), 16) // Sort descending
            );

            foreach (var row in filteredRows)
            {

                paddress = Convert.ToByte(row["Register"].ToString().Replace("0x", ""), 16);
                ushort regValue = Convert.ToUInt16(row["Register"].ToString().Replace("0x", ""), 16);
                byte databyte = Convert.ToByte(row["Value"].ToString().Replace("0x", ""), 16);

                ad4368.WriteRegister(regValue, databyte);
            }
            CheckPowerRegister(RF_PLL_POWER_REG);
            Thread.Sleep(100); // Wait for the power register to update
            RFLockSampling(RF_PLL_LKDET_REG, 0);
        }


        private void Cmd_DAC_Init_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabAD9175)
            {
                if (ad9175 != null)
                {

                    //Tested
                    ad9175.IO_DAC_IO_Reset();
                    


                    //comboRegisters9175.DataSource = ad9175.LoadComboRegister9175();
                    List<string> regDumpData = ad9175.LoadComboRegister9175();

                    comboRegisters9175.DataSource = regDumpData;

                    LogStatus("DAC9175 reinitialized on SPI CS1");


                    ad9175.DAC9175_InitEngine(dac_ini_file);
                    labelFilePath9175.Text = $"DAC File Path: {dac_ini_file}";

                    int code;
                    
                    try
                    {
                        //code = ad9175.RUN_CSV(); // Run the CSV initialization for DAC9175

                        code = ad9175.RUN_Engine();

                        ad9175.ReadAllRegisters();
                        if (code == 0)
                            LogStatus($"DAC9175 initialization completed!");
                        else
                            LogStatus($"DAC9175 initialization failed with error code: {code}");

                    }
                    catch (Exception ex)
                    {
                        code = ex.HResult; // Get the error code from the exception
                        LogStatus($"DAC9175 initialization failed with error code: {code}");
                        MessageBox.Show($"DAC9175 initialization failed with error code: {code}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ComboRegisters9175_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabAD9175)
            {
                if (comboRegisters9175.DataSource != null)
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
                        textDAC9175_Value.Text = $"0x{valbyte:X2}";
                    }

                    textDAC9175_Value.Focus();
                }
            }

        }

        private void Cmd_FT_Temp_Read_Click(object sender, EventArgs e)
        {
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

        private void Cmd_RF_Temp_Read_Click(object sender, EventArgs e)
        {
            try
            {
                double tempval = tmp100.ReadTemperature(TMP100.AddressIndex.TMP100_RF_CHIP);
                label5.Text = $"{tempval:F1} °C"; // Format the temperature
                LogStatus("Temp RF: " + label5.Text);

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
                    double voltdata;


                    ad7091.Init(ftDev, i2cBus, IO_Exp, InterfaceManager, out voltdata);
                    Thread.Sleep(10);
                    ad7091.Init(ftDev, i2cBus, IO_Exp, InterfaceManager, out voltdata);
                    // Read the ADC value and convert it to voltage
                    label7.Text = $"{voltdata:F2} V";

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
                if (fpga == null)
                {
                    MessageBox.Show("FPGA interface not initialized. Please reconnect the FTDI device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(fpga_address) || string.IsNullOrWhiteSpace(fpga_data))
                {
                    MessageBox.Show("Please enter both address and data values.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    fpga.SpiWrite(HexStringToUInt(fpga_address), HexStringToUInt(fpga_data));
                    LogStatus($"The register address {fpga_address} passed value [{NormalizeHexString(fpga_data)}] to FPGA");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to write to FPGA: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogStatus("Writing to FPGA Caused an ERROR!!!");
                }

            }
        }

        public string NormalizeHexString(string hexInput)
        {
            if (string.IsNullOrWhiteSpace(hexInput))
                return "0x00000000";

            string clean = hexInput.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? hexInput.Substring(2)
                : hexInput;

            if (!uint.TryParse(clean, System.Globalization.NumberStyles.HexNumber, null, out uint value))
                return "0x00000000"; // Fallback or throw

            return $"0x{value:X8}";
        }

        private void Cmd_FPGA_Read_Click(object sender, EventArgs e)
        {

            if (selectedTab == tabFPGA)
            {
                if (fpga == null)
                {
                    MessageBox.Show("FPGA interface not initialized. Please reconnect the FTDI device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(fpga_address) || string.IsNullOrWhiteSpace(fpga_data))
                {
                    MessageBox.Show("Please enter both address and data values.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    uint retval = fpga.SpiRead(HexStringToUInt(fpga_address));
                    LogStatus($"The register address {fpga_address} gets value [0x{retval:X8}] from FPGA");
                    textFPGA_Value.Text = $"0x{retval:X8}"; // Display the read value in the text box
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Failed to write to FPGA: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogStatus($"Writing to FPGA Caused an ERROR!!! {ex.Message}");
                }
            }
        }

        public static uint HexStringToUInt(string hex)
        {
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);

            return Convert.ToUInt32(hex, 16);
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

        private void TextFPGA_Address_KeyPress(object sender, KeyPressEventArgs e)
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


        private void TextFPGA_Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedTab == tabFPGA)
            {
                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textFPGA_Value.Text))
                {

                    if (IsValidHex(textFPGA_Value.Text))
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
                if (!string.IsNullOrWhiteSpace(textDAC9175_Value.Text))
                {
                    string regaddress = daq_address;//    comboRegisters9175.SelectedItem?.ToString()?.Trim(); // Get selected value as string

                    string dataRaw = daq_value;//   textDAC9175_Value.Text.Trim();

                    if (!TryParseHexU16_sec(regaddress, out ushort regValue))
                    {
                        MessageBox.Show("Register address must be in 0xXXXX format (e.g. 0x002B).",
                                        "Invalid address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textRegDAC9175.Focus();
                        return;
                    }

                    if (!TryParseHexByte(dataRaw, out byte dataByte))
                    {
                        MessageBox.Show("Data value must be in 0xXX format (00 - FF).",
                                        "Invalid data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textDAC9175_Value.Focus();
                        textDAC9175_Value.Clear();
                        return;
                    }

                    ad9175.WriteRegister(regValue, dataByte);


                }
                else
                {
                    MessageBox.Show("The textbox value is empty or wrong!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textDAC9175_Value.Focus();
                    textDAC9175_Value.Clear();
                }

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

        // Import FPGA vector file with relevant data
        private void Cmd_FPGA_Import_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabFPGA)
            {
                if (fpga == null)
                {
                    MessageBox.Show("FPGA interface not initialized. Please reconnect the FTDI device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string fpgavector_file = fpga.LoadVectorDataCsv(); //load vector file
                LogStatus($"FPGA Vectors file loaded");

                if (!string.IsNullOrEmpty(fpgavector_file))
                {
                    try
                    {
                        // load data from vector file
                        fpga.LoadVectorFile(fpgavector_file);
                        LogStatusFPGA($"FPGA vector file loaded: {fpgavector_file}");
                    }
                    catch (Exception ex)
                    {
                        LogStatusFPGA($"Error loading FPGA vector file: {ex.Message}");
                        MessageBox.Show($"Error loading FPGA vector file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    LogStatusFPGA("No FPGA vector file selected.");
                }
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

            for (uint addr = StartAddress; addr < StopAddress; addr += 4)
            {

                fpga.SpiWrite(addr, counternumber);
                LogStatusFPGA($"The FPGA register address 0x{addr:X8} received value [0x{counternumber:X8}]");
                Thread.Sleep(10);
                //uint returnvalue = fpga.SpiRead(addr);
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

            for (uint addr = StartAddress; addr < StopAddress; addr += 4)
            {
                uint receiveddata = fpga.SpiRead(addr);
                LogStatusFPGA($"The FPGA register address 0x{addr:X8} has value [0x{receiveddata:X8}]");
            }

        }

        private void Cmd_Read_Registers_Click(object sender, EventArgs e)
        {
            /*if (selectedTab == tabFPGA)
            {
                string startaddress = "0x00001000";
                string stopaaddress = "0x00001020";
                Read_FPGA_Registers(HexStringToUInt(startaddress), HexStringToUInt(stopaaddress));
            }*/

        }

        private void ComboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboDevice.SelectedItem.ToString() == "PCAL6416A")
            {


            }
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

                    // Write values to SERIN lines of HMC1119
                    ftDev = InterfaceManager.GetSpi(); // Get current SPI interface
                    hmc1119.Init(ftDev, i2cBus, IO_Exp, InterfaceManager); // Initialize HMC1119 with the current I2C device

                    hmc1119.SetAttenuation(HMC1119.ChipIndex.HMC1119_CHIP1, att1_value);
                    Thread.Sleep(10);

                    hmc1119.SetAttenuation(HMC1119.ChipIndex.HMC1119_CHIP2, att2_value);
                    Thread.Sleep(10);

                    hmc1119.SetAttenuation(HMC1119.ChipIndex.HMC1119_CHIP3, att3_value);
                    Thread.Sleep(10);

                }
            }
        }

        private void NumericATT1_ValueChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                if (numericATT1.Value == numericATT1.Maximum)
                {
                    MessageBox.Show("Status: MAX VALUE REACHED!", "Warning");
                    att1_value = (float)numericATT1.Maximum; // Reset ATT1 value to the last valid value
                }

                else
                    att1_value = (float)numericATT1.Value;

            }
        }

        private void NumericATT2_ValueChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                if (numericATT2.Value == numericATT2.Maximum)
                {
                    MessageBox.Show("Status: MAX VALUE REACHED!", "Warning");
                    att2_value = (float)numericATT2.Maximum; // Reset ATT1 value to the last valid value
                }
                else
                    att2_value = (float)numericATT2.Value;

            }
        }

        private void NumericATT3_ValueChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                if (numericATT3.Value == numericATT3.Maximum)
                {
                    MessageBox.Show("Status: MAX VALUE REACHED!", "Warning");
                    att3_value = (float)numericATT3.Maximum; // Reset ATT1 value to the last valid value
                }
                else
                    att3_value = (float)numericATT3.Value;

            }
        }

        private void CheckAmp1_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {

                hmc8414.Init(i2cBus, IO_Exp, InterfaceManager);

                if (checkAmp1.CheckState == CheckState.Checked)
                {
                    txLineData.bypass1 = true; // AMP ON
                    checkAmp1.Text = "AMP ON";

                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP1, txLineData.bypass1);
                    //IO_Exp.SetPinState(6, false);

                }
                else
                {
                    txLineData.bypass1 = false; // BYPASS OFF (AMP ON)
                    checkAmp1.Text = "BYPASS ON";

                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP1, txLineData.bypass1);

                }

                //hmc8414.SetBypass(HMC8414.ChipIndex.HMC8414_CHIP1, txLineData.bypass1); // Set HMC8414 Chip 1 to the desired Bypass mode

                LogStatus($"Amplifier 1 Status: {checkAmp1.Text}");
            }

        }

        private void CheckAmp2_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabRFLine)
            {
                 hmc8414.Init(i2cBus, IO_Exp, InterfaceManager);

                if (checkAmp2.CheckState == CheckState.Checked)
                {
                    txLineData.bypass2 = true; // AMP ON
                    checkAmp2.Text = "AMP ON";

                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP2, txLineData.bypass2);
                }
                else
                {
                    txLineData.bypass2 = false; // BYP
                    checkAmp2.Text = "BUPASS ON";
                    hmc8414.SetAmplifier(HMC8414.ChipIndex.HMC8414_CHIP2, txLineData.bypass2);
                }

                //hmc8414.SetBypass(HMC8414.ChipIndex.HMC8414_CHIP2, txLineData.bypass2); // Set HMC8414 Chip 2 to the desired Bypass mode

                LogStatus($"Amplifier 2 Status: {checkAmp2.Text}");
            }

        }

        private void NumericATT1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void NumericATT2_KeyPress(object sender, KeyPressEventArgs e)
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

        private void NumericATT3_KeyPress(object sender, KeyPressEventArgs e)
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

        private void Cmd_PRBS_Click(object sender, EventArgs e)
        {
            //PRBS7 - PRBS15 -PRBS31

            string prmbs_value = comboPRBS.SelectedItem?.ToString();

            ad9175.PRBS_Test(prmbs_value);
            //for (int dacline = 0; dacline <= 7; dacline++)
                //ad9175.PRBS_NEW(dacline, prmbs_value);
        }

        


        private void Cmd_I2C_Write_Click(object sender, EventArgs e)
        {

        }

        private void TextI2C_Reg_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void TextI2C_Val_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Cmd_I2C_Read_Click(object sender, EventArgs e)
        {

        }

        

        private void Cmd_RFPLL_Init_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabAD4368)
            {
                if (ad4368 != null)
                {
                    if (DT4368.Rows.Count > 0)
                    {
                        DT4368.Clear();
                        //dataGridViewAD4368.Update();
                    }
                    // With this line, using the public ParsingFile method to set the file path and parse the file:
                    ad4368.ParsingFile(rf_pll_ini_file);


                    labelFilePathAD4368.Text = $"File Path: {rf_pll_ini_file}";
                    try
                    {

                        ad4368.WriteRegister(RF_PLL_POWER_REG, 0x83); // AD4368 RF PLL POWER OFF
                        LogStatus("AD4368 RF PLL reinitialized on SPI CS1");
                        WriteToRFPLL();
                        LogStatus("AD4368 RF PLL initialized successfully.");

                        Cmd_ReadAll_AD4368.Enabled = true;
                        Cmd_WriteAll_AD4368.Enabled = true;
                        Cmd_Export_AD4368_File.Enabled = true;

                    }
                    catch (Exception ex)
                    {

                        LogStatus($"AD4368 RF PLL initialization failed with error: {ex.Message}");
                        return;
                    }

                }
            }
        }

        private void Cmd_UpdateFS_Ioutfs_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                if (ad9175 != null)
                {

                    int dac_num = ComboDAC_index.SelectedIndex; // Get the selected DAC index from the combo box

                    if (dac_num > -1)
                    {

                        dac_fs_value = (float)numericDAC_FS.Value; // Get the DAC full-scale value from the numeric up-down control
                        ad9175.DAC_FullScale(dac_num, dac_fs_value);
                        LogStatus($"DAC Ioutfs set to {dac_fs_value} mA");
                    }
                    else
                    {
                        MessageBox.Show("Please select a DAC from the combo box.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                    }

                }
                else
                {
                    MessageBox.Show("DAC is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void numericDAC_FS_ValueChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                if (numericDAC_FS.Value == numericDAC_FS.Maximum)
                {
                    MessageBox.Show("Status: MAX VALUE REACHED!", "Warning");
                    dac_fs_value = (float)numericDAC_FS.Maximum; // set dac fs value for 25.977 mA
                }
                else
                    dac_fs_value = (float)numericDAC_FS.Value;



                if (numericDAC_FS.Value == numericDAC_FS.Minimum)
                {
                    MessageBox.Show("Status: MAX VALUE REACHED!", "Warning");
                    dac_fs_value = (float)numericDAC_FS.Minimum; // set dac fs value for 25.977 mA
                }
                else
                    dac_fs_value = (float)numericDAC_FS.Value;

            }
        }

        private void Cmd_STPL_Click(object sender, EventArgs e)
        {
            //byte sampind = 0x00;
            //ushort testdata = 0x0F;

            Cmd_STPL.Enabled = false; // Disable the button to prevent multiple clicks during the test
            _stplCancelToken = new CancellationTokenSource();
            Cmd_Stop_STPL.Enabled = true;
            var token = _stplCancelToken.Token;

            if (selectedTab == tabAD9175)
            {
                if (ad9175 != null)
                {

                    if (stpl_data != string.Empty && comboBox2.SelectedIndex > -1 && comboBox3.SelectedIndex > -1 && comboBox4.SelectedIndex > -1 && comboBox5.SelectedIndex > -1)
                    {
                        // Step 1: Initialize STPL test parameters
                        ushort testdata = HexStringToUShort(stpl_data); // Example: 0x0F = 15 in decimal, will be shifted by 4 bits (x16)
                        byte link = (byte)comboBox2.SelectedIndex;
                        byte ch = (byte)comboBox3.SelectedIndex;
                        byte iq = (byte)comboBox4.SelectedIndex;
                        byte sampind = (byte)comboBox5.SelectedIndex;



                        Task.Run(() =>
                        {
                            try
                            {
                                if (token.IsCancellationRequested)
                                    return;

                                RunSTPLTest(testdata, link, ch, iq, sampind);
                                // LogStatus must run on the UI thread
                                BeginInvoke(new Action(() =>
                                {
                                    LogStatus($"STPL Test completed for Link: {link}, Channel: {ch}, IQ Path: {iq}, Sample Index: 0x{sampind:X2}");
                                }));

                            }
                            catch (Exception ex)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    LogStatus($"STPL Test failed with error: {ex.Message}");
                                }));

                            }
                            finally
                            {
                                BeginInvoke(new Action(() =>
                                {

                                    Cmd_STPL.Enabled = true; // Re-enable the button after the test completes
                                    Cmd_Stop_STPL.Enabled = false; // Disable the stop button
                                }));
                            }
                        }, token);



                    }
                    else
                    {
                        MessageBox.Show("CHECK IF All values are correct and not empty!!!", "Warning");
                    }
                    /*
                    Task.Run(() =>
                    {
                        try
                        {
                            // Run STPL test with default parameters
                            //RunSTPLTest(0x0F, 0, 0, 0, 15); // Example: 0x0F = 15 in decimal, linkSel = 0 (DAC0), channelSel = 0 (Ch0), iqSel = 0 (I path), sampleIndex = from 0 to 15
                            for (int k = 0; k < 2; k++) // linksel DAC0 - 0, DAC1 - 1
                            {
                                for (int i = 0; i < 4; i++) //chanelselect 0 - 4
                                {
                                    for (int j = 0; j < 2; j++) //iqsel I -0 , Q - 1
                                    {
                                        if (token.IsCancellationRequested)
                                            return;

                                        RunSTPLTest(testdata, (byte)k, (byte)(i), (byte)j, sampind);

                                        // LogStatus must run on the UI thread
                                        BeginInvoke(new Action(() =>
                                        {
                                            LogStatus($"STPL Test completed for Link: {k}, Channel: {i}, IQ Path: {j}, Sample Index: 0x{sampind:X2}");
                                        }));

                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                LogStatus($"STPL Test failed with error: {ex.Message}");
                            }));

                        }
                        finally
                        {
                            BeginInvoke(new Action(() =>
                            {

                                Cmd_STPL.Enabled = true; // Re-enable the button after the test completes
                                Cmd_Stop_STPL.Enabled = false; // Disable the stop button
                            }));
                        }                            
                        
                    }, token);  */

                }
                else
                {
                    LogStatus("DAC Handle is not initialized.");
                }
            }
        }

        void RunSTPLTest(
            ushort expectedSample,   // Expected sample value (12-bit max), will be shifted by 4 bits (x16)
            byte linkSel,            // 0 = Link 0 (DAC0), 1 = Link 1 (DAC1)
            byte channelSel,         // 0 = Ch0, 1 = Ch1, 2 = Ch2
            byte iqSel,              // 0 = I path, 1 = Q path
            byte sampleIndex         // 0 to 15 = which sample inside frame to test
            )
        {
            // Step 3: Set expected reference sample (shift left by 4 bits)
            ushort shiftedSample = (ushort)expectedSample;// (ushort)(expectedSample << 4);
            byte refMSB = (byte)((shiftedSample >> 8) & 0xFF);
            byte refLSB = (byte)(shiftedSample & 0xFF);
            ad9175.WriteRegister(REG_SHORT_TPL_TEST_2, refMSB);
            ad9175.WriteRegister(REG_SHORT_TPL_TEST_1, refLSB);

            // Step 4 & 6: Select link and IQ path
            byte tplTest3 = 0x00;
            tplTest3 |= (byte)((linkSel & 0x01) << 7);
            tplTest3 |= (byte)((iqSel & 0x01) << 6);
            ad9175.WriteRegister(REG_SHORT_TPL_TEST_3, tplTest3);

            // Step 5 & 7: Select channel and sample index
            byte tplTest0 = 0x00;
            tplTest0 |= (byte)((sampleIndex & 0x0F) << 4);   // bits 7:4 = sample index
            tplTest0 |= (byte)((channelSel & 0x03) << 2);    // bits 3:2 = channel
            tplTest0 |= 0x01; // bit 0 = enable test
            ad9175.WriteRegister(REG_SHORT_TPL_TEST_0, tplTest0);

            // Step 9: Trigger reset
            tplTest0 |= 0x02;  // bit 1 = set reset
            ad9175.WriteRegister(REG_SHORT_TPL_TEST_0, tplTest0);

            tplTest0 &= 0xFD;  // bit 1 = clear reset
            ad9175.WriteRegister(REG_SHORT_TPL_TEST_0, tplTest0);

            // Step 10: Wait desired time
            double desiredTimeSec = 1.0; // Example: 1 GSPS, BER = 1e-10
            LogStatus("Waiting for test to complete...");
            Thread.Sleep((int)(desiredTimeSec * 1000)); // Convert seconds to ms

            // Step 11: Read result
            byte result = ad9175.ReadRegister(REG_SHORT_TPL_TEST_3);
            bool fail = (result & 0x01) != 0;

            LogStatus($"Result value: 0x{result:X2}");

            if (fail)
                LogStatus("STPL Test FAILED.");
            else
                LogStatus("STPL Test PASSED.");

        }

        private void Cmd_StartSweep_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                if (ad9175 != null)
                {
                    if (checkBox1.Checked) // single sweep
                    {
                        if (ComboDAC_index.SelectedIndex < 0)
                        {
                            MessageBox.Show("Please select a DAC from the combo box.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(textStart.Text))
                        {
                            MessageBox.Show("Please enter valid start frequency.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // FIX: Convert start_freq (string) to float before passing to Calibration_NCO
                        if (!float.TryParse(textStart.Text, out float startFreqFloat))
                        {
                            MessageBox.Show("Start frequency must be a valid number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        ad9175.Calibration_NCO(ComboDAC_index.SelectedIndex, startFreqFloat, (int)numericTone_Amplitude.Value);

                        LogStatus("Single sweep Done.");
                    }
                    else
                    {
                        if (ComboDAC_index.SelectedIndex < 0)
                        {
                            MessageBox.Show("Please select a DAC from the combo box.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(textStart.Text))
                        {
                            MessageBox.Show("Please enter valid start frequency.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(textStop.Text))
                        {
                            MessageBox.Show("Please enter valid start frequency.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // FIX: Convert start_freq (string) to float before passing to Calibration_NCO
                        if (!float.TryParse(textStart.Text, out float startFreqFloat))
                        {
                            MessageBox.Show("Start frequency must be a valid number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (!float.TryParse(textStop.Text, out float stopFreqFloat))
                        {
                            MessageBox.Show("Start frequency must be a valid number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                        LogStatus("Sweep started with the specified parameters.");
                        //do for loop with calibration NCO
                        for (float freq = startFreqFloat; freq <= stopFreqFloat; freq += float.Parse(textStep.Text))
                        {
                            ad9175.Calibration_NCO(ComboDAC_index.SelectedIndex, freq, (int)numericTone_Amplitude.Value);
                            Thread.Sleep((int)numericTime.Value); // milliseconds
                        }

                    }



                }
                else
                {
                    LogStatus("DAC Handle is not initialized.");
                }
            }
        }

        private void NCO_Control(bool ncoflag)
        {
            if (ncoflag)
            {
                textStart.Enabled = true;
                textStop.Enabled = false;
                textStep.Enabled = false;
                numericTone_Amplitude.Enabled = true;
                numericTime.Enabled = false;
            }
            else
            {
                textStart.Enabled = true;
                textStop.Enabled = true;
                textStep.Enabled = true;
                numericTone_Amplitude.Enabled = true;
                numericTime.Enabled = true;
            }

        }

        //daq combobox for dac0/1
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                NCO_Control(true); // Enable NCO control

            }
            else
            {
                NCO_Control(false); // Disable NCO control
            }
        }

        private void textRegDAC9175_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedTab == tabAD9175)
            {

                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textRegDAC9175.Text))
                {
                    // Validate Hex value entered in this field
                    if (IsHexString4bytes_DAC(textRegDAC9175.Text))
                    {
                        daq_address = textRegDAC9175.Text;
                        textDAC9175_Value.Focus();
                    }
                    else
                    {
                        textRegDAC9175.Clear();
                        textRegDAC9175.Focus();
                        daq_address = string.Empty;
                        MessageBox.Show("The register address is not correct!", "Warning");
                    }
                }
            }
        }

        private void textDAC9175_Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedTab == tabAD9175)
            {

                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textDAC9175_Value.Text))
                {
                    // Validate Hex value entered in this field
                    if (IsHexString(textDAC9175_Value.Text))
                    {
                        daq_value = textDAC9175_Value.Text;
                        Cmd_WriteReg9175.Focus();
                    }
                    else
                    {
                        textDAC9175_Value.Clear();
                        textDAC9175_Value.Focus();
                        daq_value = string.Empty;
                        MessageBox.Show("The register value is not correct!", "Warning");
                    }
                }
            }

        }

        private void Cmd_ReadRegAD9175_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                if (string.IsNullOrWhiteSpace(daq_address))
                {
                    MessageBox.Show("Please enter a valid register address and value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //string selectedRegisterAddress = comboRegAddress.SelectedItem.ToString();
                int selectedHex = Convert.ToInt32(daq_address.Substring(2), 16); // Convert hex string to int
                byte valbyte = ad9175.ReadRegister((ushort)selectedHex);
                textDAC9175_Value.Text = $"0x{valbyte:X2}";
            }
        }

        private void textStart_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textStart.Text))
                {
                    // Validate Hex value entered in this field
                    if (Regex.IsMatch(textStart.Text, "\\d{4}"))
                    {
                        start_freq = textStart.Text;
                        if (checkBox1.Checked)
                        {
                            Cmd_StartSweep.Focus();
                        }
                        else
                        {
                            // If sweep is enabled, focus on stop frequency control
                            textStop.Focus();
                        }

                    }
                    else
                    {
                        textStart.Clear();
                        textStart.Focus();
                        start_freq = string.Empty;
                        MessageBox.Show("The start frequency is not correct!", "Warning");
                    }
                }
            }
        }

        private void textStop_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textStop.Text))
                {
                    // Validate Hex value entered in this field
                    if (Regex.IsMatch(textStop.Text, "\\d{4}"))
                    {
                        stop_freq = textStop.Text;
                        textStep.Focus();
                    }
                    else
                    {
                        textStop.Clear();
                        textStop.Focus();
                        stop_freq = string.Empty;
                        MessageBox.Show("The stop frequency is not correct!", "Warning");
                    }
                }
            }

        }

        private void textStep_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (selectedTab == tabAD9175)
            {
                if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textStop.Text))
                {
                    // Validate Hex value entered in this field
                    if (Regex.IsMatch(textStep.Text, "^\\d{1,3}$"))
                    {
                        step_freq = textStep.Text;
                        Cmd_StartSweep.Focus();
                    }
                    else
                    {
                        textStep.Clear();
                        textStep.Focus();
                        step_freq = string.Empty;
                        MessageBox.Show("The step frequency is not correct!", "Warning");
                    }
                }
            }

        }

        private void Cmd_WriteDAC9175_Click(object sender, EventArgs e)
        {

        }

        private void Cmd_WriteFPGA_Click(object sender, EventArgs e)
        {

        }

        private void Cmd_FPGA_Tests_Click(object sender, EventArgs e)
        {
            uint deft = fpga.SpiReadByName("container_version");
        }



        private void Cmd_Load_JESD204_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabFPGA )
            {
                fpga.WriteReadFPGA();
            }

            
        }

        public static string ExtractLinkModePrefix(string input)
        {
            input = input.ToLower();

            // Match either "uplink modem" or "downlink modem" (ignore trailing digits)
            var match = Regex.Match(input, @"^(uplink|downlink)\s+(modem|dac)");     //Regex.Match(input, @"(uplink|downlink)\s+modem");
            if (match.Success)
            {
                // Replace space with underscore to normalize the format
                return match.Value.Replace(" ", "_");
            }

            return null; // or throw exception if not matched
        }

        public int ExtractTrailingNumber(string input)
        {
            var match = Regex.Match(input, @"(\d+)$");  // Match digits at end of string
            if (match.Success && int.TryParse(match.Value, out int result))
            {
                return result;
            }
            return -1; // or throw exception if preferred
        }
        private void comboBoxDebugger_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedTab == tabFPGA)
            {
                string test_mode = comboBoxDebugger.SelectedItem.ToString();

                fpga.StreamNum = ExtractTrailingNumber(test_mode);
                fpga.DebugMode = ExtractLinkModePrefix(test_mode);
            }
        }

        private void Cmd_Activate_Player_Click(object sender, EventArgs e)
        {
            // Disable the button to prevent duplicate clicks
            Cmd_Activate_Player.Enabled = false;
            Cmd_Stop_Player.Enabled = true; // Enable the stop button

            _playbackCancelToken = new CancellationTokenSource();
            var token = _playbackCancelToken.Token;

            Task.Run(() =>
            {
                try
                {
                    fpga.ActivatePlayer(fpga.DebugMode, true);

                    // Polling for cancel request inside long-running method
                    while (!token.IsCancellationRequested)
                    {
                        Thread.Sleep(100); // or do something useful
                    }

                    fpga.ActivatePlayer(fpga.DebugMode, false); // Stop playback safely
                }
                catch (Exception ex)
                {
                    // Optionally log or report the error on UI thread
                    BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Playback Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                finally
                {
                    // Re-enable the button on the UI thread
                    BeginInvoke(new Action(() =>
                    {
                        Cmd_Activate_Player.Enabled = true;
                        Cmd_Stop_Player.Enabled = false;
                    }));
                }
            }, token);
        }

        private void Cmd_Stop_Player_Click(object sender, EventArgs e)
        {
            if (_playbackCancelToken != null && !_playbackCancelToken.IsCancellationRequested)
            {
                _playbackCancelToken.Cancel();
            }

            fpga.StopPlayer(fpga.DebugMode);
            Cmd_Stop_Player.Enabled = false;
        }

        private void Cmd_Link_Status_Click(object sender, EventArgs e)
        {

        }

        private void Cmd_Stop_STPL_Click(object sender, EventArgs e)
        {
            if (_stplCancelToken != null && !_stplCancelToken.IsCancellationRequested)
            {
                _stplCancelToken.Cancel();
            }

            Cmd_Stop_STPL.Enabled = false;
            Cmd_STPL.Enabled = true;
        }

        private void comboMAINDAC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabAD9175)
            {
                int MAIN_DAC = comboMAINDAC.SelectedIndex;
                //int selectedHex = Convert.ToInt32(selectedRegisterAddress9175.Substring(2), 16); // Convert hex string to int

                if (ad9175 != null && ftDev != null)
                {

                    if (MAIN_DAC == 0)
                    {
                        ad9175.WriteRegister(0x0008, 0x40);
                        LogStatus($"Main DAC set to DAC0");
                    }
                    else
                    {
                        ad9175.WriteRegister(0x0008, 0x80);
                        LogStatus($"Main DAC set to DAC1");
                    }

                }
                else
                {
                    LogStatus("AD9175 Handle is not initialized.");
                }


            }
        }

        private void Cmd_DAC_Reg_Read_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                int dac_num = comboMAINDAC.SelectedIndex; // Get the selected DAC index from the combo box
                //-------------------------------------------------------
                if (dac_num == 0)
                    ad9175.WriteRegister(0x0008, 0x40); // Select DAC0
                else
                    ad9175.WriteRegister(0x0008, 0x80); // Select DAC1
                //-------------------------------------------------------
                ad9175.ReadAllRegisters();               

                LogStatus($"DAC {dac_num} registers read successfully.");
            }

        }

        private void Select_Dir_For_Burn_SkyPLL_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                try
                {
                    folderDialog.SelectedPath = Directory.GetCurrentDirectory();

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFolder = folderDialog.SelectedPath;

                        string prod_fw_path = Path.Combine(selectedFolder, "prod_fw.burn.hex.txt");
                        string nvm_burn_path = Path.Combine(selectedFolder, "nvm_burn_fw.boot.hex.txt");
                        string user_config_path = Path.Combine(selectedFolder, "user_config.burn.hex.txt");

                        try
                        {
                            using (FileStream fs_prod_fw = File.OpenRead(prod_fw_path)) { }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show($"File not found: {prod_fw_path}", "Error");
                            return;
                        }

                        try
                        {
                            using (FileStream fs_nvm_burn = File.OpenRead(nvm_burn_path)) { }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show($"File not found: {nvm_burn_path}", "Error");
                            return;
                        }

                        try
                        {
                            using (FileStream fs_user_config = File.OpenRead(user_config_path)) { }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show($"File not found: {user_config_path}", "Error");
                            return;
                        }

                        // All files are present
                        config_file_pll = user_config_path;
                        nvm_file_pll = nvm_burn_path;
                        Prod_file_pll = prod_fw_path;
                        label_UserConfigSi55XX.Text = "User Conig: " + user_config_path;
                        label_NvmFwSi55XX.Text = "NVM FW: " + nvm_burn_path;
                        label_ProdFwSi55XX.Text = "Prod FW: " + prod_fw_path;

                        Btn_Burn_SkyPLL.Enabled = true;
                        Btn_Load_SkyPLL.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning");
                }
            }
        }

        private void Select_Dir_For_Load_SkyPLL_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                try
                {
                    folderDialog.SelectedPath = Directory.GetCurrentDirectory();

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFolder = folderDialog.SelectedPath;

                        string prod_fw_path = Path.Combine(selectedFolder, "prod_fw.boot.hex.txt");
                        string user_config_path = Path.Combine(selectedFolder, "user_config.boot.hex.txt");

                        try
                        {
                            using (FileStream fs_prod_fw = File.OpenRead(prod_fw_path)) { }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show($"File not found: {prod_fw_path}", "Error");
                            return;
                        }

                        try
                        {
                            using (FileStream fs_user_config = File.OpenRead(user_config_path)) { }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show($"File not found: {user_config_path}", "Error");
                            return;
                        }

                        // All files are present
                        config_file_pll = user_config_path;
                        Prod_file_pll = prod_fw_path;
                        label_UserConfigSi55XX.Text = "User Conig: " + user_config_path;
                        label_NvmFwSi55XX.Text = "NVM FW: N/A";
                        label_ProdFwSi55XX.Text = "Prod FW: " + prod_fw_path;

                        Btn_Load_SkyPLL.Enabled = true;
                        Btn_Burn_SkyPLL.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning");
                }
            }
        }

        private void button_GetInfo_Click(object sender, EventArgs e)
        {
            double temp_skyworks;
            string info_skyworks;

            if ((si5518.SioTest() != SI55XX.ResiltCode.OK))
            {
                label_SioTest.Text = "SioTest: Failed";
            }
            else
            {
                label_SioTest.Text = "SioTest: Passed";
            }

            if ((si5518.ReadTemperature(out temp_skyworks) != SI55XX.ResiltCode.OK))
            {
                MessageBox.Show("Failed to read temperature!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            label_Temp.Text = $"Temp: {temp_skyworks:F1} °C"; // Format the temperature

            if ((si5518.ReadInfo(out info_skyworks) != SI55XX.ResiltCode.OK))
            {
                MessageBox.Show("Failed to read device info!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            label_DevInfo.Text = $"DevInfo: {info_skyworks}";
        }

        private void button_CheckRefStat_Click(object sender, EventArgs e)
        {
            bool ref_stat;

            if ((si5518.CheckReferenceStatus(out ref_stat) != SI55XX.ResiltCode.OK))
            {
                MessageBox.Show("Failed to read the reference status!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label_RefStat.Text = "RefStat: Failed";
                return;
            }

            string status = ref_stat ? "Locked" : "Unlocked";
            label_RefStat.Text = $"RefStat: {status}";
        }

        private void Btn_Load_SkyPLL_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Prod_file_pll))
            {
                MessageBox.Show("Prod FW file path is not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(config_file_pll))
            {
                MessageBox.Show("User Config file path is not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if ((si5518.InitPllSynth(Prod_file_pll, config_file_pll) != SI55XX.ResiltCode.OK))
            {
                MessageBox.Show("Failed to load RAM!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("FW boot finished and user config loaded, check the lock status.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Btn_Burn_SkyPLL_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nvm_file_pll))
            {
                MessageBox.Show("NVM file path is not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(Prod_file_pll))
            {
                MessageBox.Show("Prod FW file path is not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(config_file_pll))
            {
                MessageBox.Show("User Config file path is not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show(
                "This is one-time process, are you sure?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
                return;

            MessageBox.Show("NVM burn process will take several seconds, please wait.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if ((si5518.BurnNvmPllSynth(nvm_file_pll, Prod_file_pll, config_file_pll) != SI55XX.ResiltCode.OK))
            {
                MessageBox.Show("Failed to burn NVM!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("NVM burn finished, please perform a power cycle.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool RegexIsValidHex(string input)
        {
            return HexRegex.IsMatch(input);
        }

        private void textSTPL_Data_KeyPress(object sender, KeyPressEventArgs e)
        {
            

            if (e.KeyChar == 13 && !string.IsNullOrWhiteSpace(textSTPL_Data.Text))
            {
                // If it's the first char, only '0' is allowed
                if (textSTPL_Data.Text.Length == 0 && e.KeyChar != '0')
                {
                    e.Handled = true;
                    return;
                }

                // If it's the second char, only 'x' or 'X' is allowed
                if (textSTPL_Data.Text.Length == 1 && (e.KeyChar != 'x' && e.KeyChar != 'X'))
                {
                    e.Handled = true;
                    return;
                }

                // After "0x", allow hex digits only
                if (textSTPL_Data.Text.Length >= 2 && !Uri.IsHexDigit(e.KeyChar))
                {
                    e.Handled = true;
                }

                if (RegexIsValidHex(textSTPL_Data.Text))
                {
                    stpl_data = textSTPL_Data.Text;
                    comboBox2.Focus(); // Move focus to the next control
                }                   
                else
                {
                    textSTPL_Data.Clear();
                    textSTPL_Data.Focus();
                    stpl_data = string.Empty;
                    MessageBox.Show("The STPL data is not correct!", "Warning");
                }



                
            }
        }
        
        //test button for DAC tests
        private void button1_Click(object sender, EventArgs e)
        {

            List<string> register_list = new List<string>();
            register_list = ad9175.LoadComboRegister9175();


            // DUMP AREA for register
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            string filePath = Path.Combine(exeFolder, $"regDump_{timestamp}.csv");

            ad9175.Read_Dump(register_list, filePath);
            LogStatus($"Register dump saved to {filePath}");
        }

        // TEST Channel NCO button
        private void button2_Click(object sender, EventArgs e)
        {
            if(selectedTab == tabAD9175)
            {
                if (ad9175 != null)
                {
                    
                    if (ComboDAC_index.SelectedIndex < 0)
                    {
                        MessageBox.Show("Please select a DAC from the combo box.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (comboCh_NCO.SelectedIndex < 0) 
                    {
                        MessageBox.Show("Please select a Channel from the combo box.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(textStart.Text))
                    {
                        MessageBox.Show("Please enter valid start frequency.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                        // FIX: Convert start_freq (string) to float before passing to Calibration_NCO
                    if (!float.TryParse(textStart.Text, out float startFreqFloat))
                    {
                        MessageBox.Show("Start frequency must be a valid number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //ad9175.Calibration_NCO(ComboDAC_index.SelectedIndex, startFreqFloat, (int)numericTone_Amplitude.Value);
                    ad9175.Channel_NCO(ComboDAC_index.SelectedIndex, comboCh_NCO.SelectedIndex, startFreqFloat, (int)numericTone_Amplitude.Value);

                    LogStatus("Channel NCO Done.");
                }
                else
                {
                    LogStatus("DAC Handle is not initialized.");
                }                
            }
            
        }

        private void comboCh_NCO_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = false;
            int ch_num = comboCh_NCO.SelectedIndex;
            int MAIN_DAC = ComboDAC_index.SelectedIndex;

            if (ch_num > 2 && MAIN_DAC == 0)
            {
                MessageBox.Show($"For DAC{MAIN_DAC} the channels must be from 0 to 2");
                ch_num = -1;                
                comboCh_NCO.Focus();
                return;
            }
            else if(ch_num < 3 && MAIN_DAC == 1)
            {
                MessageBox.Show($"For DAC{MAIN_DAC} the channels must be from 3 to 5");
                ch_num = -1;
                comboCh_NCO.Focus();
                return;
            }

            if (ch_num == -1)
                button2.Enabled = false;
            else
                button2.Enabled = true;

        }

        private void Cmd_QBD_Click(object sender, EventArgs e)
        {
            if (selectedTab == tabAD9175)
            {
                int dac_num = comboMAINDAC.SelectedIndex; // Get the selected DAC index from the combo box
                //-------------------------------------------------------
                if (dac_num == 0)
                {
                    // Set QBD bit for DAC0
                    byte regVal = ad9175.ReadRegister(0x0300);
                    regVal = (byte)(regVal & ~0x04);  //clears bit 2
                    ad9175.WriteRegister(0x0300, regVal); 
                }
                else
                {
                    // Set QBD bit for DAC0
                    byte regVal = ad9175.ReadRegister(0x0300);
                    regVal = (byte)(regVal | 0x04);  //set bit 2 to 1
                    ad9175.WriteRegister(0x0300, regVal); 
                }
                   
                //-------------------------------------------------------
                ad9175.ReadAllRegisters();

                LogStatus($"DAC {dac_num} registers read successfully.");
            }
        }

        private void Cmd_Load_DAC_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabAD9175)
            {
                if (ad9175 != null)
                {

                    //Tested
                    ad9175.IO_DAC_IO_Reset();
                    LogStatus("DAC9175 reinitialized on SPI CS1");

                    comboRegisters9175.DataSource = null; // Clear existing data source

                    //comboRegisters9175.DataSource = ad9175.LoadComboRegister9175();
                    List<string> regDumpData = ad9175.LoadComboRegister9175();

                    comboRegisters9175.DataSource = regDumpData; // Set new data source

                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                        openFileDialog.Title = "Select DAC9175 csv Configuration File";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            dac_ini_file = Path.Combine(openFileDialog.FileName);
                        }
                        else
                        {
                            LogStatus("DAC9175 initialization canceled by user.");
                            return;
                        }
                    }


                    ad9175.DAC9175_InitEngine(dac_ini_file);
                    labelFilePath9175.Text = $"DAC File Path: {dac_ini_file}";

                    int code;

                    try
                    {
                        //code = ad9175.RUN_CSV(); // Run the CSV initialization for DAC9175

                        code = ad9175.RUN_Engine();

                        ad9175.ReadAllRegisters();
                        if (code == 0)
                        {
                            LogStatus($"DAC9175 initialization completed!");
                            if (dac_ini_file.Contains("DAC0"))
                                LogStatus("DAC0 configuration loaded");
                            else
                                LogStatus("DAC1 configuration loaded");
                        }
                            
                        else
                            LogStatus($"DAC9175 initialization failed with error code: {code}");

                    }
                    catch (Exception ex)
                    {
                        code = ex.HResult; // Get the error code from the exception
                        LogStatus($"DAC9175 initialization failed with error code: {code}");
                        MessageBox.Show($"DAC9175 initialization failed with error code: {code}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void comboDAC_Power_SelectedIndexChanged(object sender, EventArgs e)
        {
            // register 0x0090 bit 0 = DAC0 power down 1 , power up 0; bit 1 = DAC1 power down 1 , power up 0 
            int dac_power = comboDAC_Power.SelectedIndex;
            switch (dac_power)
            {
                case 0:
                    // DAC0 OFF Power, DAC1 ON
                    ad9175.WriteRegister(0x0090, 0x01);
                    LogStatus("DAC0 Power OFF|DAC1 Power ON");
                    break;
                case 1:
                    // DAC0 On Power, DAC1 OFF Power
                    ad9175.WriteRegister(0x0090, 0x02);
                    LogStatus("DAC0 Power ON|DAC1 Power OFF");
                    break;
                case 2:
                    // DAC0 DAC 1 OFF Power
                    ad9175.WriteRegister(0x0090, 0x03);
                    LogStatus("DAC0 and DAC1 Power OFF");
                    break;
                case 3:
                    // DAC0 DAC 1 ON Power
                    ad9175.WriteRegister(0x0090, 0x00);
                    LogStatus("DAC0 and DAC1 Power ON");
                    break;
                default:
                    LogStatus("Invalid DAC Power selection");
                    break;
            }
            
        }
    }
}



