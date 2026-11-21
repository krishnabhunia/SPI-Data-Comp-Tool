# Task workspaces

The application has three workspaces for comparison, data/report exports and settings. The common workspace host introduced in [issue #65](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/65) still preserves each retained task's inputs, results and close guards.

Analysis & Migration and Pre-merge Checks are retired. Their nine routes, eight task-form implementations and dedicated setup/results controls, layouts and resources have been removed from the current application. They are not hidden tasks or alternate launch modes. [Release v2.16.0](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/releases/tag/v2.16.0) preserves the earlier application for anyone who needs those workflows.

SQL Scripts is also retired. Its Generate scripts and Execute scripts & queries tasks, form implementations, layouts, resources and SQL report templates are removed. Oracle/SQL Server comparison and SQL Server database-to-Excel export remain supported tasks. See [UPDATING.md](UPDATING.md) for the two empty portable-package compatibility files used by older updaters; they do not contain SQL or restore a retired task.

## Workspace map

| Workspace | Tasks | Explicit routes |
|---|---|---:|
| Comparison | Database comparison; Excel comparison; Excel Sheet Audit; Workbook identity check; Batch comparison | 5 |
| Data & Reports | Database to Excel; Excel column selection; Merge log reports | 3 |
| Settings | Preferences, rules & updates; Connection profiles | 2 |
| **Total** | **10 distinct directly launched implementation types** | **10** |

The main window's left rail lists every current task once, grouped by workspace, and is the only way to open a task: there is no menu bar, no home tiles and no Help menu. Tasks open inside the main window; switching between them does not clear inputs or results. No workflow page is created until it is selected, and choosing a workspace does not automatically run database queries, comparisons or report exports.

## Preserved colour themes

The workspace shell keeps the existing **Ocean** (default), **Berry**, **Forest** and **Midnight** (dark) themes. No palette is removed or replaced. Choose **Settings > Preferences, rules & updates > General > Colour theme**; the General tab remains part of the existing preferences page, not another theme-specific form.

Theme changes apply live to the rail, header/status areas and loaded workflow pages, including pages that are not currently visible. Newly opened tasks inherit the current palette. Recolouring does not recreate a page, clear its inputs/results or start an operation. Source/Target tints and semantic error/success colours remain distinct. Open Settings pickers stay synchronized, including several Settings windows opened by checks.

The choice remains in `%AppData%\SPI Data Comparison Tool\theme.txt`, outside the installation folder. Ocean is the safe fallback for a missing, unreadable, unknown or oversized saved value. A failed save is reported while keeping the selected theme for the current session. Native Windows controls retain the rendering limitations described in [PROJECT_GUIDE.md](PROJECT_GUIDE.md).

## Exact route inventory

| Route key | Implementation | Retained distinction |
|---|---|---|
| `comparison.database` | `DB_Comparison_Form` | Database inputs and existing comparison modes. |
| `comparison.excel` | `Excel_Excel_Comparison` | Workbook inputs; primary-key and non-primary-key modes remain. |
| `comparison.excel-audit` | `ExcelSheetAuditForm` | Separate Design A audit of selected worksheets, optional mapped single/composite keys, formatting/layout and a two-sheet report. No existing engine is replaced. |
| `comparison.identity` | `WorkbookIdentityForm` | Answers whether two workbooks are identical: all sheets or one sheet, rows by position or by a ticked primary key, 18 checks on screen and a five-sheet report. It uses its own engine; no existing task is replaced. |
| `comparison.batch` | `Compare_Multiples_List_Tables_Forms` | Table/file lists, per-item processing and batch reports. |
| `reports.database` | `SQL_DB_To_Excel` | Selected database-table exports. |
| `reports.excel` | `CustomisedExcel` | Column projection from the first input worksheet. |
| `reports.logs` | `MergeLogsExport` | Merge-log parsing into summaries and rejected-record reports. |
| `settings.preferences` | `User_Settings` | Existing General, Comparison rules and Updates tabs. |
| `settings.profiles` | `UserProfileForm` | Real profile management with existing save/cancel safeguards. |

## Session and close behaviour

| Action | Behaviour |
|---|---|
| Reopen a task from the rail | Activate its existing page in the main window. |
| Switch to another task | Keep the previous page's state and any running job; only its visibility changes. |
| New session (supported tasks only) | Open the task in a separate window — one task, no rail — with independent inputs; the main window's page is unchanged. There is no Ctrl+click shortcut. |
| Preferences or profiles | No explicit parallel session; each Settings task retains its own state. |
| Open Advanced mapping from Database/Excel comparison | Create a transient page in that Comparison workspace using fresh copied input tables. Each mapping keeps its own state; closing it removes its transient navigation entry. Standalone callers still work. |
| Close task | Run the page's normal close guards. A busy page can refuse closure; other tasks remain open. |
| Close a workspace / exit from the main window | Prepare the loaded pages first. If a page refuses, show that page and keep existing work. Successful closure then completes page cleanup. |
| Enter / Escape | Respect the selected page's normal dialog-key behaviour. No workspace-wide Enter action starts a job or an update. |

Settings contains two task pages, not one combined profile/preferences editor. Updates and About are tabs of **Settings > Preferences, rules & updates**; the main window has no Help menu. Rules have one route: **Settings › Rules**. Comparison pages have no Import Rules button; they show a read-only *Rules: <file> (set in Settings)* line and read the saved file when **Include rules** is ticked.

Installation is blocked by data workspaces, profile tasks and busy workers, including hidden pages. See [UPDATING.md](UPDATING.md) before installing or downgrading.

## Shared controls and retained workflows

The shared `ConnectionPanel` serves the retained database comparison, batch comparison, database export and profile surfaces. Page-specific data and jobs remain independent even when they use the same presentation component. The active profile editor and comparison workers remain. Advanced Mapping's displayed Target dimensions come from its actual copied target table.

The common workspace host provides lazy page creation, page reuse, readable window titles and all-or-none close preparation for the retained routes. Settings preferences and profiles keep their separate state. Retired routes are absent from the catalog and their form classes cannot be opened by an alternate menu or self-test launch.

## Scope and boundaries

- The current application has **14 concrete Form classes**: 10 routed task implementations, `MainForm`, `AboutBox1`, `ViewForMultiReference` and `WorkspaceForm`. The abstract `WorkspacePageForm` is a lifecycle base, not another opening window.
- The retained comparison algorithms, advanced mappings and report schemas remain separate. The Excel audit and workbook identity check continue to have their own engines and documented scopes.
- SQL generation, arbitrary script execution and their report-template actions are absent from the current application. Retained database comparison/export still use their own database queries; see [COMPARISON_QA.md](COMPARISON_QA.md).
- A shared host is not a complete visual redesign of every older page. Rendering, keyboard use, scaling and real-world workflows still require Windows acceptance.
- Current builds intentionally omit the retired business workflows. Their earlier implementations remain in historical releases, not behind a compatibility switch in this application. The earlier retirement archive at v2.16.0 remains unchanged.

## Verification

`WorkspaceUiChecks` is configured in the offline Windows UI runner to check the 10 routes across three workspaces, lazy construction, independent sessions, transient mappings, failed creation, guarded close/exit, hidden workers and updater eligibility. `StartupChecks` covers 40 saved-profile/navigation scenarios across the retained routes. The retained palette/source contracts and `ThemeChecks` runner cover four-theme consistency, eligible windows, live changes and temporary-file preference checks. The separate `--check-excel-audit` suite uses temporary workbook fixtures; see [EXCEL_SHEET_AUDIT.md](EXCEL_SHEET_AUDIT.md). Form-opening checks and validation suites cover the retained implementation; [VALIDATION.md](VALIDATION.md) describes their scope. These are configured checks, not a claim that a particular commit's continuous integration run has passed. They do not connect to live databases or prove complete business-output parity.
