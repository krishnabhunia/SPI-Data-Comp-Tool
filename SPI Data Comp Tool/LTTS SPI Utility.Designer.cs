namespace SPI_Data_Comp_Tool
{
    partial class LTTS_Data_Comparison
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LTTS_Data_Comparison));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sPIAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sPISpecNoteCreationUtilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sPIDeltaChangeTrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.gb_LTTSDetails = new System.Windows.Forms.GroupBox();
            this.gb_logincredentials = new System.Windows.Forms.GroupBox();
            this.btn_Login = new System.Windows.Forms.Button();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_LoginStatus = new System.Windows.Forms.Label();
            this.panel_control = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gb_LTTSDetails.SuspendLayout();
            this.gb_logincredentials.SuspendLayout();
            this.panel_control.SuspendLayout();
            this.SuspendLayout();
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
            // userProfileToolStripMenuItem
            // 
            this.userProfileToolStripMenuItem.Enabled = false;
            this.userProfileToolStripMenuItem.Name = "userProfileToolStripMenuItem";
            this.userProfileToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.userProfileToolStripMenuItem.Text = "&User Profile";
            this.userProfileToolStripMenuItem.Click += new System.EventHandler(this.userProfileToolStripMenuItem_Click);
            // 
            // userSettingsToolStripMenuItem
            // 
            this.userSettingsToolStripMenuItem.Name = "userSettingsToolStripMenuItem";
            this.userSettingsToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.userSettingsToolStripMenuItem.Text = "User Settings";
            this.userSettingsToolStripMenuItem.Click += new System.EventHandler(this.userSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // analysisToolStripMenuItem
            // 
            this.analysisToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sPIAnalysisToolStripMenuItem,
            this.sPISpecNoteCreationUtilityToolStripMenuItem,
            this.sPIDeltaChangeTrackerToolStripMenuItem,
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
            // sPIDeltaChangeTrackerToolStripMenuItem
            // 
            this.sPIDeltaChangeTrackerToolStripMenuItem.Enabled = false;
            this.sPIDeltaChangeTrackerToolStripMenuItem.Name = "sPIDeltaChangeTrackerToolStripMenuItem";
            this.sPIDeltaChangeTrackerToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPIDeltaChangeTrackerToolStripMenuItem.Text = "SPI Delta Change &Tracker";
            this.sPIDeltaChangeTrackerToolStripMenuItem.Visible = false;
            this.sPIDeltaChangeTrackerToolStripMenuItem.Click += new System.EventHandler(this.sPIDeltaChangeTrackerToolStripMenuItem_Click);
            // 
            // sPIDeltaChangeLegacyIDToolStripMenuItem
            // 
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Enabled = false;
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Name = "sPIDeltaChangeLegacyIDToolStripMenuItem";
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Text = "SPI Delta Change &Legacy ID";
            this.sPIDeltaChangeLegacyIDToolStripMenuItem.Click += new System.EventHandler(this.sPIDeltaChangeLegacyIDToolStripMenuItem_Click);
            // 
            // sPIDeltaChangeTrackerOptimisedToolStripMenuItem
            // 
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Enabled = false;
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Name = "sPIDeltaChangeTrackerOptimisedToolStripMenuItem";
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Text = "SPI Delta Change &Tracker Optimised";
            this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Click += new System.EventHandler(this.sPIDeltaChangeTrackerOptimisedToolStripMenuItem_Click);
            // 
            // sPIFKExtractionToolStripMenuItem
            // 
            this.sPIFKExtractionToolStripMenuItem.Enabled = false;
            this.sPIFKExtractionToolStripMenuItem.Name = "sPIFKExtractionToolStripMenuItem";
            this.sPIFKExtractionToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sPIFKExtractionToolStripMenuItem.Text = "SPI FK Extraction";
            this.sPIFKExtractionToolStripMenuItem.Click += new System.EventHandler(this.sPIFKExtractionToolStripMenuItem_Click);
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
            // compareListTablesToolStripMenuItem
            // 
            this.compareListTablesToolStripMenuItem.Enabled = false;
            this.compareListTablesToolStripMenuItem.Name = "compareListTablesToolStripMenuItem";
            this.compareListTablesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.compareListTablesToolStripMenuItem.Text = "Compare &List Tables";
            this.compareListTablesToolStripMenuItem.Click += new System.EventHandler(this.compareListTablesToolStripMenuItem_Click);
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
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
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Enabled = false;
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.configurationToolStripMenuItem.Text = "&Configuration";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(227, 6);
            // 
            // generateInsertScriptsToolStripMenuItem
            // 
            this.generateInsertScriptsToolStripMenuItem.Enabled = false;
            this.generateInsertScriptsToolStripMenuItem.Name = "generateInsertScriptsToolStripMenuItem";
            this.generateInsertScriptsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.generateInsertScriptsToolStripMenuItem.Text = "&Generate Scripts";
            this.generateInsertScriptsToolStripMenuItem.Click += new System.EventHandler(this.generateInsertScriptsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(227, 6);
            // 
            // executeScriptsFromExcelToolStripMenuItem
            // 
            this.executeScriptsFromExcelToolStripMenuItem.Enabled = false;
            this.executeScriptsFromExcelToolStripMenuItem.Name = "executeScriptsFromExcelToolStripMenuItem";
            this.executeScriptsFromExcelToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.executeScriptsFromExcelToolStripMenuItem.Text = "&Execute Scripts From Excel";
            this.executeScriptsFromExcelToolStripMenuItem.Click += new System.EventHandler(this.executeScriptsFromExcelToolStripMenuItem_Click);
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
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 256);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(322, 81);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // btn_Exit_Logout
            // 
            this.btn_Exit_Logout.Location = new System.Drawing.Point(8, 7);
            this.btn_Exit_Logout.Name = "btn_Exit_Logout";
            this.btn_Exit_Logout.Size = new System.Drawing.Size(32, 23);
            this.btn_Exit_Logout.TabIndex = 3;
            this.btn_Exit_Logout.Text = "Exit";
            this.btn_Exit_Logout.UseVisualStyleBackColor = true;
            this.btn_Exit_Logout.Click += new System.EventHandler(this.btn_Exit_Logout_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "label3";
            // 
            // btn_Open_DB_Comparison
            // 
            this.btn_Open_DB_Comparison.Location = new System.Drawing.Point(46, 7);
            this.btn_Open_DB_Comparison.Name = "btn_Open_DB_Comparison";
            this.btn_Open_DB_Comparison.Size = new System.Drawing.Size(118, 23);
            this.btn_Open_DB_Comparison.TabIndex = 0;
            this.btn_Open_DB_Comparison.Text = "Open DB Comparison";
            this.btn_Open_DB_Comparison.UseVisualStyleBackColor = true;
            this.btn_Open_DB_Comparison.Click += new System.EventHandler(this.btn_Open_DB_Comparison_Click);
            // 
            // btn_Excel_Comparison
            // 
            this.btn_Excel_Comparison.Location = new System.Drawing.Point(170, 7);
            this.btn_Excel_Comparison.Name = "btn_Excel_Comparison";
            this.btn_Excel_Comparison.Size = new System.Drawing.Size(130, 23);
            this.btn_Excel_Comparison.TabIndex = 1;
            this.btn_Excel_Comparison.Text = "Open Excel Comparison";
            this.btn_Excel_Comparison.UseVisualStyleBackColor = true;
            this.btn_Excel_Comparison.Click += new System.EventHandler(this.btn_Excel_Comparison_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.ErrorImage")));
            this.pictureBox2.Image = global::SPI_Data_Comp_Tool.Properties.Resources.utility_logo;
            this.pictureBox2.Location = new System.Drawing.Point(9, 150);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(239, 101);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.ErrorImage")));
            this.pictureBox1.Image = global::SPI_Data_Comp_Tool.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(9, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(239, 119);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // gb_LTTSDetails
            // 
            this.gb_LTTSDetails.Controls.Add(this.label2);
            this.gb_LTTSDetails.Controls.Add(this.pictureBox2);
            this.gb_LTTSDetails.Controls.Add(this.pictureBox1);
            this.gb_LTTSDetails.Location = new System.Drawing.Point(16, 28);
            this.gb_LTTSDetails.Name = "gb_LTTSDetails";
            this.gb_LTTSDetails.Size = new System.Drawing.Size(350, 359);
            this.gb_LTTSDetails.TabIndex = 9;
            this.gb_LTTSDetails.TabStop = false;
            this.gb_LTTSDetails.Visible = false;
            // 
            // gb_logincredentials
            // 
            this.gb_logincredentials.Controls.Add(this.btn_Login);
            this.gb_logincredentials.Controls.Add(this.tb_password);
            this.gb_logincredentials.Controls.Add(this.tb_username);
            this.gb_logincredentials.Controls.Add(this.label4);
            this.gb_logincredentials.Controls.Add(this.label1);
            this.gb_logincredentials.Location = new System.Drawing.Point(372, 118);
            this.gb_logincredentials.Name = "gb_logincredentials";
            this.gb_logincredentials.Size = new System.Drawing.Size(280, 144);
            this.gb_logincredentials.TabIndex = 10;
            this.gb_logincredentials.TabStop = false;
            this.gb_logincredentials.Text = "Login Details";
            // 
            // btn_Login
            // 
            this.btn_Login.Location = new System.Drawing.Point(95, 105);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(75, 23);
            this.btn_Login.TabIndex = 4;
            this.btn_Login.Text = "Submit";
            this.btn_Login.UseVisualStyleBackColor = true;
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(95, 69);
            this.tb_password.Name = "tb_password";
            this.tb_password.PasswordChar = '*';
            this.tb_password.Size = new System.Drawing.Size(168, 20);
            this.tb_password.TabIndex = 3;
            // 
            // tb_username
            // 
            this.tb_username.Location = new System.Drawing.Point(95, 38);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(168, 20);
            this.tb_username.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Password : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username : ";
            // 
            // lbl_LoginStatus
            // 
            this.lbl_LoginStatus.AutoSize = true;
            this.lbl_LoginStatus.Location = new System.Drawing.Point(5, 45);
            this.lbl_LoginStatus.Name = "lbl_LoginStatus";
            this.lbl_LoginStatus.Size = new System.Drawing.Size(68, 13);
            this.lbl_LoginStatus.TabIndex = 11;
            this.lbl_LoginStatus.Text = "Please Login";
            // 
            // panel_control
            // 
            this.panel_control.Controls.Add(this.btn_Open_DB_Comparison);
            this.panel_control.Controls.Add(this.lbl_LoginStatus);
            this.panel_control.Controls.Add(this.btn_Exit_Logout);
            this.panel_control.Controls.Add(this.label3);
            this.panel_control.Controls.Add(this.btn_Excel_Comparison);
            this.panel_control.Location = new System.Drawing.Point(16, 393);
            this.panel_control.Name = "panel_control";
            this.panel_control.Size = new System.Drawing.Size(350, 90);
            this.panel_control.TabIndex = 12;
            // 
            // LTTS_Data_Comparison
            // 
            this.AcceptButton = this.btn_Login;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 494);
            this.Controls.Add(this.panel_control);
            this.Controls.Add(this.gb_logincredentials);
            this.Controls.Add(this.gb_LTTSDetails);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "LTTS_Data_Comparison";
            this.Text = "LTTS Data Comparison";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gb_LTTSDetails.ResumeLayout(false);
            this.gb_logincredentials.ResumeLayout(false);
            this.gb_logincredentials.PerformLayout();
            this.panel_control.ResumeLayout(false);
            this.panel_control.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.GroupBox gb_LTTSDetails;
        private System.Windows.Forms.GroupBox gb_logincredentials;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Login;
        private System.Windows.Forms.TextBox tb_password;
        private System.Windows.Forms.TextBox tb_username;
        private System.Windows.Forms.Label lbl_LoginStatus;
        private System.Windows.Forms.Panel panel_control;
        private System.Windows.Forms.ToolStripMenuItem sPISpecNoteCreationUtilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDBToExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQLDBToExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customisedExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sPIDeltaChangeTrackerToolStripMenuItem;
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