<#
.SYNOPSIS
  Signs Windows executables with the owner's code-signing certificate through SSL.com eSigner (cloud signing).

.DESCRIPTION
  Since June 2023 a code-signing certificate's private key cannot be exported to a .pfx file: it lives on a
  hardware token or in a cloud HSM. GitHub's runners cannot use a USB token, so the build signs through
  SSL.com eSigner with SSL.com's CodeSignTool. See docs/CODE_SIGNING.md for the one-time set-up.

  The four secrets of the protected "code-signing" environment are passed in as environment variables
  (only to the signing steps of the master-only "sign" job):
    SIGN_ES_USERNAME, SIGN_ES_PASSWORD, SIGN_ES_CREDENTIAL_ID, SIGN_ES_TOTP_SECRET
  - none set  -> signing is skipped (the build works exactly as before, unsigned)
  - some set  -> the build fails, so a half-finished set-up is never mistaken for a signed release
  - all set   -> every file is signed, time-stamped and checked with Get-AuthenticodeSignature

.PARAMETER Path
  The files to sign. Each file is signed in place.

.PARAMETER CheckOnly
  Only report whether signing is set up: writes configured=true/false to the step's GitHub output.
#>
param(
    [string[]] $Path,
    [switch] $CheckOnly
)

$ErrorActionPreference = 'Stop'

# CodeSignTool is downloaded from SSL.com's GitHub releases and must match this exact file.
$ToolVersion = 'v1.3.0'
$ToolUrl = "https://github.com/SSLcom/CodeSignTool/releases/download/$ToolVersion/CodeSignTool-$ToolVersion-windows.zip"
$ToolSha256 = 'E22094505DECBE622AFE5B0C27ABC618ED2BA179BD94F3450490352399D5EF2A'

function Test-SigningConfigured {
    $names = 'SIGN_ES_USERNAME', 'SIGN_ES_PASSWORD', 'SIGN_ES_CREDENTIAL_ID', 'SIGN_ES_TOTP_SECRET'
    $set = @($names | Where-Object { -not [string]::IsNullOrWhiteSpace([Environment]::GetEnvironmentVariable($_)) })
    if ($set.Count -eq 0) { return $false }
    if ($set.Count -ne $names.Count) {
        $missing = $names | Where-Object { $set -notcontains $_ }
        throw "Code signing is only partly set up. Add the missing repository secret(s): $($missing -join ', ') (see docs/CODE_SIGNING.md)."
    }
    return $true
}

$configured = Test-SigningConfigured
if ($CheckOnly) {
    if ($env:GITHUB_OUTPUT) { "configured=$($configured.ToString().ToLowerInvariant())" | Out-File $env:GITHUB_OUTPUT -Append -Encoding utf8 }
}
if (-not $configured) {
    # Plain output, not a workflow annotation: the build stays free of warnings until signing is set up.
    Write-Host "Code signing skipped: no signing certificate is set up yet (see docs/CODE_SIGNING.md). Files stay unsigned."
    exit 0
}
if ($CheckOnly) {
    Write-Host "Code signing is set up."
    exit 0
}
if (-not $Path) { throw 'Give the files to sign with -Path.' }

$files = foreach ($p in $Path) {
    $resolved = @(Resolve-Path $p -ErrorAction Stop)
    if ($resolved.Count -eq 0) { throw "Nothing to sign at $p" }
    $resolved | ForEach-Object { $_.Path }
}

$temp = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
$toolRoot = Join-Path $temp "CodeSignTool-$ToolVersion"
$tool = Join-Path $toolRoot 'jar\code_sign_tool-1.3.0.jar'
$java = Join-Path $toolRoot 'jdk-11.0.2\bin\java.exe'
if (-not (Test-Path $tool)) {
    $zip = Join-Path $temp "CodeSignTool-$ToolVersion-windows.zip"
    Write-Host "Downloading CodeSignTool $ToolVersion"
    Invoke-WebRequest -Uri $ToolUrl -OutFile $zip -UseBasicParsing
    $actual = (Get-FileHash $zip -Algorithm SHA256).Hash
    if ($actual -ne $ToolSha256) { throw "CodeSignTool download does not match the expected SHA-256 ($actual)." }
    Expand-Archive $zip -DestinationPath $toolRoot -Force
    if (-not (Test-Path $tool) -or -not (Test-Path $java)) { throw 'CodeSignTool or its Java runtime is missing from the download.' }
}

foreach ($file in $files) {
    $name = Split-Path $file -Leaf
    Write-Host "Signing $name"
    # Java is started directly (not through CodeSignTool.bat and cmd), so characters such as & or ^ in a
    # secret are passed unchanged. Secrets are masked in the log by GitHub; the command line is never printed.
    Push-Location $toolRoot
    try {
        $output = & $java -jar $tool sign `
            "-username=$env:SIGN_ES_USERNAME" "-password=$env:SIGN_ES_PASSWORD" `
            "-credential_id=$env:SIGN_ES_CREDENTIAL_ID" "-totp_secret=$env:SIGN_ES_TOTP_SECRET" `
            "-input_file_path=$file" '-override=true' 2>&1
        $code = $LASTEXITCODE
    }
    finally { Pop-Location }
    $output | ForEach-Object { Write-Host $_ }
    if ($code -ne 0) { throw "CodeSignTool failed for $name (exit code $code)." }

    $signature = Get-AuthenticodeSignature $file
    if ($signature.Status -ne 'Valid') { throw "$name is not validly signed after signing: $($signature.Status) $($signature.StatusMessage)" }
    if ($null -eq $signature.TimeStamperCertificate) { throw "$name was signed without a time stamp, so it would stop being trusted when the certificate expires." }
    Write-Host "Signed $name by $($signature.SignerCertificate.Subject)"
}
