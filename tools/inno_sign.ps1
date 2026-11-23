param([Parameter(Mandatory)][string]$Path)
. "$PSScriptRoot/windows_signing.ps1"
try {
    Assert-SpiSigningContext
    $resolved = (Get-Item -LiteralPath $Path).FullName
    $allowed = @($env:SPI_SIGNING_OUTPUT_ROOT, $env:SPI_SIGNING_TEMP_ROOT) | Where-Object { $_ }
    $inside = @($allowed | Where-Object { $resolved.StartsWith([IO.Path]::GetFullPath($_).TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase) })
    if (-not $inside) { throw 'Inno signing target is outside this job output/temp directories' }
    Sign-SpiFile -Path $resolved
    exit 0
} catch {
    Write-Error $_ -ErrorAction Continue
    exit 1
}
