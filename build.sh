#!/usr/bin/env sh
# Runs the toolchain: build, publish, clean, lint or run.
set -eu

# The physical path, so a project reached through a symbolic link is built, analysed and described by
# one path: an analyzer reads its `.editorconfig` by path, and two spellings of one directory are two
# directories to it.
here=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd -P)
command=${1:-build}

if [ "$#" -gt 0 ]; then
    shift
fi

exec dotnet msbuild "$here/build/Harness.proj" -t:"$command" "$@"
