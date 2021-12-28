param (
    [string]
    $Configuration = "Release"
)
trap {
    Write-Error $_
    Write-Host $_.ScriptStackTrace
    Exit 1
}

Write-Host "Benchmark cases:"
dotnet run --project ./OpenCCSharp.Benchmarking/OpenCCSharp.Benchmarking.csproj `
    --no-build -c $Configuration `
    -- --list tree

Write-Host
# Using * as filter may result in no match in Linux.
dotnet run --project ./OpenCCSharp.Benchmarking/OpenCCSharp.Benchmarking.csproj `
    --no-build -c $Configuration `
    -- --filter "*"

Exit $LASTEXITCODE
