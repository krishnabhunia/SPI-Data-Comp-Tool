"""Rewrite a WinForms *.Designer.cs from a layout spec (tools/layouts/<Form>.py).

Every existing control keeps its field name, events and properties (Text, Enabled, Items ...);
only position/size/parent are regenerated. Containers are laid out with TableLayoutPanel /
FlowLayoutPanel so controls spread evenly and grow with the window.

usage: python tools/layout_gen.py tools/layouts/<Form>.py      (rewrites the designer file in place)
       python tools/layout_gen.py --check tools/layouts/*.py    (fails if a designer file is out of date)
"""
import importlib.util
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"
WF = "System.Windows.Forms."

# ---------------- spec helpers (imported by layout files) ----------------


class Node:
    def __init__(self, name, kind, **o):
        self.name, self.kind, self.o = name, kind, o
        self.children = []  # list of (node, cell dict)


def C(name, **o):
    """An existing control (or a new one when new=Type)."""
    return Node(name, "ctrl", **o)


def T(name, cols, rows, cells, **o):
    """New TableLayoutPanel. cols/rows: list like ['120', '50%', 'auto']; cells: [(node, col, row, span?)]."""
    n = Node(name, "table", cols=cols, rows=rows, **o)
    for cell in cells:
        node, col, row = cell[0], cell[1], cell[2]
        span = cell[3] if len(cell) > 3 else 1
        rspan = cell[4] if len(cell) > 4 else 1
        n.children.append((node, {"col": col, "row": row, "span": span, "rspan": rspan}))
    return n


def F(name, items, direction="LeftToRight", **o):
    """New FlowLayoutPanel holding items in order."""
    n = Node(name, "flow", direction=direction, **o)
    n.children = [(i, {}) for i in items]
    return n


def G(name, child, **o):
    """Existing GroupBox/Panel holding one child laid out to fill it."""
    n = Node(name, "group", **o)
    n.children = [(child, {})]
    return n


# ---------------- designer parsing ----------------

STMT_THIS_PROP = re.compile(r"^this\.(\w+)\.(\w+)")
STMT_FORM_PROP = re.compile(r"^this\.(\w+)\s*(=|\.)")
NEW_RE = re.compile(r"^this\.(\w+)\s*=\s*new\s+([\w\.]+)\((.*)\);$", re.S)


def split_statements(body):
    stmts, buf, depth = [], [], 0
    for line in body.splitlines():
        s = line.strip()
        if not buf and (not s or s.startswith("//")):
            continue
        buf.append(line.rstrip())
        depth += s.count("(") + s.count("{") - s.count(")") - s.count("}")
        if depth <= 0 and s.endswith(";"):
            stmts.append("\n".join(buf))
            buf, depth = [], 0
    if buf:
        stmts.append("\n".join(buf))
    return stmts


def parse(path):
    src = path.read_text(encoding="utf-8-sig")
    m = re.search(r"(private void InitializeComponent\(\)\s*\{)(.*?)(\n        \}\n)", src, re.S)
    if not m:
        raise SystemExit(f"InitializeComponent not found in {path}")
    head, body, tail = src[: m.end(1)], m.group(2), src[m.start(3):]
    fields = re.findall(r"^\s*private ([\w\.]+) (\w+);\s*$", tail, re.M)
    return src, head, body, tail, fields


# ---------------- generation ----------------

LAYOUT_PROPS = {"Location", "Size", "TabIndex", "Anchor", "Dock", "Margin", "Font", "MaximumSize", "MinimumSize"}
CONTAINER_PROPS = LAYOUT_PROPS | {"Padding", "AutoSize", "AutoSizeMode"}
FORM_DROP = {"ClientSize", "AutoScroll", "Font", "AutoScaleDimensions", "AutoScaleMode", "MinimumSize",
             "MaximumSize", "StartPosition", "AutoScrollMinSize", "Padding"}


def qualify(tp):
    """WinForms type names get their namespace; our own controls (new="SPI_Data_Comp_Tool.X") stay as given."""
    return tp if "." in tp else WF + tp


def cs(text):
    return text.replace("\\", "\\\\").replace('"', '\\"')


def style(spec, kind):
    s = str(spec).strip().lower()
    if s == "auto":
        return f"new {WF}{kind}Style({WF}SizeType.AutoSize)"
    if s.endswith("%"):
        return f"new {WF}{kind}Style({WF}SizeType.Percent, {float(s[:-1]):g}F)"
    return f"new {WF}{kind}Style({WF}SizeType.Absolute, {float(s):g}F)"


def anchor_expr(a):
    parts = [p.strip() for p in a.split(",")]
    return "((" + WF + "AnchorStyles)(" + " | ".join(f"{WF}AnchorStyles.{p}" for p in parts) + "))"


def pad(v):
    if isinstance(v, int):
        v = (v, v, v, v)
    return f"new {WF}Padding({', '.join(str(x) for x in v)})"


def generate(spec_path):
    spec = load_spec(spec_path)
    designer = PROJ / spec.DESIGNER
    src, head, body, tail, fields = parse(designer)
    field_types = {n: t for t, n in fields}
    stmts = split_statements(body)
    keep_frozen = set(getattr(spec, "FROZEN", []))
    drop = set(getattr(spec, "DROP", []))
    texts = getattr(spec, "TEXTS", {})
    extra = getattr(spec, "PROPS", {})

    # collect layout tree
    order, info = [], {}

    def walk(node, parent, cell):
        info[node.name] = (node, parent, cell)
        order.append(node.name)
        for ch, c in node.children:
            walk(ch, node, c)

    walk(spec.LAYOUT, None, {})
    laid = set(order)

    # frozen subtrees: everything under a frozen control keeps its statements untouched
    parent_of = {}
    for s in stmts:
        mm = re.match(r"^this\.(\w+)\.Controls\.Add\(this\.(\w+)", s.strip())
        if mm:
            parent_of[mm.group(2)] = mm.group(1)

    def frozen(n):
        while n:
            if n in keep_frozen:
                return True
            n = parent_of.get(n)
        return False

    # panels/labels that no longer appear in the spec are removed
    stale = [n for n, tp in field_types.items() if n not in laid and n not in drop and not frozen(n)
             and tp.split(".")[-1] in ("TableLayoutPanel", "FlowLayoutPanel") and n.startswith(("tlp_", "flp_"))]
    drop |= set(stale)
    managed = [n for n in order if info[n][0].kind in ("table", "flow") or info[n][0].o.get("new")]
    new_nodes = [n for n in managed if n not in field_types]  # created on an earlier run are reused
    for n in order:
        nd = info[n][0]
        if nd.kind in ("ctrl", "group") and not nd.o.get("new") and n not in field_types:
            raise SystemExit(f"{spec.DESIGNER}: unknown control in spec: {n}")

    pre, inst, init_begin, per_ctrl, form_stmts, other = [], {}, [], {}, [], []
    for s in stmts:
        t = s.strip()
        mn = NEW_RE.match(t)
        if mn and mn.group(1) in field_types or (mn and mn.group(1) == "components"):
            inst[mn.group(1)] = s
            continue
        if "ISupportInitialize" in t:
            nm = re.search(r"\(this\.(\w+)\)\)\.(BeginInit|EndInit)", t)
            if nm and nm.group(2) == "BeginInit":
                init_begin.append(nm.group(1))
            continue
        if re.match(r"^this\.(\w+\.)?(SuspendLayout|ResumeLayout|PerformLayout)\(", t):
            continue
        mp = STMT_THIS_PROP.match(t)
        if mp and mp.group(1) in field_types:
            per_ctrl.setdefault(mp.group(1), []).append(s)
            continue
        mf = STMT_FORM_PROP.match(t)
        if mf:
            form_stmts.append(s)
            continue
        pre.append(s)

    orphan = [n for n, tp in field_types.items()
              if tp.split(".")[-1] in VISUAL and n not in laid and n not in drop and not frozen(n)]
    if orphan:
        raise SystemExit(f"{spec.DESIGNER}: controls not placed in the spec: {', '.join(sorted(orphan))}")

    def mentions_dropped(stmt):
        return any(re.search(rf"this\.{re.escape(d)}\b", stmt) for d in drop)

    form_stmts = [x for x in form_stmts if not mentions_dropped(x)]
    if getattr(spec, "TITLE", None) is not None:  # window title bar text
        form_stmts = [x for x in form_stmts if not re.match(r"^\s*this\.Text = ", x)]
        form_stmts.append(f'            this.Text = "{cs(spec.TITLE)}";')
    for k in list(per_ctrl):
        per_ctrl[k] = [x for x in per_ctrl[k] if not (mentions_dropped(x) and not x.strip().startswith(f"this.{k}."))
                       or not mentions_dropped(x.replace(f"this.{k}.", "", 1))]
    pre = [x for x in pre if not mentions_dropped(x)]

    I = "            "
    out = []
    for s in pre:
        out.append(s)
    # instantiations: existing kept controls in original order, then new ones
    for n, s in inst.items():
        if n in drop or n in managed:
            continue
        out.append(s)
    for n in managed:
        nd = info[n][0]
        tp = {"table": "TableLayoutPanel", "flow": "FlowLayoutPanel"}.get(nd.kind) or nd.o["new"]
        out.append(f"{I}this.{n} = new {qualify(tp)}();")
    containers = [n for n in order if info[n][0].kind in ("table", "flow", "group")]
    frozen_containers = [n for n, tp in field_types.items() if frozen(n) and tp.split(".")[-1] in CONTAINERS]
    for n in init_begin:
        if n not in drop:
            out.append(f"{I}((System.ComponentModel.ISupportInitialize)(this.{n})).BeginInit();")
    for n in containers + frozen_containers:
        out.append(f"{I}this.{n}.SuspendLayout();")
    out.append(f"{I}this.SuspendLayout();")

    tab = [0]

    def block(n, lines):
        out.append(f"{I}// \n{I}// {n}\n{I}// ")
        out.extend(lines)

    def gen_for(n):
        nd, parent, cell = info[n]
        o = nd.o
        lines = []
        own = [] if n in managed else per_ctrl.get(n, [])  # spec-made panels/labels are rebuilt from scratch
        for s in own:
            prop = STMT_THIS_PROP.match(s.strip()).group(2)
            if prop == "Controls":
                continue
            if prop in (CONTAINER_PROPS if nd.kind == "group" else LAYOUT_PROPS):
                continue
            if prop == "Text" and n in texts:
                continue
            if prop in extra.get(n, {}):
                continue
            if prop == "AutoSize" and "autosize" in o:
                continue
            lines.append(s)
        if nd.kind in ("table", "flow") or o.get("new"):
            lines.append(f'{I}this.{n}.Name = "{n}";')
            if o.get("new") == "Label" and n not in texts:
                lines.append(f'{I}this.{n}.Text = "{cs(o.get("text", ""))}";')
        if n in texts:
            lines.append(f'{I}this.{n}.Text = "{cs(texts[n])}";')
        if nd.kind == "table":
            lines.append(f"{I}this.{n}.ColumnCount = {len(o['cols'])};")
            for c in o["cols"]:
                lines.append(f"{I}this.{n}.ColumnStyles.Add({style(c, 'Column')});")
            lines.append(f"{I}this.{n}.RowCount = {len(o['rows'])};")
            for r in o["rows"]:
                lines.append(f"{I}this.{n}.RowStyles.Add({style(r, 'Row')});")
            for ch, c in nd.children:
                lines.append(f"{I}this.{n}.Controls.Add(this.{ch.name}, {c['col']}, {c['row']});")
                if c["span"] > 1:
                    lines.append(f"{I}this.{n}.SetColumnSpan(this.{ch.name}, {c['span']});")
                if c["rspan"] > 1:
                    lines.append(f"{I}this.{n}.SetRowSpan(this.{ch.name}, {c['rspan']});")
        elif nd.kind in ("flow", "group"):
            if nd.kind == "flow":
                lines.append(f"{I}this.{n}.FlowDirection = {WF}FlowDirection.{o['direction']};")
                lines.append(f"{I}this.{n}.WrapContents = {'true' if o.get('wrap') else 'false'};")
            for ch, _ in nd.children:
                lines.append(f"{I}this.{n}.Controls.Add(this.{ch.name});")
        # layout properties
        default_dock = "Fill" if nd.kind in ("table", "group") and parent is not None else None
        if parent is None:
            default_dock = "Fill"
        dock, anchor = o.get("dock", default_dock), o.get("anchor")
        if nd.kind == "flow" and "dock" not in o and "anchor" not in o:
            anchor = "Left"
        if anchor:
            lines.append(f"{I}this.{n}.Anchor = {anchor_expr(anchor)};")
        elif dock:
            lines.append(f"{I}this.{n}.Dock = {WF}DockStyle.{dock};")
        if nd.kind == "flow":
            autosize = o.get("autosize", True)
        elif nd.kind == "table":
            autosize = o.get("autosize", False)
        else:
            autosize = o.get("autosize")
        if autosize is not None:
            lines.append(f"{I}this.{n}.AutoSize = {'true' if autosize else 'false'};")
            if autosize and nd.kind in ("table", "flow", "group"):
                lines.append(f"{I}this.{n}.AutoSizeMode = {WF}AutoSizeMode.GrowAndShrink;")
        if "margin" in o:
            lines.append(f"{I}this.{n}.Margin = {pad(o['margin'])};")
        elif nd.kind in ("table", "flow") and parent is not None and parent.kind != "group":
            lines.append(f"{I}this.{n}.Margin = {pad(0)};")
        if "padding" in o:
            lines.append(f"{I}this.{n}.Padding = {pad(o['padding'])};")
        elif nd.kind == "group":
            lines.append(f"{I}this.{n}.Padding = {pad((10, 6, 10, 10))};")
        if "size" in o:
            w, h = o["size"]
            lines.append(f"{I}this.{n}.Size = new System.Drawing.Size({w}, {h});")
        if "minsize" in o:
            w, h = o["minsize"]
            lines.append(f"{I}this.{n}.MinimumSize = new System.Drawing.Size({w}, {h});")
        for k, v in extra.get(n, {}).items():
            lines.append(f"{I}this.{n}.{k} = {v};")
        if nd.kind == "ctrl" or nd.kind == "group":
            lines.append(f"{I}this.{n}.TabIndex = {tab[0]};")
            tab[0] += 1
        elif nd.kind in ("table", "flow"):
            lines.append(f"{I}this.{n}.TabIndex = {tab[0]};")
            tab[0] += 1
        block(n, lines)

    for n in order:
        gen_for(n)
    # frozen and non-visual components: original statements unchanged
    emitted = set(order) | drop
    for n in field_types:
        if n in emitted:
            continue
        if n in per_ctrl:
            block(n, per_ctrl[n])
    # form
    form_lines = []
    for s in form_stmts:
        mf = STMT_FORM_PROP.match(s.strip())
        if mf.group(1) in FORM_DROP or s.strip().startswith("this.Controls.Add("):
            continue
        form_lines.append(s)
    cw, ch = spec.CLIENT
    mw, mh = spec.MINIMUM
    form_lines += [
        f"{I}this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);",
        f"{I}this.AutoScaleMode = {WF}AutoScaleMode.Font;",
        f"{I}this.ClientSize = new System.Drawing.Size({cw}, {ch});",
        f"{I}this.Font = new System.Drawing.Font(\"Segoe UI\", 9F);",
        f"{I}this.MinimumSize = new System.Drawing.Size({mw}, {mh});",
        f"{I}this.StartPosition = {WF}FormStartPosition.CenterScreen;",
        f"{I}this.Padding = {pad(getattr(spec, 'FORM_PADDING', 12))};",
        f"{I}this.Controls.Add(this.{spec.LAYOUT.name});",
    ]
    for n in getattr(spec, "TOP", []):
        form_lines.append(f"{I}this.Controls.Add(this.{n});")
    for n in getattr(spec, "FROZEN", []):
        form_lines.append(f"{I}this.Controls.Add(this.{n});")
    block(spec.FORM_NAME, form_lines)
    for n in init_begin:
        if n not in drop:
            out.append(f"{I}((System.ComponentModel.ISupportInitialize)(this.{n})).EndInit();")
    for n in containers + frozen_containers:
        out.append(f"{I}this.{n}.ResumeLayout(false);")
        out.append(f"{I}this.{n}.PerformLayout();")
    out.append(f"{I}this.ResumeLayout(false);")
    out.append(f"{I}this.PerformLayout();")
    out.append("")

    new_body = "\n" + "\n".join(out)
    # fields: drop removed, add new
    new_tail = tail
    for n in drop:
        new_tail = re.sub(rf"^\s*private [\w\.]+ {n};\s*\n", "", new_tail, flags=re.M)
    decl = "".join(f"        private {qualify({'table': 'TableLayoutPanel', 'flow': 'FlowLayoutPanel'}.get(info[n][0].kind) or info[n][0].o['new'])} {n};\n"
                   for n in new_nodes)
    if decl and "#endregion" in new_tail:
        new_tail = new_tail.replace("        #endregion\n", "        #endregion\n\n" + decl, 1)
    text = head + new_body + new_tail
    return designer, text


VISUAL = {"Button", "TextBox", "Label", "ComboBox", "CheckBox", "RadioButton", "GroupBox", "Panel", "DataGridView",
          "RichTextBox", "ListBox", "CheckedListBox", "TabControl", "TabPage", "ProgressBar", "LinkLabel", "PictureBox",
          "NumericUpDown", "DateTimePicker", "TreeView", "ListView", "MaskedTextBox", "SplitContainer"}
CONTAINERS = {"GroupBox", "Panel", "TabPage", "TabControl", "SplitContainer", "TableLayoutPanel", "FlowLayoutPanel"}


def top_level(form_stmts):
    return {re.search(r"this\.(\w+)\)", s).group(1) for s in form_stmts if s.strip().startswith("this.Controls.Add(")}


def load_spec(path):
    s = importlib.util.spec_from_file_location("layout_spec", path)
    mod = importlib.util.module_from_spec(s)
    sys.modules["layout_gen"] = sys.modules[__name__]
    s.loader.exec_module(mod)
    return mod


def main(argv):
    check = "--check" in argv
    paths = [a for a in argv[1:] if not a.startswith("--")]
    bad = 0
    for p in paths:
        designer, text = generate(Path(p))
        current = designer.read_text(encoding="utf-8-sig")
        if check:
            if current.replace("\r\n", "\n") != text.replace("\r\n", "\n"):
                print(f"out of date: {designer.name} (run python tools/layout_gen.py {p})")
                bad += 1
        else:
            designer.write_text(text, encoding="utf-8-sig")
            print(f"wrote {designer.name}")
    return 1 if bad else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
