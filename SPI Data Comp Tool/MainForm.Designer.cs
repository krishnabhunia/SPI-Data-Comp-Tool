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
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sPIAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sPISpecNoteCreationUtilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sPIDeltaChangeLegacyIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sPIFKExtractionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareListTablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customisedExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.preMergingToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preMergingDuplicacyCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.generateInsertScriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.executeScriptsFromExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDBToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLDBToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableExtractionAfterDeltaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeLogsExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutUsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_LoginStatus = new System.Windows.Forms.Label();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_Header = new System.Windows.Forms.Panel();
            this.tlp_Header = new System.Windows.Forms.TableLayoutPanel();
            this.ico_Header = new SPI_Data_Comp_Tool.GlyphIcon();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.lbl_Subtitle = new System.Windows.Forms.Label();
            this.tlp_Body = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_Start = new System.Windows.Forms.Label();
            this.tile_CompareDb = new SPI_Data_Comp_Tool.TileButton();
            this.tile_CompareExcel = new SPI_Data_Comp_Tool.TileButton();
            this.tile_DeltaTracker = new SPI_Data_Comp_Tool.TileButton();
            this.tile_ExportExcel = new SPI_Data_Comp_Tool.TileButton();
            this.pnl_Status = new System.Windows.Forms.Panel();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_Version = new System.Windows.Forms.Label();
            this.tlp_Root.SuspendLayout();
            this.pnl_Header.SuspendLayout();
            this.tlp_Header.SuspendLayout();
            this.tlp_Body.SuspendLayout();
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
            this.tlp_Root.Controls.Add(this.tlp_Body, 0, 1);
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
            this.lbl_Subtitle.Text = "Compare Oracle, SQL Server and Excel data";
            this.lbl_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Subtitle.AutoSize = true;
            this.lbl_Subtitle.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.lbl_Subtitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lbl_Subtitle.TabIndex = 5;
            // 
            // tlp_Body
            // 
            this.tlp_Body.Name = "tlp_Body";
            this.tlp_Body.ColumnCount = 2;
            this.tlp_Body.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Body.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Body.RowCount = 4;
            this.tlp_Body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Body.Controls.Add(this.lbl_Start, 0, 0);
            this.tlp_Body.SetColumnSpan(this.lbl_Start, 2);
            this.tlp_Body.Controls.Add(this.tile_CompareDb, 0, 1);
            this.tlp_Body.Controls.Add(this.tile_CompareExcel, 1, 1);
            this.tlp_Body.Controls.Add(this.tile_DeltaTracker, 0, 2);
            this.tlp_Body.Controls.Add(this.tile_ExportExcel, 1, 2);
            this.tlp_Body.Controls.Add(this.label3, 0, 3);
            this.tlp_Body.SetColumnSpan(this.label3, 2);
            this.tlp_Body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Body.AutoSize = false;
            this.tlp_Body.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_Body.Padding = new System.Windows.Forms.Padding(20, 14, 20, 12);
            this.tlp_Body.TabIndex = 6;
            // 
            // lbl_Start
            // 
            this.lbl_Start.Name = "lbl_Start";
            this.lbl_Start.Text = "Start with";
            this.lbl_Start.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Start.AutoSize = true;
            this.lbl_Start.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lbl_Start.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lbl_Start.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbl_Start.TabIndex = 7;
            // 
            // tile_CompareDb
            // 
            this.tile_CompareDb.Name = "tile_CompareDb";
            this.tile_CompareDb.Text = "Compare databases";
            this.tile_CompareDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tile_CompareDb.Margin = new System.Windows.Forms.Padding(0, 0, 7, 7);
            this.tile_CompareDb.Tone = 0;
            this.tile_CompareDb.Glyph = SPI_Data_Comp_Tool.Glyph.Database;
            this.tile_CompareDb.Hint = "Oracle or SQL Server, table by table";
            this.tile_CompareDb.TabIndex = 8;
            // 
            // tile_CompareExcel
            // 
            this.tile_CompareExcel.Name = "tile_CompareExcel";
            this.tile_CompareExcel.Text = "Compare Excel files";
            this.tile_CompareExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tile_CompareExcel.Margin = new System.Windows.Forms.Padding(7, 0, 0, 7);
            this.tile_CompareExcel.Tone = 1;
            this.tile_CompareExcel.Glyph = SPI_Data_Comp_Tool.Glyph.Sheet;
            this.tile_CompareExcel.Hint = "Two workbooks, sheet by sheet";
            this.tile_CompareExcel.TabIndex = 9;
            // 
            // tile_DeltaTracker
            // 
            this.tile_DeltaTracker.Name = "tile_DeltaTracker";
            this.tile_DeltaTracker.Text = "Delta change tracker";
            this.tile_DeltaTracker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tile_DeltaTracker.Margin = new System.Windows.Forms.Padding(0, 7, 7, 0);
            this.tile_DeltaTracker.Tone = 2;
            this.tile_DeltaTracker.Glyph = SPI_Data_Comp_Tool.Glyph.Delta;
            this.tile_DeltaTracker.Hint = "What changed since the backup";
            this.tile_DeltaTracker.TabIndex = 10;
            // 
            // tile_ExportExcel
            // 
            this.tile_ExportExcel.Name = "tile_ExportExcel";
            this.tile_ExportExcel.Text = "Export to Excel";
            this.tile_ExportExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tile_ExportExcel.Margin = new System.Windows.Forms.Padding(7, 7, 0, 0);
            this.tile_ExportExcel.Tone = 3;
            this.tile_ExportExcel.Glyph = SPI_Data_Comp_Tool.Glyph.Export;
            this.tile_ExportExcel.Hint = "Tables from a database to a workbook";
            this.tile_ExportExcel.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.Name = "label3";
            this.label3.Text = "label3";
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.AutoSize = true;
            this.label3.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
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
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.analysisToolStripMenuItem,
            this.toolToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.exportDBToExcelToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(664, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userProfileToolStripMenuItem,
            this.userSettingsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compareDBToolStripMenuItem,
            this.compareExcelToolStripMenuItem,
            this.compareListTablesToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.toolToolStripMenuItem.Text = "&Compare";
            // 
            // compareDBToolStripMenuItem
            // 
            this.compareDBToolStripMenuItem.Enabled = false;
            this.compareDBToolStripMenuItem.Name = "compareDBToolStripMenuItem";
            this.compareDBToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.compareDBToolStripMenuItem.Text = "Compare &DB";
            this.compareDBToolStripMenuItem.Click += new System.EventHandler(this.compareDBToolStripMenuItem_Click);
            // 
            // compareExcelToolStripMenuItem
            // 
            this.compareExcelToolStripMenuItem.Enabled = false;
            this.compareExcelToolStripMenuItem.Name = "compareExcelToolStripMenuItem";
            this.compareExcelToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.compareExcelToolStripMenuItem.Text = "Compare &Excel";
            this.compareExcelToolStripMenuItem.Click += new System.EventHandler(this.compareExcelToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.aboutUsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Text = "Check for &updates…";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // aboutUsToolStripMenuItem
            // 
            this.aboutUsToolStripMenuItem.Name = "aboutUsToolStripMenuItem";
            this.aboutUsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.aboutUsToolStripMenuItem.Text = "&About Us";
            this.aboutUsToolStripMenuItem.Click += new System.EventHandler(this.aboutUsToolStripMenuItem_Click);
            // 
            // analysisToolStripMenuItem
            // 
            this.analysisToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sPIAnalysisToolStripMenuItem,
            this.sPISpecNoteCreationUtilityToolStripMenuItem,
            this.sPIDeltaChangeLegacyIDToolStripMenuItem,
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem,
            this.sPIFKExtractionToolStripMenuItem});
            this.analysisToolStripMenuItem.Name = "analysisToolStripMenuItem";
            this.analysisToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.analysisToolStripMenuItem.Text = "&Analysis";
            // 
            // sPIAnalysisToolStripMenuItem
            // 
            this.sPIAnalysisToolStripMenuItem.Enabled = false;
            this.sPIAnalysisToolStripMenuItem.Name = "sPIAnalysisToolStripMenuItem";
            this.sPIAnalysisToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPIAnalysisToolStripMenuItem.Text = "SPI Spec Form &Gap Analysis";
            this.sPIAnalysisToolStripMenuItem.Click += new System.EventHandler(this.sPIAnalysisToolStripMenuItem_Click);
            // 
            // sPISpecNoteCreationUtilityToolStripMenuItem
            // 
            this.sPISpecNoteCreationUtilityToolStripMenuItem.Enabled = false;
            this.sPISpecNoteCreationUtilityToolStripMenuItem.Name = "sPISpecNoteCreationUtilityToolStripMenuItem";
            this.sPISpecNoteCreationUtilityToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPISpecNoteCreationUtilityToolStripMenuItem.Text = "SPI Spec Note &Creation Utility";
            this.sPISpecNoteCreationUtilityToolStripMenuItem.Click += new System.EventHandler(this.sPISpecNoteCreationUtilityToolStripMenuItem_Click);
            // 
            // exportDBToExcelToolStripMenuItem
            // 
            this.exportDBToExcelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sQLDBToExcelToolStripMenuItem,
            this.tableExtractionAfterDeltaToolStripMenuItem,
            this.mergeLogsExportToolStripMenuItem});
            this.exportDBToExcelToolStripMenuItem.Name = "exportDBToExcelToolStripMenuItem";
            this.exportDBToExcelToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.exportDBToExcelToolStripMenuItem.Text = "&Export";
            // 
            // sQLDBToExcelToolStripMenuItem
            // 
            this.sQLDBToExcelToolStripMenuItem.Enabled = false;
            this.sQLDBToExcelToolStripMenuItem.Name = "sQLDBToExcelToolStripMenuItem";
            this.sQLDBToExcelToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.sQLDBToExcelToolStripMenuItem.Text = "&Export DB To Excel";
            this.sQLDBToExcelToolStripMenuItem.Click += new System.EventHandler(this.sQLDBToExcelToolStripMenuItem_Click_1);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customisedExcelToolStripMenuItem,
            this.toolStripSeparator2,
            this.preMergingToolToolStripMenuItem,
            this.preMergingDuplicacyCheckToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.toolStripSeparator3,
            this.generateInsertScriptsToolStripMenuItem,
            this.toolStripSeparator4,
            this.executeScriptsFromExcelToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // customisedExcelToolStripMenuItem
            // 
            this.customisedExcelToolStripMenuItem.Enabled = false;
            this.customisedExcelToolStripMenuItem.Name = "customisedExcelToolStripMenuItem";
            this.customisedExcelToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.customisedExcelToolStripMenuItem.Text = "Customised &Excel";
            this.customisedExcelToolStripMenuItem.Click += new System.EventHandler(this.customisedExcelToolStripMenuItem_Click_1);
            // 
            // compareListTablesToolStripMenuItem
            // 
            this.compareListTablesToolStripMenuItem.Enabled = false;
            this.compareListTablesToolStripMenuItem.Name = "compareListTablesToolStripMenuItem";
            this.compareListTablesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.compareListTablesToolStripMenuItem.Text = "Compare &List Tables";
            this.compareListTablesToolStripMenuItem.Click += new System.EventHandler(this.compareListTablesToolStripMenuItem_Click);
            // 
            // preMergingToolToolStripMenuItem
            // 
            this.preMergingToolToolStripMenuItem.Enabled = false;
            this.preMergingToolToolStripMenuItem.Name = "preMergingToolToolStripMenuItem";
            this.preMergingToolToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.preMergingToolToolStripMenuItem.Text = "Pre &Merging Tool";
            this.preMergingToolToolStripMenuItem.Click += new System.EventHandler(this.preMergingToolToolStripMenuItem_Click);
            // 
            // preMergingDuplicacyCheckToolStripMenuItem
            // 
            this.preMergingDuplicacyCheckToolStripMenuItem.Enabled = false;
            this.preMergingDuplicacyCheckToolStripMenuItem.Name = "preMergingDuplicacyCheckToolStripMenuItem";
            this.preMergingDuplicacyCheckToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.preMergingDuplicacyCheckToolStripMenuItem.Text = "Pre Merging &Duplicacy Check";
            this.preMergingDuplicacyCheckToolStripMenuItem.Click += new System.EventHandler(this.preMergingDuplicacyCheckToolStripMenuItem_Click);
            // 
            // userProfileToolStripMenuItem
            // 
            this.userProfileToolStripMenuItem.Enabled = false;
            this.userProfileToolStripMenuItem.Name = "userProfileToolStripMenuItem";
            this.userProfileToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.userProfileToolStripMenuItem.Text = "&User Profile";
            this.userProfileToolStripMenuItem.Click += new System.EventHandler(this.userProfileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Enabled = false;
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.configurationToolStripMenuItem.Text = "&Configuration";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // sPIDeltaChangeLegacyIDToolStripMenuItem
            // 
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Enabled = false;
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Name = "sPIDeltaChangeLegacyIDToolStripMenuItem";
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Text = "SPI Delta Change &Legacy ID";
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Click += new System.EventHandler(this.sPIDeltaChangeLegacyIDToolStripMenuItem_Click);
            // 
            // generateInsertScriptsToolStripMenuItem
            // 
            this.generateInsertScriptsToolStripMenuItem.Enabled = false;
            this.generateInsertScriptsToolStripMenuItem.Name = "generateInsertScriptsToolStripMenuItem";
            this.generateInsertScriptsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.generateInsertScriptsToolStripMenuItem.Text = "&Generate Scripts";
            this.generateInsertScriptsToolStripMenuItem.Click += new System.EventHandler(this.generateInsertScriptsToolStripMenuItem_Click);
            // 
            // executeScriptsFromExcelToolStripMenuItem
            // 
            this.executeScriptsFromExcelToolStripMenuItem.Enabled = false;
            this.executeScriptsFromExcelToolStripMenuItem.Name = "executeScriptsFromExcelToolStripMenuItem";
            this.executeScriptsFromExcelToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.executeScriptsFromExcelToolStripMenuItem.Text = "&Execute Scripts From Excel";
            this.executeScriptsFromExcelToolStripMenuItem.Click += new System.EventHandler(this.executeScriptsFromExcelToolStripMenuItem_Click);
            // 
            // tableExtractionAfterDeltaToolStripMenuItem
            // 
            this.tableExtractionAfterDeltaToolStripMenuItem.Enabled = false;
            this.tableExtractionAfterDeltaToolStripMenuItem.Name = "tableExtractionAfterDeltaToolStripMenuItem";
            this.tableExtractionAfterDeltaToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.tableExtractionAfterDeltaToolStripMenuItem.Text = "&Table Extraction After Delta";
            this.tableExtractionAfterDeltaToolStripMenuItem.Click += new System.EventHandler(this.tableExtractionAfterDeltaToolStripMenuItem_Click);
            // 
            // mergeLogsExportToolStripMenuItem
            // 
            this.mergeLogsExportToolStripMenuItem.Enabled = false;
            this.mergeLogsExportToolStripMenuItem.Name = "mergeLogsExportToolStripMenuItem";
            this.mergeLogsExportToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.mergeLogsExportToolStripMenuItem.Text = "&Merge Logs Export";
            this.mergeLogsExportToolStripMenuItem.Click += new System.EventHandler(this.mergeLogsExportToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(227, 6);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(227, 6);
            // 
            // sPIDeltaChangeTrackerOptimisedToolStripMenuItem
            // 
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Enabled = false;
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Name = "sPIDeltaChangeTrackerOptimisedToolStripMenuItem";
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Text = "SPI Delta Change &Tracker Optimised";
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Click += new System.EventHandler(this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem_Click);
            // 
            // userSettingsToolStripMenuItem
            // 
            this.userSettingsToolStripMenuItem.Name = "userSettingsToolStripMenuItem";
            this.userSettingsToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.userSettingsToolStripMenuItem.Text = "User Settings";
            this.userSettingsToolStripMenuItem.Click += new System.EventHandler(this.userSettingsToolStripMenuItem_Click);
            // 
            // sPIFKExtractionToolStripMenuItem
            // 
            this.sPIFKExtractionToolStripMenuItem.Enabled = false;
            this.sPIFKExtractionToolStripMenuItem.Name = "sPIFKExtractionToolStripMenuItem";
            this.sPIFKExtractionToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPIFKExtractionToolStripMenuItem.Text = "SPI FK Extraction";
            this.sPIFKExtractionToolStripMenuItem.Click += new System.EventHandler(this.sPIFKExtractionToolStripMenuItem_Click);
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
            this.ClientSize = new System.Drawing.Size(660, 500);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(600, 470);
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
            this.tlp_Body.ResumeLayout(false);
            this.tlp_Body.PerformLayout();
            this.pnl_Status.ResumeLayout(false);
            this.pnl_Status.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnl_Header;
        private System.Windows.Forms.TableLayoutPanel tlp_Header;
        private SPI_Data_Comp_Tool.GlyphIcon ico_Header;
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Label lbl_Subtitle;
        private System.Windows.Forms.TableLayoutPanel tlp_Body;
        private System.Windows.Forms.Label lbl_Start;
        private SPI_Data_Comp_Tool.TileButton tile_CompareDb;
        private SPI_Data_Comp_Tool.TileButton tile_CompareExcel;
        private SPI_Data_Comp_Tool.TileButton tile_DeltaTracker;
        private SPI_Data_Comp_Tool.TileButton tile_ExportExcel;
        private System.Windows.Forms.Panel pnl_Status;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.Label lbl_Version;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutUsToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sPIAnalysisToolStripMenuItem;
        private System.Windows.Forms.Label lbl_LoginStatus;
        private System.Windows.Forms.ToolStripMenuItem sPISpecNoteCreationUtilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDBToExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQLDBToExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customisedExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareListTablesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preMergingToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preMergingDuplicacyCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sPIDeltaChangeLegacyIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateInsertScriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem executeScriptsFromExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tableExtractionAfterDeltaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeLogsExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem sPIDeltaChangeTrackerOptimisedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sPIFKExtractionToolStripMenuItem;
    }
}
