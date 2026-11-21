# Comparison and logging regression checklist

> Use **disposable** SQL Server and Oracle databases and synthetic spreadsheets. A passing build or window-opening check does not establish comparison correctness or complete workflow coverage.

## Automated checks

These checks are configured in the repository. Inspect the successful [Actions run](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/actions) for the exact commit under review; this checklist is not a claim that every run has passed.

| Check | What it covers | What it does not establish |
| --- | --- | --- |
| `python -m unittest discover -s tests -p "test_*.py" -v` | Python tests and selected source contracts for connection handling, profile safety, redaction, layout, packaging, versioning and workflows | Live database behaviour, comprehensive comparison correctness or performance limits |
| **Windows data-integrity source checks** (`.github/workflows/data-integrity-checks.yml`) | Runs the Python regression suite on pull requests | Windows executable interaction or database integration |
| **Build and checks** (`.github/workflows/build.yml`) | Builds the Windows application, validates generated layouts and runs regression checks | Proof that every business workflow is correct |
| `--check-ui <log file>` | Executes synthetic Windows UI/profile/import checks: navigation, Settings/Rules, profile candidates, temporary-file writes, and comparison/workbook validation fixtures | Full Oracle/SQL Server integration, production files, or an exhaustive comparison dataset |
| Comparison correctness suite inside `--check-ui` (`tests/ComparisonCorrectnessChecks.cs`) | Runs the actual Database, List and Excel comparison/report paths on synthetic snapshots; reopens the output workbooks; checks repeated keys, typed/composite keys, source-only/target-only rows, conditional target rules and reordered columns | Live provider reads, Microsoft Excel rendering, arbitrary customer files or universal size limits |
| `--check-sql-integration <log file>` (`tests/DatabaseIntegrationChecks.cs`) | Creates two uniquely named disposable databases on the dedicated `SPI_Comparison_Checks_CI` LocalDB instance; runs the actual SQL Server Source/Target form loaders, primary/reference comparisons and workbook exports; checks a real target-connection failure; removes its databases/files in `finally` | Oracle integration, SQL authentication/production permissions, external-server network failures or interactive Excel fidelity |
| `--check-startup <log file>` | 40 real workspace-navigation scenarios: all 10 routes with no remembered profile and a valid legacy profile, plus five unavailable/invalid-file cases for the four profile-consuming routes | Live database connectivity, real profile files or complete task output correctness |
| Workbook identity suite inside `--check-ui` (`tests/WorkbookIdentityChecks.cs`) | Engine and report on synthetic workbooks: value, type, formula, format and layout differences; primary keys; skipped checks; a report that opens without an Excel repair prompt | Real customer workbooks, very large files, or opening the report in Excel itself |
| `--list-forms` / `--open-form` | Creates, shows and closes every listed window; a failed opening fails the check | Completing the workflow represented by each window |
| Portable/installer checks | Loads embedded dependencies and smoke-tests the x64 installer and uninstaller | Upgrade/rollback compatibility across all previously released versions or Windows installations |

The UI checks use synthetic data and temporary test files, not real connection profiles or live databases. The **separate, explicit** SQL integration command connects only to `(localdb)\SPI_Comparison_Checks_CI`. The CI wrapper must create that dedicated instance and remove it after the test; the command accepts no server/connection-string input and never uses the user's normal `MSSQLLocalDB` instance. Missing LocalDB or a failed database/report/cleanup assertion fails the check instead of silently skipping it. No screenshots are captured or published. See [PROJECT_GUIDE.md](PROJECT_GUIDE.md) for build/release details and the [release page](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/releases) for published packages.

## Reference-key matching and comparison rules

- In the existing **non-primary-key mode**, repeated keys pair one source row with one available target row in original target order. Each target row can be used once. Extra duplicates remain in the appropriate Source/Target Rows Only worksheet. The engine does not perform a Cartesian join or silently discard unpaired duplicates.
- This is occurrence-order pairing, not an optimal assignment between duplicate groups. Reordering different values under the same repeated key can produce differences. Use a genuinely unique composite key when rows must match independently of their order.
- Null reference keys match null reference keys and remain distinct from empty-string keys. Key predicates escape column names and text; dates/decimals use culture-independent literals and large integer/decimal keys retain precision. Explicit primary-key mode continues to require unique, non-null keys.
- Cell-value comparison retains the legacy rules: values are converted to text and trimmed, the Ignore case option applies, and configured rules may suppress a difference. In particular, null and empty **value cells** both normalize to empty text; the distinct null/empty behavior above concerns reference keys. This change does not redefine existing value-normalization options.
- Conditional rules evaluate the actual matched target row. Report rows are mapped by column name before export, so different source/target column orders do not put target values under the wrong headers.
- Repeated-key candidates are cached in original-order queues. A deterministic 2,000-row repeated-key test checks ordering and exhaustion. This is regression coverage, not a claimed upper data-size or performance limit.

## Rollback-sensitive behaviour

[PR #20](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/pull/20) restored the exact state before PRs #17 and #18 at the owner's request. The application does **not** currently have the A/B application sign-in screen introduced in PR #17. Database connection authentication is a separate concern and must still be tested independently for source and target.

The SQL Scripts workspace and its generation/execution forms are now removed. The historical SQL preview, typed execution confirmation and transaction/rollback design from PR #18 was not restored. Any future proposal to reintroduce script execution or redesign application authentication requires a separate reviewed change; it is outside the current comparison/export acceptance scope.

## Manual Windows acceptance matrix

The following are **acceptance expectations to verify**, not a record of completed testing. Record the application commit/version, fixture definitions, database/driver versions and actual outputs for each run.

| Scenario | Expected result |
| --- | --- |
| Source and target use different Windows-authentication settings | Each connection uses its own selected authentication method |
| Oracle source A and Oracle target B contain deliberately different rows | Database comparison uses the distinct source and target connections and reports the expected differences |
| Oracle target is offline but source is online | Target load fails; it must not silently reuse the source connection |
| SQL Server source and target tables have equal rows | No false mismatch |
| Source has an extra row; target has another extra row | Both source-only and target-only counts are correct |
| Key values are duplicated in one or both tables | No accidental row multiplication is reported as a match |
| Null vs empty string, case changes, numeric/decimal precision, dates | Behavior matches agreed comparison rules |
| Connection failure or an exception while a password is present | Neither UI messages nor disk logs reveal connection-string password assignments |
| Save a database profile while debugger is attached | Saved JSON has blank passwords for both source and target |
| Current task inventory | Three workspace tiles and 10 task routes are available; retired script-generation/execution forms cannot be opened through menus, workspace routes or a self-test launch |

## Known follow-ups

- The new suites configure real SQL Server LocalDB-to-workbook acceptance and synthetic Database/List/Excel report correctness checks; use the successful Actions run for the exact commit as execution evidence. Disposable Oracle integration, representative performance/data-size limits, interactive Microsoft Excel fidelity and broader upgrade/rollback testing remain outstanding. See [issue #7](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/7); do not close it solely because builds, releases or these bounded checks pass.
- [Issue #9](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/9) concerns the retired SQL-execution workflow. Removing that workflow does not implement or verify its historical safety proposals; any future reintroduction needs its own acceptance plan.
- Credentials previously committed to Git history require owner-controlled rotation if still active. Removing current literals, masking logs or passing a secrets check does not rotate a credential or erase historical copies.
