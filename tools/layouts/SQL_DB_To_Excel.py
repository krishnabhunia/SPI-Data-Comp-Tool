"""Export database to Excel - single-connection panel + table list + save."""
from layout_gen import C, T, F, G
from _connection import PANEL_DROP, connection_box

DESIGNER = "SQL_DB_To_Excel.Designer.cs"
FORM_NAME = "SQL_DB_To_Excel"
CLIENT = (1220, 800)
MINIMUM = (1000, 680)
TITLE = "Export database to Excel"
# this window names its one connection's fields without SRC; label15 / groupBox4 held the old profile picker
DROP = PANEL_DROP + ["rb_Oracle", "rb_SQL", "cb_windowsAuthentication", "cb_DefaultPort", "tb_DefaultPort",
                     "groupBox4", "label15"]

TEXTS = {
    "groupBox2": "Connection", "rb_SRC": "Source", "rb_TRG": "Target",
    "btn_Connect": "Connect and load tables", "btn_Cancel": "Cancel", "lbl_Status": "Ready",
    "groupBox3": "Save Excel to", "btn_ExportToExcel": "Export to Excel",
    "groupBox1": "Tables", "lbl_Find": "Find", "cb_SelectAll": "Select all", "cb_SkipEmptyTable": "Skip empty tables",
}

TABLES = T("tlp_Tables", cols=["auto", "100%", "auto", "auto"], rows=["auto", "100%"], cells=[
    (C("lbl_Find", autosize=True, anchor="Left", margin=(0, 3, 8, 3)), 0, 0),
    (C("tb_Find", anchor="Left, Right", margin=(0, 3, 16, 3)), 1, 0),
    (C("cb_SelectAll", autosize=True, anchor="Left", margin=(0, 3, 16, 3)), 2, 0),
    (C("cb_SkipEmptyTable", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 3, 0),
    (C("clb_TableList", dock="Fill", margin=(0, 8, 0, 0)), 0, 1, 4),
])

SAVE = T("tlp_Save", cols=["100%"], rows=["100%", "auto"], cells=[
    (C("linkLabel1", dock="Fill", margin=(0, 0, 0, 8)), 0, 0),
    (C("btn_ExportToExcel", dock="Fill", size=(160, 36), margin=0), 0, 1),
])

LAYOUT = T("tlp_Root", cols=["420", "100%"], rows=["auto", "100%"], cells=[
    (connection_box("groupBox2", below=[
        F("flp_LoadFrom", [C("lbl_LoadFrom", new="Label", text="Fill from profile half", autosize=True, margin=(0, 4, 12, 0)),
                           C("rb_SRC", autosize=True, margin=(0, 2, 16, 0)), C("rb_TRG", autosize=True, margin=(0, 2, 0, 0))],
          margin=(0, 8, 0, 0)),
        F("flp_Connect", [C("btn_Connect", size=(190, 32), margin=(0, 0, 8, 0)), C("btn_Cancel", size=(100, 32), margin=0)],
          margin=(0, 8, 0, 0)),
        C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 8, 0, 0)),
        C("lbl_Status", anchor="Left, Right", autosize=True, margin=(0, 6, 0, 0)),
    ]), 0, 0),
    (G("groupBox3", SAVE, margin=(0, 12, 12, 0)), 0, 1),
    (G("groupBox1", TABLES, margin=0), 1, 0, 1, 2),
])
