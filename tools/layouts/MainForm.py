"""Main window: the single shell. One left rail (built from WorkspaceCatalog) is the only way to open a task."""
from layout_gen import C, T, G

DESIGNER = "MainForm.Designer.cs"
CLEAN_WHITESPACE = True
FORM_NAME = "MainForm"
CLIENT = (1000, 640)
MINIMUM = (840, 570)
FORM_PADDING = 0

DROP = [
    "btn_WorkspaceScripts", "menuStrip1", "btn_WorkspaceComparison", "btn_WorkspaceReports", "btn_WorkspaceSettings",
    "lbl_Start", "label3",
    "gb_logincredentials", "btn_Login", "tb_username", "tb_password", "label1", "label4", "gb_AppDetails", "panel_control",
    "pictureBox1", "pictureBox2", "label2", "btn_Exit_Logout", "btn_Open_DB_Comparison", "btn_Excel_Comparison",
    "tile_CompareDb", "tile_CompareExcel", "tile_DeltaTracker", "tile_ExportExcel", "sPIDeltaChangeTrackerToolStripMenuItem",
    "fileToolStripMenuItem", "userProfileToolStripMenuItem", "userSettingsToolStripMenuItem", "exitToolStripMenuItem",
    "analysisToolStripMenuItem", "sPIAnalysisToolStripMenuItem", "sPISpecNoteCreationUtilityToolStripMenuItem",
    "sPIDeltaChangeLegacyIDToolStripMenuItem", "sPIDeltaChangeTrackerOptimisedToolStripMenuItem", "sPIFKExtractionToolStripMenuItem",
    "toolToolStripMenuItem", "compareDBToolStripMenuItem", "compareExcelToolStripMenuItem", "compareListTablesToolStripMenuItem",
    "toolsToolStripMenuItem", "customisedExcelToolStripMenuItem", "preMergingToolToolStripMenuItem", "preMergingDuplicacyCheckToolStripMenuItem",
    "configurationToolStripMenuItem", "generateInsertScriptsToolStripMenuItem", "executeScriptsFromExcelToolStripMenuItem",
    "exportDBToExcelToolStripMenuItem", "sQLDBToExcelToolStripMenuItem", "tableExtractionAfterDeltaToolStripMenuItem",
    "mergeLogsExportToolStripMenuItem", "helpToolStripMenuItem", "checkForUpdatesToolStripMenuItem", "aboutUsToolStripMenuItem",
    "toolStripSeparator1", "toolStripSeparator2", "toolStripSeparator3", "toolStripSeparator4",
]

TEXTS = {
    "lbl_Title": "SPI Data Comparison Tool",
    "lbl_Subtitle": "Oracle, SQL Server and Excel — compare, export and merge logs",
    "lbl_LoginStatus": "Ready",
}

MUTED = "System.Drawing.Color.FromArgb(90, 90, 90)"
PROPS = {
    "lbl_Title": {"Font": 'new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold)'},
    "lbl_Subtitle": {"Font": 'new System.Drawing.Font("Segoe UI", 9.75F)'},
    "ico_Header": {"Glyph": "SPI_Data_Comp_Tool.Glyph.Database", "AccessibleRole": "System.Windows.Forms.AccessibleRole.Graphic"},
    "lbl_Version": {"ForeColor": MUTED},
    "lbl_LoginStatus": {"ForeColor": MUTED},
}

LAYOUT = T("tlp_Root", cols=["100%"], rows=["auto", "100%", "auto"], cells=[
    (G("pnl_Header", new="Panel", child=T("tlp_Header", cols=["auto", "100%"], rows=["auto", "auto"], autosize=True, cells=[
        (C("ico_Header", new="SPI_Data_Comp_Tool.GlyphIcon", size=(40, 40), anchor="Left", margin=(0, 0, 14, 0)), 0, 0, 1, 2),
        (C("lbl_Title", new="Label", autosize=True, anchor="Left", margin=0), 1, 0),
        (C("lbl_Subtitle", new="Label", autosize=True, anchor="Left", margin=(2, 0, 0, 0)), 1, 1),
    ]), dock="Fill", autosize=True, margin=0, padding=(20, 14, 20, 14)), 0, 0),
    (C("pnl_Shell", new="Panel", dock="Fill", margin=0), 0, 1),
    (G("pnl_Status", new="Panel", child=T("tlp_Status", cols=["100%", "auto"], rows=["auto"], autosize=True, cells=[
        (C("lbl_LoginStatus", autosize=True, anchor="Left", margin=0), 0, 0),
        (C("lbl_Version", new="Label", autosize=True, anchor="Right", margin=0), 1, 0),
    ]), dock="Fill", autosize=True, margin=0, padding=(14, 7, 14, 7)), 0, 2),
])
