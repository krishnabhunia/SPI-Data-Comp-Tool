param([Parameter(Mandatory)][string]$Version,
      [Parameter(Mandatory)][string]$InputRoot,
      [Parameter(Mandatory)][string]$OutputRoot)
. "$PSScriptRoot/windows_signing.ps1"
Assert-SpiSigningContext
python "$PSScriptRoot/signing_handoff.py" verify --root $InputRoot --version $Version --sha $env:GITHUB_SHA --run-id $env:GITHUB_RUN_ID --attempt $env:GITHUB_RUN_ATTEMPT
if ($LASTEXITCODE -ne 0) { throw 'Untrusted signing input' }
if (Test-Path -LiteralPath $OutputRoot) { throw 'Signed distribution must use a fresh output directory' }
$inputDir = (Resolve-Path -LiteralPath $InputRoot).Path
$outputDir = (New-Item -ItemType Directory -Path $OutputRoot).FullName
$env:SPI_SIGNING_OUTPUT_ROOT = $outputDir
$env:SPI_SIGNING_TEMP_ROOT = (New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('spi-sign-' + [Guid]::NewGuid().ToString('N')))).FullName
# Inno's generated uninstall and setup self-copy files stay in this disposable job directory.
$env:TEMP = $env:SPI_SIGNING_TEMP_ROOT
$env:TMP = $env:SPI_SIGNING_TEMP_ROOT
Copy-Item -LiteralPath "$inputDir/portable" -Destination $outputDir -Recurse
# MSIX is an unsigned Partner Center submission asset. Preserve its hash-bound
# bytes; the EXE signing backend does not certify or publish a Store package.
Copy-Item -LiteralPath "$inputDir/store" -Destination $outputDir -Recurse
Copy-Item -LiteralPath "$inputDir/THIRD_PARTY_NOTICES.md" -Destination $outputDir
Copy-Item -LiteralPath "$inputDir/THIRD_PARTY_NOTICES.md" -Destination "$inputDir/x64"
Sign-SpiFile -Path "$outputDir/portable/SPI_Data_Comp_Tool_$Version.exe"
Sign-SpiFile -Path "$inputDir/x64/SPI Data Comp Tool.exe"
$iscc = Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6/ISCC.exe'
if (-not (Test-Path -LiteralPath $iscc)) { throw 'Preinstall reviewed Inno Setup 6 on the isolated signing image' }
$inno = (Get-Item -LiteralPath $iscc).VersionInfo
if ([version]::new($inno.FileMajorPart, $inno.FileMinorPart, $inno.FileBuildPart, $inno.FilePrivatePart) -lt [version]'6.7.4.0') {
    throw 'Signing requires reviewed Inno Setup 6.7.4 or newer, including signed temporary self-copies'
}
$setupDir = (New-Item -ItemType Directory -Path "$outputDir/windows-x64").FullName
$cache = (New-Item -ItemType Directory -Path "$env:SPI_SIGNING_TEMP_ROOT/uninstaller").FullName
function Quote-Inno([string]$Value) { return '$q' + $Value.Replace('$', '$$') + '$q' }
$callback = (Quote-Inno (Join-Path $PSHOME 'pwsh.exe')) + ' -NoProfile -NonInteractive -File ' +
    (Quote-Inno "$PSScriptRoot/inno_sign.ps1") + ' -Path $f'
$arguments = @('/Qp', "/DAppVersion=$Version", "/DNumericVersion=$(($Version -split '-')[0])",
    "/DSourceDir=$inputDir/x64", "/DOutputDir=$setupDir", '/DSpiSigning=1', "/DSigningWorkDir=$cache",
    "/Sspi_authenticode=$callback", "$PSScriptRoot/../windows/installer/SPI_Data_Comp_Tool.iss")
& $iscc @arguments
if ($LASTEXITCODE -ne 0) { throw "Signed installer compilation failed ($LASTEXITCODE)" }
Test-SpiSignedFile -Path "$setupDir/SPI_Data_Comp_Tool_$Version.exe" -Thumbprint $env:WINDOWS_SIGNING_THUMBPRINT
python "$PSScriptRoot/signing_handoff.py" distribution --root $outputDir --version $Version
if ($LASTEXITCODE -ne 0) { throw 'Signed distribution layout failed validation' }
Write-Host "Signing complete for $env:GITHUB_SHA, run $env:GITHUB_RUN_ID/$env:GITHUB_RUN_ATTEMPT; installer smoke checks still required"
