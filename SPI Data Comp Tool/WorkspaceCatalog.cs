using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    internal enum WorkspaceKind { Comparison, DataReports, Settings }

    internal sealed class WorkspaceTask
    {
        internal WorkspaceTask(string key, string title, string description, WorkspaceKind workspace,
            Func<WorkspacePageForm> create, bool allowSeparateSession = true)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("A task needs a stable key.", nameof(key));
            Key = key;
            Title = title;
            Description = description;
            Workspace = workspace;
            CreatePage = create ?? throw new ArgumentNullException(nameof(create));
            AllowSeparateSession = allowSeparateSession;
        }

        internal string Key { get; }
        internal string Title { get; }
        internal string Description { get; }
        internal WorkspaceKind Workspace { get; }
        internal Func<WorkspacePageForm> CreatePage { get; }
        internal bool AllowSeparateSession { get; }
    }

    /// <summary>
    /// The rail's tasks, each reachable exactly once. "Compare databases" and "Compare workbooks" choose an
    /// existing workflow with their mode bar (<see cref="Modes"/>); no database is opened by the catalog.
    /// </summary>
    internal static class WorkspaceCatalog
    {
        private static readonly ReadOnlyCollection<WorkspaceTask> modes = Array.AsReadOnly(new[]
        {
            new WorkspaceTask("comparison.database", "One table", "Compare two database tables. Advanced mapping stays with this session.",
                WorkspaceKind.Comparison, () => new DB_Comparison_Form()),
            new WorkspaceTask("comparison.batch", "A list of tables", "Compare every database table named in a list; one report per table plus a summary.",
                WorkspaceKind.Comparison, () => new Compare_Multiples_List_Tables_Forms(true)),
            new WorkspaceTask("comparison.excel", "Values", "Compare two workbooks, with optional keys and advanced column mappings.",
                WorkspaceKind.Comparison, () => new Excel_Excel_Comparison()),
            new WorkspaceTask("comparison.excel-audit", "Values and formatting", "Audit two worksheets: values, headers, formatting and layout.",
                WorkspaceKind.Comparison, () => new ExcelSheetAuditForm()),
            new WorkspaceTask("comparison.identity", "Identical?", "Find out whether two workbooks are identical: headers, values, formatting and layout, with a five-sheet report.",
                WorkspaceKind.Comparison, () => new WorkbookIdentityForm()),
            new WorkspaceTask("comparison.batch-folders", "Two folders", "Compare the workbooks of two folders by value; one report per file plus a summary.",
                WorkspaceKind.Comparison, () => new Compare_Multiples_List_Tables_Forms(false)),
        });

        private static readonly ReadOnlyCollection<WorkspaceTask> tasks = Array.AsReadOnly(new[]
        {
            new WorkspaceTask("comparison.databases", "Compare databases", "Find rows that differ between a Source and a Target table, or for every table in a list.",
                WorkspaceKind.Comparison, () => CompareModePage.Databases(CreateMode)),
            new WorkspaceTask("comparison.workbooks", "Compare workbooks", "One tool for value comparison, full sheet audit and identity check, for one pair or two folders.",
                WorkspaceKind.Comparison, () => CompareModePage.Workbooks(CreateMode)),
            new WorkspaceTask("reports.database", "Database to Excel", "Export selected database tables with the existing output options.",
                WorkspaceKind.DataReports, () => new SQL_DB_To_Excel()),
            new WorkspaceTask("reports.excel", "Excel column selection", "Choose columns from the first worksheet and export a customised workbook.",
                WorkspaceKind.DataReports, () => new CustomisedExcel()),
            new WorkspaceTask("reports.logs", "Merge log reports", "Convert merge logs into summary and rejected-record worksheets.",
                WorkspaceKind.DataReports, () => new MergeLogsExport()),
            new WorkspaceTask("settings.preferences", "Preferences, rules & updates", "General preferences, comparison rules and explicit Normal/Beta updates.",
                WorkspaceKind.Settings, () => new User_Settings(), false),
            new WorkspaceTask("settings.profiles", "Connection profiles", "Manage saved connection profiles; keep password and save/cancel safeguards.",
                WorkspaceKind.Settings, () => new UserProfileForm(), false),
        });

        /// <summary>The workflows behind the two Compare tasks' mode bars. They are not rail entries.</summary>
        internal static IReadOnlyList<WorkspaceTask> Modes => modes;

        private static WorkspacePageForm CreateMode(string key) => FindMode(key).CreatePage();

        /// <summary>The rail task and mode-bar choices that show a comparison mode.</summary>
        internal static string TaskOfMode(string modeKey, out string[] options)
        {
            switch (modeKey)
            {
                case "comparison.database": options = new[] { "table" }; return "comparison.databases";
                case "comparison.batch": options = new[] { "list" }; return "comparison.databases";
                case "comparison.excel": options = new[] { "values", "pair" }; return "comparison.workbooks";
                case "comparison.excel-audit": options = new[] { "formatting", "pair" }; return "comparison.workbooks";
                case "comparison.identity": options = new[] { "identical", "pair" }; return "comparison.workbooks";
                case "comparison.batch-folders": options = new[] { "values", "folders" }; return "comparison.workbooks";
                default: throw new ArgumentException("Unknown comparison mode: " + modeKey, nameof(modeKey));
            }
        }

        internal static WorkspaceTask FindMode(string key)
        {
            var mode = modes.SingleOrDefault(item => item.Key == key);
            if (mode == null) throw new ArgumentException("Unknown comparison mode: " + key, nameof(key));
            return mode;
        }

        internal static IReadOnlyList<WorkspaceTask> Tasks => tasks;

        /// <summary>A rail task, or one of the comparison modes behind the two Compare tasks.</summary>
        internal static WorkspaceTask Find(string key)
        {
            var task = tasks.SingleOrDefault(item => item.Key == key) ?? modes.SingleOrDefault(item => item.Key == key);
            if (task == null) throw new ArgumentException("Unknown workspace task: " + key, nameof(key));
            return task;
        }

        internal static IEnumerable<WorkspaceTask> TasksFor(WorkspaceKind workspace)
        {
            return tasks.Where(item => item.Workspace == workspace);
        }

        internal static string Title(WorkspaceKind workspace)
        {
            switch (workspace)
            {
                case WorkspaceKind.Comparison: return "Comparison";
                case WorkspaceKind.DataReports: return "Data & Reports";
                case WorkspaceKind.Settings: return "Settings";
                default: throw new ArgumentOutOfRangeException(nameof(workspace));
            }
        }

        internal static void OpenAdvancedMapping(WorkspacePageForm source, Func<ViewForMultiReference> create)
        {
            if (source.HostWorkspace != null && !source.HostWorkspace.IsDisposed)
            {
                // Always take a new independent snapshot; a previous mapping may refer to different inputs.
                source.HostWorkspace.OpenTransient("Advanced mapping", () => create());
                return;
            }
            // Preserve standalone/synthetic callers while migration to workspace pages is staged.
            var view = create();
            try { view.Show(source); }
            catch { view.Dispose(); throw; }
        }
    }
}
