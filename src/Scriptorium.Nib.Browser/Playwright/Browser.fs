namespace rec Glutinum.Playwright

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AllowNullLiteral>]
[<Interface>]
type Browser =
    /// <summary>
    /// Creates a new page in a new browser context. Closing this page will close the context as well.
    ///
    /// This is a convenience API that should only be used for the single-page scenarios and short snippets. Production
    /// code and testing frameworks should explicitly create
    /// [browser.newContext([options])](https://playwright.dev/docs/api/class-browser#browser-new-context) followed by the
    /// [browserContext.newPage()](https://playwright.dev/docs/api/class-browsercontext#browser-context-new-page) to
    /// control their exact life times.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member newPage: ?options: Browser.newPage.options -> JS.Promise<Page>
    abstract member close: ?options: Browser.close.options -> JS.Promise<unit>

module Browser =

    module newPage =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?acceptDownloads: bool,
                ?baseURL: string,
                ?bypassCSP: bool,
                ?clientCertificates: ResizeArray<Browser.newPage.options.clientCertificates>,
                ?colorScheme: Browser.newPage.options.colorScheme,
                ?contrast: Browser.newPage.options.contrast,
                ?deviceScaleFactor: float,
                ?extraHTTPHeaders: Browser.newPage.options.extraHTTPHeaders,
                ?forcedColors: Browser.newPage.options.forcedColors,
                ?geolocation: Browser.newPage.options.geolocation,
                ?hasTouch: bool,
                ?httpCredentials: Browser.newPage.options.httpCredentials,
                ?ignoreHTTPSErrors: bool,
                ?isMobile: bool,
                ?javaScriptEnabled: bool,
                ?locale: string,
                ?logger: Logger,
                ?offline: bool,
                ?permissions: ResizeArray<string>,
                ?proxy: Browser.newPage.options.proxy,
                ?recordHar: Browser.newPage.options.recordHar,
                ?recordVideo: Browser.newPage.options.recordVideo,
                ?reducedMotion: Browser.newPage.options.reducedMotion,
                ?screen: Browser.newPage.options.screen,
                ?serviceWorkers: Browser.newPage.options.serviceWorkers,
                ?storageState: U2<string, Browser.newPage.options.storageState.U2.Case2>,
                ?strictSelectors: bool,
                ?timezoneId: string,
                ?userAgent: string,
                ?videoSize: Browser.newPage.options.videoSize,
                ?videosPath: string,
                ?viewport: Browser.newPage.options.viewport
            ) =

            member val acceptDownloads : bool option = nativeOnly with get, set
            member val baseURL : string option = nativeOnly with get, set
            member val bypassCSP : bool option = nativeOnly with get, set
            member val clientCertificates : ResizeArray<Browser.newPage.options.clientCertificates> option = nativeOnly with get, set
            member val colorScheme : Browser.newPage.options.colorScheme option = nativeOnly with get, set
            member val contrast : Browser.newPage.options.contrast option = nativeOnly with get, set
            member val deviceScaleFactor : float option = nativeOnly with get, set
            member val extraHTTPHeaders : Browser.newPage.options.extraHTTPHeaders option = nativeOnly with get, set
            member val forcedColors : Browser.newPage.options.forcedColors option = nativeOnly with get, set
            member val geolocation : Browser.newPage.options.geolocation option = nativeOnly with get, set
            member val hasTouch : bool option = nativeOnly with get, set
            member val httpCredentials : Browser.newPage.options.httpCredentials option = nativeOnly with get, set
            member val ignoreHTTPSErrors : bool option = nativeOnly with get, set
            member val isMobile : bool option = nativeOnly with get, set
            member val javaScriptEnabled : bool option = nativeOnly with get, set
            member val locale : string option = nativeOnly with get, set
            member val logger : Logger option = nativeOnly with get, set
            member val offline : bool option = nativeOnly with get, set
            member val permissions : ResizeArray<string> option = nativeOnly with get, set
            member val proxy : Browser.newPage.options.proxy option = nativeOnly with get, set
            member val recordHar : Browser.newPage.options.recordHar option = nativeOnly with get, set
            member val recordVideo : Browser.newPage.options.recordVideo option = nativeOnly with get, set
            member val reducedMotion : Browser.newPage.options.reducedMotion option = nativeOnly with get, set
            member val screen : Browser.newPage.options.screen option = nativeOnly with get, set
            member val serviceWorkers : Browser.newPage.options.serviceWorkers option = nativeOnly with get, set
            member val storageState : U2<string, Browser.newPage.options.storageState.U2.Case2> option = nativeOnly with get, set
            member val strictSelectors : bool option = nativeOnly with get, set
            member val timezoneId : string option = nativeOnly with get, set
            member val userAgent : string option = nativeOnly with get, set
            member val videoSize : Browser.newPage.options.videoSize option = nativeOnly with get, set
            member val videosPath : string option = nativeOnly with get, set
            member val viewport : Browser.newPage.options.viewport option = nativeOnly with get, set

        module options =

            [<Global>]
            [<AllowNullLiteral>]
            type clientCertificates
                [<ParamObject; Emit("$0")>]
                (
                    origin: string,
                    ?certPath: string,
                    ?cert: Buffer,
                    ?keyPath: string,
                    ?key: Buffer,
                    ?pfxPath: string,
                    ?pfx: Buffer,
                    ?passphrase: string
                ) =

                member val origin : string = nativeOnly with get, set
                member val certPath : string option = nativeOnly with get, set
                member val cert : Buffer option = nativeOnly with get, set
                member val keyPath : string option = nativeOnly with get, set
                member val key : Buffer option = nativeOnly with get, set
                member val pfxPath : string option = nativeOnly with get, set
                member val pfx : Buffer option = nativeOnly with get, set
                member val passphrase : string option = nativeOnly with get, set

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type colorScheme =
                | light
                | dark
                | ``no-preference``

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type contrast =
                | ``no-preference``
                | more

            [<AllowNullLiteral>]
            [<Interface>]
            type extraHTTPHeaders =
                [<EmitIndexer>]
                abstract member Item: key: string -> string with get, set

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type forcedColors =
                | active
                | none

            [<Global>]
            [<AllowNullLiteral>]
            type geolocation
                [<ParamObject; Emit("$0")>]
                (
                    latitude: float,
                    longitude: float,
                    ?accuracy: float
                ) =

                member val latitude : float = nativeOnly with get, set
                member val longitude : float = nativeOnly with get, set
                member val accuracy : float option = nativeOnly with get, set

            [<Global>]
            [<AllowNullLiteral>]
            type httpCredentials
                [<ParamObject; Emit("$0")>]
                (
                    username: string,
                    password: string,
                    ?origin: string,
                    ?send: Browser.newPage.options.httpCredentials.send
                ) =

                member val username : string = nativeOnly with get, set
                member val password : string = nativeOnly with get, set
                member val origin : string option = nativeOnly with get, set
                member val send : Browser.newPage.options.httpCredentials.send option = nativeOnly with get, set

            [<Global>]
            [<AllowNullLiteral>]
            type proxy
                [<ParamObject; Emit("$0")>]
                (
                    server: string,
                    ?bypass: string,
                    ?username: string,
                    ?password: string
                ) =

                member val server : string = nativeOnly with get, set
                member val bypass : string option = nativeOnly with get, set
                member val username : string option = nativeOnly with get, set
                member val password : string option = nativeOnly with get, set

            [<Global>]
            [<AllowNullLiteral>]
            type recordHar
                [<ParamObject; Emit("$0")>]
                (
                    path: string,
                    ?omitContent: bool,
                    ?content: Browser.newPage.options.recordHar.content,
                    ?mode: Browser.newPage.options.recordHar.mode,
                    ?urlFilter: U2<string, RegExp>
                ) =

                member val path : string = nativeOnly with get, set
                member val omitContent : bool option = nativeOnly with get, set
                member val content : Browser.newPage.options.recordHar.content option = nativeOnly with get, set
                member val mode : Browser.newPage.options.recordHar.mode option = nativeOnly with get, set
                member val urlFilter : U2<string, RegExp> option = nativeOnly with get, set

            [<Global>]
            [<AllowNullLiteral>]
            type recordVideo
                [<ParamObject; Emit("$0")>]
                (
                    ?dir: string,
                    ?size: Browser.newPage.options.recordVideo.size,
                    ?showActions: Browser.newPage.options.recordVideo.showActions
                ) =

                member val dir : string option = nativeOnly with get, set
                member val size : Browser.newPage.options.recordVideo.size option = nativeOnly with get, set
                member val showActions : Browser.newPage.options.recordVideo.showActions option = nativeOnly with get, set

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type reducedMotion =
                | reduce
                | ``no-preference``

            [<Global>]
            [<AllowNullLiteral>]
            type screen
                [<ParamObject; Emit("$0")>]
                (
                    width: float,
                    height: float
                ) =

                member val width : float = nativeOnly with get, set
                member val height : float = nativeOnly with get, set

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type serviceWorkers =
                | allow
                | block

            [<Global>]
            [<AllowNullLiteral>]
            type videoSize
                [<ParamObject; Emit("$0")>]
                (
                    width: float,
                    height: float
                ) =

                member val width : float = nativeOnly with get, set
                member val height : float = nativeOnly with get, set

            [<Global>]
            [<AllowNullLiteral>]
            type viewport
                [<ParamObject; Emit("$0")>]
                (
                    width: float,
                    height: float
                ) =

                member val width : float = nativeOnly with get, set
                member val height : float = nativeOnly with get, set

            module httpCredentials =

                [<RequireQualifiedAccess>]
                [<StringEnum(CaseRules.None)>]
                type send =
                    | unauthorized
                    | always

            module recordHar =

                [<RequireQualifiedAccess>]
                [<StringEnum(CaseRules.None)>]
                type content =
                    | omit
                    | embed
                    | attach

                [<RequireQualifiedAccess>]
                [<StringEnum(CaseRules.None)>]
                type mode =
                    | full
                    | minimal

            module recordVideo =

                [<Global>]
                [<AllowNullLiteral>]
                type size
                    [<ParamObject; Emit("$0")>]
                    (
                        width: float,
                        height: float
                    ) =

                    member val width : float = nativeOnly with get, set
                    member val height : float = nativeOnly with get, set

                [<Global>]
                [<AllowNullLiteral>]
                type showActions
                    [<ParamObject; Emit("$0")>]
                    (
                        ?duration: float,
                        ?position: Browser.newPage.options.recordVideo.showActions.position,
                        ?fontSize: float
                    ) =

                    member val duration : float option = nativeOnly with get, set
                    member val position : Browser.newPage.options.recordVideo.showActions.position option = nativeOnly with get, set
                    member val fontSize : float option = nativeOnly with get, set

                module showActions =

                    [<RequireQualifiedAccess>]
                    [<StringEnum(CaseRules.None)>]
                    type position =
                        | ``top-left``
                        | top
                        | ``top-right``
                        | ``bottom-left``
                        | bottom
                        | ``bottom-right``

            module storageState =

                module U2 =

                    [<Global>]
                    [<AllowNullLiteral>]
                    type Case2
                        [<ParamObject; Emit("$0")>]
                        (
                            cookies: ResizeArray<Browser.newPage.options.storageState.U2.Case2.cookies>,
                            origins: ResizeArray<Browser.newPage.options.storageState.U2.Case2.origins>
                        ) =

                        member val cookies : ResizeArray<Browser.newPage.options.storageState.U2.Case2.cookies> = nativeOnly with get, set
                        member val origins : ResizeArray<Browser.newPage.options.storageState.U2.Case2.origins> = nativeOnly with get, set

                    module Case2 =

                        [<Global>]
                        [<AllowNullLiteral>]
                        type cookies
                            [<ParamObject; Emit("$0")>]
                            (
                                name: string,
                                value: string,
                                domain: string,
                                path: string,
                                expires: float,
                                httpOnly: bool,
                                secure: bool,
                                sameSite: Browser.newPage.options.storageState.U2.Case2.cookies.sameSite
                            ) =

                            member val name : string = nativeOnly with get, set
                            member val value : string = nativeOnly with get, set
                            member val domain : string = nativeOnly with get, set
                            member val path : string = nativeOnly with get, set
                            member val expires : float = nativeOnly with get, set
                            member val httpOnly : bool = nativeOnly with get, set
                            member val secure : bool = nativeOnly with get, set
                            member val sameSite : Browser.newPage.options.storageState.U2.Case2.cookies.sameSite = nativeOnly with get, set

                        [<Global>]
                        [<AllowNullLiteral>]
                        type origins
                            [<ParamObject; Emit("$0")>]
                            (
                                origin: string,
                                localStorage: ResizeArray<Browser.newPage.options.storageState.U2.Case2.origins.localStorage>
                            ) =

                            member val origin : string = nativeOnly with get, set
                            member val localStorage : ResizeArray<Browser.newPage.options.storageState.U2.Case2.origins.localStorage> = nativeOnly with get, set

                        module cookies =

                            [<RequireQualifiedAccess>]
                            [<StringEnum(CaseRules.None)>]
                            type sameSite =
                                | Strict
                                | Lax
                                | None

                        module origins =

                            [<Global>]
                            [<AllowNullLiteral>]
                            type localStorage
                                [<ParamObject; Emit("$0")>]
                                (
                                    name: string,
                                    value: string
                                ) =

                                member val name : string = nativeOnly with get, set
                                member val value : string = nativeOnly with get, set

    module close =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?reason: string
            ) =

            member val reason : string option = nativeOnly with get, set
