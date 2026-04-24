module EasyBuild.Commands.Test

open Spectre.Console.Cli
open SimpleExec
open BlackFox.CommandLine
open System.IO
open EasyBuild.Workspace
open System.ComponentModel
open Microsoft.FSharp.Reflection

type Runtime =
    | DotNet
    | JavaScript
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

    static member fromString(value: string) =
        match value.ToLowerInvariant() with
        | "all" -> AllRuntime
        | "dotnet" -> DotNet
        | "javascript"
        | "js" -> JavaScript
        | _ ->
            failwith
                $"""Unknown runtime value: '%s{value}'

It should be one of the following:
- dotnet
- javascript
- js
        """

type Project =
    | Fable_Ink
    | Fable_Nib
    | Fable_Nib_Dom
    | Fable_Nib_Snapshot
    | Fable_Parchment
    | Fable_Quill
    | AllProject

    static member All =
        FSharpType.GetUnionCases(typeof<Project>)
        |> Array.map (fun case -> FSharpValue.MakeUnion(case, [||]) :?> Project)
        |> Array.filter (fun du -> du <> AllProject)

    member this.ToArgs =
        match this with
        | AllProject -> "all"
        | Fable_Ink -> "ink"
        | Fable_Nib -> "nib"
        | Fable_Nib_Dom -> "nib-dom"
        | Fable_Nib_Snapshot -> "nib-snapshot"
        | Fable_Parchment -> "parchment"
        | Fable_Quill -> "quill"

    static member fromString(value: string) =
        match value.ToLowerInvariant() with
        | "all" -> AllProject
        | "ink" -> Fable_Ink
        | "nib" -> Fable_Nib
        | "nib-dom" -> Fable_Nib_Dom
        | "nib-snapshot" -> Fable_Nib_Snapshot
        | "parchment" -> Fable_Parchment
        | "quill" -> Fable_Quill
        | _ ->
            failwith
                $"""Unknown project value: '%s{value}'

It should be one of the following:
- ink
- nib
- nib-dom
- nib-snapshot
- parchment
- quill
        """

    member this.Dir =
        match this with
        | Fable_Ink -> Workspace.tests.``Fable.Ink.Test``.``.``
        | Fable_Nib -> Workspace.tests.``Fable.Nib.Test``.``.``
        | Fable_Nib_Dom -> Workspace.tests.``Fable.Nib.Browser.Test``.``.``
        | Fable_Nib_Snapshot -> Workspace.tests.``Fable.Nib.Snapshot.Test``.``.``
        | Fable_Parchment -> Workspace.tests.``Fable.Parchment.Test``.``.``
        | Fable_Quill -> Workspace.tests.``Fable.Quill.Test``.``.``
        | AllProject -> failwith "All is not a real project, it should be captured before"

    member this.SupportedRuntimes =
        match this with
        | Fable_Ink ->
            [
                DotNet
                JavaScript
            ]
        | Fable_Nib ->
            [
                DotNet
                JavaScript
            ]
        | Fable_Nib_Dom -> [ JavaScript ]
        | Fable_Nib_Snapshot ->
            [
                DotNet
                JavaScript
            ]
        | Fable_Parchment ->
            [
                DotNet
                JavaScript
            ]
        | Fable_Quill ->
            [
                DotNet
                JavaScript
            ]
        | AllProject -> failwith "All is not a real project, it should be captured before"

let private testProject (project: Project) (runtime: Runtime) (isWatch: bool) =

    let dotnet = "run"

    let javascript =
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw "--runScript"
        |> CmdLine.toString

    // Ensure Playwright browsers are installed for the DOM tests
    match project with
    | Fable_Nib_Dom ->
        Command.Run("npx", "playwright install chromium", workingDirectory = project.Dir)
    | Fable_Ink
    | Fable_Nib
    | Fable_Nib_Snapshot
    | Fable_Parchment
    | Fable_Quill
    | AllProject -> ()

    if isWatch then
        let dotnetRun (args: string) =
            Command.RunAsync("dotnet", args, workingDirectory = project.Dir)

        failwith "TODO"
    else
        try
            let dotnetRun (args: string) =
                Command.Run("dotnet", args, workingDirectory = project.Dir)

            match runtime with
            | DotNet -> dotnetRun dotnet
            | JavaScript -> dotnetRun javascript
            | AllRuntime ->
                dotnetRun javascript
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
- nib-dom
- nib-snapshot
- parchment
- quill
- all

Default: all""")>]
    member val Project = "all" with get, set

    [<CommandArgument(1, "[RUNTIME]")>]
    [<Description("""Runtime to test against

Accepted values:
- dotnet
- javascript (or js)
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
