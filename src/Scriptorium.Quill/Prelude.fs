namespace Scriptorium.Quill

open Fable.Core
open Fable.Core.JsInterop

module Prelude =

    let cwd: string =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        emitJsExpr
            ()
            """
    process.cwd()
        """
    #endif

    #if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyExpr () "__import__('os').getcwd()"
    #endif

    #if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON)
        System.Environment.CurrentDirectory
    #endif

    module Performance =

        [<Emit("performance.now()")>]
        let now () : float = jsNative

    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    type UniversalStopwatch() =
        let startTime: float = Performance.now ()
        member _.ElapsedMs() : int = int (Performance.now () - startTime)
    #endif

    #if FABLE_COMPILER_PYTHON
    // fable-library-python has no `Stopwatch.StartNew` mapping, so use time.perf_counter (seconds).
    type UniversalStopwatch() =
        let now () : float =
            Fable.Core.PyInterop.emitPyExpr () "__import__('time').perf_counter()"

        let startTime = now ()
        member _.ElapsedMs() : int = int ((now () - startTime) * 1000.0)
    #endif

    #if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON)
    type UniversalStopwatch() =
        let sw = System.Diagnostics.Stopwatch.StartNew()
        member _.ElapsedMs() : int = int sw.ElapsedMilliseconds
    #endif

    let currentPlatform: TargetPlatform =
        // TypeScript compiles to and runs on the JavaScript runtime, so it reports as JavaScript.
        if Compiler.isDotnet then
            DotNet
        elif Compiler.isPython then
            Python
        else
            JavaScript

    let isCI: bool =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        emitJsExpr () "!!process.env['CI']"
    #endif

    #if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyExpr () "__import__('os').environ.get('CI') is not None"
    #endif

    #if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON)
        System.Environment.GetEnvironmentVariable("CI") |> isNull |> not
    #endif
