namespace SPI_Data_Comp_Tool
{
    partial class Excel_Excel_Comparison
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Excel_Excel_Comparison));
            this.lblError = new System.Windows.Forms.Label();
            this.lbl_FirstExcelFileName = new System.Windows.Forms.Label();
            this.lbl_SecondExcelFileName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_SelectLoad = new System.Windows.Forms.Button();
            this.lbl_LoadStatus = new System.Windows.Forms.Label();
            this.lbl_Timer = new System.Windows.Forms.Label();
            this.btn_Compare = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_select_first_clb = new System.Windows.Forms.CheckBox();
            this.lb_IgnoreTable2 = new System.Windows.Forms.ListBox();
            this.lb_IgnoreTable1 = new System.Windows.Forms.ListBox();
            this.clb_secondExcel = new System.Windows.Forms.CheckedListBox();
            this.clb_firstExcel = new System.Windows.Forms.CheckedListBox();
            this.clb_reference_ID = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbl_Parameter = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_ProcessBy_MultiThreading = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Validate = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_fileName = new System.Windows.Forms.TextBox();
            this.llbl_filePath = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_Generate = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lbl_Percentage = new System.Windows.Forms.Label();
            this.bgw_multiThreading = new System.ComponentModel.BackgroundWorker();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btn_SecondExcelSheet = new System.Windows.Forms.Button();
            this.btn_FirstExcelSheet = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.Location = new System.Drawing.Point(6, 128);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(96, 13);
            this.lblError.TabIndex = 2;
            this.lblError.Text = "Error Status : Okay";
            // 
            // lbl_FirstExcelFileName
            // 
            this.lbl_FirstExcelFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_FirstExcelFileName.Location = new System.Drawing.Point(175, 18);
            this.lbl_FirstExcelFileName.Name = "lbl_FirstExcelFileName";
            this.lbl_FirstExcelFileName.Size = new System.Drawing.Size(1140, 64);
            this.lbl_FirstExcelFileName.TabIndex = 3;
            this.lbl_FirstExcelFileName.Text = "No File Selected";
            // 
            // lbl_SecondExcelFileName
            // 
            this.lbl_SecondExcelFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SecondExcelFileName.Location = new System.Drawing.Point(175, 82);
            this.lbl_SecondExcelFileName.Name = "lbl_SecondExcelFileName";
            this.lbl_SecondExcelFileName.Size = new System.Drawing.Size(1057, 65);
            this.lbl_SecondExcelFileName.TabIndex = 4;
            this.lbl_SecondExcelFileName.Text = "No File Selected";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_SelectLoad);
            this.groupBox1.Controls.Add(this.lbl_LoadStatus);
            this.groupBox1.Controls.Add(this.lbl_FirstExcelFileName);
            this.groupBox1.Controls.Add(this.lbl_SecondExcelFileName);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1346, 160);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Excel";
            // 
            // btn_SelectLoad
            // 
            this.btn_SelectLoad.Location = new System.Drawing.Point(16, 19);
            this.btn_SelectLoad.Name = "btn_SelectLoad";
            this.btn_SelectLoad.Size = new System.Drawing.Size(143, 23);
            this.btn_SelectLoad.TabIndex = 7;
            this.btn_SelectLoad.Text = "Select Both Files and Load";
            this.btn_SelectLoad.UseVisualStyleBackColor = true;
            this.btn_SelectLoad.Click += new System.EventHandler(this.btn_SelectLoad_Click);
            // 
            // lbl_LoadStatus
            // 
            this.lbl_LoadStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LoadStatus.Location = new System.Drawing.Point(16, 57);
            this.lbl_LoadStatus.Name = "lbl_LoadStatus";
            this.lbl_LoadStatus.Size = new System.Drawing.Size(143, 90);
            this.lbl_LoadStatus.TabIndex = 6;
            this.lbl_LoadStatus.Text = "Status : Okay";
            // 
            // lbl_Timer
            // 
            this.lbl_Timer.AutoSize = true;
            this.lbl_Timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Timer.Location = new System.Drawing.Point(7, 105);
            this.lbl_Timer.Name = "lbl_Timer";
            this.lbl_Timer.Size = new System.Drawing.Size(33, 13);
            this.lbl_Timer.TabIndex = 9;
            this.lbl_Timer.Text = "Timer";
            // 
            // btn_Compare
            // 
            this.btn_Compare.Location = new System.Drawing.Point(16, 117);
            this.btn_Compare.Name = "btn_Compare";
            this.btn_Compare.Size = new System.Drawing.Size(200, 23);
            this.btn_Compare.TabIndex = 8;
            this.btn_Compare.Text = "Start Compare and Generate Excel";
            this.btn_Compare.UseVisualStyleBackColor = true;
            this.btn_Compare.Click += new System.EventHandler(this.btn_Compare_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cb_select_first_clb);
            this.groupBox2.Controls.Add(this.lb_IgnoreTable2);
            this.groupBox2.Controls.Add(this.lb_IgnoreTable1);
            this.groupBox2.Controls.Add(this.clb_secondExcel);
            this.groupBox2.Controls.Add(this.clb_firstExcel);
            this.groupBox2.Controls.Add(this.clb_reference_ID);
            this.groupBox2.Location = new System.Drawing.Point(12, 171);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(872, 320);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "View Data";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(690, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(156, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Ignored Column in Target Table";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(522, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(164, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Ignored Columns in Source Table";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(353, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Target Table Columns";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(181, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Source Table Columns";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Select Multiple Reference ID";
            // 
            // cb_select_first_clb
            // 
            this.cb_select_first_clb.AutoSize = true;
            this.cb_select_first_clb.Location = new System.Drawing.Point(184, 293);
            this.cb_select_first_clb.Name = "cb_select_first_clb";
            this.cb_select_first_clb.Size = new System.Drawing.Size(149, 17);
            this.cb_select_first_clb.TabIndex = 4;
            this.cb_select_first_clb.Text = "Select All / None (Toggle)";
            this.cb_select_first_clb.UseVisualStyleBackColor = true;
            this.cb_select_first_clb.CheckedChanged += new System.EventHandler(this.cb_select_first_clb_CheckedChanged);
            // 
            // lb_IgnoreTable2
            // 
            this.lb_IgnoreTable2.FormattingEnabled = true;
            this.lb_IgnoreTable2.Location = new System.Drawing.Point(693, 45);
            this.lb_IgnoreTable2.Name = "lb_IgnoreTable2";
            this.lb_IgnoreTable2.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoreTable2.Size = new System.Drawing.Size(150, 238);
            this.lb_IgnoreTable2.TabIndex = 3;
            // 
            // lb_IgnoreTable1
            // 
            this.lb_IgnoreTable1.FormattingEnabled = true;
            this.lb_IgnoreTable1.Location = new System.Drawing.Point(525, 45);
            this.lb_IgnoreTable1.Name = "lb_IgnoreTable1";
            this.lb_IgnoreTable1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoreTable1.Size = new System.Drawing.Size(150, 238);
            this.lb_IgnoreTable1.TabIndex = 2;
            // 
            // clb_secondExcel
            // 
            this.clb_secondExcel.CheckOnClick = true;
            this.clb_secondExcel.FormattingEnabled = true;
            this.clb_secondExcel.Location = new System.Drawing.Point(356, 45);
            this.clb_secondExcel.Name = "clb_secondExcel";
            this.clb_secondExcel.Size = new System.Drawing.Size(150, 244);
            this.clb_secondExcel.TabIndex = 2;
            // 
            // clb_firstExcel
            // 
            this.clb_firstExcel.CheckOnClick = true;
            this.clb_firstExcel.FormattingEnabled = true;
            this.clb_firstExcel.Location = new System.Drawing.Point(184, 45);
            this.clb_firstExcel.Name = "clb_firstExcel";
            this.clb_firstExcel.Size = new System.Drawing.Size(150, 244);
            this.clb_firstExcel.TabIndex = 1;
            this.clb_firstExcel.SelectedIndexChanged += new System.EventHandler(this.clb_firstExcel_SelectedIndexChanged);
            // 
            // clb_reference_ID
            // 
            this.clb_reference_ID.CheckOnClick = true;
            this.clb_reference_ID.FormattingEnabled = true;
            this.clb_reference_ID.Location = new System.Drawing.Point(16, 45);
            this.clb_reference_ID.Name = "clb_reference_ID";
            this.clb_reference_ID.Size = new System.Drawing.Size(150, 244);
            this.clb_reference_ID.TabIndex = 0;
            this.clb_reference_ID.SelectedIndexChanged += new System.EventHandler(this.clb_reference_ID_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbl_Timer);
            this.groupBox3.Controls.Add(this.lbl_Parameter);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.lblError);
            this.groupBox3.Controls.Add(this.lbl_Status);
            this.groupBox3.Location = new System.Drawing.Point(902, 341);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(456, 150);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Status";
            // 
            // lbl_Parameter
            // 
            this.lbl_Parameter.AutoSize = true;
            this.lbl_Parameter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Parameter.Location = new System.Drawing.Point(7, 85);
            this.lbl_Parameter.Name = "lbl_Parameter";
            this.lbl_Parameter.Size = new System.Drawing.Size(60, 13);
            this.lbl_Parameter.TabIndex = 3;
            this.lbl_Parameter.Text = "Parameters";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(7, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Status : ";
            // 
            // lbl_Status
            // 
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(59, 19);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(377, 52);
            this.lbl_Status.TabIndex = 1;
            this.lbl_Status.Text = "Okay";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_ProcessBy_MultiThreading);
            this.groupBox4.Controls.Add(this.btn_Cancel);
            this.groupBox4.Controls.Add(this.btn_Compare);
            this.groupBox4.Controls.Add(this.btn_Validate);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.tb_fileName);
            this.groupBox4.Controls.Add(this.llbl_filePath);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(902, 171);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(333, 150);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output";
            // 
            // btn_ProcessBy_MultiThreading
            // 
            this.btn_ProcessBy_MultiThreading.Location = new System.Drawing.Point(99, 83);
            this.btn_ProcessBy_MultiThreading.Margin = new System.Windows.Forms.Padding(1);
            this.btn_ProcessBy_MultiThreading.Name = "btn_ProcessBy_MultiThreading";
            this.btn_ProcessBy_MultiThreading.Size = new System.Drawing.Size(193, 23);
            this.btn_ProcessBy_MultiThreading.TabIndex = 10;
            this.btn_ProcessBy_MultiThreading.Text = "Process By MultiThreading";
            this.btn_ProcessBy_MultiThreading.UseVisualStyleBackColor = true;
            this.btn_ProcessBy_MultiThreading.Click += new System.EventHandler(this.btn_ProcessBy_MultiThreading_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(222, 117);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 9;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Validate
            // 
            this.btn_Validate.Location = new System.Drawing.Point(16, 83);
            this.btn_Validate.Name = "btn_Validate";
            this.btn_Validate.Size = new System.Drawing.Size(75, 23);
            this.btn_Validate.TabIndex = 6;
            this.btn_Validate.Text = "Validate";
            this.btn_Validate.UseVisualStyleBackColor = true;
            this.btn_Validate.Click += new System.EventHandler(this.btn_Validate_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Filename : ";
            // 
            // tb_fileName
            // 
            this.tb_fileName.Location = new System.Drawing.Point(80, 49);
            this.tb_fileName.Name = "tb_fileName";
            this.tb_fileName.Size = new System.Drawing.Size(215, 20);
            this.tb_fileName.TabIndex = 4;
            // 
            // llbl_filePath
            // 
            this.llbl_filePath.AutoSize = true;
            this.llbl_filePath.Location = new System.Drawing.Point(62, 22);
            this.llbl_filePath.Name = "llbl_filePath";
            this.llbl_filePath.Size = new System.Drawing.Size(55, 13);
            this.llbl_filePath.TabIndex = 3;
            this.llbl_filePath.TabStop = true;
            this.llbl_filePath.Text = "linkLabel1";
            this.llbl_filePath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Path : ";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 19);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(827, 38);
            this.progressBar1.TabIndex = 8;
            // 
            // btn_Generate
            // 
            this.btn_Generate.Enabled = false;
            this.btn_Generate.Location = new System.Drawing.Point(446, 621);
            this.btn_Generate.Name = "btn_Generate";
            this.btn_Generate.Size = new System.Drawing.Size(239, 23);
            this.btn_Generate.TabIndex = 0;
            this.btn_Generate.Text = "OLD Start Compare and Generate Excel Report";
            this.btn_Generate.UseVisualStyleBackColor = true;
            this.btn_Generate.Visible = false;
            this.btn_Generate.Click += new System.EventHandler(this.btn_Generate_Click);
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Location = new System.Drawing.Point(12, 622);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Size = new System.Drawing.Size(425, 32);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // lbl_Percentage
            // 
            this.lbl_Percentage.AutoSize = true;
            this.lbl_Percentage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Percentage.Location = new System.Drawing.Point(377, 29);
            this.lbl_Percentage.Name = "lbl_Percentage";
            this.lbl_Percentage.Size = new System.Drawing.Size(87, 16);
            this.lbl_Percentage.TabIndex = 9;
            this.lbl_Percentage.Text = "Progress Bar";
            // 
            // bgw_multiThreading
            // 
            this.bgw_multiThreading.WorkerReportsProgress = true;
            this.bgw_multiThreading.WorkerSupportsCancellation = true;
            this.bgw_multiThreading.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_multiThreading_DoWork);
            this.bgw_multiThreading.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_multiThreading_ProgressChanged);
            this.bgw_multiThreading.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_multiThreading_RunWorkerCompleted);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(809, 677);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 12;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Visible = false;
            // 
            // btn_SecondExcelSheet
            // 
            this.btn_SecondExcelSheet.Location = new System.Drawing.Point(731, 648);
            this.btn_SecondExcelSheet.Name = "btn_SecondExcelSheet";
            this.btn_SecondExcelSheet.Size = new System.Drawing.Size(153, 23);
            this.btn_SecondExcelSheet.TabIndex = 11;
            this.btn_SecondExcelSheet.Text = "Select Second Excel Sheet";
            this.btn_SecondExcelSheet.UseVisualStyleBackColor = true;
            this.btn_SecondExcelSheet.Visible = false;
            // 
            // btn_FirstExcelSheet
            // 
            this.btn_FirstExcelSheet.Location = new System.Drawing.Point(731, 619);
            this.btn_FirstExcelSheet.Name = "btn_FirstExcelSheet";
            this.btn_FirstExcelSheet.Size = new System.Drawing.Size(153, 23);
            this.btn_FirstExcelSheet.TabIndex = 10;
            this.btn_FirstExcelSheet.Text = "Select First Excel Sheet";
            this.btn_FirstExcelSheet.UseVisualStyleBackColor = true;
            this.btn_FirstExcelSheet.Visible = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lbl_Percentage);
            this.groupBox5.Controls.Add(this.progressBar1);
            this.groupBox5.Location = new System.Drawing.Point(12, 498);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(872, 73);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Progress Status";
            // 
            // Excel_Excel_Comparison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1370, 749);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btn_SecondExcelSheet);
            this.Controls.Add(this.btn_FirstExcelSheet);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Generate);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Excel_Excel_Comparison";
            this.Text = "Excel_Excel_Comparison";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Excel_Excel_Comparison_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lbl_FirstExcelFileName;
        private System.Windows.Forms.Label lbl_SecondExcelFileName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox clb_secondExcel;
        private System.Windows.Forms.CheckedListBox clb_firstExcel;
        private System.Windows.Forms.CheckedListBox clb_reference_ID;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbl_LoadStatus;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btn_Generate;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.ListBox lb_IgnoreTable1;
        private System.Windows.Forms.ListBox lb_IgnoreTable2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.LinkLabel llbl_filePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_fileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cb_select_first_clb;
        private System.Windows.Forms.Button btn_SelectLoad;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Validate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_Compare;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lbl_Timer;
        private System.Windows.Forms.Label lbl_Percentage;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Label lbl_Parameter;
        private System.Windows.Forms.Button btn_ProcessBy_MultiThreading;
        private System.ComponentModel.BackgroundWorker bgw_multiThreading;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btn_SecondExcelSheet;
        private System.Windows.Forms.Button btn_FirstExcelSheet;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}