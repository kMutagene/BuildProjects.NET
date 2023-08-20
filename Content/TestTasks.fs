module TestTasks

open BlackFox.Fake
open Fake.DotNet

open ProjectInfo
open BasicTasks

#if (individuaPackageVersions)

let buildTests = 
    BuildTask.create "BuildTests" [clean; build] {
        testProjects
        |> List.iter (fun pInfo ->
            let proj = pInfo.ProjFile
            proj
            |> DotNet.build (fun p ->
                p
                // Use this if you want to speed up your build. Especially helpful in large projects
                // Ensure that the order in your project list is correct (e.g. projects that are depended on are built first)
                //|> DotNet.Options.withCustomParams (Some "--no-dependencies")
            )
        )
    }

#endif

let runTests = BuildTask.create "RunTests" [clean; build] {
    testProjects
    |> Seq.iter (fun testProject ->
        Fake.DotNet.DotNet.test(fun testParams ->
            {
                testParams with
                    Logger = Some "console;verbosity=detailed"
                    Configuration = DotNet.BuildConfiguration.fromString configuration
                    NoBuild = true
            }
        ) testProject
    )
}