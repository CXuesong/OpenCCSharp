trap {
    Write-Error $_
    Write-Host $_.ScriptStackTrace
    Exit 1
}

function CheckLastExitCode {
    if ($LASTEXITCODE) {
        Write-Error "Last operation exited with $LASTEXITCODE"
    }
}

$SolutionDir = (Resolve-Path $PSScriptRoot/../..).Path

dotnet build "$SolutionDir/OpenCCSharp.BuildTools"
CheckLastExitCode
dotnet run --project "$SolutionDir/OpenCCSharp.BuildTools" -- `
    build-preset-dictionaries "$SolutionDir/OpenCCSharp.Presets/PresetDictionaryList.txt" "$SolutionDir/vendor/OpenCC/data/dictionary" "$SolutionDir/OpenCCSharp.Presets/ConversionDictionaries"
CheckLastExitCode
