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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Excel_Excel_Comparison));
            this.lblError = new System.Windows.Forms.Label();
            this.lbl_FirstExcelFileName = new System.Windows.Forms.Label();
            this.lbl_SecondExcelFileName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_MultiReference = new System.Windows.Forms.CheckBox();
            this.btn_SelectLoad = new System.Windows.Forms.Button();
            this.lbl_LoadStatus = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lbl_Timer = new System.Windows.Forms.Label();
            this.btn_Compare = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_Primary_Key = new System.Windows.Forms.CheckBox();
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
            this.cb_IncludeDiff_Columns = new System.Windows.Forms.CheckBox();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.cb_IncludeRule = new System.Windows.Forms.CheckBox();
            this.btn_RuleForm = new System.Windows.Forms.Button();
            this.btn_ProcessBy_MultiThreading = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Validate = new System.Windows.Forms.Button();
            this.llbl_filePath = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lbl_Percentage = new System.Windows.Forms.Label();
            this.bgw_multiThreading = new System.ComponentModel.BackgroundWorker();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.backgroundWorkerInstrument = new System.ComponentModel.BackgroundWorker();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lbl_InstrumentText = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_TestSystemMemoryOutException = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.Location = new System.Drawing.Point(7, 129);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(300, 96);
            this.lblError.TabIndex = 2;
            this.lblError.Text = "Error Status : Okay";
            this.lblError.Visible = false;
            // 
            // lbl_FirstExcelFileName
            // 
            this.lbl_FirstExcelFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_FirstExcelFileName.Location = new System.Drawing.Point(277, 21);
            this.lbl_FirstExcelFileName.Name = "lbl_FirstExcelFileName";
            this.lbl_FirstExcelFileName.Size = new System.Drawing.Size(902, 53);
            this.lbl_FirstExcelFileName.TabIndex = 3;
            this.lbl_FirstExcelFileName.Text = "No File Selected";
            // 
            // lbl_SecondExcelFileName
            // 
            this.lbl_SecondExcelFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SecondExcelFileName.Location = new System.Drawing.Point(277, 83);
            this.lbl_SecondExcelFileName.Name = "lbl_SecondExcelFileName";
            this.lbl_SecondExcelFileName.Size = new System.Drawing.Size(905, 57);
            this.lbl_SecondExcelFileName.TabIndex = 4;
            this.lbl_SecondExcelFileName.Text = "No File Selected";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_MultiReference);
            this.groupBox1.Controls.Add(this.btn_SelectLoad);
            this.groupBox1.Controls.Add(this.lbl_LoadStatus);
            this.groupBox1.Controls.Add(this.lbl_FirstExcelFileName);
            this.groupBox1.Controls.Add(this.lbl_SecondExcelFileName);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1201, 160);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Excel";
            // 
            // cb_MultiReference
            // 
            this.cb_MultiReference.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_MultiReference.Location = new System.Drawing.Point(13, 21);
            this.cb_MultiReference.Name = "cb_MultiReference";
            this.cb_MultiReference.Size = new System.Drawing.Size(252, 37);
            this.cb_MultiReference.TabIndex = 13;
            this.cb_MultiReference.Text = "Select For Different Column Name and RefID comparison";
            this.cb_MultiReference.UseVisualStyleBackColor = true;
            // 
            // btn_SelectLoad
            // 
            this.btn_SelectLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SelectLoad.Location = new System.Drawing.Point(13, 64);
            this.btn_SelectLoad.Name = "btn_SelectLoad";
            this.btn_SelectLoad.Size = new System.Drawing.Size(258, 29);
            this.btn_SelectLoad.TabIndex = 7;
            this.btn_SelectLoad.Text = "Select Both Files and Load";
            this.btn_SelectLoad.UseVisualStyleBackColor = true;
            this.btn_SelectLoad.Click += new System.EventHandler(this.btn_SelectLoad_Click);
            // 
            // lbl_LoadStatus
            // 
            this.lbl_LoadStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LoadStatus.Location = new System.Drawing.Point(16, 99);
            this.lbl_LoadStatus.Name = "lbl_LoadStatus";
            this.lbl_LoadStatus.Size = new System.Drawing.Size(255, 28);
            this.lbl_LoadStatus.TabIndex = 6;
            this.lbl_LoadStatus.Text = "Status : Okay";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(13, 80);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(258, 23);
            this.btnLoad.TabIndex = 12;
            this.btnLoad.Text = "Generate Instrumentation Analysis Excel Input File";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click_1);
            // 
            // lbl_Timer
            // 
            this.lbl_Timer.AutoSize = true;
            this.lbl_Timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Timer.Location = new System.Drawing.Point(7, 110);
            this.lbl_Timer.Name = "lbl_Timer";
            this.lbl_Timer.Size = new System.Drawing.Size(33, 13);
            this.lbl_Timer.TabIndex = 9;
            this.lbl_Timer.Text = "Timer";
            // 
            // btn_Compare
            // 
            this.btn_Compare.Enabled = false;
            this.btn_Compare.Location = new System.Drawing.Point(19, 125);
            this.btn_Compare.Name = "btn_Compare";
            this.btn_Compare.Size = new System.Drawing.Size(200, 23);
            this.btn_Compare.TabIndex = 8;
            this.btn_Compare.Text = "Start Compare and Generate Excel";
            this.btn_Compare.UseVisualStyleBackColor = true;
            this.btn_Compare.Click += new System.EventHandler(this.btn_Compare_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_Primary_Key);
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
            this.groupBox2.Size = new System.Drawing.Size(879, 404);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "View Data";
            // 
            // cb_Primary_Key
            // 
            this.cb_Primary_Key.AutoSize = true;
            this.cb_Primary_Key.Checked = true;
            this.cb_Primary_Key.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Primary_Key.Location = new System.Drawing.Point(10, 379);
            this.cb_Primary_Key.Name = "cb_Primary_Key";
            this.cb_Primary_Key.Size = new System.Drawing.Size(169, 17);
            this.cb_Primary_Key.TabIndex = 10;
            this.cb_Primary_Key.Text = "If reference Key is Primary Key";
            this.cb_Primary_Key.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(754, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 16);
            this.label7.TabIndex = 9;
            this.label7.Text = "Ignored Col in Trg";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(621, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "Ignored Col in SRC";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(416, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Target Table Columns";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(210, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Source Table Columns";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(177, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Select Multiple Reference ID";
            // 
            // cb_select_first_clb
            // 
            this.cb_select_first_clb.AutoSize = true;
            this.cb_select_first_clb.Location = new System.Drawing.Point(213, 376);
            this.cb_select_first_clb.Name = "cb_select_first_clb";
            this.cb_select_first_clb.Size = new System.Drawing.Size(149, 17);
            this.cb_select_first_clb.TabIndex = 4;
            this.cb_select_first_clb.Text = "Select All / None (Toggle)";
            this.cb_select_first_clb.UseVisualStyleBackColor = true;
            this.cb_select_first_clb.CheckedChanged += new System.EventHandler(this.cb_select_first_clb_CheckedChanged);
            // 
            // lb_IgnoreTable2
            // 
            this.lb_IgnoreTable2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_IgnoreTable2.FormattingEnabled = true;
            this.lb_IgnoreTable2.HorizontalScrollbar = true;
            this.lb_IgnoreTable2.ItemHeight = 16;
            this.lb_IgnoreTable2.Location = new System.Drawing.Point(757, 44);
            this.lb_IgnoreTable2.Name = "lb_IgnoreTable2";
            this.lb_IgnoreTable2.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoreTable2.Size = new System.Drawing.Size(111, 324);
            this.lb_IgnoreTable2.TabIndex = 3;
            // 
            // lb_IgnoreTable1
            // 
            this.lb_IgnoreTable1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_IgnoreTable1.FormattingEnabled = true;
            this.lb_IgnoreTable1.HorizontalScrollbar = true;
            this.lb_IgnoreTable1.ItemHeight = 16;
            this.lb_IgnoreTable1.Location = new System.Drawing.Point(624, 45);
            this.lb_IgnoreTable1.Name = "lb_IgnoreTable1";
            this.lb_IgnoreTable1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoreTable1.Size = new System.Drawing.Size(118, 324);
            this.lb_IgnoreTable1.TabIndex = 2;
            // 
            // clb_secondExcel
            // 
            this.clb_secondExcel.CheckOnClick = true;
            this.clb_secondExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clb_secondExcel.FormattingEnabled = true;
            this.clb_secondExcel.HorizontalScrollbar = true;
            this.clb_secondExcel.Location = new System.Drawing.Point(419, 45);
            this.clb_secondExcel.Name = "clb_secondExcel";
            this.clb_secondExcel.Size = new System.Drawing.Size(190, 327);
            this.clb_secondExcel.TabIndex = 2;
            // 
            // clb_firstExcel
            // 
            this.clb_firstExcel.CheckOnClick = true;
            this.clb_firstExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clb_firstExcel.FormattingEnabled = true;
            this.clb_firstExcel.HorizontalScrollbar = true;
            this.clb_firstExcel.Location = new System.Drawing.Point(213, 45);
            this.clb_firstExcel.Name = "clb_firstExcel";
            this.clb_firstExcel.Size = new System.Drawing.Size(190, 327);
            this.clb_firstExcel.TabIndex = 1;
            this.clb_firstExcel.SelectedIndexChanged += new System.EventHandler(this.clb_firstExcel_SelectedIndexChanged);
            // 
            // clb_reference_ID
            // 
            this.clb_reference_ID.CheckOnClick = true;
            this.clb_reference_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clb_reference_ID.FormattingEnabled = true;
            this.clb_reference_ID.HorizontalScrollbar = true;
            this.clb_reference_ID.Location = new System.Drawing.Point(10, 45);
            this.clb_reference_ID.Name = "clb_reference_ID";
            this.clb_reference_ID.Size = new System.Drawing.Size(190, 327);
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
            this.groupBox3.Location = new System.Drawing.Point(897, 513);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(316, 235);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Status";
            // 
            // lbl_Parameter
            // 
            this.lbl_Parameter.AutoSize = true;
            this.lbl_Parameter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Parameter.Location = new System.Drawing.Point(7, 90);
            this.lbl_Parameter.Name = "lbl_Parameter";
            this.lbl_Parameter.Size = new System.Drawing.Size(60, 13);
            this.lbl_Parameter.TabIndex = 3;
            this.lbl_Parameter.Text = "Parameters";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(7, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 16);
            this.label8.TabIndex = 7;
            this.label8.Text = "Status : ";
            // 
            // lbl_Status
            // 
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(67, 19);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(239, 67);
            this.lbl_Status.TabIndex = 1;
            this.lbl_Status.Text = "Okay";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_TestSystemMemoryOutException);
            this.groupBox4.Controls.Add(this.cb_IncludeDiff_Columns);
            this.groupBox4.Controls.Add(this.cb_IgnoreCase);
            this.groupBox4.Controls.Add(this.cb_IncludeRule);
            this.groupBox4.Controls.Add(this.btn_RuleForm);
            this.groupBox4.Controls.Add(this.btn_ProcessBy_MultiThreading);
            this.groupBox4.Controls.Add(this.btn_Cancel);
            this.groupBox4.Controls.Add(this.btn_Compare);
            this.groupBox4.Controls.Add(this.btn_Validate);
            this.groupBox4.Controls.Add(this.llbl_filePath);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(897, 178);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(316, 329);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output";
            // 
            // cb_IncludeDiff_Columns
            // 
            this.cb_IncludeDiff_Columns.AutoSize = true;
            this.cb_IncludeDiff_Columns.Checked = true;
            this.cb_IncludeDiff_Columns.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_IncludeDiff_Columns.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IncludeDiff_Columns.Location = new System.Drawing.Point(19, 19);
            this.cb_IncludeDiff_Columns.Name = "cb_IncludeDiff_Columns";
            this.cb_IncludeDiff_Columns.Size = new System.Drawing.Size(207, 20);
            this.cb_IncludeDiff_Columns.TabIndex = 14;
            this.cb_IncludeDiff_Columns.Text = "Include Only Different Columns";
            this.cb_IncludeDiff_Columns.UseVisualStyleBackColor = true;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IgnoreCase.Location = new System.Drawing.Point(19, 45);
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.Size = new System.Drawing.Size(100, 20);
            this.cb_IgnoreCase.TabIndex = 13;
            this.cb_IgnoreCase.Text = "Ignore Case";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            // 
            // cb_IncludeRule
            // 
            this.cb_IncludeRule.AutoSize = true;
            this.cb_IncludeRule.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IncludeRule.Location = new System.Drawing.Point(19, 71);
            this.cb_IncludeRule.Name = "cb_IncludeRule";
            this.cb_IncludeRule.Size = new System.Drawing.Size(108, 20);
            this.cb_IncludeRule.TabIndex = 12;
            this.cb_IncludeRule.Text = "Include Rules";
            this.cb_IncludeRule.UseVisualStyleBackColor = true;
            this.cb_IncludeRule.CheckedChanged += new System.EventHandler(this.cb_IncludeRule_CheckedChanged);
            // 
            // btn_RuleForm
            // 
            this.btn_RuleForm.Location = new System.Drawing.Point(133, 68);
            this.btn_RuleForm.Name = "btn_RuleForm";
            this.btn_RuleForm.Size = new System.Drawing.Size(121, 23);
            this.btn_RuleForm.TabIndex = 11;
            this.btn_RuleForm.Text = "Open Rule Form";
            this.btn_RuleForm.UseVisualStyleBackColor = true;
            this.btn_RuleForm.Click += new System.EventHandler(this.btn_RuleForm_Click);
            // 
            // btn_ProcessBy_MultiThreading
            // 
            this.btn_ProcessBy_MultiThreading.Location = new System.Drawing.Point(113, 96);
            this.btn_ProcessBy_MultiThreading.Margin = new System.Windows.Forms.Padding(1);
            this.btn_ProcessBy_MultiThreading.Name = "btn_ProcessBy_MultiThreading";
            this.btn_ProcessBy_MultiThreading.Size = new System.Drawing.Size(193, 23);
            this.btn_ProcessBy_MultiThreading.TabIndex = 10;
            this.btn_ProcessBy_MultiThreading.Text = "Process By MultiThreading";
            this.btn_ProcessBy_MultiThreading.UseVisualStyleBackColor = true;
            this.btn_ProcessBy_MultiThreading.Visible = false;
            this.btn_ProcessBy_MultiThreading.Click += new System.EventHandler(this.btn_ProcessBy_MultiThreading_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(231, 125);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 9;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Validate
            // 
            this.btn_Validate.Enabled = false;
            this.btn_Validate.Location = new System.Drawing.Point(19, 96);
            this.btn_Validate.Name = "btn_Validate";
            this.btn_Validate.Size = new System.Drawing.Size(75, 23);
            this.btn_Validate.TabIndex = 6;
            this.btn_Validate.Text = "Validate";
            this.btn_Validate.UseVisualStyleBackColor = true;
            this.btn_Validate.Click += new System.EventHandler(this.btn_Validate_Click);
            // 
            // llbl_filePath
            // 
            this.llbl_filePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llbl_filePath.Location = new System.Drawing.Point(19, 182);
            this.llbl_filePath.Name = "llbl_filePath";
            this.llbl_filePath.Size = new System.Drawing.Size(287, 135);
            this.llbl_filePath.TabIndex = 3;
            this.llbl_filePath.TabStop = true;
            this.llbl_filePath.Text = "Not Set";
            this.llbl_filePath.Visible = false;
            this.llbl_filePath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.llbl_filePath.MouseHover += new System.EventHandler(this.llbl_filePath_MouseHover);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Click on link to open : ";
            this.label1.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 16);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(852, 28);
            this.progressBar1.TabIndex = 8;
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
            this.lbl_Percentage.Location = new System.Drawing.Point(313, 22);
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
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lbl_Percentage);
            this.groupBox5.Controls.Add(this.progressBar1);
            this.groupBox5.Location = new System.Drawing.Point(12, 580);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(879, 53);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Progress Status";
            // 
            // backgroundWorkerInstrument
            // 
            this.backgroundWorkerInstrument.WorkerReportsProgress = true;
            this.backgroundWorkerInstrument.WorkerSupportsCancellation = true;
            this.backgroundWorkerInstrument.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerInstrument_DoWork);
            this.backgroundWorkerInstrument.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerInstrument_ProgressChanged);
            this.backgroundWorkerInstrument.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerInstrument_RunWorkerCompleted);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.button1);
            this.groupBox7.Controls.Add(this.lbl_InstrumentText);
            this.groupBox7.Controls.Add(this.btnLoad);
            this.groupBox7.Location = new System.Drawing.Point(12, 635);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(879, 113);
            this.groupBox7.TabIndex = 15;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "For Instrument Analysis";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(278, 80);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbl_InstrumentText
            // 
            this.lbl_InstrumentText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_InstrumentText.Location = new System.Drawing.Point(16, 20);
            this.lbl_InstrumentText.Name = "lbl_InstrumentText";
            this.lbl_InstrumentText.Size = new System.Drawing.Size(852, 56);
            this.lbl_InstrumentText.TabIndex = 13;
            this.lbl_InstrumentText.Text = "lbl_InstrumentText";
            // 
            // timer1
            // 
            this.timer1.Interval = 999;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // btn_TestSystemMemoryOutException
            // 
            this.btn_TestSystemMemoryOutException.Location = new System.Drawing.Point(133, 43);
            this.btn_TestSystemMemoryOutException.Name = "btn_TestSystemMemoryOutException";
            this.btn_TestSystemMemoryOutException.Size = new System.Drawing.Size(174, 23);
            this.btn_TestSystemMemoryOutException.TabIndex = 15;
            this.btn_TestSystemMemoryOutException.Text = "Test System Memory Out Exception";
            this.btn_TestSystemMemoryOutException.UseVisualStyleBackColor = true;
            this.btn_TestSystemMemoryOutException.Visible = false;
            this.btn_TestSystemMemoryOutException.Click += new System.EventHandler(this.btn_TestSystemMemoryOutException_Click);
            // 
            // Excel_Excel_Comparison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1221, 749);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
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
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
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
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.ListBox lb_IgnoreTable1;
        private System.Windows.Forms.ListBox lb_IgnoreTable2;
        private System.Windows.Forms.LinkLabel llbl_filePath;
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cb_IncludeRule;
        private System.Windows.Forms.Button btn_RuleForm;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
        private System.Windows.Forms.CheckBox cb_IncludeDiff_Columns;
        private System.Windows.Forms.CheckBox cb_MultiReference;
        private System.ComponentModel.BackgroundWorker backgroundWorkerInstrument;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label lbl_InstrumentText;
        private System.Windows.Forms.CheckBox cb_Primary_Key;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_TestSystemMemoryOutException;
    }
}