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
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.btn_ExportToExcel = new System.Windows.Forms.Button();
            this.clb_TableList = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_SkipEmptyTable = new System.Windows.Forms.CheckBox();
            this.cb_SelectAll = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.rb_SRC = new System.Windows.Forms.RadioButton();
            this.rb_TRG = new System.Windows.Forms.RadioButton();
            this.tb_Find = new System.Windows.Forms.TextBox();
            this.lbl_Find = new System.Windows.Forms.Label();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.flp_LoadFrom = new System.Windows.Forms.FlowLayoutPanel();
            this.lbl_LoadFrom = new System.Windows.Forms.Label();
            this.flp_Connect = new System.Windows.Forms.FlowLayoutPanel();
            this.tlp_Save = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Tables = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.flp_LoadFrom.SuspendLayout();
            this.flp_Connect.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tlp_Save.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlp_Tables.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 420F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowCount = 2;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.groupBox2, 0, 0);
            this.tlp_Root.Controls.Add(this.groupBox3, 0, 1);
            this.tlp_Root.Controls.Add(this.groupBox1, 1, 0);
            this.tlp_Root.SetRowSpan(this.groupBox1, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connection";
            this.groupBox2.Controls.Add(this.tlp_Connection);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox2.TabIndex = 1;
            // 
            // tlp_Connection
            // 
            this.tlp_Connection.Name = "tlp_Connection";
            this.tlp_Connection.ColumnCount = 1;
            this.tlp_Connection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Connection.RowCount = 5;
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.Controls.Add(this.connection, 0, 0);
            this.tlp_Connection.Controls.Add(this.flp_LoadFrom, 0, 1);
            this.tlp_Connection.Controls.Add(this.flp_Connect, 0, 2);
            this.tlp_Connection.Controls.Add(this.progressBar1, 0, 3);
            this.tlp_Connection.Controls.Add(this.lbl_Status, 0, 4);
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
            // flp_LoadFrom
            // 
            this.flp_LoadFrom.Name = "flp_LoadFrom";
            this.flp_LoadFrom.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flp_LoadFrom.WrapContents = false;
            this.flp_LoadFrom.Controls.Add(this.lbl_LoadFrom);
            this.flp_LoadFrom.Controls.Add(this.rb_SRC);
            this.flp_LoadFrom.Controls.Add(this.rb_TRG);
            this.flp_LoadFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.flp_LoadFrom.AutoSize = true;
            this.flp_LoadFrom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flp_LoadFrom.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flp_LoadFrom.TabIndex = 4;
            // 
            // lbl_LoadFrom
            // 
            this.lbl_LoadFrom.Name = "lbl_LoadFrom";
            this.lbl_LoadFrom.Text = "Fill from profile half";
            this.lbl_LoadFrom.AutoSize = true;
            this.lbl_LoadFrom.Margin = new System.Windows.Forms.Padding(0, 4, 12, 0);
            this.lbl_LoadFrom.TabIndex = 5;
            // 
            // rb_SRC
            // 
            this.rb_SRC.Checked = true;
            this.rb_SRC.Name = "rb_SRC";
            this.rb_SRC.TabStop = true;
            this.rb_SRC.UseVisualStyleBackColor = true;
            this.rb_SRC.CheckedChanged += new System.EventHandler(this.rb_SRC_CheckedChanged);
            this.rb_SRC.Text = "Source";
            this.rb_SRC.AutoSize = true;
            this.rb_SRC.Margin = new System.Windows.Forms.Padding(0, 2, 16, 0);
            this.rb_SRC.TabIndex = 6;
            // 
            // rb_TRG
            // 
            this.rb_TRG.Name = "rb_TRG";
            this.rb_TRG.UseVisualStyleBackColor = true;
            this.rb_TRG.CheckedChanged += new System.EventHandler(this.rb_TRG_CheckedChanged);
            this.rb_TRG.Text = "Target";
            this.rb_TRG.AutoSize = true;
            this.rb_TRG.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.rb_TRG.TabIndex = 7;
            // 
            // flp_Connect
            // 
            this.flp_Connect.Name = "flp_Connect";
            this.flp_Connect.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flp_Connect.WrapContents = false;
            this.flp_Connect.Controls.Add(this.btn_Connect);
            this.flp_Connect.Controls.Add(this.btn_Cancel);
            this.flp_Connect.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.flp_Connect.AutoSize = true;
            this.flp_Connect.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flp_Connect.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flp_Connect.TabIndex = 8;
            // 
            // btn_Connect
            // 
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            this.btn_Connect.Text = "Connect and load tables";
            this.btn_Connect.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btn_Connect.Size = new System.Drawing.Size(190, 32);
            this.btn_Connect.TabIndex = 9;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Cancel.Size = new System.Drawing.Size(100, 32);
            this.btn_Cancel.TabIndex = 10;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 11;
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoEllipsis = true;
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Text = "Ready";
            this.lbl_Status.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lbl_Status.TabIndex = 12;
            // 
            // groupBox3
            // 
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Save Excel to";
            this.groupBox3.Controls.Add(this.tlp_Save);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.groupBox3.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox3.TabIndex = 13;
            // 
            // tlp_Save
            // 
            this.tlp_Save.Name = "tlp_Save";
            this.tlp_Save.ColumnCount = 1;
            this.tlp_Save.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Save.RowCount = 2;
            this.tlp_Save.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Save.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Save.Controls.Add(this.linkLabel1, 0, 0);
            this.tlp_Save.Controls.Add(this.btn_ExportToExcel, 0, 1);
            this.tlp_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Save.AutoSize = false;
            this.tlp_Save.TabIndex = 14;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "D:/";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.linkLabel1.TabIndex = 15;
            // 
            // btn_ExportToExcel
            // 
            this.btn_ExportToExcel.Name = "btn_ExportToExcel";
            this.btn_ExportToExcel.UseVisualStyleBackColor = true;
            this.btn_ExportToExcel.Click += new System.EventHandler(this.btn_ExportToExcel_Click);
            this.btn_ExportToExcel.Text = "Export to Excel";
            this.btn_ExportToExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ExportToExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_ExportToExcel.Size = new System.Drawing.Size(160, 36);
            this.btn_ExportToExcel.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tables";
            this.groupBox1.Controls.Add(this.tlp_Tables);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox1.TabIndex = 17;
            // 
            // tlp_Tables
            // 
            this.tlp_Tables.Name = "tlp_Tables";
            this.tlp_Tables.ColumnCount = 4;
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.RowCount = 2;
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Tables.Controls.Add(this.lbl_Find, 0, 0);
            this.tlp_Tables.Controls.Add(this.tb_Find, 1, 0);
            this.tlp_Tables.Controls.Add(this.cb_SelectAll, 2, 0);
            this.tlp_Tables.Controls.Add(this.cb_SkipEmptyTable, 3, 0);
            this.tlp_Tables.Controls.Add(this.clb_TableList, 0, 1);
            this.tlp_Tables.SetColumnSpan(this.clb_TableList, 4);
            this.tlp_Tables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Tables.AutoSize = false;
            this.tlp_Tables.TabIndex = 18;
            // 
            // lbl_Find
            // 
            this.lbl_Find.Name = "lbl_Find";
            this.lbl_Find.Text = "Find";
            this.lbl_Find.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Find.AutoSize = true;
            this.lbl_Find.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.lbl_Find.TabIndex = 19;
            // 
            // tb_Find
            // 
            this.tb_Find.Name = "tb_Find";
            this.tb_Find.TextChanged += new System.EventHandler(this.tb_Find_TextChanged);
            this.tb_Find.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.tb_Find.Margin = new System.Windows.Forms.Padding(0, 3, 16, 3);
            this.tb_Find.TabIndex = 20;
            // 
            // cb_SelectAll
            // 
            this.cb_SelectAll.Name = "cb_SelectAll";
            this.cb_SelectAll.UseVisualStyleBackColor = true;
            this.cb_SelectAll.CheckedChanged += new System.EventHandler(this.cb_SelectAll_CheckedChanged);
            this.cb_SelectAll.Text = "Select all";
            this.cb_SelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_SelectAll.AutoSize = true;
            this.cb_SelectAll.Margin = new System.Windows.Forms.Padding(0, 3, 16, 3);
            this.cb_SelectAll.TabIndex = 21;
            // 
            // cb_SkipEmptyTable
            // 
            this.cb_SkipEmptyTable.Name = "cb_SkipEmptyTable";
            this.cb_SkipEmptyTable.UseVisualStyleBackColor = true;
            this.cb_SkipEmptyTable.Text = "Skip empty tables";
            this.cb_SkipEmptyTable.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_SkipEmptyTable.AutoSize = true;
            this.cb_SkipEmptyTable.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_SkipEmptyTable.TabIndex = 22;
            // 
            // clb_TableList
            // 
            this.clb_TableList.CheckOnClick = true;
            this.clb_TableList.FormattingEnabled = true;
            this.clb_TableList.HorizontalScrollbar = true;
            this.clb_TableList.Name = "clb_TableList";
            this.clb_TableList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clb_TableList.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.clb_TableList.TabIndex = 23;
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
            // SQL_DB_To_Excel
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SQL_DB_To_Excel";
            this.Text = "Export database to Excel";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 800);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1000, 680);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.flp_LoadFrom.ResumeLayout(false);
            this.flp_LoadFrom.PerformLayout();
            this.flp_Connect.ResumeLayout(false);
            this.flp_Connect.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tlp_Save.ResumeLayout(false);
            this.tlp_Save.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlp_Tables.ResumeLayout(false);
            this.tlp_Tables.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.FlowLayoutPanel flp_LoadFrom;
        private System.Windows.Forms.Label lbl_LoadFrom;
        private System.Windows.Forms.FlowLayoutPanel flp_Connect;
        private System.Windows.Forms.TableLayoutPanel tlp_Save;
        private System.Windows.Forms.TableLayoutPanel tlp_Tables;

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_ExportToExcel;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.CheckedListBox clb_TableList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_SelectAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox cb_SkipEmptyTable;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btn_Cancel;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.RadioButton rb_TRG;
        private System.Windows.Forms.RadioButton rb_SRC;
        private System.Windows.Forms.Label lbl_Find;
        private System.Windows.Forms.TextBox tb_Find;
    }
}