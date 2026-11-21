using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Synthetic checks of the "Workbook identity check" window. Every workbook is created in a temporary folder
    /// and deleted again; no database, no user file and no other program is touched.
    /// </summary>
    internal static class WorkbookIdentityFormChecks
    {
        internal static void Run(Action<string> passed)
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-identity-form-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            SynchronizationContext before = SynchronizationContext.Current;
            bool automatic = WindowsFormsSynchronizationContext.AutoInstall;
            try
            {
                KeepUiContext();
                string a = Book(folder, "form-a.xlsx", new[]
                {
                    R("ID", "Name", "Amount"), R(1, "Asha", 10.5), R(2, "Sana", 20), R(3, "Ravi", 30),
                });
                // The same records in another order, with the key column under another name and two changed values.
                string b = Book(folder, "form-b.xlsx", new[]
                {
                    R("Ref", "name", "Amount"), R(3, "Ravi", 30), R(1, "Asha", 10.75), R(2, "SANA", 20),
                });
                CheckRoute(passed);
                CheckInputs(folder, a, b, passed);
                CheckComparison(folder, a, b, passed);
                CheckKeys(folder, a, b, passed);
                CheckClosing(folder, a, b, passed);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(before);
                WindowsFormsSynchronizationContext.AutoInstall = automatic;
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        // ------------------------------------------------------------------ helpers
        /// <summary>
        /// The app runs inside Application.Run, so a background worker always reports back on the window's thread.
        /// These checks have no running message loop: they pump with DoEvents, and Windows Forms removes its
        /// synchronisation context every time DoEvents returns. A worker started after that would report back on
        /// a pool thread, at the same time as the check reads the window. Keeping one context for the whole suite
        /// gives the checks the same single-threaded order the app has.
        /// </summary>
        private static void KeepUiContext()
        {
            WindowsFormsSynchronizationContext.AutoInstall = false;
            SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
        }

        private static void Require(bool valid, string message)
        {
            if (!valid) throw new InvalidOperationException("Workbook identity form check failed: " + message);
        }

        private static object[] R(params object[] cells) { return cells; }

        private static string Book(string folder, string name, object[][] rows)
        {
            string path = Path.Combine(folder, name);
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet sheet = workbook.Worksheets.Add("Data");
                for (int r = 0; r < rows.Length; r++)
                    for (int c = 0; c < rows[r].Length; c++)
                    {
                        object value = rows[r][c];
                        if (value is string) sheet.Cell(r + 1, c + 1).SetValue((string)value);
                        else sheet.Cell(r + 1, c + 1).SetValue(Convert.ToDouble(value));
                    }
                workbook.SaveAs(path);
            }
            return path;
        }

        private static T Find<T>(Control root, string name) where T : Control
        {
            T found = root.Controls.Find(name, true).OfType<T>().SingleOrDefault();
            Require(found != null, "the control " + name + " is missing");
            return found;
        }

        /// <summary>Lets the background work finish and its result reach the window.</summary>
        private static void Wait(WorkbookIdentityForm form)
        {
            Stopwatch clock = Stopwatch.StartNew();
            while (form.IsWorking)
            {
                Require(clock.ElapsedMilliseconds < 20000, "the background work did not finish in time");
                Application.DoEvents();
                Thread.Sleep(5);
            }
            Application.DoEvents();
        }

        private static bool Rejected(WorkbookIdentityForm form)
        {
            try { form.CaptureRequest(); }
            catch (ValidationFailure) { return true; }
            return false;
        }

        private static WorkbookIdentityForm Open(string a, string b, string report)
        {
            var form = new WorkbookIdentityForm { ShowInTaskbar = false };
            try
            {
                form.Show();
                Application.DoEvents();
                Find<CheckBox>(form, "chk_IdentityOpen").Checked = false;
                if (report != null) Find<TextBox>(form, "tb_IdentityOutput").Text = report;
                if (a != null)
                {
                    Find<TextBox>(form, "tb_IdentityFileA").Text = a;
                    Require(form.LoadWorkbook(true, false), "File A was not read: " + form.StatusText);
                    Wait(form);
                }
                if (b != null)
                {
                    Find<TextBox>(form, "tb_IdentityFileB").Text = b;
                    Require(form.LoadWorkbook(false, false), "File B was not read: " + form.StatusText);
                    Wait(form);
                }
                return form;
            }
            catch
            {
                form.Dispose();
                throw;
            }
        }

        private static void Tick(WorkbookIdentityForm form, string list, string column, bool ticked)
        {
            CheckedListBox box = Find<CheckedListBox>(form, list);
            int index = Enumerable.Range(0, box.Items.Count).FirstOrDefault(i => box.Items[i].ToString().StartsWith(column, StringComparison.Ordinal));
            Require(box.Items.Count > 0 && box.Items[index].ToString().StartsWith(column, StringComparison.Ordinal), "the key list has no column " + column);
            box.SetItemChecked(index, ticked);
        }

        // ------------------------------------------------------------------ checks
        private static void CheckRoute(Action<string> passed)
        {
            WorkspaceTask task = WorkspaceCatalog.Find("comparison.identity");
            Require(task.Workspace == WorkspaceKind.Comparison && task.Title == "Workbook identity check", "the task is not in the Comparison workspace");
            using (WorkspacePageForm page = task.CreatePage())
                Require(page.GetType() == typeof(WorkbookIdentityForm), "the route opens another window");
            using (WorkspacePageForm excel = WorkspaceCatalog.Find("comparison.excel").CreatePage())
            using (WorkspacePageForm audit = WorkspaceCatalog.Find("comparison.excel-audit").CreatePage())
                Require(excel.GetType() == typeof(Excel_Excel_Comparison) && audit.GetType() == typeof(ExcelSheetAuditForm),
                    "an existing Excel task was replaced");
            passed("Workbook identity check has its own route; the existing Excel tasks keep theirs.");
        }

        private static void CheckInputs(string folder, string a, string b, Action<string> passed)
        {
            string report = Path.Combine(folder, "inputs-report.xlsx");
            using (WorkbookIdentityForm empty = Open(null, null, report))
            {
                Require(Rejected(empty) && !empty.StartComparison(null) && !empty.IsWorking, "a comparison started without any workbook");
                Require(empty.StatusText.IndexOf("File A", StringComparison.Ordinal) >= 0, "the missing workbook was not named in the status line");
                empty.Close();
            }
            using (WorkbookIdentityForm form = Open(a, b, report))
            {
                Require(Find<Label>(form, "lbl_IdentityLoadedA").Text == "Loaded: 1 sheet, 3 columns, 3 data rows", "File A was described wrongly: "
                    + Find<Label>(form, "lbl_IdentityLoadedA").Text);
                ComboBox sheets = Find<ComboBox>(form, "cb_IdentitySheetA");
                Require(sheets.Items.Count == 2 && (string)sheets.Items[0] == WorkbookIdentityForm.AllSheets && (string)sheets.Items[1] == "Data" && sheets.SelectedIndex == 0,
                    "the sheet list of File A is wrong");
                CheckedListBox keys = Find<CheckedListBox>(form, "clb_IdentityKeysB");
                Require(keys.Items.Count == 3 && keys.Items[0].ToString() == "Ref" && !keys.Enabled, "the key list of File B is wrong");
                Require(!Rejected(form), "complete inputs were refused: " + form.StatusText);

                TextBox thresholds = Find<TextBox>(form, "tb_IdentityThresholds"), tolerance = Find<TextBox>(form, "tb_IdentityTolerance");
                foreach (string wrong in new[] { "", "0, x", "0, 0", "-1", "0, 0.01,", "1,2,3,4,5,6,7,8,9" })
                {
                    thresholds.Text = wrong;
                    Require(Rejected(form), "the thresholds \"" + wrong + "\" were accepted");
                }
                thresholds.Text = "0; 0.5";
                Require(form.CaptureRequest().Options.Thresholds.SequenceEqual(new[] { 0, 0.5 }), "two thresholds were not read");
                foreach (string wrong in new[] { "", "-1", "abc", "0,5" })
                {
                    tolerance.Text = wrong;
                    Require(Rejected(form), "the tolerance \"" + wrong + "\" was accepted");
                }
                tolerance.Text = "0.005";
                Require(form.CaptureRequest().Options.Tolerance == 0.005, "the tolerance was not read");

                TextBox output = Find<TextBox>(form, "tb_IdentityOutput");
                output.Text = a;
                Require(Rejected(form), "the report could be written over File A");
                output.Text = Path.Combine(folder, "report.txt");
                Require(Rejected(form), "a report that is not an .xlsx file was accepted");
                output.Text = report;

                // One sheet on one side and all sheets on the other cannot be paired.
                sheets.SelectedIndex = 1;
                Wait(form);
                Require(Rejected(form), "one sheet in File A was paired with all sheets of File B");
                Find<ComboBox>(form, "cb_IdentitySheetB").SelectedIndex = 1;
                Wait(form);
                IdentityOptions chosen = form.CaptureRequest().Options;
                Require(chosen.SheetA == "Data" && chosen.SheetB == "Data", "the chosen sheets were not passed on");
                Require(!File.Exists(report), "checking the inputs wrote a report");
                form.Close();
            }
            passed("Workbook identity form reads both workbooks, lists sheets and key columns, and refuses wrong inputs beside the field.");
        }

        private static void CheckComparison(string folder, string a, string b, Action<string> passed)
        {
            string report = Path.Combine(folder, "position-report.xlsx");
            byte[] beforeA = File.ReadAllBytes(a), beforeB = File.ReadAllBytes(b);
            using (WorkbookIdentityForm form = Open(a, b, report))
            {
                Require(form.StartComparison(null), "the comparison did not start: " + form.StatusText);
                Require(form.IsWorking && !Find<Button>(form, "btn_IdentityCompare").Enabled && Find<Button>(form, "btn_IdentityCancel").Enabled,
                    "the inputs stayed open, or Cancel was not offered, while the comparison ran");
                Wait(form);
                IdentityResult result = form.Result;
                Require(result != null && !result.Identical && !result.Keyed, "the comparison by row position gave no result: " + form.StatusText);
                Require(Find<Label>(form, "lbl_IdentityVerdict").Text == result.Verdict && Find<GroupBox>(form, "gb_IdentityResult").Visible,
                    "the verdict is not shown");
                Require(Find<DataGridView>(form, "dgv_IdentitySummary").Rows.Count == IdentityResult.Checks.Length, "the Summary tab does not list the 18 checks");
                TabControl tabs = Find<TabControl>(form, "tabs_IdentityResult");
                Require(tabs.TabPages.Count == 5 && tabs.TabPages[2].Text == "Data Differences (" + result.Data.Count + ")", "the result tabs are wrong: " + tabs.TabPages[2].Text);
                DataGridView data = Find<DataGridView>(form, "dgv_IdentityDataDifferences");
                Require(result.Data.Count > 0 && data.Rows.Count == result.Data.Count && (string)data.Rows[0].Cells[0].Value == "1"
                    && data.Columns[0].HeaderText == "SrNo." && data.Columns.Cast<DataGridViewColumn>().All(column => column.HeaderText != "Key"),
                    "the Data Differences tab does not show the rows of the report");
                using (var written = new XLWorkbook(report))
                    Require(written.Worksheets.Select(sheet => sheet.Name).SequenceEqual(IdentityReportWriter.SheetNames), "the report does not have its five sheets");
                Require(beforeA.SequenceEqual(File.ReadAllBytes(a)) && beforeB.SequenceEqual(File.ReadAllBytes(b)), "an input workbook was changed");
                Require(Find<Button>(form, "btn_IdentityOpenReport").Enabled && Find<Button>(form, "btn_IdentityCompare").Enabled, "the buttons were not released");

                // An existing report is kept unless the replacement is confirmed.
                byte[] kept = File.ReadAllBytes(report);
                Require(!form.StartComparison(path => false) && !form.IsWorking && kept.SequenceEqual(File.ReadAllBytes(report)),
                    "an existing report was replaced without confirmation");
                Require(form.StartComparison(path => string.Equals(path, report, StringComparison.OrdinalIgnoreCase)), "a confirmed replacement did not start");
                Wait(form);
                Require(form.Result != null && form.StatusText.IndexOf("finished", StringComparison.Ordinal) >= 0, "the confirmed replacement did not finish: " + form.StatusText);

                // A result on screen is marked once an input is edited.
                Label stale = Find<Label>(form, "lbl_IdentityStale");
                Require(!stale.Visible, "a fresh result was marked as out of date");
                Find<CheckBox>(form, "chk_IdentityFormulas").Checked = false;
                Require(stale.Visible, "an edited input did not mark the result as out of date");
                form.Close();
            }

            // The same workbook on both sides is identical.
            string copy = Path.Combine(folder, "form-a-copy.xlsx");
            File.Copy(a, copy);
            using (WorkbookIdentityForm same = Open(a, copy, Path.Combine(folder, "same-report.xlsx")))
            {
                Require(same.StartComparison(null), "the second comparison did not start: " + same.StatusText);
                Wait(same);
                Require(same.Result != null && same.Result.Identical && Find<Label>(same, "lbl_IdentityVerdict").Text.StartsWith("IDENTICAL", StringComparison.Ordinal),
                    "two equal workbooks were not reported as identical");
                same.Close();
            }
            passed("Workbook identity form compares in the background, shows the verdict and the five tabs, writes the report and keeps an existing one until confirmed.");
        }

        private static void CheckKeys(string folder, string a, string b, Action<string> passed)
        {
            using (WorkbookIdentityForm form = Open(a, b, Path.Combine(folder, "key-report.xlsx")))
            {
                Find<RadioButton>(form, "rb_IdentityKey").Checked = true;
                CheckedListBox listA = Find<CheckedListBox>(form, "clb_IdentityKeysA");
                Require(listA.Enabled && Rejected(form), "a key comparison was accepted without a key column");

                // Parts are numbered in the order they are ticked, and close up when one is removed.
                Tick(form, "clb_IdentityKeysA", "Name", true);
                Tick(form, "clb_IdentityKeysA", "ID", true);
                Require(form.KeyOrder(true).SequenceEqual(new[] { "Name", "ID" }) && listA.Items[0].ToString() == "ID   (key part 2)",
                    "the key parts are not numbered in the order ticked");
                Tick(form, "clb_IdentityKeysA", "Name", false);
                Require(form.KeyOrder(true).SequenceEqual(new[] { "ID" }) && listA.Items[0].ToString() == "ID   (key part 1)" && listA.Items[1].ToString() == "Name",
                    "removing a key part did not renumber the rest");
                Require(Rejected(form), "a key in Source without a key in Target was accepted");
                Tick(form, "clb_IdentityKeysB", "Ref", true);
                Tick(form, "clb_IdentityKeysB", "Amount", true);
                Require(Rejected(form), "one key column was paired with two");
                Tick(form, "clb_IdentityKeysB", "Amount", false);

                Require(form.StartComparison(null), "the key comparison did not start: " + form.StatusText);
                Wait(form);
                IdentityResult result = form.Result;
                Require(result != null && result.Keyed && result.Count("rows_only") == 0, "rows were not paired by the key: " + form.StatusText);
                // 10.5 against 10.75, and "Sana" against "SANA"; the header "Name" against "name" is a header difference, not a value.
                Require(result.Count("values") == 2 && result.Count("header_case") == 1, "the key comparison found the wrong differences");
                DataGridView data = Find<DataGridView>(form, "dgv_IdentityDataDifferences");
                Require(data.Columns.Cast<DataGridViewColumn>().Any(column => column.HeaderText == "Key"), "the Key column is missing from the Data Differences tab");
                Require(Find<Label>(form, "lbl_IdentityMatching").Text.IndexOf("File A: ID", StringComparison.Ordinal) >= 0, "the result does not say which key was used");

                // Reading the workbook again keeps a ticked key that is still there.
                Require(form.LoadWorkbook(true, true), "File A was not read again");
                Wait(form);
                Require(form.KeyOrder(true).SequenceEqual(new[] { "ID" }), "reading the workbook again lost the ticked key");
                form.Close();
            }
            passed("Workbook identity form pairs rows by a ticked key, numbers the key parts in tick order and allows different key names in the two files.");
        }

        private static void CheckClosing(string folder, string a, string b, Action<string> passed)
        {
            string report = Path.Combine(folder, "closing-report.xlsx");
            using (WorkbookIdentityForm form = Open(a, b, report))
            {
                Require(form.StartComparison(null), "the comparison did not start: " + form.StatusText);
                // The result reaches the window only through the message loop, so the task is still busy here.
                Require(!form.PrepareWorkspaceClose(CloseReason.UserClosing) && form.IsWorking, "the task could be closed while it was working");
                Wait(form);
                if (form.StatusText.IndexOf("Cancelled", StringComparison.Ordinal) >= 0)
                    Require(form.Result == null && !File.Exists(report), "a cancelled comparison left a result or a report");
                Require(form.PrepareWorkspaceClose(CloseReason.UserClosing), "the task could not be closed after the work had stopped");
                form.Close();
            }
            passed("Workbook identity form refuses to close while it is working and closes once the work has stopped.");
        }
    }
}
