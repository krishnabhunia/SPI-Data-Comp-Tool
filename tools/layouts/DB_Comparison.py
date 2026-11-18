"""Compare DB (DB_Comparison_Form) - template for every Source/Target window."""
from layout_gen import C, T, F, G

DESIGNER = "DB Comparison.Designer.cs"
FORM_NAME = "DB_Comparison_Form"
CLIENT = (1180, 700)
MINIMUM = (980, 620)
FROZEN = ["groupBox15", "groupBox16"]          # hidden helper boxes the code still uses; left untouched
DROP = ["groupBox12", "groupBox13", "gb_Source", "gb_Target",   # empty wrappers, not used by the code
        # now inside the shared ConnectionPanel (the window reaches them through properties of the same names)
        "gb_Source1", "gb_Target1", "rb_SRC_oracle", "rb_SRC_SQL", "cb_WindowAuthentication_SRC", "cb_SRC_DefaultPort",
        "tb_SRC_DefaultPort", "tb_SRC_DataSource", "tb_SRC_DBName", "tb_SRC_UserName", "tb_SRC_Password", "rb_TRG_Oracle",
        "rb_TRG_SQL", "cb_WindowAuthentication_TRG", "cb_TRG_DefaultPort", "tb_TRG_DefaultPort", "tb_TRG_DataSource", "tb_TRG_DBName",
        "tb_TRG_UserName", "tb_TRG_Password", "btn_Copy_SRC_TRG", "btn_TRG_SRC", "cb_UserProfileList", "label15",
        "label3", "label5", "label7", "label9", "label2", "label4",
        "label6", "label8", "lbl_SRC_Platform", "lbl_SRC_Port", "lbl_TRG_Platform", "lbl_TRG_Port"]

TEXTS = {
    "groupBox14": "Connection", "cb_FullName": "Show full names in lists",
    "label16": "Source table", "label17": "Target table",
    "btn_Load_SRC_Table": "Load tables", "btn_Load_TRG_Table": "Load tables",
    "groupBox3": "Compare", "cb_ColumnMapping": "Different column names / RefID comparison",
    "cb_IncludeRule": "Include rules", "btn_RuleForm": "Open rules…", "cb_IgnoreCase": "Ignore case",
    "btn_Connect_Reference_ID": "1   Connect and load", "btn_Set": "2   Set and validate",
    "btnGenerateOutput": "3   Compare and generate Excel", "btn_Cancel": "Cancel", "btn_Clear_Reset": "Clear / Reset",
    "label13": "Report", "llbl_File_path": "",
    "groupBox9": "View data", "groupBox7": "Reference key", "cb_PrimaryKey": "Select primary key",
    "groupBox5": "Source columns", "cb_selectAll_toggle": "Select all / none", "groupBox6": "Target columns",
    "groupBox1": "Ignored in Source", "groupBox2": "Ignored in Target", "groupBox10": "Status",
    "lblStatus": "Okay", "lblExpressionStatus": "", "lblCal": "Total rows : columns",
    "groupBox11": "Progress", "lbl_PercentageShow": "0 %", "label1": "Elapsed", "lblTimer": "",
}

PROPS = {
    "lblCal": {"ForeColor": "System.Drawing.Color.FromArgb(90, 90, 90)"},
    "label1": {"ForeColor": "System.Drawing.Color.FromArgb(90, 90, 90)"},
}

LABEL = dict(autosize=True, anchor="Left", margin=(0, 3, 12, 3))
FIELD = dict(anchor="Left, Right", margin=(0, 3, 0, 3))


def table_picker(name, label, combo, button):
    return T(name, cols=["auto", "100%", "auto"], rows=["auto"], autosize=True, cells=[
        (C(label, **LABEL), 0, 0),
        (C(combo, **FIELD), 1, 0),
        (C(button, size=(110, 30), margin=(8, 0, 0, 0)), 2, 0),
    ])


# the Source and Target pickers line up under the panel's two halves (its Copy column is 100 px)
TABLES = T("tlp_Tables", cols=["50%", "100", "50%"], rows=["auto", "auto"], autosize=True, cells=[
    (table_picker("tlp_SRC_Table", "label16", "cb_SRC_TableName", "btn_Load_SRC_Table"), 0, 0),
    (table_picker("tlp_TRG_Table", "label17", "cb_TRG_TableName", "btn_Load_TRG_Table"), 2, 0),
    (C("cb_FullName", autosize=True, anchor="Left", margin=(0, 6, 0, 0)), 0, 1, 3),
], margin=(0, 8, 0, 0))

COMPARE = T("tlp_Compare", cols=["50%", "50%"], rows=["auto"] * 7 + ["100%", "auto", "auto"], cells=[
    (C("cb_ColumnMapping", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 0, 2),
    (C("cb_IncludeRule", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 1),
    (C("btn_RuleForm", size=(110, 28), anchor="Right", margin=(0, 3, 0, 3)), 1, 1),
    (C("cb_IgnoreCase", autosize=True, anchor="Left", margin=(0, 3, 0, 3)), 0, 2, 2),
    (C("btn_Connect_Reference_ID", dock="Fill", size=(120, 34), margin=(0, 12, 0, 4)), 0, 3, 2),
    (C("btn_Set", dock="Fill", size=(120, 34), margin=(0, 4, 0, 4)), 0, 4, 2),
    (C("btnGenerateOutput", dock="Fill", size=(120, 36), margin=(0, 4, 0, 8)), 0, 5, 2),
    (C("btn_Cancel", dock="Fill", size=(100, 30), margin=(0, 0, 4, 0)), 0, 6),
    (C("btn_Clear_Reset", dock="Fill", size=(100, 30), margin=(4, 0, 0, 0)), 1, 6),
    (C("label13", autosize=True, anchor="Left", margin=(0, 6, 0, 2)), 0, 8, 2),
    (C("llbl_File_path", anchor="Left, Right", size=(100, 36), margin=0), 0, 9, 2),
])


def column(group, table, top, lst, margin):
    cells = [(C(lst, dock="Fill", margin=0), 0, 1)]
    if top:
        cells.insert(0, (C(top, autosize=True, anchor="Left", margin=0), 0, 0))
    return G(group, T(table, cols=["100%"], rows=["28", "100%"], cells=cells), margin=margin)


VIEW = T("tlp_View", cols=["20%"] * 5 + ["240"], rows=["100%"], cells=[
    (column("groupBox7", "tlp_RefKey", "cb_PrimaryKey", "clb_ReferenceID", (0, 0, 3, 0)), 0, 0),
    (column("groupBox5", "tlp_SrcCols", "cb_selectAll_toggle", "cbL_SRC_Cols", (3, 0, 3, 0)), 1, 0),
    (column("groupBox6", "tlp_TrgCols", None, "lb_TRG_Cols", (3, 0, 3, 0)), 2, 0),
    (column("groupBox1", "tlp_IgnSrc", None, "lb_IgnoredCol_SRC", (3, 0, 3, 0)), 3, 0),
    (column("groupBox2", "tlp_IgnTrg", None, "lb_IgnoredCol_TRG", (3, 0, 3, 0)), 4, 0),
    (G("groupBox10", T("tlp_Status", cols=["100%"], rows=["35%", "45%", "20%"], cells=[
        (C("lblStatus", dock="Fill", margin=0), 0, 0),
        (C("lblExpressionStatus", dock="Fill", margin=0), 0, 1),
        (C("lblCal", dock="Fill", margin=0), 0, 2),
    ]), margin=(3, 0, 0, 0)), 5, 0),
])

PROGRESS = T("tlp_Progress", cols=["100%", "auto", "auto", "auto"], rows=["auto"], autosize=True, cells=[
    (C("progressBar1", anchor="Left, Right", size=(300, 14), margin=(0, 4, 16, 4)), 0, 0),
    (C("lbl_PercentageShow", autosize=True, anchor="Left", margin=(0, 0, 24, 0)), 1, 0),
    (C("label1", autosize=True, anchor="Left", margin=(0, 0, 6, 0)), 2, 0),
    (C("lblTimer", autosize=True, anchor="Left", minsize=(80, 0), margin=0), 3, 0),
])

LAYOUT = T("tlp_Root", cols=["100%"], rows=["auto", "100%", "auto"], cells=[
    (T("tlp_Top", cols=["100%", "320"], rows=["auto"], autosize=True, dock="Fill", cells=[
        (G("groupBox14", T("tlp_Connection", cols=["100%"], rows=["auto", "auto"], autosize=True, cells=[
            (C("connection", new="SPI_Data_Comp_Tool.ConnectionPanel", anchor="Left, Right", autosize=True, margin=0), 0, 0),
            (TABLES, 0, 1),
        ]), autosize=True, margin=(0, 0, 12, 0)), 0, 0),
        (G("groupBox3", COMPARE, margin=0), 1, 0),
    ]), 0, 0),
    (G("groupBox9", VIEW, margin=(0, 12, 0, 0)), 0, 1),
    (G("groupBox11", PROGRESS, autosize=True, margin=(0, 12, 0, 0)), 0, 2),
])
