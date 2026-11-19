"""Main window - layout spec for tools/layout_gen.py."""
from layout_gen import C, T, F, G

DESIGNER = "MainForm.Designer.cs"
FORM_NAME = "MainForm"
CLIENT = (660, 500)
MINIMUM = (600, 470)
FORM_PADDING = 0
TOP = ["menuStrip1"]           # docked to the top, outside the layout table

# the old login box and its two holder panels are gone (login removed in 2.2.0)
DROP = ["gb_logincredentials", "btn_Login", "tb_username", "tb_password", "label1", "label4",
        "gb_AppDetails", "panel_control",
        # the first Delta Change Tracker window: its menu entry was always hidden, the window is removed
        "sPIDeltaChangeTrackerToolStripMenuItem",
        # the two pictures, the two plain buttons and Exit made way for the header band and the tool tiles
        # (File > Exit and the window's own close button still close the app)
        "pictureBox1", "pictureBox2", "label2", "btn_Open_DB_Comparison", "btn_Excel_Comparison", "btn_Exit_Logout"]

TEXTS = {
    "lbl_Title": "SPI Data Comparison Tool",
    "lbl_Subtitle": "Compare Oracle, SQL Server and Excel data",
    "lbl_Start": "Start with",
    "tile_CompareDb": "Compare databases",
    "tile_CompareExcel": "Compare Excel files",
    "tile_DeltaTracker": "Delta change tracker",
    "tile_ExportExcel": "Export to Excel",
    "lbl_LoginStatus": "Ready",
    "checkForUpdatesToolStripMenuItem": "Check for &updates…",
}

MUTED = "System.Drawing.Color.FromArgb(90, 90, 90)"
TILE = "SPI_Data_Comp_Tool.TileButton"

PROPS = {
    "lbl_Title": {"Font": 'new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold)'},
    "lbl_Subtitle": {"Font": 'new System.Drawing.Font("Segoe UI", 9.75F)'},
    "ico_Header": {"Glyph": "SPI_Data_Comp_Tool.Glyph.Database", "AccessibleRole": "System.Windows.Forms.AccessibleRole.Graphic"},
    "lbl_Start": {"ForeColor": MUTED, "Font": 'new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold)'},
    "tile_CompareDb": {"Tone": "0", "Glyph": "SPI_Data_Comp_Tool.Glyph.Database", "Hint": '"Oracle or SQL Server, table by table"'},
    "tile_CompareExcel": {"Tone": "1", "Glyph": "SPI_Data_Comp_Tool.Glyph.Sheet", "Hint": '"Two workbooks, sheet by sheet"'},
    "tile_DeltaTracker": {"Tone": "2", "Glyph": "SPI_Data_Comp_Tool.Glyph.Delta", "Hint": '"What changed since the backup"'},
    "tile_ExportExcel": {"Tone": "3", "Glyph": "SPI_Data_Comp_Tool.Glyph.Export", "Hint": '"Tables from a database to a workbook"'},
    "label3": {"ForeColor": MUTED},
    "lbl_Version": {"ForeColor": MUTED},
    "lbl_LoginStatus": {"ForeColor": MUTED},
}

LAYOUT = T("tlp_Root", cols=["100%"], rows=["auto", "100%", "auto"], cells=[
    # header band in the theme's colour
    (G("pnl_Header", new="Panel", child=T("tlp_Header", cols=["auto", "100%"], rows=["auto", "auto"], autosize=True, cells=[
        (C("ico_Header", new="SPI_Data_Comp_Tool.GlyphIcon", size=(40, 40), anchor="Left", margin=(0, 0, 14, 0)), 0, 0, 1, 2),
        (C("lbl_Title", new="Label", autosize=True, anchor="Left", margin=0), 1, 0),
        (C("lbl_Subtitle", new="Label", autosize=True, anchor="Left", margin=(2, 0, 0, 0)), 1, 1),
    ]), dock="Fill", autosize=True, margin=0, padding=(20, 14, 20, 14)), 0, 0),
    # the four tools most people start with; every other tool is in the menus
    (T("tlp_Body", cols=["50%", "50%"], rows=["auto", "50%", "50%", "auto"], padding=(20, 14, 20, 12), cells=[
        (C("lbl_Start", new="Label", autosize=True, anchor="Left", margin=(0, 0, 0, 10)), 0, 0, 2),
        (C("tile_CompareDb", new=TILE, dock="Fill", margin=(0, 0, 7, 7)), 0, 1),
        (C("tile_CompareExcel", new=TILE, dock="Fill", margin=(7, 0, 0, 7)), 1, 1),
        (C("tile_DeltaTracker", new=TILE, dock="Fill", margin=(0, 7, 7, 0)), 0, 2),
        (C("tile_ExportExcel", new=TILE, dock="Fill", margin=(7, 7, 0, 0)), 1, 2),
        (C("label3", autosize=True, dock="Fill", margin=(0, 12, 0, 0)), 0, 3, 2),
    ]), 0, 1),
    (G("pnl_Status", new="Panel", child=T("tlp_Status", cols=["100%", "auto"], rows=["auto"], autosize=True, cells=[
        (C("lbl_LoginStatus", autosize=True, anchor="Left", margin=0), 0, 0),
        (C("lbl_Version", new="Label", autosize=True, anchor="Right", margin=0), 1, 0),
    ]), dock="Fill", autosize=True, margin=0, padding=(14, 7, 14, 7)), 0, 2),
])
