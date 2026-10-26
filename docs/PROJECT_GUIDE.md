# SPI Data Comparison Tool — implementation guide

## Overview
This is a **Windows Forms / C# desktop project** for database and Excel comparisons, SPI delta tracking, merging workflows, exports and related data-analysis utilities. Different forms implement different workflows; their existence is not proof every process has been end-to-end tested.

## Verified source map
| Source | Responsibility |
|---|---|
| `SPI Data Comp Tool/Program.cs` | Startup; chooses main or debug-specific forms |
| `SPI Data Comp Tool/DB Comparison.cs` | Database-comparison UI |
| `SPI Data Comp Tool/Excel_Excel_Comparison.cs` | Excel comparison |
| `SPI Data Comp Tool/SPIDeltaChangeTracker.cs` | Delta tracking form |
| `SPI Data Comp Tool/SQL_DB_To_Excel.cs` | Export UI |
| `SPI Data Comp Tool.sln` | Visual Studio solution |

## Architecture
~~~mermaid
flowchart LR
 User[Desktop user] --> Forms[WinForms screens]
 Forms --> Compare[Database / spreadsheet comparison logic]
 Compare --> DB[(SQL Server or Oracle)]
 Compare --> Excel[Excel inputs and exports]
~~~

## Typical processing
~~~mermaid
sequenceDiagram
 participant U as Desktop user
 participant F as Comparison form
 participant S as External source
 U->>F: Select comparison / export
 F->>S: Request data from configured inputs
 S-->>F: Data / errors
 F-->>U: UI report / export
~~~

## Setup flow
~~~mermaid
flowchart TD
 Clone[Clone repository] --> VS[Install Visual Studio and .NET desktop workload]
 VS --> Packages[Restore NuGet packages and drivers]
 Packages --> Configure[Prepare safe local Excel / DB fixtures]
 Configure --> Build[Build .sln]
 Build --> Verify[Manually verify desktop forms and output]
~~~

## Local installation
1. On Windows, install Visual Studio with **.NET desktop development** and the .NET Framework targeting pack specified in the project file.
2. Open `SPI Data Comp Tool.sln` and restore NuGet dependencies.
3. Check third-party database drivers and the EPPlus licensing context: `Program.cs` sets a **Commercial** license context, which is not a substitute for a valid license.
4. Build the application with Visual Studio, and start the main form.
5. Use disposable database fixtures and test Excel files for comparison. Back up your data before running any data-modifying or merge action.

## Supported workflows vs. assumptions
The source has forms for database comparison, Excel comparison, delta change tracking and database-to-Excel export. Exact connection strings, supported server versions, data-size limits and complete workflows must be validated manually: the repository does not supply safe integration fixtures for all operations.

## Verification
The pull request's GitHub Actions workflow performs Markdown checks and attempts to build the solution on a Windows runner. **Read CI results** rather than assuming success. Desktop GUI interactions, live database connectivity and correctness of comparison results require manual acceptance tests.

## Limitations and security
- Some screens may update data; test in isolated environments, never on production by default.
- Avoid embedding real database credentials in commits, documentation or CI logs.
- The Excel library's license terms and database drivers need separate validation.
- No genuine geographic data or measured benchmarks were found, so geographical maps and performance charts are not fabricated.
