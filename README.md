# Rorolala Desktop plugin template

## Introduction

A plugin is one assembly the Desktop discovers in the `plugins` directory beside its program, holding
one type that implements `IRolaPlugin`. This template is a repository of its own holding a C# solution
that builds one: it registers a dock, the menu item that opens it and its own translations. Building it
needs the contract's sources and nothing else from the tree, and it needs no Rust build.

Take a copy and rename it. Do not edit the template in place, since every plugin starts from the same
files.

## Prepare it

Take a copy, lay the contract's sources under the project's own `vendor/`, beside the `bin/` and `obj/`
a build writes, then rename the placeholders.

### 1. Take a copy

Work in a repository of your own. Clone the template and start its history again:

```sh
git clone <template-url> my-plugin
cd my-plugin
rm -rf .git
git init -b main
```

```powershell
git clone <template-url> my-plugin
Set-Location my-plugin
Remove-Item -Recurse -Force .git
git init -b main
```

A checkout of the rorolala tree holds the same files under `dev/tmpls/desktop-plugin`, so copying that
directory gives the same starting point.

### 2. Fetch the sources

The project references two projects from the rorolala tree, the contract in `app/desktop/Contract` and
the translations in `utils/desktop-i18n`. It looks for them under `ExamplePlugin/vendor/rorolala-vcs`.
GitHub serves the tree as a gzipped archive at the URL below. `git archive --remote` is not answered by
GitHub, so this is the archive to use.

Pin `ref` to the commit the Desktop you run was built from. `main` moves, and the contract version the
plugin was built against is checked when the host loads it, so a plugin built against another contract
is not loaded. In a checkout of the tree, `git rev-parse HEAD` names that commit, and the commit must
be one GitHub holds.

```sh
ref=<commit>
mkdir -p ExamplePlugin/vendor/rorolala-vcs
curl -L -o ExamplePlugin/vendor/rorolala-vcs.tar.gz "https://codeload.github.com/rorolala-vcs/rorolala-vcs/tar.gz/$ref"
tar -xzf ExamplePlugin/vendor/rorolala-vcs.tar.gz -C ExamplePlugin/vendor/rorolala-vcs --strip-components=1
rm ExamplePlugin/vendor/rorolala-vcs.tar.gz
```

```powershell
$ref = '<commit>'
New-Item -ItemType Directory -Force ExamplePlugin/vendor/rorolala-vcs | Out-Null
curl.exe -L -o ExamplePlugin/vendor/rorolala-vcs.tar.gz "https://codeload.github.com/rorolala-vcs/rorolala-vcs/tar.gz/$ref"
tar -xzf ExamplePlugin/vendor/rorolala-vcs.tar.gz -C ExamplePlugin/vendor/rorolala-vcs --strip-components=1
Remove-Item ExamplePlugin/vendor/rorolala-vcs.tar.gz
```

To build against a checkout instead, skip this step and pass the checkout's path on every build:
`-p:RorolalaSource=<path>`.

### 3. Rename the placeholders

| What                   | From                                 | To                                             |
| ---------------------- | ------------------------------------ | ---------------------------------------------- |
| Directory and project  | `ExamplePlugin/ExamplePlugin.csproj` | `<Name>/<Name>.csproj`                         |
| Namespace and assembly | `ExamplePlugin`                      | `<Name>`, one word, matched by the file name   |
| Plugin identity        | `rorolala.example`                   | `<vendor>.<plugin>`, lower case, dot separated |
| Dock name id           | `rorolala.example.dock`              | `<vendor>.<plugin>.<dock>`                     |
| Translation prefix     | `rorolala_example`                   | `<vendor>_<plugin>`                            |
| Translation directory  | `i18n/rorolala_example`              | `i18n/<vendor>_<plugin>`                       |

```sh
rg -n 'ExamplePlugin|rorolala\.example|rorolala_example' .
```

Every hit is a place a name is left. The identity and the dock name id are written into the user's
configuration and dock layout, so they must not change once the plugin has been shipped.

## Toolchain

`build.sh` and `build.ps1` run the same five commands. They publish the Desktop program from the
vendored sources, build this plugin, and lay the two together under `run/`, so none of them needs a
rorolala checkout or a Rust build.

```sh
sh build.sh build
sh build.sh run
sh build.sh publish
sh build.sh lint
sh build.sh clean
```

```powershell
.\build.ps1 build
.\build.ps1 run
.\build.ps1 publish
.\build.ps1 lint
.\build.ps1 clean
```

| Command   | What it does                                                                                    |
| --------- | ----------------------------------------------------------------------------------------------- |
| `build`   | publishes the Desktop program into `run/`, builds the plugin, and lays it in `run/plugins`      |
| `run`     | builds, then starts `run/RorolalaDesktop`, which needs a desktop session                        |
| `publish` | writes what a user installs: `publish/ExamplePlugin.dll` and `publish/i18n/`                    |
| `lint`    | rebuilds the plugin with `AnalysisMode=All` and every diagnostic an error                       |
| `clean`   | removes `run/`, `publish/`, `.harness/`, the plugin's build output and the vendored build cache |

The commands are the targets of `build/Harness.proj`; the scripts pass the name to it. A run keeps
its user data under `.harness/`, so it never touches the Desktop a user has installed. The harness
fetches the fonts the program is drawn in and stubs the window icon its project names, since neither
is committed to the tree the sources come from.

### Choosing vendored plugins

`build/Plugins.props` says which of the vendored Desktop plugins this plugin uses. The toolchain
builds and lays each one that is on, and the project references it, so its types are in scope.

| Switch           | Plugin       |
| ---------------- | ------------ |
| `WithGit`        | Git          |
| `WithVcs`        | Rorolala VCS |
| `WithFileSystem` | File System  |
| `WithTerminal`   | Terminal     |

All four depend on the File System plugin. Every switch is off by default, which is a window holding
nothing but this plugin. Both the project and the toolchain read the file, so one edit changes both
what is compiled against and what is laid for the run.

## Install what it publishes

`publish` writes the plugin's assembly and translations, which is the whole of what a user installs.
Copy both into the `plugins` directory beside the Desktop program, and nothing else: the contract and
Avalonia are the host's own, and a second of each is a second type of the same name.

```sh
desktop=<the directory holding RorolalaDesktop>
mkdir -p "$desktop/plugins"
cp publish/ExamplePlugin.dll "$desktop/plugins/"
cp -R publish/i18n "$desktop/plugins/"
```

```powershell
$desktop = '<the directory holding RorolalaDesktop>'
New-Item -ItemType Directory -Force "$desktop/plugins" | Out-Null
Copy-Item publish/ExamplePlugin.dll "$desktop/plugins/"
Copy-Item -Recurse publish/i18n "$desktop/plugins/"
```

A development export of the Desktop is `build/bin/desktop`, so its plugins directory is
`build/bin/desktop/plugins`.

Start the Desktop. The Plugin Manager lists the new plugin, the Window menu gains its dock, and
right-clicking a file offers the context-menu item. A plugin that was not loaded is named with the
reason on its card in the Plugin Manager and in the log.

## What the template holds

| File                                              | What it is                                                               |
| ------------------------------------------------- | ------------------------------------------------------------------------ |
| `.editorconfig`                                   | the lint's floor: formatting, and the one rule a plugin is excused       |
| `.gitignore`                                      | what a copy ignores: the project's `vendor/`, build output, editor files |
| `ExamplePlugin.sln`                               | the solution, holding the one project                                    |
| `build.sh`, `build.ps1`                           | the five toolchain commands                                              |
| `build/Harness.proj`                              | the targets the commands run                                             |
| `build/Plugins.props`                             | which vendored plugins are built, laid and referenced                    |
| `build/desk.ico`                                  | the stand-in window icon, for a tree without one                         |
| `ExamplePlugin/ExamplePlugin.csproj`              | the project, and the two references it compiles against                  |
| `ExamplePlugin/ExamplePlugin.cs`                  | the manifest, the dock registration, the context menu, the i18n          |
| `ExamplePlugin/ExampleDock.cs`                    | the dock view, with one command in its header                            |
| `ExamplePlugin/ExampleView.cs`                    | an Avalonia control, drawn in the look's colours                         |
| `ExamplePlugin/i18n/rorolala_example/strings.yml` | the plugin's words in `en` and `zh-CN`                                   |

`Manifest` states the contract version read from the contract assembly, so rebuilding against another
archive is the only way to change it. `Initialize` is where everything the plugin contributes is
registered, and it runs once, before any window is shown.

## What else the host offers

`IPluginHost` is the whole of what a plugin may reach. The interface and the types it returns are in
`ExamplePlugin/vendor/rorolala-vcs/app/desktop/Contract`, and each member is documented there.

| Service             | What it registers                                            |
| ------------------- | ------------------------------------------------------------ |
| `host.Docks`        | docks, toggle or made anew on every activation               |
| `host.ContextMenus` | items on a file, a directory or empty space                  |
| `host.Menu`         | a top-level menu and its items                               |
| `host.Navigation`   | buttons in the file browser's navigation area                |
| `host.Config`       | settings shown in the Preferences panel                      |
| `host.Icons`        | pictures the plugin draws, by key                            |
| `host.Palette`      | the look's colours as values, for a mark built from a colour |
| `host.Motion`       | the look's durations and distances                           |
| `host.Windows`      | showing a dock in a window of its own                        |
| `host.Files`        | telling, and hearing, that the files changed                 |
| `host.Dialogs`      | asking the user something                                    |
| `host.Log`          | what the plugin wants to say in the log                      |

## Before you ship

- **Match the versions.** The host loads a plugin only when the contract version and the Avalonia
  major and minor versions agree with its own. Rebuild against the commit of the Desktop you target.
- **The identity is permanent.** It is written into the user's configuration, as is every dock name
  id, so changing one orphans them.
- **The template has no picture.** A dock registered without an `IconKey` is listed without a mark.
  To add one, embed a PNG in the assembly and hand it to `host.Icons.Add` in `Initialize`, then name
  the key in the dock registration.
- **Do not carry the host's assemblies.** A plugin that ships `RorolalaDesktop.Contract.dll` or
  Avalonia beside it is carrying a second copy of a type the host already has.
