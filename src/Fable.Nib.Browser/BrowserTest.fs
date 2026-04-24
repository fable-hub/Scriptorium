namespace Fable.Nib.Browser

open System.Runtime.CompilerServices
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Fable.Quill
open Glutinum.Playwright

open type Glutinum.Playwright.Exports

module private Internal =

    let awaitPromise (p: Promise<'T>) : Async<'T> =
        Async.FromContinuations(fun (resolve, reject, _) ->
            emitJsStatement (p, resolve, reject) "$0.then($1, $2)"
        )

    let run (headless: bool) (f: Page -> Promise<unit>) : Async<unit> =
        async {
            let opts =
                jsOptions<LaunchOptions>(fun o ->
                    o.headless <- Some headless
                )

            let! browser = awaitPromise (chromium.launch opts)
            let! page = awaitPromise (browser.newPage ())
            let mutable error = None

            try
                do! awaitPromise (f page)
            with ex ->
                error <- Some ex

            try
                do! awaitPromise (page.close ())
            with _ ->
                ()

            try
                do! awaitPromise (browser.close ())
            with _ ->
                ()

            match error with
            | Some ex -> raise ex
            | None -> ()
        }

    /// Creates a headless Chromium browser and page, runs f, then closes both.
    let withPage (f: Page -> Promise<unit>) : Async<unit> = run true f

    /// Like withPage but opens a visible browser window — useful for debugging.
    let withHeadedPage (f: Page -> Promise<unit>) : Async<unit> = run false f


type BrowserTest =

    static member testPage
        (
            name: string,
            body: Page -> Promise<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Test.testAsync (
            name,
            Internal.withPage body,
            ?filePath = filePath,
            ?lineNumber = lineNumber
        )

    static member xtestPage
        (
            name: string,
            body: Page -> Promise<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Test.xtestAsync (
            name,
            Internal.withPage body,
            ?filePath = filePath,
            ?lineNumber = lineNumber
        )

    static member ftestPage
        (
            name: string,
            body: Page -> Promise<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Test.ftestAsync (
            name,
            Internal.withPage body,
            ?filePath = filePath,
            ?lineNumber = lineNumber
        )

    static member dtestPage
        (
            name: string,
            body: Page -> Promise<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Test.ftestAsync (
            name,
            noTimeout,
            Internal.withHeadedPage (fun page ->
                promise {
                    do! page.pause()
                    do! body page
                }
            ),
            ?filePath = filePath,
            ?lineNumber = lineNumber
        )
