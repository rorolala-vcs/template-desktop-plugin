# Runs the toolchain: build, publish, clean, lint or run.
param(
    [Parameter(Position = 0)]
    [string] $Command = 'build',
    [Parameter(ValueFromRemainingArguments)]
    [string[]] $Rest
)

dotnet msbuild (Join-Path $PSScriptRoot 'build/Harness.proj') "-t:$Command" @Rest
exit $LASTEXITCODE
