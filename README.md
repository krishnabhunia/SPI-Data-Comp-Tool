# SPI Data Comparison Tool

Generic Windows Forms (.NET Framework 4.6.2) desktop utility for database and Excel comparison, SPI delta analysis, pre-merge duplicate checks and data exports. It works against Oracle and SQL Server.

| SrNo. | Topic | Where |
|---|---|---|
| 1 | Architecture, feature inventory, diagrams, setup, testing, limitations | [docs/PROJECT_GUIDE.md](docs/PROJECT_GUIDE.md) |
| 2 | Connection-profile template (no real servers or passwords) | `SPI Data Comp Tool/UserProfile/userProfile.example.json` |
| 3 | App icon and pictures (original, regenerate with `python tools/make_app_images.py`) | `SPI Data Comp Tool/app.ico`, `SPI Data Comp Tool/Resources/*.png` |
| 4 | CI build and downloadable portable zip | `.github/workflows/build.yml` → Actions → artifact `SPI_Data_Comp_Tool_<version>` |

## Quick start
1. Open `SPI Data Comp Tool.sln` in Visual Studio (with the **.NET desktop development** workload) and restore NuGet packages.
2. Build `Release | Any CPU` and run. The main window opens ready to use; there is no login screen (since 2.2.0).
3. Create your own connection profile from **File > User Profile** (or copy `userProfile.example.json` to `userProfile.json` and fill it in). `userProfile.json` is git-ignored — never commit it.
