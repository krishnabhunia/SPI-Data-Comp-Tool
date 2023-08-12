namespace SPI_Data_Comp_Tool
{
    partial class DBConnectionOracleAndSQLDuplex
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rb_Target_Oracle = new System.Windows.Forms.RadioButton();
            this.rb_Target_SQL = new System.Windows.Forms.RadioButton();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.rb_oracle = new System.Windows.Forms.RadioButton();
            this.rb_sql = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_Target_DefaultPort = new System.Windows.Forms.TextBox();
            this.cb_Target_DefaultPort = new System.Windows.Forms.CheckBox();
            this.cb_windowsAuthentication = new System.Windows.Forms.CheckBox();
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
            this.cb_WindowsAuthSource = new System.Windows.Forms.CheckBox();
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
            this.btn_Connect_Reference_ID = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rb_Target_Oracle);
            this.groupBox4.Controls.Add(this.rb_Target_SQL);
            this.groupBox4.Location = new System.Drawing.Point(383, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(354, 48);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Select Target Database Platform : ";
            // 
            // rb_Target_Oracle
            // 
            this.rb_Target_Oracle.AutoSize = true;
            this.rb_Target_Oracle.Checked = true;
            this.rb_Target_Oracle.Location = new System.Drawing.Point(59, 20);
            this.rb_Target_Oracle.Name = "rb_Target_Oracle";
            this.rb_Target_Oracle.Size = new System.Drawing.Size(56, 17);
            this.rb_Target_Oracle.TabIndex = 17;
            this.rb_Target_Oracle.TabStop = true;
            this.rb_Target_Oracle.Text = "Oracle";
            this.rb_Target_Oracle.UseVisualStyleBackColor = true;
            // 
            // rb_Target_SQL
            // 
            this.rb_Target_SQL.AutoSize = true;
            this.rb_Target_SQL.Location = new System.Drawing.Point(141, 19);
            this.rb_Target_SQL.Name = "rb_Target_SQL";
            this.rb_Target_SQL.Size = new System.Drawing.Size(46, 17);
            this.rb_Target_SQL.TabIndex = 18;
            this.rb_Target_SQL.TabStop = true;
            this.rb_Target_SQL.Text = "SQL";
            this.rb_Target_SQL.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.rb_oracle);
            this.groupBox8.Controls.Add(this.rb_sql);
            this.groupBox8.Location = new System.Drawing.Point(10, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(354, 48);
            this.groupBox8.TabIndex = 28;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Select Source Database Platform : ";
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
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_Target_DefaultPort);
            this.groupBox2.Controls.Add(this.cb_Target_DefaultPort);
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
            this.groupBox2.Location = new System.Drawing.Point(383, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(354, 265);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target : SQL Connection and Settings";
            // 
            // tb_Target_DefaultPort
            // 
            this.tb_Target_DefaultPort.Enabled = false;
            this.tb_Target_DefaultPort.Location = new System.Drawing.Point(130, 232);
            this.tb_Target_DefaultPort.Name = "tb_Target_DefaultPort";
            this.tb_Target_DefaultPort.Size = new System.Drawing.Size(207, 20);
            this.tb_Target_DefaultPort.TabIndex = 14;
            this.tb_Target_DefaultPort.Text = "1521";
            // 
            // cb_Target_DefaultPort
            // 
            this.cb_Target_DefaultPort.AutoSize = true;
            this.cb_Target_DefaultPort.Checked = true;
            this.cb_Target_DefaultPort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Target_DefaultPort.Location = new System.Drawing.Point(19, 234);
            this.cb_Target_DefaultPort.Name = "cb_Target_DefaultPort";
            this.cb_Target_DefaultPort.Size = new System.Drawing.Size(104, 17);
            this.cb_Target_DefaultPort.TabIndex = 13;
            this.cb_Target_DefaultPort.Text = "Use Default Port";
            this.cb_Target_DefaultPort.UseVisualStyleBackColor = true;
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_WindowsAuthSource);
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
            this.groupBox1.Location = new System.Drawing.Point(10, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 265);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source : Oracle Connection and Settings";
            // 
            // cb_WindowsAuthSource
            // 
            this.cb_WindowsAuthSource.AutoSize = true;
            this.cb_WindowsAuthSource.Location = new System.Drawing.Point(16, 31);
            this.cb_WindowsAuthSource.Name = "cb_WindowsAuthSource";
            this.cb_WindowsAuthSource.Size = new System.Drawing.Size(141, 17);
            this.cb_WindowsAuthSource.TabIndex = 13;
            this.cb_WindowsAuthSource.Text = "Windows Authentication";
            this.cb_WindowsAuthSource.UseVisualStyleBackColor = true;
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
            // btn_Connect_Reference_ID
            // 
            this.btn_Connect_Reference_ID.Location = new System.Drawing.Point(10, 328);
            this.btn_Connect_Reference_ID.Name = "btn_Connect_Reference_ID";
            this.btn_Connect_Reference_ID.Size = new System.Drawing.Size(111, 23);
            this.btn_Connect_Reference_ID.TabIndex = 30;
            this.btn_Connect_Reference_ID.Text = "Connect and Load";
            this.btn_Connect_Reference_ID.UseVisualStyleBackColor = true;
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(10, 358);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(111, 13);
            this.lbl_Status.TabIndex = 31;
            this.lbl_Status.Text = "Status : Not Cofigured";
            // 
            // DBConnectionOracleAndSQLDuplex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.btn_Connect_Reference_ID);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "DBConnectionOracleAndSQLDuplex";
            this.Size = new System.Drawing.Size(749, 384);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rb_Target_Oracle;
        private System.Windows.Forms.RadioButton rb_Target_SQL;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton rb_oracle;
        private System.Windows.Forms.RadioButton rb_sql;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tb_Target_DefaultPort;
        private System.Windows.Forms.CheckBox cb_Target_DefaultPort;
        private System.Windows.Forms.CheckBox cb_windowsAuthentication;
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
        private System.Windows.Forms.CheckBox cb_WindowsAuthSource;
        private System.Windows.Forms.TextBox tb_Default_Port;
        private System.Windows.Forms.CheckBox cb_Default_Port;
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
        private System.Windows.Forms.Label lbl_Status;
    }
}
