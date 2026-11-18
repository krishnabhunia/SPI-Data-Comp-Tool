namespace SPI_Data_Comp_Tool
{
    partial class Compare_Multiples_List_Tables_Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Compare_Multiples_List_Tables_Forms));
            this.components = new System.ComponentModel.Container();
            this.gb_DBConnection = new System.Windows.Forms.GroupBox();
            this.btn_TestDBConnection = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cb_IncludeDiff_Columns = new System.Windows.Forms.CheckBox();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.l_lb_Output_Set = new System.Windows.Forms.LinkLabel();
            this.l_lbl_outputFolder = new System.Windows.Forms.LinkLabel();
            this.cb_IncludeRule = new System.Windows.Forms.CheckBox();
            this.btn_RuleForm = new System.Windows.Forms.Button();
            this.btn_Compare = new System.Windows.Forms.Button();
            this.btn_ListTableExcel = new System.Windows.Forms.Button();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.gb_Logs = new System.Windows.Forms.GroupBox();
            this.btn_ClearLogs = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.gb_Excel = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.llb_Excel_TRG = new System.Windows.Forms.LinkLabel();
            this.llb_Excel_SRC = new System.Windows.Forms.LinkLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.bgw_List = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_DB = new System.Windows.Forms.TabPage();
            this.tabPage_Excel = new System.Windows.Forms.TabPage();
            this.cb_PK_As_ReferenceKey = new System.Windows.Forms.CheckBox();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Excel = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Compare = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Output = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Log = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage_DB.SuspendLayout();
            this.gb_DBConnection.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.tabPage_Excel.SuspendLayout();
            this.gb_Excel.SuspendLayout();
            this.tlp_Excel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlp_Compare.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tlp_Output.SuspendLayout();
            this.gb_Logs.SuspendLayout();
            this.tlp_Log.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 360F));
            this.tlp_Root.RowCount = 2;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.tabControl1, 0, 0);
            this.tlp_Root.Controls.Add(this.groupBox1, 1, 0);
            this.tlp_Root.Controls.Add(this.gb_Logs, 0, 1);
            this.tlp_Root.SetColumnSpan(this.gb_Logs, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Controls.Add(this.tabPage_DB);
            this.tabControl1.Controls.Add(this.tabPage_Excel);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.tabControl1.Size = new System.Drawing.Size(800, 440);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage_DB
            // 
            this.tabPage_DB.Name = "tabPage_DB";
            this.tabPage_DB.UseVisualStyleBackColor = true;
            this.tabPage_DB.Text = "Databases";
            this.tabPage_DB.Controls.Add(this.gb_DBConnection);
            this.tabPage_DB.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tabPage_DB.TabIndex = 2;
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
            this.gb_DBConnection.TabIndex = 3;
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
            this.tlp_Connection.TabIndex = 4;
            // 
            // connection
            // 
            this.connection.Name = "connection";
            this.connection.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.connection.AutoSize = true;
            this.connection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.connection.TabIndex = 5;
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
            this.btn_TestDBConnection.TabIndex = 6;
            // 
            // tabPage_Excel
            // 
            this.tabPage_Excel.Name = "tabPage_Excel";
            this.tabPage_Excel.UseVisualStyleBackColor = true;
            this.tabPage_Excel.Click += new System.EventHandler(this.tabPage_Excel_Click);
            this.tabPage_Excel.Text = "Excel folders";
            this.tabPage_Excel.Controls.Add(this.gb_Excel);
            this.tabPage_Excel.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tabPage_Excel.TabIndex = 7;
            // 
            // gb_Excel
            // 
            this.gb_Excel.Name = "gb_Excel";
            this.gb_Excel.TabStop = false;
            this.gb_Excel.Text = "Excel folders";
            this.gb_Excel.Controls.Add(this.tlp_Excel);
            this.gb_Excel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Excel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.gb_Excel.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Excel.TabIndex = 8;
            // 
            // tlp_Excel
            // 
            this.tlp_Excel.Name = "tlp_Excel";
            this.tlp_Excel.ColumnCount = 1;
            this.tlp_Excel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Excel.RowCount = 4;
            this.tlp_Excel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Excel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Excel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Excel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Excel.Controls.Add(this.label1, 0, 0);
            this.tlp_Excel.Controls.Add(this.llb_Excel_SRC, 0, 1);
            this.tlp_Excel.Controls.Add(this.label10, 0, 2);
            this.tlp_Excel.Controls.Add(this.llb_Excel_TRG, 0, 3);
            this.tlp_Excel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Excel.AutoSize = false;
            this.tlp_Excel.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Name = "label1";
            this.label1.Text = "Source folder";
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label1.AutoSize = true;
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label1.TabIndex = 10;
            // 
            // llb_Excel_SRC
            // 
            this.llb_Excel_SRC.Name = "llb_Excel_SRC";
            this.llb_Excel_SRC.TabStop = true;
            this.llb_Excel_SRC.Text = "No Source Folder Selected";
            this.llb_Excel_SRC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Excel_SRC_LinkClicked);
            this.llb_Excel_SRC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.llb_Excel_SRC.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.llb_Excel_SRC.TabIndex = 11;
            // 
            // label10
            // 
            this.label10.Name = "label10";
            this.label10.Text = "Target folder";
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label10.AutoSize = true;
            this.label10.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label10.TabIndex = 12;
            // 
            // llb_Excel_TRG
            // 
            this.llb_Excel_TRG.Name = "llb_Excel_TRG";
            this.llb_Excel_TRG.TabStop = true;
            this.llb_Excel_TRG.Text = "No Target Folder Selected";
            this.llb_Excel_TRG.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Excel_TRG_LinkClicked);
            this.llb_Excel_TRG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.llb_Excel_TRG.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.llb_Excel_TRG.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compare";
            this.groupBox1.Controls.Add(this.tlp_Compare);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox1.TabIndex = 14;
            // 
            // tlp_Compare
            // 
            this.tlp_Compare.Name = "tlp_Compare";
            this.tlp_Compare.ColumnCount = 2;
            this.tlp_Compare.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Compare.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowCount = 8;
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.Controls.Add(this.groupBox4, 0, 0);
            this.tlp_Compare.SetColumnSpan(this.groupBox4, 2);
            this.tlp_Compare.Controls.Add(this.cb_PK_As_ReferenceKey, 0, 1);
            this.tlp_Compare.SetColumnSpan(this.cb_PK_As_ReferenceKey, 2);
            this.tlp_Compare.Controls.Add(this.cb_IncludeDiff_Columns, 0, 2);
            this.tlp_Compare.SetColumnSpan(this.cb_IncludeDiff_Columns, 2);
            this.tlp_Compare.Controls.Add(this.cb_IgnoreCase, 0, 3);
            this.tlp_Compare.SetColumnSpan(this.cb_IgnoreCase, 2);
            this.tlp_Compare.Controls.Add(this.cb_IncludeRule, 0, 4);
            this.tlp_Compare.Controls.Add(this.btn_RuleForm, 1, 4);
            this.tlp_Compare.Controls.Add(this.btn_ListTableExcel, 0, 5);
            this.tlp_Compare.SetColumnSpan(this.btn_ListTableExcel, 2);
            this.tlp_Compare.Controls.Add(this.btn_Compare, 0, 6);
            this.tlp_Compare.SetColumnSpan(this.btn_Compare, 2);
            this.tlp_Compare.Controls.Add(this.btnCancel, 0, 7);
            this.tlp_Compare.SetColumnSpan(this.btnCancel, 2);
            this.tlp_Compare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Compare.AutoSize = false;
            this.tlp_Compare.TabIndex = 15;
            // 
            // groupBox4
            // 
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output folder";
            this.groupBox4.Controls.Add(this.tlp_Output);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.groupBox4.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox4.TabIndex = 16;
            // 
            // tlp_Output
            // 
            this.tlp_Output.Name = "tlp_Output";
            this.tlp_Output.ColumnCount = 1;
            this.tlp_Output.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Output.RowCount = 2;
            this.tlp_Output.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Output.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Output.Controls.Add(this.l_lb_Output_Set, 0, 0);
            this.tlp_Output.Controls.Add(this.l_lbl_outputFolder, 0, 1);
            this.tlp_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Output.AutoSize = false;
            this.tlp_Output.TabIndex = 17;
            // 
            // l_lb_Output_Set
            // 
            this.l_lb_Output_Set.Name = "l_lb_Output_Set";
            this.l_lb_Output_Set.TabStop = true;
            this.l_lb_Output_Set.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l_lb_Output_Set_LinkClicked);
            this.l_lb_Output_Set.Text = "Choose output folder…";
            this.l_lb_Output_Set.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.l_lb_Output_Set.AutoSize = true;
            this.l_lb_Output_Set.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.l_lb_Output_Set.TabIndex = 18;
            // 
            // l_lbl_outputFolder
            // 
            this.l_lbl_outputFolder.Name = "l_lbl_outputFolder";
            this.l_lbl_outputFolder.TabStop = true;
            this.l_lbl_outputFolder.Text = "No Folder Selected";
            this.l_lbl_outputFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l_lbl_outputFolder_LinkClicked);
            this.l_lbl_outputFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l_lbl_outputFolder.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.l_lbl_outputFolder.TabIndex = 19;
            // 
            // cb_PK_As_ReferenceKey
            // 
            this.cb_PK_As_ReferenceKey.Checked = true;
            this.cb_PK_As_ReferenceKey.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_PK_As_ReferenceKey.Name = "cb_PK_As_ReferenceKey";
            this.cb_PK_As_ReferenceKey.UseVisualStyleBackColor = true;
            this.cb_PK_As_ReferenceKey.Text = "Reference key is the primary key";
            this.cb_PK_As_ReferenceKey.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_PK_As_ReferenceKey.AutoSize = true;
            this.cb_PK_As_ReferenceKey.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_PK_As_ReferenceKey.TabIndex = 20;
            // 
            // cb_IncludeDiff_Columns
            // 
            this.cb_IncludeDiff_Columns.Checked = true;
            this.cb_IncludeDiff_Columns.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_IncludeDiff_Columns.Name = "cb_IncludeDiff_Columns";
            this.cb_IncludeDiff_Columns.UseVisualStyleBackColor = true;
            this.cb_IncludeDiff_Columns.Text = "Only columns that differ";
            this.cb_IncludeDiff_Columns.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IncludeDiff_Columns.AutoSize = true;
            this.cb_IncludeDiff_Columns.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IncludeDiff_Columns.TabIndex = 21;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            this.cb_IgnoreCase.Text = "Ignore case";
            this.cb_IgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IgnoreCase.TabIndex = 22;
            // 
            // cb_IncludeRule
            // 
            this.cb_IncludeRule.Name = "cb_IncludeRule";
            this.cb_IncludeRule.UseVisualStyleBackColor = true;
            this.cb_IncludeRule.Text = "Include rules";
            this.cb_IncludeRule.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IncludeRule.AutoSize = true;
            this.cb_IncludeRule.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IncludeRule.TabIndex = 23;
            // 
            // btn_RuleForm
            // 
            this.btn_RuleForm.Name = "btn_RuleForm";
            this.btn_RuleForm.UseVisualStyleBackColor = true;
            this.btn_RuleForm.Click += new System.EventHandler(this.btn_RuleForm_Click);
            this.btn_RuleForm.Text = "Open rules…";
            this.btn_RuleForm.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btn_RuleForm.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btn_RuleForm.Size = new System.Drawing.Size(110, 28);
            this.btn_RuleForm.TabIndex = 24;
            // 
            // btn_ListTableExcel
            // 
            this.btn_ListTableExcel.Name = "btn_ListTableExcel";
            this.btn_ListTableExcel.UseVisualStyleBackColor = true;
            this.btn_ListTableExcel.Click += new System.EventHandler(this.btn_ListTableExcel_Click);
            this.btn_ListTableExcel.Text = "Choose list-table Excel…";
            this.btn_ListTableExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ListTableExcel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.btn_ListTableExcel.Size = new System.Drawing.Size(160, 32);
            this.btn_ListTableExcel.TabIndex = 25;
            // 
            // btn_Compare
            // 
            this.btn_Compare.Name = "btn_Compare";
            this.btn_Compare.UseVisualStyleBackColor = true;
            this.btn_Compare.Click += new System.EventHandler(this.btn_Compare_Click);
            this.btn_Compare.Text = "Compare and generate Excel";
            this.btn_Compare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Compare.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.btn_Compare.Size = new System.Drawing.Size(160, 36);
            this.btn_Compare.TabIndex = 26;
            // 
            // btnCancel
            // 
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 27;
            // 
            // gb_Logs
            // 
            this.gb_Logs.Name = "gb_Logs";
            this.gb_Logs.TabStop = false;
            this.gb_Logs.Text = "Log";
            this.gb_Logs.Controls.Add(this.tlp_Log);
            this.gb_Logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Logs.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.gb_Logs.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Logs.TabIndex = 28;
            // 
            // tlp_Log
            // 
            this.tlp_Log.Name = "tlp_Log";
            this.tlp_Log.ColumnCount = 2;
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowCount = 3;
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.Controls.Add(this.progressBar1, 0, 0);
            this.tlp_Log.Controls.Add(this.btn_ClearLogs, 1, 0);
            this.tlp_Log.Controls.Add(this.lbl_Progress, 0, 1);
            this.tlp_Log.SetColumnSpan(this.lbl_Progress, 2);
            this.tlp_Log.Controls.Add(this.rtb_Log, 0, 2);
            this.tlp_Log.SetColumnSpan(this.rtb_Log, 2);
            this.tlp_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Log.AutoSize = false;
            this.tlp_Log.TabIndex = 29;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 30;
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.Color.White;
            this.btn_ClearLogs.Name = "btn_ClearLogs";
            this.btn_ClearLogs.UseVisualStyleBackColor = false;
            this.btn_ClearLogs.Click += new System.EventHandler(this.btn_ClearLogs_Click);
            this.btn_ClearLogs.Text = "Clear log";
            this.btn_ClearLogs.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_ClearLogs.Size = new System.Drawing.Size(110, 30);
            this.btn_ClearLogs.TabIndex = 31;
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Text = "Progress";
            this.lbl_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.lbl_Progress.TabIndex = 32;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.rtb_Log.TabIndex = 33;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select Folder To save Output Files";
            this.folderBrowserDialog1.SelectedPath = "D:\\Krishna\\Output";
            // 
            // bgw_List
            // 
            this.bgw_List.WorkerReportsProgress = true;
            this.bgw_List.WorkerSupportsCancellation = true;
            this.bgw_List.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_List_DoWork);
            this.bgw_List.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_List_ProgressChanged);
            this.bgw_List.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_List_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Compare_Multiples_List_Tables_Forms
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Compare_Multiples_List_Tables_Forms";
            this.Text = "Compare list tables";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 880);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 760);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabControl1.PerformLayout();
            this.tabPage_DB.ResumeLayout(false);
            this.tabPage_DB.PerformLayout();
            this.gb_DBConnection.ResumeLayout(false);
            this.gb_DBConnection.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.tabPage_Excel.ResumeLayout(false);
            this.tabPage_Excel.PerformLayout();
            this.gb_Excel.ResumeLayout(false);
            this.gb_Excel.PerformLayout();
            this.tlp_Excel.ResumeLayout(false);
            this.tlp_Excel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlp_Compare.ResumeLayout(false);
            this.tlp_Compare.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tlp_Output.ResumeLayout(false);
            this.tlp_Output.PerformLayout();
            this.gb_Logs.ResumeLayout(false);
            this.gb_Logs.PerformLayout();
            this.tlp_Log.ResumeLayout(false);
            this.tlp_Log.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Excel;
        private System.Windows.Forms.TableLayoutPanel tlp_Compare;
        private System.Windows.Forms.TableLayoutPanel tlp_Output;
        private System.Windows.Forms.TableLayoutPanel tlp_Log;

        private System.Windows.Forms.GroupBox gb_DBConnection;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_ListTableExcel;
        private System.Windows.Forms.Label lbl_Progress;
        private System.Windows.Forms.GroupBox gb_Logs;
        private System.Windows.Forms.Button btn_Compare;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox gb_Excel;
        private System.Windows.Forms.LinkLabel llb_Excel_TRG;
        private System.Windows.Forms.LinkLabel llb_Excel_SRC;
        private System.Windows.Forms.Button btn_ClearLogs;
        private System.Windows.Forms.Button btn_TestDBConnection;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.LinkLabel l_lbl_outputFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.LinkLabel l_lb_Output_Set;
        private System.Windows.Forms.CheckBox cb_IncludeDiff_Columns;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
        private System.Windows.Forms.CheckBox cb_IncludeRule;
        private System.Windows.Forms.Button btn_RuleForm;
        private System.ComponentModel.BackgroundWorker bgw_List;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_DB;
        private System.Windows.Forms.TabPage tabPage_Excel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cb_PK_As_ReferenceKey;
    }
}