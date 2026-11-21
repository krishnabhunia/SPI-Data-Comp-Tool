using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// The independent Design A Excel audit page. It does not reuse or modify the legacy comparison engine.
    /// All workbook inspection, comparison and export work runs off the UI thread against a captured request.
    /// </summary>
    public sealed class ExcelSheetAuditForm : WorkspacePageForm
    {
        private const int PreviewPageSize = 500;
        private readonly BackgroundWorker worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        private readonly System.Windows.Forms.Timer elapsedTimer = new System.Windows.Forms.Timer { Interval = 500 };
        private readonly Stopwatch elapsed = new Stopwatch();
        private readonly TextBox fileA = Field("tb_AuditFileA");
        private readonly TextBox fileB = Field("tb_AuditFileB");
        private readonly ComboBox sheetA = SheetPicker("cb_AuditSheetA");
        private readonly ComboBox sheetB = SheetPicker("cb_AuditSheetB");
        private readonly NumericUpDown headerA = HeaderPicker("nud_AuditHeaderA");
        private readonly NumericUpDown headerB = HeaderPicker("nud_AuditHeaderB");
        private readonly Label metadataA = Note("Load a workbook to choose its worksheet and headers.");
        private readonly Label metadataB = Note("Load a workbook to choose its worksheet and headers.");
        private readonly RadioButton positional = new RadioButton { Name = "rb_AuditPosition", Text = "No key · Row by row", Checked = true, AutoSize = true };
        private readonly RadioButton primaryKey = new RadioButton { Name = "rb_AuditKey", Text = "Primary key", AutoSize = true };
        private readonly DataGridView keyMappings = new DataGridView { Name = "dgv_AuditKeys" };
        private readonly Button addKey = ActionButton("btn_AuditAddKey", "Add key column");
        private readonly Button removeKey = ActionButton("btn_AuditRemoveKey", "Remove selected");
        private readonly CheckBox values = Check("chk_AuditValues", "Values and data types");
        private readonly CheckBox headers = Check("chk_AuditHeaders", "Headers and column order");
        private readonly CheckBox formatting = Check("chk_AuditFormatting", "Cell formatting");
        private readonly CheckBox layout = Check("chk_AuditLayout", "Worksheet layout");
        private readonly TextBox output = Field("tb_AuditOutput");
        private readonly TextBox temporary = Field("tb_AuditTemp");
        private readonly NumericUpDown maxDetails = new NumericUpDown
        {
            Name = "nud_AuditMaxDetails", Minimum = 1, Maximum = 1000000, Value = 100000,
            ThousandsSeparator = true, Width = 125, Dock = DockStyle.Fill
        };
        private readonly Button validate = ActionButton("btn_AuditValidate", "Validate");
        private readonly Button compare = ActionButton("btn_AuditCompare", "Compare sheets");
        private readonly Button export = ActionButton("btn_AuditExport", "Export report");
        private readonly Button openReport = ActionButton("btn_AuditOpenReport", "Open report");
        private readonly Button cancel = ActionButton("btn_AuditCancel", "Cancel");
        private readonly TabControl tabs = new TabControl { Name = "tabs_ExcelAudit", Dock = DockStyle.Fill };
        private readonly TabPage setupTab = new TabPage("Setup comparison");
        private readonly TabPage dataTab = new TabPage("Data Differences");
        private readonly TabPage summaryTab = new TabPage("Summary & other differences");
        private readonly DataGridView dataGrid = new DataGridView { Name = "dgv_AuditData" };
        private readonly DataGridView otherGrid = new DataGridView { Name = "dgv_AuditOther" };
        private readonly Label dataPageLabel = Note("No comparison results.");
        private readonly Label otherPageLabel = Note("No comparison results.");
        private readonly Label overall = Note("Load two worksheets, choose matching rules, then compare.");
        private readonly Label metricValues = Note("Values: not checked");
        private readonly Label metricStructure = Note("Headers & structure: not checked");
        private readonly Label metricFormatting = Note("Formatting: not checked");
        private readonly Label summaryText = Note("No comparison has been run.");
        private readonly Label phase = Note("Ready · No comparison has been run.");
        private readonly Label elapsedLabel = Note("Elapsed: 00:00:00");
        private readonly ProgressBar progress = new ProgressBar { Name = "pb_AuditProgress", Dock = DockStyle.Fill, Height = 12 };
        private readonly RichTextBox log = new RichTextBox
        {
            Name = "rtb_Log", Dock = DockStyle.Fill, ReadOnly = true, Height = 62,
            DetectUrls = false, BorderStyle = BorderStyle.FixedSingle
        };
        private readonly List<Button> pageButtons = new List<Button>();
        private ExcelAuditSheetInfo sourceInfo;
        private ExcelAuditSheetInfo targetInfo;
        private ExcelAuditResult result;
        private CancellationTokenSource cancellation;
        private IDisposable inputLock;
        private AuditJob currentJob;
        private bool suppressChanges;
        private int revision;
        private int dataPage;
        private int otherPage;
        private string savedReport;

        public ExcelSheetAuditForm()
        {
            Name = "ExcelSheetAuditForm";
            Text = "Excel Sheet Audit";
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 9F);
            ClientSize = new Size(1120, 850);
            MinimumSize = new Size(820, 600);
            StartPosition = FormStartPosition.CenterParent;
            output.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Excel_Audit_Report.xlsx");
            temporary.Text = Path.GetTempPath();
            BuildLayout();
            HookActions();
            UiTheme.Apply(this, compare, export);
            RefreshAvailability();
        }

        private static TextBox Field(string name)
        {
            return new TextBox { Name = name, Dock = DockStyle.Fill, Margin = new Padding(3, 5, 3, 5) };
        }

        private static ComboBox SheetPicker(string name)
        {
            return new ComboBox { Name = name, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        }

        private static NumericUpDown HeaderPicker(string name)
        {
            return new NumericUpDown { Name = name, Minimum = 1, Maximum = 1048576, Value = 1, Dock = DockStyle.Fill };
        }

        private static Button ActionButton(string name, string caption)
        {
            return new Button { Name = name, Text = caption, AutoSize = true, MinimumSize = new Size(84, 30), Margin = new Padding(4) };
        }

        private static Label Note(string text)
        {
            return new Label { Text = text, AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(3), TextAlign = ContentAlignment.MiddleLeft };
        }

        private static CheckBox Check(string name, string caption)
        {
            return new CheckBox { Name = name, Text = caption, Checked = true, AutoSize = true, Margin = new Padding(4, 5, 4, 0) };
        }

        private static TableLayoutPanel Table(int columns)
        {
            var table = new TableLayoutPanel { ColumnCount = columns, Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0) };
            for (int i = 0; i < columns; i++) table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columns));
            return table;
        }

        private static void AddRow(TableLayoutPanel table, Control control, float height = 0)
        {
            int row = table.RowCount++;
            table.RowStyles.Add(height == 0 ? new RowStyle(SizeType.AutoSize) : new RowStyle(SizeType.Absolute, height));
            table.Controls.Add(control, 0, row);
            table.SetColumnSpan(control, table.ColumnCount);
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, Padding = new Padding(10) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var heading = Table(1);
            heading.Padding = new Padding(12, 8, 12, 8);
            UiTheme.MarkHeader(heading);
            var title = Note("Excel Sheet Audit");
            title.Font = new Font(Font.FontFamily, 16F, FontStyle.Bold);
            AddRow(heading, title);
            AddRow(heading, Note("New separate form · Your existing Excel comparison remains available and unchanged."));
            root.Controls.Add(heading, 0, 0);
            tabs.TabPages.AddRange(new[] { setupTab, dataTab, summaryTab });
            root.Controls.Add(tabs, 0, 1);
            BuildSetup();
            BuildResults();
            var status = Table(2);
            status.RowCount = 3;
            status.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            status.RowStyles.Add(new RowStyle(SizeType.Absolute, 13));
            status.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            status.Controls.Add(phase, 0, 0);
            status.Controls.Add(elapsedLabel, 1, 0);
            elapsedLabel.TextAlign = ContentAlignment.MiddleRight;
            status.Controls.Add(progress, 0, 1);
            status.SetColumnSpan(progress, 2);
            status.Controls.Add(log, 0, 2);
            status.SetColumnSpan(log, 2);
            root.Controls.Add(status, 0, 2);
            var footer = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = true };
            footer.Controls.AddRange(new Control[] { cancel, openReport, export, compare, validate });
            root.Controls.Add(footer, 0, 3);
            Controls.Add(root);
        }

        private void BuildSetup()
        {
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
            var setup = Table(1);
            setup.Dock = DockStyle.Top;
            var files = Table(2);
            files.Controls.Add(FileCard(true), 0, 0);
            files.Controls.Add(FileCard(false), 1, 0);
            AddRow(setup, files);
            var rules = Table(2);
            rules.ColumnStyles[0].Width = 57;
            rules.ColumnStyles[1].Width = 43;
            rules.Controls.Add(MatchingCard(), 0, 0);
            rules.Controls.Add(ChecksCard(), 1, 0);
            AddRow(setup, rules);
            AddRow(setup, OutputCard());
            var scope = Note("Values and cell formats cover the named table columns at and below each selected header row. " +
                "Column names ignore case; text/key values and spaces remain exact. Supported worksheet-layout metadata is checked separately. " +
                "Saved formulas are compared, not recalculated. Pre-header content, unnamed-column styles, charts, images, macros and pixel-perfect rendering are outside the checked scope.");
            scope.MaximumSize = new Size(950, 0);
            AddRow(setup, scope);
            scroll.Controls.Add(setup);
            setupTab.Controls.Add(scroll);
        }

        private GroupBox FileCard(bool source)
        {
            var group = new GroupBox
            {
                Name = source ? "gb_AuditSource" : "gb_AuditTarget", Text = source ? "File A · Source" : "File B · Target",
                Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(12, 28, 12, 10), Margin = new Padding(4)
            };
            UiTheme.MarkSide(group, source);
            var grid = Table(1);
            AddRow(grid, Note("Workbook (.xlsx or .xlsm)"));
            var path = new TableLayoutPanel { ColumnCount = 3, Dock = DockStyle.Fill, AutoSize = true };
            path.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            path.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            path.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var browse = ActionButton(source ? "btn_AuditBrowseA" : "btn_AuditBrowseB", "Browse…");
            var load = ActionButton(source ? "btn_AuditLoadA" : "btn_AuditLoadB", "Load");
            browse.MinimumSize = load.MinimumSize = new Size(64, 28);
            browse.Click += (sender, args) => BrowseInput(source);
            load.Click += (sender, args) => LoadMetadata(source);
            path.Controls.Add(source ? fileA : fileB, 0, 0);
            path.Controls.Add(browse, 1, 0);
            path.Controls.Add(load, 2, 0);
            AddRow(grid, path);
            var selection = Table(2);
            selection.ColumnStyles[0].Width = 74;
            selection.ColumnStyles[1].Width = 26;
            selection.Controls.Add(Note("Worksheet"), 0, 0);
            selection.Controls.Add(Note("Header row"), 1, 0);
            selection.Controls.Add(source ? sheetA : sheetB, 0, 1);
            selection.Controls.Add(source ? headerA : headerB, 1, 1);
            AddRow(grid, selection);
            AddRow(grid, source ? metadataA : metadataB);
            group.Controls.Add(grid);
            return group;
        }

        private GroupBox MatchingCard()
        {
            var group = new GroupBox { Name = "gb_AuditMatching", Text = "Match records · Primary key is optional", AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(12, 26, 12, 10), Margin = new Padding(4) };
            var grid = Table(1);
            var radios = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
            radios.Controls.AddRange(new Control[] { positional, primaryKey });
            AddRow(grid, radios);
            ConfigureGrid(keyMappings, false);
            keyMappings.Height = 112;
            keyMappings.Columns.Add(new DataGridViewTextBoxColumn { Name = "Order", HeaderText = "#", ReadOnly = true, Width = 35, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            keyMappings.Columns.Add(new DataGridViewComboBoxColumn { Name = "KeyA", HeaderText = "File A key column", FlatStyle = FlatStyle.Flat, DisplayStyleForCurrentCellOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            keyMappings.Columns.Add(new DataGridViewComboBoxColumn { Name = "KeyB", HeaderText = "File B key column", FlatStyle = FlatStyle.Flat, DisplayStyleForCurrentCellOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            AddRow(grid, keyMappings, 112);
            var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
            buttons.Controls.AddRange(new Control[] { addKey, removeKey });
            AddRow(grid, buttons);
            AddRow(grid, Note("Add one row for a single key or several rows for an ordered composite key.\r\nThe complete key must be nonblank and unique in each file; invalid keys stop the run."));
            group.Controls.Add(grid);
            return group;
        }

        private GroupBox ChecksCard()
        {
            var group = new GroupBox { Name = "gb_AuditChecks", Text = "What to compare", AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(12, 26, 12, 10), Margin = new Padding(4) };
            var grid = Table(1);
            AddRow(grid, values);
            AddRow(grid, Note("Text, numbers, dates, types and saved formulas"));
            AddRow(grid, headers);
            AddRow(grid, Note("Names, case changes, count and column positions"));
            AddRow(grid, formatting);
            AddRow(grid, Note("Fonts, alignment, borders, fill and number formats"));
            AddRow(grid, layout);
            AddRow(grid, Note("Named-column dimensions, table-row visibility, merged ranges and pane settings"));
            group.Controls.Add(grid);
            return group;
        }

        private GroupBox OutputCard()
        {
            var group = new GroupBox { Text = "Report output", AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(12, 26, 12, 10), Margin = new Padding(4) };
            var grid = Table(1);
            var path = Table(2);
            path.ColumnStyles[0].Width = 85;
            path.ColumnStyles[1].Width = 15;
            var browse = ActionButton("btn_AuditBrowseOutput", "Browse…");
            browse.Click += (sender, args) => BrowseOutput();
            path.Controls.Add(output, 0, 0);
            path.Controls.Add(browse, 1, 0);
            AddRow(grid, path);
            AddRow(grid, Note("Exactly two report worksheets: Summary (totals and non-data findings) + Data Differences. Compare first, then Export report."));
            var options = Table(4);
            options.ColumnStyles[0].Width = 15;
            options.ColumnStyles[1].Width = 50;
            options.ColumnStyles[2].Width = 20;
            options.ColumnStyles[3].Width = 15;
            options.Controls.Add(Note("Temporary folder"), 0, 0);
            options.Controls.Add(temporary, 1, 0);
            options.Controls.Add(Note("Details per section"), 2, 0);
            options.Controls.Add(maxDetails, 3, 0);
            AddRow(grid, options);
            AddRow(grid, Note("Temporary data is deleted after the run. Detail/memory limits never reduce the total counts; omitted details are disclosed."));
            group.Controls.Add(grid);
            return group;
        }

        private static void ConfigureGrid(DataGridView grid, bool readOnly)
        {
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = readOnly;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToOrderColumns = readOnly;
            grid.RowHeadersVisible = false;
            grid.MultiSelect = readOnly;
            grid.SelectionMode = readOnly ? DataGridViewSelectionMode.CellSelect : DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            grid.RowTemplate.Height = readOnly ? 42 : 27;
            grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
        }

        private void BuildResults()
        {
            BuildDetailPage(dataTab, dataGrid, dataPageLabel, true);
            BuildDetailPage(summaryTab, otherGrid, otherPageLabel, false);
        }

        private void BuildDetailPage(TabPage page, DataGridView grid, Label pageLabel, bool data)
        {
            var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), ColumnCount = 1, RowCount = data ? 3 : 6 };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            if (!data)
            {
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panel.Controls.Add(overall, 0, 0);
                var metrics = Table(3);
                metrics.Controls.Add(metricValues, 0, 0);
                metrics.Controls.Add(metricStructure, 1, 0);
                metrics.Controls.Add(metricFormatting, 2, 0);
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panel.Controls.Add(metrics, 0, 1);
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panel.Controls.Add(summaryText, 0, 2);
            }
            int offset = data ? 0 : 3;
            var caption = Note(data ? "Data differences only · Values, types, formulas and records present in only one file." : "All non-data findings · Headers, formatting, layout and incomplete-scope notices.");
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(caption, 0, offset);
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            ConfigureGrid(grid, true);
            foreach (string name in new[] { "Category", "Difference", "Primary key", "File A column", "File B column", "File A cell", "File B cell", "File A value", "File B value", "Details" })
            {
                int column = grid.Columns.Add(name.Replace(" ", string.Empty), name);
                grid.Columns[column].Width = name.Contains("value") || name == "Details" ? 180 : 120;
                grid.Columns[column].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            panel.Controls.Add(grid, 0, offset + 1);
            var navigation = new TableLayoutPanel { ColumnCount = 3, Dock = DockStyle.Fill, AutoSize = true };
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var previous = ActionButton(data ? "btn_AuditDataPrevious" : "btn_AuditOtherPrevious", "Previous");
            var next = ActionButton(data ? "btn_AuditDataNext" : "btn_AuditOtherNext", "Next");
            previous.Click += (sender, args) => MovePage(data, -1);
            next.Click += (sender, args) => MovePage(data, 1);
            pageButtons.Add(previous);
            pageButtons.Add(next);
            navigation.Controls.Add(previous, 0, 0);
            navigation.Controls.Add(pageLabel, 1, 0);
            navigation.Controls.Add(next, 2, 0);
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(navigation, 0, offset + 2);
            page.Controls.Add(panel);
        }

        private void HookActions()
        {
            fileA.TextChanged += (sender, args) => InputChanged(true, true);
            fileB.TextChanged += (sender, args) => InputChanged(false, true);
            sheetA.SelectedIndexChanged += (sender, args) => InputChanged(true, false);
            sheetB.SelectedIndexChanged += (sender, args) => InputChanged(false, false);
            headerA.ValueChanged += (sender, args) => InputChanged(true, false);
            headerB.ValueChanged += (sender, args) => InputChanged(false, false);
            primaryKey.CheckedChanged += (sender, args) => { InvalidateResult(); RefreshAvailability(); };
            foreach (CheckBox check in new[] { values, headers, formatting, layout }) check.CheckedChanged += (sender, args) => InvalidateResult();
            temporary.TextChanged += (sender, args) => InvalidateResult();
            maxDetails.ValueChanged += (sender, args) => InvalidateResult();
            output.TextChanged += (sender, args) => { savedReport = null; RefreshAvailability(); };
            addKey.Click += (sender, args) =>
            {
                keyMappings.Rows.Add(keyMappings.Rows.Count + 1, null, null);
                InvalidateResult();
                RefreshAvailability();
            };
            removeKey.Click += (sender, args) =>
            {
                if (keyMappings.CurrentRow == null) return;
                keyMappings.Rows.Remove(keyMappings.CurrentRow);
                for (int i = 0; i < keyMappings.Rows.Count; i++) keyMappings.Rows[i].Cells[0].Value = i + 1;
                InvalidateResult();
                RefreshAvailability();
            };
            keyMappings.CurrentCellDirtyStateChanged += (sender, args) => { if (keyMappings.IsCurrentCellDirty) keyMappings.CommitEdit(DataGridViewDataErrorContexts.Commit); };
            keyMappings.CellValueChanged += (sender, args) => { if (args.ColumnIndex > 0) InvalidateResult(); };
            keyMappings.DataError += (sender, args) => { args.ThrowException = false; SetStatus("Choose key columns from the loaded headers.", true); };
            validate.Click += (sender, args) => StartComparison(true);
            compare.Click += (sender, args) => StartComparison(false);
            export.Click += (sender, args) => ExportReport();
            openReport.Click += (sender, args) => OpenSavedReport();
            cancel.Click += (sender, args) => CancelCurrent();
            worker.DoWork += Work;
            worker.ProgressChanged += (sender, args) =>
            {
                if (IsDisposed || Disposing) return;
                var status = args.UserState as ExcelAuditProgress;
                if (status != null) phase.Text = status.Phase + " · " + status.Rows.ToString("N0", CultureInfo.CurrentCulture) + " rows";
            };
            worker.RunWorkerCompleted += Completed;
            elapsedTimer.Tick += (sender, args) => elapsedLabel.Text = "Elapsed: " + elapsed.Elapsed.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        private void InputChanged(bool source, bool fileChanged)
        {
            if (suppressChanges) return;
            if (source) sourceInfo = null; else targetInfo = null;
            if (fileChanged)
            {
                suppressChanges = true;
                try { (source ? sheetA : sheetB).Items.Clear(); }
                finally { suppressChanges = false; }
            }
            (source ? metadataA : metadataB).Text = "Click Load after changing a file, worksheet or header row.";
            ResetKeyChoices();
            InvalidateResult();
        }

        private void ResetKeyChoices()
        {
            suppressChanges = true;
            try
            {
                keyMappings.Rows.Clear();
                var a = (DataGridViewComboBoxColumn)keyMappings.Columns["KeyA"];
                var b = (DataGridViewComboBoxColumn)keyMappings.Columns["KeyB"];
                a.Items.Clear(); b.Items.Clear();
                if (sourceInfo != null) a.Items.AddRange(sourceInfo.Headers.Values.Distinct(StringComparer.OrdinalIgnoreCase).Cast<object>().ToArray());
                if (targetInfo != null) b.Items.AddRange(targetInfo.Headers.Values.Distinct(StringComparer.OrdinalIgnoreCase).Cast<object>().ToArray());
            }
            finally { suppressChanges = false; }
            RefreshAvailability();
        }

        private void InvalidateResult()
        {
            if (suppressChanges) return;
            revision++;
            result = null;
            savedReport = null;
            dataGrid.Rows.Clear(); otherGrid.Rows.Clear();
            dataPageLabel.Text = otherPageLabel.Text = "No current comparison results.";
            overall.Text = "Inputs or rules changed. Run a fresh comparison.";
            summaryText.Text = "No current comparison result. Previous validation is not reused.";
            metricValues.Text = "Values: not checked";
            metricStructure.Text = "Headers & structure: not checked";
            metricFormatting.Text = "Formatting: not checked";
            RefreshAvailability();
        }

        private void BrowseInput(bool source)
        {
            using (var dialog = new OpenFileDialog { Filter = "Excel workbooks (*.xlsx;*.xlsm)|*.xlsx;*.xlsm", CheckFileExists = true, Multiselect = false, Title = source ? "Choose File A" : "Choose File B" })
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    (source ? fileA : fileB).Text = dialog.FileName;
                    LoadMetadata(source);
                }
        }

        private void BrowseOutput()
        {
            using (var dialog = new SaveFileDialog { Filter = "Excel report (*.xlsx)|*.xlsx", AddExtension = true, DefaultExt = "xlsx", FileName = "Excel_Audit_Report.xlsx", OverwritePrompt = false, Title = "Choose comparison report" })
                if (dialog.ShowDialog(this) == DialogResult.OK) output.Text = dialog.FileName;
        }

        private void LoadMetadata(bool source)
        {
            var field = source ? fileA : fileB;
            string path = null;
            if (!FormValidation.Check(this, () =>
            {
                FormValidation.IsBusy(worker, field);
                path = FormValidation.ReadableFile(field.Text, field, source ? "File A" : "File B", ".xlsx", ".xlsm");
                FormValidation.ExistingDirectory(temporary.Text, temporary, "temporary folder");
            }, message => SetStatus(message, true))) return;
            if (source) sourceInfo = null; else targetInfo = null;
            (source ? metadataA : metadataB).Text = "Loading worksheet headers…";
            ResetKeyChoices();
            InvalidateResult();
            StartJob(new AuditJob
            {
                Kind = source ? JobKind.LoadA : JobKind.LoadB, Path = path,
                Sheet = (source ? sheetA : sheetB).SelectedItem as string,
                Header = (int)(source ? headerA : headerB).Value,
                TempDirectory = temporary.Text, Revision = revision
            }, "Loading worksheet headers");
        }

        internal ExcelAuditOptions CaptureOptions()
        {
            FormValidation.IsBusy(worker, compare);
            string a = FormValidation.ReadableFile(fileA.Text, fileA, "File A", ".xlsx", ".xlsm");
            string b = FormValidation.ReadableFile(fileB.Text, fileB, "File B", ".xlsx", ".xlsm");
            FormValidation.Selection(sheetA, "File A worksheet");
            FormValidation.Selection(sheetB, "File B worksheet");
            FormValidation.Require(sourceInfo != null, fileA, "Load File A again after changing its file, worksheet or header row.");
            FormValidation.Require(targetInfo != null, fileB, "Load File B again after changing its file, worksheet or header row.");
            FormValidation.Require(values.Checked || headers.Checked || formatting.Checked || layout.Checked, values, "Select at least one comparison check.");
            ValidateOutput(a, b);
            string temp = FormValidation.ExistingDirectory(temporary.Text, temporary, "temporary folder");
            keyMappings.EndEdit();
            var keysA = new List<string>();
            var keysB = new List<string>();
            if (primaryKey.Checked)
            {
                FormValidation.Require(keyMappings.Rows.Count > 0, keyMappings, "Add at least one primary-key mapping.");
                foreach (DataGridViewRow row in keyMappings.Rows)
                {
                    string ka = row.Cells[1].Value as string, kb = row.Cells[2].Value as string;
                    FormValidation.Require(!string.IsNullOrWhiteSpace(ka) && !string.IsNullOrWhiteSpace(kb), keyMappings, "Choose both key columns on every mapping row.");
                    FormValidation.Require(sourceInfo.Headers.Values.Count(name => string.Equals(name, ka, StringComparison.OrdinalIgnoreCase)) == 1
                        && targetInfo.Headers.Values.Count(name => string.Equals(name, kb, StringComparison.OrdinalIgnoreCase)) == 1,
                        keyMappings, "Each selected key must identify exactly one loaded column in its file.");
                    keysA.Add(ka); keysB.Add(kb);
                }
                FormValidation.Require(keysA.Distinct(StringComparer.OrdinalIgnoreCase).Count() == keysA.Count
                    && keysB.Distinct(StringComparer.OrdinalIgnoreCase).Count() == keysB.Count,
                    keyMappings, "Select each key component only once in each file.");
            }
            return new ExcelAuditOptions
            {
                FileA = a, FileB = b, SheetA = sheetA.Text, SheetB = sheetB.Text,
                HeaderRowA = (int)headerA.Value, HeaderRowB = (int)headerB.Value,
                KeysA = keysA.ToArray(), KeysB = keysB.ToArray(), CompareValues = values.Checked,
                CompareHeaders = headers.Checked, CompareFormatting = formatting.Checked, CompareLayout = layout.Checked,
                TempDirectory = temp, MaxDetails = (int)maxDetails.Value
            };
        }

        private string ValidateOutput(string a, string b)
        {
            string path = FormValidation.OutputFile(output.Text, output, "report file", ".xlsx");
            FormValidation.DifferentFiles(path, a, output);
            FormValidation.DifferentFiles(path, b, output);
            return path;
        }

        private void StartComparison(bool validationOnly)
        {
            ExcelAuditOptions options = null;
            if (!FormValidation.Check(this, () => options = CaptureOptions(), message => SetStatus(message, true))) return;
            InvalidateResult();
            StartJob(new AuditJob { Kind = validationOnly ? JobKind.Validate : JobKind.Compare, Options = options.Snapshot(), Revision = revision },
                validationOnly ? "Validating worksheets and complete keys (full scan)" : "Comparing worksheets");
        }

        private void ExportReport()
        {
            string path = null;
            if (!FormValidation.Check(this, () =>
            {
                FormValidation.IsBusy(worker, export);
                FormValidation.Require(result != null, compare, "Complete a comparison before exporting.");
                path = ValidateOutput(result.Options.FileA, result.Options.FileB);
            }, message => SetStatus(message, true))) return;
            bool overwrite = File.Exists(path);
            if (overwrite && MessageBox.Show(this, "Replace the existing report?\r\n\r\n" + path + "\r\n\r\nNeither input workbook will be overwritten.",
                "Confirm report replacement", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            savedReport = null;
            StartJob(new AuditJob { Kind = JobKind.Export, Result = result, Path = path, Overwrite = overwrite, Revision = revision }, "Writing the two-sheet report");
        }

        private void StartJob(AuditJob job, string message)
        {
            if (worker.IsBusy) return;
            cancellation = new CancellationTokenSource();
            job.Token = cancellation.Token;
            currentJob = job;
            cancel.Enabled = true;
            inputLock = FormValidation.LockInputs(this, cancel);
            elapsed.Restart();
            elapsedTimer.Start();
            progress.Style = ProgressBarStyle.Marquee;
            SetStatus(message);
            try { worker.RunWorkerAsync(job); }
            catch (Exception error)
            {
                FinishJob();
                SetStatus("Could not start: " + error.Message, true);
            }
        }

        private void Work(object sender, DoWorkEventArgs args)
        {
            var job = (AuditJob)args.Argument;
            try
            {
                job.Token.ThrowIfCancellationRequested();
                if (job.Kind == JobKind.LoadA || job.Kind == JobKind.LoadB)
                {
                    string[] names = ExcelAuditReader.ListSheets(job.Path);
                    if (names.Length == 0) throw new ExcelAuditException("The workbook contains no readable worksheets.");
                    string selected = names.Contains(job.Sheet, StringComparer.Ordinal) ? job.Sheet : names[0];
                    using (var reader = new ExcelAuditReader(job.Path, selected, job.Header, job.TempDirectory, job.Token))
                        args.Result = reader.Info;
                }
                else if (job.Kind == JobKind.Export)
                {
                    ExcelAuditReportWriter.Write(job.Result, job.Path, job.Overwrite, job.Token);
                    args.Result = job.Path;
                }
                else
                {
                    long lastNotification = 0;
                    string lastPhase = null;
                    var clock = Stopwatch.StartNew();
                    Action<ExcelAuditProgress> report = value =>
                    {
                        if (clock.ElapsedMilliseconds - lastNotification < 150 && value.Phase == lastPhase) return;
                        lastNotification = clock.ElapsedMilliseconds;
                        lastPhase = value.Phase;
                        worker.ReportProgress(0, value);
                    };
                    args.Result = job.Kind == JobKind.Validate
                        ? ExcelAuditEngine.Validate(job.Options, job.Token, report)
                        : ExcelAuditEngine.Compare(job.Options, job.Token, report);
                }
            }
            catch (OperationCanceledException) { args.Cancel = true; }
        }

        private void Completed(object sender, RunWorkerCompletedEventArgs args)
        {
            AuditJob job = currentJob;
            FinishJob();
            if (IsDisposed || Disposing) { worker.Dispose(); return; }
            if (args.Cancelled) SetStatus("Cancelled. No incomplete report was published.");
            else if (args.Error != null) SetStatus("Operation failed: " + Common_Data.RedactConnectionCredentials(args.Error.Message), true);
            else if (job == null || job.Revision != revision) SetStatus("Inputs changed while work was running. The stale result was discarded; load or compare again.", true);
            else if (job.Kind == JobKind.LoadA || job.Kind == JobKind.LoadB)
            {
                var info = (ExcelAuditSheetInfo)args.Result;
                bool source = job.Kind == JobKind.LoadA;
                suppressChanges = true;
                try
                {
                    ComboBox picker = source ? sheetA : sheetB;
                    picker.Items.Clear(); picker.Items.AddRange(info.SheetNames);
                    picker.SelectedItem = info.Name;
                    if (source) sourceInfo = info; else targetInfo = info;
                    (source ? metadataA : metadataB).Text = info.Headers.Count.ToString("N0") + " header columns loaded · Header row " + info.HeaderRow;
                }
                finally { suppressChanges = false; }
                ResetKeyChoices();
                SetStatus((source ? "File A" : "File B") + " loaded. Changing its worksheet or header row requires Load again.");
                foreach (string warning in info.Warnings.Take(10)) AppendLog("Scope: " + warning);
            }
            else if (job.Kind == JobKind.Validate)
            {
                var checkedResult = (ExcelAuditResult)args.Result;
                SetStatus(checkedResult.Complete ? "Validation passed. This is not an equality result; choose Compare sheets next."
                    : "Input validation completed with scope warnings. Some features cannot be fully checked.", !checkedResult.Complete);
                foreach (ExcelAuditFinding finding in checkedResult.OtherDetails.Where(item => item.Category == ExcelAuditCategory.Scope).Take(20))
                    AppendLog("Scope: " + finding.Details);
            }
            else if (job.Kind == JobKind.Compare)
            {
                PresentResult((ExcelAuditResult)args.Result);
                SetStatus(result.Complete ? "Comparison finished. Review the results, then Export report." : "Comparison finished with unchecked scope. Review Summary before exporting.", !result.Complete);
                tabs.SelectedTab = result.DataCount > 0 ? dataTab : summaryTab;
            }
            else if (job.Kind == JobKind.Export)
            {
                savedReport = (string)args.Result;
                SetStatus("Report saved: " + savedReport);
            }
            RefreshAvailability();
        }

        private void FinishJob()
        {
            elapsed.Stop();
            if (!IsDisposed && !Disposing) elapsedTimer.Stop();
            if (inputLock != null) { inputLock.Dispose(); inputLock = null; }
            if (cancellation != null) { cancellation.Dispose(); cancellation = null; }
            currentJob = null;
            if (!IsDisposed && !Disposing)
            {
                progress.Style = ProgressBarStyle.Blocks;
                progress.Value = 0;
                elapsedLabel.Text = "Elapsed: " + elapsed.Elapsed.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
                RefreshAvailability();
            }
        }

        private void CancelCurrent()
        {
            if (!worker.IsBusy || cancellation == null) return;
            cancellation.Cancel();
            worker.CancelAsync();
            cancel.Enabled = false;
            SetStatus("Cancellation requested. Waiting for temporary files and open workbooks to close safely.");
        }

        internal void PresentResult(ExcelAuditResult completed)
        {
            result = completed ?? throw new ArgumentNullException(nameof(completed));
            savedReport = null;
            dataPage = otherPage = 0;
            overall.Text = !result.Complete ? "Not fully checked · see scope notices below"
                : result.Matches ? "Matches under selected rules" : "Differences found · " + result.TotalCount.ToString("N0") + " findings";
            metricValues.Text = result.Options.CompareValues ? "Values & records: " + result.DataCount.ToString("N0") + " findings" : "Values & records: not selected";
            metricStructure.Text = result.Options.CompareHeaders || result.Options.CompareLayout
                ? "Headers & structure: " + (result.HeaderCount + result.StructureCount).ToString("N0") + " findings"
                : "Headers & structure: not selected";
            metricFormatting.Text = result.Options.CompareFormatting ? "Formatting: " + result.FormattingCount.ToString("N0") + " findings" : "Formatting: not selected";
            summaryText.Text = "File A: " + result.Options.SheetA + " · " + result.RowsA.ToString("N0") + " records    File B: " + result.Options.SheetB + " · " + result.RowsB.ToString("N0") + " records\r\n"
                + "Matched: " + result.MatchedRows.ToString("N0") + "    Only A: " + result.OnlyA.ToString("N0") + "    Only B: " + result.OnlyB.ToString("N0") + "\r\n"
                + "Matching: " + (result.Options.UseKeys ? string.Join(" + ", result.Options.KeysA) + " → " + string.Join(" + ", result.Options.KeysB)
                    : "Data row position relative to each selected header row; no sorting")
                + ". Values/styles cover named columns from each selected header row; layout covers supported metadata. "
                + "Equality is limited to these selected checks, not byte-for-byte identity.";
            PopulatePage(true); PopulatePage(false);
            RefreshAvailability();
        }

        private void MovePage(bool data, int direction)
        {
            if (result == null) return;
            int count = (data ? result.DataDetails : result.OtherDetails).Count;
            int maximum = Math.Max(0, (count - 1) / PreviewPageSize);
            if (data) dataPage = Math.Max(0, Math.Min(maximum, dataPage + direction));
            else otherPage = Math.Max(0, Math.Min(maximum, otherPage + direction));
            PopulatePage(data);
        }

        private void PopulatePage(bool data)
        {
            var grid = data ? dataGrid : otherGrid;
            var caption = data ? dataPageLabel : otherPageLabel;
            var details = data ? result.DataDetails : result.OtherDetails;
            int start = (data ? dataPage : otherPage) * PreviewPageSize;
            grid.Rows.Clear();
            foreach (ExcelAuditFinding item in details.Skip(start).Take(PreviewPageSize))
                grid.Rows.Add(item.Category.ToString(), item.Difference, item.Key, item.ColumnA, item.ColumnB,
                    item.CellA, item.CellB, item.ValueA, item.ValueB, item.Details);
            long omitted = data ? result.OmittedData : result.OmittedOther;
            caption.Text = details.Count == 0 ? "No retained findings in this section." : "Showing " + (start + 1).ToString("N0") + "–" + Math.Min(start + PreviewPageSize, details.Count).ToString("N0") + " of " + details.Count.ToString("N0") + " retained findings.";
            caption.Text += " Total: " + (data ? result.DataCount : result.OtherCount).ToString("N0") + ".";
            if (omitted > 0) caption.Text += " " + omitted.ToString("N0") + " details omitted by the configured row/memory limits; total counts remain complete.";
            int offset = data ? 0 : 2;
            pageButtons[offset].Enabled = start > 0;
            pageButtons[offset + 1].Enabled = start + PreviewPageSize < details.Count;
        }

        private void OpenSavedReport()
        {
            if (string.IsNullOrEmpty(savedReport) || !File.Exists(savedReport)) { SetStatus("The saved report is no longer available. Export it again.", true); return; }
            try { Process.Start(new ProcessStartInfo(savedReport) { UseShellExecute = true }); }
            catch (Exception error) { SetStatus("The report is saved, but it could not be opened: " + error.Message, true); }
        }

        private void RefreshAvailability()
        {
            if (IsDisposed || Disposing) return;
            bool busy = worker.IsBusy || currentJob != null;
            if (busy) { cancel.Enabled = cancellation != null && !cancellation.IsCancellationRequested; return; }
            cancel.Enabled = false;
            keyMappings.Enabled = primaryKey.Checked;
            addKey.Enabled = primaryKey.Checked && sourceInfo != null && targetInfo != null;
            removeKey.Enabled = primaryKey.Checked && keyMappings.Rows.Count > 0;
            compare.Enabled = validate.Enabled = true;
            export.Enabled = result != null;
            openReport.Enabled = !string.IsNullOrEmpty(savedReport);
            if (result == null) foreach (Button button in pageButtons) button.Enabled = false;
        }

        private void SetStatus(string message, bool error = false)
        {
            if (IsDisposed || Disposing) return;
            phase.Text = message;
            phase.ForeColor = UiTheme.Readable(error ? Color.Red : Color.Black);
            AppendLog(message);
        }

        private void AppendLog(string message)
        {
            if (IsDisposed || Disposing) return;
            if (log.TextLength > 24000) log.Text = log.Text.Substring(log.TextLength - 16000);
            log.AppendText(DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture) + "  " + message + Environment.NewLine);
            log.SelectionStart = log.TextLength;
            log.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (worker.IsBusy)
            {
                e.Cancel = true;
                CancelCurrent();
                SetStatus("This task is still stopping. Close it again after the operation finishes.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cancellation != null) cancellation.Cancel();
                elapsedTimer.Stop(); elapsedTimer.Dispose();
                if (inputLock != null) { inputLock.Dispose(); inputLock = null; }
                if (!worker.IsBusy)
                {
                    worker.Dispose();
                    if (cancellation != null) { cancellation.Dispose(); cancellation = null; }
                }
            }
            base.Dispose(disposing);
        }

        private enum JobKind { LoadA, LoadB, Validate, Compare, Export }

        private sealed class AuditJob
        {
            internal JobKind Kind;
            internal string Path;
            internal string Sheet;
            internal int Header;
            internal string TempDirectory;
            internal ExcelAuditOptions Options;
            internal ExcelAuditResult Result;
            internal bool Overwrite;
            internal int Revision;
            internal CancellationToken Token;
        }
    }
}
