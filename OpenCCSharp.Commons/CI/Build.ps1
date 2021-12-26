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
dotnet build OpenCCSharp.sln -c $Configuration

$BuildResult = $LASTEXITCODE

Exit $BuildResult
