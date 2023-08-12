namespace SPI_Data_Comp_Tool
{
    partial class PreMergingTool_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreMergingTool_Form));
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cb_UserProfileList = new System.Windows.Forms.ComboBox();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.gb_Source1 = new System.Windows.Forms.GroupBox();
            this.tb_SchemaName = new System.Windows.Forms.TextBox();
            this.cb_Schema = new System.Windows.Forms.CheckBox();
            this.cb_WindowAuthentication_SRC = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_SRC_DefaultPort = new System.Windows.Forms.TextBox();
            this.tb_SRC_UserName = new System.Windows.Forms.TextBox();
            this.cb_SRC_DefaultPort = new System.Windows.Forms.CheckBox();
            this.tb_SRC_Password = new System.Windows.Forms.TextBox();
            this.tb_SRC_DBName = new System.Windows.Forms.TextBox();
            this.tb_SRC_DataSource = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gb_Source = new System.Windows.Forms.GroupBox();
            this.rb_SRC_oracle = new System.Windows.Forms.RadioButton();
            this.rb_SRC_SQL = new System.Windows.Forms.RadioButton();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_ClearLogs = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_ListTableExcel = new System.Windows.Forms.Button();
            this.btn_Duplicate = new System.Windows.Forms.Button();
            this.lb_link = new System.Windows.Forms.Label();
            this.llb_File = new System.Windows.Forms.LinkLabel();
            this.cb_WithoutSpecialCharacters = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.groupBox12.SuspendLayout();
            this.gb_Source1.SuspendLayout();
            this.gb_Source.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label15);
            this.groupBox12.Controls.Add(this.cb_UserProfileList);
            this.groupBox12.Controls.Add(this.btn_Connect);
            this.groupBox12.Controls.Add(this.gb_Source1);
            this.groupBox12.Controls.Add(this.gb_Source);
            this.groupBox12.Location = new System.Drawing.Point(12, 12);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(393, 392);
            this.groupBox12.TabIndex = 29;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "DB Connection";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 13);
            this.label15.TabIndex = 36;
            this.label15.Text = "User Profile : ";
            // 
            // cb_UserProfileList
            // 
            this.cb_UserProfileList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_UserProfileList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_UserProfileList.FormattingEnabled = true;
            this.cb_UserProfileList.ItemHeight = 13;
            this.cb_UserProfileList.Location = new System.Drawing.Point(85, 21);
            this.cb_UserProfileList.Name = "cb_UserProfileList";
            this.cb_UserProfileList.Size = new System.Drawing.Size(299, 21);
            this.cb_UserProfileList.TabIndex = 35;
            this.cb_UserProfileList.SelectedIndexChanged += new System.EventHandler(this.cb_UserProfileList_SelectedIndexChanged);
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(146, 357);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(126, 23);
            this.btn_Connect.TabIndex = 25;
            this.btn_Connect.Text = "Test DB Connection";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // gb_Source1
            // 
            this.gb_Source1.Controls.Add(this.tb_SchemaName);
            this.gb_Source1.Controls.Add(this.cb_Schema);
            this.gb_Source1.Controls.Add(this.cb_WindowAuthentication_SRC);
            this.gb_Source1.Controls.Add(this.label7);
            this.gb_Source1.Controls.Add(this.tb_SRC_DefaultPort);
            this.gb_Source1.Controls.Add(this.tb_SRC_UserName);
            this.gb_Source1.Controls.Add(this.cb_SRC_DefaultPort);
            this.gb_Source1.Controls.Add(this.tb_SRC_Password);
            this.gb_Source1.Controls.Add(this.tb_SRC_DBName);
            this.gb_Source1.Controls.Add(this.tb_SRC_DataSource);
            this.gb_Source1.Controls.Add(this.label9);
            this.gb_Source1.Controls.Add(this.label5);
            this.gb_Source1.Controls.Add(this.label3);
            this.gb_Source1.Location = new System.Drawing.Point(18, 117);
            this.gb_Source1.Name = "gb_Source1";
            this.gb_Source1.Size = new System.Drawing.Size(363, 234);
            this.gb_Source1.TabIndex = 2;
            this.gb_Source1.TabStop = false;
            this.gb_Source1.Text = "Source : Oracle Connection and Settings";
            // 
            // tb_SchemaName
            // 
            this.tb_SchemaName.Enabled = false;
            this.tb_SchemaName.Location = new System.Drawing.Point(141, 140);
            this.tb_SchemaName.Name = "tb_SchemaName";
            this.tb_SchemaName.Size = new System.Drawing.Size(207, 20);
            this.tb_SchemaName.TabIndex = 32;
            // 
            // cb_Schema
            // 
            this.cb_Schema.AutoSize = true;
            this.cb_Schema.Location = new System.Drawing.Point(16, 142);
            this.cb_Schema.Name = "cb_Schema";
            this.cb_Schema.Size = new System.Drawing.Size(118, 17);
            this.cb_Schema.TabIndex = 31;
            this.cb_Schema.Text = "Use Schema Name";
            this.cb_Schema.UseVisualStyleBackColor = true;
            this.cb_Schema.CheckedChanged += new System.EventHandler(this.cb_Schema_CheckedChanged);
            // 
            // cb_WindowAuthentication_SRC
            // 
            this.cb_WindowAuthentication_SRC.AutoSize = true;
            this.cb_WindowAuthentication_SRC.Location = new System.Drawing.Point(16, 26);
            this.cb_WindowAuthentication_SRC.Name = "cb_WindowAuthentication_SRC";
            this.cb_WindowAuthentication_SRC.Size = new System.Drawing.Size(136, 17);
            this.cb_WindowAuthentication_SRC.TabIndex = 30;
            this.cb_WindowAuthentication_SRC.Text = "Window Authentication";
            this.cb_WindowAuthentication_SRC.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 175);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Username";
            // 
            // tb_SRC_DefaultPort
            // 
            this.tb_SRC_DefaultPort.Enabled = false;
            this.tb_SRC_DefaultPort.Location = new System.Drawing.Point(141, 53);
            this.tb_SRC_DefaultPort.Name = "tb_SRC_DefaultPort";
            this.tb_SRC_DefaultPort.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DefaultPort.TabIndex = 12;
            this.tb_SRC_DefaultPort.Text = "1521";
            // 
            // tb_SRC_UserName
            // 
            this.tb_SRC_UserName.Location = new System.Drawing.Point(141, 170);
            this.tb_SRC_UserName.Name = "tb_SRC_UserName";
            this.tb_SRC_UserName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_UserName.TabIndex = 7;
            // 
            // cb_SRC_DefaultPort
            // 
            this.cb_SRC_DefaultPort.AutoSize = true;
            this.cb_SRC_DefaultPort.Checked = true;
            this.cb_SRC_DefaultPort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SRC_DefaultPort.Location = new System.Drawing.Point(16, 55);
            this.cb_SRC_DefaultPort.Name = "cb_SRC_DefaultPort";
            this.cb_SRC_DefaultPort.Size = new System.Drawing.Size(104, 17);
            this.cb_SRC_DefaultPort.TabIndex = 11;
            this.cb_SRC_DefaultPort.Text = "Use Default Port";
            this.cb_SRC_DefaultPort.UseVisualStyleBackColor = true;
            this.cb_SRC_DefaultPort.CheckedChanged += new System.EventHandler(this.cb_Default_Port_CheckedChanged);
            // 
            // tb_SRC_Password
            // 
            this.tb_SRC_Password.Location = new System.Drawing.Point(141, 201);
            this.tb_SRC_Password.Name = "tb_SRC_Password";
            this.tb_SRC_Password.PasswordChar = '*';
            this.tb_SRC_Password.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_Password.TabIndex = 8;
            // 
            // tb_SRC_DBName
            // 
            this.tb_SRC_DBName.Location = new System.Drawing.Point(141, 110);
            this.tb_SRC_DBName.Name = "tb_SRC_DBName";
            this.tb_SRC_DBName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DBName.TabIndex = 6;
            // 
            // tb_SRC_DataSource
            // 
            this.tb_SRC_DataSource.Location = new System.Drawing.Point(141, 81);
            this.tb_SRC_DataSource.Name = "tb_SRC_DataSource";
            this.tb_SRC_DataSource.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DataSource.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 204);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "DB Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Data Source";
            // 
            // gb_Source
            // 
            this.gb_Source.Controls.Add(this.rb_SRC_oracle);
            this.gb_Source.Controls.Add(this.rb_SRC_SQL);
            this.gb_Source.Controls.Add(this.comboBox1);
            this.gb_Source.Location = new System.Drawing.Point(18, 66);
            this.gb_Source.Name = "gb_Source";
            this.gb_Source.Size = new System.Drawing.Size(363, 48);
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
            "10",
            "11",
            "12"});
            this.comboBox1.Location = new System.Drawing.Point(277, 15);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(67, 21);
            this.comboBox1.TabIndex = 20;
            this.comboBox1.Visible = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_ClearLogs);
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.rtb_Log);
            this.groupBox2.Controls.Add(this.lbl_Progress);
            this.groupBox2.Location = new System.Drawing.Point(12, 410);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1267, 451);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Logs";
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.Color.White;
            this.btn_ClearLogs.Location = new System.Drawing.Point(1174, 16);
            this.btn_ClearLogs.Name = "btn_ClearLogs";
            this.btn_ClearLogs.Size = new System.Drawing.Size(75, 23);
            this.btn_ClearLogs.TabIndex = 2;
            this.btn_ClearLogs.Text = "Clear Logs";
            this.btn_ClearLogs.UseVisualStyleBackColor = false;
            this.btn_ClearLogs.Click += new System.EventHandler(this.btn_ClearLogs_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 19);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1156, 15);
            this.progressBar1.TabIndex = 1;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.SystemColors.HighlightText;
            this.rtb_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_Log.Location = new System.Drawing.Point(12, 64);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(1237, 369);
            this.rtb_Log.TabIndex = 0;
            this.rtb_Log.Text = "";
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Progress.Location = new System.Drawing.Point(14, 40);
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Size = new System.Drawing.Size(63, 16);
            this.lbl_Progress.TabIndex = 0;
            this.lbl_Progress.Text = "Progress";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_ListTableExcel);
            this.groupBox1.Location = new System.Drawing.Point(16, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(574, 51);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pre Merging Excel File";
            // 
            // btn_ListTableExcel
            // 
            this.btn_ListTableExcel.Location = new System.Drawing.Point(12, 19);
            this.btn_ListTableExcel.Name = "btn_ListTableExcel";
            this.btn_ListTableExcel.Size = new System.Drawing.Size(164, 23);
            this.btn_ListTableExcel.TabIndex = 1;
            this.btn_ListTableExcel.Text = "Pre Merging Excel Process";
            this.btn_ListTableExcel.UseVisualStyleBackColor = true;
            this.btn_ListTableExcel.Click += new System.EventHandler(this.btn_ListTableExcel_Click);
            // 
            // btn_Duplicate
            // 
            this.btn_Duplicate.Location = new System.Drawing.Point(9, 78);
            this.btn_Duplicate.Name = "btn_Duplicate";
            this.btn_Duplicate.Size = new System.Drawing.Size(110, 23);
            this.btn_Duplicate.TabIndex = 4;
            this.btn_Duplicate.Text = "Check Duplicate";
            this.btn_Duplicate.UseVisualStyleBackColor = true;
            this.btn_Duplicate.Click += new System.EventHandler(this.btn_Duplicate_Click);
            // 
            // lb_link
            // 
            this.lb_link.AutoSize = true;
            this.lb_link.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_link.Location = new System.Drawing.Point(6, 21);
            this.lb_link.Name = "lb_link";
            this.lb_link.Size = new System.Drawing.Size(188, 18);
            this.lb_link.TabIndex = 3;
            this.lb_link.Text = "Click on link to open the file";
            this.lb_link.Visible = false;
            // 
            // llb_File
            // 
            this.llb_File.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llb_File.Location = new System.Drawing.Point(6, 55);
            this.llb_File.Name = "llb_File";
            this.llb_File.Size = new System.Drawing.Size(844, 153);
            this.llb_File.TabIndex = 2;
            this.llb_File.TabStop = true;
            this.llb_File.Text = "No File";
            this.llb_File.Visible = false;
            this.llb_File.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_File_LinkClicked);
            // 
            // cb_WithoutSpecialCharacters
            // 
            this.cb_WithoutSpecialCharacters.AutoSize = true;
            this.cb_WithoutSpecialCharacters.Location = new System.Drawing.Point(9, 23);
            this.cb_WithoutSpecialCharacters.Name = "cb_WithoutSpecialCharacters";
            this.cb_WithoutSpecialCharacters.Size = new System.Drawing.Size(215, 17);
            this.cb_WithoutSpecialCharacters.TabIndex = 5;
            this.cb_WithoutSpecialCharacters.Text = "Check By Removing Special Characters";
            this.cb_WithoutSpecialCharacters.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.llb_File);
            this.groupBox3.Controls.Add(this.lb_link);
            this.groupBox3.Location = new System.Drawing.Point(411, 184);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(868, 220);
            this.groupBox3.TabIndex = 36;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cb_IgnoreCase);
            this.groupBox4.Controls.Add(this.cb_WithoutSpecialCharacters);
            this.groupBox4.Controls.Add(this.btn_Duplicate);
            this.groupBox4.Location = new System.Drawing.Point(12, 10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(242, 117);
            this.groupBox4.TabIndex = 37;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Check Duplicacy in Single Domain";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(411, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(868, 159);
            this.tabControl1.TabIndex = 38;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(860, 96);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Pre Merging";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(860, 133);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Check Duplicacy";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Location = new System.Drawing.Point(9, 48);
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.Size = new System.Drawing.Size(83, 17);
            this.cb_IgnoreCase.TabIndex = 6;
            this.cb_IgnoreCase.Text = "Ignore Case";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            // 
            // PreMergingTool_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1286, 872);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox12);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PreMergingTool_Form";
            this.Text = "Pre Merging Tool Form";
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.gb_Source1.ResumeLayout(false);
            this.gb_Source1.PerformLayout();
            this.gb_Source.ResumeLayout(false);
            this.gb_Source.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.GroupBox gb_Source1;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_SRC;
        private System.Windows.Forms.TextBox tb_SRC_DefaultPort;
        private System.Windows.Forms.CheckBox cb_SRC_DefaultPort;
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
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_ClearLogs;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Label lbl_Progress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_ListTableExcel;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.LinkLabel llb_File;
        private System.Windows.Forms.Label lb_link;
        private System.Windows.Forms.TextBox tb_SchemaName;
        private System.Windows.Forms.CheckBox cb_Schema;
        private System.Windows.Forms.Button btn_Duplicate;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cb_UserProfileList;
        private System.Windows.Forms.CheckBox cb_WithoutSpecialCharacters;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
    }
}