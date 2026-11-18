"""SPI Delta Change Legacy ID - Connection panel + controls + status log."""
from layout_gen import C, T, G
from _connection import PANEL_DROP, TEXTS as CONN_TEXTS, connection_box

DESIGNER = "SPIDeltaChangeLegacyID.Designer.cs"
FORM_NAME = "SPIDeltaChangeLegacyID"
CLIENT = (1260, 860)
MINIMUM = (1080, 720)
TITLE = "SPI delta change legacy ID"
DROP = PANEL_DROP + ["groupBox2", "label15", "button1"]

TEXTS = dict(CONN_TEXTS, **{
    "groupBox14": "Connection",
    "groupBox1": "Run", "btn_Connect_Reference_ID": "Test connections",
    "label1": "Choose the Excel file to generate legacy IDs from.", "btn_ListTableExcel": "Browse…",
    "groupBox10": "Status", "btn_Clear_Reset": "Clear / Reset",
})

RUN = T("tlp_Run", cols=["100%"], rows=["auto", "auto", "auto", "100%"], cells=[
    (C("btn_Connect_Reference_ID", dock="Fill", size=(160, 32), margin=(0, 0, 0, 12)), 0, 0),
    (C("label1", anchor="Left, Right", autosize=True, margin=(0, 0, 0, 6)), 0, 1),
    (C("btn_ListTableExcel", dock="Fill", size=(160, 36), margin=0), 0, 2),
])

STATUS = T("tlp_Status", cols=["100%", "auto"], rows=["auto", "100%"], cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 8, 4)), 0, 0),
    (C("btn_Clear_Reset", size=(110, 30), margin=0), 1, 0),
    (C("rtb_Log", dock="Fill", margin=(0, 8, 0, 0)), 0, 1, 2),
])

LAYOUT = T("tlp_Root", cols=["100%", "320"], rows=["auto", "100%"], cells=[
    (connection_box("groupBox14"), 0, 0),
    (G("groupBox1", RUN, margin=0), 1, 0),
    (G("groupBox10", STATUS, margin=(0, 12, 0, 0)), 0, 1, 2),
])
