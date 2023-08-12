namespace SPI_Data_Comp_Tool
{
    partial class SPI_FK_Extraction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SPI_FK_Extraction));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_ImportExcel = new System.Windows.Forms.Button();
            this.gb_Logs = new System.Windows.Forms.GroupBox();
            this.btn_ClearLogs = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.gb_DBConnection = new System.Windows.Forms.GroupBox();
            this.btn_TestDBConnection = new System.Windows.Forms.Button();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.gb_Source1 = new System.Windows.Forms.GroupBox();
            this.cb_WindowAuthentication_SRC = new System.Windows.Forms.CheckBox();
            this.tb_SRC_Password = new System.Windows.Forms.TextBox();
            this.tb_SRC_UserName = new System.Windows.Forms.TextBox();
            this.tb_SRC_DBName = new System.Windows.Forms.TextBox();
            this.tb_SRC_DataSource = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_UserProfileList = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.gb_Logs.SuspendLayout();
            this.gb_DBConnection.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.gb_Source1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_ImportExcel);
            this.groupBox1.Location = new System.Drawing.Point(355, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(104, 237);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // btn_ImportExcel
            // 
            this.btn_ImportExcel.Location = new System.Drawing.Point(18, 28);
            this.btn_ImportExcel.Name = "btn_ImportExcel";
            this.btn_ImportExcel.Size = new System.Drawing.Size(75, 23);
            this.btn_ImportExcel.TabIndex = 0;
            this.btn_ImportExcel.Text = "Import Excel";
            this.btn_ImportExcel.UseVisualStyleBackColor = true;
            this.btn_ImportExcel.Click += new System.EventHandler(this.btn_ImportExcel_Click);
            // 
            // gb_Logs
            // 
            this.gb_Logs.Controls.Add(this.btn_ClearLogs);
            this.gb_Logs.Controls.Add(this.progressBar1);
            this.gb_Logs.Controls.Add(this.rtb_Log);
            this.gb_Logs.Controls.Add(this.lbl_Progress);
            this.gb_Logs.Location = new System.Drawing.Point(12, 255);
            this.gb_Logs.Name = "gb_Logs";
            this.gb_Logs.Size = new System.Drawing.Size(999, 482);
            this.gb_Logs.TabIndex = 34;
            this.gb_Logs.TabStop = false;
            this.gb_Logs.Text = "Logs";
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.Color.White;
            this.btn_ClearLogs.Location = new System.Drawing.Point(918, 15);
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
            this.progressBar1.Size = new System.Drawing.Size(900, 15);
            this.progressBar1.TabIndex = 1;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_Log.Location = new System.Drawing.Point(12, 59);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(981, 412);
            this.rtb_Log.TabIndex = 0;
            this.rtb_Log.Text = "";
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Progress.Location = new System.Drawing.Point(14, 39);
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Size = new System.Drawing.Size(63, 16);
            this.lbl_Progress.TabIndex = 0;
            this.lbl_Progress.Text = "Progress";
            // 
            // gb_DBConnection
            // 
            this.gb_DBConnection.Controls.Add(this.btn_TestDBConnection);
            this.gb_DBConnection.Controls.Add(this.groupBox12);
            this.gb_DBConnection.Location = new System.Drawing.Point(12, 12);
            this.gb_DBConnection.Name = "gb_DBConnection";
            this.gb_DBConnection.Size = new System.Drawing.Size(337, 237);
            this.gb_DBConnection.TabIndex = 35;
            this.gb_DBConnection.TabStop = false;
            this.gb_DBConnection.Text = "DataBase Connection Settings";
            // 
            // btn_TestDBConnection
            // 
            this.btn_TestDBConnection.Location = new System.Drawing.Point(98, 208);
            this.btn_TestDBConnection.Name = "btn_TestDBConnection";
            this.btn_TestDBConnection.Size = new System.Drawing.Size(150, 23);
            this.btn_TestDBConnection.TabIndex = 33;
            this.btn_TestDBConnection.Text = "Test Both DB Connection";
            this.btn_TestDBConnection.UseVisualStyleBackColor = true;
            this.btn_TestDBConnection.Click += new System.EventHandler(this.btn_TestDBConnection_Click);
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.gb_Source1);
            this.groupBox12.Location = new System.Drawing.Point(6, 19);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(323, 183);
            this.groupBox12.TabIndex = 28;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Source";
            // 
            // gb_Source1
            // 
            this.gb_Source1.Controls.Add(this.cb_WindowAuthentication_SRC);
            this.gb_Source1.Controls.Add(this.tb_SRC_Password);
            this.gb_Source1.Controls.Add(this.tb_SRC_UserName);
            this.gb_Source1.Controls.Add(this.tb_SRC_DBName);
            this.gb_Source1.Controls.Add(this.tb_SRC_DataSource);
            this.gb_Source1.Controls.Add(this.label9);
            this.gb_Source1.Controls.Add(this.label7);
            this.gb_Source1.Controls.Add(this.label5);
            this.gb_Source1.Controls.Add(this.label3);
            this.gb_Source1.Location = new System.Drawing.Point(6, 19);
            this.gb_Source1.Name = "gb_Source1";
            this.gb_Source1.Size = new System.Drawing.Size(308, 158);
            this.gb_Source1.TabIndex = 2;
            this.gb_Source1.TabStop = false;
            this.gb_Source1.Text = "Source : Oracle Connection and Settings";
            // 
            // cb_WindowAuthentication_SRC
            // 
            this.cb_WindowAuthentication_SRC.AutoSize = true;
            this.cb_WindowAuthentication_SRC.Location = new System.Drawing.Point(16, 23);
            this.cb_WindowAuthentication_SRC.Name = "cb_WindowAuthentication_SRC";
            this.cb_WindowAuthentication_SRC.Size = new System.Drawing.Size(136, 17);
            this.cb_WindowAuthentication_SRC.TabIndex = 30;
            this.cb_WindowAuthentication_SRC.Text = "Window Authentication";
            this.cb_WindowAuthentication_SRC.UseVisualStyleBackColor = true;
            this.cb_WindowAuthentication_SRC.CheckedChanged += new System.EventHandler(this.cb_WindowAuthentication_SRC_CheckedChanged);
            // 
            // tb_SRC_Password
            // 
            this.tb_SRC_Password.Location = new System.Drawing.Point(86, 129);
            this.tb_SRC_Password.Name = "tb_SRC_Password";
            this.tb_SRC_Password.PasswordChar = '*';
            this.tb_SRC_Password.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_Password.TabIndex = 8;
            // 
            // tb_SRC_UserName
            // 
            this.tb_SRC_UserName.Location = new System.Drawing.Point(86, 101);
            this.tb_SRC_UserName.Name = "tb_SRC_UserName";
            this.tb_SRC_UserName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_UserName.TabIndex = 7;
            // 
            // tb_SRC_DBName
            // 
            this.tb_SRC_DBName.Location = new System.Drawing.Point(86, 73);
            this.tb_SRC_DBName.Name = "tb_SRC_DBName";
            this.tb_SRC_DBName.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DBName.TabIndex = 6;
            // 
            // tb_SRC_DataSource
            // 
            this.tb_SRC_DataSource.Location = new System.Drawing.Point(86, 47);
            this.tb_SRC_DataSource.Name = "tb_SRC_DataSource";
            this.tb_SRC_DataSource.Size = new System.Drawing.Size(207, 20);
            this.tb_SRC_DataSource.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 133);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Password";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 104);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Username";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "DB Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Data Source";
            // 
            // cb_UserProfileList
            // 
            this.cb_UserProfileList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_UserProfileList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_UserProfileList.FormattingEnabled = true;
            this.cb_UserProfileList.ItemHeight = 13;
            this.cb_UserProfileList.Location = new System.Drawing.Point(465, 12);
            this.cb_UserProfileList.Name = "cb_UserProfileList";
            this.cb_UserProfileList.Size = new System.Drawing.Size(299, 21);
            this.cb_UserProfileList.TabIndex = 36;
            this.cb_UserProfileList.Visible = false;
            this.cb_UserProfileList.SelectedIndexChanged += new System.EventHandler(this.cb_UserProfileList_SelectedIndexChanged);
            // 
            // SPI_FK_Extraction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1203, 738);
            this.Controls.Add(this.cb_UserProfileList);
            this.Controls.Add(this.gb_DBConnection);
            this.Controls.Add(this.gb_Logs);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SPI_FK_Extraction";
            this.Text = "SPI_FK_Extraction";
            this.groupBox1.ResumeLayout(false);
            this.gb_Logs.ResumeLayout(false);
            this.gb_Logs.PerformLayout();
            this.gb_DBConnection.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.gb_Source1.ResumeLayout(false);
            this.gb_Source1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_ImportExcel;
        private System.Windows.Forms.GroupBox gb_Logs;
        private System.Windows.Forms.Button btn_ClearLogs;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Label lbl_Progress;
        private System.Windows.Forms.GroupBox gb_DBConnection;
        private System.Windows.Forms.Button btn_TestDBConnection;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.GroupBox gb_Source1;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_SRC;
        private System.Windows.Forms.TextBox tb_SRC_Password;
        private System.Windows.Forms.TextBox tb_SRC_UserName;
        private System.Windows.Forms.TextBox tb_SRC_DBName;
        private System.Windows.Forms.TextBox tb_SRC_DataSource;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_UserProfileList;
    }
}