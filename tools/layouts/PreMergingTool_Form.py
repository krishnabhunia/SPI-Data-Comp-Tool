"""Pre-merging tool - single-connection panel (+ schema) + task tabs + output + log."""
from layout_gen import C, T, F, G
from _connection import PANEL_DROP, LABEL, FIELD, connection_box

DESIGNER = "PreMergingTool_Form.Designer.cs"
FORM_NAME = "PreMergingTool_Form"
CLIENT = (1280, 860)
MINIMUM = (1080, 720)
TITLE = "Pre-merging tool"
# groupBox12 is this window's Connection group, so it stays; comboBox1 was a hidden debug picker
DROP = [d for d in PANEL_DROP if d != "groupBox12"] + ["label15", "comboBox1"]
KEEP = ["tabControl1"]          # the two task tabs keep their own layout

TEXTS = {
    "groupBox12": "Connection", "cb_Schema": "Schema name", "btn_Connect": "Test connection",
    "groupBox3": "Output", "lb_link": "Open the output file:",
    "groupBox2": "Log", "btn_ClearLogs": "Clear log", "lbl_Progress": "Progress",
}

LOG = T("tlp_Log", cols=["100%", "auto"], rows=["auto", "auto", "100%"], cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 8, 4)), 0, 0),
    (C("btn_ClearLogs", size=(110, 30), margin=0), 1, 0),
    (C("lbl_Progress", autosize=True, anchor="Left", margin=(0, 4, 0, 4)), 0, 1, 2),
    (C("rtb_Log", dock="Fill", margin=0), 0, 2, 2),
])

RIGHT = T("tlp_Right", cols=["100%"], rows=["auto", "100%"], cells=[
    (C("tabControl1", dock="Fill", size=(700, 180), margin=(0, 0, 0, 12)), 0, 0),
    (G("groupBox3", T("tlp_Output", cols=["100%"], rows=["auto", "100%"], cells=[
        (C("lb_link", autosize=True, anchor="Left", margin=(0, 0, 0, 4)), 0, 0),
        (C("llb_File", dock="Fill", margin=0), 0, 1),
    ]), margin=0), 0, 1),
])

LAYOUT = T("tlp_Root", cols=["440", "100%"], rows=["auto", "100%"], cells=[
    (connection_box("groupBox12", below=[
        T("tlp_Schema", cols=["auto", "100%"], rows=["auto"], autosize=True, cells=[
            (C("cb_Schema", **LABEL), 0, 0), (C("tb_SchemaName", **FIELD), 1, 0)], margin=(0, 8, 0, 0)),
        C("btn_Connect", size=(150, 30), anchor="Right", margin=(0, 8, 0, 0)),
    ]), 0, 0),
    (RIGHT, 1, 0),
    (G("groupBox2", LOG, margin=(0, 12, 0, 0)), 0, 1, 2),
])
