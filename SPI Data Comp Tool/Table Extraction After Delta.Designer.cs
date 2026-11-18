namespace SPI_Data_Comp_Tool
{
    partial class Table_Extraction_After_Delta_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Table_Extraction_After_Delta_Form));
            this.components = new System.ComponentModel.Container();
            this.gb_DBConnection = new System.Windows.Forms.GroupBox();
            this.btn_TestDBConnection = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gb_Selective = new System.Windows.Forms.GroupBox();
            this.cb_CHG_NUM_From_Excel = new System.Windows.Forms.CheckBox();
            this.cb_ExcelSheetName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btn_Excel = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnOLDvsNew = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btn_Chg_Num = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btn_Chg_NumVsChg_Num = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.lblTimer = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bgw_TableExtractionAfterDelta = new System.ComponentModel.BackgroundWorker();
            this.bgw_old_VS_new = new System.ComponentModel.BackgroundWorker();
            this.bgw_Chg_Num = new System.ComponentModel.BackgroundWorker();
            this.bgw_Chg_numVsChg_Num = new System.ComponentModel.BackgroundWorker();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Input = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Selective = new System.Windows.Forms.TableLayoutPanel();
            this.flp_Timer = new System.Windows.Forms.FlowLayoutPanel();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.gb_DBConnection.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlp_Input.SuspendLayout();
            this.gb_Selective.SuspendLayout();
            this.tlp_Selective.SuspendLayout();
            this.flp_Timer.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tlp_Status.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tlp_Root.RowCount = 2;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.gb_DBConnection, 0, 0);
            this.tlp_Root.Controls.Add(this.groupBox1, 1, 0);
            this.tlp_Root.Controls.Add(this.groupBox10, 0, 1);
            this.tlp_Root.SetColumnSpan(this.groupBox10, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // gb_DBConnection
            // 
            this.gb_DBConnection.Name = "gb_DBConnection";
            this.gb_DBConnection.TabStop = false;
            this.gb_DBConnection.Text = "Connection";
            this.gb_DBConnection.Controls.Add(this.tlp_Connection);
            this.gb_DBConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_DBConnection.AutoSize = true;
            this.gb_DBConnection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gb_DBConnection.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.gb_DBConnection.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_DBConnection.TabIndex = 1;
            // 
            // tlp_Connection
            // 
            this.tlp_Connection.Name = "tlp_Connection";
            this.tlp_Connection.ColumnCount = 1;
            this.tlp_Connection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Connection.RowCount = 2;
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.Controls.Add(this.connection, 0, 0);
            this.tlp_Connection.Controls.Add(this.btn_TestDBConnection, 0, 1);
            this.tlp_Connection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Connection.AutoSize = true;
            this.tlp_Connection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Connection.TabIndex = 2;
            // 
            // connection
            // 
            this.connection.Name = "connection";
            this.connection.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.connection.AutoSize = true;
            this.connection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.connection.TabIndex = 3;
            // 
            // btn_TestDBConnection
            // 
            this.btn_TestDBConnection.Name = "btn_TestDBConnection";
            this.btn_TestDBConnection.UseVisualStyleBackColor = true;
            this.btn_TestDBConnection.Click += new System.EventHandler(this.btn_TestDBConnection_Click);
            this.btn_TestDBConnection.Text = "Test both connections";
            this.btn_TestDBConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btn_TestDBConnection.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.btn_TestDBConnection.Size = new System.Drawing.Size(170, 30);
            this.btn_TestDBConnection.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input";
            this.groupBox1.Controls.Add(this.tlp_Input);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox1.TabIndex = 5;
            // 
            // tlp_Input
            // 
            this.tlp_Input.Name = "tlp_Input";
            this.tlp_Input.ColumnCount = 1;
            this.tlp_Input.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Input.RowCount = 4;
            this.tlp_Input.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Input.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Input.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Input.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Input.Controls.Add(this.gb_Selective, 0, 0);
            this.tlp_Input.Controls.Add(this.tabControl1, 0, 1);
            this.tlp_Input.Controls.Add(this.btn_Cancel, 0, 2);
            this.tlp_Input.Controls.Add(this.flp_Timer, 0, 3);
            this.tlp_Input.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Input.AutoSize = false;
            this.tlp_Input.TabIndex = 6;
            // 
            // gb_Selective
            // 
            this.gb_Selective.Name = "gb_Selective";
            this.gb_Selective.TabStop = false;
            this.gb_Selective.Text = "Excel input";
            this.gb_Selective.Controls.Add(this.tlp_Selective);
            this.gb_Selective.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Selective.AutoSize = true;
            this.gb_Selective.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gb_Selective.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.gb_Selective.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Selective.TabIndex = 7;
            // 
            // tlp_Selective
            // 
            this.tlp_Selective.Name = "tlp_Selective";
            this.tlp_Selective.ColumnCount = 1;
            this.tlp_Selective.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Selective.RowCount = 2;
            this.tlp_Selective.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Selective.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Selective.Controls.Add(this.cb_CHG_NUM_From_Excel, 0, 0);
            this.tlp_Selective.Controls.Add(this.cb_ExcelSheetName, 0, 1);
            this.tlp_Selective.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Selective.AutoSize = true;
            this.tlp_Selective.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Selective.TabIndex = 8;
            // 
            // cb_CHG_NUM_From_Excel
            // 
            this.cb_CHG_NUM_From_Excel.Name = "cb_CHG_NUM_From_Excel";
            this.cb_CHG_NUM_From_Excel.UseVisualStyleBackColor = true;
            this.cb_CHG_NUM_From_Excel.CheckedChanged += new System.EventHandler(this.cb_CHG_NUM_From_Excel_CheckedChanged);
            this.cb_CHG_NUM_From_Excel.Text = "Input from Excel";
            this.cb_CHG_NUM_From_Excel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_CHG_NUM_From_Excel.AutoSize = true;
            this.cb_CHG_NUM_From_Excel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_CHG_NUM_From_Excel.TabIndex = 9;
            // 
            // cb_ExcelSheetName
            // 
            this.cb_ExcelSheetName.BackColor = System.Drawing.SystemColors.Window;
            this.cb_ExcelSheetName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ExcelSheetName.Enabled = false;
            this.cb_ExcelSheetName.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_ExcelSheetName.FormattingEnabled = true;
            this.cb_ExcelSheetName.Name = "cb_ExcelSheetName";
            this.cb_ExcelSheetName.SelectedIndexChanged += new System.EventHandler(this.cb_ExcelSheetName_SelectedIndexChanged);
            this.cb_ExcelSheetName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.cb_ExcelSheetName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_ExcelSheetName.TabIndex = 10;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tabControl1.TabIndex = 11;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.btn_Cancel.Size = new System.Drawing.Size(100, 30);
            this.btn_Cancel.TabIndex = 12;
            // 
            // flp_Timer
            // 
            this.flp_Timer.Name = "flp_Timer";
            this.flp_Timer.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flp_Timer.WrapContents = false;
            this.flp_Timer.Controls.Add(this.label1);
            this.flp_Timer.Controls.Add(this.lblTimer);
            this.flp_Timer.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.flp_Timer.AutoSize = true;
            this.flp_Timer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flp_Timer.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flp_Timer.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Name = "label1";
            this.label1.Text = "Elapsed";
            this.label1.AutoSize = true;
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.label1.TabIndex = 14;
            // 
            // lblTimer
            // 
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Text = "";
            this.lblTimer.AutoSize = true;
            this.lblTimer.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblTimer.TabIndex = 15;
            // 
            // groupBox10
            // 
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Status";
            this.groupBox10.Controls.Add(this.tlp_Status);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBox10.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox10.TabIndex = 16;
            // 
            // tlp_Status
            // 
            this.tlp_Status.Name = "tlp_Status";
            this.tlp_Status.ColumnCount = 2;
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.RowCount = 2;
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Status.Controls.Add(this.progressBar1, 0, 0);
            this.tlp_Status.Controls.Add(this.btn_Clear_Reset, 1, 0);
            this.tlp_Status.Controls.Add(this.rtb_Log, 0, 1);
            this.tlp_Status.SetColumnSpan(this.rtb_Log, 2);
            this.tlp_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Status.AutoSize = false;
            this.tlp_Status.TabIndex = 17;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 18;
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Name = "btn_Clear_Reset";
            this.btn_Clear_Reset.UseVisualStyleBackColor = true;
            this.btn_Clear_Reset.Click += new System.EventHandler(this.btn_Clear_Reset_Click);
            this.btn_Clear_Reset.Text = "Clear / Reset";
            this.btn_Clear_Reset.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Clear_Reset.Size = new System.Drawing.Size(110, 30);
            this.btn_Clear_Reset.TabIndex = 19;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.rtb_Log.TabIndex = 20;
            // 
            // btn_Excel
            // 
            this.btn_Excel.Location = new System.Drawing.Point(12, 19);
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.Size = new System.Drawing.Size(167, 23);
            this.btn_Excel.TabIndex = 0;
            this.btn_Excel.Text = "Table Extraction After Delta";
            this.btn_Excel.UseVisualStyleBackColor = true;
            this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // bgw_TableExtractionAfterDelta
            // 
            this.bgw_TableExtractionAfterDelta.WorkerReportsProgress = true;
            this.bgw_TableExtractionAfterDelta.WorkerSupportsCancellation = true;
            this.bgw_TableExtractionAfterDelta.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_TableExtractionAfterDelta_DoWork);
            this.bgw_TableExtractionAfterDelta.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_TableExtractionAfterDelta_ProgressChanged);
            this.bgw_TableExtractionAfterDelta.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_TableExtractionAfterDelta_RunWorkerCompleted);
            // 
            // btnOLDvsNew
            // 
            this.btnOLDvsNew.Location = new System.Drawing.Point(17, 19);
            this.btnOLDvsNew.Name = "btnOLDvsNew";
            this.btnOLDvsNew.Size = new System.Drawing.Size(167, 23);
            this.btnOLDvsNew.TabIndex = 21;
            this.btnOLDvsNew.Text = "OLD VS NEW";
            this.btnOLDvsNew.UseVisualStyleBackColor = true;
            this.btnOLDvsNew.Click += new System.EventHandler(this.btnOLDvsNew_Click);
            // 
            // bgw_old_VS_new
            // 
            this.bgw_old_VS_new.WorkerReportsProgress = true;
            this.bgw_old_VS_new.WorkerSupportsCancellation = true;
            this.bgw_old_VS_new.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_old_VS_new_DoWork);
            this.bgw_old_VS_new.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_old_VS_new_ProgressChanged);
            this.bgw_old_VS_new.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_old_VS_new_RunWorkerCompleted);
            // 
            // btn_Chg_Num
            // 
            this.btn_Chg_Num.Location = new System.Drawing.Point(19, 19);
            this.btn_Chg_Num.Name = "btn_Chg_Num";
            this.btn_Chg_Num.Size = new System.Drawing.Size(167, 23);
            this.btn_Chg_Num.TabIndex = 24;
            this.btn_Chg_Num.Text = "Change Num";
            this.btn_Chg_Num.UseVisualStyleBackColor = true;
            this.btn_Chg_Num.Click += new System.EventHandler(this.btn_Chg_Num_Click);
            // 
            // bgw_Chg_Num
            // 
            this.bgw_Chg_Num.WorkerReportsProgress = true;
            this.bgw_Chg_Num.WorkerSupportsCancellation = true;
            this.bgw_Chg_Num.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_Chg_Num_DoWork);
            this.bgw_Chg_Num.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_Chg_Num_ProgressChanged);
            this.bgw_Chg_Num.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_Chg_Num_RunWorkerCompleted);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btn_Excel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(452, 150);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Table Extraction After Delta";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnOLDvsNew);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(452, 150);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Old Versus New";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btn_Chg_Num);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(452, 150);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Change Num";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btn_Chg_NumVsChg_Num);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(452, 150);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "CHG_NUM VS CHG_NUM";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btn_Chg_NumVsChg_Num
            // 
            this.btn_Chg_NumVsChg_Num.Location = new System.Drawing.Point(23, 19);
            this.btn_Chg_NumVsChg_Num.Name = "btn_Chg_NumVsChg_Num";
            this.btn_Chg_NumVsChg_Num.Size = new System.Drawing.Size(167, 23);
            this.btn_Chg_NumVsChg_Num.TabIndex = 25;
            this.btn_Chg_NumVsChg_Num.Text = "Chg_Num Vs Chg_Num";
            this.btn_Chg_NumVsChg_Num.UseVisualStyleBackColor = true;
            this.btn_Chg_NumVsChg_Num.Click += new System.EventHandler(this.btn_Chg_NumVsChg_Num_Click);
            // 
            // bgw_Chg_numVsChg_Num
            // 
            this.bgw_Chg_numVsChg_Num.WorkerReportsProgress = true;
            this.bgw_Chg_numVsChg_Num.WorkerSupportsCancellation = true;
            this.bgw_Chg_numVsChg_Num.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_Chg_numVsChg_Num_DoWork);
            this.bgw_Chg_numVsChg_Num.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_Chg_numVsChg_Num_ProgressChanged);
            this.bgw_Chg_numVsChg_Num.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_Chg_numVsChg_Num_RunWorkerCompleted);
            // 
            // Table_Extraction_After_Delta_Form
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Table_Extraction_After_Delta_Form";
            this.Text = "Table extraction after delta";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 860);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.gb_DBConnection.ResumeLayout(false);
            this.gb_DBConnection.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlp_Input.ResumeLayout(false);
            this.tlp_Input.PerformLayout();
            this.gb_Selective.ResumeLayout(false);
            this.gb_Selective.PerformLayout();
            this.tlp_Selective.ResumeLayout(false);
            this.tlp_Selective.PerformLayout();
            this.flp_Timer.ResumeLayout(false);
            this.flp_Timer.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Input;
        private System.Windows.Forms.TableLayoutPanel tlp_Selective;
        private System.Windows.Forms.FlowLayoutPanel flp_Timer;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;

        private System.Windows.Forms.GroupBox gb_DBConnection;
        private System.Windows.Forms.Button btn_TestDBConnection;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_Clear_Reset;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_Excel;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker bgw_TableExtractionAfterDelta;
        private System.Windows.Forms.Button btnOLDvsNew;
        private System.ComponentModel.BackgroundWorker bgw_old_VS_new;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Chg_Num;
        private System.ComponentModel.BackgroundWorker bgw_Chg_Num;
        private System.Windows.Forms.CheckBox cb_CHG_NUM_From_Excel;
        private System.Windows.Forms.GroupBox gb_Selective;
        private System.Windows.Forms.ComboBox cb_ExcelSheetName;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btn_Chg_NumVsChg_Num;
        private System.ComponentModel.BackgroundWorker bgw_Chg_numVsChg_Num;
    }
}