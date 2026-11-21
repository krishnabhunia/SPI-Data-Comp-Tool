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
            check("Compare Excel files: target, load, keys, columns, validate and compare unlock in order", ExcelSteps);
            check("Compare Excel files: a new source locks the later steps and keeps keys and columns (R2)", ExcelKeepsChoices);
            check("Compare databases: target, tables, load, keys, columns, set and compare unlock in order", DatabaseSteps);
            check("Compare list tables: steps follow the Databases / Excel folders tab", ListSteps);
            check("Delta change tracker: target, table list, date filter and compare unlock in order", TrackerSteps);
            check("Delta change legacy ID: test connection, then map", LegacySteps);
            check("SPI FK extraction: test connection, then import", ForeignKeySteps);
            check("Table extraction after delta: source or Excel sheet, target, test, run", ExtractionSteps);
            check("Spec note creation: workbook, create notes, export", SpecNoteSteps);
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

        private static void TrackerSteps()
        {
            using (var form = new SPIDeltaChangeTrackerOptimised())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                var connection = Find<ConnectionPanel>(form, "connection");
                Fill(connection.Source, false);
                Fill(connection.Target, false);
                flow.Refresh();
                Require(!connection.Target.Group.Enabled, "Target connection open while Source is empty");
                Locked(form, "btn_ListTableExcel", "btn_Load_SRC_Table", "cb_DateCol", "llbl_Folder_path", "btnGenerateOutput");
                Fill(connection.Source, true); flow.Refresh();
                Require(connection.Target.Group.Enabled, "Target connection did not unlock after Source");
                Locked(form, "btn_ListTableExcel");
                Fill(connection.Target, true); flow.Refresh();
                Open(form, "btn_ListTableExcel", "btn_Load_SRC_Table");
                Locked(form, "cb_DateCol", "btnGenerateOutput");

                var src = Get<DBConnectionStatus>(form, "dbSRCConnection");
                var trg = Get<DBConnectionStatus>(form, "dbTRGConnection");
                src.boolConnection = trg.boolConnection = true;
                var list = new DataSet();
                list.Tables.Add(new DataTable());
                list.Tables[0].Columns.Add("src_table_names");
                list.Tables[0].Rows.Add("T1");
                Set(form, "dsTableList", list);
                flow.Refresh();
                Open(form, "cb_DateCol", "llbl_Folder_path");
                Find<ComboBox>(form, "cb_DateCol").Text = "CHG_DATE";
                Find<LinkLabel>(form, "llbl_Folder_path").Text = Path.GetTempPath();
                flow.Refresh();
                Open(form, "btnGenerateOutput", "cb_IgnoreCase");

                connection.Source.DataSource.Text = "other";   // changed connection: test again through the table list
                flow.Refresh();
                Locked(form, "cb_DateCol", "btnGenerateOutput");
                Open(form, "btn_ListTableExcel");
            }
        }

        private static void LegacySteps()
        {
            using (var form = new SPIDeltaChangeLegacyID())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                var connection = Find<ConnectionPanel>(form, "connection");
                Fill(connection.Source, false); flow.Refresh();
                Locked(form, "btn_Connect_Reference_ID", "btn_ListTableExcel");
                Fill(connection.Source, true); flow.Refresh();
                Open(form, "btn_Connect_Reference_ID");
                Locked(form, "btn_ListTableExcel");
                Get<DBConnectionStatus>(form, "dBConnectionStatus_SRC").boolConnection = true; flow.Refresh();
                Open(form, "btn_ListTableExcel");
            }
        }

        private static void ForeignKeySteps()
        {
            using (var form = new SPI_FK_Extraction())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                var connection = Find<ConnectionPanel>(form, "connection");
                Fill(connection.Source, false); flow.Refresh();
                Locked(form, "btn_TestDBConnection", "btn_ImportExcel");
                Fill(connection.Source, true); flow.Refresh();
                Open(form, "btn_TestDBConnection");
                Locked(form, "btn_ImportExcel");
                var status = new DBConnectionStatus { boolConnection = true };
                Set(form, "dbSRCConnection", status);
                Set(form, "boolSRCConnection", true);
                flow.Refresh();
                Open(form, "btn_ImportExcel");
            }
        }

        private static void ExtractionSteps()
        {
            using (var form = new Table_Extraction_After_Delta_Form())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                var connection = Find<ConnectionPanel>(form, "connection");
                Fill(connection.Source, false);
                Fill(connection.Target, false);
                flow.Refresh();
                Require(!connection.Target.Group.Enabled, "Target connection open while Source is empty");
                Locked(form, "btn_TestDBConnection", "btn_Excel", "btnOLDvsNew", "btn_Chg_Num", "btn_Chg_NumVsChg_Num");
                Fill(connection.Source, true); flow.Refresh();
                Require(connection.Target.Group.Enabled, "Target connection did not unlock after Source");
                Locked(form, "btn_TestDBConnection");
                Fill(connection.Target, true); flow.Refresh();
                Open(form, "btn_TestDBConnection");
                Locked(form, "btn_Excel", "btn_Chg_Num");
                Get<DBConnectionStatus>(form, "dBConnectionStatus_SRC").boolConnection = true;
                Get<DBConnectionStatus>(form, "dBConnectionStatus_TRG").boolConnection = true;
                flow.Refresh();
                Open(form, "btn_Excel", "btnOLDvsNew", "btn_Chg_Num", "btn_Chg_NumVsChg_Num");
            }
        }

        private static void SpecNoteSteps()
        {
            using (var form = new SPI_Spec_Note_Creation_Utility())
            {
                form.CreateControl();
                StepFlow flow = Get<StepFlow>(form, "steps");
                flow.Refresh();
                Locked(form, "btn_Spec_Create", "btn_GenerateExcel");
                var input = new DataSet();
                var table = new DataTable("Spec");
                foreach (string column in new[] { "CMPNT_ID", "B", "C", "Size" }) table.Columns.Add(column);
                table.Rows.Add("1", "b", "c", "2in");
                input.Tables.Add(table);
                Set(form, "ds_SpecNote", input);
                flow.Refresh();
                Open(form, "btn_Spec_Create");
                Locked(form, "btn_GenerateExcel");
                Set(form, "dt_SpecNote", SPI_Spec_Note_Creation_Utility.BuildNotes(table, form));
                flow.Refresh();
                Open(form, "btn_GenerateExcel");
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
