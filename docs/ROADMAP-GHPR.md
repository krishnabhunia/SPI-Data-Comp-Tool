# SPI change queue

Queue numbers are permanent request identifiers, separate from issue numbers. Retain delivered and rejected rows for their audit history.

| Queue | Request | State | Issue | Design | PR / Version |
|---|---|---|---|---|---|
| Q1 | Legitimate optional Windows Authenticode signing <!-- ghpr-request: spi-authenticode-20261123-8f916eda-39ba-47ad-8a9e-2a52c758ec19 --> | Approved 23 November 2026 — built in PR #116; conflict resolution against PR #115 awaiting updated checks | [#113](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/113) | [r1](designs/spi-authenticode-r1.html) | [PR #116](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/pull/116) / CI-managed version |

## Q1 scope

Add optional public-trust signing for the portable EXE, x64 application, Inno installer, temporary setup copies and uninstaller. Sign only trusted protected-master release builds, isolate credentials from PRs, fail publication when enabled signing fails, preserve updater-compatible packaging and document external certificate/provider/CI setup. Signing cannot guarantee SmartScreen reputation. No merge or manual release is authorized.

See the [inspection record](designs/spi-authenticode-inspection.md) for current binary and CI evidence. Design r1 was explicitly approved in the chat on 23 November 2026. Implementation is in PR #116; current-head CI and independent review are required before ready-for-review. Real signing and SmartScreen behavior remain unverified until external setup is complete.
