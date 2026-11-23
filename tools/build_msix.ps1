<#
  Builds the Microsoft Store (MSIX) package of the 64-bit app.

    tools/build_msix.ps1 -SourceDir "SPI Data Comp Tool/bin/x64/Release" -Version 4.5.0 `
      -IdentityName <from Partner Center> -Publisher "CN=<from Partner Center>" `
      -PublisherDisplayName <from Partner Center> -OutFile dist/store/SPI_Data_Comp_Tool_4.5.0_x64.msix

  The package is left unsigned: Partner Center signs it when it is submitted, which is what removes
  the SmartScreen "Unknown publisher" warning for Store installs. See docs/MICROSOFT_STORE.md.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $SourceDir,
    [Parameter(Mandatory)] [string] $Version,
    [Parameter(Mandatory)] [string] $IdentityName,
    [Parameter(Mandatory)] [string] $Publisher,
    [Parameter(Mandatory)] [string] $PublisherDisplayName,
    [Parameter(Mandatory)] [string] $OutFile
)
$ErrorActionPreference = 'Stop'

# The Store needs four numbers with the last one 0; a beta (4.5.0-beta003) packs as 4.5.0.0.
$numeric = ($Version -split '-')[0]
if ($numeric -notmatch '^\d+\.\d+\.\d+$') { throw "Version '$Version' is not x.y.z or x.y.z-betaNNN" }
$packageVersion = "$numeric.0"
if ($IdentityName -notmatch '^[A-Za-z0-9.\-]{3,50}$') { throw "Identity name '$IdentityName' is not a valid package name" }
if ($Publisher -notmatch '^CN=') { throw "Publisher '$Publisher' must start with CN=" }

$root = Split-Path $PSScriptRoot -Parent
$template = Join-Path $root 'windows/msix/AppxManifest.xml'
$icon = Join-Path $root 'SPI Data Comp Tool/app.ico'
$source = (Resolve-Path $SourceDir).Path
$exe = Join-Path $source 'SPI Data Comp Tool.exe'
if (-not (Test-Path $exe)) { throw "The 64-bit app is missing: $exe" }

$layout = Join-Path ([IO.Path]::GetTempPath()) ("spi-msix-" + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $layout | Out-Null
try {
    # Same files as the installer: no debug symbols, XML docs or ClickOnce output.
    Get-ChildItem $source -File | Where-Object { $_.Extension -notin '.pdb', '.xml', '.application', '.manifest' } |
        Copy-Item -Destination $layout
    if (Get-ChildItem $layout -Filter *.dll) { throw 'DLL files next to the exe: the libraries must be embedded' }
    # Store logos are drawn from the app icon here, so the repository keeps only its generated images.
    Add-Type -AssemblyName System.Drawing
    $assetDir = Join-Path $layout 'Assets'
    New-Item -ItemType Directory -Force $assetDir | Out-Null
    # Use the icon's largest frame. A 256 px frame is stored as PNG, which Icon.ToBitmap cannot
    # read on .NET Framework, so read that frame from the .ico directory directly.
    $bytes = [IO.File]::ReadAllBytes($icon)
    $best = $null
    for ($i = 0; $i -lt [BitConverter]::ToUInt16($bytes, 4); $i++) {
        $entry = 6 + 16 * $i
        $size = if ($bytes[$entry] -eq 0) { 256 } else { [int] $bytes[$entry] }
        if (-not $best -or $size -gt $best.Size) {
            $best = @{ Size = $size; Length = [BitConverter]::ToInt32($bytes, $entry + 8); Offset = [BitConverter]::ToInt32($bytes, $entry + 12) }
        }
    }
    if (-not $best) { throw "No image in $icon" }
    $frame = New-Object byte[] $best.Length
    [Array]::Copy($bytes, $best.Offset, $frame, 0, $best.Length)
    $source256 = New-Object IO.MemoryStream (, $frame)
    if ($frame[0] -eq 0x89 -and $frame[1] -eq 0x50) { $art = [System.Drawing.Image]::FromStream($source256) }
    else { $art = (New-Object System.Drawing.Icon $icon, $best.Size, $best.Size).ToBitmap() }
    try {
        foreach ($logo in @(@('StoreLogo.png', 50, 1), @('Square44x44Logo.png', 44, 2), @('Square150x150Logo.png', 150, 25))) {
            $px = $logo[1]; $pad = $logo[2]
            $bitmap = New-Object System.Drawing.Bitmap $px, $px, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
            $g = [System.Drawing.Graphics]::FromImage($bitmap)
            $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $g.Clear([System.Drawing.Color]::Transparent)
            $g.DrawImage($art, $pad, $pad, $px - 2 * $pad, $px - 2 * $pad)
            $g.Dispose()
            $bitmap.Save((Join-Path $assetDir $logo[0]), [System.Drawing.Imaging.ImageFormat]::Png)
            $bitmap.Dispose()
        }
    }
    finally { $art.Dispose(); $source256.Dispose() }

    # Values go into XML attributes and text, so escape them.
    $values = @{
        '$IdentityName$'         = [Security.SecurityElement]::Escape($IdentityName)
        '$Publisher$'            = [Security.SecurityElement]::Escape($Publisher)
        '$PublisherDisplayName$' = [Security.SecurityElement]::Escape($PublisherDisplayName)
        '$Version$'              = $packageVersion
    }
    $manifest = Get-Content $template -Raw
    foreach ($token in $values.Keys) { $manifest = $manifest.Replace($token, $values[$token]) }
    if ($manifest -match '\$[A-Za-z]+\$') { throw "Unreplaced token in the manifest: $($Matches[0])" }
    [xml] $manifest | Out-Null   # must still be well-formed XML
    [IO.File]::WriteAllText((Join-Path $layout 'AppxManifest.xml'), $manifest, (New-Object Text.UTF8Encoding $false))

    $kits = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'
    $makeappx = Get-ChildItem $kits -Recurse -Filter makeappx.exe -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -match '\\x64\\' } | Sort-Object FullName -Descending | Select-Object -First 1
    if (-not $makeappx) { throw 'makeappx.exe (Windows SDK) not found' }

    $out = [IO.Path]::GetFullPath($OutFile)
    New-Item -ItemType Directory -Force (Split-Path $out) | Out-Null
    # makeappx checks the manifest against the MSIX schema while packing.
    & $makeappx.FullName pack /o /h SHA256 /d $layout /p $out
    if ($LASTEXITCODE -ne 0) { throw "makeappx pack failed ($LASTEXITCODE)" }

    # Read the package back: the manifest and the app must be inside it.
    $check = Join-Path ([IO.Path]::GetTempPath()) ("spi-msix-check-" + [guid]::NewGuid().ToString('N'))
    & $makeappx.FullName unpack /o /p $out /d $check | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "makeappx unpack failed ($LASTEXITCODE)" }
    foreach ($f in 'AppxManifest.xml', 'SPI Data Comp Tool.exe', 'SPI Data Comp Tool.exe.config', 'Assets/StoreLogo.png') {
        if (-not (Test-Path (Join-Path $check $f))) { throw "The package is missing $f" }
    }
    $packed = [xml] (Get-Content (Join-Path $check 'AppxManifest.xml') -Raw)
    if ($packed.Package.Identity.Version -ne $packageVersion) { throw 'The packed manifest has the wrong version' }
    Remove-Item $check -Recurse -Force
    Write-Host ("MSIX {0}: {1} {2}, {3:N1} MB" -f (Split-Path $out -Leaf), $IdentityName, $packageVersion, ((Get-Item $out).Length / 1MB))
}
finally {
    Remove-Item $layout -Recurse -Force -ErrorAction SilentlyContinue
}
