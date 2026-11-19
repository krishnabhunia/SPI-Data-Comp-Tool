using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Checks of the shared input checker, run by CI as "exe --check-inputs FILE". Uses a window made here in
    /// memory with made-up values; it never connects to a database.
    /// </summary>
    internal static class InputCheckChecks
    {
        public static int Run(string outputFile)
        {
            var lines = new List<string>();
            try
            {
                Fields(lines);
                Connections(lines);
                Reporting(lines);
                File.WriteAllLines(outputFile, lines);
                return 0;
            }
            catch (Exception ex)
            {
                lines.Add("FAIL " + ex.Message);
                File.WriteAllLines(outputFile, lines);
                return 1;
            }
        }

        private static void Check(bool condition, string what, List<string> lines)
        {
            if (!condition) throw new InvalidOperationException(what);
            lines.Add("PASS " + what);
        }

        private static TextBox Box(Form form, string text, bool enabled = true)
        {
            var box = new TextBox { Text = text, Enabled = enabled, Width = 200, Top = form.Controls.Count * 26 };
            form.Controls.Add(box);
            return box;
        }

        private static void Fields(List<string> lines)
        {
            using (var form = new Form())
            {
                Check(InputCheck.For(form).Required(Box(form, "x"), "Table name").Passed, "a filled field passes", lines);
                InputCheck empty = InputCheck.For(form).Required(Box(form, "   "), "Table name");
                Check(!empty.Passed && empty.Messages[0] == "Table name is empty.", "spaces alone count as empty, and the reason names the field", lines);
                Check(InputCheck.For(form).Required(Box(form, "", false), "Table name").Passed, "a switched-off field is not checked", lines);

                Check(InputCheck.For(form).Port(Box(form, "1521"), "Source port").Passed && InputCheck.For(form).Port(Box(form, "65535"), "p").Passed, "ports 1521 and 65535 pass", lines);
                InputCheck port = InputCheck.For(form).Port(Box(form, "70000"), "Source port");
                Check(port.Messages.Single() == "Source port must be a number from 1 to 65535 (it is 70000).", "a port above 65535 is refused with the range and the value", lines);
                Check(!InputCheck.For(form).Port(Box(form, "0"), "p").Passed && !InputCheck.For(form).Port(Box(form, "15a"), "p").Passed
                      && InputCheck.For(form).Port(Box(form, ""), "Source port").Messages.Single() == "Source port is empty.", "0, letters and an empty port are refused", lines);

                string file = Path.Combine(Path.GetTempPath(), "spi-input-check-" + Guid.NewGuid().ToString("N") + ".xlsx");
                File.WriteAllText(file, "x");
                try
                {
                    Check(InputCheck.For(form).FileExists(Box(form, file), "Excel file", ".xlsx", ".xls").Passed, "a file that exists with the right type passes", lines);
                    Check(InputCheck.For(form).FileExists(Box(form, file), "Script file", ".sql").Messages.Single() == "Script file must be a .sql file.", "the wrong file type is refused", lines);
                    Check(InputCheck.For(form).FolderExists(Box(form, Path.GetTempPath()), "Output folder").Passed, "a folder that exists passes", lines);
                }
                finally
                {
                    File.Delete(file);
                }
                Check(InputCheck.For(form).FileExists(Box(form, file), "Excel file").Messages.Single().StartsWith("Excel file was not found: "), "a file that is not there is refused", lines);
                Check(InputCheck.For(form).FileExists(Box(form, ""), "Excel file").Messages.Single() == "Excel file is not chosen."
                      && !InputCheck.For(form).FolderExists(Box(form, "Q:\\no\\such|folder"), "Output folder").Passed, "no file chosen and an impossible folder are refused", lines);

                Check(InputCheck.For(form).ValidName(Box(form, "dbo.COMPONENT_1"), "Table name").Passed && InputCheck.For(form).ValidName(Box(form, "[dbo].[Line]"), "t").Passed, "ordinary table names pass", lines);
                Check(!InputCheck.For(form).ValidName(Box(form, "LINE; DROP TABLE X"), "Table name").Passed && !InputCheck.For(form).ValidName(Box(form, "1LINE"), "t").Passed
                      && !InputCheck.For(form).ValidName(Box(form, "a'b"), "t").Passed, "a name with spaces, quotes, ; or a leading digit is refused", lines);
                Check(InputCheck.For(form).ValidFileName(Box(form, "Delta_2026-11"), "Output name").Passed && !InputCheck.For(form).ValidFileName(Box(form, "a/b:c"), "Output name").Passed, "a file name with / or : is refused", lines);

                var ticks = new CheckedListBox(); ticks.Items.Add("A"); form.Controls.Add(ticks);
                Check(InputCheck.For(form).AnyChosen(ticks, "the table list").Messages.Single() == "Nothing is chosen in the table list.", "a list with nothing ticked is refused", lines);
                ticks.SetItemChecked(0, true);
                Check(InputCheck.For(form).AnyChosen(ticks, "the table list").Passed, "a list with a ticked item passes", lines);
            }
        }

        private static void Connections(List<string> lines)
        {
            using (var form = new Form())
            {
                TextBox port = Box(form, "1521"), source = Box(form, "srv"), db = Box(form, "SPI"), user = Box(form, "u"), pwd = Box(form, "p");
                Check(InputCheck.For(form).Connection("Source", true, false, port, source, db, user, pwd).Passed, "a complete Oracle connection passes", lines);
                port.Text = ""; pwd.Text = "";
                InputCheck oracle = InputCheck.For(form).Connection("Source", true, false, port, source, db, user, pwd);
                Check(oracle.Messages.SequenceEqual(new[] { "Source port is empty.", "Source password is empty." }), "Oracle needs its port and password", lines);

                source.Text = ""; db.Text = ""; user.Text = "";
                InputCheck windows = InputCheck.For(form).Connection("Target", false, true, port, source, db, user, pwd);
                Check(windows.Messages.SequenceEqual(new[] { "Target data source is empty.", "Target database is empty." }),
                      "SQL Server with Windows authentication needs only data source and database (no port, user or password)", lines);
                InputCheck sql = InputCheck.For(form).Connection("Target", false, false, port, source, db, user, pwd);
                Check(sql.Messages.Count == 4 && sql.Messages[3] == "Target password is empty. Fill it in, or tick Windows authentication.",
                      "SQL Server without Windows authentication also needs user name and password", lines);

                using (var panel = new ConnectionPanel())
                {
                    form.Controls.Add(panel);
                    panel.Mode = ConnectionMode.SourceAndTarget;
                    InputCheck both = panel.AddChecks(InputCheck.For(form));
                    Check(both.Messages.Any(m => m.StartsWith("Source ")) && both.Messages.Any(m => m.StartsWith("Target ")), "an empty Connection panel reports Source and Target", lines);
                    Check(panel.AddChecks(InputCheck.For(form), target: false).Messages.All(m => m.StartsWith("Source ")), "a Source-only check leaves Target alone", lines);
                    panel.Mode = ConnectionMode.SourceOnly;
                    Check(panel.AddChecks(InputCheck.For(form)).Messages.All(m => m.StartsWith("Source ")), "a Source-only panel never checks Target", lines);
                }
            }
        }

        private static void Reporting(List<string> lines)
        {
            using (var form = new Form { ShowInTaskbar = false })
            {
                TextBox good = Box(form, "ok"), bad1 = Box(form, ""), bad2 = Box(form, "70000");
                var log = new RichTextBox { Name = "rtb_Log", Top = 120, Width = 300, Height = 80 };
                var status = new Label { Top = 210, Width = 300 };
                form.Controls.Add(log);
                form.Controls.Add(status);
                form.Show();
                Application.DoEvents();

                bool started = InputCheck.For(form).Required(good, "Database").Required(bad1, "Data source").Port(bad2, "Port").Report(status);
                Application.DoEvents();
                Check(!started, "with a wrong input the action does not start", lines);
                Check(log.Text.Contains("Data source is empty.") && log.Text.Contains("Port must be a number from 1 to 65535 (it is 70000).")
                      && log.Text.Contains("2 inputs need fixing. Nothing was started."), "every reason and the count are written to the window's log", lines);
                Check(status.Text == "2 inputs need fixing. Nothing was started.", "the status line shows the count", lines);
                Check(form.ActiveControl == bad1, "the cursor moves to the first wrong field", lines);
                Check(InputCheck.ReasonAt(bad1) == "Data source is empty." && InputCheck.ReasonAt(bad2).StartsWith("Port must be") && InputCheck.ReasonAt(good) == string.Empty,
                      "each wrong field carries a mark with its reason; a correct field has none", lines);

                bad1.Text = "srv"; bad2.Text = "1521";
                Check(InputCheck.For(form).Required(bad1, "Data source").Port(bad2, "Port").Report(status), "after fixing the inputs the action starts", lines);
                Check(InputCheck.ReasonAt(bad1) == string.Empty && InputCheck.ReasonAt(bad2) == string.Empty, "the marks are gone once the inputs are right", lines);
                InputCheck one = InputCheck.For(form).Required(Box(form, ""), "Schema");
                Check(one.Summary() == "1 input needs fixing. Nothing was started.", "one problem is worded in the singular", lines);
                form.Close();
            }
        }
    }
}
