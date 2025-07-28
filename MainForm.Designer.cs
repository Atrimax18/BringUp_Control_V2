namespace BringUp_Control
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Cmd_Exit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabWelcome = new System.Windows.Forms.TabPage();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label26 = new System.Windows.Forms.Label();
            this.tabAD4368 = new System.Windows.Forms.TabPage();
            this.label32 = new System.Windows.Forms.Label();
            this.Cmd_RFPLL_Init = new System.Windows.Forms.Button();
            this.checkRFLOCK = new System.Windows.Forms.CheckBox();
            this.Cmd_PowerONOFF = new System.Windows.Forms.Button();
            this.radioRF_POWER_Status = new System.Windows.Forms.RadioButton();
            this.Cmd_WriteAll_AD4368 = new System.Windows.Forms.Button();
            this.Cmd_ReadAll_AD4368 = new System.Windows.Forms.Button();
            this.Cmd_Export_AD4368_File = new System.Windows.Forms.Button();
            this.Cmd_WriteReg_AD4368 = new System.Windows.Forms.Button();
            this.textAD4368_Value = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridViewAD4368 = new System.Windows.Forms.DataGridView();
            this.comboMUXOUT = new System.Windows.Forms.ComboBox();
            this.labelMUXOUT = new System.Windows.Forms.Label();
            this.comboRegAddress = new System.Windows.Forms.ComboBox();
            this.labelRegAddress = new System.Windows.Forms.Label();
            this.labelFilePathAD4368 = new System.Windows.Forms.Label();
            this.tabAD9175 = new System.Windows.Forms.TabPage();
            this.label44 = new System.Windows.Forms.Label();
            this.textRegDAC9175 = new System.Windows.Forms.TextBox();
            this.Cmd_Link_Status = new System.Windows.Forms.Button();
            this.Cmd_STP = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.Cmd_UpdateFS_Ioutfs = new System.Windows.Forms.Button();
            this.label42 = new System.Windows.Forms.Label();
            this.numericDAC_FS = new System.Windows.Forms.NumericUpDown();
            this.label41 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Cmd_PRBS = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.Cmd_ReadRegAD9175 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label43 = new System.Windows.Forms.Label();
            this.numericTime = new System.Windows.Forms.NumericUpDown();
            this.label31 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label40 = new System.Windows.Forms.Label();
            this.numericTone_Amplitude = new System.Windows.Forms.NumericUpDown();
            this.label39 = new System.Windows.Forms.Label();
            this.Cmd_StartSweep = new System.Windows.Forms.Button();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.textStop = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.textStart = new System.Windows.Forms.TextBox();
            this.textStep = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.Cmd_NCO = new System.Windows.Forms.Button();
            this.ComboDAC_index = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.Cmd_DAC_Init = new System.Windows.Forms.Button();
            this.Cmd_WriteDAC9175 = new System.Windows.Forms.Button();
            this.Cmd_ReadDAC9175 = new System.Windows.Forms.Button();
            this.Cmd_WriteReg9175 = new System.Windows.Forms.Button();
            this.textDAC9175_Value = new System.Windows.Forms.TextBox();
            this.comboRegisters9175 = new System.Windows.Forms.ComboBox();
            this.labelDAC9175_Register = new System.Windows.Forms.Label();
            this.labelFilePath9175 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridViewAD9175 = new System.Windows.Forms.DataGridView();
            this.Cmd_Export9175_file = new System.Windows.Forms.Button();
            this.Cmd_Import9175_file = new System.Windows.Forms.Button();
            this.tabSi5518 = new System.Windows.Forms.TabPage();
            this.Cmd_Burn_SkyPLL = new System.Windows.Forms.Button();
            this.Cmd_Config = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.Cmd_Load_SI_FW = new System.Windows.Forms.Button();
            this.Cmd_Export_SkyWorks = new System.Windows.Forms.Button();
            this.Cmd_Import_SkyWorks = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.tabRFLine = new System.Windows.Forms.TabPage();
            this.numericATT3 = new System.Windows.Forms.NumericUpDown();
            this.numericATT2 = new System.Windows.Forms.NumericUpDown();
            this.numericATT1 = new System.Windows.Forms.NumericUpDown();
            this.checkAmp2 = new System.Windows.Forms.CheckBox();
            this.checkAmp1 = new System.Windows.Forms.CheckBox();
            this.Cmd_Read_ADC = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.Cmd_UpdateTX_Values = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabFPGA = new System.Windows.Forms.TabPage();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Cmd_FPGA_Tests = new System.Windows.Forms.Button();
            this.Cmd_WriteFPGA = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.Cmd_Read_Registers = new System.Windows.Forms.Button();
            this.Cmd_LoadCounter = new System.Windows.Forms.Button();
            this.Cmd_FPGA_Read = new System.Windows.Forms.Button();
            this.textFPGA_Value = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.Cmd_FPGA_Export = new System.Windows.Forms.Button();
            this.Cmd_FPGA_Write = new System.Windows.Forms.Button();
            this.textFPGA_Output = new System.Windows.Forms.TextBox();
            this.textFPGA_Address = new System.Windows.Forms.TextBox();
            this.Cmd_FPGA_Import = new System.Windows.Forms.Button();
            this.tabMux = new System.Windows.Forms.TabPage();
            this.Cmd_I2C_Read = new System.Windows.Forms.Button();
            this.Cmd_I2C_Write = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.textI2C_Val = new System.Windows.Forms.TextBox();
            this.textI2C_Reg = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboDevice = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioMUX = new System.Windows.Forms.RadioButton();
            this.radioFPGA = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Cmd_FT_Temp_Read = new System.Windows.Forms.Button();
            this.Cmd_RF_Temp_Read = new System.Windows.Forms.Button();
            this.textLog = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tabControl1.SuspendLayout();
            this.tabWelcome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.tabAD4368.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAD4368)).BeginInit();
            this.tabAD9175.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericDAC_FS)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTone_Amplitude)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAD9175)).BeginInit();
            this.tabSi5518.SuspendLayout();
            this.tabRFLine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericATT3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericATT2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericATT1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabFPGA.SuspendLayout();
            this.tabMux.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // Cmd_Exit
            // 
            this.Cmd_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_Exit.Location = new System.Drawing.Point(1019, 696);
            this.Cmd_Exit.Name = "Cmd_Exit";
            this.Cmd_Exit.Size = new System.Drawing.Size(129, 66);
            this.Cmd_Exit.TabIndex = 0;
            this.Cmd_Exit.Text = "Exit";
            this.Cmd_Exit.UseVisualStyleBackColor = true;
            this.Cmd_Exit.Click += new System.EventHandler(this.Cmd_Exit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 752);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "FTDI Status:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabWelcome);
            this.tabControl1.Controls.Add(this.tabAD4368);
            this.tabControl1.Controls.Add(this.tabAD9175);
            this.tabControl1.Controls.Add(this.tabSi5518);
            this.tabControl1.Controls.Add(this.tabRFLine);
            this.tabControl1.Controls.Add(this.tabFPGA);
            this.tabControl1.Controls.Add(this.tabMux);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(15, 65);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(852, 684);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // tabWelcome
            // 
            this.tabWelcome.Controls.Add(this.pictureBox3);
            this.tabWelcome.Controls.Add(this.label26);
            this.tabWelcome.Location = new System.Drawing.Point(4, 25);
            this.tabWelcome.Name = "tabWelcome";
            this.tabWelcome.Padding = new System.Windows.Forms.Padding(3);
            this.tabWelcome.Size = new System.Drawing.Size(844, 655);
            this.tabWelcome.TabIndex = 8;
            this.tabWelcome.Text = "Welcome";
            this.tabWelcome.UseVisualStyleBackColor = true;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::BringUp_Control.Properties.Resources.TX_FEM;
            this.pictureBox3.Location = new System.Drawing.Point(36, 27);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(743, 291);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(3, 631);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(228, 16);
            this.label26.TabIndex = 1;
            this.label26.Text = "BringUp Control (c), Satixfy 2025";
            // 
            // tabAD4368
            // 
            this.tabAD4368.Controls.Add(this.label32);
            this.tabAD4368.Controls.Add(this.Cmd_RFPLL_Init);
            this.tabAD4368.Controls.Add(this.checkRFLOCK);
            this.tabAD4368.Controls.Add(this.Cmd_PowerONOFF);
            this.tabAD4368.Controls.Add(this.radioRF_POWER_Status);
            this.tabAD4368.Controls.Add(this.Cmd_WriteAll_AD4368);
            this.tabAD4368.Controls.Add(this.Cmd_ReadAll_AD4368);
            this.tabAD4368.Controls.Add(this.Cmd_Export_AD4368_File);
            this.tabAD4368.Controls.Add(this.Cmd_WriteReg_AD4368);
            this.tabAD4368.Controls.Add(this.textAD4368_Value);
            this.tabAD4368.Controls.Add(this.groupBox1);
            this.tabAD4368.Controls.Add(this.comboMUXOUT);
            this.tabAD4368.Controls.Add(this.labelMUXOUT);
            this.tabAD4368.Controls.Add(this.comboRegAddress);
            this.tabAD4368.Controls.Add(this.labelRegAddress);
            this.tabAD4368.Controls.Add(this.labelFilePathAD4368);
            this.tabAD4368.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabAD4368.Location = new System.Drawing.Point(4, 25);
            this.tabAD4368.Name = "tabAD4368";
            this.tabAD4368.Padding = new System.Windows.Forms.Padding(3);
            this.tabAD4368.Size = new System.Drawing.Size(844, 655);
            this.tabAD4368.TabIndex = 0;
            this.tabAD4368.Text = "PLL 4368";
            this.tabAD4368.UseVisualStyleBackColor = true;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(202, 29);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(96, 16);
            this.label32.TabIndex = 18;
            this.label32.Text = "REG VALUE:";
            // 
            // Cmd_RFPLL_Init
            // 
            this.Cmd_RFPLL_Init.Location = new System.Drawing.Point(606, 159);
            this.Cmd_RFPLL_Init.Name = "Cmd_RFPLL_Init";
            this.Cmd_RFPLL_Init.Size = new System.Drawing.Size(114, 45);
            this.Cmd_RFPLL_Init.TabIndex = 17;
            this.Cmd_RFPLL_Init.Text = "RF PLL INIT";
            this.Cmd_RFPLL_Init.UseVisualStyleBackColor = true;
            this.Cmd_RFPLL_Init.Click += new System.EventHandler(this.Cmd_RFPLL_Init_Click);
            // 
            // checkRFLOCK
            // 
            this.checkRFLOCK.AutoSize = true;
            this.checkRFLOCK.Enabled = false;
            this.checkRFLOCK.Location = new System.Drawing.Point(606, 465);
            this.checkRFLOCK.Name = "checkRFLOCK";
            this.checkRFLOCK.Size = new System.Drawing.Size(94, 20);
            this.checkRFLOCK.TabIndex = 16;
            this.checkRFLOCK.Text = "PLL LOCK";
            this.checkRFLOCK.UseVisualStyleBackColor = true;
            // 
            // Cmd_PowerONOFF
            // 
            this.Cmd_PowerONOFF.Location = new System.Drawing.Point(606, 585);
            this.Cmd_PowerONOFF.Name = "Cmd_PowerONOFF";
            this.Cmd_PowerONOFF.Size = new System.Drawing.Size(114, 58);
            this.Cmd_PowerONOFF.TabIndex = 14;
            this.Cmd_PowerONOFF.Text = "RF POWER ON";
            this.Cmd_PowerONOFF.UseVisualStyleBackColor = true;
            this.Cmd_PowerONOFF.Click += new System.EventHandler(this.Cmd_PowerONOFF_Click);
            // 
            // radioRF_POWER_Status
            // 
            this.radioRF_POWER_Status.AutoCheck = false;
            this.radioRF_POWER_Status.AutoSize = true;
            this.radioRF_POWER_Status.Enabled = false;
            this.radioRF_POWER_Status.Location = new System.Drawing.Point(606, 559);
            this.radioRF_POWER_Status.Name = "radioRF_POWER_Status";
            this.radioRF_POWER_Status.Size = new System.Drawing.Size(107, 20);
            this.radioRF_POWER_Status.TabIndex = 13;
            this.radioRF_POWER_Status.TabStop = true;
            this.radioRF_POWER_Status.Text = "POWER ON";
            this.radioRF_POWER_Status.UseVisualStyleBackColor = true;
            // 
            // Cmd_WriteAll_AD4368
            // 
            this.Cmd_WriteAll_AD4368.Enabled = false;
            this.Cmd_WriteAll_AD4368.Location = new System.Drawing.Point(606, 342);
            this.Cmd_WriteAll_AD4368.Name = "Cmd_WriteAll_AD4368";
            this.Cmd_WriteAll_AD4368.Size = new System.Drawing.Size(114, 45);
            this.Cmd_WriteAll_AD4368.TabIndex = 12;
            this.Cmd_WriteAll_AD4368.Text = "Write To Device";
            this.Cmd_WriteAll_AD4368.UseVisualStyleBackColor = true;
            this.Cmd_WriteAll_AD4368.Click += new System.EventHandler(this.Cmd_WriteAll_AD4368_Click);
            // 
            // Cmd_ReadAll_AD4368
            // 
            this.Cmd_ReadAll_AD4368.Enabled = false;
            this.Cmd_ReadAll_AD4368.Location = new System.Drawing.Point(606, 291);
            this.Cmd_ReadAll_AD4368.Name = "Cmd_ReadAll_AD4368";
            this.Cmd_ReadAll_AD4368.Size = new System.Drawing.Size(114, 45);
            this.Cmd_ReadAll_AD4368.TabIndex = 11;
            this.Cmd_ReadAll_AD4368.Text = "Read From Device";
            this.Cmd_ReadAll_AD4368.UseVisualStyleBackColor = true;
            this.Cmd_ReadAll_AD4368.Click += new System.EventHandler(this.Cmd_ReadAll_AD4368_Click);
            // 
            // Cmd_Export_AD4368_File
            // 
            this.Cmd_Export_AD4368_File.Enabled = false;
            this.Cmd_Export_AD4368_File.Location = new System.Drawing.Point(606, 210);
            this.Cmd_Export_AD4368_File.Name = "Cmd_Export_AD4368_File";
            this.Cmd_Export_AD4368_File.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Export_AD4368_File.TabIndex = 10;
            this.Cmd_Export_AD4368_File.Text = "Export File";
            this.Cmd_Export_AD4368_File.UseVisualStyleBackColor = true;
            this.Cmd_Export_AD4368_File.Click += new System.EventHandler(this.Cmd_Export_AD4368_File_Click);
            // 
            // Cmd_WriteReg_AD4368
            // 
            this.Cmd_WriteReg_AD4368.Location = new System.Drawing.Point(304, 29);
            this.Cmd_WriteReg_AD4368.Name = "Cmd_WriteReg_AD4368";
            this.Cmd_WriteReg_AD4368.Size = new System.Drawing.Size(114, 45);
            this.Cmd_WriteReg_AD4368.TabIndex = 7;
            this.Cmd_WriteReg_AD4368.Text = "Write Register";
            this.Cmd_WriteReg_AD4368.UseVisualStyleBackColor = true;
            this.Cmd_WriteReg_AD4368.Click += new System.EventHandler(this.Cmd_WriteReg_AD4368_Click);
            // 
            // textAD4368_Value
            // 
            this.textAD4368_Value.Location = new System.Drawing.Point(205, 48);
            this.textAD4368_Value.Name = "textAD4368_Value";
            this.textAD4368_Value.Size = new System.Drawing.Size(93, 22);
            this.textAD4368_Value.TabIndex = 6;
            this.textAD4368_Value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextAD4368_Value_KeyPress);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridViewAD4368);
            this.groupBox1.Location = new System.Drawing.Point(9, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(550, 490);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "AD4368 REGISTER DATA:";
            // 
            // dataGridViewAD4368
            // 
            this.dataGridViewAD4368.AllowUserToAddRows = false;
            this.dataGridViewAD4368.AllowUserToDeleteRows = false;
            this.dataGridViewAD4368.AllowUserToResizeRows = false;
            this.dataGridViewAD4368.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridViewAD4368.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAD4368.Location = new System.Drawing.Point(6, 21);
            this.dataGridViewAD4368.Name = "dataGridViewAD4368";
            this.dataGridViewAD4368.ReadOnly = true;
            this.dataGridViewAD4368.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewAD4368.Size = new System.Drawing.Size(538, 463);
            this.dataGridViewAD4368.TabIndex = 0;
            // 
            // comboMUXOUT
            // 
            this.comboMUXOUT.FormattingEnabled = true;
            this.comboMUXOUT.Items.AddRange(new object[] {
            "0000: HighZ",
            "0001: LKDET",
            "0010: Low",
            "0011: Low",
            "0100: DivRCLK/2",
            "0101: DivNCLK/2",
            "0110: Reserved",
            "0111: low",
            "1000: high",
            "1001: reserved",
            "1010: reserved",
            "1011: low",
            "1100: low",
            "1101: low",
            ""});
            this.comboMUXOUT.Location = new System.Drawing.Point(9, 97);
            this.comboMUXOUT.Name = "comboMUXOUT";
            this.comboMUXOUT.Size = new System.Drawing.Size(129, 24);
            this.comboMUXOUT.TabIndex = 4;
            this.comboMUXOUT.SelectedIndexChanged += new System.EventHandler(this.ComboMUXOUT_SelectedIndexChanged);
            // 
            // labelMUXOUT
            // 
            this.labelMUXOUT.AutoSize = true;
            this.labelMUXOUT.Location = new System.Drawing.Point(6, 78);
            this.labelMUXOUT.Name = "labelMUXOUT";
            this.labelMUXOUT.Size = new System.Drawing.Size(75, 16);
            this.labelMUXOUT.TabIndex = 3;
            this.labelMUXOUT.Text = "MUXOUT:";
            // 
            // comboRegAddress
            // 
            this.comboRegAddress.FormattingEnabled = true;
            this.comboRegAddress.Location = new System.Drawing.Point(9, 48);
            this.comboRegAddress.Name = "comboRegAddress";
            this.comboRegAddress.Size = new System.Drawing.Size(129, 24);
            this.comboRegAddress.TabIndex = 2;
            this.comboRegAddress.SelectedIndexChanged += new System.EventHandler(this.ComboRegAddress_SelectedIndexChanged);
            // 
            // labelRegAddress
            // 
            this.labelRegAddress.AutoSize = true;
            this.labelRegAddress.Location = new System.Drawing.Point(6, 29);
            this.labelRegAddress.Name = "labelRegAddress";
            this.labelRegAddress.Size = new System.Drawing.Size(165, 16);
            this.labelRegAddress.TabIndex = 1;
            this.labelRegAddress.Text = "REGISTER ADDRESS:";
            // 
            // labelFilePathAD4368
            // 
            this.labelFilePathAD4368.AutoSize = true;
            this.labelFilePathAD4368.Location = new System.Drawing.Point(6, 3);
            this.labelFilePathAD4368.Name = "labelFilePathAD4368";
            this.labelFilePathAD4368.Size = new System.Drawing.Size(72, 16);
            this.labelFilePathAD4368.TabIndex = 0;
            this.labelFilePathAD4368.Text = "File Path:";
            // 
            // tabAD9175
            // 
            this.tabAD9175.Controls.Add(this.label44);
            this.tabAD9175.Controls.Add(this.textRegDAC9175);
            this.tabAD9175.Controls.Add(this.Cmd_Link_Status);
            this.tabAD9175.Controls.Add(this.Cmd_STP);
            this.tabAD9175.Controls.Add(this.groupBox7);
            this.tabAD9175.Controls.Add(this.groupBox6);
            this.tabAD9175.Controls.Add(this.textBox4);
            this.tabAD9175.Controls.Add(this.Cmd_ReadRegAD9175);
            this.tabAD9175.Controls.Add(this.groupBox5);
            this.tabAD9175.Controls.Add(this.Cmd_DAC_Init);
            this.tabAD9175.Controls.Add(this.Cmd_WriteDAC9175);
            this.tabAD9175.Controls.Add(this.Cmd_ReadDAC9175);
            this.tabAD9175.Controls.Add(this.Cmd_WriteReg9175);
            this.tabAD9175.Controls.Add(this.textDAC9175_Value);
            this.tabAD9175.Controls.Add(this.comboRegisters9175);
            this.tabAD9175.Controls.Add(this.labelDAC9175_Register);
            this.tabAD9175.Controls.Add(this.labelFilePath9175);
            this.tabAD9175.Controls.Add(this.groupBox2);
            this.tabAD9175.Controls.Add(this.Cmd_Export9175_file);
            this.tabAD9175.Controls.Add(this.Cmd_Import9175_file);
            this.tabAD9175.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabAD9175.Location = new System.Drawing.Point(4, 25);
            this.tabAD9175.Name = "tabAD9175";
            this.tabAD9175.Padding = new System.Windows.Forms.Padding(3);
            this.tabAD9175.Size = new System.Drawing.Size(844, 655);
            this.tabAD9175.TabIndex = 1;
            this.tabAD9175.Text = "DAC 9175";
            this.tabAD9175.UseVisualStyleBackColor = true;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(6, 72);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(184, 16);
            this.label44.TabIndex = 30;
            this.label44.Text = "REGISTER VALUE[0xFF]:";
            // 
            // textRegDAC9175
            // 
            this.textRegDAC9175.Location = new System.Drawing.Point(9, 47);
            this.textRegDAC9175.Name = "textRegDAC9175";
            this.textRegDAC9175.Size = new System.Drawing.Size(94, 22);
            this.textRegDAC9175.TabIndex = 29;
            this.textRegDAC9175.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textRegDAC9175_KeyPress);
            // 
            // Cmd_Link_Status
            // 
            this.Cmd_Link_Status.Location = new System.Drawing.Point(725, 354);
            this.Cmd_Link_Status.Name = "Cmd_Link_Status";
            this.Cmd_Link_Status.Size = new System.Drawing.Size(113, 45);
            this.Cmd_Link_Status.TabIndex = 28;
            this.Cmd_Link_Status.Text = "Link Status";
            this.Cmd_Link_Status.UseVisualStyleBackColor = true;
            // 
            // Cmd_STP
            // 
            this.Cmd_STP.Location = new System.Drawing.Point(740, 582);
            this.Cmd_STP.Name = "Cmd_STP";
            this.Cmd_STP.Size = new System.Drawing.Size(83, 39);
            this.Cmd_STP.TabIndex = 27;
            this.Cmd_STP.Text = "STP";
            this.Cmd_STP.UseVisualStyleBackColor = true;
            this.Cmd_STP.Click += new System.EventHandler(this.Cmd_STP_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.Cmd_UpdateFS_Ioutfs);
            this.groupBox7.Controls.Add(this.label42);
            this.groupBox7.Controls.Add(this.numericDAC_FS);
            this.groupBox7.Controls.Add(this.label41);
            this.groupBox7.Location = new System.Drawing.Point(384, 513);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(335, 115);
            this.groupBox7.TabIndex = 26;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "FULL SCALE (Ioutfs):";
            // 
            // Cmd_UpdateFS_Ioutfs
            // 
            this.Cmd_UpdateFS_Ioutfs.Location = new System.Drawing.Point(82, 63);
            this.Cmd_UpdateFS_Ioutfs.Name = "Cmd_UpdateFS_Ioutfs";
            this.Cmd_UpdateFS_Ioutfs.Size = new System.Drawing.Size(100, 46);
            this.Cmd_UpdateFS_Ioutfs.TabIndex = 27;
            this.Cmd_UpdateFS_Ioutfs.Text = "Set Value";
            this.Cmd_UpdateFS_Ioutfs.UseVisualStyleBackColor = true;
            this.Cmd_UpdateFS_Ioutfs.Click += new System.EventHandler(this.Cmd_UpdateFS_Ioutfs_Click);
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(179, 35);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(85, 16);
            this.label42.TabIndex = 2;
            this.label42.Text = "< 25.977mA";
            // 
            // numericDAC_FS
            // 
            this.numericDAC_FS.DecimalPlaces = 3;
            this.numericDAC_FS.Increment = new decimal(new int[] {
            97,
            0,
            0,
            196608});
            this.numericDAC_FS.Location = new System.Drawing.Point(97, 33);
            this.numericDAC_FS.Maximum = new decimal(new int[] {
            25977,
            0,
            0,
            196608});
            this.numericDAC_FS.Minimum = new decimal(new int[] {
            15625,
            0,
            0,
            196608});
            this.numericDAC_FS.Name = "numericDAC_FS";
            this.numericDAC_FS.Size = new System.Drawing.Size(76, 22);
            this.numericDAC_FS.TabIndex = 1;
            this.numericDAC_FS.Value = new decimal(new int[] {
            19531,
            0,
            0,
            196608});
            this.numericDAC_FS.ValueChanged += new System.EventHandler(this.numericDAC_FS_ValueChanged);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(6, 35);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(85, 16);
            this.label41.TabIndex = 0;
            this.label41.Text = "15.625mA <";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.comboBox1);
            this.groupBox6.Controls.Add(this.Cmd_PRBS);
            this.groupBox6.Location = new System.Drawing.Point(384, 400);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(335, 61);
            this.groupBox6.TabIndex = 25;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "PRBS TEST:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "PRBS7",
            "PRBS15",
            "PRBS31"});
            this.comboBox1.Location = new System.Drawing.Point(6, 21);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(88, 24);
            this.comboBox1.TabIndex = 18;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // Cmd_PRBS
            // 
            this.Cmd_PRBS.Location = new System.Drawing.Point(131, 13);
            this.Cmd_PRBS.Name = "Cmd_PRBS";
            this.Cmd_PRBS.Size = new System.Drawing.Size(97, 38);
            this.Cmd_PRBS.TabIndex = 17;
            this.Cmd_PRBS.Text = "PRBS Test";
            this.Cmd_PRBS.UseVisualStyleBackColor = true;
            this.Cmd_PRBS.Click += new System.EventHandler(this.Cmd_PRBS_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(760, 532);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(78, 22);
            this.textBox4.TabIndex = 24;
            // 
            // Cmd_ReadRegAD9175
            // 
            this.Cmd_ReadRegAD9175.Location = new System.Drawing.Point(256, 72);
            this.Cmd_ReadRegAD9175.Name = "Cmd_ReadRegAD9175";
            this.Cmd_ReadRegAD9175.Size = new System.Drawing.Size(113, 45);
            this.Cmd_ReadRegAD9175.TabIndex = 20;
            this.Cmd_ReadRegAD9175.Text = "Read Register";
            this.Cmd_ReadRegAD9175.UseVisualStyleBackColor = true;
            this.Cmd_ReadRegAD9175.Click += new System.EventHandler(this.Cmd_ReadRegAD9175_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label43);
            this.groupBox5.Controls.Add(this.numericTime);
            this.groupBox5.Controls.Add(this.label31);
            this.groupBox5.Controls.Add(this.checkBox1);
            this.groupBox5.Controls.Add(this.label40);
            this.groupBox5.Controls.Add(this.numericTone_Amplitude);
            this.groupBox5.Controls.Add(this.label39);
            this.groupBox5.Controls.Add(this.Cmd_StartSweep);
            this.groupBox5.Controls.Add(this.label38);
            this.groupBox5.Controls.Add(this.label37);
            this.groupBox5.Controls.Add(this.label36);
            this.groupBox5.Controls.Add(this.textStop);
            this.groupBox5.Controls.Add(this.label35);
            this.groupBox5.Controls.Add(this.textStart);
            this.groupBox5.Controls.Add(this.textStep);
            this.groupBox5.Controls.Add(this.label34);
            this.groupBox5.Controls.Add(this.label33);
            this.groupBox5.Controls.Add(this.Cmd_NCO);
            this.groupBox5.Controls.Add(this.ComboDAC_index);
            this.groupBox5.Controls.Add(this.label25);
            this.groupBox5.Location = new System.Drawing.Point(6, 400);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(372, 228);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Calib NCO Freq:";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(174, 197);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(56, 16);
            this.label43.TabIndex = 33;
            this.label43.Text = "milisec";
            // 
            // numericTime
            // 
            this.numericTime.Location = new System.Drawing.Point(96, 195);
            this.numericTime.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericTime.Name = "numericTime";
            this.numericTime.Size = new System.Drawing.Size(72, 22);
            this.numericTime.TabIndex = 32;
            this.numericTime.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(8, 201);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(82, 16);
            this.label31.TabIndex = 31;
            this.label31.Text = "Step Time:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(250, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(106, 20);
            this.checkBox1.TabIndex = 30;
            this.checkBox1.Text = "Single NCO";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(175, 166);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(20, 16);
            this.label40.TabIndex = 29;
            this.label40.Text = "%";
            // 
            // numericTone_Amplitude
            // 
            this.numericTone_Amplitude.Location = new System.Drawing.Point(96, 164);
            this.numericTone_Amplitude.Name = "numericTone_Amplitude";
            this.numericTone_Amplitude.Size = new System.Drawing.Size(72, 22);
            this.numericTone_Amplitude.TabIndex = 28;
            this.numericTone_Amplitude.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(8, 170);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(82, 16);
            this.label39.TabIndex = 27;
            this.label39.Text = "Tone Amp:";
            // 
            // Cmd_StartSweep
            // 
            this.Cmd_StartSweep.Location = new System.Drawing.Point(254, 47);
            this.Cmd_StartSweep.Name = "Cmd_StartSweep";
            this.Cmd_StartSweep.Size = new System.Drawing.Size(97, 39);
            this.Cmd_StartSweep.TabIndex = 25;
            this.Cmd_StartSweep.Text = "Start";
            this.Cmd_StartSweep.UseVisualStyleBackColor = true;
            this.Cmd_StartSweep.Click += new System.EventHandler(this.Cmd_StartSweep_Click);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(175, 132);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(37, 16);
            this.label38.TabIndex = 24;
            this.label38.Text = "MHz";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(8, 132);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(75, 16);
            this.label37.TabIndex = 23;
            this.label37.Text = "StepFreq:";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(175, 94);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(37, 16);
            this.label36.TabIndex = 22;
            this.label36.Text = "MHz";
            // 
            // textStop
            // 
            this.textStop.Location = new System.Drawing.Point(91, 91);
            this.textStop.Name = "textStop";
            this.textStop.Size = new System.Drawing.Size(77, 22);
            this.textStop.TabIndex = 21;
            this.textStop.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textStop_KeyPress);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(174, 58);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(37, 16);
            this.label35.TabIndex = 20;
            this.label35.Text = "MHz";
            // 
            // textStart
            // 
            this.textStart.Location = new System.Drawing.Point(91, 55);
            this.textStart.Name = "textStart";
            this.textStart.Size = new System.Drawing.Size(77, 22);
            this.textStart.TabIndex = 19;
            this.textStart.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textStart_KeyPress);
            // 
            // textStep
            // 
            this.textStep.Location = new System.Drawing.Point(91, 129);
            this.textStep.Name = "textStep";
            this.textStep.Size = new System.Drawing.Size(77, 22);
            this.textStep.TabIndex = 16;
            this.textStep.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textStep_KeyPress);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 58);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(79, 16);
            this.label34.TabIndex = 18;
            this.label34.Text = "Start Freq:";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(6, 21);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(91, 16);
            this.label33.TabIndex = 17;
            this.label33.Text = "DAC INDEX:";
            // 
            // Cmd_NCO
            // 
            this.Cmd_NCO.Location = new System.Drawing.Point(254, 164);
            this.Cmd_NCO.Name = "Cmd_NCO";
            this.Cmd_NCO.Size = new System.Drawing.Size(97, 53);
            this.Cmd_NCO.TabIndex = 15;
            this.Cmd_NCO.Text = "Calib NCO";
            this.Cmd_NCO.UseVisualStyleBackColor = true;
            this.Cmd_NCO.Click += new System.EventHandler(this.Cmd_NCO_Click);
            // 
            // ComboDAC_index
            // 
            this.ComboDAC_index.FormattingEnabled = true;
            this.ComboDAC_index.Items.AddRange(new object[] {
            "DAC0",
            "DAC1"});
            this.ComboDAC_index.Location = new System.Drawing.Point(103, 18);
            this.ComboDAC_index.Name = "ComboDAC_index";
            this.ComboDAC_index.Size = new System.Drawing.Size(76, 24);
            this.ComboDAC_index.TabIndex = 16;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(8, 94);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(79, 16);
            this.label25.TabIndex = 14;
            this.label25.Text = "Stop Freq:";
            // 
            // Cmd_DAC_Init
            // 
            this.Cmd_DAC_Init.Location = new System.Drawing.Point(725, 87);
            this.Cmd_DAC_Init.Name = "Cmd_DAC_Init";
            this.Cmd_DAC_Init.Size = new System.Drawing.Size(113, 45);
            this.Cmd_DAC_Init.TabIndex = 13;
            this.Cmd_DAC_Init.Text = "DAC INIT";
            this.Cmd_DAC_Init.UseVisualStyleBackColor = true;
            this.Cmd_DAC_Init.Click += new System.EventHandler(this.Cmd_DAC_Init_Click);
            // 
            // Cmd_WriteDAC9175
            // 
            this.Cmd_WriteDAC9175.Location = new System.Drawing.Point(725, 290);
            this.Cmd_WriteDAC9175.Name = "Cmd_WriteDAC9175";
            this.Cmd_WriteDAC9175.Size = new System.Drawing.Size(114, 58);
            this.Cmd_WriteDAC9175.TabIndex = 12;
            this.Cmd_WriteDAC9175.Text = "Write To Device";
            this.Cmd_WriteDAC9175.UseVisualStyleBackColor = true;
            // 
            // Cmd_ReadDAC9175
            // 
            this.Cmd_ReadDAC9175.Location = new System.Drawing.Point(725, 226);
            this.Cmd_ReadDAC9175.Name = "Cmd_ReadDAC9175";
            this.Cmd_ReadDAC9175.Size = new System.Drawing.Size(114, 58);
            this.Cmd_ReadDAC9175.TabIndex = 11;
            this.Cmd_ReadDAC9175.Text = "Read From Device";
            this.Cmd_ReadDAC9175.UseVisualStyleBackColor = true;
            this.Cmd_ReadDAC9175.Click += new System.EventHandler(this.Cmd_ReadDAC9175_Click);
            // 
            // Cmd_WriteReg9175
            // 
            this.Cmd_WriteReg9175.Location = new System.Drawing.Point(256, 17);
            this.Cmd_WriteReg9175.Name = "Cmd_WriteReg9175";
            this.Cmd_WriteReg9175.Size = new System.Drawing.Size(113, 45);
            this.Cmd_WriteReg9175.TabIndex = 10;
            this.Cmd_WriteReg9175.Text = "Write Register";
            this.Cmd_WriteReg9175.UseVisualStyleBackColor = true;
            this.Cmd_WriteReg9175.Click += new System.EventHandler(this.Cmd_WriteReg9175_Click);
            // 
            // textDAC9175_Value
            // 
            this.textDAC9175_Value.Location = new System.Drawing.Point(9, 91);
            this.textDAC9175_Value.Name = "textDAC9175_Value";
            this.textDAC9175_Value.Size = new System.Drawing.Size(101, 22);
            this.textDAC9175_Value.TabIndex = 9;
            this.textDAC9175_Value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textDAC9175_Value_KeyPress);
            // 
            // comboRegisters9175
            // 
            this.comboRegisters9175.FormattingEnabled = true;
            this.comboRegisters9175.Location = new System.Drawing.Point(676, 37);
            this.comboRegisters9175.Name = "comboRegisters9175";
            this.comboRegisters9175.Size = new System.Drawing.Size(129, 24);
            this.comboRegisters9175.TabIndex = 5;
            this.comboRegisters9175.SelectedIndexChanged += new System.EventHandler(this.ComboRegisters9175_SelectedIndexChanged);
            // 
            // labelDAC9175_Register
            // 
            this.labelDAC9175_Register.AutoSize = true;
            this.labelDAC9175_Register.Location = new System.Drawing.Point(6, 27);
            this.labelDAC9175_Register.Name = "labelDAC9175_Register";
            this.labelDAC9175_Register.Size = new System.Drawing.Size(226, 16);
            this.labelDAC9175_Register.TabIndex = 4;
            this.labelDAC9175_Register.Text = "REGISTER ADDRESS[0xFFFF]:";
            // 
            // labelFilePath9175
            // 
            this.labelFilePath9175.AutoSize = true;
            this.labelFilePath9175.Location = new System.Drawing.Point(6, 3);
            this.labelFilePath9175.Name = "labelFilePath9175";
            this.labelFilePath9175.Size = new System.Drawing.Size(107, 16);
            this.labelFilePath9175.TabIndex = 3;
            this.labelFilePath9175.Text = "DAC File Path:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridViewAD9175);
            this.groupBox2.Location = new System.Drawing.Point(6, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(712, 254);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "DAC9175 REGISTER DATA:";
            // 
            // dataGridViewAD9175
            // 
            this.dataGridViewAD9175.AllowUserToAddRows = false;
            this.dataGridViewAD9175.AllowUserToDeleteRows = false;
            this.dataGridViewAD9175.AllowUserToResizeRows = false;
            this.dataGridViewAD9175.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAD9175.Location = new System.Drawing.Point(6, 21);
            this.dataGridViewAD9175.Name = "dataGridViewAD9175";
            this.dataGridViewAD9175.ReadOnly = true;
            this.dataGridViewAD9175.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewAD9175.Size = new System.Drawing.Size(700, 223);
            this.dataGridViewAD9175.TabIndex = 0;
            // 
            // Cmd_Export9175_file
            // 
            this.Cmd_Export9175_file.Enabled = false;
            this.Cmd_Export9175_file.Location = new System.Drawing.Point(725, 138);
            this.Cmd_Export9175_file.Name = "Cmd_Export9175_file";
            this.Cmd_Export9175_file.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Export9175_file.TabIndex = 1;
            this.Cmd_Export9175_file.Text = "Export File";
            this.Cmd_Export9175_file.UseVisualStyleBackColor = true;
            this.Cmd_Export9175_file.Click += new System.EventHandler(this.Cmd_Export9175_file_Click);
            // 
            // Cmd_Import9175_file
            // 
            this.Cmd_Import9175_file.Location = new System.Drawing.Point(725, 465);
            this.Cmd_Import9175_file.Name = "Cmd_Import9175_file";
            this.Cmd_Import9175_file.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Import9175_file.TabIndex = 0;
            this.Cmd_Import9175_file.Text = "Import File";
            this.Cmd_Import9175_file.UseVisualStyleBackColor = true;
            this.Cmd_Import9175_file.Click += new System.EventHandler(this.Cmd_Import9175_file_Click);
            // 
            // tabSi5518
            // 
            this.tabSi5518.Controls.Add(this.Cmd_Burn_SkyPLL);
            this.tabSi5518.Controls.Add(this.Cmd_Config);
            this.tabSi5518.Controls.Add(this.label28);
            this.tabSi5518.Controls.Add(this.button1);
            this.tabSi5518.Controls.Add(this.label30);
            this.tabSi5518.Controls.Add(this.label29);
            this.tabSi5518.Controls.Add(this.Cmd_Load_SI_FW);
            this.tabSi5518.Controls.Add(this.Cmd_Export_SkyWorks);
            this.tabSi5518.Controls.Add(this.Cmd_Import_SkyWorks);
            this.tabSi5518.Controls.Add(this.label27);
            this.tabSi5518.Location = new System.Drawing.Point(4, 25);
            this.tabSi5518.Name = "tabSi5518";
            this.tabSi5518.Size = new System.Drawing.Size(844, 655);
            this.tabSi5518.TabIndex = 4;
            this.tabSi5518.Text = "Si55XX";
            this.tabSi5518.UseVisualStyleBackColor = true;
            // 
            // Cmd_Burn_SkyPLL
            // 
            this.Cmd_Burn_SkyPLL.Location = new System.Drawing.Point(717, 368);
            this.Cmd_Burn_SkyPLL.Name = "Cmd_Burn_SkyPLL";
            this.Cmd_Burn_SkyPLL.Size = new System.Drawing.Size(114, 46);
            this.Cmd_Burn_SkyPLL.TabIndex = 10;
            this.Cmd_Burn_SkyPLL.Text = "Burn Files";
            this.Cmd_Burn_SkyPLL.UseVisualStyleBackColor = true;
            this.Cmd_Burn_SkyPLL.Click += new System.EventHandler(this.Cmd_Burn_SkyPLL_Click);
            // 
            // Cmd_Config
            // 
            this.Cmd_Config.Location = new System.Drawing.Point(717, 203);
            this.Cmd_Config.Name = "Cmd_Config";
            this.Cmd_Config.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Config.TabIndex = 9;
            this.Cmd_Config.Text = "Import User Config";
            this.Cmd_Config.UseVisualStyleBackColor = true;
            this.Cmd_Config.Click += new System.EventHandler(this.Cmd_Config_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(603, 476);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(75, 16);
            this.label28.TabIndex = 8;
            this.label28.Text = "Temp: 0.0";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(717, 461);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 47);
            this.button1.TabIndex = 7;
            this.button1.Text = "Get Temp";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(12, 122);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(81, 16);
            this.label30.TabIndex = 6;
            this.label30.Text = "PROD FW:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(12, 72);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(71, 16);
            this.label29.TabIndex = 5;
            this.label29.Text = "NVM FW:";
            // 
            // Cmd_Load_SI_FW
            // 
            this.Cmd_Load_SI_FW.Location = new System.Drawing.Point(717, 317);
            this.Cmd_Load_SI_FW.Name = "Cmd_Load_SI_FW";
            this.Cmd_Load_SI_FW.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Load_SI_FW.TabIndex = 4;
            this.Cmd_Load_SI_FW.Text = "Import Prod FW";
            this.Cmd_Load_SI_FW.UseVisualStyleBackColor = true;
            this.Cmd_Load_SI_FW.Click += new System.EventHandler(this.Cmd_Load_SI_FW_Click);
            // 
            // Cmd_Export_SkyWorks
            // 
            this.Cmd_Export_SkyWorks.Enabled = false;
            this.Cmd_Export_SkyWorks.Location = new System.Drawing.Point(15, 419);
            this.Cmd_Export_SkyWorks.Name = "Cmd_Export_SkyWorks";
            this.Cmd_Export_SkyWorks.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Export_SkyWorks.TabIndex = 3;
            this.Cmd_Export_SkyWorks.Text = "Export File";
            this.Cmd_Export_SkyWorks.UseVisualStyleBackColor = true;
            this.Cmd_Export_SkyWorks.Click += new System.EventHandler(this.Cmd_Export_SkyWorks_Click);
            // 
            // Cmd_Import_SkyWorks
            // 
            this.Cmd_Import_SkyWorks.Location = new System.Drawing.Point(717, 266);
            this.Cmd_Import_SkyWorks.Name = "Cmd_Import_SkyWorks";
            this.Cmd_Import_SkyWorks.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Import_SkyWorks.TabIndex = 2;
            this.Cmd_Import_SkyWorks.Text = "Import  NVM File";
            this.Cmd_Import_SkyWorks.UseVisualStyleBackColor = true;
            this.Cmd_Import_SkyWorks.Click += new System.EventHandler(this.Cmd_Import_SkyWorks_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(12, 21);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(92, 16);
            this.label27.TabIndex = 0;
            this.label27.Text = "User Config:";
            // 
            // tabRFLine
            // 
            this.tabRFLine.Controls.Add(this.numericATT3);
            this.tabRFLine.Controls.Add(this.numericATT2);
            this.tabRFLine.Controls.Add(this.numericATT1);
            this.tabRFLine.Controls.Add(this.checkAmp2);
            this.tabRFLine.Controls.Add(this.checkAmp1);
            this.tabRFLine.Controls.Add(this.Cmd_Read_ADC);
            this.tabRFLine.Controls.Add(this.label7);
            this.tabRFLine.Controls.Add(this.Cmd_UpdateTX_Values);
            this.tabRFLine.Controls.Add(this.label6);
            this.tabRFLine.Controls.Add(this.label19);
            this.tabRFLine.Controls.Add(this.label18);
            this.tabRFLine.Controls.Add(this.label17);
            this.tabRFLine.Controls.Add(this.label16);
            this.tabRFLine.Controls.Add(this.label15);
            this.tabRFLine.Controls.Add(this.label14);
            this.tabRFLine.Controls.Add(this.label12);
            this.tabRFLine.Controls.Add(this.label11);
            this.tabRFLine.Controls.Add(this.label10);
            this.tabRFLine.Controls.Add(this.pictureBox1);
            this.tabRFLine.Location = new System.Drawing.Point(4, 25);
            this.tabRFLine.Name = "tabRFLine";
            this.tabRFLine.Size = new System.Drawing.Size(844, 655);
            this.tabRFLine.TabIndex = 5;
            this.tabRFLine.Text = "TX RF LINEUP";
            this.tabRFLine.UseVisualStyleBackColor = true;
            // 
            // numericATT3
            // 
            this.numericATT3.DecimalPlaces = 2;
            this.numericATT3.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.numericATT3.Location = new System.Drawing.Point(613, 199);
            this.numericATT3.Name = "numericATT3";
            this.numericATT3.Size = new System.Drawing.Size(56, 22);
            this.numericATT3.TabIndex = 25;
            this.numericATT3.ValueChanged += new System.EventHandler(this.NumericATT3_ValueChanged);
            this.numericATT3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericATT3_KeyPress);
            // 
            // numericATT2
            // 
            this.numericATT2.DecimalPlaces = 2;
            this.numericATT2.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.numericATT2.Location = new System.Drawing.Point(454, 199);
            this.numericATT2.Name = "numericATT2";
            this.numericATT2.Size = new System.Drawing.Size(56, 22);
            this.numericATT2.TabIndex = 24;
            this.numericATT2.ValueChanged += new System.EventHandler(this.NumericATT2_ValueChanged);
            this.numericATT2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericATT2_KeyPress);
            // 
            // numericATT1
            // 
            this.numericATT1.DecimalPlaces = 2;
            this.numericATT1.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.numericATT1.Location = new System.Drawing.Point(233, 199);
            this.numericATT1.Name = "numericATT1";
            this.numericATT1.Size = new System.Drawing.Size(56, 22);
            this.numericATT1.TabIndex = 23;
            this.numericATT1.TabStop = false;
            this.numericATT1.ValueChanged += new System.EventHandler(this.NumericATT1_ValueChanged);
            this.numericATT1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericATT1_KeyPress);
            // 
            // checkAmp2
            // 
            this.checkAmp2.AutoSize = true;
            this.checkAmp2.Location = new System.Drawing.Point(525, 55);
            this.checkAmp2.Name = "checkAmp2";
            this.checkAmp2.Size = new System.Drawing.Size(134, 20);
            this.checkAmp2.TabIndex = 22;
            this.checkAmp2.Text = "BYPASS MODE";
            this.checkAmp2.UseVisualStyleBackColor = true;
            this.checkAmp2.CheckedChanged += new System.EventHandler(this.CheckAmp2_CheckedChanged);
            // 
            // checkAmp1
            // 
            this.checkAmp1.AutoSize = true;
            this.checkAmp1.Location = new System.Drawing.Point(287, 55);
            this.checkAmp1.Name = "checkAmp1";
            this.checkAmp1.Size = new System.Drawing.Size(134, 20);
            this.checkAmp1.TabIndex = 21;
            this.checkAmp1.Text = "BYPASS MODE";
            this.checkAmp1.UseVisualStyleBackColor = true;
            this.checkAmp1.CheckedChanged += new System.EventHandler(this.CheckAmp1_CheckedChanged);
            // 
            // Cmd_Read_ADC
            // 
            this.Cmd_Read_ADC.Location = new System.Drawing.Point(13, 412);
            this.Cmd_Read_ADC.Name = "Cmd_Read_ADC";
            this.Cmd_Read_ADC.Size = new System.Drawing.Size(163, 51);
            this.Cmd_Read_ADC.TabIndex = 20;
            this.Cmd_Read_ADC.Text = "READ ADC VALUE";
            this.Cmd_Read_ADC.UseVisualStyleBackColor = true;
            this.Cmd_Read_ADC.Click += new System.EventHandler(this.Cmd_Read_ADC_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(158, 374);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 16);
            this.label7.TabIndex = 14;
            this.label7.Text = "0.0 V";
            // 
            // Cmd_UpdateTX_Values
            // 
            this.Cmd_UpdateTX_Values.Location = new System.Drawing.Point(689, 412);
            this.Cmd_UpdateTX_Values.Name = "Cmd_UpdateTX_Values";
            this.Cmd_UpdateTX_Values.Size = new System.Drawing.Size(140, 51);
            this.Cmd_UpdateTX_Values.TabIndex = 16;
            this.Cmd_UpdateTX_Values.Text = "UPDATE RF ATT VALUES";
            this.Cmd_UpdateTX_Values.UseVisualStyleBackColor = true;
            this.Cmd_UpdateTX_Values.Click += new System.EventHandler(this.Cmd_UpdateTX_Values_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(10, 374);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 16);
            this.label6.TabIndex = 13;
            this.label6.Text = "RF ADC7091 Value:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(675, 199);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(26, 16);
            this.label19.TabIndex = 15;
            this.label19.Text = "dB";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(513, 199);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(26, 16);
            this.label18.TabIndex = 14;
            this.label18.Text = "dB";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(292, 199);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(26, 16);
            this.label17.TabIndex = 13;
            this.label17.Text = "dB";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(610, 178);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(88, 16);
            this.label16.TabIndex = 11;
            this.label16.Text = "3. HMC1119";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(522, 28);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(88, 16);
            this.label15.TabIndex = 9;
            this.label15.Text = "2. HMC8414";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(451, 178);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 16);
            this.label14.TabIndex = 7;
            this.label14.Text = "2. HMC1119";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(284, 28);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(88, 16);
            this.label12.TabIndex = 6;
            this.label12.Text = "1. HMC8414";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(230, 177);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 16);
            this.label11.TabIndex = 4;
            this.label11.Text = "1. HMC1119";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(766, 98);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 18);
            this.label10.TabIndex = 2;
            this.label10.Text = "RF OUT";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 86);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(816, 235);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tabFPGA
            // 
            this.tabFPGA.Controls.Add(this.comboBox2);
            this.tabFPGA.Controls.Add(this.button4);
            this.tabFPGA.Controls.Add(this.button3);
            this.tabFPGA.Controls.Add(this.button2);
            this.tabFPGA.Controls.Add(this.Cmd_FPGA_Tests);
            this.tabFPGA.Controls.Add(this.Cmd_WriteFPGA);
            this.tabFPGA.Controls.Add(this.label22);
            this.tabFPGA.Controls.Add(this.Cmd_Read_Registers);
            this.tabFPGA.Controls.Add(this.Cmd_LoadCounter);
            this.tabFPGA.Controls.Add(this.Cmd_FPGA_Read);
            this.tabFPGA.Controls.Add(this.textFPGA_Value);
            this.tabFPGA.Controls.Add(this.label21);
            this.tabFPGA.Controls.Add(this.label9);
            this.tabFPGA.Controls.Add(this.Cmd_FPGA_Export);
            this.tabFPGA.Controls.Add(this.Cmd_FPGA_Write);
            this.tabFPGA.Controls.Add(this.textFPGA_Output);
            this.tabFPGA.Controls.Add(this.textFPGA_Address);
            this.tabFPGA.Controls.Add(this.Cmd_FPGA_Import);
            this.tabFPGA.Location = new System.Drawing.Point(4, 25);
            this.tabFPGA.Name = "tabFPGA";
            this.tabFPGA.Size = new System.Drawing.Size(844, 655);
            this.tabFPGA.TabIndex = 7;
            this.tabFPGA.Text = "FPGA";
            this.tabFPGA.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "DAC0",
            "DAC1",
            "ADC0",
            "modem uplink0",
            "modem uplink1",
            "modem uplink2",
            "modem uplink3",
            "modem donwlink0",
            "modem donwlink1",
            "modem donwlink2",
            "modem donwlink3"});
            this.comboBox2.Location = new System.Drawing.Point(666, 420);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(125, 24);
            this.comboBox2.TabIndex = 17;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(670, 499);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(125, 38);
            this.button4.TabIndex = 16;
            this.button4.Text = "Stop Player";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(670, 450);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(125, 43);
            this.button3.TabIndex = 15;
            this.button3.Text = "Activate Player";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(666, 18);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(145, 52);
            this.button2.TabIndex = 14;
            this.button2.Text = "LOAD FPGA JESD204";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Cmd_FPGA_Tests
            // 
            this.Cmd_FPGA_Tests.Location = new System.Drawing.Point(666, 350);
            this.Cmd_FPGA_Tests.Name = "Cmd_FPGA_Tests";
            this.Cmd_FPGA_Tests.Size = new System.Drawing.Size(145, 55);
            this.Cmd_FPGA_Tests.TabIndex = 13;
            this.Cmd_FPGA_Tests.Text = "FPGA Tests";
            this.Cmd_FPGA_Tests.UseVisualStyleBackColor = true;
            // 
            // Cmd_WriteFPGA
            // 
            this.Cmd_WriteFPGA.Location = new System.Drawing.Point(666, 289);
            this.Cmd_WriteFPGA.Name = "Cmd_WriteFPGA";
            this.Cmd_WriteFPGA.Size = new System.Drawing.Size(145, 55);
            this.Cmd_WriteFPGA.TabIndex = 12;
            this.Cmd_WriteFPGA.Text = "Write to FPGA";
            this.Cmd_WriteFPGA.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(20, 104);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(96, 16);
            this.label22.TabIndex = 11;
            this.label22.Text = "FPGA DATA:";
            // 
            // Cmd_Read_Registers
            // 
            this.Cmd_Read_Registers.Location = new System.Drawing.Point(666, 228);
            this.Cmd_Read_Registers.Name = "Cmd_Read_Registers";
            this.Cmd_Read_Registers.Size = new System.Drawing.Size(145, 55);
            this.Cmd_Read_Registers.TabIndex = 10;
            this.Cmd_Read_Registers.Text = "Read from FPGA";
            this.Cmd_Read_Registers.UseVisualStyleBackColor = true;
            this.Cmd_Read_Registers.Click += new System.EventHandler(this.Cmd_Read_Registers_Click);
            // 
            // Cmd_LoadCounter
            // 
            this.Cmd_LoadCounter.Location = new System.Drawing.Point(666, 579);
            this.Cmd_LoadCounter.Name = "Cmd_LoadCounter";
            this.Cmd_LoadCounter.Size = new System.Drawing.Size(145, 54);
            this.Cmd_LoadCounter.TabIndex = 9;
            this.Cmd_LoadCounter.Text = "Load Counter";
            this.Cmd_LoadCounter.UseVisualStyleBackColor = true;
            this.Cmd_LoadCounter.Click += new System.EventHandler(this.Cmd_LoadCounter_Click);
            // 
            // Cmd_FPGA_Read
            // 
            this.Cmd_FPGA_Read.Location = new System.Drawing.Point(325, 58);
            this.Cmd_FPGA_Read.Name = "Cmd_FPGA_Read";
            this.Cmd_FPGA_Read.Size = new System.Drawing.Size(105, 39);
            this.Cmd_FPGA_Read.TabIndex = 8;
            this.Cmd_FPGA_Read.Text = "Read Data";
            this.Cmd_FPGA_Read.UseVisualStyleBackColor = true;
            this.Cmd_FPGA_Read.Click += new System.EventHandler(this.Cmd_FPGA_Read_Click);
            // 
            // textFPGA_Value
            // 
            this.textFPGA_Value.Location = new System.Drawing.Point(201, 55);
            this.textFPGA_Value.Name = "textFPGA_Value";
            this.textFPGA_Value.Size = new System.Drawing.Size(117, 22);
            this.textFPGA_Value.TabIndex = 7;
            this.textFPGA_Value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextFPGA_Value_KeyPress);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(198, 36);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(60, 16);
            this.label21.TabIndex = 6;
            this.label21.Text = "VALUE:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(128, 16);
            this.label9.TabIndex = 5;
            this.label9.Text = "FPGA ADDRESS:";
            // 
            // Cmd_FPGA_Export
            // 
            this.Cmd_FPGA_Export.Location = new System.Drawing.Point(666, 166);
            this.Cmd_FPGA_Export.Name = "Cmd_FPGA_Export";
            this.Cmd_FPGA_Export.Size = new System.Drawing.Size(145, 45);
            this.Cmd_FPGA_Export.TabIndex = 4;
            this.Cmd_FPGA_Export.Text = "Export Data";
            this.Cmd_FPGA_Export.UseVisualStyleBackColor = true;
            this.Cmd_FPGA_Export.Click += new System.EventHandler(this.Cmd_FPGA_Export_Click);
            // 
            // Cmd_FPGA_Write
            // 
            this.Cmd_FPGA_Write.Location = new System.Drawing.Point(325, 13);
            this.Cmd_FPGA_Write.Name = "Cmd_FPGA_Write";
            this.Cmd_FPGA_Write.Size = new System.Drawing.Size(105, 39);
            this.Cmd_FPGA_Write.TabIndex = 3;
            this.Cmd_FPGA_Write.Text = "Write Data";
            this.Cmd_FPGA_Write.UseVisualStyleBackColor = true;
            this.Cmd_FPGA_Write.Click += new System.EventHandler(this.Cmd_FPGA_Write_Click);
            // 
            // textFPGA_Output
            // 
            this.textFPGA_Output.Location = new System.Drawing.Point(23, 123);
            this.textFPGA_Output.Multiline = true;
            this.textFPGA_Output.Name = "textFPGA_Output";
            this.textFPGA_Output.ReadOnly = true;
            this.textFPGA_Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textFPGA_Output.Size = new System.Drawing.Size(506, 510);
            this.textFPGA_Output.TabIndex = 2;
            // 
            // textFPGA_Address
            // 
            this.textFPGA_Address.Location = new System.Drawing.Point(23, 55);
            this.textFPGA_Address.Name = "textFPGA_Address";
            this.textFPGA_Address.Size = new System.Drawing.Size(172, 22);
            this.textFPGA_Address.TabIndex = 1;
            this.textFPGA_Address.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextFPGA_Address_KeyPress);
            // 
            // Cmd_FPGA_Import
            // 
            this.Cmd_FPGA_Import.Location = new System.Drawing.Point(666, 90);
            this.Cmd_FPGA_Import.Name = "Cmd_FPGA_Import";
            this.Cmd_FPGA_Import.Size = new System.Drawing.Size(145, 45);
            this.Cmd_FPGA_Import.TabIndex = 0;
            this.Cmd_FPGA_Import.Text = "LOAD Vectors";
            this.Cmd_FPGA_Import.UseVisualStyleBackColor = true;
            this.Cmd_FPGA_Import.Click += new System.EventHandler(this.Cmd_FPGA_Import_Click);
            // 
            // tabMux
            // 
            this.tabMux.Controls.Add(this.Cmd_I2C_Read);
            this.tabMux.Controls.Add(this.Cmd_I2C_Write);
            this.tabMux.Controls.Add(this.label24);
            this.tabMux.Controls.Add(this.label23);
            this.tabMux.Controls.Add(this.textI2C_Val);
            this.tabMux.Controls.Add(this.textI2C_Reg);
            this.tabMux.Controls.Add(this.groupBox4);
            this.tabMux.Controls.Add(this.label13);
            this.tabMux.Controls.Add(this.label8);
            this.tabMux.Controls.Add(this.comboDevice);
            this.tabMux.Controls.Add(this.groupBox3);
            this.tabMux.Location = new System.Drawing.Point(4, 25);
            this.tabMux.Name = "tabMux";
            this.tabMux.Size = new System.Drawing.Size(844, 655);
            this.tabMux.TabIndex = 6;
            this.tabMux.Text = "TX MUXES";
            this.tabMux.UseVisualStyleBackColor = true;
            // 
            // Cmd_I2C_Read
            // 
            this.Cmd_I2C_Read.Location = new System.Drawing.Point(135, 409);
            this.Cmd_I2C_Read.Name = "Cmd_I2C_Read";
            this.Cmd_I2C_Read.Size = new System.Drawing.Size(107, 54);
            this.Cmd_I2C_Read.TabIndex = 13;
            this.Cmd_I2C_Read.Text = "READ";
            this.Cmd_I2C_Read.UseVisualStyleBackColor = true;
            this.Cmd_I2C_Read.Click += new System.EventHandler(this.Cmd_I2C_Read_Click);
            // 
            // Cmd_I2C_Write
            // 
            this.Cmd_I2C_Write.Location = new System.Drawing.Point(135, 349);
            this.Cmd_I2C_Write.Name = "Cmd_I2C_Write";
            this.Cmd_I2C_Write.Size = new System.Drawing.Size(107, 54);
            this.Cmd_I2C_Write.TabIndex = 12;
            this.Cmd_I2C_Write.Text = "WRITE";
            this.Cmd_I2C_Write.UseVisualStyleBackColor = true;
            this.Cmd_I2C_Write.Click += new System.EventHandler(this.Cmd_I2C_Write_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(8, 412);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(78, 16);
            this.label24.TabIndex = 11;
            this.label24.Text = "I2C DATA:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(8, 368);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(114, 16);
            this.label23.TabIndex = 10;
            this.label23.Text = "I2C REGISTER:";
            // 
            // textI2C_Val
            // 
            this.textI2C_Val.Location = new System.Drawing.Point(9, 431);
            this.textI2C_Val.Name = "textI2C_Val";
            this.textI2C_Val.Size = new System.Drawing.Size(107, 22);
            this.textI2C_Val.TabIndex = 9;
            this.textI2C_Val.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextI2C_Val_KeyPress);
            // 
            // textI2C_Reg
            // 
            this.textI2C_Reg.Location = new System.Drawing.Point(9, 387);
            this.textI2C_Reg.Name = "textI2C_Reg";
            this.textI2C_Reg.Size = new System.Drawing.Size(107, 22);
            this.textI2C_Reg.TabIndex = 8;
            this.textI2C_Reg.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextI2C_Reg_KeyPress);
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(200, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(293, 217);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "IO EXPANDER";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(629, 12);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(203, 16);
            this.label13.TabIndex = 3;
            this.label13.Text = "DEBUG/MANUAL CONTROL";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 272);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 16);
            this.label8.TabIndex = 2;
            this.label8.Text = "DEVICE MUX:";
            // 
            // comboDevice
            // 
            this.comboDevice.FormattingEnabled = true;
            this.comboDevice.Items.AddRange(new object[] {
            "DAC9175",
            "PLL4368",
            "ADC7091",
            "Si5518",
            "PCAL6416A",
            "FPGA"});
            this.comboDevice.Location = new System.Drawing.Point(9, 291);
            this.comboDevice.Name = "comboDevice";
            this.comboDevice.Size = new System.Drawing.Size(158, 24);
            this.comboDevice.TabIndex = 1;
            this.comboDevice.SelectedIndexChanged += new System.EventHandler(this.ComboDevice_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioMUX);
            this.groupBox3.Controls.Add(this.radioFPGA);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(177, 106);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "FPGA_MUX SWITCH";
            // 
            // radioMUX
            // 
            this.radioMUX.AutoSize = true;
            this.radioMUX.Checked = true;
            this.radioMUX.Location = new System.Drawing.Point(6, 52);
            this.radioMUX.Name = "radioMUX";
            this.radioMUX.Size = new System.Drawing.Size(118, 20);
            this.radioMUX.TabIndex = 1;
            this.radioMUX.TabStop = true;
            this.radioMUX.Text = "MUX (default)";
            this.radioMUX.UseVisualStyleBackColor = true;
            // 
            // radioFPGA
            // 
            this.radioFPGA.AutoSize = true;
            this.radioFPGA.Location = new System.Drawing.Point(6, 26);
            this.radioFPGA.Name = "radioFPGA";
            this.radioFPGA.Size = new System.Drawing.Size(65, 20);
            this.radioFPGA.TabIndex = 1;
            this.radioFPGA.Text = "FPGA";
            this.radioFPGA.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "FTDI Temp:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(111, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "N/A °C";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(312, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "RF Temp: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(397, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "N/A °C";
            // 
            // Cmd_FT_Temp_Read
            // 
            this.Cmd_FT_Temp_Read.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_FT_Temp_Read.Location = new System.Drawing.Point(169, 9);
            this.Cmd_FT_Temp_Read.Name = "Cmd_FT_Temp_Read";
            this.Cmd_FT_Temp_Read.Size = new System.Drawing.Size(103, 34);
            this.Cmd_FT_Temp_Read.TabIndex = 17;
            this.Cmd_FT_Temp_Read.Text = "TEMP Read";
            this.Cmd_FT_Temp_Read.UseVisualStyleBackColor = true;
            this.Cmd_FT_Temp_Read.Click += new System.EventHandler(this.Cmd_FT_Temp_Read_Click);
            // 
            // Cmd_RF_Temp_Read
            // 
            this.Cmd_RF_Temp_Read.Location = new System.Drawing.Point(455, 9);
            this.Cmd_RF_Temp_Read.Name = "Cmd_RF_Temp_Read";
            this.Cmd_RF_Temp_Read.Size = new System.Drawing.Size(103, 34);
            this.Cmd_RF_Temp_Read.TabIndex = 18;
            this.Cmd_RF_Temp_Read.Text = "TEMP Read";
            this.Cmd_RF_Temp_Read.UseVisualStyleBackColor = true;
            this.Cmd_RF_Temp_Read.Click += new System.EventHandler(this.Cmd_RF_Temp_Read_Click);
            // 
            // textLog
            // 
            this.textLog.Location = new System.Drawing.Point(874, 161);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLog.Size = new System.Drawing.Size(274, 477);
            this.textLog.TabIndex = 19;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(873, 139);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(72, 16);
            this.label20.TabIndex = 20;
            this.label20.Text = "Log data:";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::BringUp_Control.Properties.Resources.satixfy_logo_hg;
            this.pictureBox2.Location = new System.Drawing.Point(974, 9);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(174, 146);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 774);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.textLog);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.Cmd_RF_Temp_Read);
            this.Controls.Add(this.Cmd_FT_Temp_Read);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cmd_Exit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabWelcome.ResumeLayout(false);
            this.tabWelcome.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.tabAD4368.ResumeLayout(false);
            this.tabAD4368.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAD4368)).EndInit();
            this.tabAD9175.ResumeLayout(false);
            this.tabAD9175.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericDAC_FS)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTone_Amplitude)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAD9175)).EndInit();
            this.tabSi5518.ResumeLayout(false);
            this.tabSi5518.PerformLayout();
            this.tabRFLine.ResumeLayout(false);
            this.tabRFLine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericATT3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericATT2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericATT1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabFPGA.ResumeLayout(false);
            this.tabFPGA.PerformLayout();
            this.tabMux.ResumeLayout(false);
            this.tabMux.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cmd_Exit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAD4368;
        private System.Windows.Forms.TabPage tabAD9175;
        private System.Windows.Forms.ComboBox comboMUXOUT;
        private System.Windows.Forms.Label labelMUXOUT;
        private System.Windows.Forms.ComboBox comboRegAddress;
        private System.Windows.Forms.Label labelRegAddress;
        private System.Windows.Forms.Label labelFilePathAD4368;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridViewAD4368;
        private System.Windows.Forms.Button Cmd_WriteReg_AD4368;
        private System.Windows.Forms.TextBox textAD4368_Value;
        private System.Windows.Forms.Button Cmd_WriteAll_AD4368;
        private System.Windows.Forms.Button Cmd_ReadAll_AD4368;
        private System.Windows.Forms.Button Cmd_Export_AD4368_File;
        private System.Windows.Forms.Button Cmd_PowerONOFF;
        private System.Windows.Forms.Label labelFilePath9175;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Cmd_Export9175_file;
        private System.Windows.Forms.Button Cmd_Import9175_file;
        private System.Windows.Forms.CheckBox checkRFLOCK;
        private System.Windows.Forms.RadioButton radioRF_POWER_Status;
        private System.Windows.Forms.DataGridView dataGridViewAD9175;
        private System.Windows.Forms.Button Cmd_WriteReg9175;
        private System.Windows.Forms.TextBox textDAC9175_Value;
        private System.Windows.Forms.ComboBox comboRegisters9175;
        private System.Windows.Forms.Label labelDAC9175_Register;
        private System.Windows.Forms.TabPage tabSi5518;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabRFLine;
        private System.Windows.Forms.TabPage tabMux;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioMUX;
        private System.Windows.Forms.RadioButton radioFPGA;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboDevice;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button Cmd_FT_Temp_Read;
        private System.Windows.Forms.Button Cmd_RF_Temp_Read;
        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button Cmd_WriteDAC9175;
        private System.Windows.Forms.Button Cmd_ReadDAC9175;
        private System.Windows.Forms.Button Cmd_UpdateTX_Values;
        private System.Windows.Forms.Button Cmd_Read_ADC;
        private System.Windows.Forms.Button Cmd_DAC_Init;
        private System.Windows.Forms.CheckBox checkAmp2;
        private System.Windows.Forms.CheckBox checkAmp1;
        private System.Windows.Forms.TabPage tabFPGA;
        private System.Windows.Forms.Button Cmd_FPGA_Write;
        private System.Windows.Forms.TextBox textFPGA_Output;
        private System.Windows.Forms.TextBox textFPGA_Address;
        private System.Windows.Forms.Button Cmd_FPGA_Import;
        private System.Windows.Forms.TextBox textFPGA_Value;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button Cmd_FPGA_Export;
        private System.Windows.Forms.Button Cmd_FPGA_Read;
        private System.Windows.Forms.Button Cmd_LoadCounter;
        private System.Windows.Forms.Button Cmd_Read_Registers;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox textI2C_Val;
        private System.Windows.Forms.TextBox textI2C_Reg;
        private System.Windows.Forms.Button Cmd_I2C_Write;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button Cmd_I2C_Read;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button Cmd_WriteFPGA;
        private System.Windows.Forms.TabPage tabWelcome;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.NumericUpDown numericATT1;
        private System.Windows.Forms.NumericUpDown numericATT3;
        private System.Windows.Forms.NumericUpDown numericATT2;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button Cmd_Export_SkyWorks;
        private System.Windows.Forms.Button Cmd_Import_SkyWorks;
        private System.Windows.Forms.Button Cmd_FPGA_Tests;
        private System.Windows.Forms.Button Cmd_Load_SI_FW;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Button Cmd_Config;
        private System.Windows.Forms.Button Cmd_Burn_SkyPLL;
        private System.Windows.Forms.Button Cmd_NCO;
        private System.Windows.Forms.TextBox textStep;
        private System.Windows.Forms.Button Cmd_PRBS;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button Cmd_RFPLL_Init;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.ComboBox ComboDAC_index;
        private System.Windows.Forms.Button Cmd_ReadRegAD9175;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox textStop;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox textStart;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.NumericUpDown numericTone_Amplitude;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Button Cmd_StartSweep;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.NumericUpDown numericDAC_FS;
        private System.Windows.Forms.Button Cmd_UpdateFS_Ioutfs;
        private System.Windows.Forms.Button Cmd_STP;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.NumericUpDown numericTime;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TextBox textRegDAC9175;
        private System.Windows.Forms.Button Cmd_Link_Status;
    }
}

