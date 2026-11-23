# Windows publisher signing

The published 4.4.0 portable EXE and installer are unsigned. These optional signing backends do not acquire a certificate or configure a signing service. When neither backend is configured, releases remain **unsigned**, and their notes and version comment say so. Configure the chosen backend using its guide below. SHA-256 checksums check download integrity; they do not establish a publisher.

A valid public-trust Authenticode signature displays the validated certificate identity. Microsoft still evaluates SmartScreen reputation independently, and signed files can still warn. EV certificates no longer receive automatic SmartScreen reputation. Do not disable Defender/SmartScreen, remove download-zone information, install a test root or claim a warning-free release. See [Microsoft's explanation](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/smartscreen-reputation).

## What the workflow does

### Backend selection after integrating PR #115

| Configuration | Selected backend and reported coverage |
|---|---|
| `WINDOWS_SIGNING_MODE=store` | The isolated `windows-sign` job uses the `windows-code-signing` environment. It signs and verifies the portable app, x64 payload, setup, temporary setup copies and uninstall. The SSL.com job is skipped. |
| Mode missing or `off` | The inherited master-only SSL.com `sign` job uses the separate `code-signing` environment. With all four SSL.com secrets configured, it signs the portable app, installed app and outer installer. Its uninstaller/temporary setup copies are not covered. With no secrets, output remains unsigned; partial configuration fails. See [SSL.com setup](CODE_SIGNING.md). |
| Any pull request | Both production signing jobs are skipped. No signing environment or signing credentials are used; the beta is explicitly unsigned. |

`off` disables the certificate-store backend; it preserves the independently configured SSL.com path merged in PR #115. The two jobs never process the same release. A selected store signing failure blocks publication and never falls back to SSL.com. Release notes use the successful backend's actual result: unsigned, app/setup signing, or complete store verification.

`Build and checks` is the existing Windows build/release workflow. PRs, including same-repository PRs, use GitHub-hosted Windows runners and remain unsigned even if signing is enabled for releases. They never select the signing environment or signing runner. Fork PR publication remains excluded by the existing release rule.

With no signing backend configured, the existing build, regression, install/uninstall and unsigned publication paths continue. With `store` selected, that backend is eligible **only for a push to protected master**. A manual run or another ref is rejected, rather than silently publishing unsigned output. Invalid modes and incomplete enabled store configuration fail closed. Both production signing jobs wait for the build and source/documentation checks.

The build stages only an exact, allowlisted portable payload, x64 EXE/config and notice, with a SHA-256 manifest binding its version, source commit, workflow run and attempt. The signing job downloads the named input from the same run, checks that manifest and rejects extra files, links, altered bytes and other provenance. The manifest is a same-run integrity check, not a signature or a substitute for admitting only trusted builds.

Signing uses the exact certificate selected from Windows `My`, `/fd SHA256`, RFC 3161 `/tr`, and `/td SHA256`. The SHA-1 thumbprint identifies the certificate; it does **not** choose the file digest. The script checks code-signing EKU, validity, Windows trust with online revocation, private-key availability and the expected signer. Each resulting EXE must pass `Get-AuthenticodeSignature`, have a timestamp and pass `signtool verify /pa /all /v /tw` with exit code zero. Warnings are failures.

The final portable EXE is signed after version stamping and Costura weaving. The x64 EXE is signed before packing. Inno's `SignTool` callback and `SignedUninstaller=yes` sign setup, temporary setup self-copies and uninstall. Compilation fails without the callback. The signed installer is then installed into a disposable location; its application bytes, application/uninstaller signatures, dependency loading, UI/startup checks and uninstall are checked before uploading the final artifact. No signed bytes are stamped or woven again.

The internal handoff contains no key or credentials, has one-day retention and is deleted by a separate job with narrowly scoped Actions write permission. Publication of an enabled signing release requires both signing and cleanup to succeed. On a completed successful build, exactly one final `SPI_Data_Comp_Tool_<version>.zip` is downloadable in this workflow's Artifacts group. Platform contents are uploaded directly, so its first extraction reveals `portable/` and `windows-x64/`, without a nested ZIP. Existing notices and updater compatibility anchors remain intact. A forced termination or infrastructure failure can leave a nonsecret temporary input until cleanup/retention; it cannot promote an unsigned release as signed.

The release ZIP and standalone setup checksums are calculated after the selected backend completes. Release notes and the PR version comment distinguish **unsigned**, SSL.com **application and installer signed**, and store **signed and verified**. Complete store coverage is claimed only after its verification and smoke checks. Old releases/tags are not converted to signed releases by this change. Other diagnostic workflows retain their existing evidence artifacts.

## Required external certificate acquisition

1. Select a CA trusted by Windows for public code signing and complete its actual publisher identity checks. The certificate's validated legal identity determines the publisher name; an app metadata string cannot substitute for it.
2. Obtain a code-signing certificate with a nonexportable hardware/HSM/cloud-protected private key and a Windows SignTool-compatible CSP/KSP. Confirm the CA's current hardware and automation requirements. A TLS certificate, strong-name key, Git signing key or self-signed certificate does not qualify.
3. Confirm the provider can authenticate noninteractively in an isolated CI job, supports SHA-256 and supplies an RFC 3161 timestamp endpoint. Provider contracts, identity validation, fees and access credentials are owner actions. This repository has no PFX import/export path and never stores or prints a private key.

Microsoft Artifact Signing is a legitimate managed alternative, but this revision implements the certificate-store backend, not an Azure adapter. [Check current eligibility](https://learn.microsoft.com/en-us/azure/artifact-signing/quickstart) before selecting it: public-trust individual enrollment currently supports US/Canada. No Azure account, profile or OIDC credentials have been established here. An eligible service integration would need its own reviewed adapter.

## Required secure CI configuration

Do these steps **before** setting the signing mode to `store`:

1. Protect `master` with required checks/review and restrict who can change the release workflow, signing scripts and public configuration. Create the GitHub environment `windows-code-signing`, restrict deployment to `master`, require an authorized reviewer and disallow self-review where supported. A protected-branch boolean alone does not establish the environment rules.
2. Provision a **fresh ephemeral Windows x64 runner per admitted release job**, labelled `spi-code-signing` in addition to `self-hosted`, `Windows`, `X64`. Destroy the VM and revoke its provider session after that job. Preinstall reviewed PowerShell 7, Python 3.10+, Git, Windows SDK x64 SignTool, Inno Setup 6.7.4 or newer in the 6.x series and the CA/provider's CSP/KSP. The signing stage does not restore packages or build application code.
3. Enforce admission **outside repository-controlled workflow code** before attaching any signing identity. A restricted organizational runner group/workflow policy can be used where available. For this personal-account repository, group restrictions may not be available: use a trusted provisioning broker that validates the GitHub job/run, repository, `push` event, protected `master`, workflow path and exact commit, admits only the `windows-sign` job after environment authorization, and rejects **all PR/manual/other workflow jobs**. Do not register an always-online runner with reusable signing access. **Labels, `if` expressions and an environment alone cannot protect a key from a modified PR workflow requesting that runner.** If this external isolation cannot be enforced, leave signing off.
4. The trusted broker/provider must create an authenticated, least-privilege, short-lived signing session only on that admitted runner and expose the certificate in `Cert:/CurrentUser/My` (or `LocalMachine/My`). Key material remains in the HSM/cloud provider. Limit the provider identity to signing with this certificate; restrict endpoints, rotate/revoke credentials and keep audit logs. No provider secrets are currently mapped by this provider-neutral workflow. Merely adding a GitHub secret does not initialize a vendor provider: provision the session out of band, or add a separately reviewed vendor-specific authentication adapter. Never place a long-lived credential in the runner image, repository, command line or handoff.
5. Ensure the signing image has normal Windows trust roots and can reach CA/revocation and timestamp services. Do not import a new root or disable revocation checking to make validation pass. `HasPrivateKey` cannot attest the provider's hardware/nonexportability; the owner must validate those properties and the isolation policy externally.
6. Set these **nonsecret repository Actions variables**. Keep them at repository scope so the policy job and signing job see the same values; do not shadow them with different environment values:

| Variable | Value |
|---|---|
| `WINDOWS_SIGNING_MODE` | Missing or `off` until setup is validated; then exactly `store` |
| `WINDOWS_SIGNING_THUMBPRINT` | Exact 40-hex thumbprint of the issued code-signing certificate; no spaces |
| `WINDOWS_SIGNING_STORE` | `CurrentUser` (default) or `LocalMachine`; always the `My` store |
| `WINDOWS_SIGNING_TIMESTAMP_URL` | Provider-approved public HTTP(S) RFC 3161 endpoint, without credentials/query/fragment/shell characters; RFC 3161 tokens are themselves signed |
| `WINDOWS_SIGNING_RUNNER_ISOLATED` | Exactly `true` **only after** the external admission/isolation controls above have been validated |

7. Have the owner review the first normal merge-triggered release's policy, source SHA/run linkage, timestamp/trust results, signer and file hashes. PR CI validates the off/rejection paths; it does not exercise your real signing identity. Keep signing disabled if the certificate or secure provider setup is incomplete. Do not trigger a publishing workflow simply to test configuration.

If enabled signing fails, publication stops. Repair missing provider sessions, expired/revoked certificates, unexpected identities, offline revocation/timestamp services or installer callback errors; never replace a failure with unsigned output while describing it as signed. For rotation, obtain and validate the replacement, update its public thumbprint deliberately, and revoke old sessions as appropriate. Do not promise continuity of SmartScreen reputation.

## Verification and remaining limits

CI runs Python eligibility/handoff/status tests and Windows negative signature tests. The latter create a temporary untrusted test certificate only in `CurrentUser/My`, never in Root/TrustedPublisher, reject it as a release identity, reject wrong/unsigned/altered files and delete the fixture key. A self-signed fixture is never distributed as a trusted release. The temporary-PE-name and missing-timestamp guards are also exercised; final distribution signatures are rechecked after the smoke checks.

**Not verified until external setup:** a real public-trust signing run, HSM/provider authentication and isolation, timestamp service acceptance, signed Inno temporary-copy/uninstaller behavior on that provider, certificate rotation, and actual SmartScreen reputation on user devices. The intended runtime acceptance is to download the eventual signed artifact, inspect its Digital Signatures and certificate identity, install/uninstall normally, and observe the OS result with normal protections enabled. Passing PR CI does not establish those results.

Primary documentation: [SignTool](https://learn.microsoft.com/en-us/windows/win32/seccrypto/signtool), [Inno SignTool](https://jrsoftware.org/inno-setup/documentation/topic_setup_signtool.htm), [Inno SignedUninstaller](https://jrsoftware.org/inno-setup/documentation/topic_setup_signeduninstaller.htm), [GitHub runner routing](https://docs.github.com/en/actions/how-tos/manage-runners/self-hosted-runners/use-in-a-workflow) and [GitHub secure use](https://docs.github.com/en/actions/reference/security/secure-use).
