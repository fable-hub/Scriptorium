namespace Scriptorium.Parchment.Sinks

open Fable.Core.JS
open Fable.Core.JsInterop
open Scriptorium.Parchment

// This module emits raw JavaScript (console API), which the Python and BEAM targets cannot compile,
// so it is excluded there. It stays available on .NET for source compatibility (as before).
#if !(FABLE_COMPILER_PYTHON || FABLE_COMPILER_BEAM)
/// <summary>
/// Sinks that rely on JavaScript-specific APIs.
/// </summary>
module JavaScript =

    /// <summary>
    /// A sink that uses the browser console API.
    ///
    /// - Error - <c>console.error</c>
    /// - Warning - <c>console.warn</c>
    /// - Debug - <c>console.debug</c>
    /// - everything else - <c>console.log</c>.
    /// </summary>
    let browserConsole () : Sink =
        fun severity msg ->
            match severity with
            | Severity.Error -> console.error(msg)
            | Severity.Warning -> console.warn(msg)
            | Severity.Debug -> emitJsStatement msg "console.debug($0)"
            | Severity.Info
            | Severity.Verbose
            | Severity.Silly -> console.log(msg)
#endif
