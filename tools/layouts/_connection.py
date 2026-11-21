"""Parts shared by every window that uses the Connection panel (imported by the specs in this folder)."""
from layout_gen import C, T

# The connection controls now live inside the shared ConnectionPanel; the window reaches them
# through one-line properties of the same names. The rest are the wrappers and labels it replaces.
PANEL_DROP = [
    "groupBox12", "groupBox13", "gb_Source", "gb_Target", "gb_Source1", "gb_Target1",
    "rb_SRC_oracle", "rb_SRC_SQL", "cb_WindowAuthentication_SRC", "cb_SRC_DefaultPort", "tb_SRC_DefaultPort",
    "tb_SRC_DataSource", "tb_SRC_DBName", "tb_SRC_UserName", "tb_SRC_Password",
    "rb_TRG_Oracle", "rb_TRG_SQL", "cb_WindowAuthentication_TRG", "cb_TRG_DefaultPort", "tb_TRG_DefaultPort",
    "tb_TRG_DataSource", "tb_TRG_DBName", "tb_TRG_UserName", "tb_TRG_Password",
    "btn_Copy_SRC_TRG", "btn_TRG_SRC", "cb_UserProfileList",
    "label2", "label3", "label4", "label5", "label6", "label7", "label8", "label9",
]

LABEL = dict(autosize=True, anchor="Left", margin=(0, 3, 12, 3))
FIELD = dict(anchor="Left, Right", margin=(0, 3, 0, 3))
TEXTS = {"cb_FullName": "Show full names in lists", "label16": "Source table", "label17": "Target table",
         "btn_Load_SRC_Table": "Load tables", "btn_Load_TRG_Table": "Load tables"}


def panel():
    """The Connection panel itself."""
    return C("connection", new="SPI_Data_Comp_Tool.ConnectionPanel", anchor="Left, Right", autosize=True, margin=0)


def picker(name, label, combo):
    """A table picker without its own load button (one button loads both lists)."""
    return T(name, cols=["auto", "100%"], rows=["auto"], autosize=True, cells=[
        (C(label, **LABEL), 0, 0),
        (C(combo, **FIELD), 1, 0),
    ])


def table_picker(name, label, combo, button):
    return T(name, cols=["auto", "100%", "auto"], rows=["auto"], autosize=True, cells=[
        (C(label, **LABEL), 0, 0),
        (C(combo, **FIELD), 1, 0),
        (C(button, size=(110, 30), margin=(8, 0, 0, 0)), 2, 0),
    ])


def under_halves(name, left=None, right=None, rows=None):
    """A row under the panel whose two cells line up with its Source and Target halves
    (the panel's Copy column is 100 px). `rows` adds more (left, right) rows."""
    all_rows = [(left, right)] + list(rows or [])
    cells = []
    for r, (lft, rgt) in enumerate(all_rows):
        if lft is not None:
            cells.append((lft, 0, r))
        if rgt is not None:
            cells.append((rgt, 2, r))
    return T(name, cols=["50%", "100", "50%"], rows=["auto"] * len(all_rows), autosize=True, cells=cells,
             margin=(0, 8, 0, 0))


def connection_box(group, below=()):
    """The Connection group: the panel, then the window's own rows under it."""
    rows = [(panel(), 0, 0)] + [(node, 0, i + 1) for i, node in enumerate(below)]
    from layout_gen import G
    return G(group, T("tlp_Connection", cols=["100%"], rows=["auto"] * len(rows), autosize=True, cells=rows),
             autosize=True, margin=(0, 0, 12, 0))
