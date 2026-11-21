"""Optimised delta tracker: shared connection panel, existing run controls and log."""
from layout_gen import C, T, G, TABS
from _connection import PANEL_DROP, LABEL, FIELD, panel, table_picker

DESIGNER = "SPIDeltaChangeTrackerOptimised.Designer.cs"
CLEAN_WHITESPACE = True
FORM_NAME = "SPIDeltaChangeTrackerOptimised"
CLIENT = (1260, 820)
MINIMUM = (1080, 720)
TITLE = "Delta change tracker"
DROP = PANEL_DROP + ["groupBox4", "label18",
                     "lblDebugMsg"]   # hidden debug label that no code used
FROZEN = ["comboBox1"]  # Existing hidden debug control, not an additional live workflow.

TEXTS = {
    "groupBox14": "Connections and tables", "label16": "Source table", "label17": "Target table",
    "btn_Load_SRC_Table": "Load tables", "btn_Load_TRG_Table": "Load tables",
    "cb_FullName": "Show full table names", "label12": "Date column", "label15": "On or after",
    "groupBox3": "Compare and export", "cb_IncludeRule": "Include rules", "btn_RuleForm": "Open rules…",
    "cb_IgnoreCase": "Ignore case", "btnGenerateOutput": "Compare and generate Excel",
    "btn_ListTableExcel": "Choose table-list workbook…", "tabPage2": "Batch table list",
    "btn_Cancel": "Cancel", "btn_Clear_Reset": "Clear / Reset", "label13": "Output folder",
    "gb_Debug": "Operation log", "btn_Clear": "Clear log", "groupBox11": "Progress",
    "label1": "Elapsed", "lblTimer": "", "lbl_PercentageShow": "0 %",
}

TABLES = T("tlp_Tables", cols=["50%", "100", "50%"], rows=["auto", "auto", "auto"], autosize=True, cells=[
    (table_picker("tlp_SourceTable", "label16", "cb_SRC_TableName", "btn_Load_SRC_Table"), 0, 0),
    (table_picker("tlp_TargetTable", "label17", "cb_TRG_TableName", "btn_Load_TRG_Table"), 2, 0),
    (T("tlp_DateFilter", cols=["auto", "100%", "auto", "auto"], rows=["auto"], autosize=True, cells=[
        (C("label12", **LABEL), 0, 0),
        (C("cb_DateCol", **FIELD), 1, 0),
        (C("label15", autosize=True, anchor="Left", margin=(16, 3, 8, 3)), 2, 0),
        (C("dateTimePicker_Date", size=(180, 25), anchor="Left", margin=0), 3, 0),
    ], margin=(0, 8, 0, 0)), 0, 1, 3),
    (C("cb_FullName", autosize=True, anchor="Left", margin=(0, 8, 0, 0)), 0, 2, 3),
], margin=(0, 8, 0, 0))

RUN = T("tlp_Run", cols=["50%", "50%"], rows=["auto"] * 7 + ["100%"], cells=[
    (C("cb_IncludeRule", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 0),
    (C("btn_RuleForm", size=(110, 30), anchor="Right", margin=0), 1, 0),
    (C("cb_IgnoreCase", autosize=True, anchor="Left", margin=(0, 6, 0, 8)), 0, 1, 2),
    (C("btnGenerateOutput", dock="Fill", size=(160, 36), margin=(0, 0, 0, 8)), 0, 2, 2),
    (C("btn_Cancel", dock="Fill", size=(100, 30), margin=(0, 0, 4, 0)), 0, 3),
    (C("btn_Clear_Reset", dock="Fill", size=(100, 30), margin=(4, 0, 0, 0)), 1, 3),
    (TABS("tabControl1", [G("tabPage2", T("tlp_Batch", cols=["100%"], rows=["auto"], autosize=True, cells=[
        (C("btn_ListTableExcel", dock="Fill", size=(150, 34), margin=0), 0, 0),
    ]))], size=(280, 84), margin=(0, 12, 0, 8)), 0, 4, 2),
    (C("label13", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 5, 2),
    (C("llbl_Folder_path", anchor="Left, Right", size=(220, 38), margin=0), 0, 6, 2),
])

PROGRESS = T("tlp_Progress", cols=["100%", "auto", "auto", "auto"], rows=["auto"], autosize=True, cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 16, 4)), 0, 0),
    (C("lbl_PercentageShow", autosize=True, anchor="Left", margin=(0, 0, 20, 0)), 1, 0),
    (C("label1", autosize=True, anchor="Left", margin=(0, 0, 8, 0)), 2, 0),
    (C("lblTimer", autosize=True, anchor="Left", minsize=(80, 0), margin=0), 3, 0),
])

LAYOUT = T("tlp_Root", cols=["100%"], rows=["auto", "auto", "100%", "auto"], cells=[
    (C("step_Strip", new="SPI_Data_Comp_Tool.StepStrip", dock="Fill", size=(800, 66), margin=(0, 0, 0, 12)), 0, 0),
    (T("tlp_Top", cols=["100%", "320"], rows=["auto"], autosize=True, cells=[
        (G("groupBox14", T("tlp_Connection", cols=["100%"], rows=["auto", "auto"], autosize=True, cells=[
            (panel(), 0, 0),
            (TABLES, 0, 1),
        ]), autosize=True, margin=(0, 0, 12, 0)), 0, 0),
        (G("groupBox3", RUN, margin=0), 1, 0),
    ]), 0, 1),
    (G("gb_Debug", T("tlp_Log", cols=["100%", "auto"], rows=["100%"], cells=[
        (C("rtb_Log", dock="Fill", minsize=(0, 130), margin=(0, 0, 8, 0)), 0, 0),
        (C("btn_Clear", size=(95, 30), anchor="Top, Right", margin=0), 1, 0),
    ]), margin=(0, 12, 0, 0)), 0, 2),
    (G("groupBox11", PROGRESS, autosize=True, margin=(0, 12, 0, 0)), 0, 3),
])
