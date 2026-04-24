module EasyBuild.Commands.Docs

open Spectre.Console.Cli
open SimpleExec
open BlackFox.CommandLine
open System.IO
open EasyBuild.Workspace

type DocsSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    member val IsWatch = false with get, set

type DocsCommand() =
    inherit Command<DocsSettings>()
    interface ICommandLimiter<DocsSettings>

    override __.Execute(_, settings, _) =
        if settings.IsWatch then
            Command.RunAsync("npx", "astro dev", workingDirectory = Workspace.docs.``.``)
            |> Async.AwaitTask
            |> Async.RunSynchronously

        else

            Command.Run("npx", "astro build", workingDirectory = Workspace.docs.``.``)

        0
