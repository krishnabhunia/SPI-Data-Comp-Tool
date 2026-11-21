# SPI Data Comparison Tool

Generic Windows Forms (.NET Framework 4.6.2) desktop utility for database and Excel comparison, SPI delta analysis, pre-merge duplicate checks and data exports. It works against Oracle and SQL Server.

The main window now groups existing tasks into six workspace tiles: **Comparison**, **Analysis & Migration**, **Pre-merge Checks**, **SQL Scripts**, **Data & Reports**, and **Settings**. Switching tasks preserves each page's inputs and results. This is a navigation/interface consolidation, not a replacement of the comparison or migration engines.

The existing **Ocean**, **Berry**, **Forest** and **Midnight** colour themes remain available. Choose one on the Settings preferences task's **General** tab; the home tiles, workspaces and loaded pages change colour immediately, without reopening the tasks. Ocean is the default; Midnight is the dark theme.

| SrNo. | Topic | Where |
|---|---|---|
| 1 | Architecture, feature inventory, diagrams, setup, testing, limitations | [docs/PROJECT_GUIDE.md](docs/PROJECT_GUIDE.md) |
| 2 | Connection-profile template (no real servers or passwords) | `SPI Data Comp Tool/UserProfile/userProfile.example.json` |
| 3 | App icon and pictures (original, regenerate with `python tools/make_app_images.py`) | `SPI Data Comp Tool/app.ico`, `SPI Data Comp Tool/Resources/*.png` |
| 4 | CI build: portable exe and 64-bit installer (one exe each, libraries embedded) | `.github/workflows/build.yml` → Actions → artifact `SPI_Data_Comp_Tool_<version>`, or the release page (`…_x64_setup.exe`) |
| 5 | In-app normal/beta updates, version selection and recovery | [docs/UPDATING.md](docs/UPDATING.md) |
| 6 | Workspace/task map, independent sessions and first-phase boundaries | [docs/WORKSPACES.md](docs/WORKSPACES.md) |

## Quick start

1. Open `SPI Data Comp Tool.sln` in Visual Studio (with the **.NET desktop development** workload) and restore NuGet packages.
2. Build `Release | Any CPU` and run. The main window opens ready to use; there is no login screen (since 2.2.0).
3. Create your own connection profile from **Settings > Connection profiles** (or copy `userProfile.example.json` to `userProfile.json` and fill it in). `userProfile.json` is git-ignored — never commit it.
4. Choose a workspace and task. Reopening a task returns to its existing page. For supported tasks, hold **Ctrl** when opening it or use **Open separate session** to start with independent inputs.
5. Optionally choose **Settings > Preferences, rules & updates > General > Colour theme**. The choice applies live and is remembered outside the installation folder when saving succeeds.
6. Open **Settings > Preferences, rules & updates > Updates**, or **Help > Check for updates**, to check published normal/beta releases. Select a version, download it, then explicitly confirm installation and restart. Close data workspaces and the Connection profiles task first. Older builds without this page need one manual upgrade first.
