"""User profiles (UserProfileForm) - the Connection panel in profile-editor use."""
from layout_gen import C, T, F, G
from _connection import PANEL_DROP, LABEL, FIELD, panel, table_picker

DESIGNER = "UserProfileForm.Designer.cs"
FORM_NAME = "UserProfileForm"
CLIENT = (1180, 760)
MINIMUM = (980, 640)
TITLE = "User profiles"

# label1 was the old "Select profile" caption; the panel has its own
DROP = PANEL_DROP + ["label1"]

TEXTS = {
    "groupBox14": "Connection", "cb_FullName": "Show full names in lists",
    "label16": "Source table", "label17": "Target table",
    "btn_Load_SRC_Table": "Load tables", "btn_Load_TRG_Table": "Load tables",
    "btn_Connect_Reference_ID": "Connect and load tables",
    "groupBox2": "Profile", "btnSaveUserProfile": "Save profile", "btnDeleteUserProfile": "Delete profile",
    "groupBox1": "Profile file", "label10": "Profiles are read from a .json file.",
    "l_lb_ReadProfile": "Choose a profile file…",
    "groupBox10": "Log", "btn_ClearLogs": "Clear log",
}




TABLES = T("tlp_Tables", cols=["50%", "100", "50%"], rows=["auto", "auto"], autosize=True, cells=[
    (table_picker("tlp_SRC_Table", "label16", "cb_SRC_TableName", "btn_Load_SRC_Table"), 0, 0),
    (table_picker("tlp_TRG_Table", "label17", "cb_TRG_TableName", "btn_Load_TRG_Table"), 2, 0),
    (C("cb_FullName", autosize=True, anchor="Left", margin=(0, 6, 0, 0)), 0, 1),
    (C("btn_Connect_Reference_ID", size=(190, 32), anchor="Right", margin=(0, 6, 0, 0)), 2, 1),
], margin=(0, 8, 0, 0))

SIDE = T("tlp_Side", cols=["100%"], rows=["auto", "100%"], cells=[
    (G("groupBox2", T("tlp_Profile", cols=["100%"], rows=["auto", "auto"], autosize=True, cells=[
        (C("btnSaveUserProfile", dock="Fill", size=(120, 32), margin=(0, 0, 0, 8)), 0, 0),
        (C("btnDeleteUserProfile", dock="Fill", size=(120, 32), margin=0), 0, 1),
    ]), autosize=True, margin=(0, 0, 0, 12)), 0, 0),
    (G("groupBox1", T("tlp_ProfileFile", cols=["100%"], rows=["auto", "auto", "100%"], cells=[
        (C("label10", anchor="Left, Right", autosize=True, margin=(0, 0, 0, 6)), 0, 0),
        (C("l_lb_ReadProfile", anchor="Left, Right", autosize=True, margin=0), 0, 1),
    ]), margin=0), 0, 1),
])

LAYOUT = T("tlp_Root", cols=["100%", "240"], rows=["auto", "100%"], cells=[
    (G("groupBox14", T("tlp_Connection", cols=["100%"], rows=["auto", "auto"], autosize=True, cells=[
        (panel(), 0, 0),
        (TABLES, 0, 1),
    ]), autosize=True, margin=(0, 0, 12, 0)), 0, 0),
    (SIDE, 1, 0),
    (G("groupBox10", T("tlp_Log", cols=["100%"], rows=["100%", "auto"], cells=[
        (C("rtb_Log", dock="Fill", margin=0), 0, 0),
        (C("btn_ClearLogs", size=(110, 30), anchor="Left", margin=(0, 8, 0, 0)), 0, 1),
    ]), margin=(0, 12, 0, 0)), 0, 1, 2),
])
