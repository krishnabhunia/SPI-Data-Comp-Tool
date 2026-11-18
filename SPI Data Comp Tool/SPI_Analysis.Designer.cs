namespace SPI_Data_Comp_Tool
{
    partial class SPI_Analysis
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.oracleAndSQLConnection1 = new OracleAndSQLConnection();
            this.btn_Show = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.workspaceTabs = new System.Windows.Forms.TabControl();
            this.setupPage = new System.Windows.Forms.TabPage();
            this.resultsPage = new System.Windows.Forms.TabPage();
            var setupLayout = new System.Windows.Forms.TableLayoutPanel();
            var actionRow = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();

            this.oracleAndSQLConnection1.Name = "oracleAndSQLConnection1";
            this.oracleAndSQLConnection1.AutoSize = true;
            this.oracleAndSQLConnection1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.oracleAndSQLConnection1.Margin = new System.Windows.Forms.Padding(0);
            this.oracleAndSQLConnection1.TabIndex = 0;

            this.btn_Show.Name = "btn_Show";
            this.btn_Show.Text = "Show results";
            this.btn_Show.AutoSize = true;
            this.btn_Show.MinimumSize = new System.Drawing.Size(130, 32);
            this.btn_Show.Enabled = false;
            this.btn_Show.TabIndex = 0;
            this.btn_Show.Click += new System.EventHandler(this.btn_Show_Click);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Text = "Load source and target data before showing results.";
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Margin = new System.Windows.Forms.Padding(12, 10, 3, 3);
            this.lbl_Status.MaximumSize = new System.Drawing.Size(850, 0);

            actionRow.AutoSize = true;
            actionRow.Dock = System.Windows.Forms.DockStyle.Fill;
            actionRow.WrapContents = true;
            actionRow.Controls.Add(this.btn_Show);
            actionRow.Controls.Add(this.lbl_Status);
            setupLayout.AutoSize = true;
            setupLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            setupLayout.Dock = System.Windows.Forms.DockStyle.Top;
            setupLayout.ColumnCount = 1;
            setupLayout.RowCount = 2;
            setupLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            setupLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            setupLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            setupLayout.Controls.Add(this.oracleAndSQLConnection1, 0, 0);
            setupLayout.Controls.Add(actionRow, 0, 1);

            this.setupPage.Name = "setupPage";
            this.setupPage.Text = "Connection setup";
            this.setupPage.Padding = new System.Windows.Forms.Padding(8);
            this.setupPage.AutoScroll = true;
            this.setupPage.Controls.Add(setupLayout);
            this.resultsPage.Name = "resultsPage";
            this.resultsPage.Text = "Results";
            this.resultsPage.Padding = new System.Windows.Forms.Padding(8);
            this.resultsPage.Controls.Add(new System.Windows.Forms.Label
            {
                Name = "resultsHint", Text = "Load both databases in Connection setup, then select Show results.",
                Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                AutoEllipsis = true,
            });
            this.workspaceTabs.Name = "workspaceTabs";
            this.workspaceTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workspaceTabs.Controls.Add(this.setupPage);
            this.workspaceTabs.Controls.Add(this.resultsPage);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ClientSize = new System.Drawing.Size(1360, 780);
            this.MinimumSize = new System.Drawing.Size(900, 620);
            this.Controls.Add(this.workspaceTabs);
            this.Name = "SPI_Analysis";
            this.Text = "SPI Spec Form Gap Analysis";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
        }

        private OracleAndSQLConnection oracleAndSQLConnection1;
        private System.Windows.Forms.Button btn_Show;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.TabControl workspaceTabs;
        private System.Windows.Forms.TabPage setupPage;
        private System.Windows.Forms.TabPage resultsPage;
    }
}
