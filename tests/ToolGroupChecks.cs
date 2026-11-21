using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Group windows (ToolGroups.cs) on real controls: every tab builds its tool inside the tab, a
    /// tool is built once, the same tool is found again by its key, and closing the window closes
    /// its tools. No database and no file is touched.
    /// Run by "SPI Data Comp Tool.exe" --check-groups OUT_FILE (see SelfTest.cs).
    /// </summary>
    internal static class ToolGroupChecks
    {
        public static int Run(string outFile)
        {
            var lines = new List<string>();
            int failed = 0;
            foreach (string group in ToolGroups.All)
            {
                try
                {
                    Check(group);
                    lines.Add("ok   " + group);
                }
                catch (Exception ex)
                {
                    failed++;
                    lines.Add("FAIL " + group + ": " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                }
            }
            try
            {
                BusyToolKeepsEveryTab();
                lines.Add("ok   a busy tool keeps the window and every tab");
            }
            catch (Exception ex)
            {
                failed++;
                lines.Add("FAIL a busy tool keeps the window and every tab: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }
            try
            {
                ProfilesInSettings();
                lines.Add("ok   Settings holds User profiles as a tab");
            }
            catch (Exception ex)
            {
                failed++;
                lines.Add("FAIL Settings holds User profiles as a tab: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }
            lines.Add(failed == 0 ? "All group window checks passed." : failed + " group window check(s) failed.");
            File.WriteAllLines(outFile, lines);
            return failed == 0 ? 0 : 1;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void Check(string group)
        {
            var tools = new List<Form>();
            using (ToolGroupWindow window = ToolGroups.Create(group))
            {
                Require(window.Text == group && window.ToolCount >= 2, "A group window needs its name and at least two tools.");
                var tabs = (TabControl)window.Controls.Find("tabs_Tools", true).Single();
                window.ShowInTaskbar = false;
                window.Show();
                Application.DoEvents();
                for (int i = 0; i < window.ToolCount; i++)
                {
                    tabs.SelectedIndex = i;
                    Application.DoEvents();
                    Form tool = window.ToolAt(i);
                    Require(tool != null && !tool.IsDisposed, "Tab " + tabs.TabPages[i].Text + " did not build its tool.");
                    Require(!tool.TopLevel && tool.Parent == tabs.TabPages[i] && tool.FormBorderStyle == FormBorderStyle.None
                        && tool.Dock == DockStyle.Fill && tool.Visible, tabs.TabPages[i].Text + " is not shown inside its tab.");
                    Require(tool.BackColor == UiTheme.Current.WindowBack, tabs.TabPages[i].Text + " is not in the colour theme.");
                    tools.Add(tool);
                }
                Require(tools.Distinct().Count() == tools.Count, "Two tabs share one tool.");
                // going back to a tab keeps its tool (and whatever the user typed in it)
                tabs.SelectedIndex = 0;
                Application.DoEvents();
                Require(ReferenceEquals(window.ToolAt(0), tools[0]), "Returning to a tab built its tool again.");
                // asking for a tool by its key shows the same one and never builds a second
                int built = 0;
                Form again = window.ShowTool(ToolGroupWindow.KeyOf(tools[1].GetType(), ModeOf(tabs.TabPages[1])),
                    () => { built++; return (Form)Activator.CreateInstance(tools[1].GetType()); });
                Require(ReferenceEquals(again, tools[1]) && built == 0 && tabs.SelectedIndex == 1, "Opening a tool again did not return to its tab.");
                bool unknown = false;
                try { window.ShowTool("no such tool"); }
                catch (ArgumentException) { unknown = true; }
                Require(unknown, "An unknown tool was accepted.");
                Require(window.Width <= Screen.FromControl(window).WorkingArea.Width && window.Height <= Screen.FromControl(window).WorkingArea.Height,
                    "The group window is larger than the screen.");
                window.Close();
                Require(window.IsDisposed || !window.Visible, "The group window did not close.");
            }
            Require(tools.All(tool => tool.IsDisposed), "Closing the group window left a tool open.");
        }

        /// <summary>A stand-in tool that refuses to close while it is "busy".</summary>
        private sealed class ProbeTool : Form
        {
            public bool Busy;
            public int Asked;
            protected override void OnFormClosing(FormClosingEventArgs e)
            {
                Asked++;
                if (Busy) e.Cancel = true;
                base.OnFormClosing(e);
            }
        }

        private static void BusyToolKeepsEveryTab()
        {
            var first = new ProbeTool();
            var busy = new ProbeTool { Busy = true };
            var last = new ProbeTool();
            using (var window = new ToolGroupWindow("Probe"))
            {
                window.AddTool("First", typeof(ProbeTool), "first", () => first);
                window.AddTool("Busy", typeof(ProbeTool), "busy", () => busy);
                window.AddTool("Last", typeof(ProbeTool), "last", () => last);
                var tabs = (TabControl)window.Controls.Find("tabs_Tools", true).Single();
                window.ShowInTaskbar = false;
                window.Show();
                for (int i = 0; i < 3; i++) { tabs.SelectedIndex = i; Application.DoEvents(); }
                tabs.SelectedIndex = 0;
                window.Close();
                Application.DoEvents();
                Require(!window.IsDisposed && window.Visible, "The window closed although a tool was busy.");
                Require(!first.IsDisposed && !busy.IsDisposed && !last.IsDisposed && ReferenceEquals(window.ToolAt(0), first)
                    && ReferenceEquals(window.ToolAt(2), last), "A tab lost its tool although the window stayed open.");
                Require(tabs.SelectedIndex == 1, "The busy tool was not shown.");
                busy.Busy = false;
                window.Close();
                Application.DoEvents();
                Require(first.IsDisposed && busy.IsDisposed && last.IsDisposed, "Closing the window left a tool open.");
            }
        }

        private static void ProfilesInSettings()
        {
            UserProfileForm profiles;
            using (var settings = new User_Settings())
            {
                var tabs = (TabControl)settings.Controls.Find("tabs_Settings", true).Single();
                var log = (GroupBox)settings.Controls.Find("gb_Logs", true).Single();
                settings.ShowInTaskbar = false;
                settings.Show();
                Application.DoEvents();
                Require(tabs.SelectedTab.Name == "tab_General" && log.Visible, "Settings did not open on General with its log.");
                profiles = settings.ShowProfiles();
                Application.DoEvents();
                Require(tabs.SelectedTab.Name == "tab_Profiles" && profiles != null && !profiles.TopLevel && profiles.Visible
                    && profiles.FormBorderStyle == FormBorderStyle.None && profiles.Dock == DockStyle.Fill
                    && settings.Controls.Find("tlp_Profiles", true).Single().Controls.Contains(profiles),
                    "User profiles is not shown inside the Profiles tab.");
                Require(!log.Visible && settings.AcceptButton == null, "The Profiles tab did not get the whole window.");
                Require(settings.Width <= Screen.FromControl(settings).WorkingArea.Width
                    && settings.Height <= Screen.FromControl(settings).WorkingArea.Height, "Settings grew beyond the screen.");
                tabs.SelectedIndex = 0;
                Application.DoEvents();
                Require(log.Visible, "Leaving Profiles did not bring the Settings log back.");
                Require(ReferenceEquals(settings.ShowProfiles(), profiles), "Returning to Profiles built the window again.");
                settings.Close();
            }
            Require(profiles.IsDisposed, "Closing Settings left User profiles open.");
        }

        private static string ModeOf(TabPage page)
        {
            int at = page.Name.LastIndexOf('_');
            string tail = at >= 0 ? page.Name.Substring(at + 1) : string.Empty;
            return tail == "check" || tail == "configuration" ? tail : string.Empty;
        }
    }
}
