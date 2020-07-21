namespace SPI_Data_Comp_Tool
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnGenerateOutput = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb_Default_Port = new System.Windows.Forms.TextBox();
            this.cb_Default_Port = new System.Windows.Forms.CheckBox();
            this.tb_Source_tableName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tb_oracle_pwd = new System.Windows.Forms.TextBox();
            this.tb_oracle_username = new System.Windows.Forms.TextBox();
            this.tb_oracle_dbName = new System.Windows.Forms.TextBox();
            this.tb_oracle_dsource = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_destination_tableName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tb_sql_pwd = new System.Windows.Forms.TextBox();
            this.tb_sql_username = new System.Windows.Forms.TextBox();
            this.tb_sql_dbName = new System.Windows.Forms.TextBox();
            this.tb_sql_dsource = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btn_Connect_Reference_ID = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblTimer = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCal = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.tb_FileName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.btn_Set = new System.Windows.Forms.Button();
            this.llbl_Folder_path = new System.Windows.Forms.LinkLabel();
            this.label13 = new System.Windows.Forms.Label();
            this.lblExpressionStatus = new System.Windows.Forms.Label();
            this.cbL_columns = new System.Windows.Forms.CheckedListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cb_selectAll_toggle = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.rb_oracle = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbL_TargetColumns = new System.Windows.Forms.CheckedListBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbl_PercentageShow = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.clb_ReferenceID = new System.Windows.Forms.CheckedListBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.rb_sql = new System.Windows.Forms.RadioButton();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.cb_windowsAuthentication = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerateOutput
            // 
            this.btnGenerateOutput.Enabled = false;
            this.btnGenerateOutput.Location = new System.Drawing.Point(14, 180);
            this.btnGenerateOutput.Name = "btnGenerateOutput";
            this.btnGenerateOutput.Size = new System.Drawing.Size(172, 23);
            this.btnGenerateOutput.TabIndex = 0;
            this.btnGenerateOutput.Text = "Compare and Generate Excel";
            this.btnGenerateOutput.UseVisualStyleBackColor = true;
            this.btnGenerateOutput.Click += new System.EventHandler(this.btnGenerateOutput_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(253, 46);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(202, 128);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Okay";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_Default_Port);
            this.groupBox1.Controls.Add(this.cb_Default_Port);
            this.groupBox1.Controls.Add(this.tb_Source_tableName);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.tb_oracle_pwd);
            this.groupBox1.Controls.Add(this.tb_oracle_username);
            this.groupBox1.Controls.Add(this.tb_oracle_dbName);
            this.groupBox1.Controls.Add(this.tb_oracle_dsource);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 265);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source : Oracle Connection and Settings";
            // 
            // tb_Default_Port
            // 
            this.tb_Default_Port.Enabled = false;
            this.tb_Default_Port.Location = new System.Drawing.Point(128, 230);
            this.tb_Default_Port.Name = "tb_Default_Port";
            this.tb_Default_Port.Size = new System.Drawing.Size(207, 20);
            this.tb_Default_Port.TabIndex = 12;
            this.tb_Default_Port.Text = "1521";
            // 
            // cb_Default_Port
            // 
            this.cb_Default_Port.AutoSize = true;
            this.cb_Default_Port.Checked = true;
            this.cb_Default_Port.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Default_Port.Location = new System.Drawing.Point(16, 232);
            this.cb_Default_Port.Name = "cb_Default_Port";
            this.cb_Default_Port.Size = new System.Drawing.Size(104, 17);
            this.cb_Default_Port.TabIndex = 11;
            this.cb_Default_Port.Text = "Use Default Port";
            this.cb_Default_Port.UseVisualStyleBackColor = true;
            this.cb_Default_Port.CheckedChanged += new System.EventHandler(this.cb_Default_Port_CheckedChanged);
            // 
            // tb_Source_tableName
            // 
            this.tb_Source_tableName.Location = new System.Drawing.Point(128, 196);
            this.tb_Source_tableName.Name = "tb_Source_tableName";
            this.tb_Source_tableName.Size = new System.Drawing.Size(207, 20);
            this.tb_Source_tableName.TabIndex = 10;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(13, 199);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(106, 13);
            this.label16.TabIndex = 9;
            this.label16.Text = "Scheme/table name:";
            // 
            // tb_oracle_pwd
            // 
            this.tb_oracle_pwd.Location = new System.Drawing.Point(128, 163);
            this.tb_oracle_pwd.Name = "tb_oracle_pwd";
            this.tb_oracle_pwd.PasswordChar = '*';
            this.tb_oracle_pwd.Size = new System.Drawing.Size(207, 20);
            this.tb_oracle_pwd.TabIndex = 8;
            // 
            // tb_oracle_username
            // 
            this.tb_oracle_username.Location = new System.Drawing.Point(128, 128);
            this.tb_oracle_username.Name = "tb_oracle_username";
            this.tb_oracle_username.Size = new System.Drawing.Size(207, 20);
            this.tb_oracle_username.TabIndex = 7;
            // 
            // tb_oracle_dbName
            // 
            this.tb_oracle_dbName.Location = new System.Drawing.Point(128, 95);
            this.tb_oracle_dbName.Name = "tb_oracle_dbName";
            this.tb_oracle_dbName.Size = new System.Drawing.Size(207, 20);
            this.tb_oracle_dbName.TabIndex = 6;
            // 
            // tb_oracle_dsource
            // 
            this.tb_oracle_dsource.Location = new System.Drawing.Point(128, 64);
            this.tb_oracle_dsource.Name = "tb_oracle_dsource";
            this.tb_oracle_dsource.Size = new System.Drawing.Size(207, 20);
            this.tb_oracle_dsource.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 163);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Password";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Username";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 98);
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_windowsAuthentication);
            this.groupBox2.Controls.Add(this.tb_destination_tableName);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.tb_sql_pwd);
            this.groupBox2.Controls.Add(this.tb_sql_username);
            this.groupBox2.Controls.Add(this.tb_sql_dbName);
            this.groupBox2.Controls.Add(this.tb_sql_dsource);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(385, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(354, 265);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target : SQL Connection and Settings";
            // 
            // tb_destination_tableName
            // 
            this.tb_destination_tableName.Location = new System.Drawing.Point(130, 200);
            this.tb_destination_tableName.Name = "tb_destination_tableName";
            this.tb_destination_tableName.Size = new System.Drawing.Size(207, 20);
            this.tb_destination_tableName.TabIndex = 11;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(17, 203);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(106, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "Scheme/table name:";
            // 
            // tb_sql_pwd
            // 
            this.tb_sql_pwd.Location = new System.Drawing.Point(130, 166);
            this.tb_sql_pwd.Name = "tb_sql_pwd";
            this.tb_sql_pwd.PasswordChar = '*';
            this.tb_sql_pwd.Size = new System.Drawing.Size(207, 20);
            this.tb_sql_pwd.TabIndex = 9;
            // 
            // tb_sql_username
            // 
            this.tb_sql_username.Location = new System.Drawing.Point(130, 132);
            this.tb_sql_username.Name = "tb_sql_username";
            this.tb_sql_username.Size = new System.Drawing.Size(207, 20);
            this.tb_sql_username.TabIndex = 8;
            // 
            // tb_sql_dbName
            // 
            this.tb_sql_dbName.Location = new System.Drawing.Point(130, 99);
            this.tb_sql_dbName.Name = "tb_sql_dbName";
            this.tb_sql_dbName.Size = new System.Drawing.Size(207, 20);
            this.tb_sql_dbName.TabIndex = 7;
            // 
            // tb_sql_dsource
            // 
            this.tb_sql_dsource.Location = new System.Drawing.Point(130, 65);
            this.tb_sql_dsource.Name = "tb_sql_dsource";
            this.tb_sql_dsource.Size = new System.Drawing.Size(207, 20);
            this.tb_sql_dsource.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 167);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Username";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "DB Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Data Source";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(253, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Status : ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(153, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Reference ID (For Comparison)";
            // 
            // btn_Connect_Reference_ID
            // 
            this.btn_Connect_Reference_ID.Location = new System.Drawing.Point(12, 76);
            this.btn_Connect_Reference_ID.Name = "btn_Connect_Reference_ID";
            this.btn_Connect_Reference_ID.Size = new System.Drawing.Size(141, 23);
            this.btn_Connect_Reference_ID.TabIndex = 10;
            this.btn_Connect_Reference_ID.Text = "Connect and Load";
            this.btn_Connect_Reference_ID.UseVisualStyleBackColor = true;
            this.btn_Connect_Reference_ID.Click += new System.EventHandler(this.btn_Connect_Reference_ID_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblTimer);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.lblCal);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.btn_Delete);
            this.groupBox3.Controls.Add(this.btn_Cancel);
            this.groupBox3.Controls.Add(this.lblStatus);
            this.groupBox3.Controls.Add(this.btn_Save);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Controls.Add(this.btnGenerateOutput);
            this.groupBox3.Controls.Add(this.tb_FileName);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.btn_Clear_Reset);
            this.groupBox3.Controls.Add(this.btn_Set);
            this.groupBox3.Controls.Add(this.llbl_Folder_path);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.lblExpressionStatus);
            this.groupBox3.Controls.Add(this.btn_Connect_Reference_ID);
            this.groupBox3.Location = new System.Drawing.Point(745, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(468, 314);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Controls and Output";
            // 
            // lblTimer
            // 
            this.lblTimer.Location = new System.Drawing.Point(339, 180);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(123, 86);
            this.lblTimer.TabIndex = 19;
            this.lblTimer.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(253, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Time Elapsed : ";
            // 
            // lblCal
            // 
            this.lblCal.Location = new System.Drawing.Point(9, 237);
            this.lblCal.Name = "lblCal";
            this.lblCal.Size = new System.Drawing.Size(238, 37);
            this.lblCal.TabIndex = 22;
            this.lblCal.Text = "Total Rows : Columns :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Load Profile";
            this.label12.Visible = false;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(172, 46);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(75, 23);
            this.btn_Delete.TabIndex = 17;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Visible = false;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Enabled = false;
            this.btn_Cancel.Location = new System.Drawing.Point(192, 180);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(55, 23);
            this.btn_Cancel.TabIndex = 20;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(78, 46);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 16;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Visible = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(78, 19);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(169, 21);
            this.comboBox2.TabIndex = 15;
            this.comboBox2.Visible = false;
            // 
            // tb_FileName
            // 
            this.tb_FileName.Location = new System.Drawing.Point(82, 154);
            this.tb_FileName.Name = "tb_FileName";
            this.tb_FileName.Size = new System.Drawing.Size(165, 20);
            this.tb_FileName.TabIndex = 17;
            this.tb_FileName.Text = "_output";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 157);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 13);
            this.label14.TabIndex = 18;
            this.label14.Text = "Filename : ";
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Location = new System.Drawing.Point(163, 76);
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
            this.btn_Set.Location = new System.Drawing.Point(12, 105);
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.Size = new System.Drawing.Size(75, 23);
            this.btn_Set.TabIndex = 13;
            this.btn_Set.Text = "Set";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            // 
            // llbl_Folder_path
            // 
            this.llbl_Folder_path.AutoSize = true;
            this.llbl_Folder_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llbl_Folder_path.Location = new System.Drawing.Point(55, 132);
            this.llbl_Folder_path.Name = "llbl_Folder_path";
            this.llbl_Folder_path.Size = new System.Drawing.Size(71, 16);
            this.llbl_Folder_path.TabIndex = 15;
            this.llbl_Folder_path.TabStop = true;
            this.llbl_Folder_path.Text = "linkLabel";
            this.llbl_Folder_path.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_Folder_path_LinkClicked);
            this.llbl_Folder_path.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(11, 135);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(38, 13);
            this.label13.TabIndex = 16;
            this.label13.Text = "Path : ";
            // 
            // lblExpressionStatus
            // 
            this.lblExpressionStatus.AutoSize = true;
            this.lblExpressionStatus.Location = new System.Drawing.Point(9, 212);
            this.lblExpressionStatus.Name = "lblExpressionStatus";
            this.lblExpressionStatus.Size = new System.Drawing.Size(37, 13);
            this.lblExpressionStatus.TabIndex = 12;
            this.lblExpressionStatus.Text = "Status";
            // 
            // cbL_columns
            // 
            this.cbL_columns.CheckOnClick = true;
            this.cbL_columns.FormattingEnabled = true;
            this.cbL_columns.Location = new System.Drawing.Point(14, 48);
            this.cbL_columns.Name = "cbL_columns";
            this.cbL_columns.Size = new System.Drawing.Size(175, 289);
            this.cbL_columns.TabIndex = 13;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cb_selectAll_toggle);
            this.groupBox5.Controls.Add(this.cbL_columns);
            this.groupBox5.Location = new System.Drawing.Point(220, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(211, 356);
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
            this.dataGridView1.Location = new System.Drawing.Point(1133, 356);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Size = new System.Drawing.Size(67, 145);
            this.dataGridView1.TabIndex = 15;
            this.dataGridView1.Visible = false;
            // 
            // rb_oracle
            // 
            this.rb_oracle.AutoSize = true;
            this.rb_oracle.Checked = true;
            this.rb_oracle.Location = new System.Drawing.Point(59, 20);
            this.rb_oracle.Name = "rb_oracle";
            this.rb_oracle.Size = new System.Drawing.Size(56, 17);
            this.rb_oracle.TabIndex = 17;
            this.rb_oracle.TabStop = true;
            this.rb_oracle.Text = "Oracle";
            this.rb_oracle.UseVisualStyleBackColor = true;
            this.rb_oracle.CheckedChanged += new System.EventHandler(this.rb_oracle_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbL_TargetColumns);
            this.groupBox6.Location = new System.Drawing.Point(445, 19);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(220, 356);
            this.groupBox6.TabIndex = 19;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Target SQL Columns";
            // 
            // cbL_TargetColumns
            // 
            this.cbL_TargetColumns.CheckOnClick = true;
            this.cbL_TargetColumns.FormattingEnabled = true;
            this.cbL_TargetColumns.Location = new System.Drawing.Point(19, 48);
            this.cbL_TargetColumns.Name = "cbL_TargetColumns";
            this.cbL_TargetColumns.Size = new System.Drawing.Size(182, 289);
            this.cbL_TargetColumns.TabIndex = 0;
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
            "7"});
            this.comboBox1.Location = new System.Drawing.Point(385, 12);
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
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 381);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(989, 33);
            this.progressBar1.TabIndex = 21;
            // 
            // lbl_PercentageShow
            // 
            this.lbl_PercentageShow.AutoSize = true;
            this.lbl_PercentageShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PercentageShow.Location = new System.Drawing.Point(383, 389);
            this.lbl_PercentageShow.Name = "lbl_PercentageShow";
            this.lbl_PercentageShow.Size = new System.Drawing.Size(113, 18);
            this.lbl_PercentageShow.TabIndex = 22;
            this.lbl_PercentageShow.Text = "Progress Bar %";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.clb_ReferenceID);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Location = new System.Drawing.Point(16, 19);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(191, 353);
            this.groupBox7.TabIndex = 23;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Select Reference Key";
            // 
            // clb_ReferenceID
            // 
            this.clb_ReferenceID.CheckOnClick = true;
            this.clb_ReferenceID.FormattingEnabled = true;
            this.clb_ReferenceID.Location = new System.Drawing.Point(12, 45);
            this.clb_ReferenceID.Name = "clb_ReferenceID";
            this.clb_ReferenceID.Size = new System.Drawing.Size(158, 289);
            this.clb_ReferenceID.TabIndex = 0;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.rb_oracle);
            this.groupBox8.Controls.Add(this.rb_sql);
            this.groupBox8.Location = new System.Drawing.Point(16, 7);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(350, 48);
            this.groupBox8.TabIndex = 24;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Select Source Database Platform : ";
            // 
            // rb_sql
            // 
            this.rb_sql.AutoSize = true;
            this.rb_sql.Location = new System.Drawing.Point(141, 19);
            this.rb_sql.Name = "rb_sql";
            this.rb_sql.Size = new System.Drawing.Size(46, 17);
            this.rb_sql.TabIndex = 18;
            this.rb_sql.TabStop = true;
            this.rb_sql.Text = "SQL";
            this.rb_sql.UseVisualStyleBackColor = true;
            this.rb_sql.CheckedChanged += new System.EventHandler(this.rb_sql_CheckedChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.groupBox6);
            this.groupBox9.Controls.Add(this.groupBox7);
            this.groupBox9.Controls.Add(this.lbl_PercentageShow);
            this.groupBox9.Controls.Add(this.progressBar1);
            this.groupBox9.Controls.Add(this.groupBox5);
            this.groupBox9.Location = new System.Drawing.Point(12, 358);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(1096, 427);
            this.groupBox9.TabIndex = 25;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "View Data";
            // 
            // cb_windowsAuthentication
            // 
            this.cb_windowsAuthentication.AutoSize = true;
            this.cb_windowsAuthentication.Location = new System.Drawing.Point(20, 31);
            this.cb_windowsAuthentication.Name = "cb_windowsAuthentication";
            this.cb_windowsAuthentication.Size = new System.Drawing.Size(141, 17);
            this.cb_windowsAuthentication.TabIndex = 12;
            this.cb_windowsAuthentication.Text = "Windows Authentication";
            this.cb_windowsAuthentication.UseVisualStyleBackColor = true;
            this.cb_windowsAuthentication.CheckedChanged += new System.EventHandler(this.cb_windowsAuthentication_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1234, 749);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox9);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Compare Oracle and SQL server Data and Generate Report";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateOutput;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_oracle_pwd;
        private System.Windows.Forms.TextBox tb_oracle_username;
        private System.Windows.Forms.TextBox tb_oracle_dbName;
        private System.Windows.Forms.TextBox tb_oracle_dsource;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_sql_pwd;
        private System.Windows.Forms.TextBox tb_sql_username;
        private System.Windows.Forms.TextBox tb_sql_dbName;
        private System.Windows.Forms.TextBox tb_sql_dsource;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btn_Connect_Reference_ID;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblExpressionStatus;
        private System.Windows.Forms.CheckedListBox cbL_columns;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cb_selectAll_toggle;
        private System.Windows.Forms.Button btn_Set;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.LinkLabel llbl_Folder_path;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tb_FileName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RadioButton rb_oracle;
        private System.Windows.Forms.RadioButton rb_sql;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tb_Source_tableName;
        private System.Windows.Forms.TextBox tb_destination_tableName;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckedListBox cbL_TargetColumns;
        private System.Windows.Forms.ComboBox comboBox1;
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
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox tb_Default_Port;
        private System.Windows.Forms.CheckBox cb_Default_Port;
        private System.Windows.Forms.CheckBox cb_windowsAuthentication;
    }
}

