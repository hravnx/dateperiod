#r "nuget: FsMake"
#r "nuget: DotEnv.Net"
#R "nuget: Nuget.Core"

open FsMake
open System
open System.IO

do dotenv.net.DotEnv.Load()

let configFrom (ctx:MakeContext) =
    if ctx.PipelineName = "release" then "Release" else "Debug"

let dotnet = Cmd.createWithArgs "dotnet"

// ENVIRONMENT --------------------------------------------------------

let isCI = EnvVar.getOptionAs<bool> "GITHUB_ACTIONS" |> Option.contains true
let forcePack = EnvVar.getOptionAs<int> "FORCE_PACK" |> Option.contains 1
let NUGET_APIKEY = EnvVar.getOrFail "NUGET_APIKEY"

// STEPS --------------------------------------------------------------

let cleanAndRestore = Step.create "cleanAndRestore" {
    [   "src/lib/obj";   "src/lib/bin"
        "test/unit/obj"; "test/unit/obj"
        ".pack" 
    ]
    |> Seq.iter (fun x -> if Directory.Exists x then Directory.Delete(x, true))
    do! dotnet ["restore"] |> Cmd.run
}

let build = Step.create "build" {
    let! ctx = Step.context
    let config = configFrom ctx
        
    do! dotnet ["build"; "-c"; config]
        |> Cmd.args ["--no-restore"]
        |> Cmd.argsMaybe isCI ["/p:ContinuousIntegrationBuild=true"]
        |> Cmd.run
}

let unitTest = Step.create "test:unit" {
    let! ctx = Step.context
    let config = configFrom ctx
    
    do! dotnet ["test"; "-c"; config]
        |> Cmd.args ["--no-build"; "test/unit"]
        |> Cmd.run
}

let verifyRepoIsClean = Step.create "git:status" {
    let! statusOut = 
        Cmd.createWithArgs "git" [ "status"; "-s" ]
        |> Cmd.redirectOutput Cmd.Redirect
        |> Cmd.result
        |> Make.map (fun x -> x.Output.Std)
    if not (forcePack || String.IsNullOrWhiteSpace statusOut) then
        do! Step.fail "Repo is not clean, release aborted"
}

let nugetPack = Step.create "nuget:pack" {
    let! ctx = Step.context
    let config = configFrom ctx
    do! dotnet ["pack"; "-c"; config]
        |> Cmd.args ["--no-build"; "-o"; "./.pack"]
        |> Cmd.run
}

let nugetPush = Step.create "nuget:push" {
    let! ctx = Step.context
    let! nugetAPIKey = NUGET_APIKEY
    ctx.Console.WriteLine (Console.warn $"API KEY {nugetAPIKey}")
}

// PIPELINES ----------------------------------------------------------

Pipelines.create {
    do! Pipeline.create "clean" {
        run cleanAndRestore
    }

    do! Pipeline.create "test" {
        run build
        run unitTest
    }

    do! Pipeline.create "release" {
        run verifyRepoIsClean
        run cleanAndRestore
        run build
        run unitTest
        run nugetPack
        run nugetPush
    }

    let! defaultPipeline = Pipeline.create "build" {
        run build
    }

    default_pipeline defaultPipeline
}
|> Pipelines.runWithArgsAndExit fsi.CommandLineArgs
