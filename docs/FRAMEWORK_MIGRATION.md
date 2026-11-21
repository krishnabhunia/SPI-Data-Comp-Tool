# .NET Framework migration plan

Reviewed 22 November 2026. Tracking: [issue #7](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/7).

## Lifecycle and current release

The released application still targets .NET Framework 4.6.2. Microsoft lists **12 January 2027** as that version's end of support. .NET Framework 4.8 and 4.8.1 follow the lifecycle of the supported Windows system on which they run. A newer targeting pack on a build machine does not by itself retarget the application or prove compatibility on customers' machines.

Primary references:

- [Microsoft .NET Framework lifecycle](https://learn.microsoft.com/lifecycle/products/microsoft-net-framework)
- [.NET Framework support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-framework)
- [Framework versions included with Windows](https://learn.microsoft.com/en-us/dotnet/framework/install/on-server-2019)
- [GitHub Windows 2022 runner software](https://github.com/actions/runner-images/blob/main/images/windows/Windows2022-Readme.md)

## Migration candidate and evidence

The `Framework migration compatibility` workflow builds the current code against **.NET Framework 4.8** in both Any CPU and x64 configurations, with warnings treated as errors. It changes only its isolated output configuration to request 4.8 runtime behavior, then runs dependency loading, real-control UI/profile/comparison checks, startup/navigation, Excel audit, themes and controlled steps.

This is an executable migration experiment. It does not change the shipped project target, installer requirement, update feed or published packages. The existing packages and embedding configuration are exercised together, including Oracle.ManagedDataAccess, EPPlus, ClosedXML, Open XML and Costura. A green run establishes those checks on that Windows runner; it does not prove an Oracle connection, Microsoft Excel automation, every supported Windows version or a signed installer.

The first candidate is 4.8 to keep the migration compatible with the broader set of Windows versions that can install it. Before adoption, the owner must identify the Windows versions actually supported by the product. If all supported machines can run 4.8.1, that later target can be validated with the same procedure. Supporting a framework on an unsupported Windows version does not restore Windows support.

## Adoption checklist

| Step | Required evidence | Current boundary |
| --- | --- | --- |
| Confirm supported Windows versions | List of supported Windows editions/builds and whether each can install the selected framework | Product decision; not inferred from the developer's machine |
| Validate the candidate build | Successful exact-commit `framework-48` check for Any CPU and x64, including embedded dependencies and executable regression groups | Automated in this repository |
| Check provider compatibility | Disposable SQL Server and Oracle comparisons, distinct Source/Target connections, authentication failures, nulls/duplicates and Excel results | SQL Server has an isolated LocalDB check; Oracle acceptance still requires a disposable Oracle environment |
| Validate spreadsheet paths | Existing engine/report checks plus supported Microsoft Excel automation on a Windows machine with Excel installed | Synthetic workbook checks run automatically; Excel automation requires separate acceptance |
| Retarget release coherently | Update the project target, application configuration, package target metadata, installer runtime check, documentation and updater prerequisite handling in one PR | No release target change in this audit |
| Validate upgrades and rollback | Old portable/installed version upgrades to the candidate; missing-runtime behavior fails with an actionable message; rollback restores files and preferences | Existing updater tests remain; cross-runtime installed acceptance is still required |
| Decide release signing | Approved code-signing certificate/service and workflow, if signed releases are required | Current releases remain unsigned; no signing identity is invented |
| Adopt before support ends | Reviewed beta on supported machines, then normal stable release before 12 January 2027 | No automatic merge or release-target change |

## Running the experiment

On a disposable Windows build machine with Visual Studio MSBuild, the .NET Framework 4.8 targeting pack and NuGet:

```powershell
nuget restore "SPI Data Comp Tool.sln" -NonInteractive
$env:RUNNER_TEMP = [System.IO.Path]::GetTempPath()
./tools/check_framework_compatibility.ps1
```

Outputs go under `bin/Compatibility48` and `obj/Compatibility48`. They are not release packages. The script exits unsuccessfully if a build or executable check fails; a missing or failed check is not recorded as acceptance.
