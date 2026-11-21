param(
    [Parameter(Mandatory = $true)][string]$AppPath,
    [Parameter(Mandatory = $true)][string]$LogPath
)

$ErrorActionPreference = 'Stop'
$instance = 'SPI_Comparison_Checks_CI'
$command = Get-Command SqlLocalDB.exe -ErrorAction SilentlyContinue
$localDb = if ($command) { $command.Source } else {
    Get-ChildItem "$env:ProgramFiles\Microsoft SQL Server\*\Tools\Binn\SqlLocalDB.exe" -ErrorAction SilentlyContinue |
        Sort-Object FullName -Descending | Select-Object -First 1 -ExpandProperty FullName
}
if (-not $localDb) { throw 'SQL Server LocalDB is required for the disposable database integration checks.' }

$instances = @(& $localDb info)
if ($LASTEXITCODE -ne 0) { throw 'Could not enumerate LocalDB instances.' }
if ($instances -contains $instance) {
    throw "Refusing to reuse or delete existing LocalDB instance '$instance'. Run on a clean test machine."
}

$created = $false
$process = $null
try {
    & $localDb create $instance
    if ($LASTEXITCODE -ne 0) { throw 'Could not create the isolated LocalDB test instance.' }
    $created = $true
    & $localDb start $instance
    if ($LASTEXITCODE -ne 0) { throw 'Could not start the isolated LocalDB test instance.' }
    $exe = (Resolve-Path $AppPath).Path
    if (Test-Path $LogPath) { Remove-Item -LiteralPath $LogPath -Force }
    $process = Start-Process -FilePath $exe -ArgumentList '--check-sql-integration', "`"$LogPath`"" -PassThru
    if (-not $process.WaitForExit(300000)) {
        Stop-Process -Id $process.Id -Force
        throw 'SQL Server integration checks timed out.'
    }
    if (Test-Path $LogPath) { Get-Content $LogPath | ForEach-Object { Write-Host $_ } }
    if ($process.ExitCode -ne 0 -or -not (Test-Path $LogPath)) {
        throw 'SQL Server integration checks failed.'
    }
}
finally {
    if ($process) {
        if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue }
        $process.Dispose()
    }
    if ($created) {
        & $localDb stop $instance -k
        $stopExit = $LASTEXITCODE
        & $localDb delete $instance
        $deleteExit = $LASTEXITCODE
        if ($stopExit -ne 0 -or $deleteExit -ne 0) { throw 'Failed to clean up the newly created LocalDB test instance.' }
    }
}
