# Task workspaces

The first-phase consolidation tracked in [issue #65](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/65) changes how existing tasks are opened and grouped. It preserves the comparison, analysis, migration, generation, execution and export engines. It does not turn similar-looking operations into one algorithm or delete a working capability.

## Workspace map

| Workspace | Tasks | Explicit routes |
|---|---|---:|
| Comparison | Database comparison; Excel comparison; Excel Sheet Audit; Workbook identity check; Batch comparison | 5 |
| Analysis & Migration | Specification gaps; Specification notes; Track data changes; Map legacy identifiers; Discover relationships; Extract after delta | 6 |
| Pre-merge Checks | Integrity & one-database checks; Cross-database duplicates; Configuration | 3 |
| SQL Scripts | Generate scripts; Execute scripts & queries | 2 |
| Data & Reports | Database to Excel; Excel column selection; Merge log reports | 3 |
| Settings | Preferences, rules & updates; Connection profiles | 2 |
| **Total** | **20 distinct directly launched implementation types plus the separate Configuration mode** | **21** |

The main window's six themed `TileButton` home tiles open each workspace's first task. Its menu exposes every explicit task. The left navigation within a workspace switches between tasks without clearing inputs or results. No workflow page is created until it is selected, and choosing a workspace does not automatically run database queries, comparisons or report exports.

## Preserved colour themes

The workspace shell keeps the existing **Ocean** (default), **Berry**, **Forest** and **Midnight** (dark) themes. No palette is removed or replaced. Choose **Settings > Preferences, rules & updates > General > Colour theme**; the General tab remains part of the existing preferences page, not another theme-specific form.

Theme changes apply live to the six home tiles, workspace navigation/header/status areas and loaded workflow pages, including pages that are not currently visible. Newly opened tasks inherit the current palette. Recolouring does not recreate a page, clear its inputs/results or start an operation. Source/Target tints and semantic error/success colours remain distinct. Open Settings pickers stay synchronized, including the deliberate modal Import Rules dialog.

The choice remains in `%AppData%\SPI Data Comparison Tool\theme.txt`, outside the installation folder. Ocean is the safe fallback for a missing, unreadable, unknown or oversized saved value. A failed save is reported while keeping the selected theme for the current session. Native Windows controls retain the rendering limitations described in [PROJECT_GUIDE.md](PROJECT_GUIDE.md); this phase does not claim complete replacement of native control drawing.

## Exact route inventory

| Route key | Implementation | Retained distinction |
|---|---|---|
| `comparison.database` | `DB_Comparison_Form` | Database inputs and existing comparison modes. |
| `comparison.excel` | `Excel_Excel_Comparison` | Workbook inputs; primary-key and non-primary-key modes remain. |
| `comparison.excel-audit` | `ExcelSheetAuditForm` | Separate Design A audit of selected worksheets, optional mapped single/composite keys, formatting/layout and a two-sheet report. No existing engine is replaced. |
| `comparison.identity` | `WorkbookIdentityForm` | Answers whether two workbooks are identical: all sheets or one sheet, rows by position or by a ticked primary key, 18 checks on screen and a five-sheet report. It uses its own engine; no existing task is replaced. |
| `comparison.batch` | `Compare_Multiples_List_Tables_Forms` | Table/file lists, per-item processing and batch reports. |
| `analysis.gaps` | `SPI_Analysis` | Connection setup and embedded results; form-pair alignment and export. |
| `analysis.notes` | `SPI_Spec_Note_Creation_Utility` | Excel-to-note transformation; no database connection is required. |
| `analysis.changes` | `SPIDeltaChangeTrackerOptimised` | Existing delta/date rules and detailed reports. |
| `analysis.legacy` | `SPIDeltaChangeLegacyID` | SQL Server reference lookup and replacement-identifier workbook. |
| `analysis.relationships` | `SPI_FK_Extraction` | Existing name-based candidate discovery, not confirmed foreign-key constraints. |
| `analysis.extraction` | `Table_Extraction_After_Delta_Form` | Existing four identifier/change-number mapping and extraction modes. |
| `premerge.integrity` | `PreMergingTool_Form` | Orphan/reference checks and duplicates within one database. |
| `premerge.duplicates` | `Pre_Merging_Duplicacy_Form` | Duplicate comparison across two environments. |
| `premerge.configuration` | `Pre_Merging_Duplicacy_Form(true)` | Separate configuration state, required schema and output columns. |
| `scripts.generate` | `Generate_IUD_Script_Form` | Generates a workbook of SQL; does not execute it. |
| `scripts.execute` | `Execute_Excel_Scripts_Form` | Explicit SQL execution and query/report export operations. |
| `reports.database` | `SQL_DB_To_Excel` | Selected database-table exports. |
| `reports.excel` | `CustomisedExcel` | Column projection from the first input worksheet. |
| `reports.logs` | `MergeLogsExport` | Merge-log parsing into summaries and rejected-record reports. |
| `settings.preferences` | `User_Settings` | Existing General, Comparison rules and Updates tabs. |
| `settings.profiles` | `UserProfileForm` | Real profile management with existing save/cancel safeguards. |

## Session and close behaviour

| Action | Behaviour |
|---|---|
| Reopen a task from the main menu | Activate its default workspace/page, restoring a minimized workspace. |
| Switch to another task | Keep the previous page's state and any running job; only its visibility changes. |
| Ctrl when opening a supported task / Open separate session | Create a separate workspace session with independent task inputs; do not overwrite the default session. |
| Preferences, profiles or Configuration | No explicit parallel session; Configuration and duplicate checking still have separate route state. |
| Open Advanced mapping from Database/Excel comparison | Create a transient page in that Comparison workspace using fresh copied input tables. Each mapping keeps its own state; closing it removes its transient navigation entry. Standalone callers still work. |
| Close task | Run the page's normal close guards. A busy page can refuse closure; other tasks remain open. |
| Close a workspace / exit from the main window | Prepare the loaded pages first. If a page refuses, show that page and keep existing work. Successful closure then completes page cleanup. |
| Enter / Escape | Respect the selected page's normal dialog-key behaviour. No workspace-wide Enter action starts a job or an update. |

Settings contains two task pages, not one combined profile/preferences editor. **Help > Check for updates** activates **Settings > Preferences, rules & updates > Updates**. Comparison **Import Rules** keeps a deliberate short-lived modal `User_Settings(true)` dialog: its synchronous caller receives the imported dataset only after closing it. That modal path is not the persistent Settings workspace and does not introduce another rules engine.

Installation is blocked by data workspaces, profile tasks and busy workers, including hidden pages. See [UPDATING.md](UPDATING.md) before installing or downgrading.

## Shared controls and removed scaffolding

The shared `ConnectionPanel` is now used by 12 connection surfaces, including Gap Analysis and the Optimised Tracker. Gap Analysis retains Oracle/SQL Server Source and SQL Server Target support. Its table/query drafts survive switching modes; **Reset to specification query** is the explicit way to restore the default template. Page-specific data and jobs remain independent even when the presentation component is shared.

Only verified obsolete UI scaffolding was retired: the hidden Database Comparison dummy-profile prototype, the inactive Excel multithreading scaffold, and the Optimised Tracker's unconnected commented profile-save handler. The active profile editor, ordinary comparison worker and instrument worker remain. Advanced Mapping's displayed Target dimensions now come from its actual copied target table.

The former three-group `ToolGroupWindow` host is superseded by the common workspace host. Its nine workflows remain available through the route inventory above: pre-merge modes stay in Pre-merge Checks, notes and delta tasks use Analysis & Migration, script generation uses SQL Scripts, and Excel/log exports use Data & Reports. Lazy creation, page reuse, Configuration's separate state, readable window titles and the all-or-none busy-close safeguard remain. The retained `--check-groups` runner verifies these workflows against the workspace implementation.

## What this phase does not claim

- The 21 existing concrete Form implementations remain. The common `WorkspaceForm`, the separate `ExcelSheetAuditForm` and the `WorkbookIdentityForm` make **24 concrete Form classes**; the abstract `WorkspacePageForm` is a lifecycle base, not another opening window. Normal navigation uses fewer independent workflow windows, not fewer business engines or source classes.
- Comparison algorithms, numeric-header matching, foreign-key inference and report schemas are not replaced by this interface change. Inferred relationships must not be treated as declared constraints, and custom SQL must not be assumed read-only.
- SQL generation and execution remain separate. The reverted PR #18 preview/transaction/rollback behaviour is not restored; see [COMPARISON_QA.md](COMPARISON_QA.md).
- A shared host is not a complete visual redesign of every older page. Rendering, keyboard use, scaling and real-world workflows still require Windows acceptance.
- Retiring old implementation classes or unifying engines requires representative before/after output fixtures, not just a reduced menu count.

## Verification

`WorkspaceUiChecks` is configured in the offline Windows UI runner to check the 21 routes, lazy construction, independent modes/sessions, transient mappings, failed creation, guarded close/exit, hidden workers and updater eligibility. The retained palette/source contracts and `ThemeChecks` runner cover four-theme consistency, eligible windows, live changes and temporary-file preference checks. The separate `--check-excel-audit` suite uses temporary workbook fixtures for the new audit; see [EXCEL_SHEET_AUDIT.md](EXCEL_SHEET_AUDIT.md). Existing form-opening checks and validation suites remain; [VALIDATION.md](VALIDATION.md) describes their coverage. These are configured checks, not a claim that a particular commit's continuous integration run has passed. They do not connect to live databases, execute user SQL or prove complete business-output parity.
