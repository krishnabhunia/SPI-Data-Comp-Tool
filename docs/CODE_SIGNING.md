# SSL.com cloud signing (application and installer)

Windows Defender SmartScreen can show **"Windows protected your PC … Unknown publisher"** for an unsigned downloaded program. This backend is ready to sign the portable app, installed app and outer installer once the owner's validated certificate and four secrets below are configured. Until then, output remains unsigned. A trusted signature establishes publisher identity; it does not guarantee SmartScreen reputation.

This is the SSL.com backend inherited from PR #115. It runs when `WINDOWS_SIGNING_MODE` is missing or `off`. Selecting `store` uses the separate [isolated certificate-store backend](WINDOWS_SIGNING.md) and skips SSL.com, even if its secrets exist. Store failures never fall back to cloud signing. The store backend additionally covers uninstall and temporary setup executables; release notes explicitly distinguish the two coverage levels.

**Only releases from `master` are signed.** Pull-request betas stay unsigned: the secrets live in a protected `code-signing` environment that only `master` can use, so code that has not been reviewed and merged can never read them or get itself signed.

## What gets signed

| SrNo. | File | How |
|---|---|---|
| 1 | Portable exe (`portable/SPI_Data_Comp_Tool_x.y.z.exe`) | The tested exe from the build is signed |
| 2 | 64-bit app exe that the installer copies | Rebuilt from the same commit (deterministic build) and signed, so the installed app is signed too |
| 3 | 64-bit installer (`windows-x64/SPI_Data_Comp_Tool_x.y.z.exe` in the release zip) | Rebuilt around the signed exe, signed, then installed once to check the installed app is signed |
| 4 | Per-user MSI (`windows-msi/SPI_Data_Comp_Tool_x.y.z_x64.msi`) | Rebuilt around the signed exe, signed, then installed once to check the MSI and the installed app are signed |

This happens in the `sign` job, which runs only for a push to `master`, after the normal build, source and documentation checks and before publication. Each signature must have a timestamp; Windows trust and revocation policy still apply. `tools/sign_windows.ps1` checks each file with `Get-AuthenticodeSignature` and fails the build if a signature is missing or invalid. The release zip is built from the resulting signed files; GitHub's digest of that zip is what the in-app updater verifies.

## One-time set-up (owner)

Since June 2023 a code-signing key can no longer be exported to a `.pfx` file; it must live on a USB token or in a cloud signing service. GitHub's build machines cannot use a USB token, so the build signs through **SSL.com eSigner** (cloud signing).

| SrNo. | Step | Where |
|---|---|---|
| 1 | Buy an **IV (individual)** or **OV (organisation)** code-signing certificate from SSL.com and complete the identity check | ssl.com |
| 2 | Enrol the certificate in **eSigner** (cloud signing) and choose automated signing; note the **credential ID** | SSL.com account |
| 3 | Copy the **TOTP secret** shown when automated signing is enabled (the secret behind the one-time codes, not a single code) | SSL.com account |
| 4 | Create an environment named **`code-signing`**. Under *Deployment branches and tags* choose **Selected branches and tags** and add only **`master`** | GitHub › repository › Settings › Environments |
| 5 | In that environment add four **environment secrets** (not repository secrets): `SIGN_ES_USERNAME` (SSL.com user name), `SIGN_ES_PASSWORD` (SSL.com password), `SIGN_ES_CREDENTIAL_ID`, `SIGN_ES_TOTP_SECRET` | GitHub › Settings › Environments › code-signing |
| 6 | Merge a PR to `master`. The **Sign** job's log shows `Signed … by CN=…` for each file, and the release has signed files | GitHub Actions |

If only some of the four secrets exist, the build fails with the names of the missing ones, so a half-finished set-up is never published as if it were signed.

## What to expect afterwards

| SrNo. | Effect | Detail |
|---|---|---|
| 1 | Publisher name | "Unknown publisher" becomes the name on the certificate straight away |
| 2 | SmartScreen | Microsoft evaluates file and publisher reputation. A newly signed app can still show the warning, and reputation does not guarantee a prompt-free installation. EV certificates no longer receive an automatic exemption |
| 3 | Updates from inside the app | Unaffected; files are checked by SHA-256 as before |

Until a backend is configured and a newly signed build is produced, existing unsigned downloads can continue to show Windows warnings. Keep normal SmartScreen, download-zone and certificate-trust protections enabled.

## Notes

- CodeSignTool v1.3.0 is downloaded from SSL.com's GitHub releases and must match the SHA-256 pinned in `tools/sign_windows.ps1`.
- Secrets are masked in the build log and are never printed by the script. They reach only the signing steps, as step-level environment variables.
- Step 4 matters: without the branch rule, a pull request that edits the workflow could ask for the `code-signing` environment.
- This SSL.com backend does not sign the Inno uninstaller or temporary setup copies. It reports application/installer coverage rather than complete store coverage. The [certificate-store backend](WINDOWS_SIGNING.md) signs these executables during Inno compilation and verifies the installed uninstaller.
