# SQL script execution safeguards (Issue #9)

This change affects **Execute Excel Scripts** and **Generate IUD Script** in the
Windows desktop application. It applies to both the existing SQL Server and Oracle
connection modes, and does **not** connect to a database during preview.

## Executing SQL from an Excel workbook

1. Choose the workbook; select **exactly one** column containing SQL statements.
2. Test the connection **after** selecting the actual server, database, credentials
   and database type. Any subsequent change to these fields requires retesting.
3. Leave **Contains Select Query** unchecked for data modification.
4. Click **Preview / Execute**. Every script is validated before any database call.
   The preview shows the server, database, database type, user/schema, total
   statement count, and the **first 50 statements** (up to 4,000 characters each).
   Review the original workbook for anything omitted or truncated.
5. **Preview only / Cancel** is the default and makes **no changes**.
   To proceed, type the exact word **EXECUTE** and click **Execute batch**.
6. The application executes the batch with **one database connection and one
   transaction**. A failed statement or cancellation requests a rollback for all
   statements in the batch. A 60-second timeout applies to each SQL command.
   Progress is displayed and cancellation is offered while the batch runs.

Only a single INSERT INTO, UPDATE or DELETE FROM statement per cell is allowed.
UPDATE/DELETE require an explicit top-level WHERE clause. DDL, stored procedure
calls, comments and multi-statement scripts are refused.
SELECT exports are validated separately as read-only-style SELECT statements.
Restricted operations include SELECT INTO, SELECT FOR UPDATE, SQL Server
sequence advancement and Oracle NEXTVAL. **The validator is a conservative
screen, not a comprehensive SQL parser.** A valid DML statement may still affect
many rows. Triggers or callable database functions may also have side effects.

**Always connect using a least-privileged database identity**, with database-side
roles, row permissions, backup and change-management controls. Local dialog
confirmation is not a database authorization boundary.

## Generating INSERT / UPDATE / DELETE scripts

- Table and column identifiers accept only plain SQL identifier components
  optionally separated by dots (for example, dbo.Product). Unusual, quoted or
  whitespace-containing identifiers are explicitly rejected rather than interpolated.
- Text values have single quotes doubled, so O'Brien becomes N'O''Brien'.
- Null or blank values become NULL. Null conditions in WHERE become IS NULL.
- Generating UPDATE or DELETE without at least one checked condition column
  is blocked; the resulting statements also pass the execution validator.
- Review the workbook carefully; even a syntactically valid WHERE may match
  more rows than intended. The generated scripts are **SQL text**, not
  parameterized commands. Prefer parameterized database operations for new code.

## Local audit, rollback limitations and testing

On execution, the application writes a local CSV event log to:

    %LOCALAPPDATA%\SPIDataComparisonTool\Audit\sql-script-runs.csv

Each row includes UTC timestamp, run ID, phase, SHA-256 target fingerprint,
statement count and affected row count. **It does not include SQL text,
connection strings, usernames, passwords or exception details.** The log is
ordinary local storage, not a tamper-proof central audit. Failure to write
the start event blocks execution; failure to append a completion event is warned.

Rollback is attempted for exceptions and cancellation. Connection loss can prevent
the application from verifying the rollback, and some database features (for
example, triggers, external side effects, or Oracle implicit DDL commits) are not
fully reversible. DDL is therefore prohibited in this execution path.

The repository's Python tests are **source-level regression contracts only**.
Before deployment, test both database providers on disposable databases:

1. Preview a harmless INSERT and verify **no database change**.
2. Explicitly execute a two-row batch and verify **one commit** and audit entry.
3. Deliberately fail the second statement and verify **no rows from the first
   statement remain**.
4. Cancel a long-running batch and verify rollback; test closing the progress
   dialog during execution.
5. Verify a user without write permissions cannot execute a batch.
6. Try multi-statements, comments, DDL, missing WHERE, invalid filenames,
   embedded apostrophes, null values and changed database settings.

Keep Issue #9 open until those SQL Server/Oracle runtime checks have been signed off.
