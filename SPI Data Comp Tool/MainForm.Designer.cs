namespace SPI_Data_Comp_Tool
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_LoginStatus = new System.Windows.Forms.Label();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_Header = new System.Windows.Forms.Panel();
            this.tlp_Header = new System.Windows.Forms.TableLayoutPanel();
            this.ico_Header = new SPI_Data_Comp_Tool.GlyphIcon();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.lbl_Subtitle = new System.Windows.Forms.Label();
            this.tlp_Workspaces = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_Start = new System.Windows.Forms.Label();
            this.btn_WorkspaceComparison = new SPI_Data_Comp_Tool.TileButton();
            this.btn_WorkspaceScripts = new SPI_Data_Comp_Tool.TileButton();
            this.btn_WorkspaceReports = new SPI_Data_Comp_Tool.TileButton();
            this.btn_WorkspaceSettings = new SPI_Data_Comp_Tool.TileButton();
            this.pnl_Status = new System.Windows.Forms.Panel();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_Version = new System.Windows.Forms.Label();
            this.tlp_Root.SuspendLayout();
            this.pnl_Header.SuspendLayout();
            this.tlp_Header.SuspendLayout();
            this.tlp_Workspaces.SuspendLayout();
            this.pnl_Status.SuspendLayout();
            this.tlp_Status.SuspendLayout();
            this.SuspendLayout();
            //
            // tlp_Root
            //
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 1;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowCount = 3;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.Controls.Add(this.pnl_Header, 0, 0);
            this.tlp_Root.Controls.Add(this.tlp_Workspaces, 0, 1);
            this.tlp_Root.Controls.Add(this.pnl_Status, 0, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            //
            // pnl_Header
            //
            this.pnl_Header.Name = "pnl_Header";
            this.pnl_Header.Controls.Add(this.tlp_Header);
            this.pnl_Header.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Header.AutoSize = true;
            this.pnl_Header.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnl_Header.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.pnl_Header.Padding = new System.Windows.Forms.Padding(20, 14, 20, 14);
            this.pnl_Header.TabIndex = 1;
            //
            // tlp_Header
            //
            this.tlp_Header.Name = "tlp_Header";
            this.tlp_Header.ColumnCount = 2;
            this.tlp_Header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Header.RowCount = 2;
            this.tlp_Header.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Header.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Header.Controls.Add(this.ico_Header, 0, 0);
            this.tlp_Header.SetRowSpan(this.ico_Header, 2);
            this.tlp_Header.Controls.Add(this.lbl_Title, 1, 0);
            this.tlp_Header.Controls.Add(this.lbl_Subtitle, 1, 1);
            this.tlp_Header.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Header.AutoSize = true;
            this.tlp_Header.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Header.TabIndex = 2;
            //
            // ico_Header
            //
            this.ico_Header.Name = "ico_Header";
            this.ico_Header.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.ico_Header.Margin = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.ico_Header.Size = new System.Drawing.Size(40, 40);
            this.ico_Header.Glyph = SPI_Data_Comp_Tool.Glyph.Database;
            this.ico_Header.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.ico_Header.TabIndex = 3;
            //
            // lbl_Title
            //
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Text = "SPI Data Comparison Tool";
            this.lbl_Title.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Title.AutoSize = true;
            this.lbl_Title.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_Title.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.TabIndex = 4;
            //
            // lbl_Subtitle
            //
            this.lbl_Subtitle.Name = "lbl_Subtitle";
            this.lbl_Subtitle.Text = "Oracle, SQL Server and Excel — four connected workspaces";
            this.lbl_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Subtitle.AutoSize = true;
            this.lbl_Subtitle.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.lbl_Subtitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lbl_Subtitle.TabIndex = 5;
            //
            // tlp_Workspaces
            //
            this.tlp_Workspaces.Name = "tlp_Workspaces";
            this.tlp_Workspaces.ColumnCount = 2;
            this.tlp_Workspaces.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Workspaces.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Workspaces.RowCount = 4;
            this.tlp_Workspaces.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Workspaces.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Workspaces.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Workspaces.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Workspaces.Controls.Add(this.lbl_Start, 0, 0);
            this.tlp_Workspaces.SetColumnSpan(this.lbl_Start, 2);
            this.tlp_Workspaces.Controls.Add(this.btn_WorkspaceComparison, 0, 1);
            this.tlp_Workspaces.Controls.Add(this.btn_WorkspaceScripts, 1, 1);
            this.tlp_Workspaces.Controls.Add(this.btn_WorkspaceReports, 0, 2);
            this.tlp_Workspaces.Controls.Add(this.btn_WorkspaceSettings, 1, 2);
            this.tlp_Workspaces.Controls.Add(this.label3, 0, 3);
            this.tlp_Workspaces.SetColumnSpan(this.label3, 2);
            this.tlp_Workspaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Workspaces.AutoSize = false;
            this.tlp_Workspaces.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_Workspaces.Padding = new System.Windows.Forms.Padding(14, 14, 14, 14);
            this.tlp_Workspaces.TabIndex = 6;
            //
            // lbl_Start
            //
            this.lbl_Start.Name = "lbl_Start";
            this.lbl_Start.Text = "Choose a workspace";
            this.lbl_Start.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Start.AutoSize = true;
            this.lbl_Start.Margin = new System.Windows.Forms.Padding(6, 0, 0, 6);
            this.lbl_Start.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lbl_Start.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbl_Start.TabIndex = 7;
            //
            // btn_WorkspaceComparison
            //
            this.btn_WorkspaceComparison.Name = "btn_WorkspaceComparison";
            this.btn_WorkspaceComparison.Text = "Comparison";
            this.btn_WorkspaceComparison.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_WorkspaceComparison.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btn_WorkspaceComparison.Size = new System.Drawing.Size(400, 94);
            this.btn_WorkspaceComparison.Tone = 0;
            this.btn_WorkspaceComparison.Glyph = SPI_Data_Comp_Tool.Glyph.Database;
            this.btn_WorkspaceComparison.Hint = "Databases, Excel, batch comparisons and advanced mapping";
            this.btn_WorkspaceComparison.TabIndex = 8;
            //
            // btn_WorkspaceScripts
            //
            this.btn_WorkspaceScripts.Name = "btn_WorkspaceScripts";
            this.btn_WorkspaceScripts.Text = "SQL Scripts";
            this.btn_WorkspaceScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_WorkspaceScripts.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btn_WorkspaceScripts.Size = new System.Drawing.Size(400, 94);
            this.btn_WorkspaceScripts.Tone = 3;
            this.btn_WorkspaceScripts.Glyph = SPI_Data_Comp_Tool.Glyph.Sheet;
            this.btn_WorkspaceScripts.Hint = "Generate scripts or explicitly execute a workbook";
            this.btn_WorkspaceScripts.TabIndex = 9;
            //
            // btn_WorkspaceReports
            //
            this.btn_WorkspaceReports.Name = "btn_WorkspaceReports";
            this.btn_WorkspaceReports.Text = "Data & Reports";
            this.btn_WorkspaceReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_WorkspaceReports.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btn_WorkspaceReports.Size = new System.Drawing.Size(400, 94);
            this.btn_WorkspaceReports.Tone = 0;
            this.btn_WorkspaceReports.Glyph = SPI_Data_Comp_Tool.Glyph.Export;
            this.btn_WorkspaceReports.Hint = "Database exports, Excel columns and merge logs";
            this.btn_WorkspaceReports.TabIndex = 10;
            //
            // btn_WorkspaceSettings
            //
            this.btn_WorkspaceSettings.Name = "btn_WorkspaceSettings";
            this.btn_WorkspaceSettings.Text = "Settings";
            this.btn_WorkspaceSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_WorkspaceSettings.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btn_WorkspaceSettings.Size = new System.Drawing.Size(400, 94);
            this.btn_WorkspaceSettings.Tone = 1;
            this.btn_WorkspaceSettings.Glyph = SPI_Data_Comp_Tool.Glyph.Database;
            this.btn_WorkspaceSettings.Hint = "Profiles, rules, colour themes and stable/beta updates";
            this.btn_WorkspaceSettings.TabIndex = 11;
            //
            // label3
            //
            this.label3.Name = "label3";
            this.label3.Text = "label3";
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.AutoSize = true;
            this.label3.Margin = new System.Windows.Forms.Padding(6, 10, 6, 0);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.label3.TabIndex = 12;
            //
            // pnl_Status
            //
            this.pnl_Status.Name = "pnl_Status";
            this.pnl_Status.Controls.Add(this.tlp_Status);
            this.pnl_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Status.AutoSize = true;
            this.pnl_Status.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnl_Status.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.pnl_Status.Padding = new System.Windows.Forms.Padding(14, 7, 14, 7);
            this.pnl_Status.TabIndex = 13;
            //
            // tlp_Status
            //
            this.tlp_Status.Name = "tlp_Status";
            this.tlp_Status.ColumnCount = 2;
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.RowCount = 1;
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.Controls.Add(this.lbl_LoginStatus, 0, 0);
            this.tlp_Status.Controls.Add(this.lbl_Version, 1, 0);
            this.tlp_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Status.AutoSize = true;
            this.tlp_Status.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Status.TabIndex = 14;
            //
            // lbl_LoginStatus
            //
            this.lbl_LoginStatus.Name = "lbl_LoginStatus";
            this.lbl_LoginStatus.Text = "Ready";
            this.lbl_LoginStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_LoginStatus.AutoSize = true;
            this.lbl_LoginStatus.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_LoginStatus.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lbl_LoginStatus.TabIndex = 15;
            //
            // lbl_Version
            //
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.Text = "";
            this.lbl_Version.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.lbl_Version.AutoSize = true;
            this.lbl_Version.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_Version.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lbl_Version.TabIndex = 16;
            //
            // menuStrip1
            //
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(664, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            //
            // MainForm
            //
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "SPI Data Comparison Tool";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 640);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(840, 570);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.Controls.Add(this.tlp_Root);
            this.Controls.Add(this.menuStrip1);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.pnl_Header.ResumeLayout(false);
            this.pnl_Header.PerformLayout();
            this.tlp_Header.ResumeLayout(false);
            this.tlp_Header.PerformLayout();
            this.tlp_Workspaces.ResumeLayout(false);
            this.tlp_Workspaces.PerformLayout();
            this.pnl_Status.ResumeLayout(false);
            this.pnl_Status.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnl_Header;
        private SPI_Data_Comp_Tool.GlyphIcon ico_Header;
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Label lbl_Subtitle;
        private System.Windows.Forms.Label lbl_Start;
        private System.Windows.Forms.Panel pnl_Status;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;

        private System.Windows.Forms.TableLayoutPanel tlp_Header;
        private System.Windows.Forms.TableLayoutPanel tlp_Workspaces;
        private SPI_Data_Comp_Tool.TileButton btn_WorkspaceComparison;
        private SPI_Data_Comp_Tool.TileButton btn_WorkspaceScripts;
        private SPI_Data_Comp_Tool.TileButton btn_WorkspaceReports;
        private SPI_Data_Comp_Tool.TileButton btn_WorkspaceSettings;
        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.Label lbl_Version;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_LoginStatus;
    }
}
