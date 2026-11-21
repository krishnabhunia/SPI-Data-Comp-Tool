# .NET Framework 4.8 adoption and acceptance

## Adopted target

The project, application runtime configuration, package target metadata, bootstrapper and installer now require **.NET Framework 4.8**. Both normal release builds and the separate compatibility workflow use that target. The compatibility script verifies the checked-in target and built configuration; it no longer overrides the target or rewrites the output configuration to make a candidate appear compatible.

Microsoft lists **12 January 2027** as .NET Framework 4.6.2's support end. Framework 4.8 follows the lifecycle of its supported Windows installation. The 4.8 target keeps compatibility with more Windows versions than 4.8.1; it does not establish product support for every operating system that can install the runtime.

Sources checked on 22 November 2026:

- [Microsoft .NET Framework lifecycle](https://learn.microsoft.com/lifecycle/products/microsoft-net-framework)
- [Framework and Windows versions](https://learn.microsoft.com/en-us/dotnet/framework/install/versions-and-dependencies)
- [Minimum installed-framework Release values](https://learn.microsoft.com/en-us/dotnet/framework/install/how-to-determine-which-versions-are-installed)

Existing dependency versions and assembly asset paths are retained. A package's `lib/net462` asset remains usable by a 4.8 application; changing the application target does not require renaming that package directory or inventing a `net48` asset. Dependency loading, Oracle managed-driver assembly loading, spreadsheet exports and Costura embedding are exercised by the executable gates.

## Install and update transition

The installer requires a framework registry Release value of at least **528040**, including later servicing versions. Its check runs before application installation. Portable-update preflight validates the declared runtime in the bounded configuration from the downloaded ZIP before changing files or settings. Missing, ambiguous and unknown runtime declarations fail closed. Tests cover the exact minimum, newer versions, a lower/unknown installed version and preserving the existing executable/settings on rejection.

**Older portable updaters cannot acquire this check before they install the new application.** Install [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) first, or use the new installer. Both published release notes and automated PR download comments state this requirement. Keep the old application folder/installer and profile/rules files until the new build has been accepted. Installing 4.8 is an in-place framework update; rolling back SPI does not downgrade Windows' framework installation.

No updater path installs the framework or changes machine registry settings automatically. For installed updates, the helper can migrate preferences before Inno Setup performs its prerequisite check, and attempts to roll those preferences back if setup fails. The portable rejection-before-write guarantee does not apply to that installed workflow. Existing application backups and rollback behavior remain in place.

## Automated evidence and remaining acceptance

| Area | Automated coverage | Remaining acceptance |
| --- | --- | --- |
| Project/runtime coherence | Project target, runtime SKU, package metadata and installer minimum agree | Successful check on the proposed PR's exact commit |
| Windows runtime compatibility | Windows 2022 and 2025 runners, Any CPU and x64; real UI/profile/startup, themes, comparison/export, updater and dependency checks | Supported Windows client editions, desktop scaling/accessibility and Microsoft Excel automation |
| Database providers | SQL Server disposable integration and Oracle assembly loading | Disposable Oracle server/authentication integration |
| Deployment | Portable extraction/rollback regressions, installer creation and install/uninstall smoke tests | Upgrade from the user's installed versions on representative machines; missing-framework installer behavior on a disposable VM |
| Performance | Deterministic synthetic matching timings, row-count assertions and memory measurements | Customer workbook/database shapes and operational limits; see [PERFORMANCE.md](PERFORMANCE.md) |
| Signing | Existing unsigned build policy preserved | Owner-provided signing identity/service if signed distribution is required |

Successful checks establish the cases they execute, not unperformed manual acceptance. The PR remains unmerged until reviewed.

## Run the checks

On a disposable Windows machine with Visual Studio MSBuild, the .NET Framework 4.8 developer pack and NuGet:

```powershell
nuget restore "SPI Data Comp Tool.sln" -NonInteractive
./tools/check_framework_compatibility.ps1
```

Check outputs go to `bin/Compatibility48` and `obj/Compatibility48`. Logs and comparison measurements go to `RUNNER_TEMP`, or the system temporary directory when run locally. CI retains these as the `framework48-evidence-windows-2022` and `framework48-evidence-windows-2025` artifacts. Normal build outputs alone are packaged for release.
