<#
  Installs the per-user MSI silently, checks and runs the installed app, then uninstalls it.

    tools/test_windows_msi.ps1 -MsiPath dist/windows-msi/SPI_Data_Comp_Tool_4.6.0_x64.msi -Version 4.6.0 -NoticePath dist/THIRD_PARTY_NOTICES.md
#>
param([Parameter(Mandatory)][string]$MsiPath,
      [Parameter(Mandatory)][string]$Version,
      [Parameter(Mandatory)][string]$NoticePath,
      [switch]$RequireSignature)
$ErrorActionPreference = 'Stop'
$msi = (Resolve-Path -LiteralPath $MsiPath).Path
$msiexec = Join-Path $env:SystemRoot 'System32\msiexec.exe'
function Invoke-Msi([string[]]$Arguments, [string]$What) {
    $process = Start-Process -FilePath $msiexec -ArgumentList $Arguments -WindowStyle Hidden -PassThru
    if (-not $process.WaitForExit(300000)) { Stop-Process -Id $process.Id -Force; throw "$What timed out" }
    if ($process.ExitCode -ne 0) { throw "$What failed (msiexec exit code $($process.ExitCode))" }
}
function Invoke-Checked([string]$File, [string[]]$Arguments) {
    $process = Start-Process -FilePath $File -ArgumentList $Arguments -WindowStyle Hidden -PassThru
    if (-not $process.WaitForExit(180000)) { Stop-Process -Id $process.Id -Force; throw "Timed out: $($Arguments[0])" }
    if ($process.ExitCode -ne 0) { throw "Failed: $($Arguments[0]) ($($process.ExitCode))" }
}
if ($RequireSignature -and (Get-AuthenticodeSignature -LiteralPath $msi).Status -ne 'Valid') { throw 'The MSI is not signed' }

$dir = Join-Path $env:LOCALAPPDATA 'Programs\SPI Data Comp Tool (MSI)'
$exe = Join-Path $dir 'SPI Data Comp Tool.exe'
$key = 'HKCU:\Software\SPI Data Comp Tool\MSI'
$installLog = Join-Path $env:RUNNER_TEMP 'msi-install.log'
Invoke-Msi @('/i', "`"$msi`"", '/qn', '/norestart', '/l*v', "`"$installLog`"") 'MSI install'
$installed = $false
try {
    $installed = $true
    if (-not (Test-Path -LiteralPath $exe)) { throw "The MSI did not install $exe" }
    $location = (Get-ItemProperty -LiteralPath $key -Name InstallLocation).InstallLocation
    if ([IO.Path]::GetFullPath($location).TrimEnd('\') -ne [IO.Path]::GetFullPath($dir).TrimEnd('\')) { throw "MSI InstallLocation is $location" }
    if ((Get-ItemProperty -LiteralPath $key -Name Version).Version -ne ($Version -split '-')[0]) { throw 'MSI registered the wrong version' }
    $shortcut = Join-Path ([Environment]::GetFolderPath('Programs')) 'SPI Data Comparison Tool.lnk'
    if (-not (Test-Path -LiteralPath $shortcut)) { throw 'The Start menu shortcut is missing' }
    $notice = Join-Path $dir 'THIRD_PARTY_NOTICES.md'
    if ((Get-FileHash -LiteralPath $notice).Hash -ne (Get-FileHash -LiteralPath $NoticePath).Hash) { throw 'Installed dependency notices differ' }
    if (Get-ChildItem -LiteralPath $dir -Recurse -Filter *.dll) { throw 'DLL files in the MSI install folder' }
    if ($RequireSignature -and (Get-AuthenticodeSignature -LiteralPath $exe).Status -ne 'Valid') { throw 'The installed app is not signed' }
    $bytes = [IO.File]::ReadAllBytes($exe)
    if ([BitConverter]::ToUInt16($bytes, [BitConverter]::ToInt32($bytes, 0x3C) + 4) -ne 0x8664) { throw 'The installed exe is not 64-bit' }
    $product = (Get-Item -LiteralPath $exe).VersionInfo.ProductVersion
    if ($product -ne $Version -and $product -ne "$(($Version -split '-')[0]).0") { throw "Installed app version is $product" }
    foreach ($check in @(@('--check-install-kind', 'msi'), @('--check-assemblies'), @('--check-ui'), @('--check-startup'))) {
        $log = Join-Path $env:RUNNER_TEMP ("msi" + $check[0] + '.txt')
        Invoke-Checked $exe (@($check[0], "`"$log`"") + @($check | Select-Object -Skip 1))
        if (-not (Test-Path -LiteralPath $log)) { throw "MSI $($check[0]) wrote no log" }
        Get-Content -LiteralPath $log | ForEach-Object { Write-Host $_ }
    }
    # Installing the same package again must upgrade in place (beta to normal of the same number).
    Invoke-Msi @('/i', "`"$msi`"", '/qn', '/norestart') 'MSI reinstall of the same version'
    if (-not (Test-Path -LiteralPath $exe)) { throw 'The same-version upgrade removed the app' }
}
finally {
    if ($installed) { Invoke-Msi @('/x', "`"$msi`"", '/qn', '/norestart') 'MSI uninstall' }
}
for ($i = 0; $i -lt 30 -and (Test-Path -LiteralPath $exe); $i++) { Start-Sleep 1 }
if (Test-Path -LiteralPath $exe) { throw 'The MSI uninstall left the exe behind' }
if (Test-Path -LiteralPath $key) { throw 'The MSI uninstall left its registration behind' }
Write-Host 'Per-user MSI: installs without admin, registers its folder, app runs as an MSI copy, upgrades in place, uninstalls cleanly'
