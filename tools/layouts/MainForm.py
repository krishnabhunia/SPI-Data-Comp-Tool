"""Main window - layout spec for tools/layout_gen.py."""
from layout_gen import C, T, F, G

DESIGNER = "MainForm.Designer.cs"
FORM_NAME = "MainForm"
CLIENT = (760, 440)
MINIMUM = (640, 400)
FORM_PADDING = 0
TOP = ["menuStrip1"]           # docked to the top, outside the layout table

# the old login box and its two holder panels are gone (login removed in 2.2.0)
DROP = ["gb_logincredentials", "btn_Login", "tb_username", "tb_password", "label1", "label4",
        "gb_AppDetails", "panel_control",
        # the first Delta Change Tracker window: its menu entry was always hidden, the window is removed
        "sPIDeltaChangeTrackerToolStripMenuItem"]

TEXTS = {
    "btn_Exit_Logout": "Exit",
    "btn_Open_DB_Comparison": "Compare databases",
    "btn_Excel_Comparison": "Compare Excel files",
    "lbl_LoginStatus": "Ready",
    "checkForUpdatesToolStripMenuItem": "Check for &updates…",
}

PROPS = {
    "pictureBox1": {"SizeMode": "System.Windows.Forms.PictureBoxSizeMode.Zoom"},
    "pictureBox2": {"SizeMode": "System.Windows.Forms.PictureBoxSizeMode.Zoom"},
    "label3": {"ForeColor": "System.Drawing.Color.FromArgb(90, 90, 90)"},
    "lbl_Version": {"ForeColor": "System.Drawing.Color.FromArgb(90, 90, 90)"},
    "lbl_LoginStatus": {"ForeColor": "System.Drawing.Color.FromArgb(90, 90, 90)"},
}

LAYOUT = T("tlp_Root", cols=["50%", "50%"], rows=["100%", "auto"], padding=(16, 16, 16, 8), cells=[
    (T("tlp_Pictures", cols=["100%"], rows=["55%", "45%"], cells=[
        (C("pictureBox1", dock="Fill", margin=(0, 0, 0, 8)), 0, 0),
        (C("pictureBox2", dock="Fill", margin=0), 0, 1),
    ], margin=(0, 0, 16, 0)), 0, 0),
    (T("tlp_About", cols=["100%"], rows=["auto", "auto", "auto", "auto", "100%", "auto"], cells=[
        (C("label2", anchor="Left, Right", autosize=True, margin=(0, 4, 0, 12)), 0, 0),
        (C("label3", anchor="Left, Right", autosize=True, margin=(0, 0, 0, 20)), 0, 1),
        (C("btn_Open_DB_Comparison", anchor="Left, Right", size=(200, 36), margin=(0, 0, 0, 8)), 0, 2),
        (C("btn_Excel_Comparison", anchor="Left, Right", size=(200, 36), margin=0), 0, 3),
        (C("btn_Exit_Logout", anchor="Right", size=(100, 32), margin=0), 0, 5),
    ]), 1, 0),
    (F("flp_Status", [
        C("lbl_LoginStatus", autosize=True, margin=(0, 0, 16, 0)),
        C("lbl_Version", new="Label", autosize=True, margin=0),
    ], margin=(0, 12, 0, 0)), 0, 1, 2),
])
