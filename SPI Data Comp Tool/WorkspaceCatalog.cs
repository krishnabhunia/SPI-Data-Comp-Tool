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

    /// <summary>One explicit route for each existing workflow/mode. No database is opened by the catalog.</summary>
    internal static class WorkspaceCatalog
    {
        private static readonly ReadOnlyCollection<WorkspaceTask> tasks = Array.AsReadOnly(new[]
        {
            new WorkspaceTask("comparison.database", "Database comparison", "Compare two database tables. Advanced mapping stays with this session.",
                WorkspaceKind.Comparison, () => new DB_Comparison_Form()),
            new WorkspaceTask("comparison.excel", "Excel comparison", "Compare two workbooks, with optional keys and advanced column mappings.",
                WorkspaceKind.Comparison, () => new Excel_Excel_Comparison()),
            new WorkspaceTask("comparison.excel-audit", "Excel Sheet Audit", "Audit two worksheets: values, headers, formatting and layout. The existing Excel comparison remains unchanged.",
                WorkspaceKind.Comparison, () => new ExcelSheetAuditForm()),
            new WorkspaceTask("comparison.identity", "Workbook identity check", "Find out whether two workbooks are identical: headers, values, formatting and layout, with a five-sheet report.",
                WorkspaceKind.Comparison, () => new WorkbookIdentityForm()),
            new WorkspaceTask("comparison.batch", "Batch comparison", "Compare a list of database tables or Excel files; retain per-item and batch reports.",
                WorkspaceKind.Comparison, () => new Compare_Multiples_List_Tables_Forms()),
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

        internal static IReadOnlyList<WorkspaceTask> Tasks => tasks;

        internal static WorkspaceTask Find(string key)
        {
            var task = tasks.SingleOrDefault(item => item.Key == key);
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
