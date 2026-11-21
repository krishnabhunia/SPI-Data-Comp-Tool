"""Spec note creation - controlled steps: workbook, create notes, export."""
from layout_gen import C, T, G

DESIGNER = "SPI_Spec_Note_Creation_Utility.Designer.cs"
FORM_NAME = "SPI_Spec_Note_Creation_Utility"
TITLE = "Spec note creation"
CLIENT = (980, 640)
MINIMUM = (820, 540)

TEXTS = {
    "gb_ControlAndStatus": "Notes", "btn_Spec_Create": "Create and show", "btn_GenerateExcel": "Export to Excel",
    "lbl_Status": "Status: Okay", "gb_ViewExcelData": "View data",
}

CONTROLS = T("tlp_Controls", cols=["auto", "auto", "100%"], rows=["auto"], autosize=True, cells=[
    (C("btn_Spec_Create", size=(150, 32), margin=(0, 0, 8, 0)), 0, 0),
    (C("btn_GenerateExcel", size=(150, 32), margin=(0, 0, 12, 0)), 1, 0),
    (C("lbl_Status", autosize=True, anchor="Left", margin=0), 2, 0),
])

LAYOUT = T("tlp_Root", cols=["100%"], rows=["auto", "auto", "auto", "100%"], cells=[
    (C("step_Strip", new="SPI_Data_Comp_Tool.StepStrip", dock="Fill", size=(800, 66), margin=(0, 0, 0, 12)), 0, 0),
    (C("excelFileReadCustomControl1", dock="Fill", size=(776, 102), margin=0), 0, 1),
    (G("gb_ControlAndStatus", CONTROLS, autosize=True, margin=(0, 12, 0, 0)), 0, 2),
    (G("gb_ViewExcelData", T("tlp_View", cols=["100%"], rows=["100%"], cells=[
        (C("dataGridView1", dock="Fill", margin=0), 0, 0),
    ]), margin=(0, 12, 0, 0)), 0, 3),
])
