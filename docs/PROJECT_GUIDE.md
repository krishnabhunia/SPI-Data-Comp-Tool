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
| `SPI Data Comp Tool/Common_Data.cs` | Shared helpers: DB connections, Excel I/O, profiles, developer mode (`GlobalDebug`) |
| `SPI Data Comp Tool/UserProfile/userProfile.example.json` | Template for a local connection profile |
| `SPI Data Comp Tool.sln` | Visual Studio solution |
| `.github/workflows/build.yml` | CI: docs check, secrets guard, Windows build, portable zip artifact |

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

## Configuration
| SrNo. | Setting | How it works |
|---|---|---|
| 1 | Connection profiles | Saved from **File > User Profile** to a JSON file you choose; its path is remembered in the user settings (`ProfilePathUser`). Template: `UserProfile/userProfile.example.json`. The real `userProfile.json` is git-ignored. |
| 2 | Developer mode | On when a debugger is attached, or when the environment variable `SPI_DATA_COMP_DEBUG=1` is set. It is no longer tied to specific Windows user names or a built-in password. In developer mode the profile form keeps passwords in the saved JSON, so keep such files local. |
| 3 | Default output file (pre-merging tool) | `Documents\Output.xlsx` until you pick another location. |
| 4 | Logs | `Desktop\LogFolder\Log_dd_MM_yyyy*.log` |

## Versioning and CI artifacts
- The version is `x.y.z` from `AssemblyFileVersion` in `Properties/AssemblyInfo.cs` (x = major, y = minor, z = fix).
- Pull-request builds are tagged `x.y.z-betaNNN` (NNN = CI run number); pushes to `master` build the plain `x.y.z`.
- Each Windows build uploads one artifact, `SPI_Data_Comp_Tool_<version>`, which downloads as a zip containing `portable/` with `SPI_Data_Comp_Tool_<version>.exe`, its `.exe.config`, the required DLLs and the `PNID_Reports/Source` and `PNID_Reports/Target_IPEP` SQL templates (read at runtime by **Execute Excel Scripts > PNID Reports**; the project copies them to the build output).

## Supported workflows vs. assumptions
The source has forms for database comparison, Excel comparison, delta change tracking and database-to-Excel export. Exact connection strings, supported server versions, data-size limits and complete workflows must be validated manually: the repository does not supply safe integration fixtures for all operations.

## Verification
The GitHub Actions workflow (`build.yml`) runs on pull requests, pushes to `master` and on demand. It checks this guide, fails if a real connection profile, a non-empty profile password, a password text box pre-filled with a literal, a signing key or the old debug password is committed, builds the solution on a Windows runner (an unresolved assembly reference fails the build) and uploads the portable zip. **Read CI results** rather than assuming success. Desktop GUI interactions, live database connectivity and correctness of comparison results require manual acceptance tests.

## Limitations and security
- Some screens may update data; test in isolated environments, never on production by default.
- Avoid embedding real database credentials in commits, documentation or CI logs. Earlier commits of this repository contained a real profile file; removing it from the latest commit does not remove it from git history, so treat those credentials as exposed and rotate them.
- Queries are built by string concatenation from table/column names taken from rule files and the UI. Only use trusted rule files.
- The Excel library's license terms and database drivers need separate validation.
- No genuine geographic data or measured benchmarks were found, so geographical maps and performance charts are not fabricated.
