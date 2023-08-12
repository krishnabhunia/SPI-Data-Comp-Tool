namespace SPI_Data_Comp_Tool
{
    partial class DBConnection
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cb_UserProfileList = new System.Windows.Forms.ComboBox();
            this.cb_FullName = new System.Windows.Forms.CheckBox();
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
            this.groupBox14.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.gb_Source1.SuspendLayout();
            this.gb_Source.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.gb_Target1.SuspendLayout();
            this.gb_Target.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.label15);
            this.groupBox14.Controls.Add(this.cb_UserProfileList);
            this.groupBox14.Controls.Add(this.cb_FullName);
            this.groupBox14.Controls.Add(this.groupBox12);
            this.groupBox14.Controls.Add(this.groupBox13);
            this.groupBox14.Location = new System.Drawing.Point(10, 11);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(799, 392);
            this.groupBox14.TabIndex = 31;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Connection Settings";
            this.groupBox14.Enter += new System.EventHandler(this.groupBox14_Enter);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(284, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 13);
            this.label15.TabIndex = 34;
            this.label15.Text = "User Profile : ";
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
            this.cb_UserProfileList.SelectedIndexChanged += new System.EventHandler(this.cb_UserProfileList_SelectedIndexChanged);
            // 
            // cb_FullName
            // 
            this.cb_FullName.AutoSize = true;
            this.cb_FullName.Location = new System.Drawing.Point(308, 370);
            this.cb_FullName.Name = "cb_FullName";
            this.cb_FullName.Size = new System.Drawing.Size(165, 17);
            this.cb_FullName.TabIndex = 30;
            this.cb_FullName.Text = "Show FullName in DropDown";
            this.cb_FullName.UseVisualStyleBackColor = true;
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
            // 
            // btn_Copy_SRC_TRG
            // 
            this.btn_Copy_SRC_TRG.Location = new System.Drawing.Point(137, 289);
            this.btn_Copy_SRC_TRG.Name = "btn_Copy_SRC_TRG";
            this.btn_Copy_SRC_TRG.Size = new System.Drawing.Size(75, 23);
            this.btn_Copy_SRC_TRG.TabIndex = 31;
            this.btn_Copy_SRC_TRG.Text = "Copy >>>";
            this.btn_Copy_SRC_TRG.UseVisualStyleBackColor = true;
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
            // 
            // DBConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox14);
            this.Name = "DBConnection";
            this.Size = new System.Drawing.Size(819, 412);
            this.Load += new System.EventHandler(this.DBConnection_Load);
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cb_UserProfileList;
        private System.Windows.Forms.CheckBox cb_FullName;
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
        private System.Windows.Forms.Button btn_Copy_SRC_TRG;
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
        private System.Windows.Forms.Button btn_TRG_SRC;
        private System.Windows.Forms.GroupBox gb_Target;
        private System.Windows.Forms.RadioButton rb_TRG_Oracle;
        private System.Windows.Forms.RadioButton rb_TRG_SQL;
    }
}
