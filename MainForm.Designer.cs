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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Cmd_Exit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAD4368 = new System.Windows.Forms.TabPage();
            this.checkRFLOCK = new System.Windows.Forms.CheckBox();
            this.Cmd_AD4368_INIT = new System.Windows.Forms.Button();
            this.Cmd_PowerONOFF = new System.Windows.Forms.Button();
            this.radioRF_POWER_Status = new System.Windows.Forms.RadioButton();
            this.Cmd_WriteAll_AD4368 = new System.Windows.Forms.Button();
            this.Cmd_ReadAll_AD4368 = new System.Windows.Forms.Button();
            this.Cmd_Export_AD4368_File = new System.Windows.Forms.Button();
            this.Cmd_Import_AD4368_File = new System.Windows.Forms.Button();
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
            this.tabAD9213 = new System.Windows.Forms.TabPage();
            this.tabSi5518 = new System.Windows.Forms.TabPage();
            this.tabRFLine = new System.Windows.Forms.TabPage();
            this.checkAmp2 = new System.Windows.Forms.CheckBox();
            this.checkAmp1 = new System.Windows.Forms.CheckBox();
            this.Cmd_Read_ADC = new System.Windows.Forms.Button();
            this.textATT3 = new System.Windows.Forms.TextBox();
            this.textATT2 = new System.Windows.Forms.TextBox();
            this.textATT1 = new System.Windows.Forms.TextBox();
            this.Cmd_UpdateTX_Values = new System.Windows.Forms.Button();
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
            this.tabMux = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioMUX = new System.Windows.Forms.RadioButton();
            this.radioFPGA = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Cmd_Init_All = new System.Windows.Forms.Button();
            this.Cmd_FT_Temp_Read = new System.Windows.Forms.Button();
            this.Cmd_RF_Temp_Read = new System.Windows.Forms.Button();
            this.textLog = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tabFPGA = new System.Windows.Forms.TabPage();
            this.Cmd_FPGA_Import = new System.Windows.Forms.Button();
            this.textFPGA = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.Cmd_FPGA_Write = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabAD4368.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAD4368)).BeginInit();
            this.tabAD9175.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAD9175)).BeginInit();
            this.tabRFLine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabMux.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabFPGA.SuspendLayout();
            this.SuspendLayout();
            // 
            // Cmd_Exit
            // 
            this.Cmd_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_Exit.Location = new System.Drawing.Point(993, 679);
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
            this.tabControl1.Controls.Add(this.tabAD4368);
            this.tabControl1.Controls.Add(this.tabAD9175);
            this.tabControl1.Controls.Add(this.tabAD9213);
            this.tabControl1.Controls.Add(this.tabSi5518);
            this.tabControl1.Controls.Add(this.tabRFLine);
            this.tabControl1.Controls.Add(this.tabMux);
            this.tabControl1.Controls.Add(this.tabFPGA);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(15, 65);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(852, 684);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabAD4368
            // 
            this.tabAD4368.Controls.Add(this.checkRFLOCK);
            this.tabAD4368.Controls.Add(this.Cmd_AD4368_INIT);
            this.tabAD4368.Controls.Add(this.Cmd_PowerONOFF);
            this.tabAD4368.Controls.Add(this.radioRF_POWER_Status);
            this.tabAD4368.Controls.Add(this.Cmd_WriteAll_AD4368);
            this.tabAD4368.Controls.Add(this.Cmd_ReadAll_AD4368);
            this.tabAD4368.Controls.Add(this.Cmd_Export_AD4368_File);
            this.tabAD4368.Controls.Add(this.Cmd_Import_AD4368_File);
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
            // checkRFLOCK
            // 
            this.checkRFLOCK.AutoSize = true;
            this.checkRFLOCK.Enabled = false;
            this.checkRFLOCK.Location = new System.Drawing.Point(606, 516);
            this.checkRFLOCK.Name = "checkRFLOCK";
            this.checkRFLOCK.Size = new System.Drawing.Size(94, 20);
            this.checkRFLOCK.TabIndex = 16;
            this.checkRFLOCK.Text = "PLL LOCK";
            this.checkRFLOCK.UseVisualStyleBackColor = true;
            // 
            // Cmd_AD4368_INIT
            // 
            this.Cmd_AD4368_INIT.Enabled = false;
            this.Cmd_AD4368_INIT.Location = new System.Drawing.Point(606, 440);
            this.Cmd_AD4368_INIT.Name = "Cmd_AD4368_INIT";
            this.Cmd_AD4368_INIT.Size = new System.Drawing.Size(114, 58);
            this.Cmd_AD4368_INIT.TabIndex = 15;
            this.Cmd_AD4368_INIT.Text = "PLL INIT";
            this.Cmd_AD4368_INIT.UseVisualStyleBackColor = true;
            this.Cmd_AD4368_INIT.Click += new System.EventHandler(this.Cmd_AD4368_INIT_Click);
            // 
            // Cmd_PowerONOFF
            // 
            this.Cmd_PowerONOFF.Enabled = false;
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
            this.Cmd_WriteAll_AD4368.Location = new System.Drawing.Point(606, 376);
            this.Cmd_WriteAll_AD4368.Name = "Cmd_WriteAll_AD4368";
            this.Cmd_WriteAll_AD4368.Size = new System.Drawing.Size(114, 58);
            this.Cmd_WriteAll_AD4368.TabIndex = 12;
            this.Cmd_WriteAll_AD4368.Text = "Write To Device";
            this.Cmd_WriteAll_AD4368.UseVisualStyleBackColor = true;
            this.Cmd_WriteAll_AD4368.Click += new System.EventHandler(this.Cmd_WriteAll_AD4368_Click);
            // 
            // Cmd_ReadAll_AD4368
            // 
            this.Cmd_ReadAll_AD4368.Enabled = false;
            this.Cmd_ReadAll_AD4368.Location = new System.Drawing.Point(606, 312);
            this.Cmd_ReadAll_AD4368.Name = "Cmd_ReadAll_AD4368";
            this.Cmd_ReadAll_AD4368.Size = new System.Drawing.Size(114, 58);
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
            // Cmd_Import_AD4368_File
            // 
            this.Cmd_Import_AD4368_File.Location = new System.Drawing.Point(606, 159);
            this.Cmd_Import_AD4368_File.Name = "Cmd_Import_AD4368_File";
            this.Cmd_Import_AD4368_File.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Import_AD4368_File.TabIndex = 9;
            this.Cmd_Import_AD4368_File.Text = "Import File";
            this.Cmd_Import_AD4368_File.UseVisualStyleBackColor = true;
            this.Cmd_Import_AD4368_File.Click += new System.EventHandler(this.Cmd_Import_AD4368_File_Click);
            // 
            // Cmd_WriteReg_AD4368
            // 
            this.Cmd_WriteReg_AD4368.Location = new System.Drawing.Point(462, 52);
            this.Cmd_WriteReg_AD4368.Name = "Cmd_WriteReg_AD4368";
            this.Cmd_WriteReg_AD4368.Size = new System.Drawing.Size(101, 63);
            this.Cmd_WriteReg_AD4368.TabIndex = 7;
            this.Cmd_WriteReg_AD4368.Text = "Write Register";
            this.Cmd_WriteReg_AD4368.UseVisualStyleBackColor = true;
            this.Cmd_WriteReg_AD4368.Click += new System.EventHandler(this.Cmd_WriteReg_AD4368_Click);
            // 
            // textAD4368_Value
            // 
            this.textAD4368_Value.Location = new System.Drawing.Point(334, 72);
            this.textAD4368_Value.Name = "textAD4368_Value";
            this.textAD4368_Value.Size = new System.Drawing.Size(116, 22);
            this.textAD4368_Value.TabIndex = 6;
            this.textAD4368_Value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textAD4368_Value_KeyPress);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridViewAD4368);
            this.groupBox1.Location = new System.Drawing.Point(13, 159);
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
            "0101: DivNCLK/2"});
            this.comboMUXOUT.Location = new System.Drawing.Point(181, 115);
            this.comboMUXOUT.Name = "comboMUXOUT";
            this.comboMUXOUT.Size = new System.Drawing.Size(129, 24);
            this.comboMUXOUT.TabIndex = 4;
            this.comboMUXOUT.SelectedIndexChanged += new System.EventHandler(this.comboMUXOUT_SelectedIndexChanged);
            // 
            // labelMUXOUT
            // 
            this.labelMUXOUT.AutoSize = true;
            this.labelMUXOUT.Location = new System.Drawing.Point(100, 118);
            this.labelMUXOUT.Name = "labelMUXOUT";
            this.labelMUXOUT.Size = new System.Drawing.Size(75, 16);
            this.labelMUXOUT.TabIndex = 3;
            this.labelMUXOUT.Text = "MUXOUT:";
            // 
            // comboRegAddress
            // 
            this.comboRegAddress.FormattingEnabled = true;
            this.comboRegAddress.Location = new System.Drawing.Point(181, 72);
            this.comboRegAddress.Name = "comboRegAddress";
            this.comboRegAddress.Size = new System.Drawing.Size(129, 24);
            this.comboRegAddress.TabIndex = 2;
            this.comboRegAddress.SelectedIndexChanged += new System.EventHandler(this.comboRegAddress_SelectedIndexChanged);
            // 
            // labelRegAddress
            // 
            this.labelRegAddress.AutoSize = true;
            this.labelRegAddress.Location = new System.Drawing.Point(10, 75);
            this.labelRegAddress.Name = "labelRegAddress";
            this.labelRegAddress.Size = new System.Drawing.Size(165, 16);
            this.labelRegAddress.TabIndex = 1;
            this.labelRegAddress.Text = "REGISTER ADDRESS:";
            // 
            // labelFilePathAD4368
            // 
            this.labelFilePathAD4368.AutoSize = true;
            this.labelFilePathAD4368.Location = new System.Drawing.Point(10, 31);
            this.labelFilePathAD4368.Name = "labelFilePathAD4368";
            this.labelFilePathAD4368.Size = new System.Drawing.Size(72, 16);
            this.labelFilePathAD4368.TabIndex = 0;
            this.labelFilePathAD4368.Text = "File Path:";
            // 
            // tabAD9175
            // 
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
            // Cmd_DAC_Init
            // 
            this.Cmd_DAC_Init.Location = new System.Drawing.Point(725, 400);
            this.Cmd_DAC_Init.Name = "Cmd_DAC_Init";
            this.Cmd_DAC_Init.Size = new System.Drawing.Size(114, 53);
            this.Cmd_DAC_Init.TabIndex = 13;
            this.Cmd_DAC_Init.Text = "DAC INIT";
            this.Cmd_DAC_Init.UseVisualStyleBackColor = true;
            this.Cmd_DAC_Init.Click += new System.EventHandler(this.Cmd_DAC_Init_Click);
            // 
            // Cmd_WriteDAC9175
            // 
            this.Cmd_WriteDAC9175.Location = new System.Drawing.Point(725, 336);
            this.Cmd_WriteDAC9175.Name = "Cmd_WriteDAC9175";
            this.Cmd_WriteDAC9175.Size = new System.Drawing.Size(114, 58);
            this.Cmd_WriteDAC9175.TabIndex = 12;
            this.Cmd_WriteDAC9175.Text = "Write To Device";
            this.Cmd_WriteDAC9175.UseVisualStyleBackColor = true;
            // 
            // Cmd_ReadDAC9175
            // 
            this.Cmd_ReadDAC9175.Location = new System.Drawing.Point(725, 272);
            this.Cmd_ReadDAC9175.Name = "Cmd_ReadDAC9175";
            this.Cmd_ReadDAC9175.Size = new System.Drawing.Size(114, 58);
            this.Cmd_ReadDAC9175.TabIndex = 11;
            this.Cmd_ReadDAC9175.Text = "Read From Device";
            this.Cmd_ReadDAC9175.UseVisualStyleBackColor = true;
            // 
            // Cmd_WriteReg9175
            // 
            this.Cmd_WriteReg9175.Enabled = false;
            this.Cmd_WriteReg9175.Location = new System.Drawing.Point(462, 52);
            this.Cmd_WriteReg9175.Name = "Cmd_WriteReg9175";
            this.Cmd_WriteReg9175.Size = new System.Drawing.Size(101, 63);
            this.Cmd_WriteReg9175.TabIndex = 10;
            this.Cmd_WriteReg9175.Text = "Write Register";
            this.Cmd_WriteReg9175.UseVisualStyleBackColor = true;
            // 
            // textDAC9175_Value
            // 
            this.textDAC9175_Value.Location = new System.Drawing.Point(334, 72);
            this.textDAC9175_Value.Name = "textDAC9175_Value";
            this.textDAC9175_Value.Size = new System.Drawing.Size(101, 22);
            this.textDAC9175_Value.TabIndex = 9;
            // 
            // comboRegisters9175
            // 
            this.comboRegisters9175.FormattingEnabled = true;
            this.comboRegisters9175.Location = new System.Drawing.Point(181, 72);
            this.comboRegisters9175.Name = "comboRegisters9175";
            this.comboRegisters9175.Size = new System.Drawing.Size(129, 24);
            this.comboRegisters9175.TabIndex = 5;
            this.comboRegisters9175.SelectedIndexChanged += new System.EventHandler(this.comboRegisters9175_SelectedIndexChanged);
            // 
            // labelDAC9175_Register
            // 
            this.labelDAC9175_Register.AutoSize = true;
            this.labelDAC9175_Register.Location = new System.Drawing.Point(10, 75);
            this.labelDAC9175_Register.Name = "labelDAC9175_Register";
            this.labelDAC9175_Register.Size = new System.Drawing.Size(165, 16);
            this.labelDAC9175_Register.TabIndex = 4;
            this.labelDAC9175_Register.Text = "REGISTER ADDRESS:";
            // 
            // labelFilePath9175
            // 
            this.labelFilePath9175.AutoSize = true;
            this.labelFilePath9175.Location = new System.Drawing.Point(10, 31);
            this.labelFilePath9175.Name = "labelFilePath9175";
            this.labelFilePath9175.Size = new System.Drawing.Size(107, 16);
            this.labelFilePath9175.TabIndex = 3;
            this.labelFilePath9175.Text = "DAC File Path:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridViewAD9175);
            this.groupBox2.Location = new System.Drawing.Point(7, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(712, 509);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "AD9175 REGISTER DATA:";
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
            this.dataGridViewAD9175.Size = new System.Drawing.Size(700, 482);
            this.dataGridViewAD9175.TabIndex = 0;
            // 
            // Cmd_Export9175_file
            // 
            this.Cmd_Export9175_file.Enabled = false;
            this.Cmd_Export9175_file.Location = new System.Drawing.Point(725, 191);
            this.Cmd_Export9175_file.Name = "Cmd_Export9175_file";
            this.Cmd_Export9175_file.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Export9175_file.TabIndex = 1;
            this.Cmd_Export9175_file.Text = "Export File";
            this.Cmd_Export9175_file.UseVisualStyleBackColor = true;
            this.Cmd_Export9175_file.Click += new System.EventHandler(this.Cmd_Export9175_file_Click);
            // 
            // Cmd_Import9175_file
            // 
            this.Cmd_Import9175_file.Location = new System.Drawing.Point(725, 140);
            this.Cmd_Import9175_file.Name = "Cmd_Import9175_file";
            this.Cmd_Import9175_file.Size = new System.Drawing.Size(114, 45);
            this.Cmd_Import9175_file.TabIndex = 0;
            this.Cmd_Import9175_file.Text = "Import File";
            this.Cmd_Import9175_file.UseVisualStyleBackColor = true;
            this.Cmd_Import9175_file.Click += new System.EventHandler(this.Cmd_Import9175_file_Click);
            // 
            // tabAD9213
            // 
            this.tabAD9213.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabAD9213.Location = new System.Drawing.Point(4, 25);
            this.tabAD9213.Name = "tabAD9213";
            this.tabAD9213.Size = new System.Drawing.Size(844, 655);
            this.tabAD9213.TabIndex = 2;
            this.tabAD9213.Text = "ADC 9213";
            this.tabAD9213.UseVisualStyleBackColor = true;
            // 
            // tabSi5518
            // 
            this.tabSi5518.Location = new System.Drawing.Point(4, 25);
            this.tabSi5518.Name = "tabSi5518";
            this.tabSi5518.Size = new System.Drawing.Size(844, 655);
            this.tabSi5518.TabIndex = 4;
            this.tabSi5518.Text = "Si55XX";
            this.tabSi5518.UseVisualStyleBackColor = true;
            // 
            // tabRFLine
            // 
            this.tabRFLine.Controls.Add(this.checkAmp2);
            this.tabRFLine.Controls.Add(this.checkAmp1);
            this.tabRFLine.Controls.Add(this.Cmd_Read_ADC);
            this.tabRFLine.Controls.Add(this.textATT3);
            this.tabRFLine.Controls.Add(this.textATT2);
            this.tabRFLine.Controls.Add(this.textATT1);
            this.tabRFLine.Controls.Add(this.Cmd_UpdateTX_Values);
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
            // checkAmp2
            // 
            this.checkAmp2.AutoSize = true;
            this.checkAmp2.Location = new System.Drawing.Point(525, 55);
            this.checkAmp2.Name = "checkAmp2";
            this.checkAmp2.Size = new System.Drawing.Size(112, 20);
            this.checkAmp2.TabIndex = 22;
            this.checkAmp2.Text = "BYPASS ON";
            this.checkAmp2.UseVisualStyleBackColor = true;
            // 
            // checkAmp1
            // 
            this.checkAmp1.AutoSize = true;
            this.checkAmp1.Location = new System.Drawing.Point(287, 55);
            this.checkAmp1.Name = "checkAmp1";
            this.checkAmp1.Size = new System.Drawing.Size(112, 20);
            this.checkAmp1.TabIndex = 21;
            this.checkAmp1.Text = "BYPASS ON";
            this.checkAmp1.UseVisualStyleBackColor = true;
            // 
            // Cmd_Read_ADC
            // 
            this.Cmd_Read_ADC.Location = new System.Drawing.Point(440, 357);
            this.Cmd_Read_ADC.Name = "Cmd_Read_ADC";
            this.Cmd_Read_ADC.Size = new System.Drawing.Size(114, 51);
            this.Cmd_Read_ADC.TabIndex = 20;
            this.Cmd_Read_ADC.Text = "READ ADC VALUE";
            this.Cmd_Read_ADC.UseVisualStyleBackColor = true;
            this.Cmd_Read_ADC.Click += new System.EventHandler(this.Cmd_Read_ADC_Click);
            // 
            // textATT3
            // 
            this.textATT3.Location = new System.Drawing.Point(613, 197);
            this.textATT3.Name = "textATT3";
            this.textATT3.Size = new System.Drawing.Size(56, 22);
            this.textATT3.TabIndex = 19;
            this.textATT3.Text = "0";
            this.textATT3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textATT3_KeyPress);
            // 
            // textATT2
            // 
            this.textATT2.Location = new System.Drawing.Point(454, 197);
            this.textATT2.Name = "textATT2";
            this.textATT2.Size = new System.Drawing.Size(56, 22);
            this.textATT2.TabIndex = 18;
            this.textATT2.Text = "0";
            this.textATT2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textATT2_KeyPress);
            // 
            // textATT1
            // 
            this.textATT1.Location = new System.Drawing.Point(233, 196);
            this.textATT1.Name = "textATT1";
            this.textATT1.Size = new System.Drawing.Size(56, 22);
            this.textATT1.TabIndex = 17;
            this.textATT1.Text = "0";
            this.textATT1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textATT1_KeyPress);
            // 
            // Cmd_UpdateTX_Values
            // 
            this.Cmd_UpdateTX_Values.Location = new System.Drawing.Point(715, 357);
            this.Cmd_UpdateTX_Values.Name = "Cmd_UpdateTX_Values";
            this.Cmd_UpdateTX_Values.Size = new System.Drawing.Size(114, 51);
            this.Cmd_UpdateTX_Values.TabIndex = 16;
            this.Cmd_UpdateTX_Values.Text = "UPDATE RF VALUES";
            this.Cmd_UpdateTX_Values.UseVisualStyleBackColor = true;
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
            this.label10.Location = new System.Drawing.Point(778, 55);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 16);
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
            // tabMux
            // 
            this.tabMux.Controls.Add(this.groupBox4);
            this.tabMux.Controls.Add(this.label13);
            this.tabMux.Controls.Add(this.label8);
            this.tabMux.Controls.Add(this.comboBox1);
            this.tabMux.Controls.Add(this.groupBox3);
            this.tabMux.Location = new System.Drawing.Point(4, 25);
            this.tabMux.Name = "tabMux";
            this.tabMux.Size = new System.Drawing.Size(844, 655);
            this.tabMux.TabIndex = 6;
            this.tabMux.Text = "TX MUXES";
            this.tabMux.UseVisualStyleBackColor = true;
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
            this.label8.Location = new System.Drawing.Point(3, 151);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 16);
            this.label8.TabIndex = 2;
            this.label8.Text = "DEVICE MUX:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "DAC9175",
            "PLL4368",
            "ADC7091",
            "SKYPLLSi5518"});
            this.comboBox1.Location = new System.Drawing.Point(3, 170);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(158, 24);
            this.comboBox1.TabIndex = 1;
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
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
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
            this.label3.Size = new System.Drawing.Size(42, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "00 °C";
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
            this.label5.Size = new System.Drawing.Size(42, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "00 °C";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(585, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 16);
            this.label6.TabIndex = 13;
            this.label6.Text = "RF ADC Value:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(701, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 16);
            this.label7.TabIndex = 14;
            this.label7.Text = "0xFF";
            // 
            // Cmd_Init_All
            // 
            this.Cmd_Init_All.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_Init_All.Location = new System.Drawing.Point(993, 606);
            this.Cmd_Init_All.Name = "Cmd_Init_All";
            this.Cmd_Init_All.Size = new System.Drawing.Size(129, 46);
            this.Cmd_Init_All.TabIndex = 16;
            this.Cmd_Init_All.Text = "Init ALL Devices";
            this.Cmd_Init_All.UseVisualStyleBackColor = true;
            this.Cmd_Init_All.Click += new System.EventHandler(this.Cmd_Init_All_Click);
            // 
            // Cmd_FT_Temp_Read
            // 
            this.Cmd_FT_Temp_Read.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_FT_Temp_Read.Location = new System.Drawing.Point(159, 9);
            this.Cmd_FT_Temp_Read.Name = "Cmd_FT_Temp_Read";
            this.Cmd_FT_Temp_Read.Size = new System.Drawing.Size(103, 34);
            this.Cmd_FT_Temp_Read.TabIndex = 17;
            this.Cmd_FT_Temp_Read.Text = "TEMP Read";
            this.Cmd_FT_Temp_Read.UseVisualStyleBackColor = true;
            this.Cmd_FT_Temp_Read.Click += new System.EventHandler(this.Cmd_FT_Temp_Read_Click);
            // 
            // Cmd_RF_Temp_Read
            // 
            this.Cmd_RF_Temp_Read.Location = new System.Drawing.Point(445, 9);
            this.Cmd_RF_Temp_Read.Name = "Cmd_RF_Temp_Read";
            this.Cmd_RF_Temp_Read.Size = new System.Drawing.Size(103, 34);
            this.Cmd_RF_Temp_Read.TabIndex = 18;
            this.Cmd_RF_Temp_Read.Text = "TEMP Read";
            this.Cmd_RF_Temp_Read.UseVisualStyleBackColor = true;
            this.Cmd_RF_Temp_Read.Click += new System.EventHandler(this.Cmd_RF_Temp_Read_Click);
            // 
            // textLog
            // 
            this.textLog.Location = new System.Drawing.Point(874, 90);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLog.Size = new System.Drawing.Size(274, 424);
            this.textLog.TabIndex = 19;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(873, 74);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(72, 16);
            this.label20.TabIndex = 20;
            this.label20.Text = "Log data:";
            // 
            // tabFPGA
            // 
            this.tabFPGA.Controls.Add(this.Cmd_FPGA_Write);
            this.tabFPGA.Controls.Add(this.textBox2);
            this.tabFPGA.Controls.Add(this.textFPGA);
            this.tabFPGA.Controls.Add(this.Cmd_FPGA_Import);
            this.tabFPGA.Location = new System.Drawing.Point(4, 25);
            this.tabFPGA.Name = "tabFPGA";
            this.tabFPGA.Size = new System.Drawing.Size(844, 655);
            this.tabFPGA.TabIndex = 7;
            this.tabFPGA.Text = "FPGA";
            this.tabFPGA.UseVisualStyleBackColor = true;
            // 
            // Cmd_FPGA_Import
            // 
            this.Cmd_FPGA_Import.Location = new System.Drawing.Point(666, 44);
            this.Cmd_FPGA_Import.Name = "Cmd_FPGA_Import";
            this.Cmd_FPGA_Import.Size = new System.Drawing.Size(145, 45);
            this.Cmd_FPGA_Import.TabIndex = 0;
            this.Cmd_FPGA_Import.Text = "Import Data";
            this.Cmd_FPGA_Import.UseVisualStyleBackColor = true;
            // 
            // textFPGA
            // 
            this.textFPGA.Location = new System.Drawing.Point(23, 55);
            this.textFPGA.Name = "textFPGA";
            this.textFPGA.Size = new System.Drawing.Size(220, 22);
            this.textFPGA.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(23, 123);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(506, 510);
            this.textBox2.TabIndex = 2;
            // 
            // Cmd_FPGA_Write
            // 
            this.Cmd_FPGA_Write.Location = new System.Drawing.Point(315, 44);
            this.Cmd_FPGA_Write.Name = "Cmd_FPGA_Write";
            this.Cmd_FPGA_Write.Size = new System.Drawing.Size(105, 39);
            this.Cmd_FPGA_Write.TabIndex = 3;
            this.Cmd_FPGA_Write.Text = "Write Data";
            this.Cmd_FPGA_Write.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 774);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.textLog);
            this.Controls.Add(this.Cmd_RF_Temp_Read);
            this.Controls.Add(this.Cmd_FT_Temp_Read);
            this.Controls.Add(this.Cmd_Init_All);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
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
            this.tabAD4368.ResumeLayout(false);
            this.tabAD4368.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAD4368)).EndInit();
            this.tabAD9175.ResumeLayout(false);
            this.tabAD9175.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAD9175)).EndInit();
            this.tabRFLine.ResumeLayout(false);
            this.tabRFLine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabMux.ResumeLayout(false);
            this.tabMux.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabFPGA.ResumeLayout(false);
            this.tabFPGA.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cmd_Exit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAD4368;
        private System.Windows.Forms.TabPage tabAD9175;
        private System.Windows.Forms.TabPage tabAD9213;
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
        private System.Windows.Forms.Button Cmd_Import_AD4368_File;
        private System.Windows.Forms.Button Cmd_PowerONOFF;
        private System.Windows.Forms.Button Cmd_AD4368_INIT;
        private System.Windows.Forms.Label labelFilePath9175;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Cmd_Export9175_file;
        private System.Windows.Forms.Button Cmd_Import9175_file;
        private System.Windows.Forms.CheckBox checkRFLOCK;
        private System.Windows.Forms.RadioButton radioRF_POWER_Status;
        private System.Windows.Forms.Timer timer1;
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
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button Cmd_Init_All;
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
        private System.Windows.Forms.TextBox textATT3;
        private System.Windows.Forms.TextBox textATT2;
        private System.Windows.Forms.TextBox textATT1;
        private System.Windows.Forms.Button Cmd_Read_ADC;
        private System.Windows.Forms.Button Cmd_DAC_Init;
        private System.Windows.Forms.CheckBox checkAmp2;
        private System.Windows.Forms.CheckBox checkAmp1;
        private System.Windows.Forms.TabPage tabFPGA;
        private System.Windows.Forms.Button Cmd_FPGA_Write;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textFPGA;
        private System.Windows.Forms.Button Cmd_FPGA_Import;
    }
}

