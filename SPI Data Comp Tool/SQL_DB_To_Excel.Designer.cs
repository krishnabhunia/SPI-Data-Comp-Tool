namespace SPI_Data_Comp_Tool
{
    partial class SQL_DB_To_Excel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQL_DB_To_Excel));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cb_UserProfileList = new System.Windows.Forms.ComboBox();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tb_DefaultPort = new System.Windows.Forms.TextBox();
            this.rb_SQL = new System.Windows.Forms.RadioButton();
            this.cb_DefaultPort = new System.Windows.Forms.CheckBox();
            this.rb_Oracle = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.tb_SRC_DBName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_SRC_UserName = new System.Windows.Forms.TextBox();
            this.tb_SRC_DataSource = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_SRC_Password = new System.Windows.Forms.TextBox();
            this.cb_windowsAuthentication = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_ExportToExcel = new System.Windows.Forms.Button();
            this.clb_TableList = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_SkipEmptyTable = new System.Windows.Forms.CheckBox();
            this.cb_SelectAll = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rb_SRC = new System.Windows.Forms.RadioButton();
            this.rb_TRG = new System.Windows.Forms.RadioButton();
            this.tb_Find = new System.Windows.Forms.TextBox();
            this.lbl_Find = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_Cancel);
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.tb_DefaultPort);
            this.groupBox2.Controls.Add(this.rb_SQL);
            this.groupBox2.Controls.Add(this.cb_DefaultPort);
            this.groupBox2.Controls.Add(this.rb_Oracle);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btn_Connect);
            this.groupBox2.Controls.Add(this.lbl_Status);
            this.groupBox2.Controls.Add(this.tb_SRC_DBName);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tb_SRC_UserName);
            this.groupBox2.Controls.Add(this.tb_SRC_DataSource);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tb_SRC_Password);
            this.groupBox2.Controls.Add(this.cb_windowsAuthentication);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(12, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(354, 418);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "DB Connection and Settings";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 21);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(103, 13);
            this.label15.TabIndex = 38;
            this.label15.Text = "Select User Profile : ";
            this.label15.Click += new System.EventHandler(this.label15_Click);
            // 
            // cb_UserProfileList
            // 
            this.cb_UserProfileList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_UserProfileList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_UserProfileList.FormattingEnabled = true;
            this.cb_UserProfileList.ItemHeight = 13;
            this.cb_UserProfileList.Location = new System.Drawing.Point(124, 17);
            this.cb_UserProfileList.Name = "cb_UserProfileList";
            this.cb_UserProfileList.Size = new System.Drawing.Size(225, 21);
            this.cb_UserProfileList.TabIndex = 37;
            this.cb_UserProfileList.SelectedIndexChanged += new System.EventHandler(this.cb_UserProfileList_SelectedIndexChanged);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(142, 210);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 17;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(17, 246);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(314, 14);
            this.progressBar1.TabIndex = 6;
            // 
            // tb_DefaultPort
            // 
            this.tb_DefaultPort.Enabled = false;
            this.tb_DefaultPort.Location = new System.Drawing.Point(168, 56);
            this.tb_DefaultPort.MaxLength = 6;
            this.tb_DefaultPort.Name = "tb_DefaultPort";
            this.tb_DefaultPort.ReadOnly = true;
            this.tb_DefaultPort.Size = new System.Drawing.Size(163, 20);
            this.tb_DefaultPort.TabIndex = 16;
            this.tb_DefaultPort.Text = "1521";
            this.tb_DefaultPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_DefaultPort_KeyPress);
            // 
            // rb_SQL
            // 
            this.rb_SQL.AutoSize = true;
            this.rb_SQL.Location = new System.Drawing.Point(106, 25);
            this.rb_SQL.Name = "rb_SQL";
            this.rb_SQL.Size = new System.Drawing.Size(46, 17);
            this.rb_SQL.TabIndex = 1;
            this.rb_SQL.TabStop = true;
            this.rb_SQL.Text = "SQL";
            this.rb_SQL.UseVisualStyleBackColor = true;
            this.rb_SQL.CheckedChanged += new System.EventHandler(this.rb_SQL_CheckedChanged);
            // 
            // cb_DefaultPort
            // 
            this.cb_DefaultPort.AutoSize = true;
            this.cb_DefaultPort.Checked = true;
            this.cb_DefaultPort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_DefaultPort.Location = new System.Drawing.Point(14, 56);
            this.cb_DefaultPort.Name = "cb_DefaultPort";
            this.cb_DefaultPort.Size = new System.Drawing.Size(134, 17);
            this.cb_DefaultPort.TabIndex = 15;
            this.cb_DefaultPort.Text = "Default Port For Oracle";
            this.cb_DefaultPort.UseVisualStyleBackColor = true;
            this.cb_DefaultPort.CheckedChanged += new System.EventHandler(this.cb_DefaultPort_CheckedChanged);
            // 
            // rb_Oracle
            // 
            this.rb_Oracle.AutoSize = true;
            this.rb_Oracle.Location = new System.Drawing.Point(35, 24);
            this.rb_Oracle.Name = "rb_Oracle";
            this.rb_Oracle.Size = new System.Drawing.Size(56, 17);
            this.rb_Oracle.TabIndex = 0;
            this.rb_Oracle.TabStop = true;
            this.rb_Oracle.Text = "Oracle";
            this.rb_Oracle.UseVisualStyleBackColor = true;
            this.rb_Oracle.CheckedChanged += new System.EventHandler(this.rb_Oracle_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Data Source";
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(14, 210);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(110, 23);
            this.btn_Connect.TabIndex = 13;
            this.btn_Connect.Text = "Connect and Load";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoEllipsis = true;
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(14, 271);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(317, 131);
            this.lbl_Status.TabIndex = 14;
            this.lbl_Status.Text = "Status : Okay";
            // 
            // tb_SRC_DBName
            // 
            this.tb_SRC_DBName.Location = new System.Drawing.Point(124, 117);
            this.tb_SRC_DBName.Name = "tb_SRC_DBName";
            this.tb_SRC_DBName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DBName.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "DB Name";
            // 
            // tb_SRC_UserName
            // 
            this.tb_SRC_UserName.Location = new System.Drawing.Point(124, 148);
            this.tb_SRC_UserName.Name = "tb_SRC_UserName";
            this.tb_SRC_UserName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_UserName.TabIndex = 8;
            // 
            // tb_SRC_DataSource
            // 
            this.tb_SRC_DataSource.Location = new System.Drawing.Point(124, 86);
            this.tb_SRC_DataSource.Name = "tb_SRC_DataSource";
            this.tb_SRC_DataSource.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DataSource.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Username";
            // 
            // tb_SRC_Password
            // 
            this.tb_SRC_Password.Location = new System.Drawing.Point(124, 178);
            this.tb_SRC_Password.Name = "tb_SRC_Password";
            this.tb_SRC_Password.PasswordChar = '*';
            this.tb_SRC_Password.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_Password.TabIndex = 9;
            // 
            // cb_windowsAuthentication
            // 
            this.cb_windowsAuthentication.AutoSize = true;
            this.cb_windowsAuthentication.Location = new System.Drawing.Point(181, 26);
            this.cb_windowsAuthentication.Name = "cb_windowsAuthentication";
            this.cb_windowsAuthentication.Size = new System.Drawing.Size(141, 17);
            this.cb_windowsAuthentication.TabIndex = 12;
            this.cb_windowsAuthentication.Text = "Windows Authentication";
            this.cb_windowsAuthentication.UseVisualStyleBackColor = true;
            this.cb_windowsAuthentication.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 179);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Password";
            // 
            // btn_ExportToExcel
            // 
            this.btn_ExportToExcel.Location = new System.Drawing.Point(13, 156);
            this.btn_ExportToExcel.Name = "btn_ExportToExcel";
            this.btn_ExportToExcel.Size = new System.Drawing.Size(102, 23);
            this.btn_ExportToExcel.TabIndex = 5;
            this.btn_ExportToExcel.Text = "Export To Excel";
            this.btn_ExportToExcel.UseVisualStyleBackColor = true;
            this.btn_ExportToExcel.Click += new System.EventHandler(this.btn_ExportToExcel_Click);
            // 
            // clb_TableList
            // 
            this.clb_TableList.CheckOnClick = true;
            this.clb_TableList.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clb_TableList.FormattingEnabled = true;
            this.clb_TableList.HorizontalScrollbar = true;
            this.clb_TableList.Location = new System.Drawing.Point(6, 85);
            this.clb_TableList.Name = "clb_TableList";
            this.clb_TableList.Size = new System.Drawing.Size(815, 650);
            this.clb_TableList.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_Find);
            this.groupBox1.Controls.Add(this.tb_Find);
            this.groupBox1.Controls.Add(this.cb_SkipEmptyTable);
            this.groupBox1.Controls.Add(this.cb_SelectAll);
            this.groupBox1.Controls.Add(this.clb_TableList);
            this.groupBox1.Location = new System.Drawing.Point(373, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(832, 741);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "List of all tables";
            // 
            // cb_SkipEmptyTable
            // 
            this.cb_SkipEmptyTable.AutoSize = true;
            this.cb_SkipEmptyTable.Location = new System.Drawing.Point(236, 19);
            this.cb_SkipEmptyTable.Name = "cb_SkipEmptyTable";
            this.cb_SkipEmptyTable.Size = new System.Drawing.Size(114, 17);
            this.cb_SkipEmptyTable.TabIndex = 7;
            this.cb_SkipEmptyTable.Text = "Skip Empty Tables";
            this.cb_SkipEmptyTable.UseVisualStyleBackColor = true;
            // 
            // cb_SelectAll
            // 
            this.cb_SelectAll.AutoSize = true;
            this.cb_SelectAll.Location = new System.Drawing.Point(6, 19);
            this.cb_SelectAll.Name = "cb_SelectAll";
            this.cb_SelectAll.Size = new System.Drawing.Size(70, 17);
            this.cb_SelectAll.TabIndex = 6;
            this.cb_SelectAll.Text = "Select All";
            this.cb_SelectAll.UseVisualStyleBackColor = true;
            this.cb_SelectAll.CheckedChanged += new System.EventHandler(this.cb_SelectAll_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.linkLabel1);
            this.groupBox3.Controls.Add(this.btn_ExportToExcel);
            this.groupBox3.Location = new System.Drawing.Point(12, 547);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(353, 201);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Path to save excel";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(10, 21);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(320, 122);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "D:/";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rb_TRG);
            this.groupBox4.Controls.Add(this.rb_SRC);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.cb_UserProfileList);
            this.groupBox4.Location = new System.Drawing.Point(12, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(355, 104);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "User Profile";
            // 
            // rb_SRC
            // 
            this.rb_SRC.AutoSize = true;
            this.rb_SRC.Checked = true;
            this.rb_SRC.Location = new System.Drawing.Point(14, 65);
            this.rb_SRC.Name = "rb_SRC";
            this.rb_SRC.Size = new System.Drawing.Size(74, 17);
            this.rb_SRC.TabIndex = 39;
            this.rb_SRC.TabStop = true;
            this.rb_SRC.Text = "Load SRC";
            this.rb_SRC.UseVisualStyleBackColor = true;
            this.rb_SRC.CheckedChanged += new System.EventHandler(this.rb_SRC_CheckedChanged);
            // 
            // rb_TRG
            // 
            this.rb_TRG.AutoSize = true;
            this.rb_TRG.Location = new System.Drawing.Point(124, 65);
            this.rb_TRG.Name = "rb_TRG";
            this.rb_TRG.Size = new System.Drawing.Size(83, 17);
            this.rb_TRG.TabIndex = 40;
            this.rb_TRG.Text = "Load Target";
            this.rb_TRG.UseVisualStyleBackColor = true;
            this.rb_TRG.CheckedChanged += new System.EventHandler(this.rb_TRG_CheckedChanged);
            // 
            // tb_Find
            // 
            this.tb_Find.Location = new System.Drawing.Point(48, 48);
            this.tb_Find.Name = "tb_Find";
            this.tb_Find.Size = new System.Drawing.Size(302, 20);
            this.tb_Find.TabIndex = 8;
            this.tb_Find.TextChanged += new System.EventHandler(this.tb_Find_TextChanged);
            // 
            // lbl_Find
            // 
            this.lbl_Find.AutoSize = true;
            this.lbl_Find.Location = new System.Drawing.Point(6, 51);
            this.lbl_Find.Name = "lbl_Find";
            this.lbl_Find.Size = new System.Drawing.Size(36, 13);
            this.lbl_Find.TabIndex = 9;
            this.lbl_Find.Text = "Find : ";
            // 
            // SQL_DB_To_Excel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 766);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SQL_DB_To_Excel";
            this.Text = "Export DataBase to Excel";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cb_windowsAuthentication;
        private System.Windows.Forms.TextBox tb_SRC_Password;
        private System.Windows.Forms.TextBox tb_SRC_UserName;
        private System.Windows.Forms.TextBox tb_SRC_DBName;
        private System.Windows.Forms.TextBox tb_SRC_DataSource;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_ExportToExcel;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.CheckedListBox clb_TableList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_SelectAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox tb_DefaultPort;
        private System.Windows.Forms.CheckBox cb_DefaultPort;
        private System.Windows.Forms.RadioButton rb_SQL;
        private System.Windows.Forms.RadioButton rb_Oracle;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox cb_SkipEmptyTable;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btn_Cancel;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cb_UserProfileList;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rb_TRG;
        private System.Windows.Forms.RadioButton rb_SRC;
        private System.Windows.Forms.Label lbl_Find;
        private System.Windows.Forms.TextBox tb_Find;
    }
}