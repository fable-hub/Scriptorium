namespace Scriptorium.Nib.Browser

open System.Runtime.CompilerServices
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Scriptorium.Quill
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

    let minimumTimeoutMs = 15_000

    // Playwright's `expect` retries for 5000 ms by default before rejecting with its message.
    let browserTimeout (config: TestConfig) : TestConfig =
        match config.TimeoutMs with
        | Some ms when ms < minimumTimeoutMs ->
            { config with
                TimeoutMs = Some minimumTimeoutMs
            }
        | _ -> config

    /// Creates a headless Chromium browser and page, runs f, then closes both.
    let withPage (f: Page -> Promise<unit>) : Async<unit> = run true f

    /// Like withPage but opens a visible browser window - useful for debugging.
    let withHeadedPage (f: Page -> Promise<unit>) : Async<unit> = run false f

type BrowserTest =

    static member testPage
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: Page -> Promise<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Test.testAsync (
            name,
            Internal.browserTimeout >> configurer,
            Internal.withPage body,
            ?filePath = filePath,
            ?lineNumber = lineNumber
        )

    static member testPage
        (
            name: string,
            body: Page -> Promise<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        BrowserTest.testPage (name, id, body, ?filePath = filePath, ?lineNumber = lineNumber)

    static member xtestPage
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: Page -> Promise<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Test.xtestAsync (
            name,
            Internal.browserTimeout >> configurer,
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
        BrowserTest.xtestPage (name, id, body, ?filePath = filePath, ?lineNumber = lineNumber)

    static member ftestPage
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: Page -> Promise<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Test.ftestAsync (
            name,
            Internal.browserTimeout >> configurer,
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
        BrowserTest.ftestPage (name, id, body, ?filePath = filePath, ?lineNumber = lineNumber)

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
