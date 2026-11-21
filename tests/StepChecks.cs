using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Controlled-step checks on real windows, without databases or user files: a later step stays
    /// locked until the one before it is done, window code cannot unlock it, an earlier change locks
    /// the later steps again while keeping the choices that still fit, and the step strip is readable
    /// in every theme. Run by "SPI Data Comp Tool.exe" --check-steps OUT_FILE (see SelfTest.cs).
    /// </summary>
    internal static class StepChecks
    {
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

            check("step engine: order, locks, busy, hidden steps", Engine);
            check("step engine: constructor notifications wait for a live window and disposal cancels them", EngineLifecycle);
            check("step engine: real text and checked-list events advance and relock steps", EngineUiEvents);
            check("step engine: background refreshes run on the window thread", EngineWorkerRefresh);
            check("Compare Excel files: target, load, keys, columns, validate and compare unlock in order", ExcelSteps);
            check("Compare Excel files: a new source locks the later steps and keeps keys and columns (R2)", ExcelKeepsChoices);
            check("Compare databases: target, tables, load, keys, columns, set and compare unlock in order", DatabaseSteps);
            check("Compare databases: shown profile selection and password edits advance without manual refresh", ProfileSelectionSteps);
            check("Compare list tables: steps follow the Databases / Excel folders tab", ListSteps);
            check("Export database to Excel: load tables, tick tables, export", ExportSteps);
            check("Customised Excel: workbook, columns, create", CustomisedSteps);
            check("step strip is readable in every theme", StripColours);

            lines.Add(failed == 0 ? "step checks passed" : failed + " step check(s) failed");
            File.WriteAllLines(outFile, lines);
            return failed == 0 ? 0 : 1;
        }

        private static void Engine()
        {
            using (var form = new Form())
            using (var strip = new StepStrip())
            {
                var a = new Button();
                var b = new Button();
                var c = new Button();
                form.Controls.AddRange(new Control[] { strip, a, b, c });
                bool aDone = false, bDone = false, skipB = false;
                var flow = new StepFlow(form, strip)
                    .Add("A", "Do A", () => aDone, a)
                    .Add(() => "B", () => "Do B", () => bDone, () => !skipB, b)
                    .Add("C", "Do C", () => false, c);
                flow.Refresh();
                Require(a.Enabled && !b.Enabled && !c.Enabled, "only the first step is open at the start");
                Require(flow.NextText().Contains("Do A"), "the strip says what to do first");
                b.Enabled = true;
                Require(!b.Enabled, "window code enabled a locked step");
                aDone = true; flow.Refresh();
                Require(b.Enabled && !c.Enabled, "step B did not unlock after A");
                Require(flow.LockedText(flow.Steps[2]).Contains("Do B first"), "a locked step says what to do first");
                bDone = true; flow.Refresh();
                Require(c.Enabled && flow.StateOf(1) == StepState.Done, "step C did not unlock after B");
                flow.Busy = true;
                Require(!a.Enabled && !b.Enabled && !c.Enabled, "busy left a step control enabled");
                c.Enabled = true;
                Require(!c.Enabled, "window code enabled a control while busy");
                flow.Busy = false;
                Require(a.Enabled && b.Enabled && c.Enabled, "not busy any more did not open the steps again");
                aDone = false; flow.Refresh();
                Require(a.Enabled && !b.Enabled && !c.Enabled, "undoing step A did not lock the later steps");
                aDone = true; bDone = false; skipB = true; flow.Refresh();
                Require(c.Enabled && !flow.Steps[1].Shown, "a step that does not apply must let the next one through");
            }
        }

        private sealed class RecreatedStepForm : Form
        {
            internal RecreatedStepForm() { }
            internal void RecreateWindow() { RecreateHandle(); }
        }

        private static void PumpEvents()
        {
            // Includes notifications queued by another notification, without sleeps or a
            // direct flow.Refresh that would conceal a broken UI event subscription.
            for (int i = 0; i < 4; i++) Application.DoEvents();
        }

        private static void EngineLifecycle()
        {
            using (var form = new RecreatedStepForm { ShowInTaskbar = false })
            {
                var input = new TextBox();
                var next = new Button();
                var strip = new StepStrip();
                form.Controls.AddRange(new Control[] { strip, input, next });
                int reads = 0;
                var flow = new StepFlow(form, strip)
                    .Add("Input", "Enter the input", () => { reads++; return input.Text == "ready"; }, input)
                    .Add("Next", "Continue", () => false, next);
                flow.Refresh();
                reads = 0;
                input.Text = "ready";
                Require(reads == 0 && !next.Enabled,
                    "A constructor-time input event evaluated an incomplete window inline.");

                form.Show();
                PumpEvents();
                Require(reads > 0 && next.Enabled, "The first shown window did not evaluate its completed input state.");
                input.Text = string.Empty; // Queue a refresh against the original handle.
                form.RecreateWindow();
                PumpEvents();
                Require(!next.Enabled, "Recreating the window lost a queued input change.");
                input.Text = "ready";
                PumpEvents();
                Require(next.Enabled, "The recreated window stopped accepting new input notifications.");

                input.Text = string.Empty;
                form.Dispose();
                reads = 0;
                flow.Later();
                flow.Refresh();
                PumpEvents();
                Require(reads == 0, "A disposed window still ran readiness callbacks.");
            }
        }

        private static void EngineUiEvents()
        {
            using (var form = new Form { ShowInTaskbar = false })
            {
                var input = new TextBox();
                var choices = new CheckedListBox();
                var next = new Button();
                choices.Items.Add("Column");
                form.Controls.AddRange(new Control[] { input, choices, next });
                var flow = new StepFlow(form, null)
                    .Add("Input", "Enter the input", () => input.Text == "ready", input)
                    .Add("Columns", "Tick a column", () => choices.CheckedItems.Count > 0, choices)
                    .Add("Next", "Continue", () => false, next);
                flow.Refresh();
                form.Show();
                PumpEvents();
                Require(!choices.Enabled && !next.Enabled, "The initial dependent steps were not locked.");
                input.Text = "ready";
                PumpEvents();
                Require(choices.Enabled && !next.Enabled, "TextChanged did not advance to the column step.");
                choices.SetItemChecked(0, true);
                PumpEvents();
                Require(next.Enabled, "ItemCheck read the old value and left the next step locked.");
                input.Text = string.Empty;
                PumpEvents();
                Require(!choices.Enabled && !next.Enabled && choices.GetItemChecked(0),
                    "Editing an earlier input failed to lock later steps or discarded the saved column choice.");
                input.Text = "ready";
                PumpEvents();
                Require(next.Enabled, "Restoring an earlier input failed to reuse the retained column choice.");
                choices.SetItemChecked(0, false);
                PumpEvents();
                Require(!next.Enabled, "Unticking the last column did not relock the next step.");
            }
        }

        private static void EngineWorkerRefresh()
        {
            bool previousCheck = Control.CheckForIllegalCrossThreadCalls;
            Control.CheckForIllegalCrossThreadCalls = true;
            try
            {
                using (var form = new Form { ShowInTaskbar = false })
                {
                    var next = new Button();
                    form.Controls.Add(next);
                    bool ready = false;
                    int callbackThread = 0;
                    int windowThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    var flow = new StepFlow(form, null)
                        .Add("Input", "Wait for input", () =>
                        {
                            callbackThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
                            return ready;
                        })
                        .Add("Next", "Continue", () => false, next);
                    flow.Refresh();
                    callbackThread = 0;
                    var early = System.Threading.Tasks.Task.Run(() => flow.Refresh());
                    Require(early.Wait(5000), "An early background refresh did not return.");
                    Require(callbackThread == 0, "A background refresh evaluated controls before the window had a handle.");
                    form.Show();
                    PumpEvents();
                    Require(!next.Enabled, "The worker scenario started with the next step unlocked.");
                    var worker = System.Threading.Tasks.Task.Run(() => { ready = true; flow.Refresh(); });
                    Require(worker.Wait(5000), "A background refresh blocked waiting for the window thread.");
                    PumpEvents();
                    Require(next.Enabled && callbackThread == windowThread,
                        "A background refresh failed to advance on the window thread.");
                }
            }
            finally { Control.CheckForIllegalCrossThreadCalls = previousCheck; }
        }

        private static OpenFileDialogData Chosen(string path)
        {
            var file = new OpenFileDialogData();
            typeof(OpenFileDialogData).GetProperty("boolFileSelected").SetValue(file, true);
            typeof(OpenFileDialogData).GetProperty("strFileNameWithPath").SetValue(file, path);
            file.SimpleFileName_WithoutExt = Path.GetFileNameWithoutExtension(path);
            return file;
        }

        private static void ExcelSteps()
        {
            using (var form = new Excel_Excel_Comparison())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                flow.Refresh();
                Locked(form, "btn_SecondExcelSheet", "btn_SelectLoad", "clb_reference_ID", "cb_Primary_Key", "clb_firstExcel", "btn_Validate", "btn_Compare");
                Open(form, "btn_FirstExcelSheet");

                Set(form, "chosenFirst", Chosen(@"C:\data\source.xlsx")); flow.Refresh();
                Open(form, "btn_SecondExcelSheet");
                Locked(form, "btn_SelectLoad", "clb_reference_ID", "cb_Primary_Key");

                Set(form, "chosenSecond", Chosen(@"C:\data\target.xlsx")); flow.Refresh();
                Open(form, "btn_SelectLoad", "cb_MultiReference");
                Locked(form, "clb_reference_ID", "cb_Primary_Key", "clb_firstExcel");

                Loaded(form);
                flow.Refresh();
                Open(form, "clb_reference_ID", "cb_Primary_Key");
                Locked(form, "clb_firstExcel", "btn_Validate", "btn_Compare");

                var keys = Find<CheckedListBox>(form, "clb_reference_ID");
                var columns = Find<CheckedListBox>(form, "clb_firstExcel");
                keys.SetItemChecked(0, true); flow.Refresh();
                Open(form, "clb_firstExcel", "cb_select_first_clb");
                Locked(form, "btn_Validate", "btn_Compare");
                columns.SetItemChecked(1, true); flow.Refresh();
                Open(form, "btn_Validate", "cb_IgnoreCase", "cb_IncludeRule");
                Locked(form, "btn_Compare");
                Set(form, "boolValidate", true); flow.Refresh();
                Open(form, "btn_Compare");

                columns.SetItemChecked(1, false);   // invalidates the validation
                flow.Refresh();
                Require(!Get<bool>(form, "boolValidate"), "a changed column kept the old validation");
                Locked(form, "btn_Validate", "btn_Compare");
            }
        }

        private static void Loaded(Excel_Excel_Comparison form)
        {
            var data = new DataSet();
            foreach (string name in new[] { "Source", "Target" })
            {
                var table = new DataTable(name);
                table.Columns.Add("ID");
                table.Columns.Add("Value");
                table.Rows.Add("1", "x");
                data.Tables.Add(table);
            }
            Set(form, "ds", data);
            Set(form, "boolSelectedBothFile", true);
            foreach (string list in new[] { "clb_reference_ID", "clb_firstExcel", "clb_secondExcel" })
            {
                var box = Find<CheckedListBox>(form, list);
                box.Items.Clear();
                box.Items.Add("ID");
                box.Items.Add("Value");
            }
        }

        private static void ExcelKeepsChoices()
        {
            using (var form = new Excel_Excel_Comparison())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                Set(form, "chosenFirst", Chosen(@"C:\data\source.xlsx"));
                Set(form, "chosenSecond", Chosen(@"C:\data\target.xlsx"));
                Loaded(form);
                var keys = Find<CheckedListBox>(form, "clb_reference_ID");
                var columns = Find<CheckedListBox>(form, "clb_firstExcel");
                keys.SetItemChecked(0, true);
                columns.SetItemChecked(1, true);
                Set(form, "boolValidate", true);
                flow.Refresh();
                Open(form, "btn_Compare");

                typeof(Excel_Excel_Comparison).GetMethod("InvalidateLoad", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(form, null);
                flow.Refresh();
                Locked(form, "clb_reference_ID", "clb_firstExcel", "btn_Validate", "btn_Compare");
                Open(form, "btn_SelectLoad");
                Require(!Get<bool>(form, "boolValidate"), "a new workbook kept the old validation");
                Require(keys.GetItemChecked(0) && columns.GetItemChecked(1), "a new workbook cleared the keys and columns");

                // after reloading, the same names are ticked again; missing names are skipped
                keys.Items.Clear(); keys.Items.Add("Value"); keys.Items.Add("ID");
                Require(StepFlow.Restore(keys, new[] { "ID", "Gone" }) == 1 && keys.GetItemChecked(1) && !keys.GetItemChecked(0),
                    "reloading did not tick the earlier key by name");
            }
        }

        private static void DatabaseSteps()
        {
            using (var form = new DB_Comparison_Form())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                var connection = Find<ConnectionPanel>(form, "connection");
                Fill(connection.Source, true);
                Fill(connection.Target, false);
                flow.Refresh();
                Require(connection.Target.Group.Enabled, "Target connection locked although Source is filled in");
                Fill(connection.Source, false);
                flow.Refresh();
                Require(!connection.Target.Group.Enabled, "Target connection open while Source is empty");
                Locked(form, "cb_SRC_TableName", "btn_Load_SRC_Table", "btn_Connect_Reference_ID", "clb_ReferenceID", "cbL_SRC_Cols", "btn_Set", "btnGenerateOutput");

                Fill(connection.Source, true); flow.Refresh();
                Require(connection.Target.Group.Enabled, "Target connection did not unlock after Source");
                Locked(form, "cb_SRC_TableName", "btn_Connect_Reference_ID");
                Fill(connection.Target, true); flow.Refresh();
                Open(form, "cb_SRC_TableName", "btn_Load_SRC_Table");
                Locked(form, "cb_TRG_TableName", "btn_Connect_Reference_ID");
                Find<ComboBox>(form, "cb_SRC_TableName").Text = "SRC_TABLE"; flow.Refresh();
                Open(form, "cb_TRG_TableName");
                Locked(form, "btn_Connect_Reference_ID");
                Find<ComboBox>(form, "cb_TRG_TableName").Text = "TRG_TABLE"; flow.Refresh();
                Open(form, "btn_Connect_Reference_ID", "cb_ColumnMapping");
                Locked(form, "clb_ReferenceID", "cb_PrimaryKey", "cbL_SRC_Cols", "btn_Set", "btnGenerateOutput");

                Set(form, "comparisonInputsLoaded", true);
                var keys = Find<CheckedListBox>(form, "clb_ReferenceID");
                var columns = Find<CheckedListBox>(form, "cbL_SRC_Cols");
                keys.Items.AddRange(new object[] { "ID", "VALUE" });
                columns.Items.AddRange(new object[] { "ID", "VALUE" });
                flow.Refresh();
                Open(form, "clb_ReferenceID", "cb_PrimaryKey");
                Locked(form, "cbL_SRC_Cols", "btn_Set");
                keys.SetItemChecked(0, true); flow.Refresh();
                Open(form, "cbL_SRC_Cols", "cb_selectAll_toggle");
                Locked(form, "btn_Set");
                columns.SetItemChecked(1, true); flow.Refresh();
                Open(form, "btn_Set", "cb_IgnoreCase");
                Locked(form, "btnGenerateOutput");
                Set(form, "comparisonPrepared", true); flow.Refresh();
                Open(form, "btnGenerateOutput");

                Find<ComboBox>(form, "cb_SRC_TableName").Text = "OTHER";   // invalidates the loaded tables
                flow.Refresh();
                Locked(form, "clb_ReferenceID", "cbL_SRC_Cols", "btn_Set", "btnGenerateOutput");
                Require(keys.GetItemChecked(0) && columns.GetItemChecked(1), "a changed table cleared the keys and columns");
            }
        }

        private static void ProfileSelectionSteps()
        {
            List<DBC> previousProfiles = Profile.ProfileList;
            string previousPath = Properties.Settings.Default.ProfilePathUser;
            string folder = Path.Combine(Path.GetTempPath(), "spi-profile-steps-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "profiles.json");
            try
            {
                Properties.Settings.Default.ProfilePathUser = string.Empty;
                Profile.ProfileList = new List<DBC>();
                File.WriteAllText(path, ProfileStore.SerializeForExport(new[]
                {
                    StepProfile("First", "server-one"), StepProfile("Second", "server-two")
                }));
                using (var form = new DB_Comparison_Form { ShowInTaskbar = false })
                {
                    form.Show();
                    PumpEvents();
                    var connection = Find<ConnectionPanel>(form, "connection");
                    Exception selectionError = null;
                    int applied = 0;
                    connection.Failed += (sender, error) => selectionError = error;
                    connection.ProfileApplied += (sender, details) => applied++;
                    ProfileLoadResult result = connection.LoadSavedProfiles(path);
                    Require(result.Loaded && !result.HasError && connection.ProfileList.Items.Count == 2,
                        "A valid saved profile file did not populate the shown window's picker.");
                    connection.ProfileList.SelectedIndex = 0;
                    PumpEvents();
                    Require(selectionError == null && applied == 1
                        && connection.Source.DataSource.Text == "server-one-source"
                        && connection.Target.DataSource.Text == "server-one-target",
                        "Selecting a profile in the shown window did not apply both connection halves.");
                    Require(connection.Source.Password.Text.Length == 0 && connection.Target.Password.Text.Length == 0,
                        "The exported profile unexpectedly supplied stored passwords.");
                    Require(!connection.Target.Group.Enabled, "A password-free Source connection incorrectly unlocked Target.");

                    const string inputMarker = "synthetic-input";
                    connection.Source.Password.Text = inputMarker;
                    PumpEvents();
                    Require(connection.Target.Group.Enabled, "Entering the Source password did not unlock Target through its input event.");
                    connection.Target.Password.Text = inputMarker;
                    PumpEvents();
                    Open(form, "btn_Connect_Reference_ID");
                    connection.Source.Password.Clear();
                    PumpEvents();
                    Require(!connection.Target.Group.Enabled, "Clearing an earlier password left Target unlocked.");
                    Locked(form, "btn_Connect_Reference_ID");
                    connection.Source.Password.Text = inputMarker;
                    PumpEvents();
                    Open(form, "btn_Connect_Reference_ID");
                    Require(connection.Target.Password.Text == inputMarker, "Revisiting Source discarded the existing Target input.");

                    connection.ProfileList.SelectedIndex = 1;
                    PumpEvents();
                    Require(selectionError == null && applied == 2
                        && connection.Source.DataSource.Text == "server-two-source"
                        && connection.Target.DataSource.Text == "server-two-target"
                        && connection.Source.Password.Text.Length == 0 && connection.Target.Password.Text.Length == 0,
                        "Changing profiles did not replace both connection halves and invalidate the previous credentials.");
                    Locked(form, "btn_Connect_Reference_ID");
                }
            }
            finally
            {
                Profile.ProfileList = previousProfiles;
                Properties.Settings.Default.ProfilePathUser = previousPath;
                Directory.Delete(folder, true);
            }
        }

        private static DBC StepProfile(string name, string server)
        {
            return new DBC
            {
                profileName = name,
                dbSRC = new DBConnectionDetailsProfile
                {
                    str_DBType = "SQL", bool_Rb_SQL = true, str_DataSource = server + "-source",
                    str_DBName = "SourceDatabase", str_UserName = "synthetic-user", str_TableName = "dbo.SourceTable"
                },
                dbTRG = new DBConnectionDetailsProfile
                {
                    str_DBType = "SQL", bool_Rb_SQL = true, str_DataSource = server + "-target",
                    str_DBName = "TargetDatabase", str_UserName = "synthetic-user", str_TableName = "dbo.TargetTable"
                }
            };
        }

        private static void ExportSteps()
        {
            using (var form = new SQL_DB_To_Excel())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                var connection = Find<ConnectionPanel>(form, "connection");
                Fill(connection.Source, false); flow.Refresh();
                Locked(form, "btn_Connect", "clb_TableList", "btn_ExportToExcel");
                Fill(connection.Source, true); flow.Refresh();
                Open(form, "btn_Connect");
                Locked(form, "clb_TableList", "btn_ExportToExcel");
                Set(form, "tablesLoaded", true);
                var tables = Find<CheckedListBox>(form, "clb_TableList");
                tables.Items.Clear(); tables.Items.Add("T1");
                flow.Refresh();
                Open(form, "clb_TableList", "cb_SelectAll");
                Locked(form, "btn_ExportToExcel");
                tables.SetItemChecked(0, true); flow.Refresh();
                Open(form, "btn_ExportToExcel");
            }
        }

        private static void CustomisedSteps()
        {
            using (var form = new CustomisedExcel())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                Require(form.Controls.Find("step_Strip", false).Length == 1, "the step strip was not attached");
                flow.Refresh();
                Open(form, "btn_ReadExcel");
                Locked(form, "checkedListBox1", "btn_CreateExcel");
                var data = new DataSet();
                data.Tables.Add(new DataTable("Rules"));
                data.Tables[0].Columns.Add("A");
                Set(form, "dsCustomisedExcel", data);
                var columns = Find<CheckedListBox>(form, "checkedListBox1");
                columns.Items.Add("A");
                flow.Refresh();
                Open(form, "checkedListBox1");
                Locked(form, "btn_CreateExcel");
                columns.SetItemChecked(0, true); flow.Refresh();
                Open(form, "btn_CreateExcel");
            }
        }

        private static void Fill(ConnectionSide side, bool filled)
        {
            side.Sql.Checked = true;
            side.WindowsLogin.Checked = false;
            side.DataSource.Text = filled ? "server" : string.Empty;
            side.Database.Text = filled ? "db" : string.Empty;
            side.User.Text = filled ? "user" : string.Empty;
            side.Password.Text = filled ? "pw" : string.Empty;
        }

        private static void ListSteps()
        {
            using (var form = new Compare_Multiples_List_Tables_Forms())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                var tabs = Find<TabControl>(form, "tabControl1");
                tabs.SelectedTab = Find<TabPage>(form, "tabPage_DB");
                flow.Refresh();
                Locked(form, "btn_ListTableExcel", "l_lb_Output_Set", "btn_Compare");
                Require(flow.Steps.Count(s => s.Shown) == 4, "Databases tab should show 4 steps");
                Set(form, "boolLoadDataBase", true);
                Set(form, "boolSRCConnection", true);
                Set(form, "boolTRGConnection", true);
                flow.Refresh();
                Open(form, "btn_ListTableExcel");
                Locked(form, "l_lb_Output_Set", "btn_Compare");
                Set(form, "boolExcelFileList", true); flow.Refresh();
                Open(form, "l_lb_Output_Set");
                Locked(form, "btn_Compare");
                Find<LinkLabel>(form, "l_lbl_outputFolder").Text = Path.GetTempPath();
                Set(form, "boolFolderPathSelected", true); flow.Refresh();
                Open(form, "btn_Compare", "cb_IgnoreCase");

                tabs.SelectedTab = Find<TabPage>(form, "tabPage_Excel");
                Set(form, "boolSRC_ExcelFile", false);
                Set(form, "boolTRG_ExcelFile", false);
                flow.Refresh();
                Require(flow.Steps.Count(s => s.Shown) == 5, "Excel folders tab should show 5 steps");
                Open(form, "llb_Excel_SRC");
                Locked(form, "llb_Excel_TRG", "btn_ListTableExcel", "btn_Compare");
                Find<LinkLabel>(form, "llb_Excel_SRC").Text = Path.GetTempPath();
                Set(form, "boolSRC_ExcelFile", true); flow.Refresh();
                Open(form, "llb_Excel_TRG");
                Locked(form, "btn_ListTableExcel");
            }
        }

        private static void StripColours()
        {
            foreach (UiPalette p in UiTheme.All)
            {
                foreach (StepState state in Enum.GetValues(typeof(StepState)))
                {
                    Color fill, mark, title;
                    StepStrip.ColorsOf(state, p, out fill, out mark, out title);
                    Require(ThemeChecks.Contrast(mark, fill) >= 3.0, p.Name + " " + state + ": the tick, number or padlock is hard to see");
                    Require(ThemeChecks.Contrast(title, p.CardBack) >= 4.5, p.Name + " " + state + ": the step name is hard to read");
                    if (state != StepState.Locked)
                        Require(ThemeChecks.Contrast(fill, p.CardBack) >= 3.0, p.Name + " " + state + ": the step circle is hard to see");
                }
                Require(ThemeChecks.Contrast(p.TextColor, p.CardBack) >= 4.5, p.Name + ": the next-step text is hard to read");
            }
        }

        private static void Locked(Control form, params string[] names)
        {
            foreach (string name in names)
                Require(!Find<Control>(form, name).Enabled, name + " should be locked");
        }

        private static void Open(Control form, params string[] names)
        {
            foreach (string name in names)
                Require(Find<Control>(form, name).Enabled, name + " should be open");
        }

        private static T Find<T>(Control form, string name) where T : Control
        {
            T found = form.Controls.Find(name, true).OfType<T>().FirstOrDefault();
            if (found == null) throw new InvalidOperationException("missing control " + name);
            return found;
        }

        private static FieldInfo Field(object target, string name)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null) throw new InvalidOperationException("missing field " + name);
            return field;
        }

        private static T Get<T>(object target, string name)
        {
            return (T)Field(target, name).GetValue(target);
        }

        private static void Set(object target, string name, object value)
        {
            Field(target, name).SetValue(target, value);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
