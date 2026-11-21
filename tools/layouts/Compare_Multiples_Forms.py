"""Compare list tables - DB / Excel tabs (Connection panel in the DB tab) + options + log."""
from layout_gen import C, T, G, TABS
from _connection import PANEL_DROP, TEXTS as CONN_TEXTS, connection_box

DESIGNER = "Compare_Multiples_Forms.Designer.cs"
FORM_NAME = "Compare_Multiples_List_Tables_Forms"
CLIENT = (1280, 880)
MINIMUM = (1100, 760)
TITLE = "Compare list tables"
DROP = PANEL_DROP + ["groupBox5", "label15", "button1", "button2"]

TEXTS = dict(CONN_TEXTS, **{
    "tabPage_DB": "Databases", "tabPage_Excel": "Excel folders",
    "gb_DBConnection": "Connection", "btn_TestDBConnection": "Test both connections",
    "gb_Excel": "Excel folders", "label1": "Source folder", "label10": "Target folder",
    "groupBox1": "Compare", "groupBox4": "Output folder", "l_lb_Output_Set": "Choose output folder…",
    "cb_PK_As_ReferenceKey": "Reference key is the primary key", "cb_IncludeDiff_Columns": "Only columns that differ",
    "cb_IgnoreCase": "Ignore case", "cb_IncludeRule": "Include rules", "btn_RuleForm": "Open rules…",
    "btn_ListTableExcel": "Choose list-table Excel…", "btn_Compare": "Compare and generate Excel", "btnCancel": "Cancel",
    "gb_Logs": "Log", "btn_ClearLogs": "Clear log", "lbl_Progress": "Progress",
})

EXCEL = T("tlp_Excel", cols=["100%"], rows=["auto", "50%", "auto", "50%"], cells=[
    (C("label1", autosize=True, anchor="Left", margin=(0, 0, 0, 4)), 0, 0),
    (C("llb_Excel_SRC", dock="Fill", margin=(0, 0, 0, 12)), 0, 1),
    (C("label10", autosize=True, anchor="Left", margin=(0, 0, 0, 4)), 0, 2),
    (C("llb_Excel_TRG", dock="Fill", margin=0), 0, 3),
])

TABS_NODE = TABS("tabControl1", [
    G("tabPage_DB", connection_box("gb_DBConnection", below=[
        C("btn_TestDBConnection", size=(170, 30), anchor="Right", margin=(0, 8, 0, 0))])),
    G("tabPage_Excel", G("gb_Excel", EXCEL, margin=0)),
], size=(800, 440), margin=(0, 0, 12, 0))

COMPARE = T("tlp_Compare", cols=["100%", "auto"], rows=["100%"] + ["auto"] * 7, cells=[
    (G("groupBox4", T("tlp_Output", cols=["100%"], rows=["auto", "100%"], cells=[
        (C("l_lb_Output_Set", autosize=True, anchor="Left", margin=(0, 0, 0, 6)), 0, 0),
        (C("l_lbl_outputFolder", dock="Fill", margin=0), 0, 1),
    ]), margin=(0, 0, 0, 8)), 0, 0, 2),
    (C("cb_PK_As_ReferenceKey", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 1, 2),
    (C("cb_IncludeDiff_Columns", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 2, 2),
    (C("cb_IgnoreCase", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 3, 2),
    (C("cb_IncludeRule", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 4),
    (C("btn_RuleForm", size=(110, 28), anchor="Right", margin=(0, 3, 0, 3)), 1, 4),
    (C("btn_ListTableExcel", dock="Fill", size=(160, 32), margin=(0, 8, 0, 4)), 0, 5, 2),
    (C("btn_Compare", dock="Fill", size=(160, 36), margin=(0, 4, 0, 4)), 0, 6, 2),
    (C("btnCancel", dock="Fill", size=(100, 30), margin=(0, 4, 0, 0)), 0, 7, 2),
])

LOG = T("tlp_Log", cols=["100%", "auto"], rows=["auto", "auto", "100%"], cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 8, 4)), 0, 0),
    (C("btn_ClearLogs", size=(110, 30), margin=0), 1, 0),
    (C("lbl_Progress", autosize=True, anchor="Left", margin=(0, 4, 0, 4)), 0, 1, 2),
    (C("rtb_Log", dock="Fill", margin=0), 0, 2, 2),
])

LAYOUT = T("tlp_Root", cols=["100%", "360"], rows=["auto", "auto", "100%"], cells=[
    (C("step_Strip", new="SPI_Data_Comp_Tool.StepStrip", dock="Fill", size=(800, 66), margin=(0, 0, 0, 12)), 0, 0, 2),
    (TABS_NODE, 0, 1),
    (G("groupBox1", COMPARE, margin=0), 1, 1),
    (G("gb_Logs", LOG, margin=(0, 12, 0, 0)), 0, 2, 2),
])
