# SPI change queue

Queue numbers are permanent request identifiers, separate from issue numbers. Retain delivered and rejected rows for their audit history.

| Queue | Request | State | Issue | Design | PR / Version |
|---|---|---|---|---|---|
| Q1 | Legitimate optional Windows Authenticode signing <!-- ghpr-request: spi-authenticode-20261123-8f916eda-39ba-47ad-8a9e-2a52c758ec19 --> | Design prepared 23 November 2026 — awaiting approval | [#113](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/113) | [r1](designs/spi-authenticode-r1.html) | Not opened / proposed 4.4.1 (CI-managed) |

## Q1 scope

Add optional public-trust signing for the portable EXE, x64 application, Inno installer, temporary setup copies and uninstaller. Sign only trusted protected-master release builds, isolate credentials from PRs, fail publication when enabled signing fails, preserve updater-compatible packaging and document external certificate/provider/CI setup. Signing cannot guarantee SmartScreen reputation. No merge or manual release is authorized.

See the [inspection record](designs/spi-authenticode-inspection.md) for current binary and CI evidence. Implementation and new-change CI await approval of the HTML revision.
