using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// One rail task with a mode bar (redesign C2): "Compare databases" (one table / a list of tables) and
    /// "Compare workbooks" (Values / Values and formatting / Identical? × One pair / Two folders).
    /// Each mode is an existing workflow page, created when it is first chosen and kept while switching,
    /// so its inputs, results and running work survive. There is no other way to open those workflows.
    /// </summary>
    public sealed class CompareModePage : WorkspacePageForm
    {
        internal sealed class Axis
        {
            internal Axis(string key, string label, params KeyValuePair<string, string>[] options)
            {
                Key = key;
                Label = label;
                Options = options;
            }

            internal string Key { get; }
            internal string Label { get; }
            internal IReadOnlyList<KeyValuePair<string, string>> Options { get; }
        }

        private readonly Axis[] axes;
        private readonly Func<string[], string> resolve;
        private readonly Func<string, WorkspacePageForm> create;
        private readonly string[] selection;
        private readonly Dictionary<string, RadioButton> choices = new Dictionary<string, RadioButton>();
        private readonly Dictionary<string, WorkspacePageForm> modes = new Dictionary<string, WorkspacePageForm>();
        private readonly Dictionary<string, string[]> selectionOf = new Dictionary<string, string[]>();
        private readonly Panel content = new Panel { Name = "pnl_ModeContent", Dock = DockStyle.Fill, Margin = new Padding(0) };
        private readonly Label status = new Label { Name = "lbl_ModeStatus", AutoSize = true, Margin = new Padding(3, 0, 3, 6), UseMnemonic = false };
        private bool switching, forwardingDialogKey;

        private CompareModePage(string title, string hint, Axis[] axes, Func<string[], string> resolve, Func<string, WorkspacePageForm> create)
        {
            this.axes = axes;
            this.resolve = resolve;
            this.create = create;
            selection = axes.Select(axis => axis.Options[0].Key).ToArray();
            Text = title;
            Name = "CompareModePage";
            ClientSize = new Size(1100, 720);

            var root = new TableLayoutPanel { Name = "tlp_ModeRoot", Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = new Padding(0), Padding = new Padding(8, 6, 8, 0) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var bar = new FlowLayoutPanel { Name = "flp_ModeBar", AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = new Padding(0) };
            for (int index = 0; index < axes.Length; index++)
            {
                Axis axis = axes[index];
                int axisIndex = index;
                var group = new FlowLayoutPanel { Name = "flp_Mode_" + axis.Key, AutoSize = true, WrapContents = false, Margin = new Padding(0, 0, 24, 4) };
                if (!string.IsNullOrEmpty(axis.Label))
                    group.Controls.Add(new Label { Text = axis.Label, AutoSize = true, Margin = new Padding(0, 9, 8, 0), Font = new Font(Font, FontStyle.Bold), UseMnemonic = false });
                foreach (var option in axis.Options)
                {
                    string optionKey = option.Key;
                    var choice = new RadioButton
                    {
                        Name = ChoiceName(axis.Key, optionKey), Text = option.Value, Appearance = Appearance.Button, AutoSize = true,
                        MinimumSize = new Size(96, 32), TextAlign = ContentAlignment.MiddleCenter, Margin = new Padding(0, 0, 4, 0),
                        Checked = optionKey == selection[axisIndex], AccessibleName = axis.Label + " " + option.Value,
                        UseMnemonic = false,
                    };
                    choice.CheckedChanged += (sender, args) =>
                    {
                        if (switching || !choice.Checked) return;
                        selection[axisIndex] = optionKey;
                        ShowSelection();
                    };
                    choices.Add(choice.Name, choice);
                    group.Controls.Add(choice);
                }
                bar.Controls.Add(group);
            }
            root.Controls.Add(bar, 0, 0);
            status.Text = hint ?? string.Empty;
            status.Visible = !string.IsNullOrEmpty(hint);
            root.Controls.Add(status, 0, 1);
            root.Controls.Add(content, 0, 2);
            Controls.Add(root);
            UiTheme.Apply(this);
        }

        internal static string ChoiceName(string axis, string option) => "mode_" + axis + "_" + option.Replace('-', '_');

        internal static CompareModePage Databases(Func<string, WorkspacePageForm> create)
        {
            return new CompareModePage("Compare databases", null,
                new[] { new Axis("scope", string.Empty, Option("table", "One table"), Option("list", "A list of tables")) },
                picked => picked[0] == "table" ? "comparison.database" : "comparison.batch", create);
        }

        internal static CompareModePage Workbooks(Func<string, WorkspacePageForm> create)
        {
            return new CompareModePage("Compare workbooks",
                "Values: keys and columns you choose. Values and formatting: also fonts, fills, merged cells and layout. " +
                "Identical?: yes or no with a five-sheet report. Two folders compares values only.",
                new[]
                {
                    new Axis("check", "Check", Option("values", "Values"), Option("formatting", "Values and formatting"), Option("identical", "Identical?")),
                    new Axis("files", "Files", Option("pair", "One pair"), Option("folders", "Two folders")),
                },
                picked =>
                {
                    if (picked[1] == "folders") return picked[0] == "values" ? "comparison.batch-folders" : null;
                    switch (picked[0])
                    {
                        case "values": return "comparison.excel";
                        case "formatting": return "comparison.excel-audit";
                        default: return "comparison.identity";
                    }
                }, create);
        }

        private static KeyValuePair<string, string> Option(string key, string label) => new KeyValuePair<string, string>(key, label);

        internal WorkspacePageForm ActiveMode { get; private set; }
        internal string ActiveModeKey { get; private set; }
        internal IReadOnlyList<WorkspacePageForm> LoadedModes => modes.Values.Where(page => !page.IsDisposed).ToList().AsReadOnly();

        /// <summary>Chooses one option per axis, as a click on the mode bar would, and returns the page shown.</summary>
        internal WorkspacePageForm Choose(params string[] options)
        {
            if (options == null || options.Length != axes.Length) throw new ArgumentException("Choose one option for each mode axis.", nameof(options));
            if (resolve(options) == null) throw new ArgumentException("This combination of modes is not available.", nameof(options));
            switching = true;
            try
            {
                for (int i = 0; i < axes.Length; i++)
                {
                    selection[i] = options[i];
                    choices[ChoiceName(axes[i].Key, options[i])].Checked = true;
                }
            }
            finally { switching = false; }
            ShowSelection();
            return ActiveMode;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ActiveMode == null) ShowSelection();
        }

        private void ShowSelection()
        {
            UpdateAvailability();
            string key = resolve(selection);
            if (key == null) return;
            WorkspacePageForm page;
            if (!modes.TryGetValue(key, out page) || page.IsDisposed)
            {
                page = create(key);
                try
                {
                    UiTheme.EnsureApplied(page);   // like a task page: every mode is in the colour theme
                    page.WindowState = FormWindowState.Normal;
                    page.TopLevel = false;
                    page.FormBorderStyle = FormBorderStyle.None;
                    page.ShowInTaskbar = false;
                    page.MinimumSize = Size.Empty;
                    page.Dock = DockStyle.Fill;
                    page.AutoScroll = true;
                    page.HostWorkspace = HostWorkspace;
                    modes[key] = page;
                    selectionOf[key] = (string[])selection.Clone();
                    content.Controls.Add(page);
                    string captured = key;
                    WorkspacePageForm capturedPage = page;
                    page.Disposed += (sender, args) => RemoveMode(captured, capturedPage);
                    page.FormClosed += (sender, args) => RemoveMode(captured, capturedPage);
                    page.Show();
                }
                catch
                {
                    modes.Remove(key);
                    content.Controls.Remove(page);
                    page.Dispose();
                    throw;
                }
            }
            foreach (var other in modes.Values.ToArray()) if (!other.IsDisposed) other.Visible = ReferenceEquals(other, page);
            ActiveMode = page;
            ActiveModeKey = key;
            page.BringToFront();
            page.Select();
        }

        /// <summary>An option that cannot combine with the other axes' choices is disabled, never silently ignored.</summary>
        private void UpdateAvailability()
        {
            for (int i = 0; i < axes.Length; i++)
                foreach (var option in axes[i].Options)
                {
                    var trial = (string[])selection.Clone();
                    trial[i] = option.Key;
                    choices[ChoiceName(axes[i].Key, option.Key)].Enabled = resolve(trial) != null;
                }
        }

        private void RemoveMode(string key, WorkspacePageForm page)
        {
            WorkspacePageForm current;
            if (!modes.TryGetValue(key, out current) || !ReferenceEquals(current, page)) return;
            modes.Remove(key);
            if (ReferenceEquals(ActiveMode, page))
            {
                ActiveMode = null;
                ActiveModeKey = null;
            }
        }

        internal override void OnHostWorkspaceChanged()
        {
            // Advanced mapping and other workspace-level actions of a mode use the task's workspace.
            foreach (var page in modes.Values) if (!page.IsDisposed) page.HostWorkspace = HostWorkspace;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.Cancel || CompletingWorkspaceClose) return;
            foreach (var pair in modes.ToArray())
            {
                if (pair.Value.IsDisposed) continue;
                if (pair.Value.PrepareWorkspaceClose(e.CloseReason)) continue;
                e.Cancel = true;
                string[] picked;
                if (selectionOf.TryGetValue(pair.Key, out picked)) Choose(picked);
                return;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            foreach (var page in modes.Values.ToArray()) page.CompleteWorkspaceClose();
            base.OnFormClosed(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (!forwardingDialogKey && ActiveMode != null && !ActiveMode.IsDisposed && (key == Keys.Enter || key == Keys.Escape))
            {
                forwardingDialogKey = true;
                try { if (ActiveMode.ProcessWorkspaceDialogKey(keyData)) return true; }
                finally { forwardingDialogKey = false; }
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
