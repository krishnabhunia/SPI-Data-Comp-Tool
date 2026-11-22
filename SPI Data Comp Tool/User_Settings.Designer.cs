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
            this.tlp_General = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_SaveExcel = new System.Windows.Forms.TableLayoutPanel();
            this.gb_Theme = new System.Windows.Forms.GroupBox();
            this.tlp_Theme = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_Theme = new System.Windows.Forms.Label();
            this.cb_Theme = new System.Windows.Forms.ComboBox();
            this.swatch_Theme = new SPI_Data_Comp_Tool.ThemeSwatch();
            this.lbl_ThemeHelp = new System.Windows.Forms.Label();
            this.tab_Rules = new System.Windows.Forms.TabPage();
            this.tlp_Rules = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_RulesHelp = new System.Windows.Forms.Label();
            this.lbl_RulesPath = new System.Windows.Forms.Label();
            this.tb_RulesPath = new System.Windows.Forms.TextBox();
            this.btn_ImportRules = new System.Windows.Forms.Button();
            this.lbl_RulesStatus = new System.Windows.Forms.Label();
            this.tab_Updates = new System.Windows.Forms.TabPage();
            this.updatesControl = new SPI_Data_Comp_Tool.UpdatesControl();
            this.tab_About = new System.Windows.Forms.TabPage();
            this.pnl_About = new System.Windows.Forms.Panel();
            this.tlp_Logs = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.tabs_Settings.SuspendLayout();
            this.tab_General.SuspendLayout();
            this.tlp_General.SuspendLayout();
            this.gb_SaveExcel.SuspendLayout();
            this.tlp_SaveExcel.SuspendLayout();
            this.gb_Theme.SuspendLayout();
            this.tlp_Theme.SuspendLayout();
            this.tab_Rules.SuspendLayout();
            this.tlp_Rules.SuspendLayout();
            this.tab_Updates.SuspendLayout();
            this.tab_About.SuspendLayout();
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
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tlp_Root.Controls.Add(this.tabs_Settings, 0, 0);
            this.tlp_Root.Controls.Add(this.gb_Logs, 0, 1);
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
            this.tabs_Settings.Controls.Add(this.tab_About);
            this.tabs_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs_Settings.TabIndex = 1;
            // 
            // tab_General
            // 
            this.tab_General.Name = "tab_General";
            this.tab_General.Text = "General";
            this.tab_General.Controls.Add(this.tlp_General);
            this.tab_General.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tab_General.TabIndex = 2;
            // 
            // tlp_General
            // 
            this.tlp_General.Name = "tlp_General";
            this.tlp_General.ColumnCount = 1;
            this.tlp_General.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_General.RowCount = 3;
            this.tlp_General.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tlp_General.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tlp_General.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_General.Controls.Add(this.gb_SaveExcel, 0, 0);
            this.tlp_General.Controls.Add(this.gb_Theme, 0, 1);
            this.tlp_General.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_General.AutoSize = false;
            this.tlp_General.TabIndex = 3;
            // 
            // gb_SaveExcel
            // 
            this.gb_SaveExcel.Name = "gb_SaveExcel";
            this.gb_SaveExcel.TabStop = false;
            this.gb_SaveExcel.Text = "Save Excel files with";
            this.gb_SaveExcel.Controls.Add(this.tlp_SaveExcel);
            this.gb_SaveExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_SaveExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.gb_SaveExcel.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_SaveExcel.TabIndex = 4;
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
            this.tlp_SaveExcel.TabIndex = 5;
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
            this.rb_EPPlus.TabIndex = 6;
            // 
            // rb_XLSave
            // 
            this.rb_XLSave.Name = "rb_XLSave";
            this.rb_XLSave.UseVisualStyleBackColor = true;
            this.rb_XLSave.CheckedChanged += new System.EventHandler(this.rb_XLSave_CheckedChanged);
            this.rb_XLSave.Text = "Microsoft Excel (needs Excel installed)";
            this.rb_XLSave.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.rb_XLSave.AutoSize = true;
            this.rb_XLSave.TabIndex = 7;
            // 
            // gb_Theme
            // 
            this.gb_Theme.Name = "gb_Theme";
            this.gb_Theme.Text = "Colour theme";
            this.gb_Theme.Controls.Add(this.tlp_Theme);
            this.gb_Theme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Theme.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.gb_Theme.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Theme.TabIndex = 8;
            // 
            // tlp_Theme
            // 
            this.tlp_Theme.Name = "tlp_Theme";
            this.tlp_Theme.ColumnCount = 3;
            this.tlp_Theme.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Theme.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Theme.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Theme.RowCount = 2;
            this.tlp_Theme.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Theme.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Theme.Controls.Add(this.lbl_Theme, 0, 0);
            this.tlp_Theme.Controls.Add(this.cb_Theme, 1, 0);
            this.tlp_Theme.Controls.Add(this.swatch_Theme, 2, 0);
            this.tlp_Theme.Controls.Add(this.lbl_ThemeHelp, 0, 1);
            this.tlp_Theme.SetColumnSpan(this.lbl_ThemeHelp, 3);
            this.tlp_Theme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Theme.AutoSize = true;
            this.tlp_Theme.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Theme.TabIndex = 9;
            // 
            // lbl_Theme
            // 
            this.lbl_Theme.Name = "lbl_Theme";
            this.lbl_Theme.Text = "Theme";
            this.lbl_Theme.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Theme.AutoSize = true;
            this.lbl_Theme.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lbl_Theme.TabIndex = 10;
            // 
            // cb_Theme
            // 
            this.cb_Theme.Name = "cb_Theme";
            this.cb_Theme.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_Theme.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.cb_Theme.Size = new System.Drawing.Size(200, 23);
            this.cb_Theme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Theme.AccessibleName = "Colour theme";
            this.cb_Theme.TabIndex = 11;
            // 
            // swatch_Theme
            // 
            this.swatch_Theme.Name = "swatch_Theme";
            this.swatch_Theme.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.swatch_Theme.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.swatch_Theme.Size = new System.Drawing.Size(240, 22);
            this.swatch_Theme.AccessibleName = "Colours of the chosen theme";
            this.swatch_Theme.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.swatch_Theme.TabIndex = 12;
            // 
            // lbl_ThemeHelp
            // 
            this.lbl_ThemeHelp.Name = "lbl_ThemeHelp";
            this.lbl_ThemeHelp.Text = "Changes the colours of every open window at once and is remembered for next time.";
            this.lbl_ThemeHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_ThemeHelp.AutoSize = true;
            this.lbl_ThemeHelp.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.lbl_ThemeHelp.TabIndex = 13;
            // 
            // tab_Rules
            // 
            this.tab_Rules.Name = "tab_Rules";
            this.tab_Rules.Text = "Comparison rules";
            this.tab_Rules.Controls.Add(this.tlp_Rules);
            this.tab_Rules.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tab_Rules.TabIndex = 14;
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
            this.tlp_Rules.TabIndex = 15;
            // 
            // lbl_RulesHelp
            // 
            this.lbl_RulesHelp.Name = "lbl_RulesHelp";
            this.lbl_RulesHelp.Text = "Import the rules workbook used by database and Excel comparisons. A successful import is saved immediately.";
            this.lbl_RulesHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_RulesHelp.AutoSize = true;
            this.lbl_RulesHelp.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.lbl_RulesHelp.TabIndex = 16;
            // 
            // lbl_RulesPath
            // 
            this.lbl_RulesPath.Name = "lbl_RulesPath";
            this.lbl_RulesPath.Text = "Rules file";
            this.lbl_RulesPath.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_RulesPath.AutoSize = true;
            this.lbl_RulesPath.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lbl_RulesPath.TabIndex = 17;
            // 
            // tb_RulesPath
            // 
            this.tb_RulesPath.Name = "tb_RulesPath";
            this.tb_RulesPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_RulesPath.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tb_RulesPath.ReadOnly = true;
            this.tb_RulesPath.AccessibleName = "Comparison rules file";
            this.tb_RulesPath.TabIndex = 18;
            // 
            // btn_ImportRules
            // 
            this.btn_ImportRules.Name = "btn_ImportRules";
            this.btn_ImportRules.Text = "&Import rules…";
            this.btn_ImportRules.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.btn_ImportRules.Margin = new System.Windows.Forms.Padding(0, 10, 0, 6);
            this.btn_ImportRules.Size = new System.Drawing.Size(136, 32);
            this.btn_ImportRules.TabIndex = 19;
            // 
            // lbl_RulesStatus
            // 
            this.lbl_RulesStatus.Name = "lbl_RulesStatus";
            this.lbl_RulesStatus.Text = "No comparison rules file selected.";
            this.lbl_RulesStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_RulesStatus.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_RulesStatus.AutoEllipsis = true;
            this.lbl_RulesStatus.TabIndex = 20;
            // 
            // tab_Updates
            // 
            this.tab_Updates.Name = "tab_Updates";
            this.tab_Updates.Text = "Updates";
            this.tab_Updates.Controls.Add(this.updatesControl);
            this.tab_Updates.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tab_Updates.TabIndex = 21;
            // 
            // updatesControl
            // 
            this.updatesControl.Name = "updatesControl";
            this.updatesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updatesControl.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.updatesControl.TabIndex = 22;
            // 
            // tab_About
            // 
            this.tab_About.Name = "tab_About";
            this.tab_About.Text = "About";
            this.tab_About.Controls.Add(this.pnl_About);
            this.tab_About.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.tab_About.TabIndex = 23;
            // 
            // pnl_About
            // 
            this.pnl_About.Name = "pnl_About";
            this.pnl_About.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_About.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.pnl_About.TabIndex = 24;
            // 
            // gb_Logs
            // 
            this.gb_Logs.Name = "gb_Logs";
            this.gb_Logs.TabStop = false;
            this.gb_Logs.Text = "Log";
            this.gb_Logs.Controls.Add(this.tlp_Logs);
            this.gb_Logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Logs.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_Logs.TabIndex = 25;
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
            this.tlp_Logs.TabIndex = 26;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.rtb_Log.TabIndex = 27;
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
            this.btn_ClearLogs.TabIndex = 28;
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Text = "";
            this.lbl_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lbl_Progress.TabIndex = 29;
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
            this.tlp_General.ResumeLayout(false);
            this.tlp_General.PerformLayout();
            this.gb_SaveExcel.ResumeLayout(false);
            this.gb_SaveExcel.PerformLayout();
            this.tlp_SaveExcel.ResumeLayout(false);
            this.tlp_SaveExcel.PerformLayout();
            this.gb_Theme.ResumeLayout(false);
            this.gb_Theme.PerformLayout();
            this.tlp_Theme.ResumeLayout(false);
            this.tlp_Theme.PerformLayout();
            this.tab_Rules.ResumeLayout(false);
            this.tab_Rules.PerformLayout();
            this.tlp_Rules.ResumeLayout(false);
            this.tlp_Rules.PerformLayout();
            this.tab_Updates.ResumeLayout(false);
            this.tab_Updates.PerformLayout();
            this.tab_About.ResumeLayout(false);
            this.tab_About.PerformLayout();
            this.gb_Logs.ResumeLayout(false);
            this.gb_Logs.PerformLayout();
            this.tlp_Logs.ResumeLayout(false);
            this.tlp_Logs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage tab_About;
        private System.Windows.Forms.Panel pnl_About;

        private System.Windows.Forms.TableLayoutPanel tlp_General;
        private System.Windows.Forms.GroupBox gb_Theme;
        private System.Windows.Forms.TableLayoutPanel tlp_Theme;
        private System.Windows.Forms.Label lbl_Theme;
        private System.Windows.Forms.ComboBox cb_Theme;
        private SPI_Data_Comp_Tool.ThemeSwatch swatch_Theme;
        private System.Windows.Forms.Label lbl_ThemeHelp;

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