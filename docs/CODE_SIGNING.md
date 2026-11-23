# Code signing (removes "Unknown publisher")

Windows Defender SmartScreen shows **"Windows protected your PC … Unknown publisher"** for any downloaded program that is not digitally signed. The build is ready to sign every published exe; it only needs the owner's code-signing certificate. Until the four secrets below exist, the build skips signing and works exactly as before.

**Only releases from `master` are signed.** Pull-request betas stay unsigned: the secrets live in a protected `code-signing` environment that only `master` can use, so code that has not been reviewed and merged can never read them or get itself signed.

## What gets signed

| SrNo. | File | How |
|---|---|---|
| 1 | Portable exe (`portable/SPI_Data_Comp_Tool_x.y.z.exe`) | The tested exe from the build is signed |
| 2 | 64-bit app exe that the installer copies | Rebuilt from the same commit (deterministic build) and signed, so the installed app is signed too |
| 3 | 64-bit installer (`SPI_Data_Comp_Tool_x.y.z_x64_setup.exe`) | Rebuilt around the signed exe, signed, then installed once to check the installed app is signed |

This happens in the `sign` job, which runs only for a push to `master`, after the normal build and its tests and before the release is published. Every signature is time-stamped, so files stay trusted after the certificate expires. `tools/sign_windows.ps1` checks each file with `Get-AuthenticodeSignature` and fails the build if a signature is missing or invalid. The release checksums are computed from the signed files, so the in-app updater keeps verifying them.

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
| 2 | SmartScreen | Microsoft builds trust from download history. A newly signed app can still show the blue screen for its first downloads; it stops once the certificate has gained reputation. EV certificates no longer skip this |
| 3 | Updates from inside the app | Unaffected; files are checked by SHA-256 as before |

Until then, on the blue screen choose **More info › Run anyway**, or right-click the downloaded exe › **Properties** › tick **Unblock**.

## Notes

- CodeSignTool v1.3.0 is downloaded from SSL.com's GitHub releases and must match the SHA-256 pinned in `tools/sign_windows.ps1`.
- Secrets are masked in the build log and are never printed by the script. They reach only the signing steps, as step-level environment variables.
- Step 4 matters: without the branch rule, a pull request that edits the workflow could ask for the `code-signing` environment.
- The Inno Setup uninstaller (`unins000.exe`) is not signed. It is created on the computer by the installer and never downloaded, so SmartScreen does not check it.
