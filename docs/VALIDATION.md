# Form and workflow validation

This inventory covers all 22 current Forms plus their shared controls. A screen with no editable input is audited for navigation/lifecycle behaviour rather than given artificial required fields. Existing updater safeguards remain in use; this change does not add a second updater, authentication system or SQL transaction framework.

## Common behaviour

- Validate on the action itself. Enabled buttons and a previously successful connection test are not proof that inputs remain valid.
- Show a non-blinking field error, reveal its tab and focus the field where possible. Correcting text clears stale feedback; the next action still validates again. Keyboard navigation is not trapped by modal validation dialogs.
- Validate active fields only. SQL Server Windows authentication ignores inactive credentials and Oracle port; source-only and Excel-source modes do not require an unused database connection.
- Build connection strings with provider builders. Password whitespace, quotes and semicolons remain literal; validation/error messages do not echo credentials.
- Validate imported candidates before replacing current data. File cancellation or invalid input must not silently replace a valid active workbook/profile/rule set.
- Prevent overlapping background operations. Lock editable inputs while the operation runs, retain relevant Cancel controls, restore controls before setting final completion state, and block closing a running worker.
- Treat cancelled, failed and partially failed runs separately from successful completion.
- Validate Excel dimensions, sheet names and destinations before exporting. Shared report writers publish a complete same-directory temporary file with atomic replacement; failed writes leave the existing destination intact.

## Form coverage

| Form class | Validation or retained safeguards |
|---|---|
| `MainForm` | Existing workflow reuse, closed-window handling and independent mode keys; redacted navigation errors. No data-entry fields. |
| `AboutBox1` | Read-only information; existing open/close smoke coverage. No artificial validation fields. |
| `ToolGroupWindow` | Holds existing tools as tabs (Pre-merging, Excel and file tools, Delta tools); no data-entry fields of its own. Each tool keeps its own validation; a tool that refuses to close keeps the window open. |
| `UserProfileForm` | Required unique profile names, applicable connection fields, strict bounded JSON, candidate-before-publication imports and password-free exports. |
| `User_Settings` | Rules workbook/path/schema, required row values, unique identifiers and supported rule types; failed imports retain current rules. Existing update channel/version/download/install validation retained. |
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
| `ViewForMultiReference` | Required selected reference columns, current data and compatible mapping selections. |
| `Generate_IUD_Script_Form` | Workbook/sheet/column choices, SQL identifiers, INSERT/UPDATE/DELETE-specific requirements, mandatory WHERE fields for UPDATE/DELETE and SET fields for UPDATE. |
| `Execute_Excel_Scripts_Form` | Applicable connection, readable workbook, exactly one selected SQL column, nonblank scripts and operation prerequisites. |
| `CustomisedExcel` | Readable workbook, expected rule/data structure, candidate validation and valid export data. |
| `MergeLogsExport` | Selected readable bounded log file, complete valid records, safe output path and candidate-before-publication handling. |
| `SPI_Spec_Note_Creation_Utility` | Required inputs, supported source table/column structure and export prerequisites. |
| `SPI_Analysis` | Current loaded source/target snapshots before showing results; prior results survive an invalid replacement. Its results control validates required fields, integral form identifiers, current pair selections, repeated arrangement and aligned export rows. |

Shared coverage: `ConnectionSide`, `ConnectionPanel`, `OracleAndSQLConnection`, `ExcelFileReadCustomControl`, `SPI_Analysis_View`, `FormHelpers`, `ReadExcel`, `WriteExcel` and `FormValidation`. `UpdatesControl` keeps the already-tested release selection, checksum, cancellation and installation gates from the updater implementation.

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

Windows builds compile both Any CPU and x64 with warnings as errors. The existing `--check-ui <log-file>` runner includes five added suites:

| Suite | Main checks |
|---|---|
| `CoreValidationChecks` | Field feedback, Unicode, port boundaries, literal connection strings, file locks/permissions, atomic failure/race cases, workbook bounds, input locking, header normalization and progress. |
| `ConnectionValidationChecks` | Real connection/profile/settings/Analysis input controls; active/inactive field combinations, strict imports and stale-input invalidation. |
| `DatabaseFormValidationChecks` | Nine database forms using synthetic data and connection-status fixtures; mode-specific gates, schemas, selections and partial-failure state. |
| `ExcelFormValidationChecks` | Excel comparison, multi-reference, script generation/execution and utilities; positive/negative data, key and schema checks without running SQL. |
| `AnalysisValidationChecks` | Actual Analysis controls, valid/invalid numeric identifiers and repeat arrangement; real synthetic workbook reads, header collisions, Unicode, leading zeros and input preservation. |

Tests use temporary synthetic data. They do not connect to live databases, execute user SQL, install a live release or write real-user preference files. The existing every-form open/close checks, updater checks, embedded dependency checks and installer smoke checks remain enabled. Check the PR's actual Actions results; this document is not evidence that a particular commit passed.

## Manual acceptance still required

Use disposable Oracle/SQL Server databases and copies of representative workbooks to test permissions, provider/server versions, real comparison correctness, SQL mutation outcomes, large files and network failures. Test keyboard navigation, scaling and assistive technology on the intended Windows machines. Do not run destructive scenarios against production.

This work does not reinstate the reverted authentication/SQL-transaction redesign, prove every business rule, rotate old credentials or resolve licensing/governance decisions. Existing issues requiring owner decisions or live database acceptance must not be closed solely because these synthetic tests pass.
