module DocumentationTasks

open Helpers
open ProjectInfo
open BasicTasks

open BlackFox.Fake

#if (individual-package-versions)

let buildDocs =
    BuildTask.create "BuildDocs" [ build ] {
        printfn "building docs with stable version %s" stableDocsVersionTag

        runDotNet
            (sprintf
                "fsdocs build --eval --clean --properties Configuration=Release --parameters fsdocs-package-version %s"
                stableDocsVersionTag)
            "./"
    }

let buildDocsPrerelease =
    BuildTask.create "BuildDocsPrerelease" [ setPrereleaseTag; build ] {
        printfn "building docs with prerelease version %s" prereleaseTag

        runDotNet
            (sprintf
                "fsdocs build --eval --clean --properties Configuration=Release --parameters fsdocs-package-version %s"
                prereleaseTag)
            "./"
    }

let watchDocs =
    BuildTask.create "WatchDocs" [ build ] {
        printfn "watching docs with stable version %s" stableDocsVersionTag

        runDotNet
            (sprintf
                "fsdocs watch --eval --clean --properties Configuration=Release --parameters fsdocs-package-version %s"
                stableDocsVersionTag)
            "./"
    }

let watchDocsPrerelease =
    BuildTask.create "WatchDocsPrerelease" [ setPrereleaseTag; build ] {
        printfn "watching docs with prerelease version %s" prereleaseTag

        runDotNet
            (sprintf
                "fsdocs watch --eval --clean --properties Configuration=Release --parameters fsdocs-package-version %s"
                prereleaseTag)
            "./"
    }

#else

let buildDocs = BuildTask.create "BuildDocs" [buildSolution] {
    printfn "building docs with stable version %s" stableVersionTag
    runDotNet 
        (sprintf "fsdocs build --eval --clean --properties Configuration=%s --parameters fsdocs-package-version %s" configuration stableVersionTag)
        "./"
}

let buildDocsPrerelease = BuildTask.create "BuildDocsPrerelease" [setPrereleaseTag; buildSolution] {
    printfn "building docs with prerelease version %s" prereleaseTag
    runDotNet 
        (sprintf "fsdocs build --eval --clean --properties Configuration=%s --parameters fsdocs-package-version %s" configuration prereleaseTag)
        "./"
}

let watchDocs = BuildTask.create "WatchDocs" [buildSolution] {
    printfn "watching docs with stable version %s" stableVersionTag
    runDotNet 
        (sprintf "fsdocs watch --eval --clean --properties Configuration=%s --parameters fsdocs-package-version %s" configuration stableVersionTag)
        "./"
}

let watchDocsPrerelease = BuildTask.create "WatchDocsPrerelease" [setPrereleaseTag; buildSolution] {
    printfn "watching docs with prerelease version %s" prereleaseTag
    runDotNet 
        (sprintf "fsdocs watch --eval --clean --properties Configuration=%s --parameters fsdocs-package-version %s" configuration prereleaseTag)
        "./"
}
#endif