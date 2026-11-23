# Code signing (removes "Unknown publisher")

Windows Defender SmartScreen shows **"Windows protected your PC … Unknown publisher"** for any downloaded program that is not digitally signed. The build is ready to sign every published exe; it only needs the owner's code-signing certificate. Until the four secrets below exist, the build skips signing and works exactly as before.

## What gets signed

| SrNo. | File | When |
|---|---|---|
| 1 | Portable exe (`portable/SPI_Data_Comp_Tool_x.y.z.exe`) | Before it is tested and packaged |
| 2 | 64-bit app exe that the installer copies | Before the installer is built, so the installed app is signed too |
| 3 | 64-bit installer (`SPI_Data_Comp_Tool_x.y.z_x64_setup.exe`) | Before it is tested, uploaded and hashed |

Beta (pull request) and normal (master) builds are both signed. Every signature is time-stamped, so files stay trusted after the certificate expires. `tools/sign_windows.ps1` checks each file with `Get-AuthenticodeSignature` and fails the build if a signature is missing or invalid. The release checksums are computed from the signed files, so the in-app updater keeps verifying them.

## One-time set-up (owner)

Since June 2023 a code-signing key can no longer be exported to a `.pfx` file; it must live on a USB token or in a cloud signing service. GitHub's build machines cannot use a USB token, so the build signs through **SSL.com eSigner** (cloud signing).

| SrNo. | Step | Where |
|---|---|---|
| 1 | Buy an **IV (individual)** or **OV (organisation)** code-signing certificate from SSL.com and complete the identity check | ssl.com |
| 2 | Enrol the certificate in **eSigner** (cloud signing) and choose automated signing; note the **credential ID** | SSL.com account |
| 3 | Copy the **TOTP secret** shown when automated signing is enabled (the secret behind the one-time codes, not a single code) | SSL.com account |
| 4 | Add four **repository secrets**: `SIGN_ES_USERNAME` (SSL.com user name), `SIGN_ES_PASSWORD` (SSL.com password), `SIGN_ES_CREDENTIAL_ID`, `SIGN_ES_TOTP_SECRET` | GitHub › repository › Settings › Secrets and variables › Actions |
| 5 | Run any build (open a PR or merge to master). The log shows `Signed … by CN=…` for each file | GitHub Actions |

If only some of the four secrets exist, the build fails with the names of the missing ones, so a half-finished set-up is never published as if it were signed.

## What to expect afterwards

| SrNo. | Effect | Detail |
|---|---|---|
| 1 | Publisher name | "Unknown publisher" becomes the name on the certificate straight away |
| 2 | SmartScreen | Microsoft builds trust from download history. A newly signed app can still show the blue screen for its first downloads; it stops once the certificate has gained reputation. EV certificates no longer skip this |
| 3 | Updates from inside the app | Unaffected; files are checked by SHA-256 as before |

Until then, on the blue screen choose **More info › Run anyway**, or right-click the downloaded exe › **Properties** › tick **Unblock**.

## Notes

- CodeSignTool v1.3.0 is downloaded from SSL.com's GitHub releases and must match the SHA-256 pinned in `tools/sign_windows.ps1`.
- Secrets are masked in the build log and are never printed by the script.
- The Inno Setup uninstaller (`unins000.exe`) is not signed. It is created on the computer by the installer and never downloaded, so SmartScreen does not check it.
