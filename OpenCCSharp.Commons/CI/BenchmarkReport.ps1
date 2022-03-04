# Assumes $PWD is the repo root
Get-ChildItem BenchmarkDotNet.Artifacts/results/*.md | % {
    Write-Host $_.Name
    Write-Host ("-" * $_.Name.Length)
    Get-Content $_
    Write-Host
}
