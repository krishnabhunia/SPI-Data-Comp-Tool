namespace SPI_Data_Comp_Tool
{
    partial class Execute_Excel_Scripts_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Execute_Excel_Scripts_Form));
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.gb_ExcelFile = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.chkb_SelectQuery = new System.Windows.Forms.CheckBox();
            this.cmb_SelectFileName = new System.Windows.Forms.ComboBox();
            this.btn_Execute = new System.Windows.Forms.Button();
            this.lbl_ExcelFile = new System.Windows.Forms.Label();
            this.clb_Checked = new System.Windows.Forms.CheckedListBox();
            this.btn_ExecuteScript = new System.Windows.Forms.Button();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.btnTestConnecton = new System.Windows.Forms.Button();
            this.rb_TRG = new System.Windows.Forms.RadioButton();
            this.rb_SRC = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_Excel = new System.Windows.Forms.TabPage();
            this.tabPage_Txt = new System.Windows.Forms.TabPage();
            this.gb_TextFile = new System.Windows.Forms.GroupBox();
            this.btn_ExecuteFromTextFiles = new System.Windows.Forms.Button();
            this.llb_Excel_TRG = new System.Windows.Forms.LinkLabel();
            this.llb_Excel_SRC = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage_PNID_Reports = new System.Windows.Forms.TabPage();
            this.cb_Toggle = new System.Windows.Forms.CheckBox();
            this.cb_SelectAll = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tb_UserCondition = new System.Windows.Forms.TextBox();
            this.tb_IpepSchema = new System.Windows.Forms.TextBox();
            this.tb_Path = new System.Windows.Forms.TextBox();
            this.cb_IpepSchema = new System.Windows.Forms.CheckBox();
            this.cb_Path = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.llb_PNID_Report_Path = new System.Windows.Forms.LinkLabel();
            this.gb_Schema = new System.Windows.Forms.GroupBox();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.tb_CommonSchema = new System.Windows.Forms.TextBox();
            this.cb_CommonSchema = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tb_Schema_3 = new System.Windows.Forms.TextBox();
            this.tb_Schema_2 = new System.Windows.Forms.TextBox();
            this.tb_Schema_1 = new System.Windows.Forms.TextBox();
            this.cb_Schema3 = new System.Windows.Forms.CheckBox();
            this.cb_Schema2 = new System.Windows.Forms.CheckBox();
            this.cb_Schema1 = new System.Windows.Forms.CheckBox();
            this.clb_PNID_Reports = new System.Windows.Forms.CheckedListBox();
            this.btn_PNID_Report = new System.Windows.Forms.Button();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.flp_LoadFrom = new System.Windows.Forms.FlowLayoutPanel();
            this.lbl_LoadFrom = new System.Windows.Forms.Label();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.flp_LoadFrom.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tlp_Status.SuspendLayout();
            this.gb_ExcelFile.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gb_TextFile.SuspendLayout();
            this.tabPage_Excel.SuspendLayout();
            this.tabPage_Txt.SuspendLayout();
            this.tabPage_PNID_Reports.SuspendLayout();
            this.gb_Schema.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 420F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowCount = 2;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.groupBox12, 0, 0);
            this.tlp_Root.Controls.Add(this.tabControl1, 1, 0);
            this.tlp_Root.Controls.Add(this.groupBox10, 0, 1);
            this.tlp_Root.SetColumnSpan(this.groupBox10, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // groupBox12
            // 
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Connection";
            this.groupBox12.Controls.Add(this.tlp_Connection);
            this.groupBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox12.AutoSize = true;
            this.groupBox12.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox12.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.groupBox12.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox12.TabIndex = 1;
            // 
            // tlp_Connection
            // 
            this.tlp_Connection.Name = "tlp_Connection";
            this.tlp_Connection.ColumnCount = 1;
            this.tlp_Connection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Connection.RowCount = 3;
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.Controls.Add(this.connection, 0, 0);
            this.tlp_Connection.Controls.Add(this.flp_LoadFrom, 0, 1);
            this.tlp_Connection.Controls.Add(this.btnTestConnecton, 0, 2);
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
            // flp_LoadFrom
            // 
            this.flp_LoadFrom.Name = "flp_LoadFrom";
            this.flp_LoadFrom.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flp_LoadFrom.WrapContents = false;
            this.flp_LoadFrom.Controls.Add(this.lbl_LoadFrom);
            this.flp_LoadFrom.Controls.Add(this.rb_SRC);
            this.flp_LoadFrom.Controls.Add(this.rb_TRG);
            this.flp_LoadFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.flp_LoadFrom.AutoSize = true;
            this.flp_LoadFrom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flp_LoadFrom.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flp_LoadFrom.TabIndex = 4;
            // 
            // lbl_LoadFrom
            // 
            this.lbl_LoadFrom.Name = "lbl_LoadFrom";
            this.lbl_LoadFrom.Text = "Fill from profile half";
            this.lbl_LoadFrom.AutoSize = true;
            this.lbl_LoadFrom.Margin = new System.Windows.Forms.Padding(0, 4, 12, 0);
            this.lbl_LoadFrom.TabIndex = 5;
            // 
            // rb_SRC
            // 
            this.rb_SRC.Checked = true;
            this.rb_SRC.Name = "rb_SRC";
            this.rb_SRC.TabStop = true;
            this.rb_SRC.UseVisualStyleBackColor = true;
            this.rb_SRC.CheckedChanged += new System.EventHandler(this.rb_SRC_CheckedChanged);
            this.rb_SRC.Text = "Source";
            this.rb_SRC.AutoSize = true;
            this.rb_SRC.Margin = new System.Windows.Forms.Padding(0, 2, 16, 0);
            this.rb_SRC.TabIndex = 6;
            // 
            // rb_TRG
            // 
            this.rb_TRG.Name = "rb_TRG";
            this.rb_TRG.UseVisualStyleBackColor = true;
            this.rb_TRG.CheckedChanged += new System.EventHandler(this.rb_TRG_CheckedChanged);
            this.rb_TRG.Text = "Target";
            this.rb_TRG.AutoSize = true;
            this.rb_TRG.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.rb_TRG.TabIndex = 7;
            // 
            // btnTestConnecton
            // 
            this.btnTestConnecton.Name = "btnTestConnecton";
            this.btnTestConnecton.UseVisualStyleBackColor = true;
            this.btnTestConnecton.Click += new System.EventHandler(this.btnTestConnecton_Click);
            this.btnTestConnecton.Text = "Test connection";
            this.btnTestConnecton.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btnTestConnecton.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.btnTestConnecton.Size = new System.Drawing.Size(150, 30);
            this.btnTestConnecton.TabIndex = 8;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_Excel);
            this.tabControl1.Controls.Add(this.tabPage_Txt);
            this.tabControl1.Controls.Add(this.tabPage_PNID_Reports);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tabControl1.Size = new System.Drawing.Size(860, 400);
            this.tabControl1.TabIndex = 9;
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
            this.groupBox10.TabIndex = 10;
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
            this.tlp_Status.TabIndex = 11;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 12;
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Name = "btn_Clear_Reset";
            this.btn_Clear_Reset.UseVisualStyleBackColor = true;
            this.btn_Clear_Reset.Click += new System.EventHandler(this.btn_Clear_Reset_Click);
            this.btn_Clear_Reset.Text = "Clear / Reset";
            this.btn_Clear_Reset.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Clear_Reset.Size = new System.Drawing.Size(110, 30);
            this.btn_Clear_Reset.TabIndex = 13;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.rtb_Log.TabIndex = 14;
            // 
            // gb_ExcelFile
            // 
            this.gb_ExcelFile.Controls.Add(this.groupBox3);
            this.gb_ExcelFile.Controls.Add(this.btn_Execute);
            this.gb_ExcelFile.Controls.Add(this.lbl_ExcelFile);
            this.gb_ExcelFile.Controls.Add(this.clb_Checked);
            this.gb_ExcelFile.Controls.Add(this.btn_ExecuteScript);
            this.gb_ExcelFile.Location = new System.Drawing.Point(12, 15);
            this.gb_ExcelFile.Name = "gb_ExcelFile";
            this.gb_ExcelFile.Size = new System.Drawing.Size(824, 335);
            this.gb_ExcelFile.TabIndex = 37;
            this.gb_ExcelFile.TabStop = false;
            this.gb_ExcelFile.Text = "Controls";
            // 
            // btn_ExecuteScript
            // 
            this.btn_ExecuteScript.Location = new System.Drawing.Point(10, 19);
            this.btn_ExecuteScript.Name = "btn_ExecuteScript";
            this.btn_ExecuteScript.Size = new System.Drawing.Size(104, 25);
            this.btn_ExecuteScript.TabIndex = 40;
            this.btn_ExecuteScript.Text = "Browse Excel File";
            this.btn_ExecuteScript.UseVisualStyleBackColor = true;
            this.btn_ExecuteScript.Click += new System.EventHandler(this.btn_ExecuteScript_Click);
            // 
            // btn_Execute
            // 
            this.btn_Execute.Location = new System.Drawing.Point(243, 297);
            this.btn_Execute.Name = "btn_Execute";
            this.btn_Execute.Size = new System.Drawing.Size(75, 23);
            this.btn_Execute.TabIndex = 43;
            this.btn_Execute.Text = "Execute";
            this.btn_Execute.UseVisualStyleBackColor = true;
            this.btn_Execute.Click += new System.EventHandler(this.btn_Execute_Click);
            // 
            // lbl_ExcelFile
            // 
            this.lbl_ExcelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ExcelFile.Location = new System.Drawing.Point(491, 21);
            this.lbl_ExcelFile.Name = "lbl_ExcelFile";
            this.lbl_ExcelFile.Size = new System.Drawing.Size(315, 267);
            this.lbl_ExcelFile.TabIndex = 42;
            this.lbl_ExcelFile.Text = "label1";
            this.lbl_ExcelFile.Visible = false;
            // 
            // clb_Checked
            // 
            this.clb_Checked.CheckOnClick = true;
            this.clb_Checked.FormattingEnabled = true;
            this.clb_Checked.Location = new System.Drawing.Point(10, 54);
            this.clb_Checked.Name = "clb_Checked";
            this.clb_Checked.Size = new System.Drawing.Size(219, 244);
            this.clb_Checked.TabIndex = 41;
            // 
            // chkb_SelectQuery
            // 
            this.chkb_SelectQuery.AutoSize = true;
            this.chkb_SelectQuery.Location = new System.Drawing.Point(11, 22);
            this.chkb_SelectQuery.Name = "chkb_SelectQuery";
            this.chkb_SelectQuery.Size = new System.Drawing.Size(131, 17);
            this.chkb_SelectQuery.TabIndex = 44;
            this.chkb_SelectQuery.Text = "Contains Select Query";
            this.chkb_SelectQuery.UseVisualStyleBackColor = true;
            this.chkb_SelectQuery.CheckedChanged += new System.EventHandler(this.chkb_SelectQuery_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.linkLabel1);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.chkb_SelectQuery);
            this.groupBox3.Controls.Add(this.cmb_SelectFileName);
            this.groupBox3.Location = new System.Drawing.Point(243, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(242, 272);
            this.groupBox3.TabIndex = 46;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 38);
            this.label1.TabIndex = 46;
            this.label1.Text = "Select Column  File Name to Be Saved With : ";
            // 
            // cmb_SelectFileName
            // 
            this.cmb_SelectFileName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_SelectFileName.Enabled = false;
            this.cmb_SelectFileName.FormattingEnabled = true;
            this.cmb_SelectFileName.Location = new System.Drawing.Point(11, 87);
            this.cmb_SelectFileName.Name = "cmb_SelectFileName";
            this.cmb_SelectFileName.Size = new System.Drawing.Size(224, 21);
            this.cmb_SelectFileName.TabIndex = 45;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(8, 116);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(228, 139);
            this.linkLabel1.TabIndex = 47;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "D:/";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // gb_TextFile
            // 
            this.gb_TextFile.Controls.Add(this.btn_ExecuteFromTextFiles);
            this.gb_TextFile.Controls.Add(this.llb_Excel_TRG);
            this.gb_TextFile.Controls.Add(this.llb_Excel_SRC);
            this.gb_TextFile.Controls.Add(this.label4);
            this.gb_TextFile.Controls.Add(this.label2);
            this.gb_TextFile.Location = new System.Drawing.Point(6, 6);
            this.gb_TextFile.Name = "gb_TextFile";
            this.gb_TextFile.Size = new System.Drawing.Size(824, 331);
            this.gb_TextFile.TabIndex = 40;
            this.gb_TextFile.TabStop = false;
            this.gb_TextFile.Text = "Select Queries from Text Files ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Output Excel File Folder : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Input Texts File Folder : ";
            // 
            // llb_Excel_TRG
            // 
            this.llb_Excel_TRG.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llb_Excel_TRG.Location = new System.Drawing.Point(151, 154);
            this.llb_Excel_TRG.Name = "llb_Excel_TRG";
            this.llb_Excel_TRG.Size = new System.Drawing.Size(653, 140);
            this.llb_Excel_TRG.TabIndex = 3;
            this.llb_Excel_TRG.TabStop = true;
            this.llb_Excel_TRG.Text = "No Target Folder Selected";
            this.llb_Excel_TRG.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Excel_TRG_LinkClicked);
            // 
            // llb_Excel_SRC
            // 
            this.llb_Excel_SRC.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llb_Excel_SRC.Location = new System.Drawing.Point(154, 16);
            this.llb_Excel_SRC.Name = "llb_Excel_SRC";
            this.llb_Excel_SRC.Size = new System.Drawing.Size(650, 122);
            this.llb_Excel_SRC.TabIndex = 2;
            this.llb_Excel_SRC.TabStop = true;
            this.llb_Excel_SRC.Text = "No Source Folder Selected";
            this.llb_Excel_SRC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Excel_SRC_LinkClicked);
            // 
            // btn_ExecuteFromTextFiles
            // 
            this.btn_ExecuteFromTextFiles.Location = new System.Drawing.Point(15, 297);
            this.btn_ExecuteFromTextFiles.Name = "btn_ExecuteFromTextFiles";
            this.btn_ExecuteFromTextFiles.Size = new System.Drawing.Size(75, 23);
            this.btn_ExecuteFromTextFiles.TabIndex = 47;
            this.btn_ExecuteFromTextFiles.Text = "Execute";
            this.btn_ExecuteFromTextFiles.UseVisualStyleBackColor = true;
            this.btn_ExecuteFromTextFiles.Click += new System.EventHandler(this.btn_ExecuteFromTextFiles_Click);
            // 
            // tabPage_Excel
            // 
            this.tabPage_Excel.Controls.Add(this.gb_ExcelFile);
            this.tabPage_Excel.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Excel.Name = "tabPage_Excel";
            this.tabPage_Excel.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Excel.Size = new System.Drawing.Size(847, 362);
            this.tabPage_Excel.TabIndex = 0;
            this.tabPage_Excel.Text = "Input as Excel";
            this.tabPage_Excel.UseVisualStyleBackColor = true;
            // 
            // tabPage_Txt
            // 
            this.tabPage_Txt.Controls.Add(this.gb_TextFile);
            this.tabPage_Txt.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Txt.Name = "tabPage_Txt";
            this.tabPage_Txt.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Txt.Size = new System.Drawing.Size(847, 362);
            this.tabPage_Txt.TabIndex = 1;
            this.tabPage_Txt.Text = "Input As Text";
            this.tabPage_Txt.UseVisualStyleBackColor = true;
            // 
            // tabPage_PNID_Reports
            // 
            this.tabPage_PNID_Reports.Controls.Add(this.cb_Toggle);
            this.tabPage_PNID_Reports.Controls.Add(this.cb_SelectAll);
            this.tabPage_PNID_Reports.Controls.Add(this.groupBox5);
            this.tabPage_PNID_Reports.Controls.Add(this.groupBox1);
            this.tabPage_PNID_Reports.Controls.Add(this.gb_Schema);
            this.tabPage_PNID_Reports.Controls.Add(this.clb_PNID_Reports);
            this.tabPage_PNID_Reports.Controls.Add(this.btn_PNID_Report);
            this.tabPage_PNID_Reports.Location = new System.Drawing.Point(4, 22);
            this.tabPage_PNID_Reports.Name = "tabPage_PNID_Reports";
            this.tabPage_PNID_Reports.Size = new System.Drawing.Size(847, 362);
            this.tabPage_PNID_Reports.TabIndex = 2;
            this.tabPage_PNID_Reports.Text = "PNID Reports";
            this.tabPage_PNID_Reports.UseVisualStyleBackColor = true;
            // 
            // clb_PNID_Reports
            // 
            this.clb_PNID_Reports.CheckOnClick = true;
            this.clb_PNID_Reports.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clb_PNID_Reports.FormattingEnabled = true;
            this.clb_PNID_Reports.Location = new System.Drawing.Point(13, 32);
            this.clb_PNID_Reports.Name = "clb_PNID_Reports";
            this.clb_PNID_Reports.Size = new System.Drawing.Size(236, 289);
            this.clb_PNID_Reports.TabIndex = 1;
            // 
            // btn_PNID_Report
            // 
            this.btn_PNID_Report.Location = new System.Drawing.Point(13, 328);
            this.btn_PNID_Report.Name = "btn_PNID_Report";
            this.btn_PNID_Report.Size = new System.Drawing.Size(132, 23);
            this.btn_PNID_Report.TabIndex = 0;
            this.btn_PNID_Report.Text = "Generate PNID Report";
            this.btn_PNID_Report.UseVisualStyleBackColor = true;
            this.btn_PNID_Report.Click += new System.EventHandler(this.btn_PNID_Report_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(16, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 16);
            this.label6.TabIndex = 3;
            this.label6.Text = "Select Path To Save";
            // 
            // llb_PNID_Report_Path
            // 
            this.llb_PNID_Report_Path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llb_PNID_Report_Path.Location = new System.Drawing.Point(18, 32);
            this.llb_PNID_Report_Path.Name = "llb_PNID_Report_Path";
            this.llb_PNID_Report_Path.Size = new System.Drawing.Size(553, 48);
            this.llb_PNID_Report_Path.TabIndex = 2;
            this.llb_PNID_Report_Path.TabStop = true;
            this.llb_PNID_Report_Path.Text = "No Output Folder Selected";
            this.llb_PNID_Report_Path.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Path_LinkClicked);
            // 
            // gb_Schema
            // 
            this.gb_Schema.Controls.Add(this.btn_Reset);
            this.gb_Schema.Controls.Add(this.tb_CommonSchema);
            this.gb_Schema.Controls.Add(this.cb_CommonSchema);
            this.gb_Schema.Controls.Add(this.label10);
            this.gb_Schema.Controls.Add(this.label8);
            this.gb_Schema.Controls.Add(this.tb_Schema_3);
            this.gb_Schema.Controls.Add(this.tb_Schema_2);
            this.gb_Schema.Controls.Add(this.tb_Schema_1);
            this.gb_Schema.Controls.Add(this.cb_Schema3);
            this.gb_Schema.Controls.Add(this.cb_Schema2);
            this.gb_Schema.Controls.Add(this.cb_Schema1);
            this.gb_Schema.Location = new System.Drawing.Point(256, 9);
            this.gb_Schema.Name = "gb_Schema";
            this.gb_Schema.Size = new System.Drawing.Size(577, 126);
            this.gb_Schema.TabIndex = 4;
            this.gb_Schema.TabStop = false;
            this.gb_Schema.Text = "Source";
            // 
            // tb_Path
            // 
            this.tb_Path.Enabled = false;
            this.tb_Path.Location = new System.Drawing.Point(125, 36);
            this.tb_Path.Name = "tb_Path";
            this.tb_Path.Size = new System.Drawing.Size(265, 20);
            this.tb_Path.TabIndex = 7;
            // 
            // tb_Schema_3
            // 
            this.tb_Schema_3.Enabled = false;
            this.tb_Schema_3.Location = new System.Drawing.Point(125, 91);
            this.tb_Schema_3.Name = "tb_Schema_3";
            this.tb_Schema_3.Size = new System.Drawing.Size(265, 20);
            this.tb_Schema_3.TabIndex = 6;
            this.tb_Schema_3.Visible = false;
            // 
            // tb_Schema_2
            // 
            this.tb_Schema_2.Enabled = false;
            this.tb_Schema_2.Location = new System.Drawing.Point(125, 65);
            this.tb_Schema_2.Name = "tb_Schema_2";
            this.tb_Schema_2.Size = new System.Drawing.Size(265, 20);
            this.tb_Schema_2.TabIndex = 5;
            // 
            // tb_Schema_1
            // 
            this.tb_Schema_1.Enabled = false;
            this.tb_Schema_1.Location = new System.Drawing.Point(125, 39);
            this.tb_Schema_1.Name = "tb_Schema_1";
            this.tb_Schema_1.Size = new System.Drawing.Size(265, 20);
            this.tb_Schema_1.TabIndex = 4;
            // 
            // cb_Path
            // 
            this.cb_Path.AutoSize = true;
            this.cb_Path.Location = new System.Drawing.Point(13, 39);
            this.cb_Path.Name = "cb_Path";
            this.cb_Path.Size = new System.Drawing.Size(90, 17);
            this.cb_Path.TabIndex = 3;
            this.cb_Path.Text = "Drawing Path";
            this.cb_Path.UseVisualStyleBackColor = true;
            this.cb_Path.CheckedChanged += new System.EventHandler(this.cb_Path_CheckedChanged);
            // 
            // cb_Schema3
            // 
            this.cb_Schema3.AutoSize = true;
            this.cb_Schema3.Location = new System.Drawing.Point(13, 93);
            this.cb_Schema3.Name = "cb_Schema3";
            this.cb_Schema3.Size = new System.Drawing.Size(74, 17);
            this.cb_Schema3.TabIndex = 2;
            this.cb_Schema3.Text = "Schema 3";
            this.cb_Schema3.UseVisualStyleBackColor = true;
            this.cb_Schema3.Visible = false;
            this.cb_Schema3.CheckedChanged += new System.EventHandler(this.cb_Schema3_CheckedChanged);
            // 
            // cb_Schema2
            // 
            this.cb_Schema2.AutoSize = true;
            this.cb_Schema2.Location = new System.Drawing.Point(13, 67);
            this.cb_Schema2.Name = "cb_Schema2";
            this.cb_Schema2.Size = new System.Drawing.Size(74, 17);
            this.cb_Schema2.TabIndex = 1;
            this.cb_Schema2.Text = "Schema 2";
            this.cb_Schema2.UseVisualStyleBackColor = true;
            this.cb_Schema2.CheckedChanged += new System.EventHandler(this.cb_Schema2_CheckedChanged);
            // 
            // cb_Schema1
            // 
            this.cb_Schema1.AutoSize = true;
            this.cb_Schema1.Location = new System.Drawing.Point(13, 41);
            this.cb_Schema1.Name = "cb_Schema1";
            this.cb_Schema1.Size = new System.Drawing.Size(74, 17);
            this.cb_Schema1.TabIndex = 0;
            this.cb_Schema1.Text = "Schema 1";
            this.cb_Schema1.UseVisualStyleBackColor = true;
            this.cb_Schema1.CheckedChanged += new System.EventHandler(this.cb_Schema1_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(396, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "e.g CAOJING";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(396, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "e.g CAOJINGpid";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.llb_PNID_Report_Path);
            this.groupBox1.Location = new System.Drawing.Point(255, 233);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(577, 88);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // tb_CommonSchema
            // 
            this.tb_CommonSchema.Location = new System.Drawing.Point(125, 14);
            this.tb_CommonSchema.Name = "tb_CommonSchema";
            this.tb_CommonSchema.Size = new System.Drawing.Size(265, 20);
            this.tb_CommonSchema.TabIndex = 12;
            this.tb_CommonSchema.TextChanged += new System.EventHandler(this.tb_CommonSchema_TextChanged);
            // 
            // cb_CommonSchema
            // 
            this.cb_CommonSchema.AutoSize = true;
            this.cb_CommonSchema.Checked = true;
            this.cb_CommonSchema.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_CommonSchema.Location = new System.Drawing.Point(13, 16);
            this.cb_CommonSchema.Name = "cb_CommonSchema";
            this.cb_CommonSchema.Size = new System.Drawing.Size(109, 17);
            this.cb_CommonSchema.TabIndex = 11;
            this.cb_CommonSchema.Text = "Common Schema";
            this.cb_CommonSchema.UseVisualStyleBackColor = true;
            this.cb_CommonSchema.CheckedChanged += new System.EventHandler(this.cb_CommonSchema_CheckedChanged);
            // 
            // btn_Reset
            // 
            this.btn_Reset.Location = new System.Drawing.Point(399, 14);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(75, 23);
            this.btn_Reset.TabIndex = 13;
            this.btn_Reset.Text = "Reset";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // tb_IpepSchema
            // 
            this.tb_IpepSchema.Enabled = false;
            this.tb_IpepSchema.Location = new System.Drawing.Point(125, 13);
            this.tb_IpepSchema.Name = "tb_IpepSchema";
            this.tb_IpepSchema.Size = new System.Drawing.Size(265, 20);
            this.tb_IpepSchema.TabIndex = 15;
            // 
            // cb_IpepSchema
            // 
            this.cb_IpepSchema.AutoSize = true;
            this.cb_IpepSchema.Location = new System.Drawing.Point(13, 16);
            this.cb_IpepSchema.Name = "cb_IpepSchema";
            this.cb_IpepSchema.Size = new System.Drawing.Size(92, 17);
            this.cb_IpepSchema.TabIndex = 14;
            this.cb_IpepSchema.Text = "IPEP Schema";
            this.cb_IpepSchema.UseVisualStyleBackColor = true;
            this.cb_IpepSchema.CheckedChanged += new System.EventHandler(this.cb_IpepSchema_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.tb_UserCondition);
            this.groupBox5.Controls.Add(this.tb_IpepSchema);
            this.groupBox5.Controls.Add(this.tb_Path);
            this.groupBox5.Controls.Add(this.cb_IpepSchema);
            this.groupBox5.Controls.Add(this.cb_Path);
            this.groupBox5.Location = new System.Drawing.Point(256, 143);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(577, 85);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Target";
            // 
            // tb_UserCondition
            // 
            this.tb_UserCondition.Location = new System.Drawing.Point(125, 60);
            this.tb_UserCondition.Name = "tb_UserCondition";
            this.tb_UserCondition.Size = new System.Drawing.Size(265, 20);
            this.tb_UserCondition.TabIndex = 17;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(396, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(179, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "e.g and d.Name NOT LIKE \'%Test%\'";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(396, 40);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 13);
            this.label14.TabIndex = 20;
            this.label14.Text = "e.g CAO";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(396, 16);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "e.g AP_CAO";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(10, 63);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(85, 13);
            this.label16.TabIndex = 21;
            this.label16.Text = "User Condition : ";
            // 
            // cb_Toggle
            // 
            this.cb_Toggle.AutoSize = true;
            this.cb_Toggle.Location = new System.Drawing.Point(190, 9);
            this.cb_Toggle.Name = "cb_Toggle";
            this.cb_Toggle.Size = new System.Drawing.Size(59, 17);
            this.cb_Toggle.TabIndex = 18;
            this.cb_Toggle.Text = "Toggle";
            this.cb_Toggle.UseVisualStyleBackColor = true;
            this.cb_Toggle.CheckedChanged += new System.EventHandler(this.cb_Toggle_CheckedChanged);
            // 
            // cb_SelectAll
            // 
            this.cb_SelectAll.AutoSize = true;
            this.cb_SelectAll.Location = new System.Drawing.Point(13, 9);
            this.cb_SelectAll.Name = "cb_SelectAll";
            this.cb_SelectAll.Size = new System.Drawing.Size(70, 17);
            this.cb_SelectAll.TabIndex = 17;
            this.cb_SelectAll.Text = "Select All";
            this.cb_SelectAll.UseVisualStyleBackColor = true;
            this.cb_SelectAll.CheckedChanged += new System.EventHandler(this.cb_SelectAll_CheckedChanged);
            // 
            // Execute_Excel_Scripts_Form
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Execute_Excel_Scripts_Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = "Execute scripts";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 820);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.flp_LoadFrom.ResumeLayout(false);
            this.flp_LoadFrom.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.gb_ExcelFile.ResumeLayout(false);
            this.gb_ExcelFile.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gb_TextFile.ResumeLayout(false);
            this.gb_TextFile.PerformLayout();
            this.tabPage_Excel.ResumeLayout(false);
            this.tabPage_Excel.PerformLayout();
            this.tabPage_Txt.ResumeLayout(false);
            this.tabPage_Txt.PerformLayout();
            this.tabPage_PNID_Reports.ResumeLayout(false);
            this.tabPage_PNID_Reports.PerformLayout();
            this.gb_Schema.ResumeLayout(false);
            this.gb_Schema.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.FlowLayoutPanel flp_LoadFrom;
        private System.Windows.Forms.Label lbl_LoadFrom;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;

        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_Clear_Reset;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.GroupBox gb_ExcelFile;
        private System.Windows.Forms.Button btn_ExecuteScript;
        private System.Windows.Forms.Button btn_Execute;
        private System.Windows.Forms.Label lbl_ExcelFile;
        private System.Windows.Forms.CheckedListBox clb_Checked;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Button btnTestConnecton;
        private System.Windows.Forms.CheckBox chkb_SelectQuery;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmb_SelectFileName;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.GroupBox gb_TextFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel llb_Excel_TRG;
        private System.Windows.Forms.LinkLabel llb_Excel_SRC;
        private System.Windows.Forms.Button btn_ExecuteFromTextFiles;
        private System.Windows.Forms.RadioButton rb_TRG;
        private System.Windows.Forms.RadioButton rb_SRC;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_Excel;
        private System.Windows.Forms.TabPage tabPage_Txt;
        private System.Windows.Forms.TabPage tabPage_PNID_Reports;
        private System.Windows.Forms.CheckedListBox clb_PNID_Reports;
        private System.Windows.Forms.Button btn_PNID_Report;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel llb_PNID_Report_Path;
        private System.Windows.Forms.GroupBox gb_Schema;
        private System.Windows.Forms.TextBox tb_Path;
        private System.Windows.Forms.TextBox tb_Schema_3;
        private System.Windows.Forms.TextBox tb_Schema_2;
        private System.Windows.Forms.TextBox tb_Schema_1;
        private System.Windows.Forms.CheckBox cb_Path;
        private System.Windows.Forms.CheckBox cb_Schema3;
        private System.Windows.Forms.CheckBox cb_Schema2;
        private System.Windows.Forms.CheckBox cb_Schema1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_CommonSchema;
        private System.Windows.Forms.CheckBox cb_CommonSchema;
        private System.Windows.Forms.Button btn_Reset;
        private System.Windows.Forms.TextBox tb_IpepSchema;
        private System.Windows.Forms.CheckBox cb_IpepSchema;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tb_UserCondition;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox cb_Toggle;
        private System.Windows.Forms.CheckBox cb_SelectAll;
    }
}