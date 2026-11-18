namespace SPI_Data_Comp_Tool
{
    partial class SPIDeltaChangeLegacyID
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SPIDeltaChangeLegacyID));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Connect_Reference_ID = new System.Windows.Forms.Button();
            this.btn_ListTableExcel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Run = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlp_Run.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tlp_Status.SuspendLayout();
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
            this.tlp_Root.Controls.Add(this.groupBox1, 1, 0);
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
            // groupBox1
            // 
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Run";
            this.groupBox1.Controls.Add(this.tlp_Run);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox1.TabIndex = 4;
            // 
            // tlp_Run
            // 
            this.tlp_Run.Name = "tlp_Run";
            this.tlp_Run.ColumnCount = 1;
            this.tlp_Run.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Run.RowCount = 4;
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Run.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Run.Controls.Add(this.btn_Connect_Reference_ID, 0, 0);
            this.tlp_Run.Controls.Add(this.label1, 0, 1);
            this.tlp_Run.Controls.Add(this.btn_ListTableExcel, 0, 2);
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
            this.btn_Connect_Reference_ID.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.btn_Connect_Reference_ID.Size = new System.Drawing.Size(160, 32);
            this.btn_Connect_Reference_ID.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Name = "label1";
            this.label1.Text = "Choose the Excel file to generate legacy IDs from.";
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.label1.AutoSize = true;
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.label1.TabIndex = 7;
            // 
            // btn_ListTableExcel
            // 
            this.btn_ListTableExcel.Name = "btn_ListTableExcel";
            this.btn_ListTableExcel.UseVisualStyleBackColor = true;
            this.btn_ListTableExcel.Click += new System.EventHandler(this.btn_ListTableExcel_Click);
            this.btn_ListTableExcel.Text = "Browse…";
            this.btn_ListTableExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ListTableExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_ListTableExcel.Size = new System.Drawing.Size(160, 36);
            this.btn_ListTableExcel.TabIndex = 8;
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
            this.groupBox10.TabIndex = 9;
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
            this.tlp_Status.TabIndex = 10;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 11;
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Name = "btn_Clear_Reset";
            this.btn_Clear_Reset.UseVisualStyleBackColor = true;
            this.btn_Clear_Reset.Click += new System.EventHandler(this.btn_Clear_Reset_Click);
            this.btn_Clear_Reset.Text = "Clear / Reset";
            this.btn_Clear_Reset.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Clear_Reset.Size = new System.Drawing.Size(110, 30);
            this.btn_Clear_Reset.TabIndex = 12;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.rtb_Log.TabIndex = 13;
            // 
            // SPIDeltaChangeLegacyID
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SPIDeltaChangeLegacyID";
            this.Text = "SPI delta change legacy ID";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 860);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1080, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlp_Run.ResumeLayout(false);
            this.tlp_Run.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Run;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_ListTableExcel;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.Button btn_Clear_Reset;
        private System.Windows.Forms.Button btn_Connect_Reference_ID;
    }
}