"""Main window: four task workspaces; menu routes are created from WorkspaceCatalog."""
from layout_gen import C, T, G

DESIGNER = "MainForm.Designer.cs"
CLEAN_WHITESPACE = True
FORM_NAME = "MainForm"
CLIENT = (1000, 640)
MINIMUM = (840, 570)
FORM_PADDING = 0
TOP = ["menuStrip1"]

DROP = [
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
    "lbl_Subtitle": "Oracle, SQL Server and Excel — four connected workspaces",
    "lbl_Start": "Choose a workspace",
    "lbl_LoginStatus": "Ready",
    "btn_WorkspaceComparison": "Comparison",
    "btn_WorkspaceScripts": "SQL Scripts",
    "btn_WorkspaceReports": "Data & Reports",
    "btn_WorkspaceSettings": "Settings",
}

MUTED = "System.Drawing.Color.FromArgb(90, 90, 90)"
PROPS = {
    "lbl_Title": {"Font": 'new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold)'},
    "lbl_Subtitle": {"Font": 'new System.Drawing.Font("Segoe UI", 9.75F)'},
    "ico_Header": {"Glyph": "SPI_Data_Comp_Tool.Glyph.Database", "AccessibleRole": "System.Windows.Forms.AccessibleRole.Graphic"},
    "lbl_Start": {"ForeColor": MUTED, "Font": 'new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold)'},
    "btn_WorkspaceComparison": {"Tone": "0", "Glyph": "SPI_Data_Comp_Tool.Glyph.Database", "Hint": '"Databases, Excel, batch comparisons and advanced mapping"'},
    "btn_WorkspaceScripts": {"Tone": "3", "Glyph": "SPI_Data_Comp_Tool.Glyph.Sheet", "Hint": '"Generate scripts or explicitly execute a workbook"'},
    "btn_WorkspaceReports": {"Tone": "0", "Glyph": "SPI_Data_Comp_Tool.Glyph.Export", "Hint": '"Database exports, Excel columns and merge logs"'},
    "btn_WorkspaceSettings": {"Tone": "1", "Glyph": "SPI_Data_Comp_Tool.Glyph.Database", "Hint": '"Profiles, rules, colour themes and stable/beta updates"'},
    "label3": {"ForeColor": MUTED},
    "lbl_Version": {"ForeColor": MUTED},
    "lbl_LoginStatus": {"ForeColor": MUTED},
}

def workspace_button(name):
    return C(name, new="SPI_Data_Comp_Tool.TileButton", dock="Fill", margin=6, size=(400, 94))

LAYOUT = T("tlp_Root", cols=["100%"], rows=["auto", "100%", "auto"], cells=[
    (G("pnl_Header", new="Panel", child=T("tlp_Header", cols=["auto", "100%"], rows=["auto", "auto"], autosize=True, cells=[
        (C("ico_Header", new="SPI_Data_Comp_Tool.GlyphIcon", size=(40, 40), anchor="Left", margin=(0, 0, 14, 0)), 0, 0, 1, 2),
        (C("lbl_Title", new="Label", autosize=True, anchor="Left", margin=0), 1, 0),
        (C("lbl_Subtitle", new="Label", autosize=True, anchor="Left", margin=(2, 0, 0, 0)), 1, 1),
    ]), dock="Fill", autosize=True, margin=0, padding=(20, 14, 20, 14)), 0, 0),
    (T("tlp_Workspaces", cols=["50%", "50%"], rows=["auto", "50%", "50%", "auto"], padding=14, cells=[
        (C("lbl_Start", new="Label", autosize=True, anchor="Left", margin=(6, 0, 0, 6)), 0, 0, 2),
        (workspace_button("btn_WorkspaceComparison"), 0, 1),
        (workspace_button("btn_WorkspaceScripts"), 1, 1),
        (workspace_button("btn_WorkspaceReports"), 0, 2),
        (workspace_button("btn_WorkspaceSettings"), 1, 2),
        (C("label3", autosize=True, dock="Fill", margin=(6, 10, 6, 0)), 0, 3, 2),
    ]), 0, 1),
    (G("pnl_Status", new="Panel", child=T("tlp_Status", cols=["100%", "auto"], rows=["auto"], autosize=True, cells=[
        (C("lbl_LoginStatus", autosize=True, anchor="Left", margin=0), 0, 0),
        (C("lbl_Version", new="Label", autosize=True, anchor="Right", margin=0), 1, 0),
    ]), dock="Fill", autosize=True, margin=0, padding=(14, 7, 14, 7)), 0, 2),
])
