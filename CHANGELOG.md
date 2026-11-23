# Changelog

## Next version (not released yet)

### Fixed

- Added optional public-trust Authenticode signing and verification for future Windows portable, installed application, setup and uninstall executables. Enabled signing fails closed; unsigned builds are explicitly identified. A real certificate and protected CI provider/runner setup are still required. Signing does not guarantee SmartScreen reputation or suppress every warning. <!-- closes: #113 -->

### Changed

- Validate the exact signing handoff and final updater-compatible package, run Windows rejection tests, and compute download hashes after signing. Existing releases remain unchanged.
