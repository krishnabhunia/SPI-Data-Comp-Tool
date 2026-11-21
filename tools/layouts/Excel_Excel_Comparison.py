"""Compare Excel files - controlled steps: source, target, load, keys, columns, validate, compare."""
from layout_gen import C, T, F, G

DESIGNER = "Excel_Excel_Comparison.Designer.cs"
FORM_NAME = "Excel_Excel_Comparison"
TITLE = "Compare Excel files"
CLIENT = (1180, 760)
MINIMUM = (980, 660)

TEXTS = {
    "groupBox1": "Workbooks", "btn_FirstExcelSheet": "Choose source…", "btn_SecondExcelSheet": "Choose target…",
    "lbl_FirstExcelFileName": "No file selected", "lbl_SecondExcelFileName": "No file selected",
    "cb_MultiReference": "Different column names / RefID comparison", "btn_SelectLoad": "Load both files",
    "lbl_LoadStatus": "Choose both workbooks, then load them.",
    "groupBox2": "Keys and columns", "label3": "Reference key", "cb_Primary_Key": "Reference key is the primary key",
    "label4": "Source columns", "cb_select_first_clb": "Select all / none", "label5": "Target columns",
    "label6": "Ignored in Source", "label7": "Ignored in Target",
    "groupBox4": "Compare", "cb_IncludeDiff_Columns": "Only columns that differ", "cb_IgnoreCase": "Ignore case",
    "cb_IncludeRule": "Include rules", "btn_RuleForm": "Open rules…", "btn_Validate": "Validate",
    "btn_Compare": "Compare and generate Excel", "btn_Cancel": "Cancel", "label1": "Report", "llbl_filePath": "",
    "groupBox3": "Status", "label8": "Status:", "lbl_Status": "Okay", "lbl_Parameter": "", "lbl_Timer": "",
    "groupBox5": "Progress", "lbl_Percentage": "0 %",
    "groupBox7": "Instrument analysis",
}


def column(table, label, lst, below=None):
    cells = [(C(label, autosize=True, anchor="Left", margin=(0, 0, 0, 4)), 0, 0),
             (C(lst, dock="Fill", margin=0), 0, 1)]
    rows = ["auto", "100%"]
    if below:
        cells.append((C(below, autosize=True, anchor="Left", margin=(0, 4, 0, 0)), 0, 2))
        rows.append("auto")
    return T(table, cols=["100%"], rows=rows, cells=cells, dock="Fill", margin=(4, 0, 4, 0))


FILES = T("tlp_Files", cols=["150", "100%"], rows=["auto", "auto", "auto", "auto"], autosize=True, cells=[
    (C("btn_FirstExcelSheet", new="Button", dock="Fill", size=(140, 30), margin=(0, 2, 10, 2)), 0, 0),
    (C("lbl_FirstExcelFileName", anchor="Left, Right", autosize=False, size=(400, 30), margin=0), 1, 0),
    (C("btn_SecondExcelSheet", new="Button", dock="Fill", size=(140, 30), margin=(0, 2, 10, 2)), 0, 1),
    (C("lbl_SecondExcelFileName", anchor="Left, Right", autosize=False, size=(400, 30), margin=0), 1, 1),
    (C("cb_MultiReference", autosize=True, anchor="Left", margin=(0, 6, 0, 2)), 0, 2, 2),
    (C("btn_SelectLoad", dock="Fill", size=(140, 32), margin=(0, 2, 10, 2)), 0, 3),
    (C("lbl_LoadStatus", anchor="Left, Right", autosize=False, size=(400, 32), margin=0), 1, 3),
])

VIEW = T("tlp_View", cols=["22%", "22%", "22%", "17%", "17%"], rows=["100%"], cells=[
    (column("tlp_RefKey", "label3", "clb_reference_ID", "cb_Primary_Key"), 0, 0),
    (column("tlp_SrcCols", "label4", "clb_firstExcel", "cb_select_first_clb"), 1, 0),
    (column("tlp_TrgCols", "label5", "clb_secondExcel"), 2, 0),
    (column("tlp_IgnSrc", "label6", "lb_IgnoreTable1"), 3, 0),
    (column("tlp_IgnTrg", "label7", "lb_IgnoreTable2"), 4, 0),
])

COMPARE = T("tlp_Compare", cols=["50%", "50%"], rows=["auto"] * 8, autosize=True, cells=[
    (C("cb_IncludeDiff_Columns", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 0, 2),
    (C("cb_IgnoreCase", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 1, 2),
    (C("cb_IncludeRule", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 2),
    (C("btn_RuleForm", size=(110, 28), anchor="Right", margin=(0, 3, 0, 3)), 1, 2),
    (C("btn_Validate", dock="Fill", size=(120, 32), margin=(0, 10, 0, 4)), 0, 3, 2),
    (C("btn_Compare", dock="Fill", size=(120, 36), margin=(0, 4, 0, 4)), 0, 4, 2),
    (C("btn_Cancel", dock="Fill", size=(100, 30), margin=(0, 0, 0, 6)), 0, 5, 2),
    (C("label1", autosize=True, anchor="Left", margin=(0, 4, 0, 2)), 0, 6, 2),
    (C("llbl_filePath", anchor="Left, Right", autosize=False, size=(100, 40), margin=0), 0, 7, 2),
])

STATUS = T("tlp_Status", cols=["auto", "100%"], rows=["auto", "auto", "auto", "100%"], cells=[
    (C("label8", autosize=True, anchor="Left", margin=(0, 0, 6, 4)), 0, 0),
    (C("lbl_Status", anchor="Left, Right", autosize=False, size=(100, 60), margin=(0, 0, 0, 4)), 1, 0),
    (C("lbl_Parameter", autosize=True, anchor="Left", margin=(0, 0, 0, 4)), 0, 1, 2),
    (C("lbl_Timer", autosize=True, anchor="Left", margin=(0, 0, 0, 4)), 0, 2, 2),
    (C("lblError", dock="Fill", margin=0), 0, 3, 2),
])

PROGRESS = T("tlp_Progress", cols=["100%", "auto"], rows=["auto"], autosize=True, cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 16, 4)), 0, 0),
    (C("lbl_Percentage", autosize=True, anchor="Left", minsize=(120, 0), margin=0), 1, 0),
])

INSTRUMENT = T("tlp_Instrument", cols=["100%", "auto"], rows=["auto"], autosize=True, cells=[
    (C("lbl_InstrumentText", anchor="Left, Right", autosize=False, size=(400, 40), margin=(0, 0, 12, 0)), 0, 0),
    (F("flp_Instrument", [
        C("btnLoad", size=(300, 30), margin=(0, 0, 6, 0)),
        C("button1", size=(75, 30), margin=(0, 0, 6, 0)),
        C("btn_TestSystemMemoryOutException", size=(220, 30), margin=0),
    ], autosize=True, anchor="Right", margin=0), 1, 0),
])

LAYOUT = T("tlp_Root", cols=["100%", "320"], rows=["auto", "auto", "100%", "auto", "auto"], cells=[
    (C("step_Strip", new="SPI_Data_Comp_Tool.StepStrip", dock="Fill", size=(800, 66), margin=(0, 0, 0, 12)), 0, 0, 2),
    (G("groupBox1", FILES, autosize=True, margin=(0, 0, 12, 0)), 0, 1),
    (G("groupBox2", VIEW, margin=(0, 12, 12, 0)), 0, 2),
    (T("tlp_Right", cols=["100%"], rows=["auto", "100%"], cells=[
        (G("groupBox4", COMPARE, autosize=True, margin=(0, 0, 0, 12)), 0, 0),
        (G("groupBox3", STATUS, margin=0), 0, 1),
    ], dock="Fill", margin=0), 1, 1, 1, 2),
    (G("groupBox5", PROGRESS, autosize=True, margin=(0, 12, 0, 0)), 0, 3, 2),
    (G("groupBox7", INSTRUMENT, autosize=True, margin=(0, 12, 0, 0)), 0, 4, 2),
])
