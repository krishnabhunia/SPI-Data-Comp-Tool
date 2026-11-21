# Isolated migration experiment: none of these outputs are packaged or released.
$ErrorActionPreference = 'Stop'
foreach ($platform in @('Any CPU', 'x64')) {
    $suffix = if ($platform -eq 'x64') { 'x64' } else { 'AnyCPU' }
    $output = "bin\Compatibility48\$suffix\"
    $intermediate = "obj\Compatibility48\$suffix\"
    & msbuild 'SPI Data Comp Tool.sln' /m /t:Rebuild /p:Configuration=Release "/p:Platform=$platform" "/p:TargetFrameworkVersion=v4.8" "/p:OutputPath=$output" "/p:IntermediateOutputPath=$intermediate" /warnaserror
    if ($LASTEXITCODE -ne 0) { throw ".NET Framework 4.8 $platform compatibility build failed." }

    $exe = (Resolve-Path "SPI Data Comp Tool\$output\SPI Data Comp Tool.exe").Path
    $configPath = "$exe.config"
    [xml]$config = Get-Content $configPath
    $config.configuration.startup.supportedRuntime.SetAttribute('sku', '.NETFramework,Version=v4.8')
    $config.Save($configPath)

    foreach ($mode in @('--check-assemblies', '--check-ui', '--check-startup', '--check-excel-audit', '--check-themes', '--check-steps')) {
        $log = Join-Path $env:RUNNER_TEMP "framework48-$suffix-$($mode.TrimStart('-')).txt"
        if (Test-Path $log) { Remove-Item -LiteralPath $log -Force }
        $process = $null
        try {
            $process = Start-Process -FilePath $exe -ArgumentList $mode, "`"$log`"" -PassThru
            if (-not $process.WaitForExit(180000)) {
                try { & tools/ci_window_report.ps1 -ProcessId $process.Id } catch { Write-Warning $_ }
                throw ".NET Framework 4.8 $platform $mode timed out."
            }
            if (Test-Path $log) { Get-Content $log | ForEach-Object { Write-Host $_ } }
            if ($process.ExitCode -ne 0 -or -not (Test-Path $log)) {
                throw ".NET Framework 4.8 $platform $mode failed."
            }
        }
        finally {
            if ($process) {
                if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue }
                $process.Dispose()
            }
        }
    }
}
