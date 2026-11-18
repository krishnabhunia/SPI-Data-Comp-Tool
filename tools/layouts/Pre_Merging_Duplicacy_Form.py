"""Pre-merging duplicacy check - Connection panel + run options + status log."""
from layout_gen import C, T, G
from _connection import PANEL_DROP, TEXTS as CONN_TEXTS, connection_box

DESIGNER = "Pre_Merging_Duplicacy_Form.Designer.cs"
FORM_NAME = "Pre_Merging_Duplicacy_Form"
CLIENT = (1260, 860)
MINIMUM = (1080, 720)
TITLE = "Pre-merging duplicacy check"
DROP = PANEL_DROP + ["groupBox2", "label15", "button1"]
FROZEN = ["groupBox1"]          # hidden box with the table pickers the code still fills; left untouched

TEXTS = dict(CONN_TEXTS, **{
    "groupBox14": "Connection",
    "groupBox3": "Run", "btn_Connect_Reference_ID": "Test connections",
    "cb_ConfigurationMode": "Run in configuration mode", "cb_IgnoreCase": "Ignore case",
    "btn_ListTableExcel": "Run pre-merging Excel process", "lb_link": "Open the output folder:",
    "groupBox10": "Status", "btn_Clear_Reset": "Clear / Reset",
})

RUN = T("tlp_Run", cols=["100%"], rows=["auto"] * 5 + ["100%"], cells=[
    (C("btn_Connect_Reference_ID", dock="Fill", size=(160, 32), margin=(0, 0, 0, 8)), 0, 0),
    (C("cb_ConfigurationMode", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 1),
    (C("cb_IgnoreCase", autosize=True, anchor="Left", margin=(0, 3, 0, 8)), 0, 2),
    (C("btn_ListTableExcel", dock="Fill", size=(160, 36), margin=(0, 0, 0, 8)), 0, 3),
    (C("lb_link", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 4),
    (C("llb_File", dock="Fill", margin=0), 0, 5),
])

STATUS = T("tlp_Status", cols=["100%", "auto"], rows=["auto", "100%"], cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 8, 4)), 0, 0),
    (C("btn_Clear_Reset", size=(110, 30), margin=0), 1, 0),
    (C("rtb_Log", dock="Fill", margin=(0, 8, 0, 0)), 0, 1, 2),
])

LAYOUT = T("tlp_Root", cols=["100%", "320"], rows=["auto", "100%"], cells=[
    (connection_box("groupBox14"), 0, 0),
    (G("groupBox3", RUN, margin=0), 1, 0),
    (G("groupBox10", STATUS, margin=(0, 12, 0, 0)), 0, 1, 2),
])
