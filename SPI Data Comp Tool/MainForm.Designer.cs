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
            this.lbl_LoginStatus = new System.Windows.Forms.Label();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_Header = new System.Windows.Forms.Panel();
            this.tlp_Header = new System.Windows.Forms.TableLayoutPanel();
            this.ico_Header = new SPI_Data_Comp_Tool.GlyphIcon();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.lbl_Subtitle = new System.Windows.Forms.Label();
            this.pnl_Shell = new System.Windows.Forms.Panel();
            this.pnl_Status = new System.Windows.Forms.Panel();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_Version = new System.Windows.Forms.Label();
            this.tlp_Root.SuspendLayout();
            this.pnl_Header.SuspendLayout();
            this.tlp_Header.SuspendLayout();
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
            this.tlp_Root.Controls.Add(this.pnl_Shell, 0, 1);
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
            this.lbl_Subtitle.Text = "Oracle, SQL Server and Excel — compare, export and merge logs";
            this.lbl_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Subtitle.AutoSize = true;
            this.lbl_Subtitle.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.lbl_Subtitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lbl_Subtitle.TabIndex = 5;
            //
            // pnl_Shell
            //
            this.pnl_Shell.Name = "pnl_Shell";
            this.pnl_Shell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Shell.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.pnl_Shell.TabIndex = 6;
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
            this.pnl_Status.TabIndex = 7;
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
            this.tlp_Status.TabIndex = 8;
            //
            // lbl_LoginStatus
            //
            this.lbl_LoginStatus.Name = "lbl_LoginStatus";
            this.lbl_LoginStatus.Text = "Ready";
            this.lbl_LoginStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_LoginStatus.AutoSize = true;
            this.lbl_LoginStatus.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_LoginStatus.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lbl_LoginStatus.TabIndex = 9;
            //
            // lbl_Version
            //
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.Text = "";
            this.lbl_Version.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.lbl_Version.AutoSize = true;
            this.lbl_Version.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_Version.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lbl_Version.TabIndex = 10;
            //
            // MainForm
            //
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.pnl_Header.ResumeLayout(false);
            this.pnl_Header.PerformLayout();
            this.tlp_Header.ResumeLayout(false);
            this.tlp_Header.PerformLayout();
            this.pnl_Status.ResumeLayout(false);
            this.pnl_Status.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnl_Shell;

        private System.Windows.Forms.Panel pnl_Header;
        private SPI_Data_Comp_Tool.GlyphIcon ico_Header;
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Label lbl_Subtitle;
        private System.Windows.Forms.Panel pnl_Status;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;

        private System.Windows.Forms.TableLayoutPanel tlp_Header;
        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.Label lbl_Version;
        private System.Windows.Forms.Label lbl_LoginStatus;
    }
}
