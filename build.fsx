#r "nuget: FsMake"
#r "nuget: DotEnv.Net"

open FsMake
open System
open System.IO

do dotenv.net.DotEnv.Load()

let configFrom (ctx:MakeContext) =
    if ctx.PipelineName = "release" then "Release" else "Debug"

let dotnet = Cmd.createWithArgs "dotnet"

// ENVIRONMENT --------------------------------------------------------

let isCI = EnvVar.getOptionAs<bool> "GITHUB_ACTIONS" |> Option.contains true
let NUGET_APIKEY = EnvVar.getOrFail "NUGET_APIKEY"

// STEPS --------------------------------------------------------------

let cleanBuildOutput () =
    [   "src/lib/obj";   "src/lib/bin"
        "test/unit/obj"; "test/unit/obj"
        ".pack" 
    ]
    |> Seq.iter (fun x -> if Directory.Exists x then Directory.Delete(x, true))

let clean = Step.create "clean" {
    cleanBuildOutput ()
}

let restore = Step.create "restore" {
    do! dotnet ["restore"]
        |> Cmd.run
}

let build = Step.create "build" {
    let! ctx = Step.context
    let config = configFrom ctx
    if config = "Release" then
        cleanBuildOutput ()
        do! dotnet ["restore"]
            |> Cmd.run
        
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

let checkClean = Step.create "git:check" {
    let! statusOut = 
        Cmd.createWithArgs "git" [ "status"; "-s" ]
        |> Cmd.redirectOutput Cmd.Redirect
        |> Cmd.result
        |> Make.map (fun x -> x.Output.Std)
    if not (isCI || String.IsNullOrWhiteSpace statusOut) then
        do! Step.fail "Repo is not clean, release aborted"
}


let pack = Step.create "nuget:pack" {
    let! ctx = Step.context
    let config = configFrom ctx
    do! dotnet ["pack"; "-c"; config]
        |> Cmd.args ["--no-build"; "-o"; "./.pack"]
        |> Cmd.run
}

let push = Step.create "nuget:push" {
    let! ctx = Step.context
    let! nugetAPIKey = NUGET_APIKEY
    ctx.Console.WriteLine (Console.warn $"API KEY {nugetAPIKey}")
}

// PIPELINES ----------------------------------------------------------

Pipelines.create {
    do! 
        Pipeline.create "clean" {
            run clean
        }

    let! build =
        Pipeline.create "build" {
            run restore
            run build
        }

    let! test = 
        Pipeline.createFrom build "test" {
            run unitTest
        }

    do!
        Pipeline.createFrom test "release" {
            run checkClean
            run pack
            run push
        }

    default_pipeline build
}
|> Pipelines.runWithArgsAndExit fsi.CommandLineArgs
