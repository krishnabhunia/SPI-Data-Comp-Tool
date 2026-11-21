# Database comparison acceptance

## Automated disposable SQL Server checks

Run the existing Windows command against a built SPI executable:

```powershell
./tools/check_sql_integration.ps1 -AppPath '.\artifacts\SPI Data Comp Tool.exe' -LogPath '.\sql-integration.log'
```

The script refuses to reuse an existing `SPI_Comparison_Checks_CI` LocalDB instance. It creates a new instance, runs the executable's explicit integration mode and removes only that instance afterwards. The executable accepts no database endpoint, credentials or saved profiles for this mode.

Each run creates two databases and two SQL logins with unique names and generated passwords. Each SQL login has only `SELECT` permission on its own fixture table. The logins and databases are removed in `finally`; credentials are never printed or saved in profile files.

| Source authentication | Target authentication | Executed coverage |
|---|---|---|
| Windows | Windows | Actual metadata buttons, Connect/Load button, primary/reference comparisons and reopened Excel output |
| Windows | SQL password | Same coverage with a separate, read-only Target login |
| SQL password | Windows | Same coverage with a separate, read-only Source login |
| SQL password | SQL password | Same coverage with distinct users and credentials for each database |

The fixture table has spaces, an apostrophe and a closing bracket in its name. The metadata picker must quote each identifier before the real loader queries it. Pure Windows regressions also cover SQL database/schema names containing punctuation and Oracle mixed-case/space-containing metadata names.

The suite verifies actual Source/Target values, unmatched rows, duplicate multiplicity, null/empty/quoted keys and output mismatch totals. It also verifies that incorrect passwords on either side, inaccessible databases and a nonexistent database fail instead of falling back to the other connection or Windows authentication. A zero-row `UPDATE` must be denied for each read-only login.

The retained Database-to-Excel metadata/count and export workers are also exercised with Windows and SQL authentication. Their displayed table names remain separate from provider-quoted query identifiers, and the SQL object lookup uses a parameter. The tests reopen the real exported workbook and verify its headers, row count and source values.

Microsoft documents SQL logins for LocalDB in [SqlLocalDB utility](https://learn.microsoft.com/en-us/sql/tools/sqllocaldb-utility#b-work-with-a-shared-instance-of-localdb). These tests use the current process's dedicated local instance and do not require access to the user's usual `MSSQLLocalDB` instance.

## Acceptance that still needs a supplied environment

Passing LocalDB checks verifies these application paths on the tested Windows runner. It does not establish remote SQL Server networking, Kerberos delegation, organization-specific roles or Oracle connectivity.

| Remaining environment | Required evidence |
|---|---|
| Disposable Oracle schemas | Distinct Source/Target hosts or services and users; SQL/Oracle and Oracle/SQL combinations; correct custom ports; case-sensitive quoted table names; denied login/read cases; compared and exported values |
| Supported user Windows machines | Open and inspect generated workbooks in Excel; exercise actual user-selected databases without writing source data |
| Organization SQL Server authentication | Approved test users and database endpoints; expected Windows identity/group behavior and SQL password permissions |

Record the tested application commit, provider versions, sanitized scenario results and cleanup outcome. Never place passwords, production connection strings, customer rows or private profile files in an issue, test log or repository. Oracle builder/metadata tests are synthetic validation, not successful Oracle connections.
