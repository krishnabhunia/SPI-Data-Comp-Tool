<#
  Builds the per-user MSI of the 64-bit app with WiX Toolset v5.

    tools/build_msi.ps1 -SourceDir "SPI Data Comp Tool/bin/x64/Release" -Version 4.6.0 `
      -OutFile dist/windows-msi/SPI_Data_Comp_Tool_4.6.0_x64.msi

  SourceDir must hold "SPI Data Comp Tool.exe", its .config and THIRD_PARTY_NOTICES.md (the same
  files the x64 setup installs). A beta (4.6.0-beta003) packs as MSI version 4.6.0.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $SourceDir,
    [Parameter(Mandatory)] [string] $Version,
    [Parameter(Mandatory)] [string] $OutFile
)
$ErrorActionPreference = 'Stop'
$WixVersion = '5.0.2'

$numeric = ($Version -split '-')[0]
if ($numeric -notmatch '^\d+\.\d+\.\d+$') { throw "Version '$Version' is not x.y.z or x.y.z-betaNNN" }
$root = Split-Path $PSScriptRoot -Parent
$source = (Resolve-Path -LiteralPath $SourceDir).Path
foreach ($file in 'SPI Data Comp Tool.exe', 'SPI Data Comp Tool.exe.config', 'THIRD_PARTY_NOTICES.md') {
    if (-not (Test-Path -LiteralPath (Join-Path $source $file))) { throw "The MSI needs $file in $source" }
}
if (Get-ChildItem -LiteralPath $source -Filter *.dll) { throw 'DLL files next to the exe: the libraries must be embedded' }

# Pinned WiX tool in a private folder, so the build never depends on what the runner has installed.
$tools = Join-Path ([IO.Path]::GetTempPath()) "spi-wix-$WixVersion"
$wix = Join-Path $tools 'wix.exe'
if (-not (Test-Path -LiteralPath $wix)) {
    & dotnet tool install wix --version $WixVersion --tool-path $tools | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "Installing WiX $WixVersion failed ($LASTEXITCODE)" }
}

$out = [IO.Path]::GetFullPath($OutFile)
New-Item -ItemType Directory -Force (Split-Path $out) | Out-Null
if (Test-Path -LiteralPath $out) { Remove-Item -LiteralPath $out -Force }
& $wix build -arch x64 -nologo `
    -d "Version=$numeric" -d "SourceDir=$source" -d "IconFile=$(Join-Path $root 'SPI Data Comp Tool/app.ico')" `
    -o $out (Join-Path $root 'windows/msi/SPI_Data_Comp_Tool.wxs')
if ($LASTEXITCODE -ne 0) { throw "wix build failed ($LASTEXITCODE)" }
Remove-Item -LiteralPath ([IO.Path]::ChangeExtension($out, '.wixpdb')) -ErrorAction SilentlyContinue

$header = [byte[]]::new(8)
$stream = [IO.File]::OpenRead($out)
try { [void]$stream.Read($header, 0, 8) } finally { $stream.Dispose() }
if ([BitConverter]::ToString($header) -ne 'D0-CF-11-E0-A1-B1-1A-E1') { throw 'The MSI is not a Windows Installer package' }
Write-Host ("MSI {0}: version {1}, {2:N1} MB" -f (Split-Path $out -Leaf), $numeric, ((Get-Item -LiteralPath $out).Length / 1MB))
