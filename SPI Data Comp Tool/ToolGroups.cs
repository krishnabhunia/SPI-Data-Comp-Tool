using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Related tools share one window, one tab per tool, instead of one window each:
    /// Pre-merging, Excel and file tools, Delta tools. Each tab holds the tool exactly as it was
    /// (same controls, same code); it is built the first time its tab is shown.
    /// Holding Ctrl while choosing a menu entry still opens that tool in a window of its own.
    /// </summary>
    internal static class ToolGroups
    {
        public const string PreMerging = "Pre-merging";
        public const string ExcelTools = "Excel and file tools";
        public const string Delta = "Delta tools";

        public static readonly string[] All = { PreMerging, ExcelTools, Delta };

        public static ToolGroupWindow Create(string group)
        {
            var window = new ToolGroupWindow(group);
            switch (group)
            {
                case PreMerging:
                    window.AddTool("Pre-merging tool", typeof(PreMergingTool_Form), "", () => new PreMergingTool_Form());
                    window.AddTool("Duplicacy check", typeof(Pre_Merging_Duplicacy_Form), "check", () => new Pre_Merging_Duplicacy_Form());
                    window.AddTool("Configuration", typeof(Pre_Merging_Duplicacy_Form), "configuration", () => new Pre_Merging_Duplicacy_Form(true));
                    break;
                case ExcelTools:
                    window.AddTool("Customised Excel", typeof(CustomisedExcel), "", () => new CustomisedExcel());
                    window.AddTool("Merge logs to Excel", typeof(MergeLogsExport), "", () => new MergeLogsExport());
                    window.AddTool("Spec notes", typeof(SPI_Spec_Note_Creation_Utility), "", () => new SPI_Spec_Note_Creation_Utility());
                    window.AddTool("Generate scripts", typeof(Generate_IUD_Script_Form), "", () => new Generate_IUD_Script_Form());
                    break;
                case Delta:
                    window.AddTool("Table extraction after delta", typeof(Table_Extraction_After_Delta_Form), "", () => new Table_Extraction_After_Delta_Form());
                    window.AddTool("Legacy ID", typeof(SPIDeltaChangeLegacyID), "", () => new SPIDeltaChangeLegacyID());
                    break;
                default:
                    window.Dispose();
                    throw new ArgumentException("Unknown tool group: " + group, nameof(group));
            }
            return window;
        }
    }

    /// <summary>One window with a tab per tool. A tool is an ordinary window of the app, shown inside its tab.</summary>
    internal sealed class ToolGroupWindow : Form
    {
        private sealed class Slot
        {
            public string Key;
            public Func<Form> Create;
            public Form Tool;
        }

        private readonly TabControl tabs;
        private bool closing;

        public ToolGroupWindow(string title)
        {
            Text = title;
            Font = new Font("Segoe UI", 9F);
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 600);
            MinimumSize = new Size(640, 420);
            Padding = new Padding(8);
            tabs = new TabControl { Name = "tabs_Tools", Dock = DockStyle.Fill };
            Controls.Add(tabs);
            tabs.SelectedIndexChanged += (sender, e) => { if (!closing) Fill(tabs.SelectedTab); };
            Load += (sender, e) => Fill(tabs.SelectedTab);   // the tab that is showing when the window opens
            FormClosing += CloseTools;
            UiTheme.Apply(this);
        }

        /// <summary>The key of a tool: its window type and the mode it is opened in.</summary>
        public static string KeyOf(Type tool, string mode) { return tool.FullName + ":" + (mode ?? string.Empty); }

        public void AddTool(string text, Type tool, string mode, Func<Form> create)
        {
            var page = new TabPage(text) { Name = "tab_" + tool.Name + (string.IsNullOrEmpty(mode) ? string.Empty : "_" + mode), Padding = new Padding(0) };
            page.Tag = new Slot { Key = KeyOf(tool, mode), Create = create };
            tabs.TabPages.Add(page);
        }

        public int ToolCount { get { return tabs.TabPages.Count; } }

        /// <summary>The tool on the tab if it has been built already; null otherwise.</summary>
        public Form ToolAt(int index) { return ((Slot)tabs.TabPages[index].Tag).Tool; }

        /// <summary>Shows the tab of the tool and returns the tool. <paramref name="create"/> builds it if this is its first use.</summary>
        public Form ShowTool(string key, Func<Form> create = null)
        {
            foreach (TabPage page in tabs.TabPages)
            {
                var slot = (Slot)page.Tag;
                if (slot.Key != key) continue;
                if (slot.Tool == null && create != null) slot.Create = create;
                tabs.SelectedTab = page;
                Fill(page);
                return slot.Tool;
            }
            throw new ArgumentException("This window has no tool " + key, nameof(key));
        }

        private void Fill(TabPage page)
        {
            if (page == null) return;
            var slot = (Slot)page.Tag;
            if (slot.Tool != null && !slot.Tool.IsDisposed) return;
            Form tool = slot.Create();
            try
            {
                Size wanted = tool.Size;
                Size least = tool.MinimumSize;
                tool.TopLevel = false;                       // a window inside the tab, not on the desktop
                tool.FormBorderStyle = FormBorderStyle.None;
                tool.Dock = DockStyle.Fill;
                UiTheme.EnsureApplied(tool);
                page.Controls.Add(tool);
                tool.FormClosed += (sender, e) => { if (ReferenceEquals(slot.Tool, sender)) slot.Tool = null; };
                slot.Tool = tool;
                MakeRoom(wanted, least);
                tool.Show();
            }
            catch
            {
                slot.Tool = null;
                tool.Dispose();
                throw;
            }
        }

        /// <summary>The window grows to the largest tool shown so far; it never shrinks under the user's hands.</summary>
        private void MakeRoom(Size wanted, Size least)
        {
            Size extra = new Size(Width - tabs.DisplayRectangle.Width, Height - tabs.DisplayRectangle.Height);
            Rectangle screen = Screen.FromControl(this).WorkingArea;
            MinimumSize = new Size(Math.Min(screen.Width, Math.Max(MinimumSize.Width, least.Width + extra.Width)),
                Math.Min(screen.Height, Math.Max(MinimumSize.Height, least.Height + extra.Height)));
            if (WindowState != FormWindowState.Normal) return;
            Size = new Size(Math.Min(screen.Width, Math.Max(Width, wanted.Width + extra.Width)),
                Math.Min(screen.Height, Math.Max(Height, wanted.Height + extra.Height)));
        }

        /// <summary>
        /// Closing the window closes its tools - all of them or none: every tool is asked first, and
        /// if one refuses (work in progress) the window stays open with every tab as it was.
        /// </summary>
        private void CloseTools(object sender, FormClosingEventArgs e)
        {
            closing = true;
            try
            {
                var open = new List<KeyValuePair<TabPage, Form>>();
                foreach (TabPage page in tabs.TabPages)
                {
                    Form tool = ((Slot)page.Tag).Tool;
                    if (tool != null && !tool.IsDisposed) open.Add(new KeyValuePair<TabPage, Form>(page, tool));
                }
                foreach (KeyValuePair<TabPage, Form> pair in open)
                {
                    if (!Refuses(pair.Value, e.CloseReason)) continue;
                    e.Cancel = true;            // nothing has been closed: show the tool that said no
                    tabs.SelectedTab = pair.Key;
                    return;
                }
                foreach (KeyValuePair<TabPage, Form> pair in open)
                {
                    pair.Value.Close();
                    if (pair.Value.IsDisposed) continue;
                    e.Cancel = true;            // it changed its answer between the question and the close
                    tabs.SelectedTab = pair.Key;
                    return;
                }
            }
            finally
            {
                closing = false;
            }
        }

        private static readonly System.Reflection.MethodInfo AskToClose = typeof(Form).GetMethod("OnFormClosing",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        /// <summary>Runs the tool's own "may I close?" handlers without closing it.</summary>
        private static bool Refuses(Form tool, CloseReason reason)
        {
            if (AskToClose == null) return false;   // cannot ask: the close itself will tell
            var question = new FormClosingEventArgs(reason, false);
            AskToClose.Invoke(tool, new object[] { question });
            return question.Cancel;
        }
    }
}
