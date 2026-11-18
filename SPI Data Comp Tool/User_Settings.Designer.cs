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
            this.tabs_Settings = new System.Windows.Forms.TabControl();
            this.tab_General = new System.Windows.Forms.TabPage();
            this.tlp_SaveExcel = new System.Windows.Forms.TableLayoutPanel();
            this.tab_Rules = new System.Windows.Forms.TabPage();
            this.tlp_Rules = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_RulesHelp = new System.Windows.Forms.Label();
            this.lbl_RulesPath = new System.Windows.Forms.Label();
            this.tb_RulesPath = new System.Windows.Forms.TextBox();
            this.btn_ImportRules = new System.Windows.Forms.Button();
            this.lbl_RulesStatus = new System.Windows.Forms.Label();
            this.tab_Updates = new System.Windows.Forms.TabPage();
            this.updatesControl = new SPI_Data_Comp_Tool.UpdatesControl();
            this.tlp_Logs = new System.Windows.Forms.TableLayoutPanel();
            this.flp_SettingsActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_Close = new System.Windows.Forms.Button();
            this.tlp_Root.SuspendLayout();
            this.tabs_Settings.SuspendLayout();
            this.tab_General.SuspendLayout();
            this.gb_SaveExcel.SuspendLayout();
            this.tlp_SaveExcel.SuspendLayout();
            this.tab_Rules.SuspendLayout();
            this.tlp_Rules.SuspendLayout();
            this.tab_Updates.SuspendLayout();
            this.gb_Logs.SuspendLayout();
            this.tlp_Logs.SuspendLayout();
            this.flp_SettingsActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 1;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowCount = 3;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.Controls.Add(this.tabs_Settings, 0, 0);
            this.tlp_Root.Controls.Add(this.gb_Logs, 0, 1);
            this.tlp_Root.Controls.Add(this.flp_SettingsActions, 0, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // tabs_Settings
            // 
            this.tabs_Settings.Name = "tabs_Settings";
            this.tabs_Settings.Controls.Add(this.tab_General);
            this.tabs_Settings.Controls.Add(this.tab_Rules);
            this.tabs_Settings.Controls.Add(this.tab_Updates);
            this.tabs_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs_Settings.TabIndex = 1;
            // 
            // tab_General
            // 
            this.tab_General.Name = "tab_General";
            this.tab_General.Text = "General";
            this.tab_General.Controls.Add(this.gb_SaveExcel);
            this.tab_General.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tab_General.TabIndex = 2;
            // 
            // gb_SaveExcel
            // 
            this.gb_SaveExcel.Name = "gb_SaveExcel";
            this.gb_SaveExcel.TabStop = false;
            this.gb_SaveExcel.Text = "Save Excel files with";
            this.gb_SaveExcel.Controls.Add(this.tlp_SaveExcel);
            this.gb_SaveExcel.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb_SaveExcel.AutoSize = true;
            this.gb_SaveExcel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gb_SaveExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.gb_SaveExcel.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_SaveExcel.TabIndex = 3;
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
            this.tlp_SaveExcel.TabIndex = 4;
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
            this.rb_EPPlus.TabIndex = 5;
            // 
            // rb_XLSave
            // 
            this.rb_XLSave.Name = "rb_XLSave";
            this.rb_XLSave.UseVisualStyleBackColor = true;
            this.rb_XLSave.CheckedChanged += new System.EventHandler(this.rb_XLSave_CheckedChanged);
            this.rb_XLSave.Text = "Microsoft Excel (needs Excel installed)";
            this.rb_XLSave.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.rb_XLSave.AutoSize = true;
            this.rb_XLSave.TabIndex = 6;
            // 
            // tab_Rules
            // 
            this.tab_Rules.Name = "tab_Rules";
            this.tab_Rules.Text = "Comparison rules";
            this.tab_Rules.Controls.Add(this.tlp_Rules);
            this.tab_Rules.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tab_Rules.TabIndex = 7;
            // 
            // tlp_Rules
            // 
            this.tlp_Rules.Name = "tlp_Rules";
            this.tlp_Rules.ColumnCount = 2;
            this.tlp_Rules.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Rules.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Rules.RowCount = 4;
            this.tlp_Rules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Rules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Rules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Rules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Rules.Controls.Add(this.lbl_RulesHelp, 0, 0);
            this.tlp_Rules.SetColumnSpan(this.lbl_RulesHelp, 2);
            this.tlp_Rules.Controls.Add(this.lbl_RulesPath, 0, 1);
            this.tlp_Rules.Controls.Add(this.tb_RulesPath, 1, 1);
            this.tlp_Rules.Controls.Add(this.btn_ImportRules, 0, 2);
            this.tlp_Rules.SetColumnSpan(this.btn_ImportRules, 2);
            this.tlp_Rules.Controls.Add(this.lbl_RulesStatus, 0, 3);
            this.tlp_Rules.SetColumnSpan(this.lbl_RulesStatus, 2);
            this.tlp_Rules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Rules.AutoSize = false;
            this.tlp_Rules.TabIndex = 8;
            // 
            // lbl_RulesHelp
            // 
            this.lbl_RulesHelp.Name = "lbl_RulesHelp";
            this.lbl_RulesHelp.Text = "Import the rules workbook used by database and Excel comparisons. A successful import is saved immediately.";
            this.lbl_RulesHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_RulesHelp.AutoSize = true;
            this.lbl_RulesHelp.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.lbl_RulesHelp.TabIndex = 9;
            // 
            // lbl_RulesPath
            // 
            this.lbl_RulesPath.Name = "lbl_RulesPath";
            this.lbl_RulesPath.Text = "Rules file";
            this.lbl_RulesPath.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_RulesPath.AutoSize = true;
            this.lbl_RulesPath.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lbl_RulesPath.TabIndex = 10;
            // 
            // tb_RulesPath
            // 
            this.tb_RulesPath.Name = "tb_RulesPath";
            this.tb_RulesPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_RulesPath.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tb_RulesPath.ReadOnly = true;
            this.tb_RulesPath.AccessibleName = "Comparison rules file";
            this.tb_RulesPath.TabIndex = 11;
            // 
            // btn_ImportRules
            // 
            this.btn_ImportRules.Name = "btn_ImportRules";
            this.btn_ImportRules.Text = "&Import rules…";
            this.btn_ImportRules.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.btn_ImportRules.Margin = new System.Windows.Forms.Padding(0, 10, 0, 6);
            this.btn_ImportRules.Size = new System.Drawing.Size(136, 32);
            this.btn_ImportRules.TabIndex = 12;
            // 
            // lbl_RulesStatus
            // 
            this.lbl_RulesStatus.Name = "lbl_RulesStatus";
            this.lbl_RulesStatus.Text = "No comparison rules file selected.";
            this.lbl_RulesStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_RulesStatus.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_RulesStatus.AutoEllipsis = true;
            this.lbl_RulesStatus.TabIndex = 13;
            // 
            // tab_Updates
            // 
            this.tab_Updates.Name = "tab_Updates";
            this.tab_Updates.Text = "Updates";
            this.tab_Updates.Controls.Add(this.updatesControl);
            this.tab_Updates.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tab_Updates.TabIndex = 14;
            // 
            // updatesControl
            // 
            this.updatesControl.Name = "updatesControl";
            this.updatesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updatesControl.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.updatesControl.TabIndex = 15;
            // 
            // gb_Logs
            // 
            this.gb_Logs.Name = "gb_Logs";
            this.gb_Logs.TabStop = false;
            this.gb_Logs.Text = "Log";
            this.gb_Logs.Controls.Add(this.tlp_Logs);
            this.gb_Logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Logs.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Logs.TabIndex = 16;
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
            this.tlp_Logs.TabIndex = 17;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.rtb_Log.TabIndex = 18;
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
            this.btn_ClearLogs.TabIndex = 19;
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Text = "";
            this.lbl_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lbl_Progress.TabIndex = 20;
            // 
            // flp_SettingsActions
            // 
            this.flp_SettingsActions.Name = "flp_SettingsActions";
            this.flp_SettingsActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flp_SettingsActions.WrapContents = false;
            this.flp_SettingsActions.Controls.Add(this.btn_Close);
            this.flp_SettingsActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp_SettingsActions.AutoSize = true;
            this.flp_SettingsActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flp_SettingsActions.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flp_SettingsActions.TabIndex = 21;
            // 
            // btn_Close
            // 
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Text = "Close";
            this.btn_Close.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Close.Size = new System.Drawing.Size(100, 32);
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Close.TabIndex = 22;
            // 
            // User_Settings
            // 
            this.Name = "User_Settings";
            this.Text = "Settings";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(760, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.tabs_Settings.ResumeLayout(false);
            this.tabs_Settings.PerformLayout();
            this.tab_General.ResumeLayout(false);
            this.tab_General.PerformLayout();
            this.gb_SaveExcel.ResumeLayout(false);
            this.gb_SaveExcel.PerformLayout();
            this.tlp_SaveExcel.ResumeLayout(false);
            this.tlp_SaveExcel.PerformLayout();
            this.tab_Rules.ResumeLayout(false);
            this.tab_Rules.PerformLayout();
            this.tlp_Rules.ResumeLayout(false);
            this.tlp_Rules.PerformLayout();
            this.tab_Updates.ResumeLayout(false);
            this.tab_Updates.PerformLayout();
            this.gb_Logs.ResumeLayout(false);
            this.gb_Logs.PerformLayout();
            this.tlp_Logs.ResumeLayout(false);
            this.tlp_Logs.PerformLayout();
            this.flp_SettingsActions.ResumeLayout(false);
            this.flp_SettingsActions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage tab_Updates;
        private SPI_Data_Comp_Tool.UpdatesControl updatesControl;

        private System.Windows.Forms.TabControl tabs_Settings;
        private System.Windows.Forms.TabPage tab_General;
        private System.Windows.Forms.TabPage tab_Rules;
        private System.Windows.Forms.TableLayoutPanel tlp_Rules;
        private System.Windows.Forms.Label lbl_RulesHelp;
        private System.Windows.Forms.Label lbl_RulesPath;
        private System.Windows.Forms.TextBox tb_RulesPath;
        private System.Windows.Forms.Button btn_ImportRules;
        private System.Windows.Forms.Label lbl_RulesStatus;
        private System.Windows.Forms.FlowLayoutPanel flp_SettingsActions;
        private System.Windows.Forms.Button btn_Close;

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