# Comparison and logging regression checklist

> Use **disposable** SQL Server and Oracle databases and synthetic spreadsheets. Do not connect this release candidate to production until manual acceptance tests pass.

## Automated checks

`python -m unittest discover -s tests -p "test_*.py" -v` checks selected source contracts for independent source/target authentication, Oracle target selection, blank passwords in exported profiles, and log redaction. It does **not** validate runtime data correctness.

## Manual Windows acceptance matrix

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
| Execute SQL scripts from an Excel sheet | Preview and backups are required; **do not test on production** |

## Known follow-ups

- The independent CI workflow performs source-contract checks; verify Windows application build checks and downloadable artifacts on [PR #4](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/pull/4).
- Full integration tests and safe transactional/script execution remain to be designed. See issues #7 and #9.
- Existing application credentials in Git history need rotation; the built-in login also needs an authentication redesign (issue #6).
