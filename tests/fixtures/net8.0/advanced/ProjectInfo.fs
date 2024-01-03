module ProjectInfo

open Fake.Core


/// Contains relevant information about a project (e.g. version info, project location)
type ProjectInfo = {
    Name: string
    ProjFile: string
    ReleaseNotes: ReleaseNotes.ReleaseNotes Option
    PackageVersionTag: string
    mutable PackagePrereleaseTag: string
    AssemblyVersion: string
    AssemblyInformationalVersion: string
} with 
    /// creates a ProjectInfo given a name, project file path, and release notes file path.
    /// version info is created from the version header of the uppermost release notes entry.
    /// Assembly version is set to X.0.0, where X is the major version from the releas enotes.
    static member create(
        name: string,
        projFile: string,
        releaseNotesPath: string
    ): ProjectInfo = 
        let release = releaseNotesPath |> ReleaseNotes.load
        let stableVersion = release.NugetVersion |> SemVer.parse
        let stableVersionTag = $"{stableVersion.Major}.{stableVersion.Minor}.{stableVersion.Patch}"
        let assemblyVersion = $"{stableVersion.Major}.0.0"
        let assemblyInformationalVersion = stableVersionTag
        {
            Name = name
            ProjFile = projFile
            ReleaseNotes = Some release
            PackagePrereleaseTag = ""
            PackageVersionTag = stableVersionTag
            AssemblyVersion = assemblyVersion
            AssemblyInformationalVersion = assemblyInformationalVersion
        }    
    static member create(
        name: string,
        projFile: string
    ): ProjectInfo = 
        {
            Name = name
            ProjFile = projFile
            ReleaseNotes = None
            PackagePrereleaseTag = ""
            PackageVersionTag = ""
            AssemblyVersion = ""
            AssemblyInformationalVersion = ""
        }

// adapt this to reflect the core project in your repository. The only effect this will have is the version displayed in the docs, as it is currently only possible to have one version displayed there.
let CoreProject = ProjectInfo.create("", "", "")

let projects = 
    [
        // add relative paths (from project root) to your projects here, including individual reslease notes files
        // e.g. ProjectInfo.create("MyProject", "src/MyProject/MyProject.fsproj", "src/MyProject/RELEASE_NOTES.md")
        CoreProject
    ]


let project = "test"

let testProjects = 
    [
        // add relative paths (from project root) to your testprojects here
        // e.g. ProjectInfo.create("MyTestProject", "tests/MyTestProject/MyTestProject.fsproj")
    ]

let solutionFile  = $"{project}.sln"

let configuration = "Release"

let gitOwner = "testowner"

let gitHome = $"https://github.com/{gitOwner}"

let projectRepo = $"https://github.com/{gitOwner}/{project}"

let pkgDir = "pkg"


/// docs are always targeting the version of the core project
let stableDocsVersionTag = CoreProject.PackageVersionTag

/// branch tag is always the version of the core project
let branchTag = CoreProject.PackageVersionTag

/// prerelease suffix used by prerelease buildtasks
let mutable prereleaseSuffix = ""

/// prerelease tag used by prerelease buildtasks
let mutable prereleaseTag = ""

/// mutable switch used to signal that we are building a prerelease version, used in prerelease buildtasks
let mutable isPrerelease = false
