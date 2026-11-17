namespace SPI_Data_Comp_Tool
{
    partial class User_Settings
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
            this.gb_SaveExcel = new System.Windows.Forms.GroupBox();
            this.rb_XLSave = new System.Windows.Forms.RadioButton();
            this.rb_EPPlus = new System.Windows.Forms.RadioButton();
            this.gb_Logs = new System.Windows.Forms.GroupBox();
            this.btn_ClearLogs = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_SaveExcel = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Logs = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.gb_SaveExcel.SuspendLayout();
            this.tlp_SaveExcel.SuspendLayout();
            this.gb_Logs.SuspendLayout();
            this.tlp_Logs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 1;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowCount = 2;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.gb_SaveExcel, 0, 0);
            this.tlp_Root.Controls.Add(this.gb_Logs, 0, 1);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // gb_SaveExcel
            // 
            this.gb_SaveExcel.Name = "gb_SaveExcel";
            this.gb_SaveExcel.TabStop = false;
            this.gb_SaveExcel.Text = "Save Excel files with";
            this.gb_SaveExcel.Controls.Add(this.tlp_SaveExcel);
            this.gb_SaveExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_SaveExcel.AutoSize = true;
            this.gb_SaveExcel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gb_SaveExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.gb_SaveExcel.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_SaveExcel.TabIndex = 1;
            // 
            // tlp_SaveExcel
            // 
            this.tlp_SaveExcel.Name = "tlp_SaveExcel";
            this.tlp_SaveExcel.ColumnCount = 1;
            this.tlp_SaveExcel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SaveExcel.RowCount = 2;
            this.tlp_SaveExcel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SaveExcel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SaveExcel.Controls.Add(this.rb_EPPlus, 0, 0);
            this.tlp_SaveExcel.Controls.Add(this.rb_XLSave, 0, 1);
            this.tlp_SaveExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_SaveExcel.AutoSize = true;
            this.tlp_SaveExcel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_SaveExcel.TabIndex = 2;
            // 
            // rb_EPPlus
            // 
            this.rb_EPPlus.Checked = true;
            this.rb_EPPlus.Name = "rb_EPPlus";
            this.rb_EPPlus.TabStop = true;
            this.rb_EPPlus.UseVisualStyleBackColor = true;
            this.rb_EPPlus.CheckedChanged += new System.EventHandler(this.rb_EPPlus_CheckedChanged);
            this.rb_EPPlus.Text = "EPPlus (recommended, no Excel needed)";
            this.rb_EPPlus.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.rb_EPPlus.AutoSize = true;
            this.rb_EPPlus.TabIndex = 3;
            // 
            // rb_XLSave
            // 
            this.rb_XLSave.Name = "rb_XLSave";
            this.rb_XLSave.UseVisualStyleBackColor = true;
            this.rb_XLSave.CheckedChanged += new System.EventHandler(this.rb_XLSave_CheckedChanged);
            this.rb_XLSave.Text = "Microsoft Excel (needs Excel installed)";
            this.rb_XLSave.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.rb_XLSave.AutoSize = true;
            this.rb_XLSave.TabIndex = 4;
            // 
            // gb_Logs
            // 
            this.gb_Logs.Name = "gb_Logs";
            this.gb_Logs.TabStop = false;
            this.gb_Logs.Text = "Log";
            this.gb_Logs.Controls.Add(this.tlp_Logs);
            this.gb_Logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Logs.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Logs.TabIndex = 5;
            // 
            // tlp_Logs
            // 
            this.tlp_Logs.Name = "tlp_Logs";
            this.tlp_Logs.ColumnCount = 2;
            this.tlp_Logs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Logs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Logs.RowCount = 2;
            this.tlp_Logs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Logs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Logs.Controls.Add(this.rtb_Log, 0, 0);
            this.tlp_Logs.SetColumnSpan(this.rtb_Log, 2);
            this.tlp_Logs.Controls.Add(this.btn_ClearLogs, 0, 1);
            this.tlp_Logs.Controls.Add(this.lbl_Progress, 1, 1);
            this.tlp_Logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Logs.AutoSize = false;
            this.tlp_Logs.TabIndex = 6;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.rtb_Log.TabIndex = 7;
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.Color.White;
            this.btn_ClearLogs.Name = "btn_ClearLogs";
            this.btn_ClearLogs.UseVisualStyleBackColor = false;
            this.btn_ClearLogs.Text = "Clear log";
            this.btn_ClearLogs.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.btn_ClearLogs.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_ClearLogs.Size = new System.Drawing.Size(110, 30);
            this.btn_ClearLogs.TabIndex = 8;
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Text = "";
            this.lbl_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lbl_Progress.TabIndex = 9;
            // 
            // User_Settings
            // 
            this.Name = "User_Settings";
            this.Text = "User Settings";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 440);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(560, 380);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.gb_SaveExcel.ResumeLayout(false);
            this.gb_SaveExcel.PerformLayout();
            this.tlp_SaveExcel.ResumeLayout(false);
            this.tlp_SaveExcel.PerformLayout();
            this.gb_Logs.ResumeLayout(false);
            this.gb_Logs.PerformLayout();
            this.tlp_Logs.ResumeLayout(false);
            this.tlp_Logs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_SaveExcel;
        private System.Windows.Forms.TableLayoutPanel tlp_Logs;

        private System.Windows.Forms.GroupBox gb_SaveExcel;
        private System.Windows.Forms.RadioButton rb_XLSave;
        private System.Windows.Forms.RadioButton rb_EPPlus;
        private System.Windows.Forms.GroupBox gb_Logs;
        private System.Windows.Forms.Button btn_ClearLogs;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Label lbl_Progress;
    }
}