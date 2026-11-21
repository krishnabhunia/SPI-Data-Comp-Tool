# Form and workflow validation

This inventory covers the 14 concrete production Form classes and their shared controls after retirement of the SQL Scripts, Analysis & Migration and Pre-merge Checks workspaces. `WorkspacePageForm` is an abstract lifecycle base, not another independently opened screen. A screen with no editable input is audited for navigation/lifecycle behaviour rather than given artificial required fields. Existing updater safeguards remain in use; the workspace host does not add a second updater or authentication system. See [WORKSPACES.md](WORKSPACES.md) for the three workspaces and 10 explicit task routes.

## Common behaviour

- Validate on the action itself. Enabled buttons and a previously successful connection test are not proof that inputs remain valid.
- Show a non-blinking field error, reveal its tab and focus the field where possible. Correcting text clears stale feedback; the next action still validates again. Keyboard navigation is not trapped by modal validation dialogs.
- Validate active fields only. SQL Server Windows authentication ignores inactive credentials and Oracle port; source-only and Excel-source modes do not require an unused database connection.
- Build connection strings with provider builders. Password whitespace, quotes and semicolons remain literal; validation/error messages do not echo credentials.
- Validate imported candidates before replacing current data. File cancellation or invalid input must not silently replace a valid active workbook/profile/rule set.
- Treat remembered profiles as optional startup input. The shared reader validates the complete bounded document before publishing it; an unreadable setting or missing, malformed or locked file shows recovery guidance while retaining the active catalog and manual connection fields.
- Keep a valid imported or saved profile catalog usable when remembering its path fails. Associate the catalog with its actual file, preserve it across later form openings, and reject stale editor writes. Failed shared-file fallback must show the error instead of silently appearing empty.
- Finish Settings initialization when optional saved preferences cannot be read. Leave unavailable rules unselected and choose one valid Excel writer (EPPlus by default); failed preference saves report the problem without resetting configuration files.
- Keep diagnostic failures out of workflow startup and error reporting. Logging tries Desktop first, then a per-user fallback; unavailable folders or failed writes must not prevent a form from opening or displaying its original error.
- Prevent overlapping background operations. Lock editable inputs while the operation runs, retain relevant Cancel controls, restore controls before setting final completion state, and block closing a running worker.
- Keep each hosted task's inputs/results independent. Switching tasks must not reset a page, bypass its close guards or hide a running worker from installation eligibility checks. Explicit parallel sessions must not replace the default session.
- Preserve Ocean, Berry, Forest and Midnight when grouping pages into workspaces. A live theme change updates presentation, not a task's inputs, results or running operation; already-loaded hidden pages and later-created pages must use the selected palette.
- Treat cancelled, failed and partially failed runs separately from successful completion.
- Validate Excel dimensions, sheet names and destinations before exporting. Shared report writers publish a complete same-directory temporary file with atomic replacement; failed writes leave the existing destination intact.

## Form coverage

| Form class | Validation or retained safeguards |
|---|---|
| `MainForm` | One left rail (every catalog task, listed once), tasks hosted in the main window, redacted errors and all-workspace exit preflight. No menu bar, tiles or data-entry fields. |
| `WorkspaceForm` | Lazy task creation, retained page state, selected-task keyboard behaviour, failed-factory preservation, close preparation and transient mapping cleanup. A blocked close selects the refusing task without disposing the other pages. |
| `AboutBox1` | Read-only information; existing open/close smoke coverage. No artificial validation fields. |
| `UserProfileForm` | Required unique profile names, applicable connection fields, shared bounded JSON loading, usable imports despite failed path persistence, actual-file provenance, stale-editor/file checks and password-free atomic exports. |
| `User_Settings` | Rules workbook/path/schema, required row values, unique identifiers and supported rule types; failed imports retain current rules. Four retained colour choices on General, live selection/picker synchronization and explicit failure-to-save feedback. Existing update channel/version/download/install validation retained. |
| `DB_Comparison_Form` | Source/Target mode, connection freshness, loaded table and comparison-column selections, busy state and export bounds. Correct provider connection is used on each side. |
| `Compare_Multiples_List_Tables_Forms` | Current connections, batch workbook schema/rows, selection validity, worker lifecycle and partial-failure reporting. |
| `SQL_DB_To_Excel` | Current SQL Server connection, selected table metadata, output folder, per-sheet limits, safe names and atomic output; worker inputs are captured before background work. |
| `Excel_Excel_Comparison` | Current workbook data, valid matching columns and key selections, null/duplicate composite keys in primary-key mode, non-primary-key mode compatibility, stale results and busy states. |
| `ExcelSheetAuditForm` | Separate two-file/worksheet/header inputs; explicit case-insensitive key-column mappings; complete null/duplicate key validation; captured worker requests, cancellation and busy-close guards; independently capped data/non-data results; two-sheet atomic report export with input-path protection and overwrite confirmation. See [EXCEL_SHEET_AUDIT.md](EXCEL_SHEET_AUDIT.md). |
| `WorkbookIdentityForm` | Two readable `.xlsx`/`.xlsm` workbooks that have been read by the form; a sheet in both files or all sheets in both; with a primary key, the same number of ticked key columns in both files; number tolerance and up to 8 difference thresholds as numbers of 0 or more; an `.xlsx` report that is neither input; confirmation before an existing report is replaced; captured request, background worker, cancellation, atomic report writing and a busy-close guard. See [WORKBOOK_IDENTITY_CHECK.md](WORKBOOK_IDENTITY_CHECK.md). |
| `ViewForMultiReference` | Required selected reference columns, current data and compatible mapping selections; count labels describe its actual copied Source/Target tables. Each hosted mapping session keeps an independent snapshot. |
| `CustomisedExcel` | Readable workbook, first-worksheet data and selected columns, candidate validation and valid export data. |
| `MergeLogsExport` | Selected readable bounded log file, complete valid records, safe output path and candidate-before-publication handling. Shared file selection derives names from the chosen file, including short `.log` names under a multi-extension filter. |

Shared coverage: `ConnectionSide`, `ConnectionPanel`, `ProfileStore`, `AppPreferences`, `StepFlow`, `Logs`, `FormHelpers`, `ReadExcel`, `WriteExcel` and `FormValidation`. `UpdatesControl` keeps the release selection, checksum, cancellation and installation gates from the updater implementation.

`WorkspaceNavigator` and `WorkspacePageForm` provide shared navigation/lifecycle behaviour. Settings preferences and profiles are separate task pages in one workspace. The existing modal Import Rules path intentionally remains a short-lived `User_Settings(true)` dialog so its caller receives imported rules synchronously; cancelling or failing an import still preserves the caller's rules.

The retained Excel column selection and merge-log export tasks use the common workspace infrastructure. A busy or refusing task keeps every loaded page and its draft state, including pages before and after it in the close order, and becomes the active page. The retired workspaces and their dedicated forms are absent from the catalog and current build. SQL Server database comparison/export and their shared connection validation remain available.

## Compatibility and explicit boundaries

| Area | Rule / boundary |
|---|---|
| Oracle port | ASCII whole number, 1–65,535. SQL Server does not validate an inactive Oracle port. |
| Passwords | Preserve exact entered characters, including leading/trailing spaces. No built-in credential policy is imposed beyond applicable required input. |
| Saved profiles | Startup and explicit imports share `ProfileStore.ReadFile`; `.json` extension matching ignores case. Legacy `str_DBType` values restore Oracle/SQL selection when both Boolean provider flags are absent or false; conflicting provider/authentication values remain invalid. Loading names does not apply a profile or replace entered fields. Failures point to **Settings > Connection profiles > Load profile** and do not overwrite the rejected file or settings. |
| Excel imports | Open XML `.xlsx` / `.xlsm`; reject empty/corrupt files, duplicate headers after trimming/ignoring case, and populated columns without headers. Unused blank spacer columns/empty sheets may be omitted. Header-only sheets can remain available; actions requiring data reject them at that action. |
| Header compatibility | The existing static reader keeps trimmed/uppercase headers; the case-preserving instance reader keeps original spelling/spaces. Both share schema validation. |
| Existing comparison engine | In primary-key mode require non-null unique composite keys. Non-primary-key mode remains available. Selected headers containing commas or leading/trailing spaces are rejected with guidance because the existing comparison engine tokenizes them. |
| Separate Excel Sheet Audit | Headers match ignoring case only; whitespace remains significant. Key mappings are ordered typed tuples, not delimiter-concatenated text. Invalid keys stop the run. Without keys, logical row positions align below independently selected header rows. Values/formulas and formatting are separate checks; unsupported semantics prevent a complete-match claim. |
| Workspace state | Task switching preserves input/result state; reopening selects the default page. Ctrl/open-separate creates an independent supported session. Preferences and profiles disallow parallel sessions and keep separate page state. |
| Controlled-step events | Readiness changes are deferred until the current UI event completes, including profile application and checked-list changes. Constructor notifications wait for a live handle; recreated handles accept new notifications, disposed pages do not run readiness callbacks, and worker requests return to the UI thread. |
| Diagnostic logs | Try `Desktop\LogFolder`, then `%LocalAppData%\SPI Data Comparison Tool\Logs`. Each session uses a distinct file and serializes concurrent messages. Creation/write/flush/close failures do not escape into the workflow; after both locations fail, logging remains disabled for that session. Public logging routes redact connection passwords. |
| Colour themes | Ocean, Berry, Forest and Midnight remain available at Settings > Preferences, rules & updates > General > Colour theme. A saved theme name is bounded to 64 bytes; missing, unreadable, oversized or unknown values fall back to Ocean. Failure to save does not undo the current-session choice. The preference file remains outside the installation folder. Native Windows rendering limitations are not removed by the workspace host. |
| Update handoff | Data workspaces and profile tasks, including hidden loaded pages, block installation. A Settings workspace containing only preferences may remain open. Closing or switching a task does not install an update. |
| Excel export | At most 16,384 columns and 1,048,575 data rows plus the header per worksheet; valid unique 1–31-character sheet names. Existing split-report workflows still apply their existing partitioning. |
| Windows paths | Reject device paths/names, invalid characters, trailing dots/spaces, missing parent folders, locked/read-only destinations and supported lexical input/output self-overwrite aliases. Validation is not a physical hard-link/reparse-point identity check. |
| Atomic output | Existing destination preserved if serialization fails. A new-file publication does not overwrite another writer that wins the race. Replacement of a previously existing file is not a compare-and-swap against unrelated concurrent edits; do not have multiple tools writing the same report. |
| Import resource limits | Profile documents: 16 MiB, JSON depth 32 and 10,000 profiles; each entry must contain Source and Target connection objects. Merge logs: 64 MiB. Existing Excel readers still load into memory. The separate audit streams worksheet cells, spools shared strings and keyed rows to disk, and applies bounded partition/detail guards; it is not a general decompression-bomb sandbox. |
| Progress | Zero/negative totals do not divide by zero; long arithmetic and clamping prevent overflow/out-of-range progress values. |

## Verification

Run source contracts and layout consistency from the repository root:

```text
python -m unittest discover -s tests -p 'test_*.py' -v
python tools/layout_gen.py --check tools/layouts/*.py
git diff --check
```

Windows builds are configured to compile both Any CPU and x64 with warnings as errors. Recognized `SelfTest` commands clear remembered profile/rules paths in memory before running, without saving or resetting user settings. Suites that need persisted-path behaviour supply temporary synthetic files and restore their in-memory overrides. Developer-mode automatic connections are disabled during checks, and unhandled UI event failures fail the process instead of leaving a waiting error dialog.

The `--check-ui <log-file>` runner includes the following validation and workspace suites:

| Suite | Main checks |
|---|---|
| `CoreValidationChecks` | Field feedback, Unicode, port boundaries, literal connection strings, file locks/permissions, atomic failure/race cases, short/Unicode/multiple-dot filenames and picker cancellation, workbook bounds, input locking, header normalization and progress. |
| `ProfileWorkflowChecks` | Actual remembered-path loading, legacy provider strings, BOM/uppercase JSON, missing/locked/malformed/null/oversized/invalidly encoded files, unavailable settings, preserved manual inputs, recovery, independent edit snapshots, password-free exports and failed atomic writes. Real editor import/save/delete checks cover failed path persistence, active-file identity and stale editor protection. |
| `ProfileLoadChecks` | Shared remembered-file fallback, visible missing/malformed/locked fallback failures and step-reason text. |
| `PreferenceChecks` | Unavailable preference reads, failed saves and rollback, consistent Excel writer defaults, fully initialized shown Settings controls and safe radio events; synthetic configuration files and injected stores leave real user settings untouched. |
| `LoggingChecks` | Injected folder/open/write/flush/close failures, Desktop fallback, exhausted locations without repeated retries, original UI error visibility, all public redaction routes, concurrent writes and separate-session files; all actual files are temporary. |
| `ConnectionValidationChecks` | Real connection/profile/settings controls; active/inactive field combinations, strict imports and stale-input invalidation. |
| `DatabaseFormValidationChecks` | Database comparison, batch comparison and SQL database export using synthetic data and connection-status fixtures; mode-specific gates, schemas, selections and partial-failure state. |
| `ExcelFormValidationChecks` | Excel comparison, multi-reference and Excel column selection; positive/negative data, key/schema checks and a distinct-target-worksheet count/snapshot fixture. Also covers real synthetic workbook reads, header collisions, Unicode, leading zeros and input preservation. |
| `WorkspaceUiChecks` | All 10 routes in three workspaces; lazy hosting, page/session reuse, transient mappings, failed construction, close vetoes, main-owner closure, hidden workers and updater eligibility. |
| `WorkbookIdentityChecks`, `WorkbookIdentityFormChecks` | The identity engine and report on synthetic workbooks; the form's route, reading of both workbooks, refused inputs, comparisons by position and by key, the written report, keeping an existing report until confirmed, and the close guard. |

The separate `--check-startup <log-file>` entry point runs `StartupChecks` through the real `WorkspaceNavigator`. All 10 routes are exercised with no remembered profile and with a valid legacy `.JSON` file. The four tasks that use saved profiles also run with missing, malformed, incomplete, null-entry and locked files: 40 route scenarios in total (10 × 2 plus 4 × 5). Each scenario checks first construction, required initialized state and later navigation/reuse; profile tasks additionally exercise selection or warning recovery and manual entry. The build workflow configures this check for both the portable executable and the installed x64 executable, with a 120-second process limit for each. This complements standalone `--open-form` smoke checks, which cannot alone detect partial initialization hidden by a constructor's error handler.

The separate `--check-excel-audit <log-file>` entry point runs `ExcelAuditChecks` against temporary Open XML fixtures: positional/single/composite-key alignment, differently named keys, invalid and duplicate keys, header/schema validation, typed exact values, saved formulas, equivalent style identifiers, font/alignment changes, detail caps, date systems, input preservation, cancellation and schema-valid two-sheet report output. It also opens the new native form without replacing the existing route. These checks run in the Windows build workflow; they are not an assertion that a particular run has passed.

The separate `--check-themes <log-file>` entry point runs `ThemeChecks`, configured to build eligible concrete forms in all four palettes, inspect actual label/button/field/grid colours, switch themes while windows are open, synchronize multiple Settings pickers and test saved choices using temporary files. Python contracts in `tests/test_themes.py` verify theme wiring and persistence boundaries; `tests/test_ui_contrast.py` checks palette contrast pairs. These complement the workspace lifecycle checks rather than prove complete rendering or business-output parity.

The separate `--check-steps <log-file>` entry point runs `StepChecks` for five retained tasks: database, Excel and batch comparison, database export and Excel column selection. It checks ordered unlocking, earlier-input invalidation, retained compatible comparison selections, conditional steps, busy-control locks and strip colours in all four themes. Shown-window regressions use actual text, checked-list and profile-selection events without manually refreshing the flow; lifecycle checks cover construction, handle recreation, disposal and requests from a worker thread. `tests/test_steps.py` checks the corresponding source, layout and CI wiring. Step readiness does not replace action-time validation.

The retained `--check-groups <log-file>` entry point runs `ToolGroupChecks` against Excel column selection and merge-log export through their current workspace routes. Its compatibility coverage includes correct page hosting/reuse and preservation of loaded pages when a hidden task refuses closure, followed by successful cleanup once the refusal clears. `tests/test_tool_groups.py` retains the corresponding route, readable-title and CI-wiring contracts. The successful `Application.Exit` scenario remains isolated in the `--check-workspace-exit <log-file>` process invoked by `WorkspaceUiChecks`.

Tests use temporary synthetic data. They do not connect to live databases, execute user SQL, install a live release or write real-user preference files. Normal diagnostic sinks are disabled in self-tests; logging checks inject temporary sinks. The existing every-form open/close checks, updater checks, embedded dependency checks and installer smoke checks remain enabled. Check the PR's actual Actions results; these configured checks are not evidence that this commit's Windows run passed.

## Manual acceptance still required

Use disposable Oracle/SQL Server databases and copies of representative workbooks to test permissions, provider/server versions, real comparison/export correctness, large files and network failures. Compare representative outputs for the retained tasks with the previous release. Verify that only the three retained workspaces and their 10 tasks are available, with no script-generation/execution form accessible through another launch path. Test task switching during work, close/cancel behaviour, keyboard navigation, scaling and assistive technology on the intended Windows machines. Switch between all four themes with several tasks loaded, check hidden pages when revisited, and verify inputs/results and running operations are unchanged. Include the native Windows controls described in the project guide.

This work does not reinstate the reverted authentication/SQL-transaction redesign, prove every business rule, rotate old credentials or resolve licensing/governance decisions. Existing issues requiring owner decisions or live database acceptance must not be closed solely because these synthetic tests pass.
