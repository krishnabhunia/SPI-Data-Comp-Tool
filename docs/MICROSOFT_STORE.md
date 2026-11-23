# Microsoft Store package (MSIX)

Windows SmartScreen shows **"Windows protected your PC — Unknown publisher"** for any downloaded exe
that has no code-signing certificate. No code change can remove that warning. The free way to remove
it is the Microsoft Store: Partner Center re-signs every submitted package with a Microsoft
certificate, so a Store install opens without the warning.

Every CI build now also produces `SPI_Data_Comp_Tool_<version>_x64.msix`, the Store package of the
64-bit app. The portable zip and the `_x64_setup.exe` installer are unchanged and still unsigned.

## What CI does

| SrNo. | Step | Check |
|---|---|---|
| 1 | `tools/build_msix.ps1` packs the 64-bit build with `windows/msix/AppxManifest.xml` and the logos in `windows/msix/Assets` | `makeappx` validates the manifest against the MSIX schema; the package is unpacked again and its files and version are checked |
| 2 | A second copy is signed with a throwaway certificate that exists only on the runner | Windows installs only signed packages; the Store signs the real one itself |
| 3 | The test copy is installed with `Add-AppxPackage` | Installed version, app execution alias `spi-data-comp-tool.exe` |
| 4 | The installed app runs `--check-package` and `--check-assemblies` | It has its package identity, its own updater refuses to install updates, every library loads |
| 5 | The test copy and certificate are removed | Clean uninstall |
| 6 | The release page gets the `.msix` and its `.msix.sha256` | The `.msix` is never put inside the updater's zip |

## The Store copy never updates itself

Store packages are installed read-only and updated by the Store. When the app runs from a package
(`PackagedApp.IsPackaged`), **Settings › Updates** can still show new versions, but **Install** stays
unavailable with the reason *"This copy was installed from the Microsoft Store. The Store installs its
updates for you."* The portable and installer copies keep the in-app updater.

Each Store update installs into a new versioned folder, so a Store copy keeps its saved preferences in
a folder named after the package family (the same for every version) instead of the install folder.
The package needs Windows 10 version 1903 or later, the first version with .NET Framework 4.8 built in.

## One-time setup (Krishna)

| SrNo. | Where | What to do |
|---|---|---|
| 1 | [partner.microsoft.com](https://partner.microsoft.com/dashboard/registration) | Create a free **individual** developer account (identity check with an ID and a selfie) |
| 2 | Partner Center › Apps and games › New product › MSIX or PWA app | Reserve the name, for example *SPI Data Comparison Tool* |
| 3 | Product › Product management › **Product identity** | Copy *Package/Identity/Name*, *Package/Identity/Publisher* and *Package/Properties/PublisherDisplayName* |
| 4 | GitHub › repository › Settings › Secrets and variables › Actions › **Variables** | Add `MSIX_IDENTITY_NAME`, `MSIX_PUBLISHER`, `MSIX_PUBLISHER_DISPLAY_NAME` with those three values (they are not secrets) |
| 5 | GitHub › Actions | Merge or re-run master: the next release carries a `.msix` with the real identity |

Until step 4 is done the package builds with placeholder identity values, and the build log says so.
Partner Center rejects a package whose identity does not match the reserved product.

## Each release

| SrNo. | Step |
|---|---|
| 1 | Download `SPI_Data_Comp_Tool_<version>_x64.msix` from the GitHub release (a normal release, not a beta) |
| 2 | Partner Center › the product › **Start update** (first time: Start submission) › Packages › upload the `.msix` |
| 3 | Fill pricing (Free), properties, age rating and the Store listing on the first submission |
| 4 | The app uses the restricted capability `runFullTrust` (a normal desktop app); in *Submission options* explain: "Windows Forms desktop app that compares databases and Excel files" |
| 5 | Submit. Certification usually takes a few days; then users install from the Store with no warning |

## Limits

| SrNo. | Limit |
|---|---|
| 1 | The package version is `x.y.z.0`; a beta packs with its plain number, so submit only normal releases |
| 2 | Each Store submission must have a higher version than the last one |
| 3 | Downloads from GitHub (portable zip, installer) still show SmartScreen until they are code-signed; once the signing secrets are set, master releases are signed (see `docs/CODE_SIGNING.md`) |
