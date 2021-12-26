param (
    [string]
    $Configuration = "Release"
)
trap {
    Write-Error $_
    Exit 1
}

# Assumes $PWD is the repo root
dotnet test ./OpenCCSharp.UnitTest/OpenCCSharp.UnitTest.csproj `
    --no-build -c $Configuration `
    --logger "console;verbosity=normal" `
    -- RunConfiguration.TestSessionTimeout=1800000
Exit $LASTEXITCODE
