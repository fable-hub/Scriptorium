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

#if FABLE_COMPILER_BEAM
        // file:get_cwd() returns {ok, Dir} where Dir is a charlist; F# strings are Erlang binaries.
        Fable.Core.BeamInterop.emitErlExpr () "list_to_binary(element(2, file:get_cwd()))"
#endif

#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON || FABLE_COMPILER_BEAM)
        System.Environment.CurrentDirectory
#endif

    module Performance =

        [<Emit("performance.now()")>]
        let now () : float = jsNative

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    type UniversalStopwatch() =
        let mutable startTime: float = Performance.now ()
        member _.Restart() : unit = startTime <- Performance.now ()
        member _.ElapsedMs() : int = int (Performance.now () - startTime)
#endif

#if FABLE_COMPILER_PYTHON
    // fable-library-python has no `Stopwatch.StartNew` mapping, so use time.perf_counter (seconds).
    type UniversalStopwatch() =
        let now () : float =
            Fable.Core.PyInterop.emitPyExpr () "__import__('time').perf_counter()"

        let mutable startTime = now ()
        member _.Restart() : unit = startTime <- now ()
        member _.ElapsedMs() : int = int ((now () - startTime) * 1000.0)
#endif

#if FABLE_COMPILER_BEAM
    type UniversalStopwatch() =
        let now () : int =
            Fable.Core.BeamInterop.emitErlExpr () "erlang:monotonic_time(millisecond)"

        let mutable startTime = now ()
        member _.Restart() : unit = startTime <- now ()
        member _.ElapsedMs() : int = now () - startTime
#endif

#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON || FABLE_COMPILER_BEAM)
    type UniversalStopwatch() =
        let sw = System.Diagnostics.Stopwatch.StartNew()
        member _.Restart() : unit = sw.Restart()
        member _.ElapsedMs() : int = int sw.ElapsedMilliseconds
#endif

    let currentPlatform: TargetPlatform =
        // TypeScript compiles to and runs on the JavaScript runtime, so it reports as JavaScript.
        if Compiler.isDotnet then
            DotNet
        elif Compiler.isPython then
            Python
        elif Compiler.isBeam then
            Beam
        else
            JavaScript

    let isCI: bool =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        emitJsExpr () "!!process.env['CI']"
#endif

#if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyExpr () "__import__('os').environ.get('CI') is not None"
#endif

#if FABLE_COMPILER_BEAM
        // os:getenv/1 returns the value (charlist) or the atom false when unset.
        Fable.Core.BeamInterop.emitErlExpr () "os:getenv(\"CI\") =/= false"
#endif

#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON || FABLE_COMPILER_BEAM)
        System.Environment.GetEnvironmentVariable("CI") |> isNull |> not
#endif

    /// How many tests the runtime can genuinely run at once. Every target reports the count the
    /// process is actually allowed to use rather than the machine's core count, so a container or
    /// a CI runner with a restricted quota is respected.
    let processorCount: int =
        if Compiler.isJavaScript || Compiler.isTypeScript then
            emitJsExpr () "globalThis.navigator?.hardwareConcurrency ?? 4"
        elif Compiler.isPython then
            // process_cpu_count() is 3.13+; sched_getaffinity is Linux-only. Both see the CPUs the
            // process may use, which cpu_count() - the last resort - does not: under an affinity
            // mask it reports the whole machine.
            Fable.Core.PyInterop.emitPyExpr
                ()
                "(getattr(__import__('os'), 'process_cpu_count', None) or (getattr(__import__('os'), 'sched_getaffinity', None) and (lambda: len(__import__('os').sched_getaffinity(0)))) or __import__('os').cpu_count)() or 4"
        elif Compiler.isBeam then
            Fable.Core.BeamInterop.emitErlExpr () "erlang:system_info(schedulers_online)"
        else
            System.Environment.ProcessorCount

    /// Put the terminal into a state where the runner's output renders correctly.
    /// No-op everywhere except BEAM, where `standard_io` and `standard_error` default to
    /// latin1 under `erl -noshell`: any codepoint > 255 is then printed as an escape (U+2717
    /// shows up as an escaped literal instead of ✗) and UTF-8 binaries passed to
    /// `io:put_chars` are re-encoded as mojibake.
    /// ANSI colour codes are pure ASCII and so were never affected. The `+pc unicode` VM
    /// flag does NOT fix this - it only affects printable-list detection in `~p`.
    let initTerminal () : unit =
#if FABLE_COMPILER_BEAM
        Fable.Core.BeamInterop.emitErlStatement () "io:setopts(standard_io, [{encoding, unicode}])"

        Fable.Core.BeamInterop.emitErlStatement
            ()
            "io:setopts(standard_error, [{encoding, unicode}])"
#endif
#if !FABLE_COMPILER_BEAM
        ()
#endif

    /// The runtime type name of an exception, or <c>None</c> when the target cannot report one.
    let exceptionTypeName (ex: exn) : string option =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        let name: string =
            emitJsExpr ex "($0 && $0.constructor && $0.constructor.name) || ''"

        match name with
        | "" -> None
        | name -> Some name
#endif

#if FABLE_COMPILER_PYTHON
        let name: string = Fable.Core.PyInterop.emitPyExpr ex "type($0).__name__"

        match name with
        | "" -> None
        | name -> Some name
#endif

#if FABLE_COMPILER_BEAM
        // Fable lowers every F# exception to a bare `#{message => Binary}` map - no type tag - so
        // there is nothing on the term to name.
        ignore ex
        None
#endif

#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON || FABLE_COMPILER_BEAM)
        match ex.GetType().FullName with
        | null
        | "" -> None
        | name -> Some name
#endif

    /// The stack trace of an exception, or <c>None</c> when the target records none.
    let exceptionStackTrace (ex: exn) : string option =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        // fable-library's `Exception` is deliberately not derived from `Error` (fable#2160), so
        // only a natively thrown error carries a stack.
        let trace: string = emitJsExpr ex "($0 && $0.stack) || ''"

        match trace with
        | "" -> None
        | trace -> Some trace
#endif

#if FABLE_COMPILER_PYTHON
        let trace: string =
            Fable.Core.PyInterop.emitPyExpr
                ex
                "''.join(__import__('traceback').format_exception(type($0), $0, $0.__traceback__))"

        match trace with
        | "" -> None
        | trace -> Some trace
#endif

#if FABLE_COMPILER_BEAM
        // The BEAM keeps the stacktrace on the catch clause, not on the exception term, so by the
        // time Fable hands the value to an F# `with` binding it is already gone.
        ignore ex
        None
#endif

#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON || FABLE_COMPILER_BEAM)
        match ex.StackTrace with
        | null
        | "" -> None
        | trace -> Some trace
#endif
