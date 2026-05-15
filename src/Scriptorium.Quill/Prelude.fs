namespace Scriptorium.Quill

open Fable.Core
open Fable.Core.JsInterop

module Prelude =

    let cwd: string =
    #if FABLE_COMPILER_JAVASCRIPT
        emitJsExpr
            ()
            """
    process.cwd()
        """
    #endif

    #if !FABLE_COMPILER_JAVASCRIPT
        System.Environment.CurrentDirectory
    #endif

    module Performance =

        [<Emit("performance.now()")>]
        let now () : float = jsNative

    #if FABLE_COMPILER_JAVASCRIPT
    type UniversalStopwatch() =
        let startTime: float = Performance.now ()
        member _.ElapsedMs() : int = int (Performance.now () - startTime)
    #endif

    #if !FABLE_COMPILER_JAVASCRIPT
    type UniversalStopwatch() =
        let sw = System.Diagnostics.Stopwatch.StartNew()
        member _.ElapsedMs() : int = int sw.ElapsedMilliseconds
    #endif

    let currentPlatform: TargetPlatform =
    #if FABLE_COMPILER_JAVASCRIPT
        JavaScript
    #endif

    #if !FABLE_COMPILER_JAVASCRIPT
        DotNet
    #endif

    let isCI: bool =
    #if FABLE_COMPILER_JAVASCRIPT
        emitJsExpr () "!!process.env['CI']"
    #endif

    #if !FABLE_COMPILER_JAVASCRIPT
        System.Environment.GetEnvironmentVariable("CI") |> isNull |> not
    #endif
