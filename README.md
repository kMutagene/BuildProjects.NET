# BuildProject

A template for scaffolding a modern build pipeline for your project using FAKE and .NET 6/7

- [BuildProject](#buildproject)
  - [Installation](#installation)
  - [Usage](#usage)
    - [Template Parameters](#template-parameters)
    - [Basic use case](#basic-use-case)
    - [Adding test projects](#adding-test-projects)
    - [Using the build project](#using-the-build-project)
    - [Scripting the build execution](#scripting-the-build-execution)
    - [Setting up individual project versions and release notes](#setting-up-individual-project-versions-and-release-notes)

## Installation

```
dotnet new --install BuildProject::*
```

## Usage

### Template Parameters

```
BuildProject (F#)
Author: Kevin Schneider

Usage:
  dotnet new BuildProject [options] [template options]

Options:
  -n, --name <name>       The name for the output being created. If no name is specified, the name of the output
                          directory is used.
  -o, --output <output>   Location to place the generated output.
  --dry-run               Displays a summary of what would happen if the given command line were run if it would result
                          in a template creation.
  --force                 Forces content to be generated even if it would change existing files.
  --no-update-check       Disables checking for the template package updates when instantiating a template.
  --project <project>     The project that should be used for context evaluation.
  -lang, --language <F#>  Specifies the template language to instantiate.
  --type <project>        Specifies the template type to instantiate.

Template options:
  -pn, --project-name <project-name>          The name of the project. usually equal to the repo anme on github and the
                                              .sln file to build. If not, customize manually.
                                              Type: string
                                              Default: TODO: set PROJECTNAME
  -go, --git-owner <git-owner>                The name of the organization or github user that owns the github repo
                                              Type: string
                                              Default: TODO: set GITOWNER
  -tf, --target-framework <target-framework>  The target framework of the build project (net6.0 or net7.0). default is
                                              net6.0
                                              Type: string
                                              Default: net6.0
  -ipv, --individual-package-versions         If set, the build project will support individual package versions and
                                              release notes per project.
                                              Type: bool
                                              Default: false
```

### Basic use case

```
dotnet new BuildProject -go <git-owner> -pn <project-name> -o <output-directory>
```

where 

- `<git-owner>` is the name of the user/org that owns the github repo.
- `<project-name>` is the name of both the .sln file and the github repo. leave this unset and adapt manually if those differ
- `<output-directory>` specifies the output directory. I will usually use a `build` folder in the root of my repository.

The template will generate the following files:

```
build.fsproj
ProjectInfo.fs
Helpers.fs
MessagePrompts.fs
BasicTasks.fs
TestTasks.fs
PackageTasks.fs
DocumentationTasks.fs
ReleaseTasks.fs
ReleaseNotesTasks.fs
Build.fs
```

where the only dynamic content is located in `ProjectInfo.fs`, which might be the content you want to adapt manually if you did not set it via the flags shown above:


```fsharp
let testProjects = 
    [
        // add relative paths (from project root) to your testprojects here
    ]

let project = "{PROJECTNAME}"

let solutionFile  = $"{project}.sln"

let gitOwner = "{GITOWNER}"

let gitHome = $"https://github.com/{gitOwner}"

let projectRepo = $"https://github.com/{gitOwner}/{project}"
```

note that testProjects need to be set manually.

### Adding test projects

I usually use the [Expecto template](https://github.com/MNie/Expecto.Template) to initialize a test project in a `tests` folder in the repository root like this:

```
C:.
├───build
│   └───<build project content initialized by this template>
├───src
│   └───<project-name>
└───tests
    └───testproject
```

to make use of the test build tasks provided by this template, you have to add the test projects to run inside `ProjectInfo.fs`:

```fsharp
let testProjects = 
    [
        "tests/testproject/testproject.fsproj"
    ]
```

### Using the build project

**in your project root**, type 

```
dotnet run --project ./<output-path>/build.fsproj
```

you can also pass build targets to run like this:

```
dotnet run --project ./<output-path>/build.fsproj <build-target-to-run>
```

examples include:

```
dotnet run --project ./<output-path>/build.fsproj runtests

dotnet run --project ./<output-path>/build.fsproj prerelease

dotnet run --project ./<output-path>/build.fsproj watchdocs
```

please note that the documentation targets assume you have a `docs` folder managed with`fsdocs`.
    
### Scripting the build execution
    
in your project root, you can create these scripts so you have to type less:

- `build.cmd`:
    
    ```
    @echo off
    cls

    dotnet run --project ./build/build.fsproj %*
    ```
    
 - `build.sh` (make executable with `chmod u+x`):
    
    ```
    #!/usr/bin/env bash

    set -eu
    set -o pipefail

    dotnet run --project ./build/build.fsproj "$@"
    ```   

### Setting up individual project versions and release notes

Since v3.0.0, the template supports individual project versions and release notes per project.

This is helpful in a monorepo that contains projects that have decoupled versions, instead of having the same version for all projects per release..

For an example of a larger project that uses this kind of set up, have a look at the [build pipeline of Plotly.NET](https://github.com/plotly/Plotly.NET/tree/dev/build)

To enable this feature, use the `--individual-package-versions` flag when creating the template.

The rest of the setup is quite similar to the basic usecase, with the difference that you have to specify both projects and testprojects in `ProjectInfo.fs` using the `ProjectInfo` type.

```fsharp
let projects = 
    [
        // add relative paths (from project root) to your projects here, including individual reslease notes files
        // e.g. ProjectInfo.create("MyProject", "src/MyProject/MyProject.fsproj", "src/MyProject/RELEASE_NOTES.md") // a project with individual release notes
    ]

let testProjects = 
    [
        // add relative paths (from project root) to your testprojects here
        // e.g. ProjectInfo.create("MyTestProject", "tests/MyTestProject/MyTestProject.fsproj") // test projects do not have release notes.
    ]
```
