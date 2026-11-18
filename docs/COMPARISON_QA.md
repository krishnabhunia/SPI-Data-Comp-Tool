# Comparison and logging regression checklist

> Use **disposable** SQL Server and Oracle databases and synthetic spreadsheets. A passing build or window-opening check does not establish comparison correctness or make data-modifying operations safe for production.

## Automated checks

These checks are configured in the repository. Inspect the successful [Actions run](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/actions) for the exact commit under review; this checklist is not a claim that every run has passed.

| Check | What it covers | What it does not establish |
| --- | --- | --- |
| `python -m unittest discover -s tests -p "test_*.py" -v` | Python tests and selected source contracts for connection handling, profile safety, redaction, layout, packaging, versioning and workflows | Live database behaviour, comprehensive comparison correctness or performance limits |
| **Windows data-integrity source checks** (`.github/workflows/data-integrity-checks.yml`) | Runs the Python regression suite on pull requests | Windows executable interaction or database integration |
| **Build and checks** (`.github/workflows/build.yml`) | Builds the Windows application, validates generated layouts and runs regression checks | Proof that every business workflow is correct |
| `--check-ui <log file>` | Executes synthetic Windows UI/profile/import checks: navigation, Settings/Rules, profile candidates and temporary-file writes, and a small Analysis fixture | Full Oracle/SQL Server integration, production files, or an exhaustive comparison dataset |
| `--list-forms` / `--open-form` | Creates, shows and closes every listed window; a failed opening fails the check | Completing the workflow represented by each window |
| Portable/installer checks | Loads embedded dependencies and smoke-tests the x64 installer and uninstaller | Upgrade/rollback compatibility across all previously released versions or Windows installations |

The UI checks use synthetic data and temporary test files, not real connection profiles or live databases. No screenshots are captured or published. See [PROJECT_GUIDE.md](PROJECT_GUIDE.md) for build/release details and the [release page](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/releases) for published packages.

## Rollback-sensitive behaviour

[PR #20](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/pull/20) restored the exact state before PRs #17 and #18 at the owner's request. The application does **not** currently have the A/B application sign-in screen introduced in PR #17. Database connection authentication is a separate concern and must still be tested independently for source and target.

Do **not** assume that the SQL preview, typed execution confirmation, transaction/rollback or cancellation safeguards from PR #18 are present. Any proposal to reintroduce those safeguards or redesign application authentication requires explicit owner approval and a separate reviewed change. This checklist does not restore or substitute for those controls.

## Manual Windows acceptance matrix

The following are **acceptance expectations to verify**, not a record of completed testing. Record the application commit/version, fixture definitions, database/driver versions and actual outputs for each run.

| Scenario | Expected result |
| --- | --- |
| Source and target use different Windows-authentication settings | Each connection uses its own selected authentication method |
| Oracle source A and Oracle target B contain deliberately different rows | The optimized tracker returns differences, not A compared with A |
| Oracle target is offline but source is online | Target load fails; it must not silently reuse the source connection |
| SQL Server source and target tables have equal rows | No false mismatch |
| Source has an extra row; target has another extra row | Both source-only and target-only counts are correct |
| Key values are duplicated in one or both tables | No accidental row multiplication is reported as a match |
| Null vs empty string, case changes, numeric/decimal precision, dates | Behavior matches agreed comparison rules |
| Connection failure or an exception while a password is present | Neither UI messages nor disk logs reveal connection-string password assignments |
| Save a database profile while debugger is attached | Saved JSON has blank passwords for both source and target |
| Execute SQL scripts from an Excel sheet or use a merging/data-modifying workflow | Use only disposable databases and synthetic records with restricted test accounts; review generated commands outside the app before execution and capture before/after data. Do not assume built-in preview, transaction rollback or cancellation, and **do not test on production** |

## Known follow-ups

- Deterministic end-to-end Oracle/SQL Server/Excel comparison fixtures, measured performance/data-size limits and broader upgrade/rollback testing remain outstanding. See [issue #7](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/7); do not close it solely because builds, releases or UI smoke checks pass.
- SQL-execution safety remains a separate decision and validation track. See [issue #9](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/9) and the rollback constraints above. Never exercise destructive acceptance scenarios against production.
- Credentials previously committed to Git history require owner-controlled rotation if still active. Removing current literals, masking logs or passing a secrets check does not rotate a credential or erase historical copies.
