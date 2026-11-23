param([Parameter(Mandatory)][string]$FixturePath)
$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/../tools/windows_signing.ps1"
function Expect-Rejection([scriptblock]$Action, [string]$Name) {
    $rejected = $false
    try { & $Action } catch { $rejected = $true; Write-Host "PASS rejected $Name" }
    if (-not $rejected) { throw "Unsafe acceptance: $Name" }
}
$tool = Get-SpiSignTool
$tempRoot = [IO.Path]::GetFullPath($env:RUNNER_TEMP)
$dir = [IO.Path]::GetFullPath((Join-Path $tempRoot ('spi-signing-tests-' + [Guid]::NewGuid().ToString('N'))))
if (-not $dir.StartsWith($tempRoot.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Unsafe test directory' }
New-Item -ItemType Directory -Path $dir | Out-Null
$cert = $null
try {
    $file = Join-Path $dir 'fixture.exe'
    Copy-Item -LiteralPath $FixturePath -Destination $file
    Expect-Rejection { Test-SpiSignedFile -Path $file -Thumbprint ('0' * 40) } 'unsigned EXE'
    Expect-Rejection { Test-SpiSignedFile -Path "$dir/missing.exe" -Thumbprint ('0' * 40) } 'missing EXE'
    Expect-Rejection { Assert-SpiSigningContext } 'PR/local signing context'
    # An isolated negative fixture, never trusted, exported or shipped. Only My is
    # touched: do not install it into Root/TrustedPublisher or disable verification.
    $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject ('CN=SPI negative test ' + [Guid]::NewGuid().ToString('N')) -CertStoreLocation Cert:/CurrentUser/My -KeyExportPolicy NonExportable -NotAfter ([DateTime]::Now.AddDays(1))
    & $tool sign /s My /sha1 $cert.Thumbprint /fd SHA256 $file
    if ($LASTEXITCODE -ne 0) { throw 'Cannot create isolated negative signature fixture' }
    $signature = Get-AuthenticodeSignature -LiteralPath $file
    if (-not $signature.SignerCertificate -or $signature.SignerCertificate.Thumbprint -ne $cert.Thumbprint -or $signature.Status -eq 'Valid') {
        throw 'Fixture must be signed but untrusted'
    }
    Expect-Rejection { Test-SpiSignedFile -Path $file -Thumbprint ('0' * 40) } 'wrong signer'
    Expect-Rejection { Test-SpiSignedFile -Path $file -Thumbprint $cert.Thumbprint } 'self-signed untrusted release'
    $innoFile = Join-Path $dir 'uninst.e32.tmp'
    Copy-Item -LiteralPath $file -Destination $innoFile
    if ((Get-AuthenticodeSignature -LiteralPath $innoFile).SignerCertificate.Thumbprint -ne $cert.Thumbprint) {
        throw 'Native signature inspection must support Inno temporary PE names'
    }
    # Synthetic metadata tests the missing-timestamp guard separately from Windows'
    # real untrusted-certificate rejection above. It cannot make a fixture trusted.
    function Get-AuthenticodeSignature {
        param([string]$LiteralPath)
        return [PSCustomObject]@{ SignerCertificate = $script:cert; Status = 'Valid'; SignatureType = 'Authenticode'; TimeStamperCertificate = $null }
    }
    try { Expect-Rejection { Test-SpiSignedFile -Path $file -Thumbprint $cert.Thumbprint } 'missing timestamp metadata' }
    finally { Remove-Item Function:/Get-AuthenticodeSignature }
    $env:WINDOWS_SIGNING_STORE = 'CurrentUser'
    $env:WINDOWS_SIGNING_THUMBPRINT = $cert.Thumbprint
    Expect-Rejection { Get-SpiSigningCertificate | Out-Null } 'self-signed certificate preflight'
    $bytes = [IO.File]::ReadAllBytes($file)
    $bytes[1024] = $bytes[1024] -bxor 1
    [IO.File]::WriteAllBytes($file, $bytes)
    Expect-Rejection { Test-SpiSignedFile -Path $file -Thumbprint $cert.Thumbprint } 'tampered signed EXE'
    Write-Host 'Windows signing rejection checks passed (8); Inno temporary PE inspection passed; no test trust roots installed'
} finally {
    if ($cert) { Remove-Item -LiteralPath "Cert:/CurrentUser/My/$($cert.Thumbprint)" -DeleteKey }
    # dir was resolved and checked against the explicitly named temporary root above.
    Remove-Item -LiteralPath $dir -Recurse -Force
}
