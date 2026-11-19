using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Colour-theme checks on real windows: every window is built in every theme and each label,
    /// button, field and grid must stay readable (WCAG AA, 4.5:1); the theme must also switch while
    /// windows are open. No database, no user file: the saved choice is exercised on a temp file.
    /// Run by "SPI Data Comp Tool.exe" --check-themes OUT_FILE (see SelfTest.cs).
    /// </summary>
    internal static class ThemeChecks
    {
        private const double Readable = 4.5;

        public static int Run(string outFile)
        {
            var lines = new List<string>();
            int failed = 0;
            Action<string, Action> check = (name, body) =>
            {
                try
                {
                    body();
                    lines.Add("ok   " + name);
                }
                catch (Exception ex)
                {
                    Exception cause = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
                    failed++;
                    lines.Add("FAIL " + name + ": " + cause.Message.Replace("\r", " ").Replace("\n", " "));
                }
            };

            try
            {
                check("themes: four, Ocean first, only Midnight dark", Themes);
                check("saved choice: missing, unknown, oversized, round trip", SavedChoice);
                check("Source and Target are recognised by name", Sides);
                check("status colours follow the theme", StatusColours);
                Type[] windows = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(Form)) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null)
                    .OrderBy(t => t.Name).ToArray();
                foreach (UiPalette palette in UiTheme.All)
                {
                    UiTheme.Use(palette, false);
                    foreach (Type window in windows)
                    {
                        UiPalette theme = palette;
                        Type type = window;
                        check(theme.Name + ": " + type.Name + " is readable", () =>
                        {
                            using (var form = (Form)Activator.CreateInstance(type))
                            {
                                UiTheme.EnsureApplied(form);
                                Require(Inspect(form, theme));
                            }
                        });
                    }
                }
                check("the theme switches while windows are open", LiveSwitch);
                check("two Settings windows show the same theme", SettingsPickers);
            }
            finally
            {
                UiTheme.Use(UiTheme.All[0], false);
            }
            lines.Add(failed == 0 ? "All theme checks passed." : failed + " theme check(s) failed.");
            File.WriteAllLines(outFile, lines);
            return failed == 0 ? 0 : 1;
        }

        private static void Require(string problems)
        {
            if (problems.Length > 0) throw new InvalidOperationException(problems);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void Themes()
        {
            Require(UiTheme.All.Length == 4 && UiTheme.All[0] == UiTheme.Ocean, "Ocean must be the first (default) of four themes.");
            Require(UiTheme.All.Select(p => p.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() == 4, "Theme names must differ.");
            Require(UiTheme.All.Count(p => p.Dark) == 1 && UiTheme.Midnight.Dark, "Only Midnight is a dark theme.");
            foreach (UiPalette p in UiTheme.All)
                Require(UiTheme.Find(" " + p.Name.ToUpperInvariant() + " ") == p, p.Name + " is not found by its name.");
            Require(UiTheme.Find("no such theme") == null && UiTheme.Find(null) == null, "An unknown name must not find a theme.");
        }

        private static void SavedChoice()
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-theme-" + Guid.NewGuid().ToString("N"));
            string file = Path.Combine(folder, "nested", "theme.txt");
            try
            {
                Require(UiTheme.ReadChoice(file) == UiTheme.Ocean, "A missing file must give the default theme.");
                foreach (UiPalette p in UiTheme.All)
                {
                    UiTheme.SaveChoice(file, p);
                    Require(UiTheme.ReadChoice(file) == p, p.Name + " was not read back.");
                }
                File.WriteAllText(file, "Sunset");
                Require(UiTheme.ReadChoice(file) == UiTheme.Ocean, "An unknown theme name must give the default theme.");
                File.WriteAllText(file, new string('x', 5000));
                Require(UiTheme.ReadChoice(file) == UiTheme.Ocean, "An oversized file must give the default theme.");
                File.WriteAllText(file, "  midnight\r\n");
                Require(UiTheme.ReadChoice(file) == UiTheme.Midnight, "The name is read without regard to case or spaces.");
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        private static void Sides()
        {
            var expected = new Dictionary<string, int>
            {
                { "Source · Oracle", 1 }, { "Source : Oracle Connection and Settings", 1 }, { "SRC_ID", 1 }, { "Only in Source", 1 },
                { "Target · SQL", 2 }, { "Target : SQL Connection and Settings", 2 }, { "TRG_ID", 2 }, { "Only in Target", 2 },
                { "Resource list", 0 }, { "Source and Target", 0 }, { "Connection", 0 }, { "", 0 },
            };
            foreach (KeyValuePair<string, int> pair in expected)
                Require(UiTheme.SideOf(pair.Key) == pair.Value, "\"" + pair.Key + "\" was not recognised as side " + pair.Value + ".");
        }

        private static void StatusColours()
        {
            UiPalette before = UiTheme.Current;
            try
            {
                foreach (UiPalette p in UiTheme.All)
                {
                    UiTheme.Use(p, false);
                    Require(UiTheme.Readable(Color.Black) == p.TextColor && UiTheme.Readable(Color.Red) == p.Danger
                        && UiTheme.Readable(Color.DarkRed) == p.Danger && UiTheme.Readable(Color.DarkGreen) == p.Success
                        && UiTheme.Readable(Color.DarkBlue) == p.Info && UiTheme.Readable(Color.Blue) == p.Info,
                        p.Name + ": a status colour is not mapped to the theme.");
                    Require(UiTheme.Readable(Color.Yellow) == Color.Yellow, p.Name + ": a colour with no meaning must be kept.");
                    foreach (UiPalette other in UiTheme.All)
                        Require(UiTheme.Readable(other.Danger) == p.Danger && UiTheme.Readable(other.TextColor) == p.TextColor
                            && UiTheme.Readable(other.MutedText) == p.MutedText,
                            p.Name + ": a colour taken from " + other.Name + " does not follow the theme.");
                }
            }
            finally
            {
                UiTheme.Use(before, false);
            }
        }

        private static void LiveSwitch()
        {
            UiTheme.Use(UiTheme.Ocean, false);
            using (var main = new MainForm())
            using (var tool = new SQL_DB_To_Excel())
            {
                UiTheme.EnsureApplied(tool);
                foreach (Form form in new Form[] { main, tool })
                {
                    form.ShowInTaskbar = false;
                    form.Show();
                }
                Pump();
                Label error = new Label { Name = "lbl_ThemeProbe", Text = "probe", AutoSize = true };
                tool.Controls.Add(error);            // a control added later is themed too
                error.ForeColor = Color.Red;         // and a colour set later follows the theme
                foreach (UiPalette p in new[] { UiTheme.Midnight, UiTheme.Berry, UiTheme.Forest, UiTheme.Ocean })
                {
                    UiTheme.Use(p, false);
                    Pump();
                    Require(error.ForeColor == p.Danger, p.Name + ": an error label kept the colour of the previous theme.");
                    Require(Inspect(main, p));
                    Require(Inspect(tool, p));
                }
                // a label set back to ordinary text must not turn red again on the next theme change
                error.ForeColor = UiTheme.TextColor;
                UiTheme.Use(UiTheme.Midnight, false);
                Require(error.ForeColor == UiTheme.Midnight.TextColor, "A label that was reset to ordinary text became an error again.");
                UiTheme.Use(UiTheme.Ocean, false);
                tool.Close();
                main.Close();
            }
        }

        private static void SettingsPickers()
        {
            UiTheme.Use(UiTheme.Ocean, false);
            using (var first = new User_Settings())
            using (var second = new User_Settings(true))
            {
                var firstPicker = (ComboBox)first.Controls.Find("cb_Theme", true).Single();
                var secondPicker = (ComboBox)second.Controls.Find("cb_Theme", true).Single();
                secondPicker.SelectedItem = UiTheme.Midnight;      // the user picks Midnight in the second window
                Require(UiTheme.Current == UiTheme.Midnight, "Picking a theme in Settings did not apply it.");
                Require(firstPicker.SelectedItem == UiTheme.Midnight, "The other Settings window still shows the old theme.");
                firstPicker.SelectedItem = UiTheme.Ocean;          // and can go back from the first one
                Require(UiTheme.Current == UiTheme.Ocean && secondPicker.SelectedItem == UiTheme.Ocean,
                    "Going back to Ocean from the other Settings window did not work.");
            }
            UiTheme.Use(UiTheme.Berry, false);   // a closed Settings window no longer listens
        }

        private static void Pump()
        {
            for (int i = 0; i < 4; i++)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(40);
            }
        }

        // ---------------- reading a window ----------------

        /// <summary>Every problem found in the window for the theme; empty when it is readable.</summary>
        internal static string Inspect(Form form, UiPalette p)
        {
            var problems = new List<string>();
            if (form.BackColor != p.WindowBack) problems.Add("the window is not in the theme's colour");
            Walk(form, p, problems);
            if (problems.Count == 0) return string.Empty;
            return problems.Count + " problem(s): " + string.Join("; ", problems.Take(6));
        }

        private static void Walk(Control parent, UiPalette p, List<string> problems)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TileButton || c is GlyphIcon || c is ThemeSwatch || c is ToolStrip) continue;   // drawn from the theme itself
                bool enabled = EnabledWithParents(c);
                if (c is Button)
                {
                    if (((Button)c).FlatStyle != FlatStyle.Flat) problems.Add(Name(c) + " is not a themed button");
                    if (enabled) Pair(c, c.ForeColor, c.BackColor, problems);
                    continue;
                }
                if (c is DataGridView)
                {
                    var grid = (DataGridView)c;
                    Pair(c, grid.DefaultCellStyle.ForeColor, grid.DefaultCellStyle.BackColor, problems);
                    Pair(c, grid.ColumnHeadersDefaultCellStyle.ForeColor, grid.ColumnHeadersDefaultCellStyle.BackColor, problems);
                    Pair(c, grid.DefaultCellStyle.SelectionForeColor, grid.DefaultCellStyle.SelectionBackColor, problems);
                    continue;
                }
                if (c is TextBoxBase || c is ListBox || c is ComboBox || c is ListView || c is TreeView || c is NumericUpDown)
                {
                    if (enabled) Pair(c, c.ForeColor, c.BackColor, problems);
                    continue;
                }
                if (c is LinkLabel)
                {
                    if (enabled) Pair(c, ((LinkLabel)c).LinkColor, Backdrop(c), problems);
                }
                else if (c is Label || c is CheckBox || c is RadioButton || c is GroupBox)
                {
                    if (enabled && c.Text.Trim().Length > 0) Pair(c, c.ForeColor, Backdrop(c), problems);
                }
                if (c.HasChildren) Walk(c, p, problems);
            }
        }

        private static bool EnabledWithParents(Control c)
        {
            for (Control at = c; at != null; at = at.Parent) if (!at.Enabled) return false;
            return true;
        }

        /// <summary>The first colour behind the control that is not see-through.</summary>
        private static Color Backdrop(Control c)
        {
            for (Control at = c; at != null; at = at.Parent)
                if (at.BackColor.A == 255) return at.BackColor;
            return SystemColors.Control;
        }

        private static string Name(Control c)
        {
            return string.IsNullOrEmpty(c.Name) ? c.GetType().Name + " \"" + c.Text + "\"" : c.Name;
        }

        private static void Pair(Control c, Color text, Color back, List<string> problems)
        {
            double ratio = Contrast(text, back);
            if (ratio < Readable)
                problems.Add(Name(c) + " " + Hex(text) + " on " + Hex(back) + " is " + ratio.ToString("0.0") + ":1");
        }

        private static string Hex(Color c) { return "#" + c.R.ToString("x2") + c.G.ToString("x2") + c.B.ToString("x2"); }

        internal static double Contrast(Color a, Color b)
        {
            double first = Luminance(a), second = Luminance(b);
            return (Math.Max(first, second) + 0.05) / (Math.Min(first, second) + 0.05);
        }

        private static double Luminance(Color c)
        {
            return 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);
        }

        private static double Channel(byte value)
        {
            double v = value / 255.0;
            return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }
    }
}
