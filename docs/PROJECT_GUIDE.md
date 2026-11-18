# SPI Data Comparison Tool — implementation guide

## Overview
This is a **Windows Forms / C# desktop project** for database and Excel comparisons, SPI delta tracking, merging workflows, exports and related data-analysis utilities. Different forms implement different workflows; their existence is not proof every process has been end-to-end tested.

## Verified source map
| Source | Responsibility |
|---|---|
| `SPI Data Comp Tool/Program.cs` | Startup; chooses main or debug-specific forms |
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
- Every image is original and drawn by `tools/make_app_images.py` (Python + Pillow): `app.ico` (exe and all window icons, 16-256 px), `Resources/app_banner.png` and `Resources/compare_diagram.png` (main window), `Resources/about_side.png` (About box).
- To change the look, edit the script, run `python tools/make_app_images.py` from the repository root (it also refreshes the embedded window icon of every form), and rebuild.
- CI fails if any other image file is committed or if former-employer names reappear in source, settings or docs.

## Window layout and look
- Layout is generated: each window has a short description in `tools/layouts/<Form>.py`, and `python tools/layout_gen.py tools/layouts/<Form>.py` rewrites its `*.Designer.cs`. Controls sit in `TableLayoutPanel` / `FlowLayoutPanel` cells, so they are evenly spread and grow with the window; every control keeps its name, events and settings, so no logic changes. CI fails if a designer file no longer matches its description (`--check`).
- Look: `UiTheme.Apply(this, mainButton)` at the end of each constructor gives the Windows 11 style - Segoe UI 9, light-grey window, white group boxes, flat buttons, one blue main button and the app icon. `UiTheme.SetIcon(this)` only sets the icon. Every button state keeps WCAG AA contrast (4.5:1 or more, checked by `tests/test_ui_contrast.py`): the main button is white on blue only while enabled; any disabled button is drawn dark grey on light grey, because Windows would otherwise draw its text light grey (unreadable on blue).
- Converted so far: main window (shows the version in its status line), Compare DB (template for the Source/Target windows), User Settings. The rest follow in batches.
- Shared code: `FormHelpers` holds the helpers several windows used to copy (status text and errors, elapsed time, summary transpose, Excel table formatting, table pickers). A window keeps a short wrapper with its own error handling and calls the helper; add new shared logic there instead of copying it.
- Connection panel: `ConnectionPanel` (a UserControl) holds the profile picker, the Source and Target halves (`ConnectionSide`) and the Copy buttons, with their Oracle / SQL Server, Windows-login and default-port behaviour. A window places it with `new="SPI_Data_Comp_Tool.ConnectionPanel"` in its layout spec, sets `Mode` (`SourceAndTarget` or `SourceOnly`), handles `ProfileApplied` for its own extras, and keeps its old control names as one-line properties (`private TextBox tb_SRC_DataSource => connection.Source.DataSource;`), so the rest of its code is unchanged. Ten windows use it; the Optimised Delta Tracker and Analysis's older query/connection control remain separate. Single-connection windows use `Mode = SourceOnly` (and `Source.Title = "Database"`); `ProfileHalf` fills that connection from a profile's Source or Target half, `SqlServerOnly` locks the type for SQL Server-only windows, `ShowProfile` hides the picker. Parts the specs share (the panel, the dropped controls, table pickers) live in `tools/layouts/_connection.py`; `KEEP = [...]` places a control but leaves its contents as designed, and `TABS(...)` lays out a TabControl's pages.

### Consolidated workflows

| Previous UI | Current route | Preserved behaviour |
|---|---|---|
| Settings and a separate Rules import form | Settings: General / Comparison rules tabs | All five existing Import Rules buttons open the Rules tab; Close/Escape returns to the comparison. Invalid/cancelled imports retain existing rules. |
| Analysis setup and a separate results window | Analysis: Connection setup / Results tabs | Source and target filters, Arrange, remarks, synchronized grids and Excel export remain. Results use an independent snapshot. Replacing results requires confirmation. |
| Fake profile list and nonfunctional Save/Delete in Analysis | Removed | No working functionality was attached; the real User Profile window remains. |
| Repeated main-menu/tool clicks opening duplicate windows | Activate the existing workflow window | Minimized windows restore. Hold Ctrl when opening a tool for an explicit parallel session; Settings, User Profile and configuration remain single-instance per main window. Duplicacy Check and its Configuration mode have separate keys. |

There are now **21 Form classes** (23 on the base branch after #44). The retired Rules dialog is available in git history; Analysis results are a reusable control, not a deleted feature. Database comparison, Excel comparison, batch comparison, Optimised Delta Tracker, legacy-ID mapping, extraction, pre-merging, duplicate checking, script generation/execution, customised Excel and merge-log export remain distinct workflows. Similar appearance alone is not a reason to delete them.

Profile edits build independent candidate lists. Cancelling a save does not change active profiles; JSON is written to a same-directory temporary file and atomically replaced before publishing a changed list. Deletion requires confirmation. If the file is saved but its path cannot be remembered, the UI explicitly reports the partial outcome and retains the active catalog. Shared pickers resolve names rather than stale list positions, and typing a new name preserves entered fields.

## Updates in the app
- **Check.** Every start, the main window asks GitHub for this repository's releases in the background (`UpdateService.FindUpdate`). Offline or a GitHub error stays silent. Build checks (`--open-form`, `--check-…`) never go online.
- **Channels.** *Stable* (default): normal releases only. *Beta* (optional): also pre-releases built from open pull requests. Chosen in **Settings › Updates**; the status line of the main window shows the version and the channel.
- **What is offered.** The highest version above the running one. A beta ranks below the normal version with the same number; a beta published before the newest normal release is not offered (it was built on older code); a release without its `.zip.sha256` file is not offered. Never a downgrade.
- **Where it shows.** An **Update to v…** entry appears at the right end of the menu bar only when an update exists. It opens the Update window: release notes, **Update now**, **Install when I close the app**, **Later**.
- **Download and check.** An installed copy (it has `unins000.exe` beside it) downloads `…_x64_setup.exe`; a portable copy downloads the zip. The file's SHA-256 must equal the one in the release's `.zip.sha256`, otherwise it is deleted and nothing is installed.
- **Install.** Installed copy: the installer runs silently (Windows asks for permission) and starts the app again (`/RELAUNCH=1`). Portable copy: the zip's `portable/` files are put beside the running exe - the new exe has the new version in its name, existing files are never overwritten, and the old exe stays as the way back.
- **Silent option.** With "Download updates in the background and install them when I close the app" on, the download happens at start-up and the install when the main window closes.
- **Choices are kept** in `%AppData%\SPI Data Comparison Tool\updates.json`, outside the app folder.
- **Needs a public repository.** The app reads releases without signing in. If this repository is made private, the updater finds nothing until releases are published somewhere public and `UpdateService.Repository` points there.
- Code: `UpdateService.cs` (logic), `UpdateDialog.cs`, `UpdateSettingsPage.cs`, `MainForm.Updates.cs`. Checks: `tests/UpdateChecks.cs`, run in CI as `--check-updates`.

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
The GitHub Actions workflow (`build.yml`) runs on pull requests, pushes to `master` and on demand. It checks this guide, runs the complete Python regression suite and generated-layout check, and rejects committed credentials/signing keys. Both Any CPU and x64 Windows builds treat warnings as errors. Actions are pinned to reviewed Node 24 releases rather than relying on forced execution of deprecated Node 20 actions.

`--check-ui <log-file>` runs real Windows controls against synthetic data: profile snapshots and failed file writes, profile selection and import schemas, window reuse and independent modes, Settings startup/failed import, embedded Analysis and failed reload preservation. `--open-form` must successfully open every eligible form; a failed form fails the build. No pictures are taken or published. Portable dependency loading and 64-bit installer install/uninstall checks remain enabled. **Read CI results** rather than assuming success. Live Oracle/SQL Server connectivity, real customer workbooks, all comparison outcomes and older Windows/.NET installations still require manual acceptance tests.

## Limitations and security
- Some screens may update data; test in isolated environments, never on production by default.
- Avoid embedding real database credentials in commits, documentation or CI logs. Earlier commits contained a real profile file and hard-coded passwords. They are gone from the latest code, but git history still holds them until it is rewritten (a separate, owner-approved step that changes every commit id). Treat those credentials as exposed and rotate any that are still in use.
- Queries are built by string concatenation from table/column names taken from rule files and the UI. Only use trusted rule files.
- The Excel library's license terms and database drivers need separate validation.
- No genuine geographic data or measured benchmarks were found, so geographical maps and performance charts are not fabricated.
