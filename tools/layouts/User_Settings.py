"""User Settings - layout spec for tools/layout_gen.py."""
from layout_gen import C, T, F, G

DESIGNER = "User_Settings.Designer.cs"
FORM_NAME = "User_Settings"
CLIENT = (680, 440)
MINIMUM = (560, 380)

TEXTS = {
    "gb_SaveExcel": "Save Excel files with",
    "rb_EPPlus": "EPPlus (recommended, no Excel needed)",
    "rb_XLSave": "Microsoft Excel (needs Excel installed)",
    "gb_Logs": "Log",
    "btn_ClearLogs": "Clear log",
    "lbl_Progress": "",
}

LAYOUT = T("tlp_Root", cols=["100%"], rows=["auto", "100%"], cells=[
    (G("gb_SaveExcel", T("tlp_SaveExcel", cols=["100%"], rows=["auto", "auto"], autosize=True, cells=[
        (C("rb_EPPlus", autosize=True, anchor="Left"), 0, 0),
        (C("rb_XLSave", autosize=True, anchor="Left"), 0, 1),
    ]), dock="Fill", autosize=True, margin=(0, 0, 0, 12)), 0, 0),
    (G("gb_Logs", T("tlp_Logs", cols=["auto", "100%"], rows=["100%", "auto"], cells=[
        (C("rtb_Log", dock="Fill", margin=(0, 0, 0, 8)), 0, 0, 2),
        (C("btn_ClearLogs", size=(110, 30), anchor="Left", margin=0), 0, 1),
        (C("lbl_Progress", autosize=True, anchor="Left", margin=(12, 0, 0, 0)), 1, 1),
    ])), 0, 1),
])
