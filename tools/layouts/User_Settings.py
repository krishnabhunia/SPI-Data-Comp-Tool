"""One settings page for output preferences, comparison rules, explicit updates and About.

No Close button of its own: like every task, it closes with the window's "Close task" button."""
from layout_gen import C, T, F, G, TABS

DESIGNER = "User_Settings.Designer.cs"
FORM_NAME = "User_Settings"
TITLE = "Settings"
CLIENT = (900, 700)
MINIMUM = (760, 600)

TEXTS = {
    "tab_General": "General",
    "tab_Rules": "Comparison rules",
    "tab_Updates": "Updates",
    "tab_About": "About",
    "gb_SaveExcel": "Save Excel files with",
    "gb_Theme": "Colour theme",
    "lbl_Theme": "Theme",
    "lbl_ThemeHelp": "Changes the colours of every open window at once and is remembered for next time.",
    "rb_EPPlus": "EPPlus (recommended, no Excel needed)",
    "rb_XLSave": "Microsoft Excel (needs Excel installed)",
    "lbl_RulesHelp": "Import the rules workbook used by database and Excel comparisons. A successful import is saved immediately.",
    "lbl_RulesPath": "Rules file",
    "btn_ImportRules": "&Import rules…",
    "lbl_RulesStatus": "No comparison rules file selected.",
    "gb_Logs": "Log",
    "btn_ClearLogs": "Clear log",
    "lbl_Progress": "",
}

DROP = ["btn_Close", "flp_SettingsActions"]   # one way to close a task: "Close task" at the bottom of the window

PROPS = {
    "cb_Theme": {"DropDownStyle": "System.Windows.Forms.ComboBoxStyle.DropDownList", "AccessibleName": '"Colour theme"'},
    "swatch_Theme": {"AccessibleName": '"Colours of the chosen theme"',
                     "AccessibleRole": "System.Windows.Forms.AccessibleRole.Graphic"},
    "tb_RulesPath": {"ReadOnly": "true", "AccessibleName": '"Comparison rules file"'},
    "lbl_RulesStatus": {"AutoEllipsis": "true"},
}

LAYOUT = T("tlp_Root", cols=["100%"], rows=["100%", "140"], cells=[
    (TABS("tabs_Settings", new="TabControl", pages=[
        G("tab_General", new="TabPage", child=T("tlp_General", cols=["100%"], rows=["92", "112", "100%"], cells=[
            (G("gb_SaveExcel", T(
                "tlp_SaveExcel", cols=["100%"], rows=["auto", "auto"], autosize=True, cells=[
                    (C("rb_EPPlus", autosize=True, anchor="Left"), 0, 0),
                    (C("rb_XLSave", autosize=True, anchor="Left"), 0, 1),
                ]), margin=(0, 0, 0, 12)), 0, 0),
            # the colour theme of every window (see UiTheme.cs); applied at once, remembered for next time
            (G("gb_Theme", new="GroupBox", child=T(
                "tlp_Theme", cols=["auto", "auto", "100%"], rows=["auto", "auto"], autosize=True, cells=[
                    (C("lbl_Theme", new="Label", autosize=True, anchor="Left", margin=(0, 0, 12, 0)), 0, 0),
                    (C("cb_Theme", new="ComboBox", size=(200, 23), anchor="Left", margin=0), 1, 0),
                    (C("swatch_Theme", new="SPI_Data_Comp_Tool.ThemeSwatch", size=(240, 22), anchor="Left",
                       margin=(12, 0, 0, 0)), 2, 0),
                    (C("lbl_ThemeHelp", new="Label", autosize=True, dock="Fill", margin=(0, 10, 0, 0)), 0, 1, 3),
                ]), margin=0), 0, 1),
        ])),
        G("tab_Rules", new="TabPage", child=T("tlp_Rules", cols=["auto", "100%"],
            rows=["auto", "auto", "auto", "100%"], cells=[
                (C("lbl_RulesHelp", new="Label", autosize=True, dock="Fill", margin=(0, 0, 0, 12)), 0, 0, 2),
                (C("lbl_RulesPath", new="Label", autosize=True, anchor="Left", margin=(0, 0, 12, 0)), 0, 1),
                (C("tb_RulesPath", new="TextBox", dock="Fill", margin=0), 1, 1),
                (C("btn_ImportRules", new="Button", size=(136, 32), anchor="Left", margin=(0, 10, 0, 6)), 0, 2, 2),
                (C("lbl_RulesStatus", new="Label", dock="Fill", margin=0), 0, 3, 2),
            ])),
        G("tab_Updates", new="TabPage", child=C("updatesControl", new="SPI_Data_Comp_Tool.UpdatesControl",
            dock="Fill", margin=0)),
        # the only About page: the main window has no Help menu (AboutBox1 is embedded here at run time)
        G("tab_About", new="TabPage", child=C("pnl_About", new="Panel", dock="Fill", margin=0)),
    ]), 0, 0),
    (G("gb_Logs", T("tlp_Logs", cols=["auto", "100%"], rows=["100%", "auto"], cells=[
        (C("rtb_Log", dock="Fill", margin=(0, 0, 0, 8)), 0, 0, 2),
        (C("btn_ClearLogs", size=(110, 30), anchor="Left", margin=0), 0, 1),
        (C("lbl_Progress", autosize=True, anchor="Left", margin=(12, 0, 0, 0)), 1, 1),
    ])), 0, 1),
])
