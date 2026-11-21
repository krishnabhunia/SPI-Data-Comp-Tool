namespace SPI_Data_Comp_Tool
{
    partial class SPIDeltaChangeTrackerOptimised
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SPIDeltaChangeTrackerOptimised));
            this.components = new System.ComponentModel.Container();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.cb_FullName = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cb_DateCol = new System.Windows.Forms.ComboBox();
            this.dateTimePicker_Date = new System.Windows.Forms.DateTimePicker();
            this.btn_Load_SRC_Table = new System.Windows.Forms.Button();
            this.cb_SRC_TableName = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cb_TRG_TableName = new System.Windows.Forms.ComboBox();
            this.btn_Load_TRG_Table = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btn_ListTableExcel = new System.Windows.Forms.Button();
            this.lblDebugMsg = new System.Windows.Forms.Label();
            this.llbl_Folder_path = new System.Windows.Forms.LinkLabel();
            this.cb_IncludeRule = new System.Windows.Forms.CheckBox();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.lbl_PercentageShow = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblTimer = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_RuleForm = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btnGenerateOutput = new System.Windows.Forms.Button();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.gb_Debug = new System.Windows.Forms.GroupBox();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Top = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Tables = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_SourceTable = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_TargetTable = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_DateFilter = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Run = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Batch = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Log = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Progress = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.tlp_Top.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.tlp_Tables.SuspendLayout();
            this.tlp_SourceTable.SuspendLayout();
            this.tlp_TargetTable.SuspendLayout();
            this.tlp_DateFilter.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tlp_Run.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tlp_Batch.SuspendLayout();
            this.gb_Debug.SuspendLayout();
            this.tlp_Log.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.tlp_Progress.SuspendLayout();
            this.SuspendLayout();
            //
            // tlp_Root
            //
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 1;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowCount = 3;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.Controls.Add(this.tlp_Top, 0, 0);
            this.tlp_Root.Controls.Add(this.gb_Debug, 0, 1);
            this.tlp_Root.Controls.Add(this.groupBox11, 0, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            //
            // tlp_Top
            //
            this.tlp_Top.Name = "tlp_Top";
            this.tlp_Top.ColumnCount = 2;
            this.tlp_Top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tlp_Top.RowCount = 1;
            this.tlp_Top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Top.Controls.Add(this.groupBox14, 0, 0);
            this.tlp_Top.Controls.Add(this.groupBox3, 1, 0);
            this.tlp_Top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Top.AutoSize = true;
            this.tlp_Top.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Top.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_Top.TabIndex = 1;
            //
            // groupBox14
            //
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Connections and tables";
            this.groupBox14.Controls.Add(this.tlp_Connection);
            this.groupBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox14.AutoSize = true;
            this.groupBox14.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox14.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.groupBox14.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox14.TabIndex = 2;
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
            this.tlp_Connection.Controls.Add(this.tlp_Tables, 0, 1);
            this.tlp_Connection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Connection.AutoSize = true;
            this.tlp_Connection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Connection.TabIndex = 3;
            //
            // connection
            //
            this.connection.Name = "connection";
            this.connection.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.connection.AutoSize = true;
            this.connection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.connection.TabIndex = 4;
            //
            // tlp_Tables
            //
            this.tlp_Tables.Name = "tlp_Tables";
            this.tlp_Tables.ColumnCount = 3;
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Tables.RowCount = 3;
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.Controls.Add(this.tlp_SourceTable, 0, 0);
            this.tlp_Tables.Controls.Add(this.tlp_TargetTable, 2, 0);
            this.tlp_Tables.Controls.Add(this.tlp_DateFilter, 0, 1);
            this.tlp_Tables.SetColumnSpan(this.tlp_DateFilter, 3);
            this.tlp_Tables.Controls.Add(this.cb_FullName, 0, 2);
            this.tlp_Tables.SetColumnSpan(this.cb_FullName, 3);
            this.tlp_Tables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Tables.AutoSize = true;
            this.tlp_Tables.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Tables.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.tlp_Tables.TabIndex = 5;
            //
            // tlp_SourceTable
            //
            this.tlp_SourceTable.Name = "tlp_SourceTable";
            this.tlp_SourceTable.ColumnCount = 3;
            this.tlp_SourceTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SourceTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SourceTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SourceTable.RowCount = 1;
            this.tlp_SourceTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SourceTable.Controls.Add(this.label16, 0, 0);
            this.tlp_SourceTable.Controls.Add(this.cb_SRC_TableName, 1, 0);
            this.tlp_SourceTable.Controls.Add(this.btn_Load_SRC_Table, 2, 0);
            this.tlp_SourceTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_SourceTable.AutoSize = true;
            this.tlp_SourceTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_SourceTable.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_SourceTable.TabIndex = 6;
            //
            // label16
            //
            this.label16.Name = "label16";
            this.label16.Text = "Source table";
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label16.AutoSize = true;
            this.label16.Margin = new System.Windows.Forms.Padding(0, 3, 12, 3);
            this.label16.TabIndex = 7;
            //
            // cb_SRC_TableName
            //
            this.cb_SRC_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_SRC_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_SRC_TableName.DropDownWidth = 500;
            this.cb_SRC_TableName.FormattingEnabled = true;
            this.cb_SRC_TableName.Name = "cb_SRC_TableName";
            this.cb_SRC_TableName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.cb_SRC_TableName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_SRC_TableName.TabIndex = 8;
            //
            // btn_Load_SRC_Table
            //
            this.btn_Load_SRC_Table.Name = "btn_Load_SRC_Table";
            this.btn_Load_SRC_Table.UseVisualStyleBackColor = true;
            this.btn_Load_SRC_Table.Click += new System.EventHandler(this.btn_Load_SRC_Table_Click);
            this.btn_Load_SRC_Table.Text = "Load tables";
            this.btn_Load_SRC_Table.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btn_Load_SRC_Table.Size = new System.Drawing.Size(110, 30);
            this.btn_Load_SRC_Table.TabIndex = 9;
            //
            // tlp_TargetTable
            //
            this.tlp_TargetTable.Name = "tlp_TargetTable";
            this.tlp_TargetTable.ColumnCount = 3;
            this.tlp_TargetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TargetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TargetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TargetTable.RowCount = 1;
            this.tlp_TargetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TargetTable.Controls.Add(this.label17, 0, 0);
            this.tlp_TargetTable.Controls.Add(this.cb_TRG_TableName, 1, 0);
            this.tlp_TargetTable.Controls.Add(this.btn_Load_TRG_Table, 2, 0);
            this.tlp_TargetTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_TargetTable.AutoSize = true;
            this.tlp_TargetTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_TargetTable.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_TargetTable.TabIndex = 10;
            //
            // label17
            //
            this.label17.Name = "label17";
            this.label17.Text = "Target table";
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label17.AutoSize = true;
            this.label17.Margin = new System.Windows.Forms.Padding(0, 3, 12, 3);
            this.label17.TabIndex = 11;
            //
            // cb_TRG_TableName
            //
            this.cb_TRG_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_TRG_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_TRG_TableName.DropDownWidth = 500;
            this.cb_TRG_TableName.Name = "cb_TRG_TableName";
            this.cb_TRG_TableName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.cb_TRG_TableName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_TRG_TableName.TabIndex = 12;
            //
            // btn_Load_TRG_Table
            //
            this.btn_Load_TRG_Table.Name = "btn_Load_TRG_Table";
            this.btn_Load_TRG_Table.UseVisualStyleBackColor = true;
            this.btn_Load_TRG_Table.Click += new System.EventHandler(this.btn_Load_TRG_Table_Click);
            this.btn_Load_TRG_Table.Text = "Load tables";
            this.btn_Load_TRG_Table.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btn_Load_TRG_Table.Size = new System.Drawing.Size(110, 30);
            this.btn_Load_TRG_Table.TabIndex = 13;
            //
            // tlp_DateFilter
            //
            this.tlp_DateFilter.Name = "tlp_DateFilter";
            this.tlp_DateFilter.ColumnCount = 4;
            this.tlp_DateFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_DateFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_DateFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_DateFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_DateFilter.RowCount = 1;
            this.tlp_DateFilter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_DateFilter.Controls.Add(this.label12, 0, 0);
            this.tlp_DateFilter.Controls.Add(this.cb_DateCol, 1, 0);
            this.tlp_DateFilter.Controls.Add(this.label15, 2, 0);
            this.tlp_DateFilter.Controls.Add(this.dateTimePicker_Date, 3, 0);
            this.tlp_DateFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_DateFilter.AutoSize = true;
            this.tlp_DateFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_DateFilter.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.tlp_DateFilter.TabIndex = 14;
            //
            // label12
            //
            this.label12.Name = "label12";
            this.label12.Text = "Date column";
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label12.AutoSize = true;
            this.label12.Margin = new System.Windows.Forms.Padding(0, 3, 12, 3);
            this.label12.TabIndex = 15;
            //
            // cb_DateCol
            //
            this.cb_DateCol.Enabled = false;
            this.cb_DateCol.FormattingEnabled = true;
            this.cb_DateCol.Items.AddRange(new object[] {
            "CHG_DATE"});
            this.cb_DateCol.Name = "cb_DateCol";
            this.cb_DateCol.Text = "CHG_DATE";
            this.cb_DateCol.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.cb_DateCol.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_DateCol.TabIndex = 16;
            //
            // label15
            //
            this.label15.Name = "label15";
            this.label15.Text = "On or after";
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label15.AutoSize = true;
            this.label15.Margin = new System.Windows.Forms.Padding(16, 3, 8, 3);
            this.label15.TabIndex = 17;
            //
            // dateTimePicker_Date
            //
            this.dateTimePicker_Date.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker_Date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_Date.Name = "dateTimePicker_Date";
            this.dateTimePicker_Date.Value = new System.DateTime(2020, 11, 27, 0, 0, 0, 0);
            this.dateTimePicker_Date.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.dateTimePicker_Date.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.dateTimePicker_Date.Size = new System.Drawing.Size(180, 25);
            this.dateTimePicker_Date.TabIndex = 18;
            //
            // cb_FullName
            //
            this.cb_FullName.Name = "cb_FullName";
            this.cb_FullName.UseVisualStyleBackColor = true;
            this.cb_FullName.CheckedChanged += new System.EventHandler(this.cb_FullName_CheckedChanged);
            this.cb_FullName.Text = "Show full table names";
            this.cb_FullName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_FullName.AutoSize = true;
            this.cb_FullName.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.cb_FullName.TabIndex = 19;
            //
            // groupBox3
            //
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Compare and export";
            this.groupBox3.Controls.Add(this.tlp_Run);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox3.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox3.TabIndex = 20;
            //
            // tlp_Run
            //
            this.tlp_Run.Name = "tlp_Run";
            this.tlp_Run.ColumnCount = 2;
            this.tlp_Run.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Run.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Run.RowCount = 8;
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Run.Controls.Add(this.cb_IncludeRule, 0, 0);
            this.tlp_Run.Controls.Add(this.btn_RuleForm, 1, 0);
            this.tlp_Run.Controls.Add(this.cb_IgnoreCase, 0, 1);
            this.tlp_Run.SetColumnSpan(this.cb_IgnoreCase, 2);
            this.tlp_Run.Controls.Add(this.btnGenerateOutput, 0, 2);
            this.tlp_Run.SetColumnSpan(this.btnGenerateOutput, 2);
            this.tlp_Run.Controls.Add(this.btn_Cancel, 0, 3);
            this.tlp_Run.Controls.Add(this.btn_Clear_Reset, 1, 3);
            this.tlp_Run.Controls.Add(this.tabControl1, 0, 4);
            this.tlp_Run.SetColumnSpan(this.tabControl1, 2);
            this.tlp_Run.Controls.Add(this.label13, 0, 5);
            this.tlp_Run.SetColumnSpan(this.label13, 2);
            this.tlp_Run.Controls.Add(this.llbl_Folder_path, 0, 6);
            this.tlp_Run.SetColumnSpan(this.llbl_Folder_path, 2);
            this.tlp_Run.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Run.AutoSize = false;
            this.tlp_Run.TabIndex = 21;
            //
            // cb_IncludeRule
            //
            this.cb_IncludeRule.Name = "cb_IncludeRule";
            this.cb_IncludeRule.UseVisualStyleBackColor = true;
            this.cb_IncludeRule.Text = "Include rules";
            this.cb_IncludeRule.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IncludeRule.AutoSize = true;
            this.cb_IncludeRule.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IncludeRule.TabIndex = 22;
            //
            // btn_RuleForm
            //
            this.btn_RuleForm.Name = "btn_RuleForm";
            this.btn_RuleForm.UseVisualStyleBackColor = true;
            this.btn_RuleForm.Click += new System.EventHandler(this.btn_RuleForm_Click);
            this.btn_RuleForm.Text = "Open rules…";
            this.btn_RuleForm.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btn_RuleForm.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_RuleForm.Size = new System.Drawing.Size(110, 30);
            this.btn_RuleForm.TabIndex = 23;
            //
            // cb_IgnoreCase
            //
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            this.cb_IgnoreCase.Text = "Ignore case";
            this.cb_IgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Margin = new System.Windows.Forms.Padding(0, 6, 0, 8);
            this.cb_IgnoreCase.TabIndex = 24;
            //
            // btnGenerateOutput
            //
            this.btnGenerateOutput.Enabled = false;
            this.btnGenerateOutput.Name = "btnGenerateOutput";
            this.btnGenerateOutput.UseVisualStyleBackColor = true;
            this.btnGenerateOutput.Click += new System.EventHandler(this.btnGenerateOutput_Click);
            this.btnGenerateOutput.Text = "Compare and generate Excel";
            this.btnGenerateOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGenerateOutput.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.btnGenerateOutput.Size = new System.Drawing.Size(160, 36);
            this.btnGenerateOutput.TabIndex = 25;
            //
            // btn_Cancel
            //
            this.btn_Cancel.Enabled = false;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.btn_Cancel.Size = new System.Drawing.Size(100, 30);
            this.btn_Cancel.TabIndex = 26;
            //
            // btn_Clear_Reset
            //
            this.btn_Clear_Reset.Name = "btn_Clear_Reset";
            this.btn_Clear_Reset.UseVisualStyleBackColor = true;
            this.btn_Clear_Reset.Click += new System.EventHandler(this.btn_Clear_Reset_Click);
            this.btn_Clear_Reset.Text = "Clear / Reset";
            this.btn_Clear_Reset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Clear_Reset.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.btn_Clear_Reset.Size = new System.Drawing.Size(100, 30);
            this.btn_Clear_Reset.TabIndex = 27;
            //
            // tabControl1
            //
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0, 12, 0, 8);
            this.tabControl1.Size = new System.Drawing.Size(280, 84);
            this.tabControl1.TabIndex = 28;
            //
            // tabPage2
            //
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Text = "Batch table list";
            this.tabPage2.Controls.Add(this.tlp_Batch);
            this.tabPage2.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tabPage2.TabIndex = 29;
            //
            // tlp_Batch
            //
            this.tlp_Batch.Name = "tlp_Batch";
            this.tlp_Batch.ColumnCount = 1;
            this.tlp_Batch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Batch.RowCount = 1;
            this.tlp_Batch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Batch.Controls.Add(this.btn_ListTableExcel, 0, 0);
            this.tlp_Batch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Batch.AutoSize = true;
            this.tlp_Batch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Batch.TabIndex = 30;
            //
            // btn_ListTableExcel
            //
            this.btn_ListTableExcel.Name = "btn_ListTableExcel";
            this.btn_ListTableExcel.UseVisualStyleBackColor = true;
            this.btn_ListTableExcel.Click += new System.EventHandler(this.btn_ListTableExcel_Click);
            this.btn_ListTableExcel.Text = "Choose table-list workbook…";
            this.btn_ListTableExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ListTableExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_ListTableExcel.Size = new System.Drawing.Size(150, 34);
            this.btn_ListTableExcel.TabIndex = 31;
            //
            // label13
            //
            this.label13.Name = "label13";
            this.label13.Text = "Output folder";
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label13.AutoSize = true;
            this.label13.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.label13.TabIndex = 32;
            //
            // llbl_Folder_path
            //
            this.llbl_Folder_path.AutoSize = true;
            this.llbl_Folder_path.Name = "llbl_Folder_path";
            this.llbl_Folder_path.TabStop = true;
            this.llbl_Folder_path.Text = "linkLabel";
            this.llbl_Folder_path.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_Folder_path_LinkClicked);
            this.llbl_Folder_path.Click += new System.EventHandler(this.llbl_Folder_path_Click);
            this.llbl_Folder_path.MouseHover += new System.EventHandler(this.llbl_Folder_path_MouseHover);
            this.llbl_Folder_path.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.llbl_Folder_path.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.llbl_Folder_path.Size = new System.Drawing.Size(220, 38);
            this.llbl_Folder_path.TabIndex = 33;
            //
            // gb_Debug
            //
            this.gb_Debug.Name = "gb_Debug";
            this.gb_Debug.TabStop = false;
            this.gb_Debug.Text = "Operation log";
            this.gb_Debug.Controls.Add(this.tlp_Log);
            this.gb_Debug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Debug.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.gb_Debug.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Debug.TabIndex = 34;
            //
            // tlp_Log
            //
            this.tlp_Log.Name = "tlp_Log";
            this.tlp_Log.ColumnCount = 2;
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowCount = 1;
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.Controls.Add(this.rtb_Log, 0, 0);
            this.tlp_Log.Controls.Add(this.btn_Clear, 1, 0);
            this.tlp_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Log.AutoSize = false;
            this.tlp_Log.TabIndex = 35;
            //
            // rtb_Log
            //
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.rtb_Log.MinimumSize = new System.Drawing.Size(0, 130);
            this.rtb_Log.TabIndex = 36;
            //
            // btn_Clear
            //
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            this.btn_Clear.Text = "Clear log";
            this.btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            this.btn_Clear.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Clear.Size = new System.Drawing.Size(95, 30);
            this.btn_Clear.TabIndex = 37;
            //
            // groupBox11
            //
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Progress";
            this.groupBox11.Controls.Add(this.tlp_Progress);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.AutoSize = true;
            this.groupBox11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox11.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBox11.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox11.TabIndex = 38;
            //
            // tlp_Progress
            //
            this.tlp_Progress.Name = "tlp_Progress";
            this.tlp_Progress.ColumnCount = 4;
            this.tlp_Progress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Progress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Progress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Progress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Progress.RowCount = 1;
            this.tlp_Progress.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Progress.Controls.Add(this.progressBar1, 0, 0);
            this.tlp_Progress.Controls.Add(this.lbl_PercentageShow, 1, 0);
            this.tlp_Progress.Controls.Add(this.label1, 2, 0);
            this.tlp_Progress.Controls.Add(this.lblTimer, 3, 0);
            this.tlp_Progress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Progress.AutoSize = true;
            this.tlp_Progress.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Progress.TabIndex = 39;
            //
            // progressBar1
            //
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 16, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 40;
            //
            // lbl_PercentageShow
            //
            this.lbl_PercentageShow.Name = "lbl_PercentageShow";
            this.lbl_PercentageShow.Text = "0 %";
            this.lbl_PercentageShow.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_PercentageShow.AutoSize = true;
            this.lbl_PercentageShow.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.lbl_PercentageShow.TabIndex = 41;
            //
            // label1
            //
            this.label1.Name = "label1";
            this.label1.Text = "Elapsed";
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label1.AutoSize = true;
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.label1.TabIndex = 42;
            //
            // lblTimer
            //
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Text = "";
            this.lblTimer.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lblTimer.AutoSize = true;
            this.lblTimer.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblTimer.MinimumSize = new System.Drawing.Size(80, 0);
            this.lblTimer.TabIndex = 43;
            //
            // comboBox1
            //
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.ItemHeight = 13;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.comboBox1.Location = new System.Drawing.Point(277, 15);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(67, 21);
            this.comboBox1.TabIndex = 20;
            this.comboBox1.Visible = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            //
            // backgroundWorker1
            //
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            //
            // backgroundWorker2
            //
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker2_ProgressChanged);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            //
            // lblDebugMsg
            //
            this.lblDebugMsg.AutoSize = true;
            this.lblDebugMsg.Location = new System.Drawing.Point(9, 461);
            this.lblDebugMsg.Name = "lblDebugMsg";
            this.lblDebugMsg.Size = new System.Drawing.Size(63, 13);
            this.lblDebugMsg.TabIndex = 36;
            this.lblDebugMsg.Text = "lblDebuMsg";
            this.lblDebugMsg.Visible = false;
            //
            // timer1
            //
            this.timer1.Interval = 499;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // SPIDeltaChangeTrackerOptimised
            //
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SPIDeltaChangeTrackerOptimised";
            this.Text = "Delta change tracker";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 820);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1080, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.lblDebugMsg);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.tlp_Top.ResumeLayout(false);
            this.tlp_Top.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.tlp_Tables.ResumeLayout(false);
            this.tlp_Tables.PerformLayout();
            this.tlp_SourceTable.ResumeLayout(false);
            this.tlp_SourceTable.PerformLayout();
            this.tlp_TargetTable.ResumeLayout(false);
            this.tlp_TargetTable.PerformLayout();
            this.tlp_DateFilter.ResumeLayout(false);
            this.tlp_DateFilter.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tlp_Run.ResumeLayout(false);
            this.tlp_Run.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabControl1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tlp_Batch.ResumeLayout(false);
            this.tlp_Batch.PerformLayout();
            this.gb_Debug.ResumeLayout(false);
            this.gb_Debug.PerformLayout();
            this.tlp_Log.ResumeLayout(false);
            this.tlp_Log.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.tlp_Progress.ResumeLayout(false);
            this.tlp_Progress.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Top;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Tables;
        private System.Windows.Forms.TableLayoutPanel tlp_SourceTable;
        private System.Windows.Forms.TableLayoutPanel tlp_TargetTable;
        private System.Windows.Forms.TableLayoutPanel tlp_DateFilter;
        private System.Windows.Forms.TableLayoutPanel tlp_Run;
        private System.Windows.Forms.TableLayoutPanel tlp_Batch;
        private System.Windows.Forms.TableLayoutPanel tlp_Log;
        private System.Windows.Forms.TableLayoutPanel tlp_Progress;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.Button btn_Load_SRC_Table;
        private System.Windows.Forms.ComboBox cb_SRC_TableName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox cb_TRG_TableName;
        private System.Windows.Forms.Button btn_Load_TRG_Table;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cb_IncludeRule;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Label lbl_PercentageShow;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_RuleForm;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btnGenerateOutput;
        private System.Windows.Forms.Button btn_Clear_Reset;
        private System.Windows.Forms.LinkLabel llbl_Folder_path;
        private System.Windows.Forms.Label label13;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker_Date;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cb_DateCol;
        private System.Windows.Forms.CheckBox cb_FullName;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.GroupBox gb_Debug;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Button btn_ListTableExcel;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.Label lblDebugMsg;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Timer timer1;
    }
}