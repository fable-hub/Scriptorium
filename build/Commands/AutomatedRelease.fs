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

        packAndPush Workspace.src.``Fable.Ink``.``.``
        packAndPush Workspace.src.``Fable.Nib``.``.``
        packAndPush Workspace.src.``Fable.Parchment``.``.``
        packAndPush Workspace.src.``Fable.Quill``.``.``
        packAndPush Workspace.src.``Fable.Nib.Browser``.``.``
        packAndPush Workspace.src.``Fable.Nib.Snapshot``.``.``

        0
