param(
    [Parameter(Mandatory)] [string] $SymbolDirectory,
    [Parameter(Mandatory)] [string] $SourceLinkExecutable,
    [string] $SourceLinkUsername,
    [string] $SourceLinkPassword
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest
Get-ChildItem -LiteralPath $SymbolDirectory -Filter Megaraz.ResultPattern.Infrastructure.pdb -Recurse | ForEach-Object {
    $arguments = @("test", $_.FullName)
    if (-not [string]::IsNullOrWhiteSpace($SourceLinkUsername) -or -not [string]::IsNullOrWhiteSpace($SourceLinkPassword)) {
        if ([string]::IsNullOrWhiteSpace($SourceLinkUsername) -or [string]::IsNullOrWhiteSpace($SourceLinkPassword)) {
            throw "Source Link authentication requires both a username and a password."
        }

        $arguments += @("--use-auth", "Basic", "--username", $SourceLinkUsername, "--password", $SourceLinkPassword)
    }

    & $SourceLinkExecutable @arguments
    if ($LASTEXITCODE -ne 0) { throw "Source Link verification failed for '$($_.FullName)' with exit code $LASTEXITCODE." }
}
