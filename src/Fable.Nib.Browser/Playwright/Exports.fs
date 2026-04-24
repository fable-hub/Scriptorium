namespace rec Glutinum.Playwright

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <summary>
    /// This object can be used to launch or connect to Chromium, returning instances of
    /// [Browser](https://playwright.dev/docs/api/class-browser).
    /// </summary>
    [<Import("chromium", "playwright")>]
    static member inline chromium: BrowserType = nativeOnly
    /// <summary>
    /// This object can be used to launch or connect to Firefox, returning instances of
    /// [Browser](https://playwright.dev/docs/api/class-browser).
    /// </summary>
    [<Import("firefox", "playwright")>]
    static member inline firefox: BrowserType = nativeOnly
    // /// <summary>
    // /// Exposes API that can be used for the Web API testing.
    // /// </summary>
    // [<Import("request", "playwright")>]
    // static member inline request: APIRequest = nativeOnly
    // /// <summary>
    // /// Selectors can be used to install custom selector engines. See [extensibility](https://playwright.dev/docs/extensibility) for more
    // /// information.
    // /// </summary>
    // [<Import("selectors", "playwright")>]
    // static member inline selectors: Selectors = nativeOnly
    /// <summary>
    /// This object can be used to launch or connect to WebKit, returning instances of
    /// [Browser](https://playwright.dev/docs/api/class-browser).
    /// </summary>
    [<Import("webkit", "playwright")>]
    static member inline webkit: BrowserType = nativeOnly

    [<Import("expect", "playwright/test")>]
    static member inline expect: Locator -> LocatorAssertions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type BrowserType =
    /// <summary>
    /// Returns the browser instance.
    ///
    /// **Usage**
    ///
    /// You can use
    /// [<c>ignoreDefaultArgs</c>](https://playwright.dev/docs/api/class-browsertype#browser-type-launch-option-ignore-default-args)
    /// to filter out <c>--mute-audio</c> from default arguments:
    ///
    /// <code lang="js">
    /// const browser = await chromium.launch({  // Or 'firefox' or 'webkit'.
    ///   ignoreDefaultArgs: ['--mute-audio']
    /// });
    /// </code>
    ///
    /// > **Chromium-only** Playwright can also be used to control the Google Chrome or Microsoft Edge browsers, but it
    /// works best with the version of Chromium it is bundled with. There is no guarantee it will work with any other
    /// version. Use
    /// [<c>executablePath</c>](https://playwright.dev/docs/api/class-browsertype#browser-type-launch-option-executable-path)
    /// option with extreme caution.
    /// >
    /// > If Google Chrome (rather than Chromium) is preferred, a
    /// [Chrome Canary](https://www.google.com/chrome/browser/canary.html) or
    /// [Dev Channel](https://www.chromium.org/getting-involved/dev-channel) build is suggested.
    /// >
    /// > Stock browsers like Google Chrome and Microsoft Edge are suitable for tests that require proprietary media codecs
    /// for video playback. See
    /// [this article](https://www.howtogeek.com/202825/what%E2%80%99s-the-difference-between-chromium-and-chrome/) for
    /// other differences between Chromium and Chrome.
    /// [This article](https://chromium.googlesource.com/chromium/src/+/lkgr/docs/chromium_browser_vs_google_chrome.md)
    /// describes some differences for Linux users.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member launch: ?options: LaunchOptions -> JS.Promise<Browser>
    /// <summary>
    /// Returns browser name. For example: <c>'chromium'</c>, <c>'webkit'</c> or <c>'firefox'</c>.
    /// </summary>
    abstract member name: unit -> string

[<AllowNullLiteral>]
[<Interface>]
type LaunchOptions =
    /// <summary>
    /// **NOTE** Use custom browser args at your own risk, as some of them may break Playwright functionality.
    ///
    /// Additional arguments to pass to the browser instance. The list of Chromium flags can be found
    /// [here](https://peter.sh/experiments/chromium-command-line-switches/).
    /// </summary>
    abstract member args: ResizeArray<string> option with get, set
    /// <summary>
    /// If specified, artifacts (traces, videos, downloads, HAR files, etc.) are saved into this directory. The directory
    /// is not cleaned up when the browser closes. If not specified, a temporary directory is used and cleaned up when the
    /// browser closes.
    /// </summary>
    abstract member artifactsDir: string option with get, set
    /// <summary>
    /// Browser distribution channel.
    ///
    /// Use "chromium" to [opt in to new headless mode](https://playwright.dev/docs/browsers#chromium-new-headless-mode).
    ///
    /// Use "chrome", "chrome-beta", "chrome-dev", "chrome-canary", "msedge", "msedge-beta", "msedge-dev", or
    /// "msedge-canary" to use branded [Google Chrome and Microsoft Edge](https://playwright.dev/docs/browsers#google-chrome--microsoft-edge).
    /// </summary>
    abstract member channel: string option with get, set
    /// <summary>
    /// Enable Chromium sandboxing. Defaults to <c>false</c>.
    /// </summary>
    abstract member chromiumSandbox: bool option with get, set
    /// <summary>
    /// If specified, accepted downloads are downloaded into this directory. Otherwise, temporary directory is created and
    /// is deleted when browser is closed. In either case, the downloads are deleted when the browser context they were
    /// created in is closed.
    /// </summary>
    abstract member downloadsPath: string option with get, set
    abstract member env: LaunchOptions.env option with get, set
    /// <summary>
    /// Path to a browser executable to run instead of the bundled one. If
    /// [<c>executablePath</c>](https://playwright.dev/docs/api/class-browsertype#browser-type-launch-option-executable-path) is
    /// a relative path, then it is resolved relative to the current working directory. Note that Playwright only works
    /// with the bundled Chromium, Firefox or WebKit, use at your own risk.
    /// </summary>
    abstract member executablePath: string option with get, set
    /// <summary>
    /// Firefox user preferences. Learn more about the Firefox user preferences at
    /// [<c>about:config</c>](https://support.mozilla.org/en-US/kb/about-config-editor-firefox).
    ///
    /// You can also provide a path to a custom [<c>policies.json</c> file](https://mozilla.github.io/policy-templates/) via
    /// <c>PLAYWRIGHT_FIREFOX_POLICIES_JSON</c> environment variable.
    /// </summary>
    abstract member firefoxUserPrefs: LaunchOptions.firefoxUserPrefs option with get, set
    /// <summary>
    /// Close the browser process on SIGHUP. Defaults to <c>true</c>.
    /// </summary>
    abstract member handleSIGHUP: bool option with get, set
    /// <summary>
    /// Close the browser process on Ctrl-C. Defaults to <c>true</c>.
    /// </summary>
    abstract member handleSIGINT: bool option with get, set
    /// <summary>
    /// Close the browser process on SIGTERM. Defaults to <c>true</c>.
    /// </summary>
    abstract member handleSIGTERM: bool option with get, set
    /// <summary>
    /// Whether to run browser in headless mode. More details for
    /// [Chromium](https://developers.google.com/web/updates/2017/04/headless-chrome) and
    /// [Firefox](https://hacks.mozilla.org/2017/12/using-headless-mode-in-firefox/). Defaults to <c>true</c>.
    /// </summary>
    abstract member headless: bool option with get, set
    /// <summary>
    /// If <c>true</c>, Playwright does not pass its own configurations args and only uses the ones from
    /// [<c>args</c>](https://playwright.dev/docs/api/class-browsertype#browser-type-launch-option-args). If an array is given,
    /// then filters out the given default arguments. Dangerous option; use with care. Defaults to <c>false</c>.
    /// </summary>
    abstract member ignoreDefaultArgs: U2<bool, ResizeArray<string>> option with get, set
    // /// <summary>
    // /// Logger sink for Playwright logging.
    // /// </summary>
    // [<Obsolete("The logs received by the logger are incomplete. Please use tracing instead.")>]
    // abstract member logger: Logger option with get, set
    /// <summary>
    /// Network proxy settings.
    /// </summary>
    abstract member proxy: LaunchOptions.proxy option with get, set
    /// <summary>
    /// Slows down Playwright operations by the specified amount of milliseconds. Useful so that you can see what is going
    /// on.
    /// </summary>
    abstract member slowMo: float option with get, set
    /// <summary>
    /// Maximum time in milliseconds to wait for the browser instance to start. Defaults to <c>30000</c> (30 seconds). Pass <c>0</c>
    /// to disable timeout.
    /// </summary>
    abstract member timeout: float option with get, set
    /// <summary>
    /// If specified, traces are saved into this directory.
    /// </summary>
    abstract member tracesDir: string option with get, set


module LaunchOptions =

    [<AllowNullLiteral>]
    [<Interface>]
    type env =
        [<EmitIndexer>]
        abstract member Item: key: string -> string option with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type firefoxUserPrefs =
        [<EmitIndexer>]
        abstract member Item: key: string -> U3<string, float, bool> with get, set

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
