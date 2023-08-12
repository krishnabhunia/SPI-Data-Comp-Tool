namespace SPI_Data_Comp_Tool
{
    partial class ViewForMultiReference
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewForMultiReference));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_Select_All_SRC = new System.Windows.Forms.CheckBox();
            this.lbl_Source = new System.Windows.Forms.Label();
            this.clb_Source = new System.Windows.Forms.CheckedListBox();
            this.btn_Add_Source_Col = new System.Windows.Forms.Button();
            this.btn_Add_Source_ID = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_Select_All_TRG = new System.Windows.Forms.CheckBox();
            this.lbl_Target = new System.Windows.Forms.Label();
            this.clb_Target = new System.Windows.Forms.CheckedListBox();
            this.btn_Add_Target_Col = new System.Windows.Forms.Button();
            this.btn_Add_Target_ID = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lb_Source_ID = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lb_Target_ID = new System.Windows.Forms.ListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lb_Source_Col = new System.Windows.Forms.ListBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lb_Target_Col = new System.Windows.Forms.ListBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_AutoMappingCols = new System.Windows.Forms.Button();
            this.btn_AutoMappingID = new System.Windows.Forms.Button();
            this.btn_Col_For_SRC_TRG = new System.Windows.Forms.Button();
            this.btn_ID_For_SRC_TRG = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.cb_IncludeRule = new System.Windows.Forms.CheckBox();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.btn_RuleForm = new System.Windows.Forms.Button();
            this.lblTimer = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.llbl_Folder_path = new System.Windows.Forms.LinkLabel();
            this.tb_FileName = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Set = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.btn_Compare = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.btn_ClearAll = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_Select_All_SRC);
            this.groupBox1.Controls.Add(this.lbl_Source);
            this.groupBox1.Controls.Add(this.clb_Source);
            this.groupBox1.Controls.Add(this.btn_Add_Source_Col);
            this.groupBox1.Controls.Add(this.btn_Add_Source_ID);
            this.groupBox1.Location = new System.Drawing.Point(22, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(343, 359);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // cb_Select_All_SRC
            // 
            this.cb_Select_All_SRC.AutoSize = true;
            this.cb_Select_All_SRC.Location = new System.Drawing.Point(7, 330);
            this.cb_Select_All_SRC.Name = "cb_Select_All_SRC";
            this.cb_Select_All_SRC.Size = new System.Drawing.Size(143, 17);
            this.cb_Select_All_SRC.TabIndex = 5;
            this.cb_Select_All_SRC.Text = "Select Toggle (All/None)";
            this.cb_Select_All_SRC.UseVisualStyleBackColor = true;
            this.cb_Select_All_SRC.CheckedChanged += new System.EventHandler(this.cb_Select_All_SRC_CheckedChanged);
            // 
            // lbl_Source
            // 
            this.lbl_Source.Location = new System.Drawing.Point(280, 220);
            this.lbl_Source.Name = "lbl_Source";
            this.lbl_Source.Size = new System.Drawing.Size(52, 93);
            this.lbl_Source.TabIndex = 4;
            this.lbl_Source.Text = "label3";
            // 
            // clb_Source
            // 
            this.clb_Source.CheckOnClick = true;
            this.clb_Source.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clb_Source.FormattingEnabled = true;
            this.clb_Source.HorizontalScrollbar = true;
            this.clb_Source.Location = new System.Drawing.Point(7, 20);
            this.clb_Source.Name = "clb_Source";
            this.clb_Source.Size = new System.Drawing.Size(267, 293);
            this.clb_Source.TabIndex = 0;
            // 
            // btn_Add_Source_Col
            // 
            this.btn_Add_Source_Col.Location = new System.Drawing.Point(283, 89);
            this.btn_Add_Source_Col.Name = "btn_Add_Source_Col";
            this.btn_Add_Source_Col.Size = new System.Drawing.Size(52, 68);
            this.btn_Add_Source_Col.TabIndex = 3;
            this.btn_Add_Source_Col.Text = "Add as Source Column";
            this.btn_Add_Source_Col.UseVisualStyleBackColor = true;
            this.btn_Add_Source_Col.Click += new System.EventHandler(this.btn_Add_Source_Col_Click);
            // 
            // btn_Add_Source_ID
            // 
            this.btn_Add_Source_ID.Location = new System.Drawing.Point(280, 20);
            this.btn_Add_Source_ID.Name = "btn_Add_Source_ID";
            this.btn_Add_Source_ID.Size = new System.Drawing.Size(52, 60);
            this.btn_Add_Source_ID.TabIndex = 2;
            this.btn_Add_Source_ID.Text = "Add as Source ID";
            this.btn_Add_Source_ID.UseVisualStyleBackColor = true;
            this.btn_Add_Source_ID.Click += new System.EventHandler(this.btn_Add_Source_ID_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_Select_All_TRG);
            this.groupBox2.Controls.Add(this.lbl_Target);
            this.groupBox2.Controls.Add(this.clb_Target);
            this.groupBox2.Controls.Add(this.btn_Add_Target_Col);
            this.groupBox2.Controls.Add(this.btn_Add_Target_ID);
            this.groupBox2.Location = new System.Drawing.Point(380, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 359);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target";
            // 
            // cb_Select_All_TRG
            // 
            this.cb_Select_All_TRG.AutoSize = true;
            this.cb_Select_All_TRG.Location = new System.Drawing.Point(7, 328);
            this.cb_Select_All_TRG.Name = "cb_Select_All_TRG";
            this.cb_Select_All_TRG.Size = new System.Drawing.Size(143, 17);
            this.cb_Select_All_TRG.TabIndex = 7;
            this.cb_Select_All_TRG.Text = "Select Toggle (All/None)";
            this.cb_Select_All_TRG.UseVisualStyleBackColor = true;
            this.cb_Select_All_TRG.CheckedChanged += new System.EventHandler(this.cb_Select_All_TRG_CheckedChanged);
            // 
            // lbl_Target
            // 
            this.lbl_Target.Location = new System.Drawing.Point(281, 220);
            this.lbl_Target.Name = "lbl_Target";
            this.lbl_Target.Size = new System.Drawing.Size(51, 92);
            this.lbl_Target.TabIndex = 6;
            this.lbl_Target.Text = "label4";
            // 
            // clb_Target
            // 
            this.clb_Target.CheckOnClick = true;
            this.clb_Target.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clb_Target.FormattingEnabled = true;
            this.clb_Target.HorizontalScrollbar = true;
            this.clb_Target.Location = new System.Drawing.Point(7, 20);
            this.clb_Target.Name = "clb_Target";
            this.clb_Target.Size = new System.Drawing.Size(267, 293);
            this.clb_Target.TabIndex = 0;
            // 
            // btn_Add_Target_Col
            // 
            this.btn_Add_Target_Col.Location = new System.Drawing.Point(281, 89);
            this.btn_Add_Target_Col.Name = "btn_Add_Target_Col";
            this.btn_Add_Target_Col.Size = new System.Drawing.Size(51, 68);
            this.btn_Add_Target_Col.TabIndex = 5;
            this.btn_Add_Target_Col.Text = "Add as Target Column";
            this.btn_Add_Target_Col.UseVisualStyleBackColor = true;
            this.btn_Add_Target_Col.Click += new System.EventHandler(this.btn_Add_Target_Col_Click);
            // 
            // btn_Add_Target_ID
            // 
            this.btn_Add_Target_ID.Location = new System.Drawing.Point(281, 20);
            this.btn_Add_Target_ID.Name = "btn_Add_Target_ID";
            this.btn_Add_Target_ID.Size = new System.Drawing.Size(51, 60);
            this.btn_Add_Target_ID.TabIndex = 4;
            this.btn_Add_Target_ID.Text = "Add as Target ID";
            this.btn_Add_Target_ID.UseVisualStyleBackColor = true;
            this.btn_Add_Target_ID.Click += new System.EventHandler(this.btn_Add_Target_ID_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lb_Source_ID);
            this.groupBox3.Location = new System.Drawing.Point(22, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(251, 362);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Source ID";
            // 
            // lb_Source_ID
            // 
            this.lb_Source_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Source_ID.FormattingEnabled = true;
            this.lb_Source_ID.HorizontalScrollbar = true;
            this.lb_Source_ID.ItemHeight = 16;
            this.lb_Source_ID.Location = new System.Drawing.Point(12, 19);
            this.lb_Source_ID.Name = "lb_Source_ID";
            this.lb_Source_ID.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_Source_ID.Size = new System.Drawing.Size(233, 324);
            this.lb_Source_ID.TabIndex = 8;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lb_Target_ID);
            this.groupBox4.Location = new System.Drawing.Point(279, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(251, 362);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Target ID";
            // 
            // lb_Target_ID
            // 
            this.lb_Target_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Target_ID.FormattingEnabled = true;
            this.lb_Target_ID.HorizontalScrollbar = true;
            this.lb_Target_ID.ItemHeight = 16;
            this.lb_Target_ID.Location = new System.Drawing.Point(13, 19);
            this.lb_Target_ID.Name = "lb_Target_ID";
            this.lb_Target_ID.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_Target_ID.Size = new System.Drawing.Size(233, 324);
            this.lb_Target_ID.TabIndex = 9;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lb_Source_Col);
            this.groupBox5.Location = new System.Drawing.Point(540, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(254, 362);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Source Column";
            // 
            // lb_Source_Col
            // 
            this.lb_Source_Col.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Source_Col.FormattingEnabled = true;
            this.lb_Source_Col.HorizontalScrollbar = true;
            this.lb_Source_Col.ItemHeight = 16;
            this.lb_Source_Col.Location = new System.Drawing.Point(12, 19);
            this.lb_Source_Col.Name = "lb_Source_Col";
            this.lb_Source_Col.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_Source_Col.Size = new System.Drawing.Size(233, 324);
            this.lb_Source_Col.TabIndex = 10;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lb_Target_Col);
            this.groupBox6.Location = new System.Drawing.Point(801, 19);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(248, 362);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Target Column";
            // 
            // lb_Target_Col
            // 
            this.lb_Target_Col.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Target_Col.FormattingEnabled = true;
            this.lb_Target_Col.HorizontalScrollbar = true;
            this.lb_Target_Col.ItemHeight = 16;
            this.lb_Target_Col.Location = new System.Drawing.Point(6, 19);
            this.lb_Target_Col.Name = "lb_Target_Col";
            this.lb_Target_Col.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_Target_Col.Size = new System.Drawing.Size(233, 324);
            this.lb_Target_Col.TabIndex = 11;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label6);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.btn_AutoMappingCols);
            this.groupBox7.Controls.Add(this.btn_AutoMappingID);
            this.groupBox7.Controls.Add(this.btn_Col_For_SRC_TRG);
            this.groupBox7.Controls.Add(this.btn_ID_For_SRC_TRG);
            this.groupBox7.Controls.Add(this.groupBox9);
            this.groupBox7.Controls.Add(this.groupBox1);
            this.groupBox7.Controls.Add(this.groupBox2);
            this.groupBox7.Location = new System.Drawing.Point(12, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(1169, 461);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Selection Panel";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(998, 418);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "label6";
            this.label6.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(998, 394);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "label5";
            this.label5.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(742, 413);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "label4";
            this.label4.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(742, 393);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "label3";
            this.label3.Visible = false;
            // 
            // btn_AutoMappingCols
            // 
            this.btn_AutoMappingCols.Location = new System.Drawing.Point(380, 413);
            this.btn_AutoMappingCols.Name = "btn_AutoMappingCols";
            this.btn_AutoMappingCols.Size = new System.Drawing.Size(235, 23);
            this.btn_AutoMappingCols.TabIndex = 13;
            this.btn_AutoMappingCols.Text = "Auto Mapping Compare Cols";
            this.btn_AutoMappingCols.UseVisualStyleBackColor = true;
            this.btn_AutoMappingCols.Click += new System.EventHandler(this.btn_AutoMappingCols_Click);
            // 
            // btn_AutoMappingID
            // 
            this.btn_AutoMappingID.Location = new System.Drawing.Point(152, 414);
            this.btn_AutoMappingID.Name = "btn_AutoMappingID";
            this.btn_AutoMappingID.Size = new System.Drawing.Size(213, 23);
            this.btn_AutoMappingID.TabIndex = 12;
            this.btn_AutoMappingID.Text = "Auto Mapping ID";
            this.btn_AutoMappingID.UseVisualStyleBackColor = true;
            this.btn_AutoMappingID.Click += new System.EventHandler(this.btn_AutoMappingID_Click);
            // 
            // btn_Col_For_SRC_TRG
            // 
            this.btn_Col_For_SRC_TRG.Location = new System.Drawing.Point(380, 384);
            this.btn_Col_For_SRC_TRG.Name = "btn_Col_For_SRC_TRG";
            this.btn_Col_For_SRC_TRG.Size = new System.Drawing.Size(235, 23);
            this.btn_Col_For_SRC_TRG.TabIndex = 11;
            this.btn_Col_For_SRC_TRG.Text = "Add as Column For Source and Target";
            this.btn_Col_For_SRC_TRG.UseVisualStyleBackColor = true;
            this.btn_Col_For_SRC_TRG.Click += new System.EventHandler(this.btn_Col_For_SRC_TRG_Click);
            // 
            // btn_ID_For_SRC_TRG
            // 
            this.btn_ID_For_SRC_TRG.Location = new System.Drawing.Point(152, 384);
            this.btn_ID_For_SRC_TRG.Name = "btn_ID_For_SRC_TRG";
            this.btn_ID_For_SRC_TRG.Size = new System.Drawing.Size(213, 23);
            this.btn_ID_For_SRC_TRG.TabIndex = 10;
            this.btn_ID_For_SRC_TRG.Text = "Add as ID in Source and Target";
            this.btn_ID_For_SRC_TRG.UseVisualStyleBackColor = true;
            this.btn_ID_For_SRC_TRG.Click += new System.EventHandler(this.btn_ID_For_SRC_TRG_Click);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.cb_IncludeRule);
            this.groupBox9.Controls.Add(this.cb_IgnoreCase);
            this.groupBox9.Controls.Add(this.btn_RuleForm);
            this.groupBox9.Controls.Add(this.lblTimer);
            this.groupBox9.Controls.Add(this.label2);
            this.groupBox9.Controls.Add(this.label14);
            this.groupBox9.Controls.Add(this.label13);
            this.groupBox9.Controls.Add(this.llbl_Folder_path);
            this.groupBox9.Controls.Add(this.tb_FileName);
            this.groupBox9.Controls.Add(this.progressBar1);
            this.groupBox9.Controls.Add(this.label1);
            this.groupBox9.Controls.Add(this.btn_Cancel);
            this.groupBox9.Controls.Add(this.btn_Set);
            this.groupBox9.Controls.Add(this.lbl_Status);
            this.groupBox9.Controls.Add(this.btn_Compare);
            this.groupBox9.Location = new System.Drawing.Point(742, 19);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(421, 359);
            this.groupBox9.TabIndex = 9;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Control And Status";
            // 
            // cb_IncludeRule
            // 
            this.cb_IncludeRule.AutoSize = true;
            this.cb_IncludeRule.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IncludeRule.Location = new System.Drawing.Point(9, 165);
            this.cb_IncludeRule.Name = "cb_IncludeRule";
            this.cb_IncludeRule.Size = new System.Drawing.Size(108, 20);
            this.cb_IncludeRule.TabIndex = 14;
            this.cb_IncludeRule.Text = "Include Rules";
            this.cb_IncludeRule.UseVisualStyleBackColor = true;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IgnoreCase.Location = new System.Drawing.Point(9, 142);
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.Size = new System.Drawing.Size(100, 20);
            this.cb_IgnoreCase.TabIndex = 25;
            this.cb_IgnoreCase.Text = "Ignore Case";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            // 
            // btn_RuleForm
            // 
            this.btn_RuleForm.Location = new System.Drawing.Point(122, 164);
            this.btn_RuleForm.Name = "btn_RuleForm";
            this.btn_RuleForm.Size = new System.Drawing.Size(121, 23);
            this.btn_RuleForm.TabIndex = 13;
            this.btn_RuleForm.Text = "Open Rule Form";
            this.btn_RuleForm.UseVisualStyleBackColor = true;
            this.btn_RuleForm.Click += new System.EventHandler(this.btn_RuleForm_Click);
            // 
            // lblTimer
            // 
            this.lblTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.Location = new System.Drawing.Point(122, 303);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(293, 44);
            this.lblTimer.TabIndex = 23;
            this.lblTimer.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 303);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 16);
            this.label2.TabIndex = 24;
            this.label2.Text = "Time Elapsed : ";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 89);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 13);
            this.label14.TabIndex = 22;
            this.label14.Text = "Filename : ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(6, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 16);
            this.label13.TabIndex = 20;
            this.label13.Text = "Path : ";
            // 
            // llbl_Folder_path
            // 
            this.llbl_Folder_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llbl_Folder_path.Location = new System.Drawing.Point(58, 16);
            this.llbl_Folder_path.Name = "llbl_Folder_path";
            this.llbl_Folder_path.Size = new System.Drawing.Size(357, 64);
            this.llbl_Folder_path.TabIndex = 19;
            this.llbl_Folder_path.TabStop = true;
            this.llbl_Folder_path.Text = "D:\\";
            this.llbl_Folder_path.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_Folder_path_LinkClicked);
            // 
            // tb_FileName
            // 
            this.tb_FileName.Location = new System.Drawing.Point(70, 86);
            this.tb_FileName.Name = "tb_FileName";
            this.tb_FileName.Size = new System.Drawing.Size(345, 20);
            this.tb_FileName.TabIndex = 21;
            this.tb_FileName.Text = "_output";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(5, 225);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(410, 20);
            this.progressBar1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 256);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Status : ";
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(168, 191);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 8;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Set
            // 
            this.btn_Set.Location = new System.Drawing.Point(5, 113);
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.Size = new System.Drawing.Size(155, 23);
            this.btn_Set.TabIndex = 4;
            this.btn_Set.Text = "Set and Validate";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(70, 256);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(345, 38);
            this.lbl_Status.TabIndex = 7;
            this.lbl_Status.Text = "label2";
            // 
            // btn_Compare
            // 
            this.btn_Compare.Location = new System.Drawing.Point(5, 191);
            this.btn_Compare.Name = "btn_Compare";
            this.btn_Compare.Size = new System.Drawing.Size(155, 23);
            this.btn_Compare.TabIndex = 5;
            this.btn_Compare.Text = "Compare and Generate Excel";
            this.btn_Compare.UseVisualStyleBackColor = true;
            this.btn_Compare.Click += new System.EventHandler(this.btn_Compare_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.groupBox10);
            this.groupBox8.Controls.Add(this.groupBox3);
            this.groupBox8.Controls.Add(this.groupBox4);
            this.groupBox8.Controls.Add(this.groupBox6);
            this.groupBox8.Controls.Add(this.groupBox5);
            this.groupBox8.Location = new System.Drawing.Point(12, 479);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(1169, 387);
            this.groupBox8.TabIndex = 7;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "View Panel";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.btn_ClearAll);
            this.groupBox10.Controls.Add(this.dataGridView1);
            this.groupBox10.Controls.Add(this.btn_Remove);
            this.groupBox10.Location = new System.Drawing.Point(1058, 19);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(105, 362);
            this.groupBox10.TabIndex = 6;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Controls";
            // 
            // btn_ClearAll
            // 
            this.btn_ClearAll.Location = new System.Drawing.Point(6, 111);
            this.btn_ClearAll.Name = "btn_ClearAll";
            this.btn_ClearAll.Size = new System.Drawing.Size(93, 68);
            this.btn_ClearAll.TabIndex = 9;
            this.btn_ClearAll.Text = "Clear All";
            this.btn_ClearAll.UseVisualStyleBackColor = true;
            this.btn_ClearAll.Click += new System.EventHandler(this.btn_ClearAll_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 310);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(34, 33);
            this.dataGridView1.TabIndex = 8;
            this.dataGridView1.Visible = false;
            // 
            // btn_Remove
            // 
            this.btn_Remove.Location = new System.Drawing.Point(6, 19);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(93, 63);
            this.btn_Remove.TabIndex = 0;
            this.btn_Remove.Text = "Remove Selection";
            this.btn_Remove.UseVisualStyleBackColor = true;
            this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // ViewForMultiReference
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1193, 878);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ViewForMultiReference";
            this.Text = "ViewForMultiReference";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckedListBox clb_Source;
        private System.Windows.Forms.CheckedListBox clb_Target;
        private System.Windows.Forms.Button btn_Add_Source_Col;
        private System.Windows.Forms.Button btn_Add_Source_ID;
        private System.Windows.Forms.Button btn_Add_Target_ID;
        private System.Windows.Forms.Button btn_Add_Target_Col;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Set;
        private System.Windows.Forms.Button btn_Compare;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ListBox lb_Source_ID;
        private System.Windows.Forms.ListBox lb_Target_ID;
        private System.Windows.Forms.ListBox lb_Source_Col;
        private System.Windows.Forms.ListBox lb_Target_Col;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button btn_Remove;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.LinkLabel llbl_Folder_path;
        private System.Windows.Forms.TextBox tb_FileName;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_Source;
        private System.Windows.Forms.Label lbl_Target;
        private System.Windows.Forms.Button btn_ClearAll;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
        private System.Windows.Forms.CheckBox cb_IncludeRule;
        private System.Windows.Forms.Button btn_RuleForm;
        private System.Windows.Forms.CheckBox cb_Select_All_SRC;
        private System.Windows.Forms.CheckBox cb_Select_All_TRG;
        private System.Windows.Forms.Button btn_ID_For_SRC_TRG;
        private System.Windows.Forms.Button btn_Col_For_SRC_TRG;
        private System.Windows.Forms.Button btn_AutoMappingCols;
        private System.Windows.Forms.Button btn_AutoMappingID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}