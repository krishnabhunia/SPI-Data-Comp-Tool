namespace SPI_Data_Comp_Tool
{
    partial class OracleAndSQLConnection
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
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.rb_SRC_oracle = new System.Windows.Forms.RadioButton();
            this.rb_SRC_SQL = new System.Windows.Forms.RadioButton();
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            this.btn_Connect_Reference_ID = new System.Windows.Forms.Button();
            this.cb_UserProfileList = new System.Windows.Forms.ComboBox();
            this.lbl_Oracle_SQL_Connection = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.cb_query = new System.Windows.Forms.CheckBox();
            this.tb_querybox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox8.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.rb_SRC_oracle);
            this.groupBox8.Controls.Add(this.rb_SRC_SQL);
            this.groupBox8.Location = new System.Drawing.Point(15, 14);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(350, 48);
            this.groupBox8.TabIndex = 27;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Select Source Database Platform : ";
            // 
            // rb_SRC_oracle
            // 
            this.rb_SRC_oracle.AutoSize = true;
            this.rb_SRC_oracle.Checked = true;
            this.rb_SRC_oracle.Location = new System.Drawing.Point(6, 20);
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
            // groupBox2
            // 
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
            this.groupBox2.Location = new System.Drawing.Point(384, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(354, 209);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target : SQL Connection and Settings";
            // 
            // tb_destination_tableName
            // 
            this.tb_destination_tableName.Location = new System.Drawing.Point(127, 167);
            this.tb_destination_tableName.Name = "tb_destination_tableName";
            this.tb_destination_tableName.Size = new System.Drawing.Size(207, 20);
            this.tb_destination_tableName.TabIndex = 11;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(14, 170);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(106, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "Scheme/table name:";
            // 
            // tb_sql_pwd
            // 
            this.tb_sql_pwd.Location = new System.Drawing.Point(127, 133);
            this.tb_sql_pwd.Name = "tb_sql_pwd";
            this.tb_sql_pwd.PasswordChar = '*';
            this.tb_sql_pwd.Size = new System.Drawing.Size(207, 20);
            this.tb_sql_pwd.TabIndex = 9;
            // 
            // tb_sql_username
            // 
            this.tb_sql_username.Location = new System.Drawing.Point(127, 99);
            this.tb_sql_username.Name = "tb_sql_username";
            this.tb_sql_username.Size = new System.Drawing.Size(207, 20);
            this.tb_sql_username.TabIndex = 8;
            // 
            // tb_sql_dbName
            // 
            this.tb_sql_dbName.Location = new System.Drawing.Point(127, 66);
            this.tb_sql_dbName.Name = "tb_sql_dbName";
            this.tb_sql_dbName.Size = new System.Drawing.Size(207, 20);
            this.tb_sql_dbName.TabIndex = 7;
            // 
            // tb_sql_dsource
            // 
            this.tb_sql_dsource.Location = new System.Drawing.Point(127, 32);
            this.tb_sql_dsource.Name = "tb_sql_dsource";
            this.tb_sql_dsource.Size = new System.Drawing.Size(207, 20);
            this.tb_sql_dsource.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 134);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Username";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "DB Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Data Source";
            // 
            // groupBox1
            // 
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
            this.groupBox1.Location = new System.Drawing.Point(11, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 209);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source : Oracle Connection and Settings";
            // 
            // tb_Source_tableName
            // 
            this.tb_Source_tableName.Location = new System.Drawing.Point(128, 164);
            this.tb_Source_tableName.Name = "tb_Source_tableName";
            this.tb_Source_tableName.Size = new System.Drawing.Size(207, 20);
            this.tb_Source_tableName.TabIndex = 10;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(13, 167);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(106, 13);
            this.label16.TabIndex = 9;
            this.label16.Text = "Scheme/table name:";
            // 
            // tb_oracle_pwd
            // 
            this.tb_oracle_pwd.Location = new System.Drawing.Point(128, 131);
            this.tb_oracle_pwd.Name = "tb_oracle_pwd";
            this.tb_oracle_pwd.PasswordChar = '*';
            this.tb_oracle_pwd.Size = new System.Drawing.Size(207, 20);
            this.tb_oracle_pwd.TabIndex = 8;
            // 
            // tb_oracle_username
            // 
            this.tb_oracle_username.Location = new System.Drawing.Point(128, 96);
            this.tb_oracle_username.Name = "tb_oracle_username";
            this.tb_oracle_username.Size = new System.Drawing.Size(207, 20);
            this.tb_oracle_username.TabIndex = 7;
            // 
            // tb_oracle_dbName
            // 
            this.tb_oracle_dbName.Location = new System.Drawing.Point(128, 63);
            this.tb_oracle_dbName.Name = "tb_oracle_dbName";
            this.tb_oracle_dbName.Size = new System.Drawing.Size(207, 20);
            this.tb_oracle_dbName.TabIndex = 6;
            // 
            // tb_oracle_dsource
            // 
            this.tb_oracle_dsource.Location = new System.Drawing.Point(128, 32);
            this.tb_oracle_dsource.Name = "tb_oracle_dsource";
            this.tb_oracle_dsource.Size = new System.Drawing.Size(207, 20);
            this.tb_oracle_dsource.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 131);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Password";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Username";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "DB Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Data Source";
            // 
            // btn_Connect_Reference_ID
            // 
            this.btn_Connect_Reference_ID.Location = new System.Drawing.Point(17, 282);
            this.btn_Connect_Reference_ID.Name = "btn_Connect_Reference_ID";
            this.btn_Connect_Reference_ID.Size = new System.Drawing.Size(126, 23);
            this.btn_Connect_Reference_ID.TabIndex = 28;
            this.btn_Connect_Reference_ID.Text = "Connect and Load";
            this.btn_Connect_Reference_ID.UseVisualStyleBackColor = true;
            this.btn_Connect_Reference_ID.Click += new System.EventHandler(this.btn_Connect_Reference_ID_Click);
            // 
            // cb_UserProfileList
            // 
            this.cb_UserProfileList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_UserProfileList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_UserProfileList.FormattingEnabled = true;
            this.cb_UserProfileList.ItemHeight = 13;
            this.cb_UserProfileList.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.cb_UserProfileList.Location = new System.Drawing.Point(6, 19);
            this.cb_UserProfileList.Name = "cb_UserProfileList";
            this.cb_UserProfileList.Size = new System.Drawing.Size(181, 21);
            this.cb_UserProfileList.TabIndex = 29;
            this.cb_UserProfileList.SelectedIndexChanged += new System.EventHandler(this.cb_UserProfileList_SelectedIndexChanged);
            // 
            // lbl_Oracle_SQL_Connection
            // 
            this.lbl_Oracle_SQL_Connection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Oracle_SQL_Connection.Location = new System.Drawing.Point(18, 315);
            this.lbl_Oracle_SQL_Connection.Name = "lbl_Oracle_SQL_Connection";
            this.lbl_Oracle_SQL_Connection.Size = new System.Drawing.Size(450, 65);
            this.lbl_Oracle_SQL_Connection.TabIndex = 29;
            this.lbl_Oracle_SQL_Connection.Text = "lbl_Oracle_SQL_Connection";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(6, 60);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 30;
            this.btn_Save.Text = "Save Profile";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(112, 60);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(75, 23);
            this.btn_Delete.TabIndex = 31;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // cb_query
            // 
            this.cb_query.AutoSize = true;
            this.cb_query.Location = new System.Drawing.Point(17, 27);
            this.cb_query.Name = "cb_query";
            this.cb_query.Size = new System.Drawing.Size(180, 17);
            this.cb_query.TabIndex = 32;
            this.cb_query.Text = "Use Query instead of table name";
            this.cb_query.UseVisualStyleBackColor = true;
            this.cb_query.CheckedChanged += new System.EventHandler(this.cb_query_CheckedChanged);
            // 
            // tb_querybox
            // 
            this.tb_querybox.Location = new System.Drawing.Point(21, 50);
            this.tb_querybox.Multiline = true;
            this.tb_querybox.Name = "tb_querybox";
            this.tb_querybox.Size = new System.Drawing.Size(450, 220);
            this.tb_querybox.TabIndex = 33;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_querybox);
            this.groupBox3.Controls.Add(this.btn_Connect_Reference_ID);
            this.groupBox3.Controls.Add(this.cb_query);
            this.groupBox3.Controls.Add(this.lbl_Oracle_SQL_Connection);
            this.groupBox3.Location = new System.Drawing.Point(756, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(517, 392);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Confiuration of table name";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cb_UserProfileList);
            this.groupBox4.Controls.Add(this.btn_Save);
            this.groupBox4.Controls.Add(this.btn_Delete);
            this.groupBox4.Location = new System.Drawing.Point(15, 296);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(350, 98);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Profile";
            // 
            // OracleAndSQLConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "OracleAndSQLConnection";
            this.Size = new System.Drawing.Size(1288, 418);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton rb_SRC_oracle;
        private System.Windows.Forms.RadioButton rb_SRC_SQL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tb_destination_tableName;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tb_sql_pwd;
        private System.Windows.Forms.TextBox tb_sql_username;
        private System.Windows.Forms.TextBox tb_sql_dbName;
        private System.Windows.Forms.TextBox tb_sql_dsource;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_Source_tableName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tb_oracle_pwd;
        private System.Windows.Forms.TextBox tb_oracle_username;
        private System.Windows.Forms.TextBox tb_oracle_dbName;
        private System.Windows.Forms.TextBox tb_oracle_dsource;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Connect_Reference_ID;
        private System.Windows.Forms.ComboBox cb_UserProfileList;
        private System.Windows.Forms.Label lbl_Oracle_SQL_Connection;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.CheckBox cb_query;
        private System.Windows.Forms.TextBox tb_querybox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}
