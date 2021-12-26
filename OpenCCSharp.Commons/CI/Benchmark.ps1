param (
    [string]
    $Configuration = "Release"
)
trap {
    Write-Error $_
    Write-Host $_.ScriptStackTrace
    Exit 1
}

# Assumes $PWD is the repo root

Write-Host "Benchmark cases:"
dotnet run ./OpenCCSharp.Benchmarking/OpenCCSharp.Benchmarking.csproj `
    --no-build -c $Configuration `
    -- --list tree

Write-Host
dotnet run ./OpenCCSharp.Benchmarking/OpenCCSharp.Benchmarking.csproj `
    --no-build -c $Configuration `
    -- --filter *

Exit $LASTEXITCODE
