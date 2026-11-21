using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    internal enum WorkspaceKind { Comparison, Analysis, PreMerge, Scripts, DataReports, Settings }

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
            new WorkspaceTask("comparison.batch", "Batch comparison", "Compare a list of database tables or Excel files; retain per-item and batch reports.",
                WorkspaceKind.Comparison, () => new Compare_Multiples_List_Tables_Forms()),
            new WorkspaceTask("analysis.gaps", "Specification gaps", "Load specification metadata, arrange form pairs and export differences.",
                WorkspaceKind.Analysis, () => new SPI_Analysis()),
            new WorkspaceTask("analysis.notes", "Specification notes", "Create notes from an Excel workbook. No database connection is required.",
                WorkspaceKind.Analysis, () => new SPI_Spec_Note_Creation_Utility()),
            new WorkspaceTask("analysis.changes", "Track data changes", "Compare source and target data with the existing delta and date rules.",
                WorkspaceKind.Analysis, () => new SPIDeltaChangeTrackerOptimised()),
            new WorkspaceTask("analysis.legacy", "Map legacy identifiers", "Look up replacement identifiers and export the existing mapping workbook.",
                WorkspaceKind.Analysis, () => new SPIDeltaChangeLegacyID()),
            new WorkspaceTask("analysis.relationships", "Discover relationships", "Review candidate relationships inferred from column names; these are not confirmed foreign keys.",
                WorkspaceKind.Analysis, () => new SPI_FK_Extraction()),
            new WorkspaceTask("analysis.extraction", "Extract after delta", "Use the existing identifier/change-number mapping and table extraction modes.",
                WorkspaceKind.Analysis, () => new Table_Extraction_After_Delta_Form()),
            new WorkspaceTask("premerge.integrity", "Integrity & one-database checks", "Check orphan references and duplicates within one database.",
                WorkspaceKind.PreMerge, () => new PreMergingTool_Form()),
            new WorkspaceTask("premerge.duplicates", "Cross-database duplicates", "Compare duplicate entries across source and target environments.",
                WorkspaceKind.PreMerge, () => new Pre_Merging_Duplicacy_Form()),
            new WorkspaceTask("premerge.configuration", "Configuration", "Compare configuration with its own inputs and output columns.",
                WorkspaceKind.PreMerge, () => new Pre_Merging_Duplicacy_Form(true), false),
            new WorkspaceTask("scripts.generate", "Generate scripts", "Create INSERT, UPDATE or DELETE scripts in a workbook. This task does not execute them.",
                WorkspaceKind.Scripts, () => new Generate_IUD_Script_Form()),
            new WorkspaceTask("scripts.execute", "Execute scripts & queries", "Explicit database execution and query/report exports. Commands can change data; use authorised databases only.",
                WorkspaceKind.Scripts, () => new Execute_Excel_Scripts_Form()),
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
                case WorkspaceKind.Analysis: return "Analysis & Migration";
                case WorkspaceKind.PreMerge: return "Pre-merge Checks";
                case WorkspaceKind.Scripts: return "SQL Scripts";
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
