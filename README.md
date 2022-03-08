# BuildProject

A template for scaffolding a modern build pipeline for your project using FAKE and .NET 6

## Installation

```
dotnet new --install BuildProject::*
```

## Usage

### Initializing the template

```
dotnet new BuildProject --git-owner <git-owner> --project-name <project-name> -o <output-directory>
```

where 

- `--git-owner` is the name of the user/org that owns the github repo.
- `--project-name` is the name of both the .sln file and the github repo. leave this unset and adapt manually if those differ
- `-o` specifies the output directory.

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

### Using the build project

**in your project root**, type 

```
dotnet run --project ./<output-path>/build.fsproj
```

you can also pass build targets to run like this:

```
dotnet run --project ./<output-path>/build.fsproj <build-target-to-run>

examples include:

```
dotnet run --project ./<output-path>/build.fsproj release

dotnet run --project ./<output-path>/build.fsproj prerelease

dotnet run --project ./<output-path>/build.fsproj watchdocs
```

please note that the documentation targets assume you have a `docs` folder managed with`fsdocs`.
