namespace Scriptorium.Parchment.Sinks

open Fable.Core
open Fable.Core.JsInterop
open Scriptorium.Parchment

/// <summary>
/// Sinks that work on all the supported runtimes.
/// </summary>
module Universal =

    let private stdout (msg: string) : unit =
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        emitJsStatement msg "(typeof process !== 'undefined' && process.stdout) ? process.stdout.write($0 + '\\n') : console.log($0)"
        #endif
        #if FABLE_COMPILER_BEAM
        // Mirrors the stderr branch. Not via Console.WriteLine: fable-library-beam lowers that to
        // io:format("~s~n"), which writes the UTF-8 binary's raw bytes. That renders correctly only
        // by accident on a latin1 device; once the device is set to unicode (see Quill's
        // initTerminal) ~s re-encodes each byte as latin1 and mangles any non-ASCII glyph.
        Fable.Core.BeamInterop.emitErlStatement msg "io:format(\"~ts~n\", [$0])"
        #endif
        #if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_BEAM)
        System.Console.WriteLine(msg)
        #endif

    let private stderr (msg: string) : unit =
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        emitJsStatement msg "(typeof process !== 'undefined' && process.stderr) ? process.stderr.write($0 + '\\n') : console.warn($0)"
        #endif
        #if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyStatement msg "print($0, file=__import__('sys').stderr)"
        #endif
        #if FABLE_COMPILER_BEAM
        // ~ts treats the binary as data (not a format string); standard_error is stderr.
        Fable.Core.BeamInterop.emitErlStatement msg "io:format(standard_error, \"~ts~n\", [$0])"
        #endif
        #if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON || FABLE_COMPILER_BEAM)
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
