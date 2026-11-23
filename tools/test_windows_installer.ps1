param([Parameter(Mandatory)][string]$SetupPath,
      [Parameter(Mandatory)][string]$NoticePath,
      [string]$PayloadPath,
      [switch]$RequireSignature)
$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/windows_signing.ps1"
function Invoke-CheckedProcess([string]$File, [string[]]$Arguments, [int]$Timeout = 120000) {
    $process = Start-Process -FilePath $File -ArgumentList $Arguments -WindowStyle Hidden -PassThru
    if (-not $process.WaitForExit($Timeout)) { Stop-Process -Id $process.Id -Force; throw "Process timed out: $File" }
    if ($process.ExitCode -ne 0) { throw "Process failed: $File ($($process.ExitCode))" }
}
if ($RequireSignature) {
    Test-SpiSignedFile -Path $SetupPath -Thumbprint $env:WINDOWS_SIGNING_THUMBPRINT
    if (-not $PayloadPath) { throw 'Signed installer checks require the expected signed x64 payload' }
}
$dir = Join-Path $env:RUNNER_TEMP ('spi-install-' + [Guid]::NewGuid().ToString('N'))
Invoke-CheckedProcess $SetupPath @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART','/CURRENTUSER',"/DIR=`"$dir`"")
$exe = Join-Path $dir 'SPI Data Comp Tool.exe'
$uninstall = Join-Path $dir 'unins000.exe'
$uninstallAllowed = -not $RequireSignature
try {
    if (-not (Test-Path -LiteralPath $exe)) { throw 'installed exe missing' }
    if ($RequireSignature) {
        Test-SpiSignedFile -Path $exe -Thumbprint $env:WINDOWS_SIGNING_THUMBPRINT
        Test-SpiSignedFile -Path $uninstall -Thumbprint $env:WINDOWS_SIGNING_THUMBPRINT
        $uninstallAllowed = $true
        if ((Get-FileHash -LiteralPath $exe).Hash -ne (Get-FileHash -LiteralPath $PayloadPath).Hash) {
            throw 'Installed application differs from the verified signed payload'
        }
    }
    $notice = Join-Path $dir 'THIRD_PARTY_NOTICES.md'
    if (-not (Test-Path -LiteralPath $notice)) { throw 'Installed third-party dependency notices are missing' }
    if ((Get-FileHash -LiteralPath $notice).Hash -ne (Get-FileHash -LiteralPath $NoticePath).Hash) { throw 'Installed dependency notices differ from the restored package inventory' }
    if (Test-Path (Join-Path $dir 'PNID_Reports')) { throw 'Retired SQL templates or portable compatibility anchors leaked into the clean install' }
    $dlls = Get-ChildItem -LiteralPath $dir -Recurse -Filter *.dll
    if ($dlls) { throw 'DLL files in the install folder' }
    $bytes = [IO.File]::ReadAllBytes($exe)
    $pe = [BitConverter]::ToInt32($bytes, 0x3C)
    if ([BitConverter]::ToUInt16($bytes, $pe + 4) -ne 0x8664) { throw 'installed exe is not 64-bit' }
    foreach ($check in 'assemblies','ui','startup') {
        $log = Join-Path $env:RUNNER_TEMP "x64-$check-checks.txt"
        Invoke-CheckedProcess $exe @("--check-$check", "`"$log`"")
        if (-not (Test-Path -LiteralPath $log)) { throw "Missing x64 $check check log" }
        Get-Content -LiteralPath $log | ForEach-Object { Write-Host $_ }
    }
} finally {
    # Never execute a failed signature target just to clean up a signing test.
    if ($uninstallAllowed -and (Test-Path -LiteralPath $uninstall)) {
        Invoke-CheckedProcess $uninstall @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART')
    }
}
for ($i = 0; $i -lt 30 -and (Test-Path -LiteralPath $exe); $i++) { Start-Sleep 1 }
if (Test-Path -LiteralPath $exe) { throw 'uninstall left the exe behind' }
Write-Host '64-bit installer: installed, 64-bit exe, no DLL files, libraries load, uninstalled cleanly'
