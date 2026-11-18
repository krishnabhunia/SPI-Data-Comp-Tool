# Updating SPI from the application

## First upgrade

Older versions do not have an update page. Install the first updater-enabled release manually from this repository's **Releases** page once. After that, open **File > User Settings > Updates**, or **Help > Check for updates**. Both routes use the same Settings window, alongside General and Comparison rules; no extra update form is opened.

## Choose the version yourself

1. Select **Normal / stable**, **Beta**, or **All releases**. Normal is the default; the filter is remembered per installation.
2. Select **Check for updates**. The application does not check on startup or merely when you open Settings.
3. Select an exact version. The list shows its channel, publication date, current/latest labels and release notes. No version is selected automatically.
4. Select **Download selected**. You can cancel the operation. A complete, matching SHA-256 checksum is required before installation becomes available.
5. Save your work and close other SPI tool windows and other copies of SPI. Select **Install & restart**, read the selected-version confirmation and approve it. Downgrades and reinstalling the current version are explicitly identified.

Only published releases with compatible assets from `krishnabhunia/SPI-Data-Comp-Tool` are shown. Drafts, manual builds, missing-asset releases and unrecognised/four-part version tags are excluded. `2.6.0` ranks above `2.6.0-beta002`, which ranks above `2.6.0-beta001`; version comparison is numeric, not alphabetical. The current application version comes from build metadata, not the executable filename.

Historical betas remain available for deliberate selection, but a high version number alone does not make an old beta current. **Latest beta** must be newer by version and publication time than the newest compatible normal release. Betas that are not demonstrably newer than that normal release are marked **Earlier beta** and carry an additional warning in their notes and installation confirmation. This prevents an old experimental major-version beta from being presented as the newest recommended beta after a later normal release.

## Installed and portable copies

| Running copy | Selected download | What happens |
|---|---|---|
| Registered 64-bit installation | The selected version's `_x64_setup.exe` | The existing installer updates the same directory with the same per-user/all-users scope. Normal Windows User Account Control and SmartScreen checks still apply. The application restarts only after setup succeeds and the executable version is verified. |
| Portable copy | The selected version's `.zip` | Only the application's executable, matching configuration and known PNID templates are staged and replaced. The current executable filename is retained, even if it contains an older version number. Unknown/user files are not deleted. A portable copy is not converted to an installed copy. |

Install detection uses the exact Inno Setup application identifier and installation directory, not just a folder name or the presence of an uninstaller. An installed update requires 64-bit Windows; the existing portable application is Any CPU. If a portable folder is not writable, move/extract the portable application to a user-writable location and update there; the updater does not circumvent filesystem permissions.

## Preferences and data

The updater preserves only these application preferences:

- The connection-profile file **path**, `ProfilePathUser`.
- The comparison-rules file **path**, `RuleFilePath`.
- The two Excel-export options, `EPPlus_Excel_Save` and `XL_Excel_Save`.
- The Normal/Beta/All update filter.

It does not back up connection-profile contents, database passwords, workbooks, unsaved results or database contents. Back up those separately as appropriate. Preferences use an atomic, per-installation cache under `%LocalAppData%\SPI Data Comp Tool\Updates`. A bounded settings bridge copies the four known application preferences to the selected version's `user.config` location, including when returning to older versions that predate the updater. Unknown settings are not imported from a backup, and existing unrelated settings are preserved. Invalid settings paths or malformed configuration stop the update rather than silently resetting preferences.

The update helper waits for the exact parent process to exit; it does not kill it. Other open tool windows block handoff. Save/close other SPI processes too: portable file locks fail safely, and setup is told not to force-close other applications.

## Verification and limitations

- Release metadata and downloads use HTTPS with a repository-specific source and a restricted GitHub asset redirect list. There is no GitHub token prompt and no private-repository authentication flow.
- Metadata, checksums, downloads and archive expansion have size/time limits. A check reads up to 1,000 published-release records; reaching that limit is reported explicitly rather than showing an incomplete list as complete.
- Downloads are streamed to a unique cache directory and checked against the release's SHA-256 manifest. Installation rechecks the package before using it. Cancelled/incomplete downloads are removed; a feed instance cleans up only its own download directories when Settings closes.
- ZIP entries are validated before applying files: traversal, unexpected files, duplicate destinations, alternate data streams, links and excessive sizes are rejected. The ZIP's known installer sibling is validated but never applied by the portable route.
- The installer is currently **unsigned**. SHA-256 detects corruption or a mismatch with the published manifest; it is **not** publisher/code-signing authentication. Never bypass an unexpected Windows warning automatically. A compromised repository or release account remains outside this integrity guarantee.
- The target framework remains .NET Framework 4.6.2. Runtime configuration opts into strong cryptography and operating-system-selected TLS, without changing machine-wide registry settings or disabling certificate verification.
- Downgrading to a version without this feature removes the update page. A later upgrade from that older version requires manual installation again.

## If installation does not finish

No failed download is offered for installation. If handoff cannot start, SPI remains open and shows the error. Once handoff succeeds, SPI closes and the isolated helper performs the update.

The helper retains its task-owned staging directory on failure. It contains `update.log`, `result.txt`, preference-only snapshots and, for portable changes, a `backup` directory; setup attempts also use `setup.log`. The failure message identifies that location. Do not delete it while investigating. Portable copy failures attempt to restore only the application files changed in that attempt; if restoration is blocked, the backup remains available for manual recovery. Installed-copy failures are not presented as an automatic rollback: do not assume a cancelled/failed installer left every file unchanged. Inspect the log and, if needed, manually reinstall the desired published version into the same directory.

The helper does not force a reboot or restart an uncertain installation after an error. Successful verified installation restarts SPI normally. User Account Control, SmartScreen, antivirus interference, old Windows/runtime combinations and real-user upgrade/downgrade acceptance still need manual verification; automated tests use synthetic files and fake release services, not production data.
