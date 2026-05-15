module EasyBuild.Commands.AutomatedRelease

open Spectre.Console.Cli
open EasyBuild.Workspace
open EasyBuild.Tools.DotNet

type AutomatedReleaseSettings() =
    inherit CommandSettings()

type AutomatedReleaseCommand() =
    inherit Command<AutomatedReleaseSettings>()
    interface ICommandLimiter<AutomatedReleaseSettings>

    override __.Execute(_, _, _) =
        let packAndPush workingDirectory =
            let nupkgPath =
                DotNet.pack (workingDirectory = workingDirectory)

            DotNet.nugetPush nupkgPath

        packAndPush Workspace.src.``Scriptorium.Ink``.``.``
        packAndPush Workspace.src.``Scriptorium.Nib``.``.``
        packAndPush Workspace.src.``Scriptorium.Parchment``.``.``
        packAndPush Workspace.src.``Scriptorium.Quill``.``.``
        packAndPush Workspace.src.``Scriptorium.Nib.Browser``.``.``
        packAndPush Workspace.src.``Scriptorium.Nib.Snapshot``.``.``

        0
