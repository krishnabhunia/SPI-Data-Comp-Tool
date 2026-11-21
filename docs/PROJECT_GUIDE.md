# SPI Data Comparison Tool — implementation guide

## Overview
This is a **Windows Forms / C# desktop project** for database and Excel comparisons, SPI delta tracking, merging workflows, exports and related data-analysis utilities. Six task-based workspaces now host the existing workflow implementations. This first-phase interface consolidation preserves the business engines; their existence is not proof every process has been end-to-end tested. See [WORKSPACES.md](WORKSPACES.md) for the complete task map and lifecycle boundaries.

## Verified source map
| Source | Responsibility |
|---|---|
| `SPI Data Comp Tool/Program.cs` | Dispatches the isolated update helper, CI checks, or normal application startup |
| `SPI Data Comp Tool/MainForm.cs` | Main window, six workspace entry points, catalog-driven menus, About and guarded exit |
| `SPI Data Comp Tool/WorkspaceCatalog.cs` | Six workspace groups and 21 explicit task routes; advanced mapping opens an independent transient page |
| `SPI Data Comp Tool/WindowNavigator.cs` | `WorkspaceNavigator`: default workspace/page reuse and explicit parallel sessions |
| `SPI Data Comp Tool/WorkspaceForm.cs`, `WorkspacePageForm.cs` | Lazy page hosting, task switching and prepare-before-close lifecycle; existing business code remains in its page implementation |
| `SPI Data Comp Tool/UiTheme.cs`, `ThemeControls.cs` | Four retained colour palettes, live styling and preference persistence; owner-drawn home tiles, glyphs and theme swatch |
| `SPI Data Comp Tool/DB Comparison.cs` | Database-comparison UI |
| `SPI Data Comp Tool/Excel_Excel_Comparison.cs` | Excel comparison |
| `SPI Data Comp Tool/ExcelSheetAuditForm.cs`, `ExcelAudit*.cs` | Separate native Excel Sheet Audit: Design A, streamed reader, typed row/key comparison, bounded details and two-sheet Open XML report; see [EXCEL_SHEET_AUDIT.md](EXCEL_SHEET_AUDIT.md) |
| `SPI Data Comp Tool/SPIDeltaChangeTrackerOptimised.cs` | Delta tracking form (the only one: the first tracker window, whose menu entry was always hidden, was removed) |
| `SPI Data Comp Tool/SQL_DB_To_Excel.cs` | Export UI |
| `SPI Data Comp Tool/Common_Data.cs` | Shared helpers: DB connections, Excel I/O, profiles, developer mode (`GlobalDebug`) |
| `SPI Data Comp Tool/UserProfile/userProfile.example.json` | Template for a local connection profile |
| `SPI Data Comp Tool.sln` | Visual Studio solution |
| `.github/workflows/build.yml` | CI: docs check, secrets and branding guard, Windows build, portable exe + 64-bit installer in one zip artifact |
| `windows/installer/SPI_Data_Comp_Tool.iss` | Inno Setup script for the 64-bit installer |
| `tools/make_app_images.py` | Draws the app icon and pictures (original artwork) |
| `SPI Data Comp Tool/UpdatesControl.cs` | The Updates tab inside the Settings preferences task; Help activates that same page |
| `SPI Data Comp Tool/UpdateFeedClient.cs`, `UpdateVersion.cs` | Published release discovery, three-part version ordering and verified downloads |
| `SPI Data Comp Tool/UpdateInstaller.cs`, `UpdateApplyWorker.cs`, `UpdatePreferences.cs` | Installed/portable update handoff, bounded file replacement and whitelisted preference preservation |
| `SPI Data Comp Tool/WorkbookIdentityReader.cs`, `WorkbookIdentityEngine.cs`, `IdentityReportWriter.cs` | Workbook identity check engine: reads two workbooks, lists every difference and writes the five-sheet report. No window code |
| `SPI Data Comp Tool/WorkbookIdentityForm.cs` | Workbook identity check task in the Comparison workspace: collects the inputs, runs the engine in the background and shows the verdict and the five report tables; see [WORKBOOK_IDENTITY_CHECK.md](WORKBOOK_IDENTITY_CHECK.md) |

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
| 1 | Connection profiles | Saved from **Settings > Connection profiles** to a JSON file you choose; its path is remembered in the user settings (`ProfilePathUser`). Template: `UserProfile/userProfile.example.json`. The real `userProfile.json` is git-ignored. |
| 2 | Login | None. The old login screen (a user name and password written in this source) was removed in 2.2.0; the main window opens unlocked. |
| 3 | Developer mode | On when a debugger is attached, or when the environment variable `SPI_DATA_COMP_DEBUG=1` is set. It is no longer tied to specific Windows user names or a built-in password. The profile editor omits passwords from new JSON exports in every mode; enter them again when needed. |
| 4 | Default output file (pre-merging tool) | `Documents\Output.xlsx` until you pick another location. |
| 5 | Logs | `Desktop\LogFolder\Log_dd_MM_yyyy*.log` |

## Branding and images
- The tool is generic: no company names, logos or trademarks.
- Every image is original and drawn by `tools/make_app_images.py` (Python + Pillow): `app.ico` (exe and all window icons, 16-256 px) and `Resources/about_side.png` (About box). The main window has no picture: its header band and six workspace tiles are drawn in the chosen colour theme.
- To change the look, edit the script, run `python tools/make_app_images.py` from the repository root (it also refreshes the embedded window icon of every form), and rebuild.
- CI fails if any other image file is committed or if former-employer names reappear in source, settings or docs.

## Window layout and look
- Generated workflow layouts live in `tools/layouts/<Form>.py`; `python tools/layout_gen.py tools/layouts/<Form>.py` rewrites the corresponding `*.Designer.cs`. Controls sit in `TableLayoutPanel` / `FlowLayoutPanel` cells, retaining their names, events and settings. CI fails if a generated designer no longer matches its description (`--check`). Not every retained workflow has a generated layout. `WorkspaceForm` builds the common task navigation/content/footer layout directly in C#.
- Look: `UiTheme.Apply(this, mainButton)` at the end of a constructor gives windows the same style - Segoe UI 9, group boxes as cards, flat buttons, one main button in the theme's accent colour, and the app icon. `UiTheme.SetIcon(this)` only sets the icon. `WorkspaceForm` calls `UiTheme.EnsureApplied` for hosted pages that have not applied the theme themselves.
- Colour themes: the existing **Ocean** (default), **Berry**, **Forest** and **Midnight** (dark) palettes in `UiTheme.cs` are preserved, not replaced by the workspace consolidation. Choose **Settings > Preferences, rules & updates > General > Colour theme**. The selection recolours the main window, workspace hosts and their loaded pages immediately; newly created pages use the current choice. Themes colour windows, card groups, buttons, fields, grids, menus, tabs, progress bars, header bands, home tiles and status areas. Source and Target retain their distinct caption-band and matching grid-column tints. Changing colour does not recreate tasks or reset their inputs/results.
- Progress bars retain the theme's accent colour on a contrasting track. On Windows, `UiTheme.PlainBarLater` defers the native drawing change until after handle creation and leaves the bar's style unchanged, avoiding handle recreation during initialization. If the native styling call is unavailable or fails, Windows keeps its native progress drawing.
- The theme name is saved in `%AppData%\SPI Data Comparison Tool\theme.txt`, outside the installation folder, so an update keeps it. Missing, unreadable, unknown or oversized choices fall back to Ocean. If saving fails, the chosen theme still applies for the current session and Settings reports that it could not be remembered. All open Settings pickers, including the short-lived modal rules dialog, follow the current theme.
- Status colours: labels may still request `Color.Red`, `Color.Black`, `Color.DarkGreen` or `Color.DarkBlue`; `UiTheme.Readable` maps their meaning to a readable colour in the active palette. Log text uses the same mapping, and cells marked yellow keep black text in every theme.
- Readability checks are configured at two levels: `tests/test_ui_contrast.py` checks palette text/background pairs against WCAG AA (4.5:1; lines and glyphs 3:1), while `--check-themes` (`tests/ThemeChecks.cs`) builds eligible windows in all four palettes and checks actual control colours and live switching. Disabled buttons are drawn by the theme because Windows would otherwise draw their text light grey. These checks are not a claim that this commit's Windows run has passed.
- What Windows draws itself retains Windows' colours, also in Midnight: the strip behind tab headers, an opened drop-down list of a non-flat combo box, the date picker, list-view column headers and message boxes. Dark title bars and scroll bars are requested where supported on Windows 10 1809 and later; older systems retain native rendering.
- Main window: a themed header band, **six workspace home tiles** and a status line with the running version. Each tile opens its group's first task; menus expose all 21 routes. `TileButton`, `GlyphIcon` and `ThemeSwatch` in `ThemeControls.cs` are retained owner-drawn controls. A tile remains a `Button`, preserving normal keyboard and accessibility semantics. Workspace selection, header and status colours also follow live theme changes. Workflow pages retain their controls and scrolling where necessary; a common host is not a claim that every older page has been visually redesigned. To add a future theme, define a palette in `UiTheme.cs`, include it in `All`, and extend `THEMES` in the contrast test and the Windows theme checks.
- Shared code: `FormHelpers` holds the helpers several windows used to copy (status text and errors, elapsed time, summary transpose, Excel table formatting, table pickers). A window keeps a short wrapper with its own error handling and calls the helper; add new shared logic there instead of copying it.
- Connection panel: `ConnectionPanel` (a UserControl) holds the profile picker, the Source and Target halves (`ConnectionSide`) and the Copy buttons, with their Oracle / SQL Server, Windows-login and default-port behaviour. A generated page places it with `new="SPI_Data_Comp_Tool.ConnectionPanel"`, sets `Mode` (`SourceAndTarget` or `SourceOnly`), handles `ProfileApplied` for its own extras, and retains legacy control-name properties where needed. Twelve connection surfaces use it, including the Optimised Delta Tracker and the embedded Gap Analysis setup control. Gap Analysis still supports Oracle/SQL Server Source and SQL Server Target; its table/query drafts survive mode switches and resetting the default query is an explicit action. Single-connection pages use `Mode = SourceOnly`; `ProfileHalf` chooses the profile half, `SqlServerOnly` restricts the provider, and `ShowProfile` controls picker visibility. Shared layout parts live in `tools/layouts/_connection.py`; `KEEP = [...]` retains designed child contents, and `TABS(...)` lays out tab pages.

### Consolidated workflows

| Previous UI | Current route | Preserved behaviour |
|---|---|---|
| Separate workflow windows | Six workspaces with 21 explicit task routes | Existing engines, inputs, outputs and per-task state remain. A page is created only when selected. Reopening returns to the same page; Ctrl/open-separate creates an independent supported session. |
| Three grouped tool windows | Existing routes in Pre-merge Checks, Analysis & Migration, SQL Scripts and Data & Reports | All nine grouped workflows remain available. The shared workspace host replaces `ToolGroupWindow`, retaining lazy page creation, state reuse, separate Configuration mode and all-or-none close preparation. Readable window titles remain. |
| Database/Excel advanced-mapping window | Transient Advanced mapping page in the current Comparison workspace | Every invocation keeps a fresh copied input snapshot; mappings from different loads are not silently reused. Standalone callers remain supported. |
| Profile editor and application settings windows | Settings workspace: Connection profiles and Preferences, rules & updates tasks | Preferences retains General / Comparison rules / Updates tabs. Help's update shortcut activates the same preferences page. Profiles and preferences do not share mutable form state. |
| Comparison Import Rules dialog | Short-lived modal `User_Settings(true)`, opened on Comparison rules | Synchronous callers receive the imported dataset only after the dialog closes; invalid/cancelled imports retain existing rules. This deliberate modal path is separate from the persistent Settings workspace. |
| Analysis setup and a separate results window | Analysis & Migration > Specification gaps: Connection setup / Results tabs | The earlier setup/results consolidation remains: independent result snapshots, filters, Arrange, remarks, synchronized grids and Excel output are preserved. |
| Hidden profile prototypes and inactive Excel multithreading scaffold | Removed | Real profile management and active comparison/instrument workers remain. The advanced-mapping count display now describes its actual loaded Target snapshot. |

The **21 existing concrete Form implementations remain** for behaviour parity. The added `WorkspaceForm` host, the separate `ExcelSheetAuditForm` and the `WorkbookIdentityForm` make **24 concrete production Form classes**; `WorkspacePageForm` is an abstract lifecycle base, not another window to open. The catalog's 21 routes cover 20 unique directly launched implementation types plus an additional Configuration route to the Duplicacy type. Advanced mapping, Main and About complete the retained inventory. This is fewer independent workflow windows in normal navigation, **not** deleted business engines. Excel Sheet Audit is additive and does not replace the original Excel comparison. Retiring old page implementations is a later change requiring output-parity fixtures and Windows acceptance.

Closing a task retains its own close guards. Closing a workspace or the main window checks loaded pages before discarding them; a refusing task is brought into view. Settings tasks and Configuration do not offer parallel sessions. Script generation and execution remain separate explicit tasks; the workspace does not turn generated SQL into an automatic execution action.

Profile edits build independent candidate lists. Cancelling a save does not change active profiles; JSON is written to a same-directory temporary file and atomically replaced before publishing a changed list. Deletion requires confirmation. If the file is saved but its path cannot be remembered, the UI explicitly reports the partial outcome and retains the active catalog. Shared pickers resolve names rather than stale list positions, and typing a new name preserves entered fields.

## Updates in the app

**Settings > Preferences, rules & updates > Updates** and **Help > Check for updates** lead to the same designer-defined Updates page. Starting the app or opening Settings performs no update network request. The main status line shows the running version and its actual Normal/Beta build type; the release filter is a separate choice. Installation requires closing data workspaces and the Connection profiles task, including hidden loaded pages; the preferences-only Settings workspace may remain open.

| Area | Current behaviour |
|---|---|
| Channels | Normal shows stable releases, Beta shows betas only, and All includes both. |
| Version selection | Choose an exact compatible published version; no automatic selection. Current/latest labels, publication dates and plain-text release notes are shown. Confirmed downgrade/reinstallation is supported. Historical betas that may lack newer normal-release fixes carry explicit warnings. |
| Download | Separate explicit action, streamed with size/time limits and cancellation. A matching SHA-256 is required and rechecked before installation. |
| Installed copy | Exact installer registration determines installation directory/scope. The staged helper waits for the parent to exit, runs setup with normal Windows permission prompts, verifies the result and restarts once. |
| Portable copy | Keeps the current executable filename and only replaces allowlisted application files, with backup/rollback. It is not converted into an installed copy. |
| Previous updater | The separate Update dialog, duplicate Settings page, startup download and install-on-close path are retired. All installation now requires explicit confirmation through the same updater. |
| Existing preferences | If no new per-install snapshot exists, legacy Stable maps to Normal and legacy Beta (which included stable releases) maps to All. The legacy file is read-only; its automatic-install choice and queued packages are not imported. |

Code: `UpdatesControl.cs`, `UpdateFeedClient.cs`, `UpdateInstaller.cs`, `UpdateApplyWorker.cs`, `UpdatePreferences.cs` and the presentation-only `MainForm.Updates.cs`. Three-part versions are compared numerically; a normal release ranks above the betas of the same version. Drafts, manual builds, four-part versions and incompatible/missing assets are excluded. Profile/rules paths and Excel-export preferences are preserved, not profile contents or database passwords.

The repository's releases must remain publicly readable: the updater does not request GitHub credentials. The installer retains its conditional `/RELAUNCH=1` hook for already-released older updater clients; the current helper never supplies that switch and controls its own verified restart. See [UPDATING.md](UPDATING.md) for migration, first-upgrade instructions, safeguards and limitations.

## Workbook identity check (form)

`WorkbookIdentityForm` is the task **Comparison > Workbook identity check** (route `comparison.identity`). It only collects inputs and shows results; reading, comparing and writing the report are done by the engine described in the next section. The user guide is [WORKBOOK_IDENTITY_CHECK.md](WORKBOOK_IDENTITY_CHECK.md).

| SrNo. | Area | How it works |
|---|---|---|
| 1 | Workbooks | Source (File A) and Target (File B), `.xlsx` or `.xlsm`. After Browse, or after leaving the path field, `WorkbookIdentityEngine.Inspect` reads the sheet names and the header row in the background. Each file has a sheet choice: all sheets paired by name, or one sheet. |
| 2 | Row matching | By row position, or by primary key. The key is chosen in two tick lists, one per file. Each list numbers its ticked columns in tick order (`key part 1`, `key part 2`); part N of Source pairs with part N of Target, so the names may differ. |
| 3 | Options | Header row, number tolerance, difference thresholds (up to 8), and switches for cell formatting, sheet layout and formulas. Numbers use a dot as the decimal separator. |
| 4 | Checking | `CaptureRequest` checks every input on the UI thread through `FormValidation`; a wrong input is marked beside its field and written to the log, with no message box. The only message box asks before an existing report is replaced, with No as the default. |
| 5 | Running | One `BackgroundWorker` reads or compares. Inputs are locked meanwhile and Cancel stops the work. The report is written with `FormValidation.WriteAtomically`, so an existing report survives a failed or cancelled run. Closing the task while it works is refused until the work has stopped. |
| 6 | Result | Shown below the inputs on the same page: the verdict, how rows were matched, Open report / Open folder and five tabs. The tabs take their titles and rows from `IdentityReportWriter.Tables`, the same source as the report sheets, and show the first 1,000 rows; the report holds all rows. Editing an input afterwards marks the result as belonging to the earlier inputs. |
| 7 | Layout | Built in code with fixed row heights, so it has no Designer file and no layout description under `tools/layouts`. |

**Checks of the form:** `tests/WorkbookIdentityFormChecks.cs`, run in CI inside `--check-ui` with temporary workbooks: the route, reading both workbooks, refused inputs, a comparison by position and by key, the report file, keeping an existing report, and the close guard.

## Workbook identity check (engine)

Answers one question: **are two Excel workbooks identical?** It is a C# port of the owner-approved Python tool (`krishnabhunia/excel-sheet-comparison`, folder `cld`) and applies the same rules, so both write the same report. This part has **no window code**; the form in the Comparison workspace is described above (this engine: [issue #77](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/77)). The existing **Excel comparison** task is unchanged.

| SrNo. | Part | What it does |
|---|---|---|
| 1 | `WorkbookIdentityReader.cs` | Reads `.xlsx` / `.xlsm` with the Open XML SDK, read-only and shared, so a workbook open in Excel can be compared. It keeps what the file stores: values, cached formula results (nothing is recalculated), formulas (filled-down "shared" formulas are rebuilt), 12 style properties per cell format and the sheet layout. |
| 2 | `WorkbookIdentityEngine.cs` | `WorkbookIdentityEngine.Compare(fileA, fileB, options, progress, cancelled)` returns an `IdentityResult`. No window code, so it can run on a background worker. |
| 3 | `IdentityReportWriter.cs` | Writes the report with ClosedXML: Summary, Headers, Data Differences, Format Differences, Layout & Other. |

- **Matching.** Sheets pair by name and columns by header name, both ignoring case; header names also ignore outer spaces. Rows pair by position, or by an optional primary key: one column or several, with different column names in each file. Key values match as exact text. Blank or repeated key values are warnings and pair in order of appearance.
- **Checks (18, each MATCH or DIFF with a count).** Sheet count and names; header count, names, case and spacing, order, font, alignment; number of data rows; rows in only one file; row order; cell values; data types; formulas; cell font, alignment, number format, fill and borders; sheet layout (column widths, row heights, merged cells, freeze panes, auto-filter, hidden rows and columns, sheet visibility); comments, hyperlinks, data validation and conditional formatting.
- **Same-platform columns.** `Difference Type Flag` is `Number` when both values are numbers or text that reads as a number (`"750"`, `" 45.5 "`), otherwise `Text`. `Difference Type Value Flag` is `True` when the values still differ on that platform and `False` when only the data type or notation differs. `Number` rows also carry `Number Diff (B − A)` and one Yes/No column per threshold (`0`, `0.01`, `0.1`, `1`), strictly greater than.
- **Options.** `IdentityOptions`: header row, number tolerance, key columns, one chosen sheet per file, and switches to skip formatting, layout or formula checks. A skipped check shows `SKIPPED` in the Summary and is not counted.
- **Formats of blank cells and rules.** A cell the file does not store is blank and shows the format of its row, else of its column, so a format set on a whole column is compared on its blank cells too. Conditional formats and data validations are compared by what the rule does (type, operator, formulas, the format it applies), not only by where it is; settings a rule type does not use, and its priority number, are left out so that the same rule saved by two programs reads the same.
- **Limits.** `.xlsx` and `.xlsm` only. Text such as `1,200`, `₹500` or `10%` is not read as a number; dates and TRUE/FALSE cells compare as text. A formula that was never calculated in Excel has no saved result and compares as blank; the Summary says so. Merged cells, data validation and conditional formatting compare by cell address, not by key. Each report sheet lists at most 200,000 rows; the Summary still counts all.
- **Checks of the engine:** `tests/WorkbookIdentityChecks.cs`, run in CI inside `--check-ui` on synthetic workbooks in a temporary folder, and `tests/test_workbook_identity.py` for source contracts. The engine's output was also compared cell by cell with the Python tool's report on the Python samples before this change; that comparison is not part of CI.

## Input checks
- **One checker for every window:** `InputCheck.cs`. Before an action starts, the window lists what it needs and stops when something is missing or wrong:
  `if (!InputCheck.For(this).Connection(connection.Source).Required(tb_Table, "Table name").Report()) return;`
- **What the user sees:** a mark beside each wrong field (hovering shows the reason), every reason and the count ("3 inputs need fixing. Nothing was started.") in the window's log (`rtb_Log`), and the cursor in the first wrong field. A window without a log shows the reason at the field. No message box.
- **Checks available:** `Required`, `Number` / `Port` (1-65535), `FileExists` / `FilePath` (with allowed file types), `FolderExists` / `FolderPath`, `ValidName` (table, schema, column), `ValidFileName`, `AnyChosen` (lists, drop-downs, grids), `Connection`, and `That` for anything else. Switched-off fields are skipped.
- **Connection rules:** Oracle needs port, data source, database, user name and password. SQL Server needs data source and database, plus user name and password unless Windows authentication is ticked. `ConnectionPanel.AddChecks` adds them for the halves a window shows.
- **Where it runs now:** every button that connects, in the 11 windows with a connection (Compare DB, Compare List Tables, Execute Scripts, Pre Merging Tool, Duplicacy Check, FK Extraction, Legacy ID, Delta Change Tracker, Export DB To Excel, Table Extraction, User Profile).
- **Checks of the checker:** `tests/InputCheckChecks.cs`, run in CI as `--check-inputs`.

## Versioning and CI artifacts
Versions are automatic - nobody edits a version number by hand. `tools/release.py` works it out in CI:

| SrNo. | When | Version | Published as |
|---|---|---|---|
| 1 | Every push to a pull request | `x.y.z-betaNNN`, e.g. `2.4.0-beta003` | GitHub **pre-release** (beta) with the zip and its SHA-256 |
| 2 | The PR is merged to `master` | `x.y.z`, e.g. `2.4.0` | GitHub **release**, marked latest |
| 3 | Manual run (Actions > Run workflow) | `x.y.z-manual` | Artifact only, nothing published |

`x.y.z` = the latest stable tag `vX.Y.Z` bumped by the kind of change (with no stable tag yet, the `2.3.0` in `Properties/AssemblyInfo.cs` counts as the last release):

| SrNo. | Kind | Bumps | How it is recognised (first match wins) |
|---|---|---|---|
| 1 | Override | as given | PR label `release:major`, `release:minor` or `release:patch`, or `[major]` / `[minor]` / `[patch]` in the PR title |
| 2 | Major (x) | `2.4.1` → `3.0.0` | A line starting `BREAKING CHANGE:` in the title or body, a type ending in `!` (`feat!:`), or type `Breaking` |
| 3 | Minor (y) | `2.4.1` → `2.5.0` | Type `Feature` / `feat` (e.g. `Claude : Feature: ...`, `feat(ui): ...`) |
| 4 | Patch (z) | `2.4.1` → `2.4.2` | Everything else: `Fix`, `Docs`, `CI`, `Refactor`, titles with no type |

- `NNN` starts at `001` for each `x.y.z` and counts up across all PRs for that version; each new commit pushed to a PR gets the next number, re-running the same commit keeps its number. CI reserves the number by tagging the PR's head commit, so a number used by a failed build is skipped.
- Updaters rank `x.y.z-betaNNN` below `x.y.z`.
- **No warnings in Actions.** The workflows use action versions that run on Node.js 24 (`actions/checkout@v6`, `upload-artifact@v6`, `download-artifact@v7`, `setup-python@v6`, `microsoft/setup-msbuild@v3`, `NuGet/setup-nuget@v3`) and a named Linux image (`ubuntu-24.04`, not `ubuntu-latest`), so a run shows no deprecation warning or image-migration notice. `tests/test_workflows.py` fails if an older version or `ubuntu-latest` comes back. When GitHub announces the next change, raise these versions in the two workflow files and in that test.
- **Release clean-up (by hand).** `Actions > Release clean-up (run by hand) > Run workflow` lists the zips of window pictures that earlier releases still carry and the leftover test builds `v3.0.0-beta001` and `v2.5.1-beta001`. It deletes them only when you type `DELETE` in the confirm box; with the box empty it only lists. Deleted files cannot be restored. Once it has run, `.github/workflows/release-cleanup.yml` can be removed.
- **Where to read the version.** Never type a version into a PR title: the number depends on which PR is merged first, so a typed one goes stale. CI shows the real one in two places: the check named `Publish <version>`, and one comment on the PR that it keeps up to date - `Beta version: vX.Y.Z-betaNNN` with download links on every PR run, replaced by `Released as vX.Y.Z` when the PR is merged to `master`.
- **No outdated betas.** If the normal `vX.Y.Z` is released while a PR build for the same number is still running (another PR was merged first), that beta is not published; the PR comment says so. Re-run all jobs or push a commit to get the next beta, which then counts from the new number.
- CI writes the version into the exe (`AssemblyVersion` / `AssemblyFileVersion` `x.y.z.0`, `AssemblyInformationalVersion` = the full name, e.g. `2.4.0-beta003`).
- Every build opens every window once: `"SPI Data Comp Tool.exe" --open-form <FormType> <log file>` creates, shows and closes the window and writes `ok` or `FAIL` with the reason (`SelfTest.cs`; `--list-forms <file>` lists the windows). A window that cannot open fails the build. The mode never connects to a database. No pictures are taken or published.
- Each Windows build uploads one artifact, `SPI_Data_Comp_Tool_<version>`, which downloads as a zip containing:
  - `portable/` — `SPI_Data_Comp_Tool_<version>.exe` (Any CPU), its `.exe.config` and the `PNID_Reports/Source` and `PNID_Reports/Target_IPEP` SQL templates (read at runtime by **Execute Excel Scripts > PNID Reports**). Unzip and run; nothing is installed.
  - `windows-x64/` — `SPI_Data_Comp_Tool_<version>.exe`, the **64-bit installer** (Inno Setup, `windows/installer/SPI_Data_Comp_Tool.iss`) built from the `Release|x64` output. It installs to `C:\Program Files\SPI Data Comparison Tool` (or, without admin rights, just for the current user under `%LocalAppData%\Programs`), adds a Start-menu shortcut (desktop shortcut optional), checks for .NET Framework 4.6.2+, upgrades an older install in place and has an uninstaller in *Apps & features*. Silent install: `/VERYSILENT /SUPPRESSMSGBOXES [/CURRENTUSER] [/DIR="..."]`.
- **No DLL files ship.** Costura (via Fody, `FodyWeavers.xml`) embeds every referenced library into the `.exe`, compressed, and loads it from there at start-up; only the `.exe.config` and the PNID templates sit beside it. CI fails if a `.dll` appears next to either exe, and runs `"SPI Data Comp Tool.exe" --check-assemblies <file>` on the portable exe and on the installed 64-bit exe to prove every library loads from the exe. The installer is also smoke-tested in CI: silent install, 64-bit check, uninstall.
- **Only libraries the code uses are referenced.** EntityFramework, System.IO.Packaging and System.IO.FileSystem.Primitives were removed (nothing used them; Open XML on .NET 4.6 uses the built-in WindowsBase). Keep System.ValueTuple (needed on .NET 4.6.2) and System.ComponentModel.Annotations (EPPlus needs it). `tests/test_cleanup.py` guards this.
- **Every window can be opened.** A window must be reachable from the main menu or from another window, and the main menu has no hidden entries; `tests/test_cleanup.py` fails otherwise. To retire a window, delete it (files, project entries, menu entry) instead of hiding its menu entry.
- **Excel automation starts only when used.** The Excel Comparison window no longer starts Excel when it opens; the debug Load button starts it, reads the sheet name and quits it again, so no `EXCEL.EXE` is left running.
- Releases also carry the installer on its own as `SPI_Data_Comp_Tool_<version>_x64_setup.exe`; `SPI_Data_Comp_Tool_<version>.zip.sha256` lists the checksums of the zip and the installer. The installer is not code-signed, so Windows SmartScreen may ask to confirm the first run.

## Supported workflows vs. assumptions
The source has forms for database comparison, Excel comparison, delta change tracking and database-to-Excel export. Exact connection strings, supported server versions, data-size limits and complete workflows must be validated manually: the repository does not supply safe integration fixtures for all operations.

## Verification

### Form input validation

All 21 retained Form implementations, the added workspace host, separate Excel Sheet Audit, Workbook identity check and shared controls have an explicit validation/lifecycle inventory. See [VALIDATION.md](VALIDATION.md) for action gates, supported inputs, file/export safeguards, Windows validation/workspace suites and manual acceptance boundaries. Read-only screens retain navigation/open-close coverage rather than artificial required fields.

The GitHub Actions workflow (`build.yml`) runs on pull requests, pushes to `master` and on demand. It checks this guide, runs the complete Python regression suite and generated-layout check, and rejects committed credentials/signing keys. Both Any CPU and x64 Windows builds treat warnings as errors. Actions are pinned to reviewed Node 24 releases rather than relying on forced execution of deprecated Node 20 actions.

`--check-themes <log-file>` is configured to construct eligible concrete windows in each of the four retained palettes, check actual text/control colours, switch themes live and verify theme-choice persistence using temporary files. When a built-app self-test times out, CI reports that process's open windows through `tools/ci_window_report.ps1`, helping identify a waiting dialog. These configured checks still require the exact commit's successful Windows run.

`--check-groups <log-file>` remains a compatibility regression entry point in `tests/ToolGroupChecks.cs`. It exercises the nine formerly grouped workflows through their current workspace routes, page reuse and preservation of all loaded pages when a task refuses closure. It uses the common workspace host rather than maintaining a second navigation/close implementation. The `--check-workspace-exit <log-file>` runner retains the isolated successful-application-exit check used by `WorkspaceUiChecks`.

`--check-ui <log-file>` is configured to run real Windows controls against synthetic data: profile snapshots and failed file writes, profile selection and import schemas, Settings startup/failed import, embedded Analysis and failed reload preservation. `WorkspaceUiChecks` adds all 21 routes across six groups, lazy loading, page/session reuse, distinct Configuration state, close vetoes, transient mappings, failed construction, hidden workers and updater installation eligibility. It also exercises updater version/feed parsing, bounded verified downloads with fake transport, staged portable replacement and preference migration, and update controls with fake services. The retained `--check-updates <log-file>` entry point exercises the same canonical updater rather than a second implementation. `--check-excel-audit <log-file>` verifies the separate audit against synthetic worksheets and validates its two-sheet report. These checks never install a live release, access a database or save real user preferences. `--open-form` must successfully open every eligible concrete form; a failed opening fails the build. No pictures are taken or published. Portable dependency loading and 64-bit installer install/uninstall checks remain enabled. **Read CI results for the exact commit** rather than assuming success. Live Oracle/SQL Server connectivity, real customer workbooks, all comparison outcomes, real-user UAC/SmartScreen and older Windows/.NET installations still require manual acceptance tests.

## Limitations and security
- Some screens may update data; test in isolated environments, never on production by default.
- Avoid embedding real database credentials in commits, documentation or CI logs. Earlier commits contained a real profile file and hard-coded passwords. They are gone from the latest code, but git history still holds them until it is rewritten (a separate, owner-approved step that changes every commit id). Treat those credentials as exposed and rotate any that are still in use.
- Queries are built by string concatenation from table/column names taken from rule files and the UI. Only use trusted rule files.
- The Excel library's license terms and database drivers need separate validation.
- No genuine geographic data or measured benchmarks were found, so geographical maps and performance charts are not fabricated.
