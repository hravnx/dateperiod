#r "nuget: FsMake"

open FsMake

let configFrom (ctx:MakeContext) =
    if ctx.PipelineName = "release" then "Release" else "Debug"

let dotnet = Cmd.createWithArgs "dotnet"

// STEPS --------------------------------------------------------------

let restore = Step.create "restore" {
    do! dotnet ["restore"]
        |> Cmd.run
}

let build = Step.create "build" {
    let! ctx = Step.context
    let config = configFrom ctx
    do! dotnet ["build"; "-c"; config]
        |> Cmd.args ["--no-restore"]
        |> Cmd.run
}

let unitTest = Step.create "test:unit" {
    let! ctx = Step.context
    let config = configFrom ctx
    
    do! dotnet ["test"; "-c"; config]
        |> Cmd.args ["--no-build"; "test/unit"]
        |> Cmd.args ["/p:ContinuousIntegrationBuild=true"]
        |> Cmd.run
}

let pack = Step.create "nuget:pack" {
    let! ctx = Step.context
    let config = configFrom ctx
    do! dotnet ["pack"; "-c"; config]
        |> Cmd.args ["--no-build"; "-o"; "./build"; "--include-source"]
        |> Cmd.run
}

// PIPELINES ----------------------------------------------------------

Pipelines.create {
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
            run pack
        }

    default_pipeline build
}
|> Pipelines.runWithArgsAndExit fsi.CommandLineArgs
