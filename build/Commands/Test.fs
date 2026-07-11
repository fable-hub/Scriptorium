module EasyBuild.Commands.Test

open System.IO
open Spectre.Console.Cli
open SimpleExec
open BlackFox.CommandLine
open EasyBuild.Workspace
open System.ComponentModel
open Microsoft.FSharp.Reflection

type Runtime =
    | DotNet
    | JavaScript
    | TypeScript
    | Python
    | Beam
    | AllRuntime

    static member All =
        FSharpType.GetUnionCases(typeof<Runtime>)
        |> Array.map (fun case -> FSharpValue.MakeUnion(case, [||]) :?> Runtime)
        |> Array.filter (fun du -> du <> AllRuntime)

    member this.ToArg =
        match this with
        | AllRuntime -> "all"
        | DotNet -> "dotnet"
        | JavaScript -> "js"
        | TypeScript -> "ts"
        | Python -> "py"
        | Beam -> "beam"

    static member fromString(value: string) =
        match value.ToLowerInvariant() with
        | "all" -> AllRuntime
        | "dotnet" -> DotNet
        | "javascript"
        | "js" -> JavaScript
        | "typescript"
        | "ts" -> TypeScript
        | "python"
        | "py" -> Python
        | "beam"
        | "erlang" -> Beam
        | _ ->
            failwith
                $"""Unknown runtime value: '%s{value}'

It should be one of the following:
- dotnet
- javascript (or js)
- typescript (or ts)
- python (or py)
- beam (or erlang)
- all
        """

type Project =
    | Scriptorium_Hedgehog
    | Scriptorium_Ink
    | Scriptorium_Nib
    | Scriptorium_Nib_Browser
    | Scriptorium_Nib_Snapshot
    | Scriptorium_Parchment
    | Scriptorium_Quill
    | AllProject

    static member All =
        FSharpType.GetUnionCases(typeof<Project>)
        |> Array.map (fun case -> FSharpValue.MakeUnion(case, [||]) :?> Project)
        |> Array.filter (fun du -> du <> AllProject)

    member this.ToArgs =
        match this with
        | AllProject -> "all"
        | Scriptorium_Hedgehog -> "hedgehog"
        | Scriptorium_Ink -> "ink"
        | Scriptorium_Nib -> "nib"
        | Scriptorium_Nib_Browser -> "nib-browser"
        | Scriptorium_Nib_Snapshot -> "nib-snapshot"
        | Scriptorium_Parchment -> "parchment"
        | Scriptorium_Quill -> "quill"

    static member fromString(value: string) =
        match value.ToLowerInvariant() with
        | "all" -> AllProject
        | "hedgehog" -> Scriptorium_Hedgehog
        | "ink" -> Scriptorium_Ink
        | "nib" -> Scriptorium_Nib
        | "nib-browser" -> Scriptorium_Nib_Browser
        | "nib-snapshot" -> Scriptorium_Nib_Snapshot
        | "parchment" -> Scriptorium_Parchment
        | "quill" -> Scriptorium_Quill
        | _ ->
            failwith
                $"""Unknown project value: '%s{value}'

It should be one of the following:
- ink
- nib
- nib-browser
- nib-snapshot
- parchment
- quill
- hedgehog
        """

    member this.Dir =
        match this with
        | Scriptorium_Hedgehog -> Workspace.tests.``Scriptorium.Hedgehog.Test``.``.``
        | Scriptorium_Ink -> Workspace.tests.``Scriptorium.Ink.Test``.``.``
        | Scriptorium_Nib -> Workspace.tests.``Scriptorium.Nib.Test``.``.``
        | Scriptorium_Nib_Browser -> Workspace.tests.``Scriptorium.Nib.Browser.Test``.``.``
        | Scriptorium_Nib_Snapshot -> Workspace.tests.``Scriptorium.Nib.Snapshot.Test``.``.``
        | Scriptorium_Parchment -> Workspace.tests.``Scriptorium.Parchment.Test``.``.``
        | Scriptorium_Quill -> Workspace.tests.``Scriptorium.Quill.Test``.``.``
        | AllProject -> failwith "All is not a real project, it should be captured before"

    member this.SupportedRuntimes =
        match this with
        | Scriptorium_Hedgehog ->
            [
                DotNet
                JavaScript
                TypeScript
                Python
            ]
        | Scriptorium_Ink ->
            [
                DotNet
                JavaScript
                TypeScript
                Python
                Beam
            ]
        | Scriptorium_Nib ->
            [
                DotNet
                JavaScript
                TypeScript
                Python
                Beam
            ]
        | Scriptorium_Nib_Browser -> [ JavaScript; TypeScript ]
        | Scriptorium_Nib_Snapshot ->
            [
                DotNet
                JavaScript
                TypeScript
                Python
            ]
        | Scriptorium_Parchment ->
            [
                DotNet
                JavaScript
                TypeScript
                Python
                Beam
            ]
        | Scriptorium_Quill ->
            [
                DotNet
                JavaScript
                TypeScript
                Python
                Beam
            ]
        | AllProject -> failwith "All is not a real project, it should be captured before"

let private testProject (project: Project) (runtime: Runtime) (isWatch: bool) =

    let dotnet = "run"

    let javascript =
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw "--runScript"
        |> CmdLine.toString

    let typescript =
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw "--lang"
        |> CmdLine.appendRaw "typescript"
        |> CmdLine.appendRaw "-o"
        |> CmdLine.appendRaw "ts-build"
        |> CmdLine.appendRaw "--run"
        |> CmdLine.appendRaw "npx"
        |> CmdLine.appendRaw "tsx"
        |> CmdLine.appendRaw "ts-build/Main.ts"
        |> CmdLine.toString

    let python =
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw "--lang"
        |> CmdLine.appendRaw "python"
        |> CmdLine.appendRaw "-o"
        |> CmdLine.appendRaw "py-build"
        |> CmdLine.appendRaw "--run"
        |> CmdLine.appendRaw "uv"
        |> CmdLine.appendRaw "run"
        |> CmdLine.appendRaw "--frozen"
        |> CmdLine.appendRaw "python"
        |> CmdLine.appendRaw "py-build/main.py"
        |> CmdLine.toString

    // Ensure Playwright browsers are installed for the browser tests
    match project with
    | Scriptorium_Nib_Browser ->
        Command.Run("npx", "playwright install chromium", workingDirectory = project.Dir)
    | Scriptorium_Hedgehog
    | Scriptorium_Ink
    | Scriptorium_Nib
    | Scriptorium_Nib_Snapshot
    | Scriptorium_Parchment
    | Scriptorium_Quill
    | AllProject -> ()

    if isWatch then
        let dotnetRun (args: string) =
            Command.RunAsync("dotnet", args, workingDirectory = project.Dir)

        failwith "TODO"
    else
        try
            let dotnetRun (args: string) =
                Command.Run("dotnet", args, workingDirectory = project.Dir)

            // BEAM has no single-step `--run`: Fable emits `.erl`, which must be compiled
            // with rebar3 and executed on the Erlang VM.
            let runBeam () =
                let beamBuildDir = Path.Combine(project.Dir, "beam-build")

                let erlLibs =
                    Path.GetFullPath(Path.Combine(beamBuildDir, "_build", "default", "lib"))

                // 1. Transpile F# to Erlang into beam-build/ (also emits fable-library-beam).
                Command.Run(
                    "dotnet",
                    "fable --lang beam -o beam-build",
                    workingDirectory = project.Dir
                )
                // 2. Compile the generated Erlang with rebar3.
                Command.Run("rebar3", "compile", workingDirectory = beamBuildDir)
                // 3. Run the suite on the BEAM. ERL_LIBS makes every compiled app's ebin
                //    (incl. fable-library-beam) loadable. Fable emits the [<EntryPoint>] as
                //    main:main/1 (argv); it runs the suite and halts with the exit code.
                Command.Run(
                    "erl",
                    "-noshell -eval main:main([]) -s init stop",
                    workingDirectory = beamBuildDir,
                    configureEnvironment =
                        (fun (env: System.Collections.Generic.IDictionary<string, string>) ->
                            env.["ERL_LIBS"] <- erlLibs
                        )
                )

            match runtime with
            | DotNet -> dotnetRun dotnet
            | JavaScript -> dotnetRun javascript
            | TypeScript -> dotnetRun typescript
            | Python -> dotnetRun python
            | Beam -> runBeam ()
            | AllRuntime ->
                dotnetRun javascript
                dotnetRun typescript
                dotnetRun python
                runBeam ()
                dotnetRun dotnet
        with :? ExitCodeException ->
            printfn
                $"""Error while testing %A{project} against %A{runtime}

Run the following command to test that combination in isolation:

./build.sh test %s{project.ToArgs} %s{runtime.ToArg}
"""

            reraise ()

type TestSettings() =
    inherit CommandSettings()

    [<CommandArgument(0, "[PROJECT]")>]
    [<Description("""Project to tests

Accepted values:
- ink
- nib
- nib-browser
- nib-snapshot
- parchment
- quill
- hedgehog
- all

Default: all""")>]
    member val Project = "all" with get, set

    [<CommandArgument(1, "[RUNTIME]")>]
    [<Description("""Runtime to test against

Accepted values:
- dotnet
- javascript (or js)
- typescript (or ts)
- python (or py)
- beam (or erlang)
- all

Default: all""")>]
    member val Runtime = "all" with get, set

    [<CommandOption("-w|--watch")>]
    member val IsWatch = false with get, set

type TestCommand() =
    inherit Command<TestSettings>()
    interface ICommandLimiter<TestSettings>

    override __.Execute(_, settings, _) =
        let projectArg = Project.fromString settings.Project
        let runtimeArg = Runtime.fromString settings.Runtime

        match projectArg, runtimeArg with
        | AllProject, AllRuntime ->
            for project in Project.All do
                for runtime in project.SupportedRuntimes do
                    testProject project runtime settings.IsWatch
        | AllProject, runtime ->
            for project in Project.All do
                if List.contains runtime project.SupportedRuntimes then
                    testProject project runtime settings.IsWatch
        | project, AllRuntime ->
            for runtime in project.SupportedRuntimes do
                testProject project runtime settings.IsWatch
        | project, runtime ->
            if List.contains runtime project.SupportedRuntimes then
                testProject project runtime settings.IsWatch
            else
                failwith
                    $"Project '{project}' does not support runtime '{runtime}'. Supported runtimes: {project.SupportedRuntimes}"

        0
