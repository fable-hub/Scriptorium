module EasyBuild.Main

open Spectre.Console.Cli
open EasyBuild.Commands.Test
open EasyBuild.Commands.Docs
open EasyBuild.Commands.Publish
open EasyBuild.Commands.AutomatedRelease
open EasyBuild.Tools.Husky
open EasyBuild.Tools.Npm

[<EntryPoint>]
let main args =

    Husky.install ()
    Npm.install ()

    let app = CommandApp()

    app.Configure(fun config ->
        config.Settings.ApplicationName <- "./build.sh"

        config
            .AddCommand<DocsCommand>("docs")
            .WithDescription("Command related to the documentation")
        |> ignore

        config
            .AddCommand<PublishCommand>("publish")
            .WithDescription("Publish the documentation to GitHub Pages")
        |> ignore

        config
            .AddCommand<AutomatedReleaseCommand>("automated-release")
            .WithDescription(
                "Automate the release process by packing and pushing the NuGet packages"
            )
        |> ignore

        config
            .AddCommand<TestCommand>("test")
            .WithDescription("Run the tests")
            .WithExample("test")
            .WithExample("test all")
            .WithExample("test quill")
            .WithExample("test --watch")
            .WithExample("test quill js --watch")
        |> ignore
    )

    app.Run(args)
