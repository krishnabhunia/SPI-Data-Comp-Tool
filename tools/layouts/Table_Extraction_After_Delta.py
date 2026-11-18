"""Table Extraction After Delta - Connection panel + input tabs + status log."""
from layout_gen import C, T, F, G
from _connection import PANEL_DROP, TEXTS as CONN_TEXTS, connection_box

DESIGNER = "Table Extraction After Delta.Designer.cs"
FORM_NAME = "Table_Extraction_After_Delta_Form"
CLIENT = (1280, 860)
MINIMUM = (1100, 720)
TITLE = "Table extraction after delta"
DROP = PANEL_DROP + ["groupBox5", "label15"]
KEEP = ["tabControl1"]          # its four tabs each hold one button; kept as designed

TEXTS = dict(CONN_TEXTS, **{
    "gb_DBConnection": "Connection", "btn_TestDBConnection": "Test both connections",
    "groupBox1": "Input", "gb_Selective": "Excel input", "cb_CHG_NUM_From_Excel": "Input from Excel",
    "btn_Cancel": "Cancel", "label1": "Elapsed", "lblTimer": "",
    "groupBox10": "Status", "btn_Clear_Reset": "Clear / Reset",
})

INPUT = T("tlp_Input", cols=["100%"], rows=["auto", "100%", "auto", "auto"], cells=[
    (G("gb_Selective", T("tlp_Selective", cols=["100%"], rows=["auto", "auto"], autosize=True, cells=[
        (C("cb_CHG_NUM_From_Excel", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 0),
        (C("cb_ExcelSheetName", anchor="Left, Right", margin=(0, 3, 0, 3)), 0, 1),
    ]), autosize=True, margin=(0, 0, 0, 8)), 0, 0),
    (C("tabControl1", dock="Fill", margin=0), 0, 1),
    (C("btn_Cancel", size=(100, 30), anchor="Left", margin=(0, 8, 0, 0)), 0, 2),
    (F("flp_Timer", [C("label1", autosize=True, margin=(0, 0, 6, 0)), C("lblTimer", autosize=True, margin=0)],
       margin=(0, 8, 0, 0)), 0, 3),
])

STATUS = T("tlp_Status", cols=["100%", "auto"], rows=["auto", "100%"], cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 8, 4)), 0, 0),
    (C("btn_Clear_Reset", size=(110, 30), margin=0), 1, 0),
    (C("rtb_Log", dock="Fill", margin=(0, 8, 0, 0)), 0, 1, 2),
])

LAYOUT = T("tlp_Root", cols=["100%", "400"], rows=["auto", "100%"], cells=[
    (connection_box("gb_DBConnection", below=[
        C("btn_TestDBConnection", size=(170, 30), anchor="Right", margin=(0, 8, 0, 0))]), 0, 0),
    (G("groupBox1", INPUT, margin=0), 1, 0),
    (G("groupBox10", STATUS, margin=(0, 12, 0, 0)), 0, 1, 2),
])
