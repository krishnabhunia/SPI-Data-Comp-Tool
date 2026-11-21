namespace SPI_Data_Comp_Tool
{
    partial class DB_Comparison_Form
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DB_Comparison_Form));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.components = new System.ComponentModel.Container();
            this.btnGenerateOutput = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tb_Source_tableName_1 = new System.Windows.Forms.TextBox();
            this.tb_destination_tableName = new System.Windows.Forms.TextBox();
            this.btn_Connect_Reference_ID = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.lbl_PercentageShow = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblTimer = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_ColumnMapping = new System.Windows.Forms.CheckBox();
            this.cb_IncludeRule = new System.Windows.Forms.CheckBox();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.btn_RuleForm = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.btn_Set = new System.Windows.Forms.Button();
            this.llbl_File_path = new System.Windows.Forms.LinkLabel();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.lblCal = new System.Windows.Forms.Label();
            this.lblExpressionStatus = new System.Windows.Forms.Label();
            this.cbL_SRC_Cols = new System.Windows.Forms.CheckedListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cb_selectAll_toggle = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lb_TRG_Cols = new System.Windows.Forms.ListBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.cb_PrimaryKey = new System.Windows.Forms.CheckBox();
            this.clb_ReferenceID = new System.Windows.Forms.CheckedListBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lb_IgnoredCol_TRG = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lb_IgnoredCol_SRC = new System.Windows.Forms.ListBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.cb_TRG_TableName = new System.Windows.Forms.ComboBox();
            this.btn_Load_TRG_Table = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.btn_Load_SRC_Table = new System.Windows.Forms.Button();
            this.cb_SRC_TableName = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cb_FullName = new System.Windows.Forms.CheckBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Top = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Tables = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_SRC_Table = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_TRG_Table = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Compare = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_View = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_RefKey = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_SrcCols = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_TrgCols = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_IgnSrc = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_IgnTrg = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Progress = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tlp_Root.SuspendLayout();
            this.tlp_Top.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.tlp_Tables.SuspendLayout();
            this.tlp_SRC_Table.SuspendLayout();
            this.tlp_TRG_Table.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tlp_Compare.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tlp_View.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tlp_RefKey.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tlp_SrcCols.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tlp_TrgCols.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlp_IgnSrc.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tlp_IgnTrg.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tlp_Status.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.tlp_Progress.SuspendLayout();
            this.groupBox15.SuspendLayout();
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
            this.tlp_Root.Controls.Add(this.groupBox9, 0, 1);
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
            this.groupBox14.Text = "Connection";
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
            this.tlp_Tables.RowCount = 2;
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.Controls.Add(this.tlp_SRC_Table, 0, 0);
            this.tlp_Tables.Controls.Add(this.tlp_TRG_Table, 2, 0);
            this.tlp_Tables.Controls.Add(this.cb_FullName, 0, 1);
            this.tlp_Tables.SetColumnSpan(this.cb_FullName, 3);
            this.tlp_Tables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Tables.AutoSize = true;
            this.tlp_Tables.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Tables.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.tlp_Tables.TabIndex = 5;
            // 
            // tlp_SRC_Table
            // 
            this.tlp_SRC_Table.Name = "tlp_SRC_Table";
            this.tlp_SRC_Table.ColumnCount = 3;
            this.tlp_SRC_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SRC_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SRC_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SRC_Table.RowCount = 1;
            this.tlp_SRC_Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SRC_Table.Controls.Add(this.label16, 0, 0);
            this.tlp_SRC_Table.Controls.Add(this.cb_SRC_TableName, 1, 0);
            this.tlp_SRC_Table.Controls.Add(this.btn_Load_SRC_Table, 2, 0);
            this.tlp_SRC_Table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_SRC_Table.AutoSize = true;
            this.tlp_SRC_Table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_SRC_Table.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_SRC_Table.TabIndex = 6;
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
            // tlp_TRG_Table
            // 
            this.tlp_TRG_Table.Name = "tlp_TRG_Table";
            this.tlp_TRG_Table.ColumnCount = 3;
            this.tlp_TRG_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TRG_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TRG_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TRG_Table.RowCount = 1;
            this.tlp_TRG_Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TRG_Table.Controls.Add(this.label17, 0, 0);
            this.tlp_TRG_Table.Controls.Add(this.cb_TRG_TableName, 1, 0);
            this.tlp_TRG_Table.Controls.Add(this.btn_Load_TRG_Table, 2, 0);
            this.tlp_TRG_Table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_TRG_Table.AutoSize = true;
            this.tlp_TRG_Table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_TRG_Table.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_TRG_Table.TabIndex = 10;
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
            // cb_FullName
            // 
            this.cb_FullName.Checked = true;
            this.cb_FullName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_FullName.Name = "cb_FullName";
            this.cb_FullName.UseVisualStyleBackColor = true;
            this.cb_FullName.CheckedChanged += new System.EventHandler(this.cb_FullName_CheckedChanged);
            this.cb_FullName.Text = "Show full names in lists";
            this.cb_FullName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_FullName.AutoSize = true;
            this.cb_FullName.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.cb_FullName.TabIndex = 14;
            // 
            // groupBox3
            // 
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Compare";
            this.groupBox3.Controls.Add(this.tlp_Compare);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox3.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox3.TabIndex = 15;
            // 
            // tlp_Compare
            // 
            this.tlp_Compare.Name = "tlp_Compare";
            this.tlp_Compare.ColumnCount = 2;
            this.tlp_Compare.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Compare.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Compare.RowCount = 10;
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.Controls.Add(this.cb_ColumnMapping, 0, 0);
            this.tlp_Compare.SetColumnSpan(this.cb_ColumnMapping, 2);
            this.tlp_Compare.Controls.Add(this.cb_IncludeRule, 0, 1);
            this.tlp_Compare.Controls.Add(this.btn_RuleForm, 1, 1);
            this.tlp_Compare.Controls.Add(this.cb_IgnoreCase, 0, 2);
            this.tlp_Compare.SetColumnSpan(this.cb_IgnoreCase, 2);
            this.tlp_Compare.Controls.Add(this.btn_Connect_Reference_ID, 0, 3);
            this.tlp_Compare.SetColumnSpan(this.btn_Connect_Reference_ID, 2);
            this.tlp_Compare.Controls.Add(this.btn_Set, 0, 4);
            this.tlp_Compare.SetColumnSpan(this.btn_Set, 2);
            this.tlp_Compare.Controls.Add(this.btnGenerateOutput, 0, 5);
            this.tlp_Compare.SetColumnSpan(this.btnGenerateOutput, 2);
            this.tlp_Compare.Controls.Add(this.btn_Cancel, 0, 6);
            this.tlp_Compare.Controls.Add(this.btn_Clear_Reset, 1, 6);
            this.tlp_Compare.Controls.Add(this.label13, 0, 8);
            this.tlp_Compare.SetColumnSpan(this.label13, 2);
            this.tlp_Compare.Controls.Add(this.llbl_File_path, 0, 9);
            this.tlp_Compare.SetColumnSpan(this.llbl_File_path, 2);
            this.tlp_Compare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Compare.AutoSize = false;
            this.tlp_Compare.TabIndex = 16;
            // 
            // cb_ColumnMapping
            // 
            this.cb_ColumnMapping.Name = "cb_ColumnMapping";
            this.cb_ColumnMapping.UseVisualStyleBackColor = true;
            this.cb_ColumnMapping.Text = "Different column names / RefID comparison";
            this.cb_ColumnMapping.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_ColumnMapping.AutoSize = true;
            this.cb_ColumnMapping.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_ColumnMapping.TabIndex = 17;
            // 
            // cb_IncludeRule
            // 
            this.cb_IncludeRule.Name = "cb_IncludeRule";
            this.cb_IncludeRule.UseVisualStyleBackColor = true;
            this.cb_IncludeRule.CheckedChanged += new System.EventHandler(this.cb_IncludeRule_CheckedChanged);
            this.cb_IncludeRule.Text = "Include rules";
            this.cb_IncludeRule.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IncludeRule.AutoSize = true;
            this.cb_IncludeRule.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IncludeRule.TabIndex = 18;
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
            this.btn_RuleForm.TabIndex = 19;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            this.cb_IgnoreCase.CheckedChanged += new System.EventHandler(this.cb_IgnoreCase_CheckedChanged);
            this.cb_IgnoreCase.Text = "Ignore case";
            this.cb_IgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IgnoreCase.TabIndex = 20;
            // 
            // btn_Connect_Reference_ID
            // 
            this.btn_Connect_Reference_ID.Name = "btn_Connect_Reference_ID";
            this.btn_Connect_Reference_ID.UseVisualStyleBackColor = true;
            this.btn_Connect_Reference_ID.Click += new System.EventHandler(this.btn_Connect_Reference_ID_Click);
            this.btn_Connect_Reference_ID.Text = "1   Connect and load";
            this.btn_Connect_Reference_ID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Connect_Reference_ID.Margin = new System.Windows.Forms.Padding(0, 12, 0, 4);
            this.btn_Connect_Reference_ID.Size = new System.Drawing.Size(120, 34);
            this.btn_Connect_Reference_ID.TabIndex = 21;
            // 
            // btn_Set
            // 
            this.btn_Set.Enabled = false;
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            this.btn_Set.Text = "2   Set and validate";
            this.btn_Set.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Set.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.btn_Set.Size = new System.Drawing.Size(120, 34);
            this.btn_Set.TabIndex = 22;
            // 
            // btnGenerateOutput
            // 
            this.btnGenerateOutput.Enabled = false;
            this.btnGenerateOutput.Name = "btnGenerateOutput";
            this.btnGenerateOutput.UseVisualStyleBackColor = true;
            this.btnGenerateOutput.Click += new System.EventHandler(this.btnGenerateOutput_Click);
            this.btnGenerateOutput.Text = "3   Compare and generate Excel";
            this.btnGenerateOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGenerateOutput.Margin = new System.Windows.Forms.Padding(0, 4, 0, 8);
            this.btnGenerateOutput.Size = new System.Drawing.Size(120, 36);
            this.btnGenerateOutput.TabIndex = 23;
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
            this.btn_Cancel.TabIndex = 24;
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
            this.btn_Clear_Reset.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.Name = "label13";
            this.label13.Text = "Report";
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label13.AutoSize = true;
            this.label13.Margin = new System.Windows.Forms.Padding(0, 6, 0, 2);
            this.label13.TabIndex = 26;
            // 
            // llbl_File_path
            // 
            this.llbl_File_path.Name = "llbl_File_path";
            this.llbl_File_path.TabStop = true;
            this.llbl_File_path.Visible = false;
            this.llbl_File_path.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_File_path_LinkClicked);
            this.llbl_File_path.Click += new System.EventHandler(this.llbl_File_path_Click);
            this.llbl_File_path.MouseHover += new System.EventHandler(this.llbl_Folder_path_MouseHover);
            this.llbl_File_path.Text = "";
            this.llbl_File_path.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.llbl_File_path.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.llbl_File_path.Size = new System.Drawing.Size(100, 36);
            this.llbl_File_path.TabIndex = 27;
            // 
            // groupBox9
            // 
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "View data";
            this.groupBox9.Controls.Add(this.tlp_View);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBox9.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox9.TabIndex = 28;
            // 
            // tlp_View
            // 
            this.tlp_View.Name = "tlp_View";
            this.tlp_View.ColumnCount = 6;
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tlp_View.RowCount = 1;
            this.tlp_View.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_View.Controls.Add(this.groupBox7, 0, 0);
            this.tlp_View.Controls.Add(this.groupBox5, 1, 0);
            this.tlp_View.Controls.Add(this.groupBox6, 2, 0);
            this.tlp_View.Controls.Add(this.groupBox1, 3, 0);
            this.tlp_View.Controls.Add(this.groupBox2, 4, 0);
            this.tlp_View.Controls.Add(this.groupBox10, 5, 0);
            this.tlp_View.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_View.AutoSize = false;
            this.tlp_View.TabIndex = 29;
            // 
            // groupBox7
            // 
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Reference key";
            this.groupBox7.Controls.Add(this.tlp_RefKey);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.groupBox7.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox7.TabIndex = 30;
            // 
            // tlp_RefKey
            // 
            this.tlp_RefKey.Name = "tlp_RefKey";
            this.tlp_RefKey.ColumnCount = 1;
            this.tlp_RefKey.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_RefKey.RowCount = 2;
            this.tlp_RefKey.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlp_RefKey.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_RefKey.Controls.Add(this.cb_PrimaryKey, 0, 0);
            this.tlp_RefKey.Controls.Add(this.clb_ReferenceID, 0, 1);
            this.tlp_RefKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_RefKey.AutoSize = false;
            this.tlp_RefKey.TabIndex = 31;
            // 
            // cb_PrimaryKey
            // 
            this.cb_PrimaryKey.Checked = true;
            this.cb_PrimaryKey.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_PrimaryKey.Name = "cb_PrimaryKey";
            this.cb_PrimaryKey.UseVisualStyleBackColor = true;
            this.cb_PrimaryKey.Text = "Select primary key";
            this.cb_PrimaryKey.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_PrimaryKey.AutoSize = true;
            this.cb_PrimaryKey.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.cb_PrimaryKey.TabIndex = 32;
            // 
            // clb_ReferenceID
            // 
            this.clb_ReferenceID.CheckOnClick = true;
            this.clb_ReferenceID.FormattingEnabled = true;
            this.clb_ReferenceID.HorizontalScrollbar = true;
            this.clb_ReferenceID.Name = "clb_ReferenceID";
            this.clb_ReferenceID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clb_ReferenceID.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.clb_ReferenceID.TabIndex = 33;
            // 
            // groupBox5
            // 
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Source columns";
            this.groupBox5.Controls.Add(this.tlp_SrcCols);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.groupBox5.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox5.TabIndex = 34;
            // 
            // tlp_SrcCols
            // 
            this.tlp_SrcCols.Name = "tlp_SrcCols";
            this.tlp_SrcCols.ColumnCount = 1;
            this.tlp_SrcCols.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SrcCols.RowCount = 2;
            this.tlp_SrcCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlp_SrcCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SrcCols.Controls.Add(this.cb_selectAll_toggle, 0, 0);
            this.tlp_SrcCols.Controls.Add(this.cbL_SRC_Cols, 0, 1);
            this.tlp_SrcCols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_SrcCols.AutoSize = false;
            this.tlp_SrcCols.TabIndex = 35;
            // 
            // cb_selectAll_toggle
            // 
            this.cb_selectAll_toggle.Name = "cb_selectAll_toggle";
            this.cb_selectAll_toggle.UseVisualStyleBackColor = true;
            this.cb_selectAll_toggle.CheckedChanged += new System.EventHandler(this.cb_selectAll_toggle_CheckedChanged);
            this.cb_selectAll_toggle.Text = "Select all / none";
            this.cb_selectAll_toggle.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_selectAll_toggle.AutoSize = true;
            this.cb_selectAll_toggle.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.cb_selectAll_toggle.TabIndex = 36;
            // 
            // cbL_SRC_Cols
            // 
            this.cbL_SRC_Cols.CheckOnClick = true;
            this.cbL_SRC_Cols.FormattingEnabled = true;
            this.cbL_SRC_Cols.HorizontalScrollbar = true;
            this.cbL_SRC_Cols.Name = "cbL_SRC_Cols";
            this.cbL_SRC_Cols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbL_SRC_Cols.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.cbL_SRC_Cols.TabIndex = 37;
            // 
            // groupBox6
            // 
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Target columns";
            this.groupBox6.Controls.Add(this.tlp_TrgCols);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.groupBox6.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox6.TabIndex = 38;
            // 
            // tlp_TrgCols
            // 
            this.tlp_TrgCols.Name = "tlp_TrgCols";
            this.tlp_TrgCols.ColumnCount = 1;
            this.tlp_TrgCols.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TrgCols.RowCount = 2;
            this.tlp_TrgCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlp_TrgCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TrgCols.Controls.Add(this.lb_TRG_Cols, 0, 1);
            this.tlp_TrgCols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_TrgCols.AutoSize = false;
            this.tlp_TrgCols.TabIndex = 39;
            // 
            // lb_TRG_Cols
            // 
            this.lb_TRG_Cols.FormattingEnabled = true;
            this.lb_TRG_Cols.HorizontalScrollbar = true;
            this.lb_TRG_Cols.ItemHeight = 16;
            this.lb_TRG_Cols.Name = "lb_TRG_Cols";
            this.lb_TRG_Cols.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_TRG_Cols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_TRG_Cols.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lb_TRG_Cols.TabIndex = 40;
            // 
            // groupBox1
            // 
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ignored in Source";
            this.groupBox1.Controls.Add(this.tlp_IgnSrc);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox1.TabIndex = 41;
            // 
            // tlp_IgnSrc
            // 
            this.tlp_IgnSrc.Name = "tlp_IgnSrc";
            this.tlp_IgnSrc.ColumnCount = 1;
            this.tlp_IgnSrc.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_IgnSrc.RowCount = 2;
            this.tlp_IgnSrc.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlp_IgnSrc.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_IgnSrc.Controls.Add(this.lb_IgnoredCol_SRC, 0, 1);
            this.tlp_IgnSrc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_IgnSrc.AutoSize = false;
            this.tlp_IgnSrc.TabIndex = 42;
            // 
            // lb_IgnoredCol_SRC
            // 
            this.lb_IgnoredCol_SRC.FormattingEnabled = true;
            this.lb_IgnoredCol_SRC.HorizontalScrollbar = true;
            this.lb_IgnoredCol_SRC.ItemHeight = 16;
            this.lb_IgnoredCol_SRC.Name = "lb_IgnoredCol_SRC";
            this.lb_IgnoredCol_SRC.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoredCol_SRC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_IgnoredCol_SRC.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lb_IgnoredCol_SRC.TabIndex = 43;
            // 
            // groupBox2
            // 
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ignored in Target";
            this.groupBox2.Controls.Add(this.tlp_IgnTrg);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox2.TabIndex = 44;
            // 
            // tlp_IgnTrg
            // 
            this.tlp_IgnTrg.Name = "tlp_IgnTrg";
            this.tlp_IgnTrg.ColumnCount = 1;
            this.tlp_IgnTrg.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_IgnTrg.RowCount = 2;
            this.tlp_IgnTrg.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlp_IgnTrg.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_IgnTrg.Controls.Add(this.lb_IgnoredCol_TRG, 0, 1);
            this.tlp_IgnTrg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_IgnTrg.AutoSize = false;
            this.tlp_IgnTrg.TabIndex = 45;
            // 
            // lb_IgnoredCol_TRG
            // 
            this.lb_IgnoredCol_TRG.FormattingEnabled = true;
            this.lb_IgnoredCol_TRG.HorizontalScrollbar = true;
            this.lb_IgnoredCol_TRG.ItemHeight = 16;
            this.lb_IgnoredCol_TRG.Name = "lb_IgnoredCol_TRG";
            this.lb_IgnoredCol_TRG.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoredCol_TRG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_IgnoredCol_TRG.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lb_IgnoredCol_TRG.TabIndex = 46;
            // 
            // groupBox10
            // 
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Status";
            this.groupBox10.Controls.Add(this.tlp_Status);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.groupBox10.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox10.TabIndex = 47;
            // 
            // tlp_Status
            // 
            this.tlp_Status.Name = "tlp_Status";
            this.tlp_Status.ColumnCount = 1;
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Status.RowCount = 3;
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_Status.Controls.Add(this.lblStatus, 0, 0);
            this.tlp_Status.Controls.Add(this.lblExpressionStatus, 0, 1);
            this.tlp_Status.Controls.Add(this.lblCal, 0, 2);
            this.tlp_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Status.AutoSize = false;
            this.tlp_Status.TabIndex = 48;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Okay";
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblStatus.TabIndex = 49;
            // 
            // lblExpressionStatus
            // 
            this.lblExpressionStatus.Name = "lblExpressionStatus";
            this.lblExpressionStatus.Text = "";
            this.lblExpressionStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblExpressionStatus.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblExpressionStatus.TabIndex = 50;
            // 
            // lblCal
            // 
            this.lblCal.Name = "lblCal";
            this.lblCal.Text = "Total rows : columns";
            this.lblCal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCal.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblCal.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lblCal.TabIndex = 51;
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
            this.groupBox11.TabIndex = 52;
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
            this.tlp_Progress.TabIndex = 53;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 16, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 54;
            // 
            // lbl_PercentageShow
            // 
            this.lbl_PercentageShow.Name = "lbl_PercentageShow";
            this.lbl_PercentageShow.Text = "0 %";
            this.lbl_PercentageShow.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_PercentageShow.AutoSize = true;
            this.lbl_PercentageShow.Margin = new System.Windows.Forms.Padding(0, 0, 24, 0);
            this.lbl_PercentageShow.TabIndex = 55;
            // 
            // label1
            // 
            this.label1.Name = "label1";
            this.label1.Text = "Elapsed";
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label1.AutoSize = true;
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.label1.TabIndex = 56;
            // 
            // lblTimer
            // 
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Text = "";
            this.lblTimer.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lblTimer.AutoSize = true;
            this.lblTimer.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblTimer.MinimumSize = new System.Drawing.Size(80, 0);
            this.lblTimer.TabIndex = 57;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Location = new System.Drawing.Point(6, 22);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Size = new System.Drawing.Size(39, 29);
            this.dataGridView1.TabIndex = 15;
            this.dataGridView1.Visible = false;
            // 
            // tb_Source_tableName_1
            // 
            this.tb_Source_tableName_1.Location = new System.Drawing.Point(6, 89);
            this.tb_Source_tableName_1.Name = "tb_Source_tableName_1";
            this.tb_Source_tableName_1.Size = new System.Drawing.Size(29, 20);
            this.tb_Source_tableName_1.TabIndex = 10;
            // 
            // tb_destination_tableName
            // 
            this.tb_destination_tableName.Location = new System.Drawing.Point(6, 125);
            this.tb_destination_tableName.Name = "tb_destination_tableName";
            this.tb_destination_tableName.Size = new System.Drawing.Size(29, 20);
            this.tb_destination_tableName.TabIndex = 11;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(51, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Load Profile";
            this.label12.Visible = false;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(6, 185);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(14, 23);
            this.btn_Delete.TabIndex = 17;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Visible = false;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(6, 156);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(24, 23);
            this.btn_Save.TabIndex = 16;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Visible = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(54, 57);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(24, 21);
            this.comboBox2.TabIndex = 15;
            this.comboBox2.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 57);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(42, 23);
            this.button1.TabIndex = 26;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.button1);
            this.groupBox15.Controls.Add(this.dataGridView1);
            this.groupBox15.Controls.Add(this.tb_Source_tableName_1);
            this.groupBox15.Controls.Add(this.tb_destination_tableName);
            this.groupBox15.Controls.Add(this.label12);
            this.groupBox15.Controls.Add(this.comboBox2);
            this.groupBox15.Controls.Add(this.btn_Delete);
            this.groupBox15.Controls.Add(this.btn_Save);
            this.groupBox15.Location = new System.Drawing.Point(1706, 15);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(59, 58);
            this.groupBox15.TabIndex = 32;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Hidden Controls";
            this.groupBox15.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DB_Comparison_Form
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DB_Comparison_Form";
            this.Text = "Compare databases";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 700);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(980, 620);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.Controls.Add(this.groupBox15);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
            this.tlp_SRC_Table.ResumeLayout(false);
            this.tlp_SRC_Table.PerformLayout();
            this.tlp_TRG_Table.ResumeLayout(false);
            this.tlp_TRG_Table.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tlp_Compare.ResumeLayout(false);
            this.tlp_Compare.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.tlp_View.ResumeLayout(false);
            this.tlp_View.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tlp_RefKey.ResumeLayout(false);
            this.tlp_RefKey.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tlp_SrcCols.ResumeLayout(false);
            this.tlp_SrcCols.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tlp_TrgCols.ResumeLayout(false);
            this.tlp_TrgCols.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlp_IgnSrc.ResumeLayout(false);
            this.tlp_IgnSrc.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tlp_IgnTrg.ResumeLayout(false);
            this.tlp_IgnTrg.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.tlp_Progress.ResumeLayout(false);
            this.tlp_Progress.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Tables;
        private System.Windows.Forms.TableLayoutPanel tlp_SRC_Table;
        private System.Windows.Forms.TableLayoutPanel tlp_TRG_Table;

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Top;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Compare;
        private System.Windows.Forms.TableLayoutPanel tlp_View;
        private System.Windows.Forms.TableLayoutPanel tlp_RefKey;
        private System.Windows.Forms.TableLayoutPanel tlp_SrcCols;
        private System.Windows.Forms.TableLayoutPanel tlp_TrgCols;
        private System.Windows.Forms.TableLayoutPanel tlp_IgnSrc;
        private System.Windows.Forms.TableLayoutPanel tlp_IgnTrg;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;
        private System.Windows.Forms.TableLayoutPanel tlp_Progress;

        private System.Windows.Forms.Button btnGenerateOutput;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btn_Connect_Reference_ID;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblExpressionStatus;
        private System.Windows.Forms.CheckedListBox cbL_SRC_Cols;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cb_selectAll_toggle;
        private System.Windows.Forms.Button btn_Set;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.LinkLabel llbl_File_path;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tb_Source_tableName_1;
        private System.Windows.Forms.TextBox tb_destination_tableName;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btn_Clear_Reset;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbl_PercentageShow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckedListBox clb_ReferenceID;
        private System.Windows.Forms.Label lblCal;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
        private System.Windows.Forms.CheckBox cb_IncludeRule;
        private System.Windows.Forms.Button btn_RuleForm;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.CheckBox cb_ColumnMapping;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lb_IgnoredCol_TRG;
        private System.Windows.Forms.ListBox lb_IgnoredCol_SRC;
        private System.Windows.Forms.ListBox lb_TRG_Cols;
        private System.Windows.Forms.ComboBox cb_TRG_TableName;
        private System.Windows.Forms.Button btn_Load_TRG_Table;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btn_Load_SRC_Table;
        private System.Windows.Forms.ComboBox cb_SRC_TableName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox cb_FullName;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.CheckBox cb_PrimaryKey;
        private System.Windows.Forms.Timer timer1;
    }
}

