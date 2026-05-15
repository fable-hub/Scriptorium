namespace Scriptorium.Parchment.Sinks

open Fable.Core.JS
open Fable.Core.JsInterop
open Scriptorium.Parchment

module JavaScript =

    /// <summary>
    /// A sink that uses the browser console API.
    ///
    /// - Error → <c>console.error</c>
    /// - Warning → <c>console.warn</c>
    /// - Debug → <c>console.debug</c>
    /// - everything else → <c>console.log</c>.
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
