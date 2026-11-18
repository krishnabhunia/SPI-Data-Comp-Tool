namespace SPI_Data_Comp_Tool
{
    partial class Pre_Merging_Duplicacy_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pre_Merging_Duplicacy_Form));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_ConfigurationMode = new System.Windows.Forms.CheckBox();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.llb_File = new System.Windows.Forms.LinkLabel();
            this.lb_link = new System.Windows.Forms.Label();
            this.btn_Connect_Reference_ID = new System.Windows.Forms.Button();
            this.btn_ListTableExcel = new System.Windows.Forms.Button();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.cb_FullName = new System.Windows.Forms.CheckBox();
            this.btn_Load_SRC_Table = new System.Windows.Forms.Button();
            this.cb_SRC_TableName = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cb_TRG_TableName = new System.Windows.Forms.ComboBox();
            this.btn_Load_TRG_Table = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Run = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tlp_Run.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tlp_Status.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tlp_Root.RowCount = 2;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.groupBox14, 0, 0);
            this.tlp_Root.Controls.Add(this.groupBox3, 1, 0);
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
            this.tlp_Connection.RowCount = 1;
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.Controls.Add(this.connection, 0, 0);
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
            // groupBox3
            // 
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Run";
            this.groupBox3.Controls.Add(this.tlp_Run);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox3.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox3.TabIndex = 4;
            // 
            // tlp_Run
            // 
            this.tlp_Run.Name = "tlp_Run";
            this.tlp_Run.ColumnCount = 1;
            this.tlp_Run.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Run.RowCount = 6;
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Run.Controls.Add(this.btn_Connect_Reference_ID, 0, 0);
            this.tlp_Run.Controls.Add(this.cb_ConfigurationMode, 0, 1);
            this.tlp_Run.Controls.Add(this.cb_IgnoreCase, 0, 2);
            this.tlp_Run.Controls.Add(this.btn_ListTableExcel, 0, 3);
            this.tlp_Run.Controls.Add(this.lb_link, 0, 4);
            this.tlp_Run.Controls.Add(this.llb_File, 0, 5);
            this.tlp_Run.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Run.AutoSize = false;
            this.tlp_Run.TabIndex = 5;
            // 
            // btn_Connect_Reference_ID
            // 
            this.btn_Connect_Reference_ID.Name = "btn_Connect_Reference_ID";
            this.btn_Connect_Reference_ID.UseVisualStyleBackColor = true;
            this.btn_Connect_Reference_ID.Click += new System.EventHandler(this.btn_Connect_Reference_ID_Click);
            this.btn_Connect_Reference_ID.Text = "Test connections";
            this.btn_Connect_Reference_ID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Connect_Reference_ID.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.btn_Connect_Reference_ID.Size = new System.Drawing.Size(160, 32);
            this.btn_Connect_Reference_ID.TabIndex = 6;
            // 
            // cb_ConfigurationMode
            // 
            this.cb_ConfigurationMode.Enabled = false;
            this.cb_ConfigurationMode.Name = "cb_ConfigurationMode";
            this.cb_ConfigurationMode.UseVisualStyleBackColor = true;
            this.cb_ConfigurationMode.Visible = false;
            this.cb_ConfigurationMode.Text = "Run in configuration mode";
            this.cb_ConfigurationMode.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_ConfigurationMode.AutoSize = true;
            this.cb_ConfigurationMode.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_ConfigurationMode.TabIndex = 7;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            this.cb_IgnoreCase.Text = "Ignore case";
            this.cb_IgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Margin = new System.Windows.Forms.Padding(0, 3, 0, 8);
            this.cb_IgnoreCase.TabIndex = 8;
            // 
            // btn_ListTableExcel
            // 
            this.btn_ListTableExcel.Name = "btn_ListTableExcel";
            this.btn_ListTableExcel.UseVisualStyleBackColor = true;
            this.btn_ListTableExcel.Click += new System.EventHandler(this.btn_ListTableExcel_Click);
            this.btn_ListTableExcel.Text = "Run pre-merging Excel process";
            this.btn_ListTableExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ListTableExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.btn_ListTableExcel.Size = new System.Drawing.Size(160, 36);
            this.btn_ListTableExcel.TabIndex = 9;
            // 
            // lb_link
            // 
            this.lb_link.Name = "lb_link";
            this.lb_link.Visible = false;
            this.lb_link.Text = "Open the output folder:";
            this.lb_link.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lb_link.AutoSize = true;
            this.lb_link.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lb_link.TabIndex = 10;
            // 
            // llb_File
            // 
            this.llb_File.Name = "llb_File";
            this.llb_File.TabStop = true;
            this.llb_File.Text = "No File";
            this.llb_File.Visible = false;
            this.llb_File.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_File_LinkClicked);
            this.llb_File.Dock = System.Windows.Forms.DockStyle.Fill;
            this.llb_File.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.llb_File.TabIndex = 11;
            // 
            // groupBox10
            // 
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Status";
            this.groupBox10.Controls.Add(this.tlp_Status);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBox10.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox10.TabIndex = 12;
            // 
            // tlp_Status
            // 
            this.tlp_Status.Name = "tlp_Status";
            this.tlp_Status.ColumnCount = 2;
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.RowCount = 2;
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Status.Controls.Add(this.progressBar1, 0, 0);
            this.tlp_Status.Controls.Add(this.btn_Clear_Reset, 1, 0);
            this.tlp_Status.Controls.Add(this.rtb_Log, 0, 1);
            this.tlp_Status.SetColumnSpan(this.rtb_Log, 2);
            this.tlp_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Status.AutoSize = false;
            this.tlp_Status.TabIndex = 13;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 14;
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Name = "btn_Clear_Reset";
            this.btn_Clear_Reset.UseVisualStyleBackColor = true;
            this.btn_Clear_Reset.Click += new System.EventHandler(this.btn_Clear_Reset_Click);
            this.btn_Clear_Reset.Text = "Clear / Reset";
            this.btn_Clear_Reset.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Clear_Reset.Size = new System.Drawing.Size(110, 30);
            this.btn_Clear_Reset.TabIndex = 15;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.rtb_Log.TabIndex = 16;
            // 
            // cb_FullName
            // 
            this.cb_FullName.AutoSize = true;
            this.cb_FullName.Location = new System.Drawing.Point(6, 83);
            this.cb_FullName.Name = "cb_FullName";
            this.cb_FullName.Size = new System.Drawing.Size(165, 17);
            this.cb_FullName.TabIndex = 30;
            this.cb_FullName.Text = "Show FullName in DropDown";
            this.cb_FullName.UseVisualStyleBackColor = true;
            this.cb_FullName.Visible = false;
            // 
            // btn_Load_SRC_Table
            // 
            this.btn_Load_SRC_Table.Location = new System.Drawing.Point(126, 16);
            this.btn_Load_SRC_Table.Name = "btn_Load_SRC_Table";
            this.btn_Load_SRC_Table.Size = new System.Drawing.Size(207, 23);
            this.btn_Load_SRC_Table.TabIndex = 13;
            this.btn_Load_SRC_Table.Text = "Load Source Tables";
            this.btn_Load_SRC_Table.UseVisualStyleBackColor = true;
            this.btn_Load_SRC_Table.Visible = false;
            this.btn_Load_SRC_Table.Click += new System.EventHandler(this.btn_Load_SRC_Table_Click);
            // 
            // cb_SRC_TableName
            // 
            this.cb_SRC_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_SRC_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_SRC_TableName.DropDownWidth = 500;
            this.cb_SRC_TableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SRC_TableName.FormattingEnabled = true;
            this.cb_SRC_TableName.Location = new System.Drawing.Point(126, 51);
            this.cb_SRC_TableName.Name = "cb_SRC_TableName";
            this.cb_SRC_TableName.Size = new System.Drawing.Size(207, 26);
            this.cb_SRC_TableName.TabIndex = 29;
            this.cb_SRC_TableName.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 51);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(106, 13);
            this.label16.TabIndex = 9;
            this.label16.Text = "Scheme/table name:";
            // 
            // cb_TRG_TableName
            // 
            this.cb_TRG_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_TRG_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_TRG_TableName.DropDownWidth = 500;
            this.cb_TRG_TableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_TRG_TableName.Location = new System.Drawing.Point(466, 61);
            this.cb_TRG_TableName.Name = "cb_TRG_TableName";
            this.cb_TRG_TableName.Size = new System.Drawing.Size(207, 26);
            this.cb_TRG_TableName.TabIndex = 26;
            this.cb_TRG_TableName.Visible = false;
            // 
            // btn_Load_TRG_Table
            // 
            this.btn_Load_TRG_Table.Location = new System.Drawing.Point(466, 29);
            this.btn_Load_TRG_Table.Name = "btn_Load_TRG_Table";
            this.btn_Load_TRG_Table.Size = new System.Drawing.Size(207, 23);
            this.btn_Load_TRG_Table.TabIndex = 31;
            this.btn_Load_TRG_Table.Text = "Load Target Tables";
            this.btn_Load_TRG_Table.UseVisualStyleBackColor = true;
            this.btn_Load_TRG_Table.Visible = false;
            this.btn_Load_TRG_Table.Click += new System.EventHandler(this.btn_Load_TRG_Table_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(353, 64);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(106, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "Scheme/table name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_FullName);
            this.groupBox1.Controls.Add(this.cb_TRG_TableName);
            this.groupBox1.Controls.Add(this.btn_Load_SRC_Table);
            this.groupBox1.Controls.Add(this.btn_Load_TRG_Table);
            this.groupBox1.Controls.Add(this.cb_SRC_TableName);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Location = new System.Drawing.Point(1232, 394);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(50, 39);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            this.groupBox1.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // Pre_Merging_Duplicacy_Form
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Pre_Merging_Duplicacy_Form";
            this.Text = "Pre-merging duplicacy check";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 860);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1080, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.Controls.Add(this.groupBox1);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tlp_Run.ResumeLayout(false);
            this.tlp_Run.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Run;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_Clear_Reset;
        private System.Windows.Forms.Button btn_Connect_Reference_ID;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.CheckBox cb_FullName;
        private System.Windows.Forms.Button btn_Load_SRC_Table;
        private System.Windows.Forms.ComboBox cb_SRC_TableName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cb_TRG_TableName;
        private System.Windows.Forms.Button btn_Load_TRG_Table;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lb_link;
        private System.Windows.Forms.LinkLabel llb_File;
        private System.Windows.Forms.Button btn_ListTableExcel;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
        private System.Windows.Forms.CheckBox cb_ConfigurationMode;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}