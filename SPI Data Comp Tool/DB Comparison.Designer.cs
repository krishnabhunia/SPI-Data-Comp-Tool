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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DB_Comparison_Form));
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
            this.label15 = new System.Windows.Forms.Label();
            this.cb_UserProfileList = new System.Windows.Forms.ComboBox();
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.lbl_ProfileStatus = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tb_ProfileName = new System.Windows.Forms.TextBox();
            this.btn_SaveProfile = new System.Windows.Forms.Button();
            this.btn_LoadProfile = new System.Windows.Forms.Button();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.gb_Target1 = new System.Windows.Forms.GroupBox();
            this.cb_TRG_TableName = new System.Windows.Forms.ComboBox();
            this.btn_Load_TRG_Table = new System.Windows.Forms.Button();
            this.tb_TRG_DefaultPort = new System.Windows.Forms.TextBox();
            this.cb_TRG_DefaultPort = new System.Windows.Forms.CheckBox();
            this.cb_WindowAuthentication_TRG = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tb_TRG_Password = new System.Windows.Forms.TextBox();
            this.tb_TRG_UserName = new System.Windows.Forms.TextBox();
            this.tb_TRG_DBName = new System.Windows.Forms.TextBox();
            this.tb_TRG_DataSource = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_TRG_SRC = new System.Windows.Forms.Button();
            this.gb_Target = new System.Windows.Forms.GroupBox();
            this.rb_TRG_Oracle = new System.Windows.Forms.RadioButton();
            this.rb_TRG_SQL = new System.Windows.Forms.RadioButton();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.gb_Source1 = new System.Windows.Forms.GroupBox();
            this.cb_WindowAuthentication_SRC = new System.Windows.Forms.CheckBox();
            this.btn_Load_SRC_Table = new System.Windows.Forms.Button();
            this.cb_SRC_TableName = new System.Windows.Forms.ComboBox();
            this.tb_SRC_DefaultPort = new System.Windows.Forms.TextBox();
            this.cb_SRC_DefaultPort = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tb_SRC_Password = new System.Windows.Forms.TextBox();
            this.tb_SRC_UserName = new System.Windows.Forms.TextBox();
            this.tb_SRC_DBName = new System.Windows.Forms.TextBox();
            this.tb_SRC_DataSource = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gb_Source = new System.Windows.Forms.GroupBox();
            this.rb_SRC_oracle = new System.Windows.Forms.RadioButton();
            this.rb_SRC_SQL = new System.Windows.Forms.RadioButton();
            this.btn_Copy_SRC_TRG = new System.Windows.Forms.Button();
            this.cb_FullName = new System.Windows.Forms.CheckBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox3.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.gb_Target1.SuspendLayout();
            this.gb_Target.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.gb_Source1.SuspendLayout();
            this.gb_Source.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerateOutput
            // 
            this.btnGenerateOutput.Enabled = false;
            this.btnGenerateOutput.Location = new System.Drawing.Point(17, 150);
            this.btnGenerateOutput.Name = "btnGenerateOutput";
            this.btnGenerateOutput.Size = new System.Drawing.Size(172, 23);
            this.btnGenerateOutput.TabIndex = 0;
            this.btnGenerateOutput.Text = "Compare and Generate Excel";
            this.btnGenerateOutput.UseVisualStyleBackColor = true;
            this.btnGenerateOutput.Click += new System.EventHandler(this.btnGenerateOutput_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(18, 18);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(188, 94);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Okay";
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
            // btn_Connect_Reference_ID
            // 
            this.btn_Connect_Reference_ID.Location = new System.Drawing.Point(15, 39);
            this.btn_Connect_Reference_ID.Name = "btn_Connect_Reference_ID";
            this.btn_Connect_Reference_ID.Size = new System.Drawing.Size(141, 23);
            this.btn_Connect_Reference_ID.TabIndex = 10;
            this.btn_Connect_Reference_ID.Text = "Connect and Load";
            this.btn_Connect_Reference_ID.UseVisualStyleBackColor = true;
            this.btn_Connect_Reference_ID.Click += new System.EventHandler(this.btn_Connect_Reference_ID_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox11);
            this.groupBox3.Controls.Add(this.cb_ColumnMapping);
            this.groupBox3.Controls.Add(this.cb_IncludeRule);
            this.groupBox3.Controls.Add(this.cb_IgnoreCase);
            this.groupBox3.Controls.Add(this.btn_RuleForm);
            this.groupBox3.Controls.Add(this.btn_Cancel);
            this.groupBox3.Controls.Add(this.btnGenerateOutput);
            this.groupBox3.Controls.Add(this.btn_Clear_Reset);
            this.groupBox3.Controls.Add(this.btn_Set);
            this.groupBox3.Controls.Add(this.llbl_File_path);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.btn_Connect_Reference_ID);
            this.groupBox3.Location = new System.Drawing.Point(817, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(404, 392);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Controls and Output";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.lbl_PercentageShow);
            this.groupBox11.Controls.Add(this.progressBar1);
            this.groupBox11.Controls.Add(this.lblTimer);
            this.groupBox11.Controls.Add(this.label1);
            this.groupBox11.Location = new System.Drawing.Point(6, 256);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(384, 119);
            this.groupBox11.TabIndex = 27;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Progress";
            // 
            // lbl_PercentageShow
            // 
            this.lbl_PercentageShow.AutoSize = true;
            this.lbl_PercentageShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PercentageShow.Location = new System.Drawing.Point(11, 34);
            this.lbl_PercentageShow.Name = "lbl_PercentageShow";
            this.lbl_PercentageShow.Size = new System.Drawing.Size(102, 16);
            this.lbl_PercentageShow.TabIndex = 22;
            this.lbl_PercentageShow.Text = "Progress Bar %";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 15);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(366, 15);
            this.progressBar1.TabIndex = 21;
            // 
            // lblTimer
            // 
            this.lblTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.Location = new System.Drawing.Point(11, 74);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(364, 34);
            this.lblTimer.TabIndex = 19;
            this.lblTimer.Text = "lblTimer";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 16);
            this.label1.TabIndex = 21;
            this.label1.Text = "Time Elapsed : ";
            // 
            // cb_ColumnMapping
            // 
            this.cb_ColumnMapping.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ColumnMapping.Location = new System.Drawing.Point(15, 17);
            this.cb_ColumnMapping.Name = "cb_ColumnMapping";
            this.cb_ColumnMapping.Size = new System.Drawing.Size(375, 23);
            this.cb_ColumnMapping.TabIndex = 33;
            this.cb_ColumnMapping.Text = "Select For Different Column Name and RefID comparison";
            this.cb_ColumnMapping.UseVisualStyleBackColor = true;
            // 
            // cb_IncludeRule
            // 
            this.cb_IncludeRule.AutoSize = true;
            this.cb_IncludeRule.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IncludeRule.Location = new System.Drawing.Point(17, 95);
            this.cb_IncludeRule.Name = "cb_IncludeRule";
            this.cb_IncludeRule.Size = new System.Drawing.Size(108, 20);
            this.cb_IncludeRule.TabIndex = 25;
            this.cb_IncludeRule.Text = "Include Rules";
            this.cb_IncludeRule.UseVisualStyleBackColor = true;
            this.cb_IncludeRule.CheckedChanged += new System.EventHandler(this.cb_IncludeRule_CheckedChanged);
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IgnoreCase.Location = new System.Drawing.Point(17, 123);
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.Size = new System.Drawing.Size(100, 20);
            this.cb_IgnoreCase.TabIndex = 23;
            this.cb_IgnoreCase.Text = "Ignore Case";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            this.cb_IgnoreCase.CheckedChanged += new System.EventHandler(this.cb_IgnoreCase_CheckedChanged);
            // 
            // btn_RuleForm
            // 
            this.btn_RuleForm.Location = new System.Drawing.Point(125, 93);
            this.btn_RuleForm.Name = "btn_RuleForm";
            this.btn_RuleForm.Size = new System.Drawing.Size(125, 23);
            this.btn_RuleForm.TabIndex = 24;
            this.btn_RuleForm.Text = "Open Rule Form";
            this.btn_RuleForm.UseVisualStyleBackColor = true;
            this.btn_RuleForm.Click += new System.EventHandler(this.btn_RuleForm_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Enabled = false;
            this.btn_Cancel.Location = new System.Drawing.Point(195, 150);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(94, 24);
            this.btn_Cancel.TabIndex = 20;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Location = new System.Drawing.Point(166, 39);
            this.btn_Clear_Reset.Name = "btn_Clear_Reset";
            this.btn_Clear_Reset.Size = new System.Drawing.Size(84, 23);
            this.btn_Clear_Reset.TabIndex = 14;
            this.btn_Clear_Reset.Text = "Clear/Reset";
            this.btn_Clear_Reset.UseVisualStyleBackColor = true;
            this.btn_Clear_Reset.Click += new System.EventHandler(this.btn_Clear_Reset_Click);
            // 
            // btn_Set
            // 
            this.btn_Set.Enabled = false;
            this.btn_Set.Location = new System.Drawing.Point(15, 66);
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.Size = new System.Drawing.Size(112, 23);
            this.btn_Set.TabIndex = 13;
            this.btn_Set.Text = "Set And Validate";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            // 
            // llbl_File_path
            // 
            this.llbl_File_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llbl_File_path.Location = new System.Drawing.Point(61, 180);
            this.llbl_File_path.Name = "llbl_File_path";
            this.llbl_File_path.Size = new System.Drawing.Size(334, 76);
            this.llbl_File_path.TabIndex = 15;
            this.llbl_File_path.TabStop = true;
            this.llbl_File_path.Text = "linkLabel";
            this.llbl_File_path.Visible = false;
            this.llbl_File_path.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_File_path_LinkClicked);
            this.llbl_File_path.Click += new System.EventHandler(this.llbl_File_path_Click);
            this.llbl_File_path.MouseHover += new System.EventHandler(this.llbl_Folder_path_MouseHover);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(17, 181);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(38, 13);
            this.label13.TabIndex = 16;
            this.label13.Text = "Path : ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(284, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 13);
            this.label15.TabIndex = 34;
            this.label15.Text = "User Profile : ";
            this.label15.Click += new System.EventHandler(this.label15_Click);
            // 
            // cb_UserProfileList
            // 
            this.cb_UserProfileList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_UserProfileList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_UserProfileList.FormattingEnabled = true;
            this.cb_UserProfileList.ItemHeight = 13;
            this.cb_UserProfileList.Location = new System.Drawing.Point(360, 19);
            this.cb_UserProfileList.Name = "cb_UserProfileList";
            this.cb_UserProfileList.Size = new System.Drawing.Size(299, 21);
            this.cb_UserProfileList.TabIndex = 20;
            this.cb_UserProfileList.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
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
            // lblCal
            // 
            this.lblCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCal.Location = new System.Drawing.Point(17, 281);
            this.lblCal.Name = "lblCal";
            this.lblCal.Size = new System.Drawing.Size(188, 92);
            this.lblCal.TabIndex = 22;
            this.lblCal.Text = "Total Rows : Columns :";
            // 
            // lblExpressionStatus
            // 
            this.lblExpressionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExpressionStatus.Location = new System.Drawing.Point(18, 123);
            this.lblExpressionStatus.Name = "lblExpressionStatus";
            this.lblExpressionStatus.Size = new System.Drawing.Size(188, 146);
            this.lblExpressionStatus.TabIndex = 12;
            this.lblExpressionStatus.Text = "Status";
            // 
            // cbL_SRC_Cols
            // 
            this.cbL_SRC_Cols.CheckOnClick = true;
            this.cbL_SRC_Cols.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbL_SRC_Cols.FormattingEnabled = true;
            this.cbL_SRC_Cols.HorizontalScrollbar = true;
            this.cbL_SRC_Cols.Location = new System.Drawing.Point(14, 48);
            this.cbL_SRC_Cols.Name = "cbL_SRC_Cols";
            this.cbL_SRC_Cols.Size = new System.Drawing.Size(165, 344);
            this.cbL_SRC_Cols.TabIndex = 13;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cb_selectAll_toggle);
            this.groupBox5.Controls.Add(this.cbL_SRC_Cols);
            this.groupBox5.Location = new System.Drawing.Point(200, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(192, 409);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Source - Uncheck to Ignore";
            // 
            // cb_selectAll_toggle
            // 
            this.cb_selectAll_toggle.AutoSize = true;
            this.cb_selectAll_toggle.Location = new System.Drawing.Point(14, 25);
            this.cb_selectAll_toggle.Name = "cb_selectAll_toggle";
            this.cb_selectAll_toggle.Size = new System.Drawing.Size(143, 17);
            this.cb_selectAll_toggle.TabIndex = 14;
            this.cb_selectAll_toggle.Text = "Select All/None (Toggle)";
            this.cb_selectAll_toggle.UseVisualStyleBackColor = true;
            this.cb_selectAll_toggle.CheckedChanged += new System.EventHandler(this.cb_selectAll_toggle_CheckedChanged);
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
            this.dataGridView1.Location = new System.Drawing.Point(6, 22);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Size = new System.Drawing.Size(39, 29);
            this.dataGridView1.TabIndex = 15;
            this.dataGridView1.Visible = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lb_TRG_Cols);
            this.groupBox6.Location = new System.Drawing.Point(398, 21);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(191, 407);
            this.groupBox6.TabIndex = 19;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Target SQL Columns";
            // 
            // lb_TRG_Cols
            // 
            this.lb_TRG_Cols.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_TRG_Cols.FormattingEnabled = true;
            this.lb_TRG_Cols.HorizontalScrollbar = true;
            this.lb_TRG_Cols.ItemHeight = 16;
            this.lb_TRG_Cols.Location = new System.Drawing.Point(12, 46);
            this.lb_TRG_Cols.Name = "lb_TRG_Cols";
            this.lb_TRG_Cols.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_TRG_Cols.Size = new System.Drawing.Size(165, 340);
            this.lb_TRG_Cols.TabIndex = 1;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.cb_PrimaryKey);
            this.groupBox7.Controls.Add(this.clb_ReferenceID);
            this.groupBox7.Location = new System.Drawing.Point(8, 19);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(187, 409);
            this.groupBox7.TabIndex = 23;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Select Reference Key";
            // 
            // cb_PrimaryKey
            // 
            this.cb_PrimaryKey.AutoSize = true;
            this.cb_PrimaryKey.Checked = true;
            this.cb_PrimaryKey.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_PrimaryKey.Location = new System.Drawing.Point(12, 25);
            this.cb_PrimaryKey.Name = "cb_PrimaryKey";
            this.cb_PrimaryKey.Size = new System.Drawing.Size(114, 17);
            this.cb_PrimaryKey.TabIndex = 34;
            this.cb_PrimaryKey.Text = "Select Primary Key";
            this.cb_PrimaryKey.UseVisualStyleBackColor = true;
            // 
            // clb_ReferenceID
            // 
            this.clb_ReferenceID.CheckOnClick = true;
            this.clb_ReferenceID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clb_ReferenceID.FormattingEnabled = true;
            this.clb_ReferenceID.HorizontalScrollbar = true;
            this.clb_ReferenceID.Location = new System.Drawing.Point(11, 47);
            this.clb_ReferenceID.Name = "clb_ReferenceID";
            this.clb_ReferenceID.Size = new System.Drawing.Size(165, 344);
            this.clb_ReferenceID.TabIndex = 0;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.groupBox2);
            this.groupBox9.Controls.Add(this.groupBox1);
            this.groupBox9.Controls.Add(this.groupBox10);
            this.groupBox9.Controls.Add(this.groupBox6);
            this.groupBox9.Controls.Add(this.groupBox7);
            this.groupBox9.Controls.Add(this.groupBox5);
            this.groupBox9.Location = new System.Drawing.Point(12, 410);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(1209, 437);
            this.groupBox9.TabIndex = 25;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "View Data";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lb_IgnoredCol_TRG);
            this.groupBox2.Location = new System.Drawing.Point(788, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(186, 405);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ignored Columns In Target";
            // 
            // lb_IgnoredCol_TRG
            // 
            this.lb_IgnoredCol_TRG.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_IgnoredCol_TRG.FormattingEnabled = true;
            this.lb_IgnoredCol_TRG.HorizontalScrollbar = true;
            this.lb_IgnoredCol_TRG.ItemHeight = 16;
            this.lb_IgnoredCol_TRG.Location = new System.Drawing.Point(11, 47);
            this.lb_IgnoredCol_TRG.Name = "lb_IgnoredCol_TRG";
            this.lb_IgnoredCol_TRG.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoredCol_TRG.Size = new System.Drawing.Size(165, 340);
            this.lb_IgnoredCol_TRG.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_IgnoredCol_SRC);
            this.groupBox1.Location = new System.Drawing.Point(595, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 407);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ignored Columns In Source";
            // 
            // lb_IgnoredCol_SRC
            // 
            this.lb_IgnoredCol_SRC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_IgnoredCol_SRC.FormattingEnabled = true;
            this.lb_IgnoredCol_SRC.HorizontalScrollbar = true;
            this.lb_IgnoredCol_SRC.ItemHeight = 16;
            this.lb_IgnoredCol_SRC.Location = new System.Drawing.Point(11, 45);
            this.lb_IgnoredCol_SRC.Name = "lb_IgnoredCol_SRC";
            this.lb_IgnoredCol_SRC.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoredCol_SRC.Size = new System.Drawing.Size(165, 340);
            this.lb_IgnoredCol_SRC.TabIndex = 0;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.lblStatus);
            this.groupBox10.Controls.Add(this.lblCal);
            this.groupBox10.Controls.Add(this.lblExpressionStatus);
            this.groupBox10.Location = new System.Drawing.Point(979, 19);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(221, 389);
            this.groupBox10.TabIndex = 26;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Status";
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
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(15, 126);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(219, 737);
            this.treeView1.TabIndex = 31;
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
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.lbl_ProfileStatus);
            this.groupBox16.Controls.Add(this.label10);
            this.groupBox16.Controls.Add(this.tb_ProfileName);
            this.groupBox16.Controls.Add(this.btn_SaveProfile);
            this.groupBox16.Controls.Add(this.btn_LoadProfile);
            this.groupBox16.Controls.Add(this.treeView1);
            this.groupBox16.Location = new System.Drawing.Point(1361, 12);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(249, 871);
            this.groupBox16.TabIndex = 32;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Profile";
            this.groupBox16.Visible = false;
            // 
            // lbl_ProfileStatus
            // 
            this.lbl_ProfileStatus.AutoSize = true;
            this.lbl_ProfileStatus.Location = new System.Drawing.Point(15, 98);
            this.lbl_ProfileStatus.Name = "lbl_ProfileStatus";
            this.lbl_ProfileStatus.Size = new System.Drawing.Size(69, 13);
            this.lbl_ProfileStatus.TabIndex = 36;
            this.lbl_ProfileStatus.Text = "Profile Status";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(139, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Enter Profile Name To Save";
            // 
            // tb_ProfileName
            // 
            this.tb_ProfileName.Location = new System.Drawing.Point(16, 41);
            this.tb_ProfileName.Name = "tb_ProfileName";
            this.tb_ProfileName.Size = new System.Drawing.Size(218, 20);
            this.tb_ProfileName.TabIndex = 34;
            // 
            // btn_SaveProfile
            // 
            this.btn_SaveProfile.Location = new System.Drawing.Point(16, 64);
            this.btn_SaveProfile.Name = "btn_SaveProfile";
            this.btn_SaveProfile.Size = new System.Drawing.Size(75, 23);
            this.btn_SaveProfile.TabIndex = 33;
            this.btn_SaveProfile.Text = "Save Profile";
            this.btn_SaveProfile.UseVisualStyleBackColor = true;
            this.btn_SaveProfile.Click += new System.EventHandler(this.btn_SaveProfile_Click);
            // 
            // btn_LoadProfile
            // 
            this.btn_LoadProfile.Location = new System.Drawing.Point(159, 67);
            this.btn_LoadProfile.Name = "btn_LoadProfile";
            this.btn_LoadProfile.Size = new System.Drawing.Size(75, 23);
            this.btn_LoadProfile.TabIndex = 32;
            this.btn_LoadProfile.Text = "Load Profile";
            this.btn_LoadProfile.UseVisualStyleBackColor = true;
            this.btn_LoadProfile.Click += new System.EventHandler(this.btn_LoadProfile_Click);
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.gb_Target1);
            this.groupBox13.Controls.Add(this.btn_TRG_SRC);
            this.groupBox13.Controls.Add(this.gb_Target);
            this.groupBox13.Location = new System.Drawing.Point(404, 47);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(379, 317);
            this.groupBox13.TabIndex = 29;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Target";
            // 
            // gb_Target1
            // 
            this.gb_Target1.Controls.Add(this.cb_TRG_TableName);
            this.gb_Target1.Controls.Add(this.btn_Load_TRG_Table);
            this.gb_Target1.Controls.Add(this.tb_TRG_DefaultPort);
            this.gb_Target1.Controls.Add(this.cb_TRG_DefaultPort);
            this.gb_Target1.Controls.Add(this.cb_WindowAuthentication_TRG);
            this.gb_Target1.Controls.Add(this.label17);
            this.gb_Target1.Controls.Add(this.tb_TRG_Password);
            this.gb_Target1.Controls.Add(this.tb_TRG_UserName);
            this.gb_Target1.Controls.Add(this.tb_TRG_DBName);
            this.gb_Target1.Controls.Add(this.tb_TRG_DataSource);
            this.gb_Target1.Controls.Add(this.label8);
            this.gb_Target1.Controls.Add(this.label6);
            this.gb_Target1.Controls.Add(this.label4);
            this.gb_Target1.Controls.Add(this.label2);
            this.gb_Target1.Location = new System.Drawing.Point(9, 67);
            this.gb_Target1.Name = "gb_Target1";
            this.gb_Target1.Size = new System.Drawing.Size(354, 219);
            this.gb_Target1.TabIndex = 3;
            this.gb_Target1.TabStop = false;
            this.gb_Target1.Text = "Target : SQL Connection and Settings";
            // 
            // cb_TRG_TableName
            // 
            this.cb_TRG_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_TRG_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_TRG_TableName.DropDownWidth = 500;
            this.cb_TRG_TableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_TRG_TableName.Location = new System.Drawing.Point(130, 188);
            this.cb_TRG_TableName.Name = "cb_TRG_TableName";
            this.cb_TRG_TableName.Size = new System.Drawing.Size(207, 26);
            this.cb_TRG_TableName.TabIndex = 26;
            // 
            // btn_Load_TRG_Table
            // 
            this.btn_Load_TRG_Table.Location = new System.Drawing.Point(130, 160);
            this.btn_Load_TRG_Table.Name = "btn_Load_TRG_Table";
            this.btn_Load_TRG_Table.Size = new System.Drawing.Size(207, 23);
            this.btn_Load_TRG_Table.TabIndex = 31;
            this.btn_Load_TRG_Table.Text = "Load Target Tables";
            this.btn_Load_TRG_Table.UseVisualStyleBackColor = true;
            this.btn_Load_TRG_Table.Click += new System.EventHandler(this.btn_Load_TRG_Table_Click);
            // 
            // tb_TRG_DefaultPort
            // 
            this.tb_TRG_DefaultPort.Enabled = false;
            this.tb_TRG_DefaultPort.Location = new System.Drawing.Point(130, 39);
            this.tb_TRG_DefaultPort.Name = "tb_TRG_DefaultPort";
            this.tb_TRG_DefaultPort.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_DefaultPort.TabIndex = 14;
            this.tb_TRG_DefaultPort.Text = "1521";
            // 
            // cb_TRG_DefaultPort
            // 
            this.cb_TRG_DefaultPort.AutoSize = true;
            this.cb_TRG_DefaultPort.Checked = true;
            this.cb_TRG_DefaultPort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TRG_DefaultPort.Location = new System.Drawing.Point(19, 41);
            this.cb_TRG_DefaultPort.Name = "cb_TRG_DefaultPort";
            this.cb_TRG_DefaultPort.Size = new System.Drawing.Size(104, 17);
            this.cb_TRG_DefaultPort.TabIndex = 13;
            this.cb_TRG_DefaultPort.Text = "Use Default Port";
            this.cb_TRG_DefaultPort.UseVisualStyleBackColor = true;
            this.cb_TRG_DefaultPort.CheckedChanged += new System.EventHandler(this.cb_Target_DefaultPort_CheckedChanged);
            // 
            // cb_WindowAuthentication_TRG
            // 
            this.cb_WindowAuthentication_TRG.AutoSize = true;
            this.cb_WindowAuthentication_TRG.Location = new System.Drawing.Point(20, 19);
            this.cb_WindowAuthentication_TRG.Name = "cb_WindowAuthentication_TRG";
            this.cb_WindowAuthentication_TRG.Size = new System.Drawing.Size(141, 17);
            this.cb_WindowAuthentication_TRG.TabIndex = 12;
            this.cb_WindowAuthentication_TRG.Text = "Windows Authentication";
            this.cb_WindowAuthentication_TRG.UseVisualStyleBackColor = true;
            this.cb_WindowAuthentication_TRG.CheckedChanged += new System.EventHandler(this.cb_windowsAuthentication_CheckedChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(17, 195);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(106, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "Scheme/table name:";
            // 
            // tb_TRG_Password
            // 
            this.tb_TRG_Password.Location = new System.Drawing.Point(130, 136);
            this.tb_TRG_Password.Name = "tb_TRG_Password";
            this.tb_TRG_Password.PasswordChar = '*';
            this.tb_TRG_Password.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_Password.TabIndex = 9;
            // 
            // tb_TRG_UserName
            // 
            this.tb_TRG_UserName.Location = new System.Drawing.Point(130, 112);
            this.tb_TRG_UserName.Name = "tb_TRG_UserName";
            this.tb_TRG_UserName.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_UserName.TabIndex = 8;
            // 
            // tb_TRG_DBName
            // 
            this.tb_TRG_DBName.Location = new System.Drawing.Point(130, 87);
            this.tb_TRG_DBName.Name = "tb_TRG_DBName";
            this.tb_TRG_DBName.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_DBName.TabIndex = 7;
            // 
            // tb_TRG_DataSource
            // 
            this.tb_TRG_DataSource.Location = new System.Drawing.Point(130, 63);
            this.tb_TRG_DataSource.Name = "tb_TRG_DataSource";
            this.tb_TRG_DataSource.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_DataSource.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 137);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Username";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "DB Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Data Source";
            // 
            // btn_TRG_SRC
            // 
            this.btn_TRG_SRC.Location = new System.Drawing.Point(139, 288);
            this.btn_TRG_SRC.Name = "btn_TRG_SRC";
            this.btn_TRG_SRC.Size = new System.Drawing.Size(75, 23);
            this.btn_TRG_SRC.TabIndex = 32;
            this.btn_TRG_SRC.Text = "<<< Copy";
            this.btn_TRG_SRC.UseVisualStyleBackColor = true;
            this.btn_TRG_SRC.Click += new System.EventHandler(this.btn_TRG_SRC_Click);
            // 
            // gb_Target
            // 
            this.gb_Target.Controls.Add(this.rb_TRG_Oracle);
            this.gb_Target.Controls.Add(this.rb_TRG_SQL);
            this.gb_Target.Location = new System.Drawing.Point(9, 19);
            this.gb_Target.Name = "gb_Target";
            this.gb_Target.Size = new System.Drawing.Size(354, 42);
            this.gb_Target.TabIndex = 25;
            this.gb_Target.TabStop = false;
            this.gb_Target.Text = "Select Target Database Platform : ";
            // 
            // rb_TRG_Oracle
            // 
            this.rb_TRG_Oracle.AutoSize = true;
            this.rb_TRG_Oracle.Checked = true;
            this.rb_TRG_Oracle.Location = new System.Drawing.Point(59, 20);
            this.rb_TRG_Oracle.Name = "rb_TRG_Oracle";
            this.rb_TRG_Oracle.Size = new System.Drawing.Size(56, 17);
            this.rb_TRG_Oracle.TabIndex = 17;
            this.rb_TRG_Oracle.TabStop = true;
            this.rb_TRG_Oracle.Text = "Oracle";
            this.rb_TRG_Oracle.UseVisualStyleBackColor = true;
            this.rb_TRG_Oracle.CheckedChanged += new System.EventHandler(this.rb_Target_Oracle_CheckedChanged);
            // 
            // rb_TRG_SQL
            // 
            this.rb_TRG_SQL.AutoSize = true;
            this.rb_TRG_SQL.Location = new System.Drawing.Point(141, 19);
            this.rb_TRG_SQL.Name = "rb_TRG_SQL";
            this.rb_TRG_SQL.Size = new System.Drawing.Size(46, 17);
            this.rb_TRG_SQL.TabIndex = 18;
            this.rb_TRG_SQL.TabStop = true;
            this.rb_TRG_SQL.Text = "SQL";
            this.rb_TRG_SQL.UseVisualStyleBackColor = true;
            this.rb_TRG_SQL.CheckedChanged += new System.EventHandler(this.rb_Target_SQL_CheckedChanged);
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.gb_Source1);
            this.groupBox12.Controls.Add(this.gb_Source);
            this.groupBox12.Controls.Add(this.btn_Copy_SRC_TRG);
            this.groupBox12.Location = new System.Drawing.Point(10, 47);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(382, 317);
            this.groupBox12.TabIndex = 28;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Source";
            // 
            // gb_Source1
            // 
            this.gb_Source1.Controls.Add(this.cb_WindowAuthentication_SRC);
            this.gb_Source1.Controls.Add(this.btn_Load_SRC_Table);
            this.gb_Source1.Controls.Add(this.cb_SRC_TableName);
            this.gb_Source1.Controls.Add(this.tb_SRC_DefaultPort);
            this.gb_Source1.Controls.Add(this.cb_SRC_DefaultPort);
            this.gb_Source1.Controls.Add(this.label16);
            this.gb_Source1.Controls.Add(this.tb_SRC_Password);
            this.gb_Source1.Controls.Add(this.tb_SRC_UserName);
            this.gb_Source1.Controls.Add(this.tb_SRC_DBName);
            this.gb_Source1.Controls.Add(this.tb_SRC_DataSource);
            this.gb_Source1.Controls.Add(this.label9);
            this.gb_Source1.Controls.Add(this.label7);
            this.gb_Source1.Controls.Add(this.label5);
            this.gb_Source1.Controls.Add(this.label3);
            this.gb_Source1.Location = new System.Drawing.Point(9, 67);
            this.gb_Source1.Name = "gb_Source1";
            this.gb_Source1.Size = new System.Drawing.Size(354, 219);
            this.gb_Source1.TabIndex = 2;
            this.gb_Source1.TabStop = false;
            this.gb_Source1.Text = "Source : Oracle Connection and Settings";
            // 
            // cb_WindowAuthentication_SRC
            // 
            this.cb_WindowAuthentication_SRC.AutoSize = true;
            this.cb_WindowAuthentication_SRC.Location = new System.Drawing.Point(16, 19);
            this.cb_WindowAuthentication_SRC.Name = "cb_WindowAuthentication_SRC";
            this.cb_WindowAuthentication_SRC.Size = new System.Drawing.Size(136, 17);
            this.cb_WindowAuthentication_SRC.TabIndex = 30;
            this.cb_WindowAuthentication_SRC.Text = "Window Authentication";
            this.cb_WindowAuthentication_SRC.UseVisualStyleBackColor = true;
            this.cb_WindowAuthentication_SRC.CheckedChanged += new System.EventHandler(this.cb_WindowAuthentication_SRC_CheckedChanged);
            // 
            // btn_Load_SRC_Table
            // 
            this.btn_Load_SRC_Table.Location = new System.Drawing.Point(128, 160);
            this.btn_Load_SRC_Table.Name = "btn_Load_SRC_Table";
            this.btn_Load_SRC_Table.Size = new System.Drawing.Size(207, 23);
            this.btn_Load_SRC_Table.TabIndex = 13;
            this.btn_Load_SRC_Table.Text = "Load Source Tables";
            this.btn_Load_SRC_Table.UseVisualStyleBackColor = true;
            this.btn_Load_SRC_Table.Click += new System.EventHandler(this.btn_Load_SRC_Table_Click);
            // 
            // cb_SRC_TableName
            // 
            this.cb_SRC_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_SRC_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_SRC_TableName.DropDownWidth = 500;
            this.cb_SRC_TableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SRC_TableName.FormattingEnabled = true;
            this.cb_SRC_TableName.Location = new System.Drawing.Point(128, 187);
            this.cb_SRC_TableName.Name = "cb_SRC_TableName";
            this.cb_SRC_TableName.Size = new System.Drawing.Size(207, 26);
            this.cb_SRC_TableName.TabIndex = 29;
            // 
            // tb_SRC_DefaultPort
            // 
            this.tb_SRC_DefaultPort.Enabled = false;
            this.tb_SRC_DefaultPort.Location = new System.Drawing.Point(128, 37);
            this.tb_SRC_DefaultPort.Name = "tb_SRC_DefaultPort";
            this.tb_SRC_DefaultPort.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DefaultPort.TabIndex = 12;
            this.tb_SRC_DefaultPort.Text = "1521";
            this.tb_SRC_DefaultPort.TextChanged += new System.EventHandler(this.tb_Default_Port_TextChanged);
            // 
            // cb_SRC_DefaultPort
            // 
            this.cb_SRC_DefaultPort.AutoSize = true;
            this.cb_SRC_DefaultPort.Checked = true;
            this.cb_SRC_DefaultPort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SRC_DefaultPort.Location = new System.Drawing.Point(16, 39);
            this.cb_SRC_DefaultPort.Name = "cb_SRC_DefaultPort";
            this.cb_SRC_DefaultPort.Size = new System.Drawing.Size(104, 17);
            this.cb_SRC_DefaultPort.TabIndex = 11;
            this.cb_SRC_DefaultPort.Text = "Use Default Port";
            this.cb_SRC_DefaultPort.UseVisualStyleBackColor = true;
            this.cb_SRC_DefaultPort.CheckedChanged += new System.EventHandler(this.cb_Default_Port_CheckedChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(13, 194);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(106, 13);
            this.label16.TabIndex = 9;
            this.label16.Text = "Scheme/table name:";
            // 
            // tb_SRC_Password
            // 
            this.tb_SRC_Password.Location = new System.Drawing.Point(128, 138);
            this.tb_SRC_Password.Name = "tb_SRC_Password";
            this.tb_SRC_Password.PasswordChar = '*';
            this.tb_SRC_Password.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_Password.TabIndex = 8;
            // 
            // tb_SRC_UserName
            // 
            this.tb_SRC_UserName.Location = new System.Drawing.Point(128, 114);
            this.tb_SRC_UserName.Name = "tb_SRC_UserName";
            this.tb_SRC_UserName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_UserName.TabIndex = 7;
            // 
            // tb_SRC_DBName
            // 
            this.tb_SRC_DBName.Location = new System.Drawing.Point(128, 89);
            this.tb_SRC_DBName.Name = "tb_SRC_DBName";
            this.tb_SRC_DBName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DBName.TabIndex = 6;
            // 
            // tb_SRC_DataSource
            // 
            this.tb_SRC_DataSource.Location = new System.Drawing.Point(128, 64);
            this.tb_SRC_DataSource.Name = "tb_SRC_DataSource";
            this.tb_SRC_DataSource.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DataSource.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 142);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Password";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Username";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "DB Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Data Source";
            // 
            // gb_Source
            // 
            this.gb_Source.Controls.Add(this.rb_SRC_oracle);
            this.gb_Source.Controls.Add(this.rb_SRC_SQL);
            this.gb_Source.Location = new System.Drawing.Point(10, 19);
            this.gb_Source.Name = "gb_Source";
            this.gb_Source.Size = new System.Drawing.Size(350, 42);
            this.gb_Source.TabIndex = 24;
            this.gb_Source.TabStop = false;
            this.gb_Source.Text = "Select Source Database Platform : ";
            // 
            // rb_SRC_oracle
            // 
            this.rb_SRC_oracle.AutoSize = true;
            this.rb_SRC_oracle.Checked = true;
            this.rb_SRC_oracle.Location = new System.Drawing.Point(59, 20);
            this.rb_SRC_oracle.Name = "rb_SRC_oracle";
            this.rb_SRC_oracle.Size = new System.Drawing.Size(56, 17);
            this.rb_SRC_oracle.TabIndex = 17;
            this.rb_SRC_oracle.TabStop = true;
            this.rb_SRC_oracle.Text = "Oracle";
            this.rb_SRC_oracle.UseVisualStyleBackColor = true;
            this.rb_SRC_oracle.CheckedChanged += new System.EventHandler(this.rb_oracle_CheckedChanged);
            // 
            // rb_SRC_SQL
            // 
            this.rb_SRC_SQL.AutoSize = true;
            this.rb_SRC_SQL.Location = new System.Drawing.Point(141, 19);
            this.rb_SRC_SQL.Name = "rb_SRC_SQL";
            this.rb_SRC_SQL.Size = new System.Drawing.Size(46, 17);
            this.rb_SRC_SQL.TabIndex = 18;
            this.rb_SRC_SQL.TabStop = true;
            this.rb_SRC_SQL.Text = "SQL";
            this.rb_SRC_SQL.UseVisualStyleBackColor = true;
            this.rb_SRC_SQL.CheckedChanged += new System.EventHandler(this.rb_sql_CheckedChanged);
            // 
            // btn_Copy_SRC_TRG
            // 
            this.btn_Copy_SRC_TRG.Location = new System.Drawing.Point(137, 289);
            this.btn_Copy_SRC_TRG.Name = "btn_Copy_SRC_TRG";
            this.btn_Copy_SRC_TRG.Size = new System.Drawing.Size(75, 23);
            this.btn_Copy_SRC_TRG.TabIndex = 31;
            this.btn_Copy_SRC_TRG.Text = "Copy >>>";
            this.btn_Copy_SRC_TRG.UseVisualStyleBackColor = true;
            this.btn_Copy_SRC_TRG.Click += new System.EventHandler(this.btn_Copy_SRC_TRG_Click);
            // 
            // cb_FullName
            // 
            this.cb_FullName.AutoSize = true;
            this.cb_FullName.Checked = true;
            this.cb_FullName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_FullName.Location = new System.Drawing.Point(308, 370);
            this.cb_FullName.Name = "cb_FullName";
            this.cb_FullName.Size = new System.Drawing.Size(165, 17);
            this.cb_FullName.TabIndex = 30;
            this.cb_FullName.Text = "Show FullName in DropDown";
            this.cb_FullName.UseVisualStyleBackColor = true;
            this.cb_FullName.CheckedChanged += new System.EventHandler(this.cb_FullName_CheckedChanged);
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.label15);
            this.groupBox14.Controls.Add(this.cb_UserProfileList);
            this.groupBox14.Controls.Add(this.cb_FullName);
            this.groupBox14.Controls.Add(this.groupBox12);
            this.groupBox14.Controls.Add(this.groupBox13);
            this.groupBox14.Location = new System.Drawing.Point(12, 12);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(799, 392);
            this.groupBox14.TabIndex = 30;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Connection Settings";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DB_Comparison_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1229, 749);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox16);
            this.Controls.Add(this.groupBox15);
            this.Controls.Add(this.groupBox14);
            this.Controls.Add(this.groupBox9);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DB_Comparison_Form";
            this.Text = "Compare Oracle and SQL server Data and Generate Report";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.gb_Target1.ResumeLayout(false);
            this.gb_Target1.PerformLayout();
            this.gb_Target.ResumeLayout(false);
            this.gb_Target.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.gb_Source1.ResumeLayout(false);
            this.gb_Source1.PerformLayout();
            this.gb_Source.ResumeLayout(false);
            this.gb_Source.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

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
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.Button btn_LoadProfile;
        private System.Windows.Forms.CheckBox cb_ColumnMapping;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lb_IgnoredCol_TRG;
        private System.Windows.Forms.ListBox lb_IgnoredCol_SRC;
        private System.Windows.Forms.ListBox lb_TRG_Cols;
        private System.Windows.Forms.Button btn_SaveProfile;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tb_ProfileName;
        private System.Windows.Forms.Label lbl_ProfileStatus;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.GroupBox gb_Target1;
        private System.Windows.Forms.ComboBox cb_TRG_TableName;
        private System.Windows.Forms.Button btn_Load_TRG_Table;
        private System.Windows.Forms.TextBox tb_TRG_DefaultPort;
        private System.Windows.Forms.CheckBox cb_TRG_DefaultPort;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_TRG;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tb_TRG_Password;
        private System.Windows.Forms.TextBox tb_TRG_UserName;
        private System.Windows.Forms.TextBox tb_TRG_DBName;
        private System.Windows.Forms.TextBox tb_TRG_DataSource;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gb_Target;
        private System.Windows.Forms.RadioButton rb_TRG_Oracle;
        private System.Windows.Forms.RadioButton rb_TRG_SQL;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.GroupBox gb_Source1;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_SRC;
        private System.Windows.Forms.Button btn_Load_SRC_Table;
        private System.Windows.Forms.ComboBox cb_SRC_TableName;
        private System.Windows.Forms.TextBox tb_SRC_DefaultPort;
        private System.Windows.Forms.CheckBox cb_SRC_DefaultPort;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tb_SRC_Password;
        private System.Windows.Forms.TextBox tb_SRC_UserName;
        private System.Windows.Forms.TextBox tb_SRC_DBName;
        private System.Windows.Forms.TextBox tb_SRC_DataSource;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox gb_Source;
        private System.Windows.Forms.RadioButton rb_SRC_oracle;
        private System.Windows.Forms.RadioButton rb_SRC_SQL;
        private System.Windows.Forms.ComboBox cb_UserProfileList;
        private System.Windows.Forms.CheckBox cb_FullName;
        private System.Windows.Forms.Button btn_Copy_SRC_TRG;
        private System.Windows.Forms.Button btn_TRG_SRC;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox cb_PrimaryKey;
        private System.Windows.Forms.Timer timer1;
    }
}

