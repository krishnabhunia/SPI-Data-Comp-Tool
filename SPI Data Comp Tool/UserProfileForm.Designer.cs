namespace SPI_Data_Comp_Tool
{
    partial class UserProfileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserProfileForm));
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.cb_SRC_TableName = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cb_TRG_TableName = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.btn_ClearLogs = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.cb_FullName = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.l_lb_ReadProfile = new System.Windows.Forms.LinkLabel();
            this.btnDeleteUserProfile = new System.Windows.Forms.Button();
            this.btnSaveUserProfile = new System.Windows.Forms.Button();
            this.btn_Connect_Reference_ID = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Tables = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_SRC_Table = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_TRG_Table = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Side = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Profile = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_ProfileFile = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Log = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.tlp_Tables.SuspendLayout();
            this.tlp_SRC_Table.SuspendLayout();
            this.tlp_TRG_Table.SuspendLayout();
            this.tlp_Side.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tlp_Profile.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlp_ProfileFile.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tlp_Log.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tlp_Root.RowCount = 2;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.groupBox14, 0, 0);
            this.tlp_Root.Controls.Add(this.tlp_Side, 1, 0);
            this.tlp_Root.Controls.Add(this.groupBox10, 0, 1);
            this.tlp_Root.SetColumnSpan(this.groupBox10, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // groupBox14
            // 
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Connection";
            this.groupBox14.Controls.Add(this.tlp_Connection);
            this.groupBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox14.AutoSize = true;
            this.groupBox14.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox14.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.groupBox14.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox14.TabIndex = 1;
            // 
            // tlp_Connection
            // 
            this.tlp_Connection.Name = "tlp_Connection";
            this.tlp_Connection.ColumnCount = 1;
            this.tlp_Connection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Connection.RowCount = 2;
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.Controls.Add(this.connection, 0, 0);
            this.tlp_Connection.Controls.Add(this.tlp_Tables, 0, 1);
            this.tlp_Connection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Connection.AutoSize = true;
            this.tlp_Connection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Connection.TabIndex = 2;
            // 
            // connection
            // 
            this.connection.Name = "connection";
            this.connection.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.connection.AutoSize = true;
            this.connection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.connection.TabIndex = 3;
            // 
            // tlp_Tables
            // 
            this.tlp_Tables.Name = "tlp_Tables";
            this.tlp_Tables.ColumnCount = 3;
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Tables.RowCount = 2;
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.Controls.Add(this.tlp_SRC_Table, 0, 0);
            this.tlp_Tables.Controls.Add(this.tlp_TRG_Table, 2, 0);
            this.tlp_Tables.Controls.Add(this.cb_FullName, 0, 1);
            this.tlp_Tables.Controls.Add(this.btn_Connect_Reference_ID, 2, 1);
            this.tlp_Tables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Tables.AutoSize = true;
            this.tlp_Tables.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Tables.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.tlp_Tables.TabIndex = 4;
            // 
            // tlp_SRC_Table
            // 
            this.tlp_SRC_Table.Name = "tlp_SRC_Table";
            this.tlp_SRC_Table.ColumnCount = 2;
            this.tlp_SRC_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SRC_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SRC_Table.RowCount = 1;
            this.tlp_SRC_Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SRC_Table.Controls.Add(this.label16, 0, 0);
            this.tlp_SRC_Table.Controls.Add(this.cb_SRC_TableName, 1, 0);
            this.tlp_SRC_Table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_SRC_Table.AutoSize = true;
            this.tlp_SRC_Table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_SRC_Table.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_SRC_Table.TabIndex = 5;
            // 
            // label16
            // 
            this.label16.Name = "label16";
            this.label16.Text = "Source table";
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label16.AutoSize = true;
            this.label16.Margin = new System.Windows.Forms.Padding(0, 3, 12, 3);
            this.label16.TabIndex = 6;
            // 
            // cb_SRC_TableName
            // 
            this.cb_SRC_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_SRC_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_SRC_TableName.DropDownWidth = 500;
            this.cb_SRC_TableName.FormattingEnabled = true;
            this.cb_SRC_TableName.Name = "cb_SRC_TableName";
            this.cb_SRC_TableName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.cb_SRC_TableName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_SRC_TableName.TabIndex = 7;
            // 
            // tlp_TRG_Table
            // 
            this.tlp_TRG_Table.Name = "tlp_TRG_Table";
            this.tlp_TRG_Table.ColumnCount = 2;
            this.tlp_TRG_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TRG_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TRG_Table.RowCount = 1;
            this.tlp_TRG_Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TRG_Table.Controls.Add(this.label17, 0, 0);
            this.tlp_TRG_Table.Controls.Add(this.cb_TRG_TableName, 1, 0);
            this.tlp_TRG_Table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_TRG_Table.AutoSize = true;
            this.tlp_TRG_Table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_TRG_Table.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_TRG_Table.TabIndex = 8;
            // 
            // label17
            // 
            this.label17.Name = "label17";
            this.label17.Text = "Target table";
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label17.AutoSize = true;
            this.label17.Margin = new System.Windows.Forms.Padding(0, 3, 12, 3);
            this.label17.TabIndex = 9;
            // 
            // cb_TRG_TableName
            // 
            this.cb_TRG_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_TRG_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_TRG_TableName.DropDownWidth = 500;
            this.cb_TRG_TableName.Name = "cb_TRG_TableName";
            this.cb_TRG_TableName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.cb_TRG_TableName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_TRG_TableName.TabIndex = 10;
            // 
            // cb_FullName
            // 
            this.cb_FullName.Name = "cb_FullName";
            this.cb_FullName.UseVisualStyleBackColor = true;
            this.cb_FullName.CheckedChanged += new System.EventHandler(this.cb_FullName_CheckedChanged);
            this.cb_FullName.Text = "Show full names in lists";
            this.cb_FullName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_FullName.AutoSize = true;
            this.cb_FullName.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.cb_FullName.TabIndex = 11;
            // 
            // btn_Connect_Reference_ID
            // 
            this.btn_Connect_Reference_ID.Name = "btn_Connect_Reference_ID";
            this.btn_Connect_Reference_ID.UseVisualStyleBackColor = true;
            this.btn_Connect_Reference_ID.Click += new System.EventHandler(this.btn_Connect_Reference_ID_Click);
            this.btn_Connect_Reference_ID.Text = "Test connection";
            this.btn_Connect_Reference_ID.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btn_Connect_Reference_ID.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.btn_Connect_Reference_ID.Size = new System.Drawing.Size(190, 32);
            this.btn_Connect_Reference_ID.TabIndex = 12;
            // 
            // tlp_Side
            // 
            this.tlp_Side.Name = "tlp_Side";
            this.tlp_Side.ColumnCount = 1;
            this.tlp_Side.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Side.RowCount = 2;
            this.tlp_Side.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Side.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Side.Controls.Add(this.groupBox2, 0, 0);
            this.tlp_Side.Controls.Add(this.groupBox1, 0, 1);
            this.tlp_Side.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Side.AutoSize = false;
            this.tlp_Side.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_Side.TabIndex = 13;
            // 
            // groupBox2
            // 
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Profile";
            this.groupBox2.Controls.Add(this.tlp_Profile);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox2.TabIndex = 14;
            // 
            // tlp_Profile
            // 
            this.tlp_Profile.Name = "tlp_Profile";
            this.tlp_Profile.ColumnCount = 1;
            this.tlp_Profile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Profile.RowCount = 2;
            this.tlp_Profile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Profile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Profile.Controls.Add(this.btnSaveUserProfile, 0, 0);
            this.tlp_Profile.Controls.Add(this.btnDeleteUserProfile, 0, 1);
            this.tlp_Profile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Profile.AutoSize = true;
            this.tlp_Profile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Profile.TabIndex = 15;
            // 
            // btnSaveUserProfile
            // 
            this.btnSaveUserProfile.Name = "btnSaveUserProfile";
            this.btnSaveUserProfile.UseVisualStyleBackColor = true;
            this.btnSaveUserProfile.Click += new System.EventHandler(this.btnSaveUserProfile_Click);
            this.btnSaveUserProfile.Text = "Save profile";
            this.btnSaveUserProfile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveUserProfile.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.btnSaveUserProfile.Size = new System.Drawing.Size(120, 32);
            this.btnSaveUserProfile.TabIndex = 16;
            // 
            // btnDeleteUserProfile
            // 
            this.btnDeleteUserProfile.Name = "btnDeleteUserProfile";
            this.btnDeleteUserProfile.UseVisualStyleBackColor = true;
            this.btnDeleteUserProfile.Click += new System.EventHandler(this.btnDeleteUserProfile_Click);
            this.btnDeleteUserProfile.Text = "Delete profile";
            this.btnDeleteUserProfile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDeleteUserProfile.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btnDeleteUserProfile.Size = new System.Drawing.Size(120, 32);
            this.btnDeleteUserProfile.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Profile file";
            this.groupBox1.Controls.Add(this.tlp_ProfileFile);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox1.TabIndex = 18;
            // 
            // tlp_ProfileFile
            // 
            this.tlp_ProfileFile.Name = "tlp_ProfileFile";
            this.tlp_ProfileFile.ColumnCount = 1;
            this.tlp_ProfileFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_ProfileFile.RowCount = 3;
            this.tlp_ProfileFile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_ProfileFile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_ProfileFile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_ProfileFile.Controls.Add(this.label10, 0, 0);
            this.tlp_ProfileFile.Controls.Add(this.l_lb_ReadProfile, 0, 1);
            this.tlp_ProfileFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_ProfileFile.AutoSize = false;
            this.tlp_ProfileFile.TabIndex = 19;
            // 
            // label10
            // 
            this.label10.Name = "label10";
            this.label10.Text = "Profiles are read from a .json file.";
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.label10.AutoSize = true;
            this.label10.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.label10.TabIndex = 20;
            // 
            // l_lb_ReadProfile
            // 
            this.l_lb_ReadProfile.Name = "l_lb_ReadProfile";
            this.l_lb_ReadProfile.TabStop = true;
            this.l_lb_ReadProfile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l_lb_ReadProfile_LinkClicked);
            this.l_lb_ReadProfile.Text = "Choose a profile file…";
            this.l_lb_ReadProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.l_lb_ReadProfile.AutoSize = true;
            this.l_lb_ReadProfile.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.l_lb_ReadProfile.TabIndex = 21;
            // 
            // groupBox10
            // 
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Log";
            this.groupBox10.Controls.Add(this.tlp_Log);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBox10.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox10.TabIndex = 22;
            // 
            // tlp_Log
            // 
            this.tlp_Log.Name = "tlp_Log";
            this.tlp_Log.ColumnCount = 1;
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.RowCount = 2;
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.Controls.Add(this.rtb_Log, 0, 0);
            this.tlp_Log.Controls.Add(this.btn_ClearLogs, 0, 1);
            this.tlp_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Log.AutoSize = false;
            this.tlp_Log.TabIndex = 23;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.rtb_Log.TabIndex = 24;
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.SystemColors.Control;
            this.btn_ClearLogs.Name = "btn_ClearLogs";
            this.btn_ClearLogs.UseVisualStyleBackColor = false;
            this.btn_ClearLogs.Click += new System.EventHandler(this.btn_ClearLogs_Click);
            this.btn_ClearLogs.Text = "Clear log";
            this.btn_ClearLogs.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.btn_ClearLogs.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.btn_ClearLogs.Size = new System.Drawing.Size(110, 30);
            this.btn_ClearLogs.TabIndex = 25;
            // 
            // UserProfileForm
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserProfileForm";
            this.Text = "User profiles";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 760);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(980, 640);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.tlp_Tables.ResumeLayout(false);
            this.tlp_Tables.PerformLayout();
            this.tlp_SRC_Table.ResumeLayout(false);
            this.tlp_SRC_Table.PerformLayout();
            this.tlp_TRG_Table.ResumeLayout(false);
            this.tlp_TRG_Table.PerformLayout();
            this.tlp_Side.ResumeLayout(false);
            this.tlp_Side.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tlp_Profile.ResumeLayout(false);
            this.tlp_Profile.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlp_ProfileFile.ResumeLayout(false);
            this.tlp_ProfileFile.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tlp_Log.ResumeLayout(false);
            this.tlp_Log.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Tables;
        private System.Windows.Forms.TableLayoutPanel tlp_SRC_Table;
        private System.Windows.Forms.TableLayoutPanel tlp_TRG_Table;
        private System.Windows.Forms.TableLayoutPanel tlp_Side;
        private System.Windows.Forms.TableLayoutPanel tlp_Profile;
        private System.Windows.Forms.TableLayoutPanel tlp_ProfileFile;
        private System.Windows.Forms.TableLayoutPanel tlp_Log;

        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.CheckBox cb_FullName;
        private System.Windows.Forms.ComboBox cb_SRC_TableName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cb_TRG_TableName;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSaveUserProfile;
        private System.Windows.Forms.Button btnDeleteUserProfile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_Connect_Reference_ID;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.LinkLabel l_lb_ReadProfile;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btn_ClearLogs;
    }
}