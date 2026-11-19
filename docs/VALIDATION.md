# Form and workflow validation

This inventory covers the 21 retained concrete Form implementations, the added `WorkspaceForm` host and their shared controls. `WorkspacePageForm` is an abstract lifecycle base, not another independently opened screen. A screen with no editable input is audited for navigation/lifecycle behaviour rather than given artificial required fields. Existing updater safeguards remain in use; the workspace consolidation does not add a second updater, authentication system or SQL transaction framework. See [WORKSPACES.md](WORKSPACES.md) for the six groups and 19 explicit task routes.

## Common behaviour

- Validate on the action itself. Enabled buttons and a previously successful connection test are not proof that inputs remain valid.
- Show a non-blinking field error, reveal its tab and focus the field where possible. Correcting text clears stale feedback; the next action still validates again. Keyboard navigation is not trapped by modal validation dialogs.
- Validate active fields only. SQL Server Windows authentication ignores inactive credentials and Oracle port; source-only and Excel-source modes do not require an unused database connection.
- Build connection strings with provider builders. Password whitespace, quotes and semicolons remain literal; validation/error messages do not echo credentials.
- Validate imported candidates before replacing current data. File cancellation or invalid input must not silently replace a valid active workbook/profile/rule set.
- Prevent overlapping background operations. Lock editable inputs while the operation runs, retain relevant Cancel controls, restore controls before setting final completion state, and block closing a running worker.
- Keep each hosted task's inputs/results independent. Switching tasks must not reset a page, bypass its close guards or hide a running worker from installation eligibility checks. Explicit parallel sessions must not replace the default session.
- Preserve Ocean, Berry, Forest and Midnight when grouping pages into workspaces. A live theme change updates presentation, not a task's inputs, results or running operation; already-loaded hidden pages and later-created pages must use the selected palette.
- Treat cancelled, failed and partially failed runs separately from successful completion.
- Validate Excel dimensions, sheet names and destinations before exporting. Shared report writers publish a complete same-directory temporary file with atomic replacement; failed writes leave the existing destination intact.

## Form coverage

| Form class | Validation or retained safeguards |
|---|---|
| `MainForm` | Six themed workspace home tiles, catalog-driven task navigation, redacted errors and all-workspace exit preflight. No data-entry fields. |
| `WorkspaceForm` | Lazy task creation, retained page state, selected-task keyboard behaviour, failed-factory preservation, close preparation and transient mapping cleanup. A blocked close selects the refusing task without disposing the other pages. |
| `AboutBox1` | Read-only information; existing open/close smoke coverage. No artificial validation fields. |
| `UserProfileForm` | Required unique profile names, applicable connection fields, strict bounded JSON, candidate-before-publication imports and password-free exports. |
| `User_Settings` | Rules workbook/path/schema, required row values, unique identifiers and supported rule types; failed imports retain current rules. Four retained colour choices on General, live selection/picker synchronization and explicit failure-to-save feedback. Existing update channel/version/download/install validation retained. |
| `DB_Comparison_Form` | Source/Target mode, connection freshness, loaded table and comparison-column selections, busy state and export bounds. Correct provider connection is used on each side. |
| `Compare_Multiples_List_Tables_Forms` | Current connections, batch workbook schema/rows, selection validity, worker lifecycle and partial-failure reporting. |
| `SPIDeltaChangeTrackerOptimised` | Separate source/target connection validation, table-list schema, operation prerequisites, stale state, worker lifecycle and partial-failure reporting. |
| `PreMergingTool_Form` | Current connections, configuration/workbook requirements, required rows and columns, output path and Excel bounds. |
| `Pre_Merging_Duplicacy_Form` | Distinct check/configuration mode requirements, current connections, configuration schema, safe output and failed/partial completion handling. |
| `Table_Extraction_After_Delta_Form` | All supported list modes, required schemas, active source/target connections, Excel-source mode without an unused source database, and worker lifecycle. |
| `SPI_FK_Extraction` | Current source connection, mapping/list schema and required extraction selections. |
| `SPIDeltaChangeLegacyID` | Active SQL Server source only, current connection test, required mapping sheets/fields, duplicate output-column prevention; the same source field may serve two mapping roles. |
| `SQL_DB_To_Excel` | Current SQL Server connection, selected table metadata, output folder, per-sheet limits, safe names and atomic output; worker inputs are captured before background work. |
| `Excel_Excel_Comparison` | Current workbook data, valid matching columns and key selections, null/duplicate composite keys in primary-key mode, non-primary-key mode compatibility, stale results and busy states. |
| `ViewForMultiReference` | Required selected reference columns, current data and compatible mapping selections; count labels describe its actual copied Source/Target tables. Each hosted mapping session keeps an independent snapshot. |
| `Generate_IUD_Script_Form` | Workbook/sheet/column choices, SQL identifiers, INSERT/UPDATE/DELETE-specific requirements, mandatory WHERE fields for UPDATE/DELETE and SET fields for UPDATE. |
| `Execute_Excel_Scripts_Form` | Applicable connection, readable workbook, exactly one selected SQL column, nonblank scripts and operation prerequisites. |
| `CustomisedExcel` | Readable workbook, first-worksheet data and selected columns, candidate validation and valid export data. |
| `MergeLogsExport` | Selected readable bounded log file, complete valid records, safe output path and candidate-before-publication handling. |
| `SPI_Spec_Note_Creation_Utility` | Required inputs, supported source table/column structure and export prerequisites. |
| `SPI_Analysis` | Current loaded source/target snapshots before showing results; prior results survive an invalid replacement. Its results control validates required fields, integral form identifiers, current pair selections, repeated arrangement and aligned export rows. |

Shared coverage: `ConnectionSide`, `ConnectionPanel`, `OracleAndSQLConnection`, `ExcelFileReadCustomControl`, `SPI_Analysis_View`, `FormHelpers`, `ReadExcel`, `WriteExcel` and `FormValidation`. `UpdatesControl` keeps the already-tested release selection, checksum, cancellation and installation gates from the updater implementation.

`WorkspaceNavigator` and `WorkspacePageForm` provide shared navigation/lifecycle behaviour. Settings preferences and profiles are separate task pages in one workspace. The existing modal Import Rules path intentionally remains a short-lived `User_Settings(true)` dialog so its caller receives imported rules synchronously; cancelling or failing an import still preserves the caller's rules.

## Compatibility and explicit boundaries

| Area | Rule / boundary |
|---|---|
| Oracle port | ASCII whole number, 1–65,535. SQL Server does not validate an inactive Oracle port. |
| Passwords | Preserve exact entered characters, including leading/trailing spaces. No built-in credential policy is imposed beyond applicable required input. |
| Excel imports | Open XML `.xlsx` / `.xlsm`; reject empty/corrupt files, duplicate headers after trimming/ignoring case, and populated columns without headers. Unused blank spacer columns/empty sheets may be omitted. Header-only sheets can remain available; actions requiring data reject them at that action. |
| Header compatibility | The existing static reader keeps trimmed/uppercase headers; the case-preserving instance reader keeps original spelling/spaces. Both share schema validation. |
| Comparison engine | In primary-key mode require non-null unique composite keys. Non-primary-key mode remains available. Selected headers containing commas or leading/trailing spaces are rejected with guidance because the existing comparison engine tokenizes them. |
| Script executor | Exactly one SQL column. The previous engine used only its first selected column; extra selections no longer silently appear executable. |
| SQL generator | Values preserve whitespace, distinguish null from empty text and escape quotes. UPDATE/DELETE require explicit WHERE selections. Generated SQL still requires review against an isolated database. |
| Analysis identifiers | Integer strings or exactly integral numeric values within the signed 32-bit range, including database decimals such as `1.00`; reject fractions, overflow, null, NaN and infinity. |
| Gap Analysis setup | Source supports Oracle or SQL Server; Target remains SQL Server. Shared connection controls validate applicable authentication/port fields. Table/query drafts survive mode switches; only the explicit reset action restores the default query. Custom SQL is not thereby guaranteed read-only. |
| Workspace state | Task switching preserves input/result state; reopening selects the default page. Ctrl/open-separate creates an independent supported session. Preferences, profiles and Configuration disallow parallel sessions; Configuration and duplicate checking retain separate mode state. |
| Colour themes | Ocean, Berry, Forest and Midnight remain available at Settings > Preferences, rules & updates > General > Colour theme. A saved theme name is bounded to 64 bytes; missing, unreadable, oversized or unknown values fall back to Ocean. Failure to save does not undo the current-session choice. The preference file remains outside the installation folder. Native Windows rendering limitations are not removed by the workspace host. |
| Update handoff | Data workspaces and profile tasks, including hidden loaded pages, block installation. A Settings workspace containing only preferences may remain open. Closing or switching a task does not install an update. |
| Excel export | At most 16,384 columns and 1,048,575 data rows plus the header per worksheet; valid unique 1–31-character sheet names. Existing split-report workflows still apply their existing partitioning. |
| Windows paths | Reject device paths/names, invalid characters, trailing dots/spaces, missing parent folders, locked/read-only destinations and supported lexical input/output self-overwrite aliases. Validation is not a physical hard-link/reparse-point identity check. |
| Atomic output | Existing destination preserved if serialization fails. A new-file publication does not overwrite another writer that wins the race. Replacement of a previously existing file is not a compare-and-swap against unrelated concurrent edits; do not have multiple tools writing the same report. |
| Import resource limits | Profile documents: 16 MiB / JSON depth 32. Merge logs: 64 MiB. Excel still loads into memory; this is not a streaming or decompression-bomb defence. |
| Progress | Zero/negative totals do not divide by zero; long arithmetic and clamping prevent overflow/out-of-range progress values. |

## Verification

Run source contracts and layout consistency from the repository root:

```text
python -m unittest discover -s tests -p 'test_*.py' -v
python tools/layout_gen.py --check tools/layouts/*.py
git diff --check
```

Windows builds compile both Any CPU and x64 with warnings as errors. The `--check-ui <log-file>` runner is configured to include the five form-validation suites and the workspace suite:

| Suite | Main checks |
|---|---|
| `CoreValidationChecks` | Field feedback, Unicode, port boundaries, literal connection strings, file locks/permissions, atomic failure/race cases, workbook bounds, input locking, header normalization and progress. |
| `ConnectionValidationChecks` | Real connection/profile/settings/Analysis input controls; active/inactive field combinations, strict imports, stale-input invalidation and shared Gap Analysis connection behaviour. |
| `DatabaseFormValidationChecks` | Nine database forms using synthetic data and connection-status fixtures; mode-specific gates, schemas, selections and partial-failure state. |
| `ExcelFormValidationChecks` | Excel comparison, multi-reference, script generation/execution and utilities; positive/negative data, key/schema checks and a distinct-target-worksheet count/snapshot fixture without running SQL. |
| `AnalysisValidationChecks` | Actual Analysis controls, valid/invalid numeric identifiers and repeat arrangement; real synthetic workbook reads, header collisions, Unicode, leading zeros and input preservation. |
| `WorkspaceUiChecks` | All 19 routes in six groups; lazy hosting, page/session reuse, distinct modes, transient mappings, failed construction, close vetoes, main-owner closure, hidden workers and updater eligibility. |

The separate `--check-themes <log-file>` entry point runs `ThemeChecks`, configured to build eligible concrete forms in all four palettes, inspect actual label/button/field/grid colours, switch themes while windows are open, synchronize multiple Settings pickers and test saved choices using temporary files. Python contracts in `tests/test_themes.py` verify theme wiring and persistence boundaries; `tests/test_ui_contrast.py` checks palette contrast pairs. These complement the workspace lifecycle checks rather than prove complete rendering or business-output parity.

Tests use temporary synthetic data. They do not connect to live databases, execute user SQL, install a live release or write real-user preference files. The existing every-form open/close checks, updater checks, embedded dependency checks and installer smoke checks remain enabled. Check the PR's actual Actions results; this document is not evidence that a particular commit passed.

## Manual acceptance still required

Use disposable Oracle/SQL Server databases and copies of representative workbooks to test permissions, provider/server versions, real comparison correctness, SQL mutation outcomes, large files and network failures. Compare representative outputs before/after hosting changes before retiring any old page implementation. Test task switching during work, close/cancel behaviour, keyboard navigation, scaling and assistive technology on the intended Windows machines. Switch between all four themes with several tasks loaded, check hidden pages when revisited, and verify inputs/results and running operations are unchanged. Include the native Windows controls described in the project guide. Do not run destructive scenarios against production.

This work does not reinstate the reverted authentication/SQL-transaction redesign, prove every business rule, rotate old credentials or resolve licensing/governance decisions. Existing issues requiring owner decisions or live database acceptance must not be closed solely because these synthetic tests pass.
