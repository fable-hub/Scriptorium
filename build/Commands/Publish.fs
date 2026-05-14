module EasyBuild.Commands.Publish

open Spectre.Console.Cli
open EasyBuild.Workspace
open EasyBuild.Tools.GhPages
open EasyBuild.Commands.Docs

type PublishSettings() =
    inherit CommandSettings()

type PublishCommand() =
    inherit Command<PublishSettings>()
    interface ICommandLimiter<PublishSettings>

    override __.Execute(_, _, ct) =
        DocsCommand().Execute(null, DocsSettings(IsWatch = false), ct) |> ignore

        GhPages.run (dist = "dist", nojekyll = true, workingDirectory = VirtualWorkspace.docs.``.``)

        0
