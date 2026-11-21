"""SPI FK extraction - SQL Server-only Connection panel + run + log."""
from layout_gen import C, T, G
from _connection import PANEL_DROP, connection_box

DESIGNER = "SPI FK Extraction.Designer.cs"
FORM_NAME = "SPI_FK_Extraction"
CLIENT = (1100, 760)
MINIMUM = (900, 640)
TITLE = "SPI FK extraction"
DROP = PANEL_DROP

TEXTS = {
    "gb_DBConnection": "Connection", "btn_TestDBConnection": "Test connection",
    "groupBox1": "Run", "btn_ImportExcel": "Import Excel…",
    "gb_Logs": "Log", "btn_ClearLogs": "Clear log", "lbl_Progress": "Progress",
}

LOG = T("tlp_Log", cols=["100%", "auto"], rows=["auto", "auto", "100%"], cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 8, 4)), 0, 0),
    (C("btn_ClearLogs", size=(110, 30), margin=0), 1, 0),
    (C("lbl_Progress", autosize=True, anchor="Left", margin=(0, 4, 0, 4)), 0, 1, 2),
    (C("rtb_Log", dock="Fill", margin=0), 0, 2, 2),
])

LAYOUT = T("tlp_Root", cols=["100%", "240"], rows=["auto", "auto", "100%"], cells=[
    (C("step_Strip", new="SPI_Data_Comp_Tool.StepStrip", dock="Fill", size=(800, 66), margin=(0, 0, 0, 12)), 0, 0, 2),
    (connection_box("gb_DBConnection", below=[
        C("btn_TestDBConnection", size=(150, 30), anchor="Right", margin=(0, 8, 0, 0))]), 0, 1),
    (G("groupBox1", T("tlp_Run", cols=["100%"], rows=["auto", "100%"], cells=[
        (C("btn_ImportExcel", dock="Fill", size=(160, 36), margin=0), 0, 0),
    ]), margin=0), 1, 1),
    (G("gb_Logs", LOG, margin=(0, 12, 0, 0)), 0, 2, 2),
])
