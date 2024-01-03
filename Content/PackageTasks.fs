module PackageTasks

open ProjectInfo

open MessagePrompts
open BasicTasks
open TestTasks

open BlackFox.Fake
open Fake.Core
open Fake.DotNet
open Fake.IO.Globbing.Operators

#if (individual-package-versions)

let pack = BuildTask.create "Pack" [ clean; build; runTests ] {
    projects
    |> List.iter (fun pInfo ->
        if promptYesNo $"creating stable package for {pInfo.Name}{System.Environment.NewLine}\tpackage version: {pInfo.PackageVersionTag}{System.Environment.NewLine}\tassembly version: {pInfo.AssemblyVersion}{System.Environment.NewLine}\tassembly informational version: {pInfo.AssemblyInformationalVersion}{System.Environment.NewLine} OK?" then
            pInfo.ProjFile
            |> Fake.DotNet.DotNet.pack (fun p ->
                let msBuildParams =
                    match pInfo.ReleaseNotes with
                    | Some r ->
                        { p.MSBuildParams with
                            Properties =
                                ([
                                    "Version",pInfo.PackageVersionTag
                                    "AssemblyVersion", pInfo.AssemblyVersion
                                    "AssemblyInformationalVersion", pInfo.AssemblyVersion
                                    "PackageReleaseNotes",  (r.Notes |> String.concat "\r\n")
                                    "TargetsForTfmSpecificContentInPackage", "" //https://github.com/dotnet/fsharp/issues/12320
                                    ]
                                    @ p.MSBuildParams.Properties)
                        }
                    | _ ->
                        { p.MSBuildParams with
                            Properties =
                                ([
                                    "Version",pInfo.PackageVersionTag
                                    "AssemblyVersion", pInfo.AssemblyVersion
                                    "AssemblyInformationalVersion", pInfo.AssemblyVersion
                                    "TargetsForTfmSpecificContentInPackage", "" //https://github.com/dotnet/fsharp/issues/12320
                                    ]
                                    @ p.MSBuildParams.Properties)
                        }
                        

                { p with
                    MSBuildParams = msBuildParams
                    OutputPath = Some pkgDir
                    NoBuild = true
                }
                |> DotNet.Options.withCustomParams (Some "--no-dependencies")
            )
        else
            failwith "aborted"
        )
    }

let packPrerelease =
    BuildTask.create
        "PackPrerelease"
        [
            clean
            build
            runTests
        ] {
        projects
        |> List.iter (fun pInfo ->
            printfn $"Please enter pre-release package suffix for {pInfo.Name}"
            let prereleaseSuffix = System.Console.ReadLine()
            pInfo.PackagePrereleaseTag <- sprintf "%s-%s" pInfo.PackageVersionTag prereleaseSuffix
            if promptYesNo $"creating prerelease package for {pInfo.Name}{System.Environment.NewLine}\tpackage version: {pInfo.PackagePrereleaseTag}{System.Environment.NewLine}\tassembly version: {pInfo.AssemblyVersion}{System.Environment.NewLine}\tassembly informational version: {pInfo.AssemblyInformationalVersion}{System.Environment.NewLine} OK?" then
                pInfo.ProjFile
                |> Fake.DotNet.DotNet.pack (fun p ->
                    let msBuildParams =
                        match pInfo.ReleaseNotes with
                        | Some r ->
                            { p.MSBuildParams with
                                Properties =
                                    ([
                                        "Version",pInfo.PackagePrereleaseTag
                                        "AssemblyVersion", pInfo.AssemblyVersion
                                        "InformationalVersion", pInfo.AssemblyInformationalVersion
                                        "PackageReleaseNotes",  (r.Notes |> String.concat "\r\n")
                                        "TargetsForTfmSpecificContentInPackage", "" //https://github.com/dotnet/fsharp/issues/12320
                                        ])
                            }
                        | _ -> 
                            { p.MSBuildParams with
                                Properties =
                                    ([
                                        "Version",pInfo.PackagePrereleaseTag
                                        "AssemblyVersion", pInfo.AssemblyVersion
                                        "InformationalVersion", pInfo.AssemblyInformationalVersion
                                        "TargetsForTfmSpecificContentInPackage", "" //https://github.com/dotnet/fsharp/issues/12320
                                        ])
                            }

                    { p with
                        VersionSuffix = Some prereleaseSuffix
                        OutputPath = Some pkgDir
                        MSBuildParams = msBuildParams
                        NoBuild = true
                    }
                    |> DotNet.Options.withCustomParams (Some "--no-dependencies")
                )
            else
                failwith "aborted"
        )
    }

#else
open System.Text.RegularExpressions

/// https://github.com/Freymaurer/Fake.Extensions.Release#release-notes-in-nuget
let private replaceCommitLink input = 
    let commitLinkPattern = @"\[\[#[a-z0-9]*\]\(.*\)\] "
    Regex.Replace(input,commitLinkPattern,"")

let pack = BuildTask.create "Pack" [clean; buildSolution; runTests] {
    if promptYesNo (sprintf "creating stable package with version %s OK?" stableVersionTag ) 
        then
            !! "src/**/*.*proj"
            -- "src/bin/*"
            |> Seq.iter (Fake.DotNet.DotNet.pack (fun p ->
                let msBuildParams =
                    {p.MSBuildParams with 
                        Properties = ([
                            "Version",stableVersionTag
                            "PackageReleaseNotes",  (release.Notes |> List.map replaceCommitLink |> String.concat "\r\n" )
                        ] @ p.MSBuildParams.Properties)
                        DisableInternalBinLog = true
                    }
                {
                    p with 
                        MSBuildParams = msBuildParams
                        OutputPath = Some pkgDir
                }
#if ( target-framework == "net8.0" )
                |> DotNet.Options.withCustomParams (Some "-tl")
#endif
            ))
    else failwith "aborted"
}

let packPrerelease = BuildTask.create "PackPrerelease" [setPrereleaseTag; clean; buildSolution; runTests] {
    if promptYesNo (sprintf "package tag will be %s OK?" prereleaseTag )
        then 
            !! "src/**/*.*proj"
            -- "src/bin/*"
            |> Seq.iter (Fake.DotNet.DotNet.pack (fun p ->
                let msBuildParams =
                    {p.MSBuildParams with 
                        Properties = ([
                            "Version", prereleaseTag
                            "PackageReleaseNotes",  (release.Notes |> List.map replaceCommitLink  |> String.toLines )
                        ] @ p.MSBuildParams.Properties)
                        DisableInternalBinLog = true
                    }
                {
                    p with 
                        VersionSuffix = Some prereleaseSuffix
                        OutputPath = Some pkgDir
                        MSBuildParams = msBuildParams
                }
#if ( target-framework == "net8.0" )
                |> DotNet.Options.withCustomParams (Some "-tl")
#endif
            ))
    else
        failwith "aborted"
}
#endif