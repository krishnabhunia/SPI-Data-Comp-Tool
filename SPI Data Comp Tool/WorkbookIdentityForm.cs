using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// The "Workbook identity check" task: are two workbooks identical? It shows the answer, the 18 checks and
    /// every difference, and writes the five-sheet report. All reading and comparing is done by
    /// <see cref="WorkbookIdentityEngine"/> on a background worker; this class only collects the inputs and
    /// shows the result. The other comparison tasks are not used or changed.
    /// </summary>
    public sealed class WorkbookIdentityForm : WorkspacePageForm
    {
        /// <summary>Rows shown per result tab. The report file always holds every row.</summary>
        internal const int PreviewRows = 1000;
        internal const string AllSheets = "All sheets (paired by name)";
        private const string DefaultReportName = "Workbook_Identity_Report.xlsx";

        private readonly BackgroundWorker worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        private readonly System.Windows.Forms.Timer elapsedTimer = new System.Windows.Forms.Timer { Interval = 500 };
        private readonly Stopwatch elapsed = new Stopwatch();

        private readonly TextBox fileA = Field("tb_IdentityFileA", "Source workbook (File A)");
        private readonly TextBox fileB = Field("tb_IdentityFileB", "Target workbook (File B)");
        private readonly ComboBox sheetA = SheetPicker("cb_IdentitySheetA", "Sheet of the Source workbook");
        private readonly ComboBox sheetB = SheetPicker("cb_IdentitySheetB", "Sheet of the Target workbook");
        private readonly Label loadedA = Note("Not loaded yet. Choose a workbook with Browse.", "lbl_IdentityLoadedA");
        private readonly Label loadedB = Note("Not loaded yet. Choose a workbook with Browse.", "lbl_IdentityLoadedB");
        private readonly RadioButton byPosition = new RadioButton
        {
            Name = "rb_IdentityPosition", Text = "By row position (row N of File A against row N of File B)", Checked = true
        };
        private readonly RadioButton byKey = new RadioButton { Name = "rb_IdentityKey", Text = "By primary key" };
        private readonly CheckedListBox keysA = KeyList("clb_IdentityKeysA", "Key columns in Source");
        private readonly CheckedListBox keysB = KeyList("clb_IdentityKeysB", "Key columns in Target");
        private readonly NumericUpDown headerRow = new NumericUpDown
        {
            Name = "nud_IdentityHeaderRow", Minimum = 1, Maximum = 1048576, Value = 1, Dock = DockStyle.Fill, Margin = new Padding(3, 5, 3, 3),
            AccessibleName = "Header row"
        };
        private readonly TextBox tolerance = Field("tb_IdentityTolerance", "Number tolerance");
        private readonly TextBox thresholds = Field("tb_IdentityThresholds", "Difference thresholds");
        private readonly CheckBox formats = Check("chk_IdentityFormats", "Compare cell formatting (font, alignment, number format, fill, borders)");
        private readonly CheckBox layout = Check("chk_IdentityLayout", "Compare sheet layout (widths, merged cells, freeze panes, hidden rows and columns)");
        private readonly CheckBox formulas = Check("chk_IdentityFormulas", "Compare formulas");
        private readonly TextBox output = Field("tb_IdentityOutput", "Report file");
        private readonly CheckBox openWhenDone = Check("chk_IdentityOpen", "Open the report when it is finished");
        private readonly Button check = ActionButton("btn_IdentityCheck", "Check inputs");
        private readonly Button compare = ActionButton("btn_IdentityCompare", "Compare and create report");
        private readonly Button cancel = ActionButton("btn_IdentityCancel", "Cancel");
        private readonly Button openReport = ActionButton("btn_IdentityOpenReport", "Open report");
        private readonly Button openFolder = ActionButton("btn_IdentityOpenFolder", "Open folder");
        private readonly Label elapsedLabel = Note("Elapsed 00:00:00", "lbl_IdentityElapsed");
        private readonly Label status = Note("Status: ready. Nothing has been started.", "lbl_IdentityStatus");
        private readonly ProgressBar progress = new ProgressBar { Name = "pb_IdentityProgress", Dock = DockStyle.Fill, Height = 12 };
        private readonly RichTextBox log = new RichTextBox
        {
            Name = "rtb_Log", Dock = DockStyle.Fill, ReadOnly = true, DetectUrls = false, BorderStyle = BorderStyle.FixedSingle, AccessibleName = "Log"
        };
        private readonly Panel scroll = new Panel { Name = "pnl_IdentityScroll", Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(6, 0, 6, 0) };
        private readonly GroupBox resultCard = new GroupBox
        {
            Name = "gb_IdentityResult", Text = "Result", Dock = DockStyle.Fill, Padding = new Padding(12, 10, 12, 8), Margin = new Padding(4), Visible = false
        };
        private readonly Label verdict = Note(string.Empty, "lbl_IdentityVerdict");
        private readonly Label matching = Note(string.Empty, "lbl_IdentityMatching");
        private readonly Label staleNote = Note("The inputs were changed after this comparison. The result below belongs to the earlier inputs.", "lbl_IdentityStale");
        private readonly Label reportLabel = Note(string.Empty, "lbl_IdentityReport");
        private readonly Label previewNote = Note(string.Empty, "lbl_IdentityPreview");
        private readonly TabControl tabs = new TabControl { Name = "tabs_IdentityResult", Dock = DockStyle.Fill };
        private readonly List<DataGridView> grids = new List<DataGridView>();
        private readonly Dictionary<DataGridView, int[]> statusColumns = new Dictionary<DataGridView, int[]>();
        private Font statusFont;

        private LoadedFile loadedFileA, loadedFileB;
        private IdentityResult result;
        private IDisposable inputLock;
        private Job currentJob;
        private bool suppressChanges;
        private string reportPath;

        public WorkbookIdentityForm()
        {
            Name = "WorkbookIdentityForm";
            Text = "Workbook identity check";
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 9F);
            ClientSize = new Size(1120, 790);
            MinimumSize = new Size(820, 600);
            StartPosition = FormStartPosition.CenterParent;
            tolerance.Text = "0";
            thresholds.Text = "0, 0.01, 0.1, 1";
            output.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DefaultReportName);
            statusFont = new Font(Font, FontStyle.Bold);
            BuildLayout();
            HookActions();
            UiTheme.Apply(this, compare);
            RefreshAvailability();
        }

        /// <summary>True while a workbook is being read or a comparison is running.</summary>
        internal bool IsWorking { get { return worker.IsBusy || currentJob != null; } }

        /// <summary>The last finished comparison, or null.</summary>
        internal IdentityResult Result { get { return result; } }

        // ------------------------------------------------------------------ small control factories
        private static TextBox Field(string name, string accessibleName)
        {
            return new TextBox { Name = name, Dock = DockStyle.Fill, Margin = new Padding(3, 5, 3, 3), AccessibleName = accessibleName };
        }

        private static ComboBox SheetPicker(string name, string accessibleName)
        {
            var picker = new ComboBox { Name = name, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, AccessibleName = accessibleName };
            picker.Items.Add(AllSheets);
            picker.SelectedIndex = 0;
            return picker;
        }

        private static CheckedListBox KeyList(string name, string accessibleName)
        {
            return new CheckedListBox { Name = name, Dock = DockStyle.Fill, CheckOnClick = true, IntegralHeight = false, Enabled = false, AccessibleName = accessibleName };
        }

        private static Button ActionButton(string name, string caption)
        {
            return new Button { Name = name, Text = caption, AutoSize = true, MinimumSize = new Size(84, 30), Margin = new Padding(4) };
        }

        /// <summary>A label that fills its cell. Text that does not fit ends in "..."; the full text stays in the tooltip of the label.</summary>
        private static Label Note(string text, string name = null, bool wrap = false)
        {
            var note = new Label
            {
                Text = text, AutoSize = false, Dock = DockStyle.Fill, Margin = new Padding(3, 0, 3, 0), AutoEllipsis = true,
                UseMnemonic = false,      // "Layout & Other" is a sheet name, not a shortcut key
                TextAlign = wrap ? ContentAlignment.TopLeft : ContentAlignment.MiddleLeft
            };
            if (name != null) note.Name = name;
            return note;
        }

        private static CheckBox Check(string name, string caption)
        {
            // Not auto-sized: a long caption wraps onto a second line when the window is narrow.
            return new CheckBox { Name = name, Text = caption, Checked = true, AutoSize = false, Dock = DockStyle.Fill, Margin = new Padding(4, 0, 4, 0) };
        }

        /// <summary>One column of rows with fixed heights. A last, empty row takes whatever height is left over.</summary>
        private static TableLayoutPanel Stack(params int[] heights)
        {
            var table = new TableLayoutPanel { ColumnCount = 1, RowCount = heights.Length + 1, Dock = DockStyle.Fill, Margin = new Padding(0) };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (int height in heights) table.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            return table;
        }

        /// <summary>One row of two columns that share the width in the given proportion.</summary>
        private static TableLayoutPanel Pair(float left, float right, Control first, Control second)
        {
            var table = new TableLayoutPanel { ColumnCount = 2, RowCount = 1, Dock = DockStyle.Fill, Margin = new Padding(0) };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, left));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, right));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            table.Controls.Add(first, 0, 0);
            table.Controls.Add(second, 1, 0);
            return table;
        }

        /// <summary>A path field with its button at a fixed width on the right.</summary>
        private static TableLayoutPanel PathRow(TextBox field, Button button)
        {
            var table = new TableLayoutPanel { ColumnCount = 2, RowCount = 1, Dock = DockStyle.Fill, Margin = new Padding(0) };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 104));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            button.AutoSize = false;
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(4, 3, 3, 3);
            table.Controls.Add(field, 0, 0);
            table.Controls.Add(button, 1, 0);
            return table;
        }

        private static GroupBox Card(string name, string caption, Control content)
        {
            var card = new GroupBox { Name = name, Text = caption, Dock = DockStyle.Fill, Padding = new Padding(12, 10, 12, 8), Margin = new Padding(4) };
            card.Controls.Add(content);
            return card;
        }

        // ------------------------------------------------------------------ layout
        // Every row has a fixed height, so the page looks the same whatever the window size; only the widths follow the window.
        private const int FilesHeight = 176, RulesHeight = 272, OutputHeight = 148, ResultHeight = 536, StaleHeight = 22;
        private TableLayoutPanel setup, resultRows;

        private void BuildLayout()
        {
            // No heading band of its own: the workspace window already shows the task name and what it does.
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 5, ColumnCount = 1, Padding = new Padding(10) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 14));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

            setup = new TableLayoutPanel { Name = "tlp_IdentitySetup", ColumnCount = 1, RowCount = 4, Dock = DockStyle.Top, Margin = new Padding(0) };
            setup.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (int height in new[] { FilesHeight, RulesHeight, OutputHeight, 0 }) setup.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            setup.Controls.Add(Pair(50, 50, FileCard(true), FileCard(false)), 0, 0);
            setup.Controls.Add(Pair(55, 45, MatchingCard(), OptionsCard()), 0, 1);
            setup.Controls.Add(OutputCard(), 0, 2);
            BuildResult();
            setup.Controls.Add(resultCard, 0, 3);
            setup.Height = FilesHeight + RulesHeight + OutputHeight;
            scroll.Controls.Add(setup);
            root.Controls.Add(scroll, 0, 0);

            var actions = new FlowLayoutPanel { Name = "flp_IdentityActions", Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0) };
            elapsedLabel.Dock = DockStyle.None;
            elapsedLabel.AutoSize = true;
            elapsedLabel.Margin = new Padding(12, 12, 4, 4);
            actions.Controls.AddRange(new Control[] { check, compare, cancel, elapsedLabel });
            root.Controls.Add(actions, 0, 1);
            root.Controls.Add(status, 0, 2);
            root.Controls.Add(progress, 0, 3);
            root.Controls.Add(log, 0, 4);
            Controls.Add(root);
        }

        private GroupBox FileCard(bool source)
        {
            TableLayoutPanel rows = Stack(22, 34, 22, 30, 26);
            Button browse = ActionButton(source ? "btn_IdentityBrowseA" : "btn_IdentityBrowseB", "Browse...");
            browse.Click += (sender, args) => BrowseInput(source);
            rows.Controls.Add(Note("Excel file (.xlsx or .xlsm)"), 0, 0);
            rows.Controls.Add(PathRow(source ? fileA : fileB, browse), 0, 1);
            rows.Controls.Add(Note("Sheet"), 0, 2);
            rows.Controls.Add(source ? sheetA : sheetB, 0, 3);
            rows.Controls.Add(source ? loadedA : loadedB, 0, 4);
            GroupBox card = Card(source ? "gb_IdentitySource" : "gb_IdentityTarget", source ? "Source workbook (File A)" : "Target workbook (File B)", rows);
            UiTheme.MarkSide(card, source);
            return card;
        }

        private GroupBox MatchingCard()
        {
            TableLayoutPanel rows = Stack(24, 24, 22, 100, 58);
            byPosition.AutoSize = byKey.AutoSize = false;
            byPosition.Dock = byKey.Dock = DockStyle.Fill;
            byPosition.Margin = byKey.Margin = new Padding(4, 0, 4, 0);
            rows.Controls.Add(byPosition, 0, 0);
            rows.Controls.Add(byKey, 0, 1);
            rows.Controls.Add(Pair(50, 50, Note("Key column(s) in Source"), Note("Key column(s) in Target")), 0, 2);
            rows.Controls.Add(Pair(50, 50, keysA, keysB), 0, 3);
            rows.Controls.Add(Note("Tick one column for a single key, or several for a combined key. Parts pair in the order ticked: part 1 with part 1, " +
                "part 2 with part 2. The two files may use different column names. Names are matched ignoring case.", "lbl_IdentityKeyHelp", true), 0, 4);
            return Card("gb_IdentityMatching", "Row matching", rows);
        }

        private GroupBox OptionsCard()
        {
            TableLayoutPanel rows = Stack(32, 32, 32, 38, 38, 26);
            rows.Controls.Add(Pair(45, 55, Note("Header row"), headerRow), 0, 0);
            rows.Controls.Add(Pair(45, 55, Note("Number tolerance"), tolerance), 0, 1);
            rows.Controls.Add(Pair(45, 55, Note("Difference thresholds"), thresholds), 0, 2);
            rows.Controls.Add(formats, 0, 3);
            rows.Controls.Add(layout, 0, 4);
            rows.Controls.Add(formulas, 0, 5);
            return Card("gb_IdentityOptions", "Options", rows);
        }

        private GroupBox OutputCard()
        {
            TableLayoutPanel rows = Stack(22, 34, 28, 22);
            Button saveAs = ActionButton("btn_IdentitySaveAs", "Save as...");
            saveAs.Click += (sender, args) => BrowseOutput();
            rows.Controls.Add(Note("Report file"), 0, 0);
            rows.Controls.Add(PathRow(output, saveAs), 0, 1);
            rows.Controls.Add(openWhenDone, 0, 2);
            rows.Controls.Add(Note("The report has five sheets: Summary, Headers, Data Differences, Format Differences, Layout & Other. " +
                "An existing file is replaced only after you confirm.", "lbl_IdentityOutputHelp"), 0, 3);
            return Card("gb_IdentityOutput", "Output", rows);
        }

        private void BuildResult()
        {
            resultRows = Stack(34, 40, 0, 40, 348, 24);
            TableLayoutPanel rows = resultRows;
            verdict.Font = new Font(Font.FontFamily, 13F, FontStyle.Bold);
            matching.TextAlign = ContentAlignment.TopLeft;
            staleNote.Visible = false;
            var links = new FlowLayoutPanel { Name = "flp_IdentityReport", Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0) };
            reportLabel.Dock = DockStyle.None;
            reportLabel.AutoSize = true;
            reportLabel.Margin = new Padding(8, 12, 4, 4);
            links.Controls.AddRange(new Control[] { openReport, openFolder, reportLabel });
            foreach (string name in IdentityReportWriter.SheetNames)
            {
                var page = new TabPage(TabCaption(name));
                var view = new DataGridView { Name = "dgv_Identity" + new string(name.Where(char.IsLetter).ToArray()) };
                ConfigureGrid(view);
                page.Controls.Add(view);
                tabs.TabPages.Add(page);
                grids.Add(view);
            }
            rows.Controls.Add(verdict, 0, 0);
            rows.Controls.Add(matching, 0, 1);
            rows.Controls.Add(staleNote, 0, 2);
            rows.Controls.Add(links, 0, 3);
            rows.Controls.Add(tabs, 0, 4);
            rows.Controls.Add(previewNote, 0, 5);
            resultCard.Controls.Add(rows);
        }

        /// <summary>Makes room for the result below the inputs, with or without the line that says it is out of date.</summary>
        private void ShowResultCard(bool stale)
        {
            int height = ResultHeight + (stale ? StaleHeight : 0);
            resultRows.RowStyles[2].Height = stale ? StaleHeight : 0;
            staleNote.Visible = stale;
            setup.RowStyles[3].Height = height;
            setup.Height = FilesHeight + RulesHeight + OutputHeight + height;
            resultCard.Visible = true;
        }

        /// <summary>A caption for a tab: the theme draws tab text with shortcut keys, so "&amp;" has to be doubled to be shown.</summary>
        private static string TabCaption(string text)
        {
            return text.Replace("&", "&&");
        }

        private void ConfigureGrid(DataGridView view)
        {
            view.Dock = DockStyle.Fill;
            view.ReadOnly = true;
            view.AllowUserToAddRows = false;
            view.AllowUserToDeleteRows = false;
            view.AllowUserToOrderColumns = false;
            view.AllowUserToResizeRows = false;
            view.RowHeadersVisible = false;
            view.MultiSelect = true;
            view.SelectionMode = DataGridViewSelectionMode.CellSelect;
            view.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            view.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            view.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            view.CellFormatting += (sender, args) =>
            {
                int[] marked;
                if (args.Value == null || !statusColumns.TryGetValue((DataGridView)sender, out marked) || Array.IndexOf(marked, args.ColumnIndex) < 0) return;
                int tone = IdentityReportWriter.Tone(Convert.ToString(args.Value, CultureInfo.InvariantCulture));
                if (tone == 0) return;
                // Asked for at paint time, so the colour follows the theme that is in use now.
                args.CellStyle.ForeColor = UiTheme.Readable(tone > 0 ? Color.DarkGreen : Color.Red);
                args.CellStyle.Font = statusFont;
            };
        }

        // ------------------------------------------------------------------ events
        private void HookActions()
        {
            fileA.TextChanged += (sender, args) => InputsChanged();
            fileB.TextChanged += (sender, args) => InputsChanged();
            fileA.Leave += (sender, args) => LoadIfChanged(true);
            fileB.Leave += (sender, args) => LoadIfChanged(false);
            sheetA.SelectedIndexChanged += (sender, args) => SheetChosen(true);
            sheetB.SelectedIndexChanged += (sender, args) => SheetChosen(false);
            byPosition.CheckedChanged += (sender, args) => { InputsChanged(); RefreshAvailability(); };
            byKey.CheckedChanged += (sender, args) => { InputsChanged(); RefreshAvailability(); };
            keysA.ItemCheck += (sender, args) => KeyTicked(keysA, args);
            keysB.ItemCheck += (sender, args) => KeyTicked(keysB, args);
            headerRow.ValueChanged += (sender, args) => InputsChanged();
            headerRow.Leave += (sender, args) => ReloadForHeaderRow();
            tolerance.TextChanged += (sender, args) => InputsChanged();
            thresholds.TextChanged += (sender, args) => InputsChanged();
            formats.CheckedChanged += (sender, args) => InputsChanged();
            layout.CheckedChanged += (sender, args) => InputsChanged();
            formulas.CheckedChanged += (sender, args) => InputsChanged();
            output.TextChanged += (sender, args) => InputsChanged();
            check.Click += (sender, args) => CheckInputs();
            compare.Click += (sender, args) => StartComparison(AskReplace);
            cancel.Click += (sender, args) => CancelCurrent();
            openReport.Click += (sender, args) => OpenReport(false);
            openFolder.Click += (sender, args) => OpenReport(true);
            worker.DoWork += Work;
            worker.ProgressChanged += (sender, args) =>
            {
                if (IsDisposed || Disposing) return;
                progress.Value = Math.Max(progress.Minimum, Math.Min(progress.Maximum, args.ProgressPercentage));
                var message = args.UserState as string;
                if (!string.IsNullOrEmpty(message)) status.Text = "Status: " + message;
            };
            worker.RunWorkerCompleted += Completed;
            elapsedTimer.Tick += (sender, args) => ShowElapsed();
        }

        private void ShowElapsed()
        {
            elapsedLabel.Text = "Elapsed " + elapsed.Elapsed.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>An input was edited: a result on screen no longer belongs to what the fields show.</summary>
        private void InputsChanged()
        {
            if (suppressChanges || IsDisposed || Disposing) return;
            if (result != null && !staleNote.Visible) ShowResultCard(true);
        }

        private void KeyTicked(CheckedListBox list, ItemCheckEventArgs args)
        {
            if (suppressChanges) return;
            var item = (KeyItem)list.Items[args.Index];
            if (args.NewValue == CheckState.Checked)
            {
                if (item.Part == 0) item.Part = list.Items.Cast<KeyItem>().Count(other => other.Part > 0) + 1;
            }
            else if (item.Part > 0)
            {
                int removed = item.Part;
                item.Part = 0;
                foreach (KeyItem other in list.Items) if (other.Part > removed) other.Part--;
            }
            list.Invalidate();      // the part numbers are part of the item text
            InputsChanged();
        }

        /// <summary>The ticked key columns of one file, in the order they were ticked.</summary>
        internal string[] KeyOrder(bool source)
        {
            return (source ? keysA : keysB).Items.Cast<KeyItem>().Where(item => item.Part > 0).OrderBy(item => item.Part).Select(item => item.Name).ToArray();
        }

        // ------------------------------------------------------------------ choosing and reading the workbooks
        private void BrowseInput(bool source)
        {
            using (var dialog = new OpenFileDialog
            {
                Filter = "Excel workbooks (*.xlsx;*.xlsm)|*.xlsx;*.xlsm", CheckFileExists = true, Multiselect = false,
                Title = source ? "Choose the Source workbook (File A)" : "Choose the Target workbook (File B)"
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                (source ? fileA : fileB).Text = dialog.FileName;
                LoadWorkbook(source, false);
            }
        }

        private void BrowseOutput()
        {
            using (var dialog = new SaveFileDialog
            {
                Filter = "Excel report (*.xlsx)|*.xlsx", AddExtension = true, DefaultExt = "xlsx", FileName = DefaultReportName,
                OverwritePrompt = false, Title = "Choose where to save the report"
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK) output.Text = dialog.FileName;
            }
        }

        private void LoadIfChanged(bool source)
        {
            if (IsWorking) return;
            string typed = (source ? fileA : fileB).Text;
            LoadedFile loaded = source ? loadedFileA : loadedFileB;
            if (string.IsNullOrWhiteSpace(typed) || (loaded != null && SamePath(loaded.Path, typed))) return;
            LoadWorkbook(source, false);
        }

        private void SheetChosen(bool source)
        {
            if (suppressChanges) return;
            InputsChanged();
            if ((source ? loadedFileA : loadedFileB) != null && !IsWorking) LoadWorkbook(source, true);
        }

        private void ReloadForHeaderRow()
        {
            if (IsWorking) return;
            int row = (int)headerRow.Value;
            bool a = loadedFileA != null && loadedFileA.HeaderRow != row, b = loadedFileB != null && loadedFileB.HeaderRow != row;
            if (a || b) StartLoad(a, b, true);
        }

        /// <summary>Reads the sheet names and the header row of one workbook in the background.</summary>
        internal bool LoadWorkbook(bool source, bool keepSheet)
        {
            return StartLoad(source, !source, keepSheet);
        }

        private bool StartLoad(bool a, bool b, bool keepSheet)
        {
            var job = new Job { Kind = JobKind.Load, HeaderRow = (int)headerRow.Value };
            if (!FormValidation.Check(this, () =>
            {
                FormValidation.IsBusy(worker, a ? fileA : fileB);
                if (a)
                {
                    job.PathA = FormValidation.ReadableFile(fileA.Text, fileA, "File A", ".xlsx", ".xlsm");
                    job.SheetA = keepSheet && loadedFileA != null && SamePath(loadedFileA.Path, job.PathA) ? ChosenSheet(sheetA) : null;
                }
                if (b)
                {
                    job.PathB = FormValidation.ReadableFile(fileB.Text, fileB, "File B", ".xlsx", ".xlsm");
                    job.SheetB = keepSheet && loadedFileB != null && SamePath(loadedFileB.Path, job.PathB) ? ChosenSheet(sheetB) : null;
                }
            }, message => SetStatus(message, true))) return false;
            return StartJob(job, a && b ? "Reading both workbooks" : a ? "Reading File A" : "Reading File B");
        }

        private static string ChosenSheet(ComboBox picker)
        {
            return picker.SelectedIndex > 0 ? (string)picker.SelectedItem : null;
        }

        private static bool SamePath(string a, string b)
        {
            try { return string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase); }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException) { return false; }
        }

        private void ShowLoaded(bool source, string path, string sheet, int row, IdentityFileInfo info, string error)
        {
            TextBox field = source ? fileA : fileB;
            ComboBox picker = source ? sheetA : sheetB;
            CheckedListBox list = source ? keysA : keysB;
            Label note = source ? loadedA : loadedB;
            string label = source ? "File A" : "File B";
            string[] ticked = KeyOrder(source);
            suppressChanges = true;
            try
            {
                picker.Items.Clear();
                picker.Items.Add(AllSheets);
                list.Items.Clear();
                if (info == null)
                {
                    picker.SelectedIndex = 0;
                    if (source) loadedFileA = null; else loadedFileB = null;
                    note.Text = Common_Data.RedactConnectionCredentials("Not loaded: " + error);
                }
                else
                {
                    picker.Items.AddRange(info.Sheets.Cast<object>().ToArray());
                    picker.SelectedIndex = sheet == null ? 0 : Math.Max(0, picker.Items.IndexOf(sheet));
                    foreach (string header in info.Headers)
                    {
                        // A key that was ticked before keeps its place when the same column is still there.
                        int part = Array.FindIndex(ticked, name => string.Equals(name, header, StringComparison.OrdinalIgnoreCase)) + 1;
                        list.Items.Add(new KeyItem(header) { Part = part }, part > 0);
                    }
                    Renumber(list);
                    var loaded = new LoadedFile { Path = path, HeaderRow = row, Info = info };
                    if (source) loadedFileA = loaded; else loadedFileB = loaded;
                    note.Text = "Loaded: " + Count(info.Sheets.Length, "sheet") + ", " + Count(info.Columns, "column") + ", " + Count(info.DataRows, "data row")
                        + (info.Sheets.Length > 1 ? " in sheet '" + info.Sheet + "'" : string.Empty);
                }
            }
            finally { suppressChanges = false; }
            if (info == null)
            {
                FormValidation.Check(this, () => FormValidation.Reject(field, label + " could not be read. " + error), message => SetStatus(message, true));
                return;
            }
            AppendLog((source ? "Source" : "Target") + " loaded: " + Path.GetFileName(path) + " - " + Count(info.Sheets.Length, "sheet") + ", "
                + Count(info.Columns, "column") + ", " + Count(info.DataRows, "data row"));
            InputsChanged();
        }

        /// <summary>Closes gaps in the part numbers after some ticked columns were not found again.</summary>
        private static void Renumber(CheckedListBox list)
        {
            int next = 1;
            foreach (KeyItem item in list.Items.Cast<KeyItem>().Where(item => item.Part > 0).OrderBy(item => item.Part).ToList()) item.Part = next++;
        }

        private static string Count(int number, string noun)
        {
            return number.ToString("N0", CultureInfo.InvariantCulture) + " " + noun + (number == 1 ? string.Empty : "s");
        }

        // ------------------------------------------------------------------ checking the inputs
        /// <summary>Checks every input and returns what a comparison needs. A wrong input is reported beside its field.</summary>
        internal IdentityRequest CaptureRequest()
        {
            FormValidation.IsBusy(worker, compare);
            string a = FormValidation.ReadableFile(fileA.Text, fileA, "File A", ".xlsx", ".xlsm");
            string b = FormValidation.ReadableFile(fileB.Text, fileB, "File B", ".xlsx", ".xlsm");
            int row = (int)headerRow.Value;
            RequireLoaded(loadedFileA, a, row, fileA, "File A");
            RequireLoaded(loadedFileB, b, row, fileB, "File B");

            var options = new IdentityOptions
            {
                HeaderRow = row, CompareFormats = formats.Checked, CompareLayout = layout.Checked, CompareFormulas = formulas.Checked,
            };
            string oneA = ChosenSheet(sheetA), oneB = ChosenSheet(sheetB);
            FormValidation.Require((oneA == null) == (oneB == null), oneA == null ? sheetA : sheetB,
                "Choose one sheet in both files, or \"" + AllSheets + "\" in both.");
            options.SheetA = oneA;
            options.SheetB = oneB;

            if (byKey.Checked)
            {
                string[] keyA = KeyOrder(true), keyB = KeyOrder(false);
                FormValidation.Require(keyA.Length > 0, keysA, "Tick at least one key column in Source, or choose \"By row position\".");
                FormValidation.Require(keyB.Length > 0, keysB, "Tick the key column(s) in Target that pair with the ones ticked in Source.");
                FormValidation.Require(keyA.Length == keyB.Length, keysB, "Tick the same number of key columns in both files. Source has "
                    + keyA.Length.ToString(CultureInfo.InvariantCulture) + ", Target has " + keyB.Length.ToString(CultureInfo.InvariantCulture) + ".");
                options.KeyA = keyA;
                options.KeyB = keyB;
            }

            double allowed;
            FormValidation.Require(TryNumber(tolerance.Text, out allowed), tolerance,
                "Enter the number tolerance as a number that is 0 or more, with a dot as the decimal separator (for example 0 or 0.005).");
            options.Tolerance = allowed;
            options.Thresholds = ParseThresholds(thresholds.Text, thresholds);

            string report = FormValidation.OutputFile(output.Text, output, "report file", ".xlsx");
            FormValidation.DifferentFiles(report, a, output);
            FormValidation.DifferentFiles(report, b, output);
            return new IdentityRequest { FileA = a, FileB = b, Options = options, Report = report, OpenWhenDone = openWhenDone.Checked };
        }

        private static void RequireLoaded(LoadedFile loaded, string path, int row, Control field, string label)
        {
            FormValidation.Require(loaded != null && SamePath(loaded.Path, path), field,
                label + " has not been read yet. Choose it with Browse, or leave the field after typing the path.");
            FormValidation.Require(loaded.HeaderRow == row, field,
                "The header row was changed after " + label + " was read. Leave the Header row field so the columns are read again.");
        }

        private static bool TryNumber(string text, out double number)
        {
            return double.TryParse((text ?? string.Empty).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out number)
                && number >= 0 && !double.IsInfinity(number);
        }

        internal static double[] ParseThresholds(string text, Control field)
        {
            const string help = " Enter numbers that are 0 or more, separated by commas, with a dot as the decimal separator (for example 0, 0.01, 0.1, 1).";
            string[] parts = (text ?? string.Empty).Split(new[] { ',', ';' }, StringSplitOptions.None).Select(part => part.Trim()).ToArray();
            if (parts.Length == 1 && parts[0].Length == 0) FormValidation.Reject(field, "Enter at least one difference threshold." + help);
            var values = new List<double>();
            foreach (string part in parts)
            {
                double value;
                FormValidation.Require(TryNumber(part, out value), field, "\"" + part + "\" is not a valid difference threshold." + help);
                FormValidation.Require(!values.Contains(value), field, "The difference threshold " + part + " is listed twice.");
                values.Add(value);
            }
            FormValidation.Require(values.Count <= 8, field, "Use at most 8 difference thresholds.");
            return values.ToArray();
        }

        private void CheckInputs()
        {
            IdentityRequest request = null;
            if (!FormValidation.Check(this, () => request = CaptureRequest(), message => SetStatus(message, true))) return;
            SetStatus("The inputs are complete. Nothing has been compared yet.");
            AppendLog(request.Options.KeyA == null
                ? "Rows will be matched by position."
                : "Rows will be matched by key: " + string.Join(" + ", request.Options.KeyA) + " (Source) with " + string.Join(" + ", request.Options.KeyB) + " (Target).");
            if (File.Exists(request.Report)) AppendLog("The report file exists already. It is replaced only after you confirm.");
        }

        // ------------------------------------------------------------------ comparing
        private bool AskReplace(string path)
        {
            return MessageBox.Show(this, "Replace the existing report?\r\n\r\n" + path + "\r\n\r\nNeither workbook that is compared will be changed.",
                "Confirm report replacement", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        /// <summary>
        /// Checks the inputs and starts the comparison. <paramref name="confirmReplace"/> is asked only when the
        /// report file exists already. Returns false when nothing was started.
        /// </summary>
        internal bool StartComparison(Func<string, bool> confirmReplace)
        {
            IdentityRequest request = null;
            if (!FormValidation.Check(this, () => request = CaptureRequest(), message => SetStatus(message, true))) return false;
            if (File.Exists(request.Report) && (confirmReplace == null || !confirmReplace(request.Report)))
            {
                SetStatus("Nothing was started. The existing report was kept.");
                return false;
            }
            AppendLog("Comparison started. " + (request.Options.KeyA == null
                ? "Rows matched by position."
                : "Rows matched by key: " + string.Join(" + ", request.Options.KeyA) + " (Source) with " + string.Join(" + ", request.Options.KeyB) + " (Target)."));
            return StartJob(new Job { Kind = JobKind.Compare, Request = request }, "Comparing the workbooks");
        }

        private bool StartJob(Job job, string message)
        {
            if (IsWorking) return false;
            currentJob = job;
            cancel.Enabled = true;
            inputLock = FormValidation.LockInputs(this, cancel);
            elapsed.Restart();
            ShowElapsed();
            elapsedTimer.Start();
            progress.Value = 0;
            SetStatus(message + "...");
            try { worker.RunWorkerAsync(job); }
            catch (Exception error)
            {
                FinishJob();
                SetStatus("Could not start: " + error.Message, true);
                return false;
            }
            return true;
        }

        private void Work(object sender, DoWorkEventArgs args)
        {
            var job = (Job)args.Argument;
            Func<bool> cancelled = () => worker.CancellationPending;
            try
            {
                if (job.Kind == JobKind.Load)
                {
                    var loaded = new LoadOutcome();
                    if (job.PathA != null) loaded.A = Inspect(job.PathA, job.SheetA, job.HeaderRow, cancelled, out loaded.ErrorA);
                    if (job.PathB != null) loaded.B = Inspect(job.PathB, job.SheetB, job.HeaderRow, cancelled, out loaded.ErrorB);
                    args.Result = loaded;
                }
                else
                {
                    IdentityRequest request = job.Request;
                    IdentityResult compared = WorkbookIdentityEngine.Compare(request.FileA, request.FileB, request.Options,
                        (percent, message) => worker.ReportProgress(percent * 9 / 10, message), cancelled);
                    if (cancelled()) { args.Cancel = true; return; }
                    worker.ReportProgress(92, "Writing the report");
                    WriteReport(compared, request.Report, cancelled);
                    args.Result = compared;
                }
                if (cancelled() && job.Kind == JobKind.Load) args.Cancel = true;
            }
            catch (OperationCanceledException) { args.Cancel = true; }
        }

        /// <summary>
        /// Writes the report beside its destination and puts it in place only when it is complete. Cancel pressed
        /// while it is being written stops here: the unfinished run publishes nothing and an existing report stays.
        /// </summary>
        internal static void WriteReport(IdentityResult compared, string path, Func<bool> cancelled)
        {
            FormValidation.WriteAtomically(path, temporary =>
            {
                IdentityReportWriter.Write(compared, temporary);
                if (cancelled != null && cancelled()) throw new OperationCanceledException();
            });
        }

        private static IdentityFileInfo Inspect(string path, string sheet, int row, Func<bool> cancelled, out string error)
        {
            error = null;
            try { return WorkbookIdentityEngine.Inspect(path, sheet, row, cancelled); }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is InvalidDataException || ex is ArgumentException
                || ex is NotSupportedException || ex is FormatException || ex is InvalidOperationException || ex is System.Xml.XmlException)
            {
                error = Common_Data.RedactConnectionCredentials(ex.Message);
                return null;
            }
        }

        private void Completed(object sender, RunWorkerCompletedEventArgs args)
        {
            Job job = currentJob;
            FinishJob();
            if (IsDisposed || Disposing) { worker.Dispose(); return; }
            if (job == null) return;
            if (args.Cancelled)
            {
                SetStatus(job.Kind == JobKind.Compare ? "Cancelled. No report was written and an existing report was kept." : "Cancelled. The workbook was not read.");
            }
            else if (args.Error != null)
            {
                SetStatus((job.Kind == JobKind.Compare ? "The comparison could not be finished: " : "The workbook could not be read: ")
                    + Common_Data.RedactConnectionCredentials(args.Error.Message), true);
            }
            else if (job.Kind == JobKind.Load)
            {
                var loaded = (LoadOutcome)args.Result;
                bool failed = false;
                if (job.PathA != null) { ShowLoaded(true, job.PathA, job.SheetA, job.HeaderRow, loaded.A, loaded.ErrorA); failed |= loaded.A == null; }
                if (job.PathB != null) { ShowLoaded(false, job.PathB, job.SheetB, job.HeaderRow, loaded.B, loaded.ErrorB); failed |= loaded.B == null; }
                if (!failed) SetStatus("ready. Choose how rows are matched, then compare.", false, false);
            }
            else
            {
                PresentResult((IdentityResult)args.Result, job.Request.Report);
                SetStatus("finished. Report written.", false, false);
                AppendLog("Report written: " + job.Request.Report);
                if (job.Request.OpenWhenDone) OpenReport(false);
            }
            RefreshAvailability();
        }

        private void FinishJob()
        {
            elapsed.Stop();
            if (inputLock != null) { inputLock.Dispose(); inputLock = null; }
            currentJob = null;
            if (IsDisposed || Disposing) return;
            elapsedTimer.Stop();
            ShowElapsed();
            progress.Value = 0;
            RefreshAvailability();
        }

        private void CancelCurrent()
        {
            if (!worker.IsBusy || worker.CancellationPending) return;
            worker.CancelAsync();
            cancel.Enabled = false;
            SetStatus("Cancelling. Waiting for the open workbooks to close safely.");
        }

        // ------------------------------------------------------------------ showing the result
        /// <summary>Shows a finished comparison: the verdict, how rows were matched and the five tables of the report.</summary>
        internal void PresentResult(IdentityResult finished, string report)
        {
            if (finished == null) throw new ArgumentNullException(nameof(finished));
            result = finished;
            reportPath = report;
            verdict.Text = finished.Verdict;
            verdict.ForeColor = UiTheme.Readable(finished.Identical ? Color.DarkGreen : Color.Red);
            matching.Text = "Rows matched: " + finished.RowMatching + ". Columns matched by header name, ignoring case.";
            staleNote.ForeColor = UiTheme.Readable(Color.Red);
            reportLabel.Text = report ?? string.Empty;

            IdentityTable[] tables = IdentityReportWriter.Tables(finished);
            var cut = new List<string>();
            for (int i = 0; i < tables.Length && i < grids.Count; i++)
            {
                Fill(grids[i], tables[i]);
                bool counted = i == 2 || i == 3;      // the two sheets that list one row per difference
                tabs.TabPages[i].Text = TabCaption(tables[i].Name + (counted ? " (" + tables[i].Rows.Count.ToString("N0", CultureInfo.InvariantCulture) + ")" : string.Empty));
                if (tables[i].Rows.Count > PreviewRows) cut.Add(tables[i].Name);
            }
            previewNote.Text = "The tabs show the same rows as the sheets of the report."
                + (cut.Count == 0 ? string.Empty : " " + string.Join(", ", cut) + ": the first "
                    + PreviewRows.ToString("N0", CultureInfo.InvariantCulture) + " rows are shown here; the report holds all of them.");
            tabs.SelectedIndex = 0;
            ShowResultCard(false);
            scroll.ScrollControlIntoView(resultCard);

            AppendLog(finished.Verdict);
            foreach (string note in finished.Notes) AppendLog("Note: " + note);
            RefreshAvailability();
        }

        private void Fill(DataGridView view, IdentityTable table)
        {
            view.SuspendLayout();
            try
            {
                view.Rows.Clear();
                view.Columns.Clear();
                AddColumn(view, "SrNo.", 48);
                for (int c = 0; c < table.Titles.Length; c++) AddColumn(view, table.Titles[c], Math.Max(60, Math.Min(320, (int)(table.Widths[c] * 7))));
                DataGridViewColumn last = view.Columns[view.Columns.Count - 1];
                last.MinimumWidth = last.Width;
                last.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                statusColumns[view] = table.StatusColumns.ToArray();      // 1-based in the table = index in the grid, because SrNo. comes first
                int shown = Math.Min(table.Rows.Count, PreviewRows);
                for (int r = 0; r < shown; r++)
                {
                    object[] source = table.Rows[r];
                    var cells = new object[table.Titles.Length + 1];
                    cells[0] = (r + 1).ToString(CultureInfo.InvariantCulture);
                    for (int c = 0; c < table.Titles.Length && c < source.Length; c++) cells[c + 1] = CellText(source[c]);
                    view.Rows.Add(cells);
                }
                if (shown == 0)
                {
                    var cells = new object[table.Titles.Length + 1];
                    cells[1] = table.Empty;
                    view.Rows.Add(cells);
                }
            }
            finally { view.ResumeLayout(); }
        }

        private static void AddColumn(DataGridView view, string title, int width)
        {
            view.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = title, Width = width, SortMode = DataGridViewColumnSortMode.NotSortable, ReadOnly = true
            });
        }

        private static string CellText(object value)
        {
            if (value == null) return string.Empty;
            if (value is double) return ((double)value).ToString("G15", CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private void OpenReport(bool folder)
        {
            if (string.IsNullOrEmpty(reportPath) || !File.Exists(reportPath))
            {
                SetStatus("The report file is no longer there. Compare again to create it.", true);
                return;
            }
            if (SelfTest.IsRunning) return;      // build checks never start another program
            try
            {
                if (folder) Process.Start(new ProcessStartInfo("explorer.exe", "/select,\"" + reportPath + "\"") { UseShellExecute = true });
                else Process.Start(new ProcessStartInfo(reportPath) { UseShellExecute = true });
            }
            catch (Exception error) when (error is Win32Exception || error is InvalidOperationException || error is IOException)
            {
                SetStatus("The report is saved, but it could not be opened: " + error.Message, true);
            }
        }

        // ------------------------------------------------------------------ status, log and closing
        private void RefreshAvailability()
        {
            if (IsDisposed || Disposing) return;
            if (IsWorking) { cancel.Enabled = worker.IsBusy && !worker.CancellationPending; return; }
            cancel.Enabled = false;
            check.Enabled = compare.Enabled = true;
            keysA.Enabled = keysB.Enabled = byKey.Checked;
            openReport.Enabled = openFolder.Enabled = !string.IsNullOrEmpty(reportPath);
        }

        private void SetStatus(string message, bool error = false, bool addToLog = true)
        {
            if (IsDisposed || Disposing) return;
            message = Common_Data.RedactConnectionCredentials(message);
            status.Text = "Status: " + message;
            status.ForeColor = UiTheme.Readable(error ? Color.Red : Color.Black);
            if (addToLog) AppendLog(message);
        }

        private void AppendLog(string message)
        {
            if (IsDisposed || Disposing) return;
            message = Common_Data.RedactConnectionCredentials(message);
            if (log.TextLength > 24000) log.Text = log.Text.Substring(log.TextLength - 16000);
            log.AppendText(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + "  " + message + Environment.NewLine);
            log.SelectionStart = log.TextLength;
            log.ScrollToCaret();
        }

        /// <summary>The text of the log, for checks.</summary>
        internal string LogText { get { return log.Text; } }

        /// <summary>The text of the status line, for checks.</summary>
        internal string StatusText { get { return status.Text; } }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (worker.IsBusy)
            {
                e.Cancel = true;
                CancelCurrent();
                SetStatus("This task is still stopping. Close it again when the work has finished.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (worker.IsBusy && !worker.CancellationPending) worker.CancelAsync();
                elapsedTimer.Stop();
                elapsedTimer.Dispose();
                if (inputLock != null) { inputLock.Dispose(); inputLock = null; }
                if (!worker.IsBusy) worker.Dispose();
                if (statusFont != null) { statusFont.Dispose(); statusFont = null; }
            }
            base.Dispose(disposing);
        }

        // ------------------------------------------------------------------ small types
        /// <summary>Everything one comparison needs, captured from the fields on the UI thread.</summary>
        internal sealed class IdentityRequest
        {
            internal string FileA { get; set; }
            internal string FileB { get; set; }
            internal IdentityOptions Options { get; set; }
            internal string Report { get; set; }
            internal bool OpenWhenDone { get; set; }
        }

        /// <summary>One column in a key list. Its text shows which part of the key it is.</summary>
        private sealed class KeyItem
        {
            internal KeyItem(string name) { Name = name; }
            internal string Name { get; }
            internal int Part { get; set; }

            public override string ToString()
            {
                return Part > 0 ? Name + "   (key part " + Part.ToString(CultureInfo.InvariantCulture) + ")" : Name;
            }
        }

        private sealed class LoadedFile
        {
            internal string Path { get; set; }
            internal int HeaderRow { get; set; }
            internal IdentityFileInfo Info { get; set; }
        }

        private enum JobKind { Load, Compare }

        private sealed class Job
        {
            internal JobKind Kind { get; set; }
            internal string PathA { get; set; }
            internal string PathB { get; set; }
            internal string SheetA { get; set; }
            internal string SheetB { get; set; }
            internal int HeaderRow { get; set; }
            internal IdentityRequest Request { get; set; }
        }

        private sealed class LoadOutcome
        {
            internal IdentityFileInfo A, B;
            internal string ErrorA, ErrorB;
        }
    }
}
