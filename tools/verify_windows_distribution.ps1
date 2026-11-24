param([Parameter(Mandatory)][string]$Root, [Parameter(Mandatory)][string]$Version)
. "$PSScriptRoot/windows_signing.ps1"
python "$PSScriptRoot/signing_handoff.py" distribution --root $Root --version $Version
if ($LASTEXITCODE -ne 0) { throw 'Final signed distribution layout failed' }
foreach ($file in "portable/SPI_Data_Comp_Tool_$Version.exe", "windows-x64/SPI_Data_Comp_Tool_$Version.exe", "windows-msi/SPI_Data_Comp_Tool_${Version}_x64.msi") {
    Test-SpiSignedFile -Path (Join-Path $Root $file) -Thumbprint $env:WINDOWS_SIGNING_THUMBPRINT
}
Write-Host "Final signed EXE and MSI bytes verified after smoke checks for $env:GITHUB_SHA; MSIX remains an unsigned Store submission"
