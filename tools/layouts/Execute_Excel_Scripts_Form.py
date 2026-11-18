"""Execute scripts - single-connection panel + the three input tabs + status log."""
from layout_gen import C, T, F, G
from _connection import PANEL_DROP, connection_box

DESIGNER = "Execute Excel Scripts Form.Designer.cs"
FORM_NAME = "Execute_Excel_Scripts_Form"
CLIENT = (1300, 820)
MINIMUM = (1100, 700)
TITLE = "Execute scripts"
# groupBox12 is this window's Connection group; label2/4/6/8 here are captions inside the kept tabs
DROP = [d for d in PANEL_DROP if d not in ("groupBox12", "label2", "label4", "label6", "label8")] + [
    "rb_Oracle", "rb_SQL", "cb_windowsAuthentication", "cb_DefaultPort", "tb_DefaultPort",
    "groupBox4", "groupBox2", "label15"]
KEEP = ["tabControl1"]          # Excel / text / PNID report tabs keep their own layout

TEXTS = {
    "groupBox12": "Connection", "rb_SRC": "Source", "rb_TRG": "Target", "btnTestConnecton": "Test connection",
    "groupBox10": "Status", "btn_Clear_Reset": "Clear / Reset",
}

STATUS = T("tlp_Status", cols=["100%", "auto"], rows=["auto", "100%"], cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 8, 4)), 0, 0),
    (C("btn_Clear_Reset", size=(110, 30), margin=0), 1, 0),
    (C("rtb_Log", dock="Fill", margin=(0, 8, 0, 0)), 0, 1, 2),
])

LAYOUT = T("tlp_Root", cols=["420", "100%"], rows=["auto", "100%"], cells=[
    (connection_box("groupBox12", below=[
        F("flp_LoadFrom", [C("lbl_LoadFrom", new="Label", text="Fill from profile half", autosize=True, margin=(0, 4, 12, 0)),
                           C("rb_SRC", autosize=True, margin=(0, 2, 16, 0)), C("rb_TRG", autosize=True, margin=(0, 2, 0, 0))],
          margin=(0, 8, 0, 0)),
        C("btnTestConnecton", size=(150, 30), anchor="Right", margin=(0, 8, 0, 0)),
    ]), 0, 0),
    (C("tabControl1", dock="Fill", size=(860, 400), margin=0), 1, 0),
    (G("groupBox10", STATUS, margin=(0, 12, 0, 0)), 0, 1, 2),
])
