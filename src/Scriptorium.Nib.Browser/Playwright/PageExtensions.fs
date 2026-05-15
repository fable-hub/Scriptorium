namespace rec Glutinum.Playwright

open Fable.Core
open Fable.Core.JS

// ---------------------------------------------------------------------------
// Bindings missing from the generated Page.fs
// ---------------------------------------------------------------------------

[<Global>]
[<AllowNullLiteral>]
type AddScriptTagOptions
    [<ParamObject; Emit("$0")>]
    (
        ?url: string,
        ?path: string,
        ?content: string
    ) =

    member val url : string option = nativeOnly with get, set
    member val path : string option = nativeOnly with get, set
    member val content : string option = nativeOnly with get, set

[<AutoOpen>]
module PageExtensions =

    type Page with

        /// Evaluates a JavaScript expression or function in the browser context and returns the result.
        [<Emit("$0.evaluate($1)")>]
        member _.evaluate(expression: string) : Promise<obj> = jsNative

        /// Evaluates a JavaScript expression or function in the browser context, passing an argument.
        [<Emit("$0.evaluate($1, $2)")>]
        member _.evaluate(expression: string, arg: 'T) : Promise<obj> = jsNative

        /// Adds a <c>&lt;script></c> tag into the page.
        [<Emit("$0.addScriptTag($1)")>]
        member _.addScriptTag(options: AddScriptTagOptions) : Promise<unit> = jsNative

    type Locator with

        /// Returns a locator matching the given selector within this locator's subtree.
        [<Emit("$0.locator($1)")>]
        member _.locator(selector: string) : Locator = jsNative
