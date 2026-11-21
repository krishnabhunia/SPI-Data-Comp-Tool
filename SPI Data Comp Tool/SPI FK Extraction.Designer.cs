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
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.step_Strip = new SPI_Data_Comp_Tool.StepStrip();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Run = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Log = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.gb_DBConnection.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlp_Run.SuspendLayout();
            this.gb_Logs.SuspendLayout();
            this.tlp_Log.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tlp_Root.RowCount = 3;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.step_Strip, 0, 0);
            this.tlp_Root.SetColumnSpan(this.step_Strip, 2);
            this.tlp_Root.Controls.Add(this.gb_DBConnection, 0, 1);
            this.tlp_Root.Controls.Add(this.groupBox1, 1, 1);
            this.tlp_Root.Controls.Add(this.gb_Logs, 0, 2);
            this.tlp_Root.SetColumnSpan(this.gb_Logs, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // step_Strip
            // 
            this.step_Strip.Name = "step_Strip";
            this.step_Strip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.step_Strip.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.step_Strip.Size = new System.Drawing.Size(800, 66);
            this.step_Strip.TabIndex = 1;
            // 
            // gb_DBConnection
            // 
            this.gb_DBConnection.Name = "gb_DBConnection";
            this.gb_DBConnection.TabStop = false;
            this.gb_DBConnection.Text = "Connection";
            this.gb_DBConnection.Controls.Add(this.tlp_Connection);
            this.gb_DBConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_DBConnection.AutoSize = true;
            this.gb_DBConnection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gb_DBConnection.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.gb_DBConnection.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_DBConnection.TabIndex = 2;
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
            this.tlp_Connection.Controls.Add(this.btn_TestDBConnection, 0, 1);
            this.tlp_Connection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Connection.AutoSize = true;
            this.tlp_Connection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Connection.TabIndex = 3;
            // 
            // connection
            // 
            this.connection.Name = "connection";
            this.connection.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.connection.AutoSize = true;
            this.connection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.connection.TabIndex = 4;
            // 
            // btn_TestDBConnection
            // 
            this.btn_TestDBConnection.Name = "btn_TestDBConnection";
            this.btn_TestDBConnection.UseVisualStyleBackColor = true;
            this.btn_TestDBConnection.Click += new System.EventHandler(this.btn_TestDBConnection_Click);
            this.btn_TestDBConnection.Text = "Test connection";
            this.btn_TestDBConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btn_TestDBConnection.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.btn_TestDBConnection.Size = new System.Drawing.Size(150, 30);
            this.btn_TestDBConnection.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Run";
            this.groupBox1.Controls.Add(this.tlp_Run);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox1.TabIndex = 6;
            // 
            // tlp_Run
            // 
            this.tlp_Run.Name = "tlp_Run";
            this.tlp_Run.ColumnCount = 1;
            this.tlp_Run.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Run.RowCount = 2;
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Run.Controls.Add(this.btn_ImportExcel, 0, 0);
            this.tlp_Run.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Run.AutoSize = false;
            this.tlp_Run.TabIndex = 7;
            // 
            // btn_ImportExcel
            // 
            this.btn_ImportExcel.Name = "btn_ImportExcel";
            this.btn_ImportExcel.UseVisualStyleBackColor = true;
            this.btn_ImportExcel.Click += new System.EventHandler(this.btn_ImportExcel_Click);
            this.btn_ImportExcel.Text = "Import Excel…";
            this.btn_ImportExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ImportExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_ImportExcel.Size = new System.Drawing.Size(160, 36);
            this.btn_ImportExcel.TabIndex = 8;
            // 
            // gb_Logs
            // 
            this.gb_Logs.Name = "gb_Logs";
            this.gb_Logs.TabStop = false;
            this.gb_Logs.Text = "Log";
            this.gb_Logs.Controls.Add(this.tlp_Log);
            this.gb_Logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Logs.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.gb_Logs.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Logs.TabIndex = 9;
            // 
            // tlp_Log
            // 
            this.tlp_Log.Name = "tlp_Log";
            this.tlp_Log.ColumnCount = 2;
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowCount = 3;
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.Controls.Add(this.progressBar1, 0, 0);
            this.tlp_Log.Controls.Add(this.btn_ClearLogs, 1, 0);
            this.tlp_Log.Controls.Add(this.lbl_Progress, 0, 1);
            this.tlp_Log.SetColumnSpan(this.lbl_Progress, 2);
            this.tlp_Log.Controls.Add(this.rtb_Log, 0, 2);
            this.tlp_Log.SetColumnSpan(this.rtb_Log, 2);
            this.tlp_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Log.AutoSize = false;
            this.tlp_Log.TabIndex = 10;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 11;
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.Color.White;
            this.btn_ClearLogs.Name = "btn_ClearLogs";
            this.btn_ClearLogs.UseVisualStyleBackColor = false;
            this.btn_ClearLogs.Click += new System.EventHandler(this.btn_ClearLogs_Click);
            this.btn_ClearLogs.Text = "Clear log";
            this.btn_ClearLogs.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_ClearLogs.Size = new System.Drawing.Size(110, 30);
            this.btn_ClearLogs.TabIndex = 12;
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Text = "Progress";
            this.lbl_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.lbl_Progress.TabIndex = 13;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.rtb_Log.TabIndex = 14;
            // 
            // SPI_FK_Extraction
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SPI_FK_Extraction";
            this.Text = "SPI FK extraction";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 760);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 640);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.gb_DBConnection.ResumeLayout(false);
            this.gb_DBConnection.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlp_Run.ResumeLayout(false);
            this.tlp_Run.PerformLayout();
            this.gb_Logs.ResumeLayout(false);
            this.gb_Logs.PerformLayout();
            this.tlp_Log.ResumeLayout(false);
            this.tlp_Log.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SPI_Data_Comp_Tool.StepStrip step_Strip;

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Run;
        private System.Windows.Forms.TableLayoutPanel tlp_Log;

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_ImportExcel;
        private System.Windows.Forms.GroupBox gb_Logs;
        private System.Windows.Forms.Button btn_ClearLogs;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Label lbl_Progress;
        private System.Windows.Forms.GroupBox gb_DBConnection;
        private System.Windows.Forms.Button btn_TestDBConnection;
    }
}