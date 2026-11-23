# Shared Authenticode operations. Dot-sourcing defines functions only; it does not sign.
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-SpiSignTool {
    $sdk = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits/10/bin'
    $tools = @(Get-ChildItem -LiteralPath $sdk -Directory | Where-Object Name -Match '^10\.0\.\d+\.\d+$' |
        Sort-Object { [version]$_.Name } -Descending | ForEach-Object { Join-Path $_.FullName 'x64/signtool.exe' } |
        Where-Object { Test-Path -LiteralPath $_ -PathType Leaf })
    if (-not $tools) { throw 'Install the Windows SDK x64 SignTool on the signing/test runner' }
    return $tools[0]
}

function Assert-SpiSigningContext {
    if ($env:GITHUB_EVENT_NAME -ne 'push' -or $env:GITHUB_REF -ne 'refs/heads/master' -or
        $env:REF_PROTECTED -ne 'true' -or $env:WINDOWS_SIGNING_MODE -ne 'store' -or
        $env:WINDOWS_SIGNING_RUNNER_ISOLATED -ne 'true') {
        throw 'Signing is restricted to isolated protected-master push jobs'
    }
    $head = & git rev-parse HEAD
    if ($LASTEXITCODE -ne 0 -or $head -cne $env:GITHUB_SHA) { throw 'Signing checkout does not match the release commit' }
}

function Get-SpiSigningCertificate {
    $thumbprint = $env:WINDOWS_SIGNING_THUMBPRINT
    $store = $env:WINDOWS_SIGNING_STORE
    if ($thumbprint -notmatch '^[A-Fa-f0-9]{40}$' -or $store -notin 'CurrentUser','LocalMachine') {
        throw 'Configure an exact certificate thumbprint and CurrentUser/LocalMachine store'
    }
    $cert = Get-Item -LiteralPath "Cert:/$store/My/$thumbprint"
    if ($cert.Subject -eq $cert.Issuer) { throw 'A self-signed leaf is not a public-release identity' }
    if (-not $cert.HasPrivateKey -or $cert.NotBefore.ToUniversalTime() -gt [DateTime]::UtcNow -or
        $cert.NotAfter.ToUniversalTime() -le [DateTime]::UtcNow) { throw 'Signing certificate/key is missing or not currently valid' }
    $eku = @($cert.Extensions | Where-Object { $_ -is [Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension] } |
        ForEach-Object { $_.EnhancedKeyUsages } | ForEach-Object Value)
    if ('1.3.6.1.5.5.7.3.3' -notin $eku) { throw 'Certificate must explicitly allow code signing' }
    $chain = [Security.Cryptography.X509Certificates.X509Chain]::new()
    try {
        $chain.ChainPolicy.RevocationMode = [Security.Cryptography.X509Certificates.X509RevocationMode]::Online
        $chain.ChainPolicy.RevocationFlag = [Security.Cryptography.X509Certificates.X509RevocationFlag]::ExcludeRoot
        $chain.ChainPolicy.UrlRetrievalTimeout = [TimeSpan]::FromSeconds(30)
        [void]$chain.ChainPolicy.ApplicationPolicy.Add([Security.Cryptography.Oid]::new('1.3.6.1.5.5.7.3.3'))
        if (-not $chain.Build($cert)) { throw 'Signing certificate does not chain to Windows trust with online revocation checks' }
    } finally { $chain.Dispose() }
    # Hardware/cloud key protection is an external provider prerequisite, not something
    # HasPrivateKey or an export-policy flag can prove. Never export/import a key here.
    return $cert
}

function Test-SpiSignedFile {
    param([Parameter(Mandatory)][string]$Path, [Parameter(Mandatory)][string]$Thumbprint)
    if ($Thumbprint -notmatch '^[a-fA-F0-9]{40}$') { throw 'Invalid expected signer thumbprint' }
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf) -or (Get-Item -LiteralPath $Path).LinkType) {
        throw 'Missing or linked signature target'
    }
    $signature = Get-AuthenticodeSignature -LiteralPath $Path
    if (-not $signature.SignerCertificate -or $signature.SignerCertificate.Thumbprint -ine $Thumbprint) {
        throw "Unexpected or missing Authenticode signer: $Path"
    }
    if ($signature.Status -ne 'Valid') { throw "Authenticode validation failed: $Path ($($signature.Status))" }
    if ($signature.SignatureType -ne 'Authenticode') { throw "An embedded Authenticode signature is required: $Path" }
    if (-not $signature.TimeStamperCertificate) { throw "Missing trusted timestamp: $Path" }
    $tool = Get-SpiSignTool
    & $tool verify /pa /all /v /tw $Path
    if ($LASTEXITCODE -ne 0) { throw "SignTool verification failed or warned: $Path ($LASTEXITCODE)" }
    Write-Host "Verified: $Path; signer=$($signature.SignerCertificate.Subject); thumbprint=$Thumbprint; SHA256=$((Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash)"
}

function Sign-SpiFile {
    param([Parameter(Mandatory)][string]$Path)
    Assert-SpiSigningContext
    $cert = Get-SpiSigningCertificate
    $timestamp = $env:WINDOWS_SIGNING_TIMESTAMP_URL
    $uri = $null
    if (-not [Uri]::TryCreate($timestamp, [UriKind]::Absolute, [ref]$uri) -or
        $uri.Scheme -notin 'https','http' -or $uri.UserInfo -or $uri.Query -or $uri.Fragment -or
        $timestamp -match '[\r\n" $`]') { throw 'Invalid public RFC 3161 timestamp URL' }
    $file = Get-Item -LiteralPath $Path
    # Inno passes PE executables with names such as uninst.e32.tmp. Validate the
    # file format rather than refusing its temporary extension or signing data.
    if ($file.LinkType -or $file.Length -gt 256MB -or $file.Length -lt 64) { throw 'Invalid executable signing target' }
    $bytes = [IO.File]::ReadAllBytes($file.FullName)
    $pe = [BitConverter]::ToInt32($bytes, 0x3C)
    if ($bytes[0] -ne 0x4D -or $bytes[1] -ne 0x5A -or $pe -lt 64 -or $pe -gt $bytes.Length - 24 -or
        [BitConverter]::ToUInt32($bytes, $pe) -ne 0x00004550) { throw 'Only PE executables may be signed' }
    $tool = Get-SpiSignTool
    $arguments = @('sign','/s','My','/sha1',$cert.Thumbprint,'/fd','SHA256','/tr',$timestamp,'/td','SHA256','/d','SPI Data Comparison Tool')
    if ($env:WINDOWS_SIGNING_STORE -eq 'LocalMachine') { $arguments += '/sm' }
    & $tool @arguments $file.FullName
    if ($LASTEXITCODE -ne 0) { throw "Signing failed or warned: $Path ($LASTEXITCODE)" }
    Test-SpiSignedFile -Path $file.FullName -Thumbprint $cert.Thumbprint
}
