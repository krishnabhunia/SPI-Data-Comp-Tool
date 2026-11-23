# SPI Authenticode inspection — 23 November 2026

Request key: `spi-authenticode-20261123-8f916eda-39ba-47ad-8a9e-2a52c758ec19`; Q1; [issue #113](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/113).

## Findings

- Default branch is `master`; inspected current head `6b5c397a8be9f61517559aaf3ae331c3cbbdd606`. The working branch is `chat-gpt-spi-authenticode-signing`.
- [PR #112](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/pull/112) is merged at that commit. Its changes consolidate Export navigation and rename Merge logs; its diff does not change signing, workflow or installer configuration. This request does not modify that PR or implement the separate pending navigation redesign/removal.
- [.github/workflows/build.yml](../../.github/workflows/build.yml) builds Any CPU portable and x64 installed applications, uses Costura to embed assemblies, builds an Inno installer, tests installation and publishes beta/stable assets. It has no Authenticode signing or verification step. Hashes are currently generated for the release ZIP and standalone setup.
- [The installer](../../windows/installer/SPI_Data_Comp_Tool.iss) has no `SignTool` or `SignedUninstaller` setting. The project's ClickOnce `PublisherName` and `SignManifests=false` do not establish Authenticode identity for either distributed EXE. Strong-name and Git commit signatures would not solve this warning either.
- GitHub API metadata reports **zero repository Actions secrets, zero Actions variables and zero environments**. Windows `CurrentUser/My` and `LocalMachine/My` contain **zero code-signing certificates**. No secret value was requested; no private key was opened. No usable signing configuration was established.
- No tracked AGENTS.md, CLAUDE.md or roadmap was found in this revision. All 112 existing open/closed issue and PR records were paginated before Q1 allocation; no Q-numbered entries or competing Authenticode request existed. Issue #113 reserves Q1 for this task.
- The current versions of `D:/Projects/Skills/Github Skills/software app.md`, `versions_management.md` and `workflows.md` could not be read because the paths do not exist. The user-provided policy snapshot remains the available policy source.

## Published binary inspection

Downloaded [release v4.4.0](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/releases/tag/v4.4.0) ZIP without executing its contents. Its SHA-256 matches GitHub's asset digest:

`a55c0d2765369d3288e59fd08963f6eabd3675be56b56eb778f281f41abd1a33`

| File inside ZIP | SHA-256 | Windows Get-AuthenticodeSignature |
|---|---|---|
| `portable/SPI_Data_Comp_Tool_4.4.0.exe` | `191ed279fb685ebb1f835cdb3ec0b32ad989f7ac1e5e138a2210478f7fd9261c` | NotSigned; no signer certificate |
| `windows-x64/SPI_Data_Comp_Tool_4.4.0.exe` (installer) | `4860bccf3cceb9c8ae9fbbb228560a35b8be40af0f3274d1ab87fd9472b3c0ae` | NotSigned; no signer certificate |

The installer bytes also match the standalone `_x64_setup.exe` asset's recorded digest. The ZIP has portable/ and windows-x64/ directly inside it, including the existing compatibility anchors and portable configuration. No modification to downloaded artifacts, trust settings, file-zone data or security policy was made.

## Existing CI evidence

PR #112 head: `f99a9701628dfcf8a836d69a146ee5ebd810fa12`.

| Run | Event / SHA | Observed result |
|---|---|---|
| [Build and checks 45014249117](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/actions/runs/45014249117) | pull_request / PR head | Completed, success |
| [Windows data-integrity checks 45014249071](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/actions/runs/45014249071) | pull_request / PR head | Completed, success |
| [Framework compatibility 45014249073](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/actions/runs/45014249073) | pull_request / PR head | Completed, success |
| [Build and checks 45016856117](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/actions/runs/45016856117) | push / current master | All five jobs passed: version, windows-build, secrets-guard, documentation, Publish 4.4.0 |
| [Framework compatibility 45016856109](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/actions/runs/45016856109) | push / current master | Completed, success |

The master Windows job includes successful UI/startup/database/Excel/updater/assembly checks and installer install/check/uninstall. These are historical CI observations, not tests run by this task and not proof of signatures. No current signing test exists.

## Design and external setup

Review [r1](spi-authenticode-r1.html). The proposed patch is 4.4.1, subject to existing automatic version planning. Signing is optional until a validated public-trust certificate and secure CI provider are configured. Enabled signing fails closed. PR builds remain explicitly unsigned and receive no credentials.

The owner needs to acquire and validate the publisher identity with a public-trust CA, configure a nonexportable hardware/cloud key plus Windows provider, provision a protected release-only environment/ephemeral signing runner, configure least-privilege provider authentication, pin the expected signing certificate and timestamp endpoint, and then enable signing. The optional pipeline does not itself acquire a certificate or establish reputation.

Inno must sign setup, temporary copies and uninstall during compilation, after the x64 payload has been signed. Signing the outer installer alone does not cover all of these files.

## Not verified

- Real public-trust signing, timestamp acceptance, certificate revocation/trust validation and provider authentication: certificate and CI configuration are unavailable.
- The installed x64 payload and uninstaller in the existing 4.4.0 installer were not extracted or executed. Their expected unsigned state follows from source configuration, not binary verification.
- On-device SmartScreen behavior and reputation: no claim of warning suppression.
- New-change local app tests, builds, independent review and CI: implementation awaits design approval. No implementation PR or new beta exists.
- Browser visual preview: unavailable because the browser security policy rejected local-file navigation. No alternate route was attempted. The HTML was generated and its content inspected, but live rendering is not claimed.

## Primary references

- [Microsoft SmartScreen reputation](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/smartscreen-reputation): a valid signature displays the publisher; new signed binaries may still warn. EV no longer grants automatic reputation.
- [Microsoft code-signing options](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/code-signing-options): public-trust certificates, managed signing and distribution choices.
- [SignTool](https://learn.microsoft.com/en-us/windows/win32/seccrypto/signtool): SHA-256 file/timestamp digests; Authenticode verification with `/pa`, `/all`, `/v`, `/tw`; treat nonzero results/warnings as failures.
- [Inno SignTool](https://jrsoftware.org/inno-setup/documentation/topic_setup_signtool.htm) and [SignedUninstaller](https://jrsoftware.org/inno-setup/documentation/topic_setup_signeduninstaller.htm): compilation-time signing also covers uninstall and temporary setup copies.
- [GitHub secure use](https://docs.github.com/en/actions/reference/security/secure-use): isolate untrusted code, protect secrets and minimize permissions.
- [Microsoft Artifact Signing setup](https://learn.microsoft.com/en-us/azure/artifact-signing/quickstart): external identity validation, signing account and certificate profile; public-trust individual enrollment currently supports US/Canada. Eligibility for this user has not been established.
