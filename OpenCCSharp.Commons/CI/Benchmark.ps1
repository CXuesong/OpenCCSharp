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
if ($IsLinux) {
    # Restore permission
    chmod a+x ./OpenCCSharp.Benchmarking/bin/*/*/OpenCCSharp.Benchmarking
}

Write-Host "Benchmark cases:"
dotnet run --project ./OpenCCSharp.Benchmarking/OpenCCSharp.Benchmarking.csproj `
    --no-build -c $Configuration `
    -- --list tree

Write-Host
dotnet run --project ./OpenCCSharp.Benchmarking/OpenCCSharp.Benchmarking.csproj `
    --no-build -c $Configuration `
    -- --filter *

Exit $LASTEXITCODE
