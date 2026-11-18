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
            this.aboutUsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Exit_Logout = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Open_DB_Comparison = new System.Windows.Forms.Button();
            this.btn_Excel_Comparison = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_LoginStatus = new System.Windows.Forms.Label();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Pictures = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_About = new System.Windows.Forms.TableLayoutPanel();
            this.flp_Status = new System.Windows.Forms.FlowLayoutPanel();
            this.lbl_Version = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tlp_Root.SuspendLayout();
            this.tlp_Pictures.SuspendLayout();
            this.tlp_About.SuspendLayout();
            this.flp_Status.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Root.RowCount = 2;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.Controls.Add(this.tlp_Pictures, 0, 0);
            this.tlp_Root.Controls.Add(this.tlp_About, 1, 0);
            this.tlp_Root.Controls.Add(this.flp_Status, 0, 1);
            this.tlp_Root.SetColumnSpan(this.flp_Status, 2);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.Padding = new System.Windows.Forms.Padding(16, 16, 16, 8);
            this.tlp_Root.TabIndex = 0;
            // 
            // tlp_Pictures
            // 
            this.tlp_Pictures.Name = "tlp_Pictures";
            this.tlp_Pictures.ColumnCount = 1;
            this.tlp_Pictures.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Pictures.RowCount = 2;
            this.tlp_Pictures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tlp_Pictures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlp_Pictures.Controls.Add(this.pictureBox1, 0, 0);
            this.tlp_Pictures.Controls.Add(this.pictureBox2, 0, 1);
            this.tlp_Pictures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Pictures.AutoSize = false;
            this.tlp_Pictures.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.tlp_Pictures.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SPI_Data_Comp_Tool.Properties.Resources.app_banner;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::SPI_Data_Comp_Tool.Properties.Resources.compare_diagram;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 3;
            // 
            // tlp_About
            // 
            this.tlp_About.Name = "tlp_About";
            this.tlp_About.ColumnCount = 1;
            this.tlp_About.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_About.RowCount = 6;
            this.tlp_About.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_About.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_About.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_About.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_About.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_About.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_About.Controls.Add(this.label2, 0, 0);
            this.tlp_About.Controls.Add(this.label3, 0, 1);
            this.tlp_About.Controls.Add(this.btn_Open_DB_Comparison, 0, 2);
            this.tlp_About.Controls.Add(this.btn_Excel_Comparison, 0, 3);
            this.tlp_About.Controls.Add(this.btn_Exit_Logout, 0, 5);
            this.tlp_About.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_About.AutoSize = false;
            this.tlp_About.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_About.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Name = "label2";
            this.label2.Text = "label2";
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.label2.AutoSize = true;
            this.label2.Margin = new System.Windows.Forms.Padding(0, 4, 0, 12);
            this.label2.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Name = "label3";
            this.label3.Text = "label3";
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.label3.AutoSize = true;
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.label3.TabIndex = 6;
            // 
            // btn_Open_DB_Comparison
            // 
            this.btn_Open_DB_Comparison.Name = "btn_Open_DB_Comparison";
            this.btn_Open_DB_Comparison.UseVisualStyleBackColor = true;
            this.btn_Open_DB_Comparison.Click += new System.EventHandler(this.btn_Open_DB_Comparison_Click);
            this.btn_Open_DB_Comparison.Text = "Compare databases";
            this.btn_Open_DB_Comparison.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.btn_Open_DB_Comparison.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.btn_Open_DB_Comparison.Size = new System.Drawing.Size(200, 36);
            this.btn_Open_DB_Comparison.TabIndex = 7;
            // 
            // btn_Excel_Comparison
            // 
            this.btn_Excel_Comparison.Name = "btn_Excel_Comparison";
            this.btn_Excel_Comparison.UseVisualStyleBackColor = true;
            this.btn_Excel_Comparison.Click += new System.EventHandler(this.btn_Excel_Comparison_Click);
            this.btn_Excel_Comparison.Text = "Compare Excel files";
            this.btn_Excel_Comparison.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.btn_Excel_Comparison.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Excel_Comparison.Size = new System.Drawing.Size(200, 36);
            this.btn_Excel_Comparison.TabIndex = 8;
            // 
            // btn_Exit_Logout
            // 
            this.btn_Exit_Logout.Name = "btn_Exit_Logout";
            this.btn_Exit_Logout.UseVisualStyleBackColor = true;
            this.btn_Exit_Logout.Click += new System.EventHandler(this.btn_Exit_Logout_Click);
            this.btn_Exit_Logout.Text = "Exit";
            this.btn_Exit_Logout.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btn_Exit_Logout.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_Exit_Logout.Size = new System.Drawing.Size(100, 32);
            this.btn_Exit_Logout.TabIndex = 9;
            // 
            // flp_Status
            // 
            this.flp_Status.Name = "flp_Status";
            this.flp_Status.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flp_Status.WrapContents = false;
            this.flp_Status.Controls.Add(this.lbl_LoginStatus);
            this.flp_Status.Controls.Add(this.lbl_Version);
            this.flp_Status.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.flp_Status.AutoSize = true;
            this.flp_Status.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flp_Status.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.flp_Status.TabIndex = 10;
            // 
            // lbl_LoginStatus
            // 
            this.lbl_LoginStatus.Name = "lbl_LoginStatus";
            this.lbl_LoginStatus.Text = "Ready";
            this.lbl_LoginStatus.AutoSize = true;
            this.lbl_LoginStatus.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.lbl_LoginStatus.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lbl_LoginStatus.TabIndex = 11;
            // 
            // lbl_Version
            // 
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.Text = "";
            this.lbl_Version.AutoSize = true;
            this.lbl_Version.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_Version.ForeColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.lbl_Version.TabIndex = 12;
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
            this.aboutUsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
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
            this.ClientSize = new System.Drawing.Size(760, 440);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.Controls.Add(this.tlp_Root);
            this.Controls.Add(this.menuStrip1);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.tlp_Pictures.ResumeLayout(false);
            this.tlp_Pictures.PerformLayout();
            this.tlp_About.ResumeLayout(false);
            this.tlp_About.PerformLayout();
            this.flp_Status.ResumeLayout(false);
            this.flp_Status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Pictures;
        private System.Windows.Forms.TableLayoutPanel tlp_About;
        private System.Windows.Forms.FlowLayoutPanel flp_Status;
        private System.Windows.Forms.Label lbl_Version;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutUsToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_Exit_Logout;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Open_DB_Comparison;
        private System.Windows.Forms.Button btn_Excel_Comparison;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sPIAnalysisToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
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