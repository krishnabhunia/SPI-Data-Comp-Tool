# SPI Data Comparison Tool

Generic Windows Forms (.NET Framework 4.8) desktop utility for database and Excel comparison, workbook audits and data exports. It works against Oracle and SQL Server.

The main window's left rail lists 7 tasks in three groups: **Comparison** (Compare databases, Compare workbooks), **Data & Reports** and **Settings**. Compare databases (One table / A list of tables) and Compare workbooks (Values / Values and formatting / Identical?, for One pair / Two folders) choose their workflow with a mode bar. Switching tasks or modes preserves each page's inputs and results. The SQL Scripts workspace and both its generation/execution forms have been removed. Database comparison and database-to-Excel export remain available.

The earlier Analysis & Migration and Pre-merge Checks workspaces, their task forms and their dedicated supporting code also remain removed. The application before that retirement is preserved in [release v2.16.0](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/releases/tag/v2.16.0).

The existing **Ocean**, **Berry**, **Forest** and **Midnight** colour themes remain available. Choose one on the Settings preferences task's **General** tab; the home tiles, workspaces and loaded pages change colour immediately, without reopening the tasks. Ocean is the default; Midnight is the dark theme.

**Compare workbooks › Values and formatting** (Excel Sheet Audit) is a separate native C# form for two selected worksheets: optional single/composite keys, case-insensitive column-name matching, exact typed values, headers, formatting and layout. It exports exactly two sheets: **Summary** and **Data Differences**. The original value comparison (**Compare workbooks › Values**) is unchanged. See the [audit guide](docs/EXCEL_SHEET_AUDIT.md) for validation, scope and resource limits.

**Compare workbooks › Identical?** (Workbook identity check) answers one question: are two workbooks identical? It compares every sheet (or one chosen sheet) by row position or by an optional primary key, lists the 18 checks on screen and writes a five-sheet report: **Summary**, **Headers**, **Data Differences**, **Format Differences** and **Layout & Other**. See the [identity check guide](docs/WORKBOOK_IDENTITY_CHECK.md).

| SrNo. | Topic | Where |
|---|---|---|
| 1 | Architecture, feature inventory, diagrams, setup, testing, limitations | [docs/PROJECT_GUIDE.md](docs/PROJECT_GUIDE.md) |
| 2 | Connection-profile template (no real servers or passwords) | `SPI Data Comp Tool/UserProfile/userProfile.example.json` |
| 3 | App icon and pictures (original, regenerate with `python tools/make_app_images.py`) | `SPI Data Comp Tool/app.ico`, `SPI Data Comp Tool/Resources/*.png` |
| 4 | CI build: portable exe and 64-bit installer (one exe each, libraries embedded) | `.github/workflows/build.yml` → Actions → artifact `SPI_Data_Comp_Tool_<version>`, or the release page (`…_x64_setup.exe`) |
| 5 | In-app normal/beta updates, version selection and recovery | [docs/UPDATING.md](docs/UPDATING.md) |
| 6 | Current workspace/task map, independent sessions and retirement boundary | [docs/WORKSPACES.md](docs/WORKSPACES.md) |
| 7 | Separate Excel Sheet Audit: Design A, key mappings, two-sheet report and limits | [docs/EXCEL_SHEET_AUDIT.md](docs/EXCEL_SHEET_AUDIT.md) |
| 8 | Workbook identity check: are two workbooks identical? Inputs, the five-sheet report and limits | [docs/WORKBOOK_IDENTITY_CHECK.md](docs/WORKBOOK_IDENTITY_CHECK.md) |
| 9 | Open-issue evidence, remaining acceptance and framework migration plan | [docs/ISSUE_AUDIT.md](docs/ISSUE_AUDIT.md), [docs/FRAMEWORK_MIGRATION.md](docs/FRAMEWORK_MIGRATION.md) |

## Quick start

1. Open `SPI Data Comp Tool.sln` in Visual Studio (with the **.NET desktop development** workload) and restore NuGet packages.
2. Build `Release | Any CPU` and run. The main window opens ready to use; there is no login screen (since 2.2.0).
3. Create your own connection profile from **Settings > Connection profiles** (or copy `userProfile.example.json` to `userProfile.json` and fill it in). `userProfile.json` is git-ignored — never commit it.
4. Choose a task in the left rail of the main window — the rail is the only way to open a task (no menu bar, no home tiles). Reopening a task returns to its existing page. For supported tasks, **New session** opens a second, independent copy in its own window.
5. Optionally choose **Settings > Preferences, rules & updates > General > Colour theme**. The choice applies live and is remembered outside the installation folder when saving succeeds.
6. Open **Settings > Preferences, rules & updates > Updates** (the only place updates are offered; **About** is the tab next to it) to check published normal/beta releases. Select a version, download it, then explicitly confirm installation and restart. Close data workspaces and the Connection profiles task first. Older builds without this page need one manual upgrade first.
