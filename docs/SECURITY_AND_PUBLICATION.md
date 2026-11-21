# Security and publication decisions

This records the remaining owner actions for issues [#6](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/6), [#8](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/8) and [#10](https://github.com/krishnabhunia/SPI-Data-Comp-Tool/issues/10). A passing build does not establish licensing rights or revoke a database credential.

## Desktop access and development diagnostics

The app opens under the signed-in Windows account. The previously reverted application sign-in gate remains absent. Database access still uses the selected database authentication and permissions; opening a desktop window is not evidence of permission to query a database.

Release builds reject developer-mode activation from both an attached debugger and `SPI_DATA_COMP_DEBUG=1`. Only Debug builds can opt into developer diagnostics and shortcuts, and self-test runs disable them. Ordinary error messages remain available with shared credential masking. These controls do not protect against a person who can modify the executable or control the operating system.

| Remaining decision | Required owner input | Completion evidence |
| --- | --- | --- |
| Whether the desktop needs an additional organizational sign-in gate | Confirm whether direct Windows-session startup is sufficient, or specify Windows/Active Directory or Microsoft Entra identity, allowed groups, offline policy and administrator-managed configuration | An approved design implemented and checked on the intended managed Windows machines |
| Historical database credentials | An administrator identifies affected active accounts privately, revokes or rotates their credentials, and reviews access where available | A sanitized confirmation of rotation/revocation and controlled connection/log checks; never put replacement secrets in this repository |
| History treatment | Decide whether historical sensitive files require a coordinated rewrite after credential rotation | A separately authorized migration plan, including collaborators and retained clones; deleting a current file alone does not erase history |

## Reproducible dependency inventory

After NuGet restore, run:

```powershell
python tools/generate_third_party_notices.py --packages-root packages --output dist/THIRD_PARTY_NOTICES.md
```

The generator reads each exact package/version from `packages.config`, checks the restored `.nuspec` identity, records authors/copyright/license metadata and its SHA-256, and includes declared license files and top-level license/notice files. For a legacy restore that omits a loose `.nuspec`, it reads the single root manifest directly from the exact restored `.nupkg` without extracting archive paths. Missing packages, mismatched metadata and unsafe file paths fail instead of silently omitting a dependency. External license URLs are recorded without downloading or substituting today's terms for the pinned package's terms. Build-only tools are identified separately.

CI includes the notice in the build artifact and the installed application. Each GitHub release also publishes it as a separate versioned `THIRD_PARTY_NOTICES.md` asset. The portable ZIP keeps its existing accepted layout so earlier strict updaters remain compatible; the release page supplies the accompanying notice.

The notice is factual package provenance, not a complete legal clearance or a new license for this repository. Microsoft documents the distinction between [license expressions, packaged license files and older license URLs](https://learn.microsoft.com/nuget/reference/nuspec#license).

## Rights that package metadata cannot establish

| Material | Current evidence | Required owner decision |
| --- | --- | --- |
| SPI application source | The repository has no selected application license | Choose the intended license and copyright owner; add the actual license only after that choice |
| EPPlus 5.7.3 | The application selects Commercial context; the vendor describes EPPlus 5 as dual licensed | Confirm the applicable entitlement for the intended use, or approve a supported replacement; the context flag does not prove entitlement |
| Other restored dependencies | The generated notice records their pinned package metadata and included terms | Review redistribution obligations and preserve required notices with releases |
| Retained datasets, workbook fixtures, images and assets | Generated branding and synthetic regression fixtures are distinguishable from externally supplied material, but a code audit cannot establish ownership | Identify the source/owner and publication permission for each externally supplied retained asset |
| Historical files and retired SQL templates | Removal from a release does not establish rights to older public revisions | Confirm historical publication rights or authorize a specific cleanup plan |

EPPlus's [version-5 configuration documentation](https://www.epplussoftware.com/docs/5.6/api/index.html) explains its license context, and the [vendor's license overview](https://www.epplussoftware.com/en/LicenseOverview) describes the commercial/noncommercial choices. No purchase, license choice or ownership declaration is made by this change.

### Retained asset review locations

After the SQL Scripts removal, the following explicit image/data files remain for owner review. The resource files also contain generated form icons; generated code and runtime-created synthetic test workbooks are part of the application source review. This list identifies review locations without asserting ownership.

| File or group | Verifiable repository evidence |
| --- | --- |
| `SPI Data Comp Tool/app.ico` | Generator: `tools/make_app_images.py`; SHA-256 `a27726e99786c73c4a0c19b0c66a2e9962037bb3414b2218f8fdd599c8b3b918` |
| `SPI Data Comp Tool/Resources/about_side.png` | Generator: `tools/make_app_images.py`; SHA-256 `9a9a99a2900929cddd6bc6631fe407785a36f54920b212d628b97e915074892c` |
| Form `*.resx` and `Properties/Resources.resx` | Embedded form icons and the About image reference; generator updates the icon resources |
| `SPI Data Comp Tool/UserProfile/userProfile.example.json` | Example profile; both password fields are empty in the audited file; SHA-256 `bca6ec53ad38c1e92e0e5674bf24efc3bcef4b6729953848bf49a3b9e711699f` |
| `SPI Data Comp Tool/jsconfig_DB_connection.json` | Legacy editor configuration included as a noncompiled project item; SHA-256 `e72df9651f3029b8431c2b528d1a6f6fdbd9cc1ee4827d23e6b3dbe3ec6be04f` |

Retired SQL templates and older real-profile revisions still require the separate historical review above. Their sensitive contents are not reproduced here.
