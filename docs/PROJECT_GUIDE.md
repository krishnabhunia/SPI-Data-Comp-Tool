# SPI Data Comparison Tool — implementation guide

## Overview
This is a **Windows Forms / C# desktop project** for database and Excel comparisons, SPI delta tracking, merging workflows, exports and related data-analysis utilities. Different forms implement different workflows; their existence is not proof every process has been end-to-end tested.

## Verified source map
| Source | Responsibility |
|---|---|
| `SPI Data Comp Tool/Program.cs` | Dispatches the isolated update helper, CI checks, or normal application startup |
| `SPI Data Comp Tool/MainForm.cs` | Main window (menus, About, exit) |
| `SPI Data Comp Tool/DB Comparison.cs` | Database-comparison UI |
| `SPI Data Comp Tool/Excel_Excel_Comparison.cs` | Excel comparison |
| `SPI Data Comp Tool/SPIDeltaChangeTrackerOptimised.cs` | Delta tracking form (the only one: the first tracker window, whose menu entry was always hidden, was removed) |
| `SPI Data Comp Tool/SQL_DB_To_Excel.cs` | Export UI |
| `SPI Data Comp Tool/Common_Data.cs` | Shared helpers: DB connections, Excel I/O, profiles, developer mode (`GlobalDebug`) |
| `SPI Data Comp Tool/UserProfile/userProfile.example.json` | Template for a local connection profile |
| `SPI Data Comp Tool.sln` | Visual Studio solution |
| `.github/workflows/build.yml` | CI: docs check, secrets and branding guard, Windows build, portable exe + 64-bit installer in one zip artifact |
| `windows/installer/SPI_Data_Comp_Tool.iss` | Inno Setup script for the 64-bit installer |
| `tools/make_app_images.py` | Draws the app icon and pictures (original artwork) |
| `SPI Data Comp Tool/UpdatesControl.cs` | The Updates page inside Settings; Help reuses that same window |
| `SPI Data Comp Tool/UpdateFeedClient.cs`, `UpdateVersion.cs` | Published release discovery, three-part version ordering and verified downloads |
| `SPI Data Comp Tool/UpdateInstaller.cs`, `UpdateApplyWorker.cs`, `UpdatePreferences.cs` | Installed/portable update handoff, bounded file replacement and whitelisted preference preservation |

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
| 2 | Login | None. The old login screen (a user name and password written in this source) was removed in 2.2.0; the main window opens unlocked. |
| 3 | Developer mode | On when a debugger is attached, or when the environment variable `SPI_DATA_COMP_DEBUG=1` is set. It is no longer tied to specific Windows user names or a built-in password. The profile editor omits passwords from new JSON exports in every mode; enter them again when needed. |
| 4 | Default output file (pre-merging tool) | `Documents\Output.xlsx` until you pick another location. |
| 5 | Logs | `Desktop\LogFolder\Log_dd_MM_yyyy*.log` |

## Branding and images
- The tool is generic: no company names, logos or trademarks.
- Every image is original and drawn by `tools/make_app_images.py` (Python + Pillow): `app.ico` (exe and all window icons, 16-256 px) and `Resources/about_side.png` (About box). The main window has no picture: its header band and tool tiles are drawn in the chosen colour theme.
- To change the look, edit the script, run `python tools/make_app_images.py` from the repository root (it also refreshes the embedded window icon of every form), and rebuild.
- CI fails if any other image file is committed or if former-employer names reappear in source, settings or docs.

## Window layout and look
- Layout is generated: each window has a short description in `tools/layouts/<Form>.py`, and `python tools/layout_gen.py tools/layouts/<Form>.py` rewrites its `*.Designer.cs`. Controls sit in `TableLayoutPanel` / `FlowLayoutPanel` cells, so they are evenly spread and grow with the window; every control keeps its name, events and settings, so no logic changes. CI fails if a designer file no longer matches its description (`--check`).
- Look: `UiTheme.Apply(this, mainButton)` at the end of a constructor gives every window the same style - Segoe UI 9, group boxes as cards, flat buttons, one main button in the theme's accent colour, and the app icon. A window opened from the main window is themed even when it does not call `Apply` (`WindowNavigator` calls `UiTheme.EnsureApplied`).
- Colour themes: `UiTheme.cs` holds four palettes - **Ocean** (default), **Berry**, **Forest** and **Midnight** (dark). The user picks one in **Settings > General > Colour theme**; it recolours every open window at once and is saved in `%AppData%\SPI Data Comparison Tool\theme.txt` (outside the app folder, so an update keeps it; a missing or unreadable file means Ocean). A theme colours the window, group boxes, buttons, fields, grids (coloured header, selection in the accent colour), menus, tabs, the progress bar (plain drawing in the accent colour), the main window's header band, tool tiles and status bar, and tells **Source** and **Target** apart: their group boxes get a tinted caption band with a coloured line, and grid columns named after Source or Target get the same tint.
- Status colours: a window may still say `Color.Red`, `Color.Black`, `Color.DarkGreen` or `Color.DarkBlue` for a label; the theme shows the readable colour for that meaning (`UiTheme.Readable`), so an error is light red on Midnight and dark red on the light themes. Log text goes through the same mapping. Cells marked yellow keep black text in every theme.
- Readability is checked twice: `tests/test_ui_contrast.py` checks every text / background pair of every palette (WCAG AA, 4.5:1; lines and glyphs 3:1), and `--check-themes` (`tests/ThemeChecks.cs`, run by CI on Windows) builds every window in every theme, reads the colours its labels, buttons, fields and grids really have, and switches themes while windows are open. A disabled button is drawn by the theme, because Windows would otherwise draw its text light grey.
- What Windows draws itself keeps Windows' colours, also in Midnight: the strip behind a tab control's tabs, an opened drop-down list of a non-flat combo box, the date picker, list-view column headers and message boxes. The title bar and scroll bars turn dark on Windows 10 1809 and later.
- Main window: a header band, four tiles for the tools most people start with (Compare databases, Compare Excel files, Delta change tracker, Export to Excel - each opens the same window as its menu entry) and a status bar with the version. `TileButton`, `GlyphIcon` and `ThemeSwatch` (in `ThemeControls.cs`) are small owner-drawn controls; a tile is still a `Button`, so keyboard and screen readers work as usual. To add a theme, add a palette to `UiTheme.cs`, list it in `All`, and add its name to `THEMES` in the contrast test.
- Group windows: related tools share one window with a tab per tool (`ToolGroups.cs`) - **Pre-merging** (Pre-merging tool, Duplicacy check, Configuration), **Excel and file tools** (Customised Excel, Merge logs to Excel, Spec notes, Generate scripts) and **Delta tools** (Table extraction after delta, Legacy ID). Each tab holds the tool's own window unchanged (`TopLevel = false`, no border, docked), built the first time its tab is shown, so no tool's code or checks changed. A menu entry opens its group window on the right tab; holding Ctrl still opens the tool in a window of its own. Closing the group window closes its tools, and a tool that is busy keeps it open. `--check-groups` (`tests/ToolGroupChecks.cs`, run by CI) builds every tab. To add a tool to a group, add one `AddTool` line and route its menu entry through `OpenTool`.
- Settings holds **User profiles** the same way: its fourth tab, **Profiles**, shows the User profiles window unchanged, built the first time the tab is shown. File > User Profile opens Settings on that tab. Settings grows once to fit it (never beyond the screen), and on that tab its own log is hidden because the profiles window has one.
- Shared code: `FormHelpers` holds the helpers several windows used to copy (status text and errors, elapsed time, summary transpose, Excel table formatting, table pickers). A window keeps a short wrapper with its own error handling and calls the helper; add new shared logic there instead of copying it.
- Connection panel: `ConnectionPanel` (a UserControl) holds the profile picker, the Source and Target halves (`ConnectionSide`) and the Copy buttons, with their Oracle / SQL Server, Windows-login and default-port behaviour. A window places it with `new="SPI_Data_Comp_Tool.ConnectionPanel"` in its layout spec, sets `Mode` (`SourceAndTarget` or `SourceOnly`), handles `ProfileApplied` for its own extras, and keeps its old control names as one-line properties (`private TextBox tb_SRC_DataSource => connection.Source.DataSource;`), so the rest of its code is unchanged. Ten windows use it; the Optimised Delta Tracker and Analysis's older query/connection control remain separate. Single-connection windows use `Mode = SourceOnly` (and `Source.Title = "Database"`); `ProfileHalf` fills that connection from a profile's Source or Target half, `SqlServerOnly` locks the type for SQL Server-only windows, `ShowProfile` hides the picker. Parts the specs share (the panel, the dropped controls, table pickers) live in `tools/layouts/_connection.py`; `KEEP = [...]` places a control but leaves its contents as designed, and `TABS(...)` lays out a TabControl's pages.

### Consolidated workflows

| Previous UI | Current route | Preserved behaviour |
|---|---|---|
| Settings and a separate Rules import form | Settings: General / Comparison rules / Updates tabs | All five existing Import Rules buttons open the Rules tab; Close/Escape returns to the comparison. Invalid/cancelled imports retain existing rules. Help's update shortcut activates the same Settings window. |
| Analysis setup and a separate results window | Analysis: Connection setup / Results tabs | Source and target filters, Arrange, remarks, synchronized grids and Excel export remain. Results use an independent snapshot. Replacing results requires confirmation. |
| Fake profile list and nonfunctional Save/Delete in Analysis | Removed | No working functionality was attached; the real User Profile window remains. |
| Repeated main-menu/tool clicks opening duplicate windows | Activate the existing workflow window | Minimized windows restore. Hold Ctrl when opening a tool for an explicit parallel session; Settings, User Profile and configuration remain single-instance per main window. Duplicacy Check and its Configuration mode have separate keys. |

There are now **21 Form classes** (23 on the base branch after #44). The retired Rules dialog is available in git history; Analysis results are a reusable control, not a deleted feature. Database comparison, Excel comparison, batch comparison, Optimised Delta Tracker, legacy-ID mapping, extraction, pre-merging, duplicate checking, script generation/execution, customised Excel and merge-log export remain distinct workflows. Similar appearance alone is not a reason to delete them.

Profile edits build independent candidate lists. Cancelling a save does not change active profiles; JSON is written to a same-directory temporary file and atomically replaced before publishing a changed list. Deletion requires confirmation. If the file is saved but its path cannot be remembered, the UI explicitly reports the partial outcome and retains the active catalog. Shared pickers resolve names rather than stale list positions, and typing a new name preserves entered fields.

## Updates in the app

**File > User Settings > Updates** and **Help > Check for updates** lead to one designer-defined Updates page. Starting the app or opening Settings performs no update network request. The main status line shows the running version and its actual Normal/Beta build type; the release filter is a separate choice.

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

All 22 current Forms and the shared input controls have an explicit validation inventory. See [VALIDATION.md](VALIDATION.md) for action gates, supported inputs, file/export safeguards, five added Windows validation suites and the manual acceptance boundaries. Read-only screens retain navigation/open-close coverage rather than artificial required fields.

The GitHub Actions workflow (`build.yml`) runs on pull requests, pushes to `master` and on demand. It checks this guide, runs the complete Python regression suite and generated-layout check, and rejects committed credentials/signing keys. Both Any CPU and x64 Windows builds treat warnings as errors. Actions are pinned to reviewed Node 24 releases rather than relying on forced execution of deprecated Node 20 actions.

`--check-themes <log-file>` builds all windows in the four colour themes and fails on any unreadable text (see *Window layout and look*). When a self-test of the built app does not finish in time, CI prints the windows that process has open (`tools/ci_window_report.ps1`), so a waiting error dialog is named in the log.

`--check-ui <log-file>` runs real Windows controls against synthetic data: profile snapshots and failed file writes, profile selection and import schemas, window reuse and independent modes, Settings startup/failed import, embedded Analysis and failed reload preservation. It also exercises updater version/feed parsing, bounded verified downloads with fake transport, staged portable replacement and preference migration, and the update controls with fake services. The retained `--check-updates <log-file>` entry point exercises the same canonical updater rather than a second implementation. These updater checks never install a live release, access a database or save real user preferences. `--open-form` must successfully open every eligible form; a failed form fails the build. No pictures are taken or published. Portable dependency loading and 64-bit installer install/uninstall checks remain enabled. **Read CI results** rather than assuming success. Live Oracle/SQL Server connectivity, real customer workbooks, all comparison outcomes, real-user UAC/SmartScreen and older Windows/.NET installations still require manual acceptance tests.

## Limitations and security
- Some screens may update data; test in isolated environments, never on production by default.
- Avoid embedding real database credentials in commits, documentation or CI logs. Earlier commits contained a real profile file and hard-coded passwords. They are gone from the latest code, but git history still holds them until it is rewritten (a separate, owner-approved step that changes every commit id). Treat those credentials as exposed and rotate any that are still in use.
- Queries are built by string concatenation from table/column names taken from rule files and the UI. Only use trusted rule files.
- The Excel library's license terms and database drivers need separate validation.
- No genuine geographic data or measured benchmarks were found, so geographical maps and performance charts are not fabricated.
