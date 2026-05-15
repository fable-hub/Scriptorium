namespace Scriptorium.Parchment.Sinks

open Fable.Core
open Fable.Core.JsInterop
open Scriptorium.Parchment

/// <summary>
/// Sinks that work on all the supported runtimes.
/// </summary>
module Universal =

    let private stdout (msg: string) : unit =
        #if FABLE_COMPILER_JAVASCRIPT
        emitJsStatement msg "(typeof process !== 'undefined' && process.stdout) ? process.stdout.write($0 + '\\n') : console.log($0)"
        #endif
        #if !FABLE_COMPILER_JAVASCRIPT
        System.Console.WriteLine(msg)
        #endif

    let private stderr (msg: string) : unit =
        #if FABLE_COMPILER_JAVASCRIPT
        emitJsStatement msg "(typeof process !== 'undefined' && process.stderr) ? process.stderr.write($0 + '\\n') : console.warn($0)"
        #endif
        #if !FABLE_COMPILER_JAVASCRIPT
        System.Console.Error.WriteLine(msg)
        #endif

    /// <summary>
    /// A sink that writes Error/Warning to stderr and everything else to stdout.
    /// On Node.js uses <c>process.stdout/stderr.write</c>; falls back to <c>console.*</c> in browsers.
    /// </summary>
    let console () : Sink =
        fun severity msg ->
            match severity with
            | Severity.Error -> stderr msg
            | Severity.Warning -> stderr msg
            | Severity.Debug
            | Severity.Info
            | Severity.Verbose
            | Severity.Silly -> stdout msg
