namespace SPI_Data_Comp_Tool
{
    partial class Compare_Multiples_List_Tables_Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Compare_Multiples_List_Tables_Forms));
            this.gb_DBConnection = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cb_UserProfileList = new System.Windows.Forms.ComboBox();
            this.btn_TestDBConnection = new System.Windows.Forms.Button();
            this.btn_TRG_SRC = new System.Windows.Forms.Button();
            this.btn_Copy_SRC_TRG = new System.Windows.Forms.Button();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.gb_Source1 = new System.Windows.Forms.GroupBox();
            this.cb_WindowAuthentication_SRC = new System.Windows.Forms.CheckBox();
            this.tb_SRC_DefaultPort = new System.Windows.Forms.TextBox();
            this.cb_SRC_DefaultPort = new System.Windows.Forms.CheckBox();
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
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.gb_Target1 = new System.Windows.Forms.GroupBox();
            this.tb_TRG_DefaultPort = new System.Windows.Forms.TextBox();
            this.cb_TRG_DefaultPort = new System.Windows.Forms.CheckBox();
            this.cb_WindowAuthentication_TRG = new System.Windows.Forms.CheckBox();
            this.tb_TRG_Password = new System.Windows.Forms.TextBox();
            this.tb_TRG_UserName = new System.Windows.Forms.TextBox();
            this.tb_TRG_DBName = new System.Windows.Forms.TextBox();
            this.tb_TRG_DataSource = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gb_Target = new System.Windows.Forms.GroupBox();
            this.rb_TRG_Oracle = new System.Windows.Forms.RadioButton();
            this.rb_TRG_SQL = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cb_IncludeDiff_Columns = new System.Windows.Forms.CheckBox();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.l_lb_Output_Set = new System.Windows.Forms.LinkLabel();
            this.l_lbl_outputFolder = new System.Windows.Forms.LinkLabel();
            this.cb_IncludeRule = new System.Windows.Forms.CheckBox();
            this.btn_RuleForm = new System.Windows.Forms.Button();
            this.btn_Compare = new System.Windows.Forms.Button();
            this.btn_ListTableExcel = new System.Windows.Forms.Button();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.gb_Logs = new System.Windows.Forms.GroupBox();
            this.btn_ClearLogs = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.gb_Excel = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.llb_Excel_TRG = new System.Windows.Forms.LinkLabel();
            this.llb_Excel_SRC = new System.Windows.Forms.LinkLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.bgw_List = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_DB = new System.Windows.Forms.TabPage();
            this.tabPage_Excel = new System.Windows.Forms.TabPage();
            this.cb_PK_As_ReferenceKey = new System.Windows.Forms.CheckBox();
            this.gb_DBConnection.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.gb_Source1.SuspendLayout();
            this.gb_Source.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.gb_Target1.SuspendLayout();
            this.gb_Target.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gb_Logs.SuspendLayout();
            this.gb_Excel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage_DB.SuspendLayout();
            this.tabPage_Excel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_DBConnection
            // 
            this.gb_DBConnection.Controls.Add(this.groupBox5);
            this.gb_DBConnection.Controls.Add(this.btn_TestDBConnection);
            this.gb_DBConnection.Controls.Add(this.btn_TRG_SRC);
            this.gb_DBConnection.Controls.Add(this.btn_Copy_SRC_TRG);
            this.gb_DBConnection.Controls.Add(this.groupBox12);
            this.gb_DBConnection.Controls.Add(this.groupBox13);
            this.gb_DBConnection.Location = new System.Drawing.Point(16, 13);
            this.gb_DBConnection.Name = "gb_DBConnection";
            this.gb_DBConnection.Size = new System.Drawing.Size(773, 409);
            this.gb_DBConnection.TabIndex = 31;
            this.gb_DBConnection.TabStop = false;
            this.gb_DBConnection.Text = "DataBase Connection Settings";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.cb_UserProfileList);
            this.groupBox5.Location = new System.Drawing.Point(17, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(746, 47);
            this.groupBox5.TabIndex = 37;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "User Profile";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(88, 16);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(103, 13);
            this.label15.TabIndex = 36;
            this.label15.Text = "Select User Profile : ";
            // 
            // cb_UserProfileList
            // 
            this.cb_UserProfileList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_UserProfileList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_UserProfileList.FormattingEnabled = true;
            this.cb_UserProfileList.ItemHeight = 13;
            this.cb_UserProfileList.Location = new System.Drawing.Point(207, 15);
            this.cb_UserProfileList.Name = "cb_UserProfileList";
            this.cb_UserProfileList.Size = new System.Drawing.Size(299, 21);
            this.cb_UserProfileList.TabIndex = 35;
            this.cb_UserProfileList.SelectedIndexChanged += new System.EventHandler(this.cb_UserProfileList_SelectedIndexChanged);
            // 
            // btn_TestDBConnection
            // 
            this.btn_TestDBConnection.Location = new System.Drawing.Point(320, 380);
            this.btn_TestDBConnection.Name = "btn_TestDBConnection";
            this.btn_TestDBConnection.Size = new System.Drawing.Size(150, 23);
            this.btn_TestDBConnection.TabIndex = 33;
            this.btn_TestDBConnection.Text = "Test Both DB Connection";
            this.btn_TestDBConnection.UseVisualStyleBackColor = true;
            this.btn_TestDBConnection.Click += new System.EventHandler(this.btn_TestDBConnection_Click);
            // 
            // btn_TRG_SRC
            // 
            this.btn_TRG_SRC.Location = new System.Drawing.Point(647, 380);
            this.btn_TRG_SRC.Name = "btn_TRG_SRC";
            this.btn_TRG_SRC.Size = new System.Drawing.Size(75, 23);
            this.btn_TRG_SRC.TabIndex = 32;
            this.btn_TRG_SRC.Text = "<<< Copy";
            this.btn_TRG_SRC.UseVisualStyleBackColor = true;
            this.btn_TRG_SRC.Click += new System.EventHandler(this.btn_TRG_SRC_Click);
            // 
            // btn_Copy_SRC_TRG
            // 
            this.btn_Copy_SRC_TRG.Location = new System.Drawing.Point(163, 381);
            this.btn_Copy_SRC_TRG.Name = "btn_Copy_SRC_TRG";
            this.btn_Copy_SRC_TRG.Size = new System.Drawing.Size(75, 23);
            this.btn_Copy_SRC_TRG.TabIndex = 31;
            this.btn_Copy_SRC_TRG.Text = "Copy >>>";
            this.btn_Copy_SRC_TRG.UseVisualStyleBackColor = true;
            this.btn_Copy_SRC_TRG.Click += new System.EventHandler(this.btn_Copy_SRC_TRG_Click);
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.gb_Source1);
            this.groupBox12.Controls.Add(this.gb_Source);
            this.groupBox12.Location = new System.Drawing.Point(17, 70);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(370, 305);
            this.groupBox12.TabIndex = 28;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Source";
            // 
            // gb_Source1
            // 
            this.gb_Source1.Controls.Add(this.cb_WindowAuthentication_SRC);
            this.gb_Source1.Controls.Add(this.tb_SRC_DefaultPort);
            this.gb_Source1.Controls.Add(this.cb_SRC_DefaultPort);
            this.gb_Source1.Controls.Add(this.tb_SRC_Password);
            this.gb_Source1.Controls.Add(this.tb_SRC_UserName);
            this.gb_Source1.Controls.Add(this.tb_SRC_DBName);
            this.gb_Source1.Controls.Add(this.tb_SRC_DataSource);
            this.gb_Source1.Controls.Add(this.label9);
            this.gb_Source1.Controls.Add(this.label7);
            this.gb_Source1.Controls.Add(this.label5);
            this.gb_Source1.Controls.Add(this.label3);
            this.gb_Source1.Location = new System.Drawing.Point(6, 80);
            this.gb_Source1.Name = "gb_Source1";
            this.gb_Source1.Size = new System.Drawing.Size(354, 213);
            this.gb_Source1.TabIndex = 2;
            this.gb_Source1.TabStop = false;
            this.gb_Source1.Text = "Source : Oracle Connection and Settings";
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
            this.cb_WindowAuthentication_SRC.CheckedChanged += new System.EventHandler(this.cb_WindowAuthentication_SRC_CheckedChanged);
            // 
            // tb_SRC_DefaultPort
            // 
            this.tb_SRC_DefaultPort.Enabled = false;
            this.tb_SRC_DefaultPort.Location = new System.Drawing.Point(128, 53);
            this.tb_SRC_DefaultPort.Name = "tb_SRC_DefaultPort";
            this.tb_SRC_DefaultPort.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DefaultPort.TabIndex = 12;
            this.tb_SRC_DefaultPort.Text = "1521";
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
            this.tb_SRC_Password.Location = new System.Drawing.Point(128, 183);
            this.tb_SRC_Password.Name = "tb_SRC_Password";
            this.tb_SRC_Password.PasswordChar = '*';
            this.tb_SRC_Password.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_Password.TabIndex = 8;
            // 
            // tb_SRC_UserName
            // 
            this.tb_SRC_UserName.Location = new System.Drawing.Point(128, 148);
            this.tb_SRC_UserName.Name = "tb_SRC_UserName";
            this.tb_SRC_UserName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_UserName.TabIndex = 7;
            // 
            // tb_SRC_DBName
            // 
            this.tb_SRC_DBName.Location = new System.Drawing.Point(128, 115);
            this.tb_SRC_DBName.Name = "tb_SRC_DBName";
            this.tb_SRC_DBName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DBName.TabIndex = 6;
            // 
            // tb_SRC_DataSource
            // 
            this.tb_SRC_DataSource.Location = new System.Drawing.Point(128, 84);
            this.tb_SRC_DataSource.Name = "tb_SRC_DataSource";
            this.tb_SRC_DataSource.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DataSource.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 187);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Password";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Username";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "DB Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Data Source";
            // 
            // gb_Source
            // 
            this.gb_Source.Controls.Add(this.rb_SRC_oracle);
            this.gb_Source.Controls.Add(this.rb_SRC_SQL);
            this.gb_Source.Location = new System.Drawing.Point(6, 19);
            this.gb_Source.Name = "gb_Source";
            this.gb_Source.Size = new System.Drawing.Size(350, 48);
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
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.gb_Target1);
            this.groupBox13.Controls.Add(this.gb_Target);
            this.groupBox13.Location = new System.Drawing.Point(392, 70);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(371, 305);
            this.groupBox13.TabIndex = 29;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Target";
            // 
            // gb_Target1
            // 
            this.gb_Target1.Controls.Add(this.tb_TRG_DefaultPort);
            this.gb_Target1.Controls.Add(this.cb_TRG_DefaultPort);
            this.gb_Target1.Controls.Add(this.cb_WindowAuthentication_TRG);
            this.gb_Target1.Controls.Add(this.tb_TRG_Password);
            this.gb_Target1.Controls.Add(this.tb_TRG_UserName);
            this.gb_Target1.Controls.Add(this.tb_TRG_DBName);
            this.gb_Target1.Controls.Add(this.tb_TRG_DataSource);
            this.gb_Target1.Controls.Add(this.label8);
            this.gb_Target1.Controls.Add(this.label6);
            this.gb_Target1.Controls.Add(this.label4);
            this.gb_Target1.Controls.Add(this.label2);
            this.gb_Target1.Location = new System.Drawing.Point(8, 80);
            this.gb_Target1.Name = "gb_Target1";
            this.gb_Target1.Size = new System.Drawing.Size(354, 213);
            this.gb_Target1.TabIndex = 3;
            this.gb_Target1.TabStop = false;
            this.gb_Target1.Text = "Target : SQL Connection and Settings";
            // 
            // tb_TRG_DefaultPort
            // 
            this.tb_TRG_DefaultPort.Enabled = false;
            this.tb_TRG_DefaultPort.Location = new System.Drawing.Point(130, 48);
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
            this.cb_TRG_DefaultPort.Location = new System.Drawing.Point(19, 50);
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
            this.cb_WindowAuthentication_TRG.Location = new System.Drawing.Point(20, 24);
            this.cb_WindowAuthentication_TRG.Name = "cb_WindowAuthentication_TRG";
            this.cb_WindowAuthentication_TRG.Size = new System.Drawing.Size(141, 17);
            this.cb_WindowAuthentication_TRG.TabIndex = 12;
            this.cb_WindowAuthentication_TRG.Text = "Windows Authentication";
            this.cb_WindowAuthentication_TRG.UseVisualStyleBackColor = true;
            this.cb_WindowAuthentication_TRG.CheckedChanged += new System.EventHandler(this.cb_WindowAuthentication_TRG_CheckedChanged);
            // 
            // tb_TRG_Password
            // 
            this.tb_TRG_Password.Location = new System.Drawing.Point(130, 182);
            this.tb_TRG_Password.Name = "tb_TRG_Password";
            this.tb_TRG_Password.PasswordChar = '*';
            this.tb_TRG_Password.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_Password.TabIndex = 9;
            // 
            // tb_TRG_UserName
            // 
            this.tb_TRG_UserName.Location = new System.Drawing.Point(130, 148);
            this.tb_TRG_UserName.Name = "tb_TRG_UserName";
            this.tb_TRG_UserName.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_UserName.TabIndex = 8;
            // 
            // tb_TRG_DBName
            // 
            this.tb_TRG_DBName.Location = new System.Drawing.Point(130, 115);
            this.tb_TRG_DBName.Name = "tb_TRG_DBName";
            this.tb_TRG_DBName.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_DBName.TabIndex = 7;
            // 
            // tb_TRG_DataSource
            // 
            this.tb_TRG_DataSource.Location = new System.Drawing.Point(130, 81);
            this.tb_TRG_DataSource.Name = "tb_TRG_DataSource";
            this.tb_TRG_DataSource.Size = new System.Drawing.Size(207, 20);
            this.tb_TRG_DataSource.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 183);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Username";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "DB Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Data Source";
            // 
            // gb_Target
            // 
            this.gb_Target.Controls.Add(this.rb_TRG_Oracle);
            this.gb_Target.Controls.Add(this.rb_TRG_SQL);
            this.gb_Target.Location = new System.Drawing.Point(8, 19);
            this.gb_Target.Name = "gb_Target";
            this.gb_Target.Size = new System.Drawing.Size(354, 48);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_PK_As_ReferenceKey);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.cb_IncludeDiff_Columns);
            this.groupBox1.Controls.Add(this.cb_IgnoreCase);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.cb_IncludeRule);
            this.groupBox1.Controls.Add(this.btn_RuleForm);
            this.groupBox1.Controls.Add(this.btn_Compare);
            this.groupBox1.Controls.Add(this.btn_ListTableExcel);
            this.groupBox1.Location = new System.Drawing.Point(832, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(367, 464);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Load Table List Excel File";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(12, 421);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 37;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cb_IncludeDiff_Columns
            // 
            this.cb_IncludeDiff_Columns.AutoSize = true;
            this.cb_IncludeDiff_Columns.Checked = true;
            this.cb_IncludeDiff_Columns.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_IncludeDiff_Columns.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IncludeDiff_Columns.Location = new System.Drawing.Point(12, 322);
            this.cb_IncludeDiff_Columns.Name = "cb_IncludeDiff_Columns";
            this.cb_IncludeDiff_Columns.Size = new System.Drawing.Size(207, 20);
            this.cb_IncludeDiff_Columns.TabIndex = 18;
            this.cb_IncludeDiff_Columns.Text = "Include Only Different Columns";
            this.cb_IncludeDiff_Columns.UseVisualStyleBackColor = true;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IgnoreCase.Location = new System.Drawing.Point(12, 345);
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.Size = new System.Drawing.Size(100, 20);
            this.cb_IgnoreCase.TabIndex = 17;
            this.cb_IgnoreCase.Text = "Ignore Case";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.l_lb_Output_Set);
            this.groupBox4.Controls.Add(this.l_lbl_outputFolder);
            this.groupBox4.Location = new System.Drawing.Point(12, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(336, 273);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output Folder";
            // 
            // l_lb_Output_Set
            // 
            this.l_lb_Output_Set.AutoSize = true;
            this.l_lb_Output_Set.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_lb_Output_Set.Location = new System.Drawing.Point(9, 18);
            this.l_lb_Output_Set.Name = "l_lb_Output_Set";
            this.l_lb_Output_Set.Size = new System.Drawing.Size(113, 16);
            this.l_lb_Output_Set.TabIndex = 1;
            this.l_lb_Output_Set.TabStop = true;
            this.l_lb_Output_Set.Text = "Click Here To Set";
            this.l_lb_Output_Set.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l_lb_Output_Set_LinkClicked);
            // 
            // l_lbl_outputFolder
            // 
            this.l_lbl_outputFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_lbl_outputFolder.Location = new System.Drawing.Point(8, 41);
            this.l_lbl_outputFolder.Name = "l_lbl_outputFolder";
            this.l_lbl_outputFolder.Size = new System.Drawing.Size(322, 211);
            this.l_lbl_outputFolder.TabIndex = 0;
            this.l_lbl_outputFolder.TabStop = true;
            this.l_lbl_outputFolder.Text = "No Folder Selected";
            this.l_lbl_outputFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l_lbl_outputFolder_LinkClicked);
            // 
            // cb_IncludeRule
            // 
            this.cb_IncludeRule.AutoSize = true;
            this.cb_IncludeRule.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_IncludeRule.Location = new System.Drawing.Point(12, 368);
            this.cb_IncludeRule.Name = "cb_IncludeRule";
            this.cb_IncludeRule.Size = new System.Drawing.Size(108, 20);
            this.cb_IncludeRule.TabIndex = 16;
            this.cb_IncludeRule.Text = "Include Rules";
            this.cb_IncludeRule.UseVisualStyleBackColor = true;
            // 
            // btn_RuleForm
            // 
            this.btn_RuleForm.Location = new System.Drawing.Point(127, 366);
            this.btn_RuleForm.Name = "btn_RuleForm";
            this.btn_RuleForm.Size = new System.Drawing.Size(121, 23);
            this.btn_RuleForm.TabIndex = 15;
            this.btn_RuleForm.Text = "Open Rule Form";
            this.btn_RuleForm.UseVisualStyleBackColor = true;
            this.btn_RuleForm.Click += new System.EventHandler(this.btn_RuleForm_Click);
            // 
            // btn_Compare
            // 
            this.btn_Compare.Location = new System.Drawing.Point(156, 392);
            this.btn_Compare.Name = "btn_Compare";
            this.btn_Compare.Size = new System.Drawing.Size(203, 23);
            this.btn_Compare.TabIndex = 2;
            this.btn_Compare.Text = "Start Compare and Generate Excel";
            this.btn_Compare.UseVisualStyleBackColor = true;
            this.btn_Compare.Click += new System.EventHandler(this.btn_Compare_Click);
            // 
            // btn_ListTableExcel
            // 
            this.btn_ListTableExcel.Location = new System.Drawing.Point(12, 392);
            this.btn_ListTableExcel.Name = "btn_ListTableExcel";
            this.btn_ListTableExcel.Size = new System.Drawing.Size(138, 23);
            this.btn_ListTableExcel.TabIndex = 1;
            this.btn_ListTableExcel.Text = "Browse List Table Excel";
            this.btn_ListTableExcel.UseVisualStyleBackColor = true;
            this.btn_ListTableExcel.Click += new System.EventHandler(this.btn_ListTableExcel_Click);
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
            // gb_Logs
            // 
            this.gb_Logs.Controls.Add(this.btn_ClearLogs);
            this.gb_Logs.Controls.Add(this.progressBar1);
            this.gb_Logs.Controls.Add(this.rtb_Log);
            this.gb_Logs.Controls.Add(this.lbl_Progress);
            this.gb_Logs.Location = new System.Drawing.Point(12, 488);
            this.gb_Logs.Name = "gb_Logs";
            this.gb_Logs.Size = new System.Drawing.Size(1187, 449);
            this.gb_Logs.TabIndex = 33;
            this.gb_Logs.TabStop = false;
            this.gb_Logs.Text = "Logs";
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.Color.White;
            this.btn_ClearLogs.Location = new System.Drawing.Point(1104, 15);
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
            this.progressBar1.Size = new System.Drawing.Size(1086, 15);
            this.progressBar1.TabIndex = 1;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_Log.Location = new System.Drawing.Point(17, 59);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(1162, 374);
            this.rtb_Log.TabIndex = 0;
            this.rtb_Log.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1205, 771);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1205, 741);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gb_Excel
            // 
            this.gb_Excel.Controls.Add(this.label10);
            this.gb_Excel.Controls.Add(this.label1);
            this.gb_Excel.Controls.Add(this.llb_Excel_TRG);
            this.gb_Excel.Controls.Add(this.llb_Excel_SRC);
            this.gb_Excel.Location = new System.Drawing.Point(15, 15);
            this.gb_Excel.Name = "gb_Excel";
            this.gb_Excel.Size = new System.Drawing.Size(773, 421);
            this.gb_Excel.TabIndex = 35;
            this.gb_Excel.TabStop = false;
            this.gb_Excel.Text = "Excel Selection";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 221);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(128, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Select Target Folder Path";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select Source Folder Path";
            // 
            // llb_Excel_TRG
            // 
            this.llb_Excel_TRG.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llb_Excel_TRG.Location = new System.Drawing.Point(6, 245);
            this.llb_Excel_TRG.Name = "llb_Excel_TRG";
            this.llb_Excel_TRG.Size = new System.Drawing.Size(746, 161);
            this.llb_Excel_TRG.TabIndex = 1;
            this.llb_Excel_TRG.TabStop = true;
            this.llb_Excel_TRG.Text = "No Target Folder Selected";
            this.llb_Excel_TRG.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Excel_TRG_LinkClicked);
            // 
            // llb_Excel_SRC
            // 
            this.llb_Excel_SRC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llb_Excel_SRC.Location = new System.Drawing.Point(10, 46);
            this.llb_Excel_SRC.Name = "llb_Excel_SRC";
            this.llb_Excel_SRC.Size = new System.Drawing.Size(746, 159);
            this.llb_Excel_SRC.TabIndex = 0;
            this.llb_Excel_SRC.TabStop = true;
            this.llb_Excel_SRC.Text = "No Source Folder Selected";
            this.llb_Excel_SRC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Excel_SRC_LinkClicked);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select Folder To save Output Files";
            this.folderBrowserDialog1.SelectedPath = "D:\\Krishna\\Output";
            // 
            // bgw_List
            // 
            this.bgw_List.WorkerReportsProgress = true;
            this.bgw_List.WorkerSupportsCancellation = true;
            this.bgw_List.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_List_DoWork);
            this.bgw_List.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_List_ProgressChanged);
            this.bgw_List.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_List_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_DB);
            this.tabControl1.Controls.Add(this.tabPage_Excel);
            this.tabControl1.Location = new System.Drawing.Point(12, 14);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(813, 468);
            this.tabControl1.TabIndex = 36;
            // 
            // tabPage_DB
            // 
            this.tabPage_DB.Controls.Add(this.gb_DBConnection);
            this.tabPage_DB.Location = new System.Drawing.Point(4, 22);
            this.tabPage_DB.Name = "tabPage_DB";
            this.tabPage_DB.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_DB.Size = new System.Drawing.Size(805, 442);
            this.tabPage_DB.TabIndex = 0;
            this.tabPage_DB.Text = "DB";
            this.tabPage_DB.UseVisualStyleBackColor = true;
            // 
            // tabPage_Excel
            // 
            this.tabPage_Excel.Controls.Add(this.gb_Excel);
            this.tabPage_Excel.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Excel.Name = "tabPage_Excel";
            this.tabPage_Excel.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Excel.Size = new System.Drawing.Size(805, 442);
            this.tabPage_Excel.TabIndex = 1;
            this.tabPage_Excel.Text = "Excel";
            this.tabPage_Excel.UseVisualStyleBackColor = true;
            this.tabPage_Excel.Click += new System.EventHandler(this.tabPage_Excel_Click);
            // 
            // cb_PK_As_ReferenceKey
            // 
            this.cb_PK_As_ReferenceKey.AutoSize = true;
            this.cb_PK_As_ReferenceKey.Checked = true;
            this.cb_PK_As_ReferenceKey.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_PK_As_ReferenceKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_PK_As_ReferenceKey.Location = new System.Drawing.Point(12, 298);
            this.cb_PK_As_ReferenceKey.Name = "cb_PK_As_ReferenceKey";
            this.cb_PK_As_ReferenceKey.Size = new System.Drawing.Size(213, 20);
            this.cb_PK_As_ReferenceKey.TabIndex = 38;
            this.cb_PK_As_ReferenceKey.Text = "if Reference Key is Primary Key";
            this.cb_PK_As_ReferenceKey.UseVisualStyleBackColor = true;
            // 
            // Compare_Multiples_List_Tables_Forms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1216, 967);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gb_Logs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Compare_Multiples_List_Tables_Forms";
            this.Text = "Compare_Multiples_List_Tables_Forms";
            this.gb_DBConnection.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.gb_Source1.ResumeLayout(false);
            this.gb_Source1.PerformLayout();
            this.gb_Source.ResumeLayout(false);
            this.gb_Source.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.gb_Target1.ResumeLayout(false);
            this.gb_Target1.PerformLayout();
            this.gb_Target.ResumeLayout(false);
            this.gb_Target.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.gb_Logs.ResumeLayout(false);
            this.gb_Logs.PerformLayout();
            this.gb_Excel.ResumeLayout(false);
            this.gb_Excel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage_DB.ResumeLayout(false);
            this.tabPage_Excel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_DBConnection;
        private System.Windows.Forms.Button btn_TRG_SRC;
        private System.Windows.Forms.Button btn_Copy_SRC_TRG;
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
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.GroupBox gb_Target1;
        private System.Windows.Forms.TextBox tb_TRG_DefaultPort;
        private System.Windows.Forms.CheckBox cb_TRG_DefaultPort;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_TRG;
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_ListTableExcel;
        private System.Windows.Forms.Label lbl_Progress;
        private System.Windows.Forms.GroupBox gb_Logs;
        private System.Windows.Forms.Button btn_Compare;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox gb_Excel;
        private System.Windows.Forms.LinkLabel llb_Excel_TRG;
        private System.Windows.Forms.LinkLabel llb_Excel_SRC;
        private System.Windows.Forms.Button btn_ClearLogs;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_TestDBConnection;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.LinkLabel l_lbl_outputFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cb_UserProfileList;
        private System.Windows.Forms.LinkLabel l_lb_Output_Set;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox cb_IncludeDiff_Columns;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
        private System.Windows.Forms.CheckBox cb_IncludeRule;
        private System.Windows.Forms.Button btn_RuleForm;
        private System.ComponentModel.BackgroundWorker bgw_List;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_DB;
        private System.Windows.Forms.TabPage tabPage_Excel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cb_PK_As_ReferenceKey;
    }
}