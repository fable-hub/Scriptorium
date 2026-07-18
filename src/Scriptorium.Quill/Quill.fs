namespace Scriptorium.Quill

open Scriptorium.Ink
open Scriptorium.Parchment
open Scriptorium.Parchment.Sinks
open Scriptorium.Quill.Prelude
open Fable.Core
open Fable.Core.JsInterop

module internal Advanced =


    let hasFocused (tests: TestCase list) : bool =
        let rec check test =
            match test with
            | TestCase.SyncTest ctx -> ctx.Mark = TestMark.Focused
            | TestCase.AsyncTest ctx -> ctx.Mark = TestMark.Focused
            | TestCase.TestList {
                                    Mark = TestMark.Focused
                                } -> true
            | TestCase.TestList ctx -> List.exists check ctx.Tests

        List.exists check tests

    /// Collects every leaf test path in the tree as a string list (root → leaf).
    let collectPaths (tests: TestCase list) : string list list =
        let rec collect (path: string list) test =
            match test with
            | TestCase.SyncTest def -> [ List.rev (def.Name :: path) ]
            | TestCase.AsyncTest def -> [ List.rev (def.Name :: path) ]
            | TestCase.TestList def -> def.Tests |> List.collect (collect (def.Name :: path))

        tests |> List.collect (collect [])

    /// Returns the full paths of any tests that share a path with another test.
    let findDuplicatePaths (tests: TestCase list) : string list =
        collectPaths tests
        |> List.groupBy id
        |> List.choose (fun (path, group) ->
            if group.Length > 1 then
                Some(path |> String.concat " > ")
            else
                None
        )

    let indent (depth: int) = System.String(' ', depth * 2)

    /// OSC 8 hyperlink: clicking opens the file at the given line in the terminal.
    /// The visible text uses a relative path; the URL uses the absolute path.
    let fileLink (filePath: string) (lineNumber: int) =
        let url = $"file://%s{filePath}:%d{lineNumber}"

        let rel =
            if filePath.StartsWith(cwd) then
                filePath.[cwd.Length + 1 ..]
            else
                filePath

        let display = $"%s{rel}:%d{lineNumber}"
        sprintf "\x1b]8;;%s\x1b\\%s\x1b]8;;\x1b\\" url display

    // ---------------------------------------------------------------------------
    // Timeout helper
    // ---------------------------------------------------------------------------

    /// Run `computation`, failing with a timeout message if it exceeds `ms`. Platform-specific: JS
    /// uses `setTimeout` via `Async.FromContinuations` (Fable's `Async.StartChild` timeout overload
    /// is broken - its `parallel2` uses `Promise.all`, so the timeout branch always fires); .NET
    /// delegates to `Async.StartChild(computation, ms)`.

    [<Emit("setTimeout($0, $1)")>]
    let jsSetTimeout (callback: unit -> unit) (ms: int) : obj = jsNative

    [<Emit("clearTimeout($0)")>]
    let jsClearTimeout (id: obj) : unit = jsNative

    let withTimeout (ms: int) (computation: Async<unit>) : Async<unit> =
        if Compiler.isJavaScript || Compiler.isTypeScript then
            Async.FromContinuations(fun (resolve, reject, _cancel) ->
                let mutable settled = false
                let mutable timerId: obj = null

                let settle f =
                    if not settled then
                        settled <- true
                        jsClearTimeout timerId
                        f ()

                // Set the timer before starting the computation so timerId is always
                // assigned by the time settle() could be called from the async side.
                timerId <-
                    jsSetTimeout
                        (fun () ->
                            settle (fun () ->
                                reject (System.TimeoutException($"Test timed out after {ms}ms"))
                            )
                        )
                        ms

                // Start the real computation.
                Async.StartImmediate(
                    async {
                        try
                            do! computation
                            settle (fun () -> resolve ())
                        with ex ->
                            settle (fun () -> reject ex)
                    }
                )
            )
        else
            async {
                let! timeoutable = Async.StartChild(computation, ms)
                do! timeoutable
            }

    let runSequentially (asyncs: Async<'a list> list) : Async<'a list> =
        async {
            let results = ResizeArray()

            for a in asyncs do
                let! r = a
                results.AddRange(r)

            return List.ofSeq results
        }

    let execute
        (anyFocused: bool)
        (globalConfig: TestConfig)
        (onResult: TestResult -> unit)
        (tests: TestCase list)
        : Async<TestResult list>
        =
        let rec run
            (path: string list)
            (focusedAncestor: bool)
            (inheritedConfig: TestConfig)
            (test: TestCase)
            : Async<TestResult list>
            =
            // Handles the full leaf lifecycle: pending/skip checks, stopwatch, result building,
            // and error handling.
            let runLeaf
                (def: TestDefinition<_>)
                (makeBody: TestConfig -> UniversalStopwatch -> Async<unit>)
                =
                let effectiveConfig = def.Configurer inheritedConfig
                let currentPath = def.Name :: path

                // Report a result and return it as a single-element list.
                let report r =
                    onResult r
                    async { return [ r ] }

                // Author explicitly marked this test as pending (xtest / todo).
                // The body is never executed regardless of focus mode or config.
                if def.Mark = TestMark.Pending then
                    report (TestResult.Pending currentPath)
                // Runtime skip: either the configurer set Skip = true (e.g. skipIf),
                // or focus mode is active and this test is not focused nor under a
                // focused ancestor - so it is sidelined for this run.
                elif
                    effectiveConfig.Skip
                    || (anyFocused && not focusedAncestor && def.Mark = TestMark.Normal)
                then
                    report (TestResult.Skipped currentPath)
                else
                    let sw = UniversalStopwatch()

                    let passed () =
                        TestResult.Passed
                            {
                                Path = currentPath
                                Duration = sw.ElapsedMs()
                                SlowThresholdMs = effectiveConfig.SlowThresholdMs
                            }

                    let failed msg =
                        TestResult.Failed
                            {
                                Path = currentPath
                                Message = msg
                                FilePath = def.FilePath
                                LineNumber = def.LineNumber
                                Duration = sw.ElapsedMs()
                                SlowThresholdMs = effectiveConfig.SlowThresholdMs
                            }

                    async {
                        try
                            do! makeBody effectiveConfig sw
                            return! report (passed ())
                        with ex ->
                            return! report (failed ex.Message)
                    }

            async {
                match test with
                | TestCase.SyncTest ctx ->
                    return!
                        runLeaf
                            ctx
                            (fun effectiveConfig sw ->
                                async {
                                    let testCtx =
                                        {
                                            Name = ctx.Name
                                            FilePath = ctx.FilePath
                                            Path = List.rev (ctx.Name :: path)
                                        }

                                    ctx.Body(testCtx)
                                    // Retroactive timeout check: synchronous code cannot be
                                    // interrupted mid-execution, so we check the wall clock
                                    // after the body returns and fail if it ran over budget.
                                    let elapsed = sw.ElapsedMs()

                                    match effectiveConfig.TimeoutMs with
                                    | Some ms when elapsed >= ms ->
                                        raise (System.TimeoutException($"Test timed out after {ms}ms"))
                                    | _ -> ()
                                }
                            )

                | TestCase.AsyncTest ctx ->
                    return!
                        runLeaf
                            ctx
                            (fun effectiveConfig _ ->
                                let testCtx =
                                    {
                                        Name = ctx.Name
                                        FilePath = ctx.FilePath
                                        Path = List.rev (ctx.Name :: path)
                                    }

                                match effectiveConfig.TimeoutMs with
                                | None -> ctx.Body(testCtx)
                                | Some ms -> withTimeout ms (ctx.Body(testCtx))
                            )

                | TestCase.TestList ctx ->
                    let listConfig = ctx.Configurer inheritedConfig
                    let currentPath = ctx.Name :: path

                    let schedule (asyncs: Async<TestResult list> list) : Async<TestResult list> =
                        if ctx.IsSequential then
                            runSequentially asyncs
                        else
                            async {
                                let! arr = Async.Parallel asyncs
                                return List.ofArray arr |> List.concat
                            }

                    match ctx.Mark with
                    | TestMark.Pending ->
                        let rec collectPending p tc =
                            match tc with
                            | TestCase.SyncTest def -> [ TestResult.Pending(def.Name :: p) ]
                            | TestCase.AsyncTest def -> [ TestResult.Pending(def.Name :: p) ]
                            | TestCase.TestList def ->
                                def.Tests |> List.collect (collectPending (def.Name :: p))

                        let pending = ctx.Tests |> List.collect (collectPending currentPath)
                        pending |> List.iter onResult
                        return pending

                    | TestMark.Focused ->
                        return! ctx.Tests |> List.map (run currentPath true listConfig) |> schedule

                    | TestMark.Normal ->
                        return!
                            ctx.Tests
                            |> List.map (run currentPath focusedAncestor listConfig)
                            |> schedule
            }

        async {
            let! resultLists = tests |> List.map (run [] false globalConfig) |> Async.Parallel

            return resultLists |> List.ofArray |> List.concat
        }

    /// Raw write without a trailing newline - used for the live dot progress line.
    let writeRaw (s: string) : unit =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        emitJsStatement
            s
            "(typeof process !== 'undefined' && process.stdout) ? process.stdout.write($0) : console.log($0)"
    #endif
    #if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyStatement s "__import__('sys').stdout.write($0); __import__('sys').stdout.flush()"
    #endif
    #if FABLE_COMPILER_BEAM
        // io:put_chars writes the binary without a trailing newline.
        Fable.Core.BeamInterop.emitErlStatement s "io:put_chars($0)"
    #endif
    #if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON || FABLE_COMPILER_BEAM)
        System.Console.Write(s)
    #endif

    let isSlow (r: PassedResult) = r.Duration > r.SlowThresholdMs

    let passedColor (r: PassedResult) : (string -> string) =
        if isSlow r then
            yellow
        else
            green

    let formatMs (r: PassedResult) = passedColor r $"{r.Duration}ms"

    let printResults (logger: Logger) (results: TestResult list) : unit =
        let getPath result =
            match result with
            | TestResult.Passed r -> r.Path
            | TestResult.Failed r -> r.Path
            | TestResult.Skipped p -> p
            | TestResult.Pending p -> p

        results
        |> List.fold
            (fun lastPath result ->
                let fullPath = getPath result |> List.rev
                let listParts = fullPath |> List.take (fullPath.Length - 1)
                let lastListParts = lastPath |> List.take (max 0 (lastPath.Length - 1))

                let shared =
                    Seq.zip listParts lastListParts
                    |> Seq.takeWhile (fun (a, b) -> a = b)
                    |> Seq.length

                for i in shared .. listParts.Length - 1 do
                    logger.info $"%s{indent i}%s{bold listParts[i]}"

                let depth = fullPath.Length - 1
                let name = List.last fullPath

                match result with
                | TestResult.Passed r ->
                    let check = passedColor r "✓"
                    logger.info $"%s{indent depth}{check} {name} {formatMs r}"
                | TestResult.Skipped _ ->
                    let dimDash = dim "-"
                    let dimName = dim name
                    logger.info $"%s{indent depth}{dimDash} {dimName}"
                | TestResult.Pending _ ->
                    let yellowStar = yellow "*"
                    let dimName = dim name
                    logger.info $"%s{indent depth}{yellowStar} {dimName}"
                | TestResult.Failed r ->
                    let redCross = red "✗"
                    let boldName = red name
                    let dimDuration = dim $"{r.Duration}ms"
                    logger.error $"%s{indent depth}{redCross} {boldName} {dimDuration}"

                    r.Message.Split('\n')
                    |> Array.iter (fun line ->
                        // let redLine = red line
                        logger.error $"%s{indent depth}  {line}"
                    )

                    let link = fileLink r.FilePath r.LineNumber
                    let dimLink = dim $"at {link}"
                    logger.error $"%s{indent depth}  {dimLink}"

                fullPath
            )
            []
        |> ignore

    let universalRunTests (funcAsync: Async<int>) : int =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        // StartAsPromise returns Promise<int>. We can't block on JS, so we chain
        // process.exit on the resolved value - the function return value is ignored.
        let promise = Async.StartAsPromise funcAsync
        emitJsStatement promise "$0.then(exitCode => process.exit(exitCode))"
        0
    #endif

    #if FABLE_COMPILER_BEAM
        // On the BEAM the run is synchronous; halt/1 exits the VM with the test exit code
        // (the analog of process.exit), so `erl` returns non-zero when tests fail.
        let exitCode = Async.RunSynchronously funcAsync
        Fable.Core.BeamInterop.emitErlStatement exitCode "halt($0)"
        exitCode
    #endif

    #if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_BEAM)
        Async.RunSynchronously funcAsync
    #endif

    let logger = Parchment.Create(Universal.console ())

open Advanced

type Runner =

    static member runTestsWith(configurer: TestConfig -> TestConfig, tests: TestCase list) =
        initTerminal ()
        let duplicates = findDuplicatePaths tests

        if not duplicates.IsEmpty then
            logger.error (red "Duplicate test paths detected - each test must have a unique path.")
            logger.error (red "The following test(s) share a path with another test:")
            logger.error ""

            for dup in duplicates do
                let cross = red "✗"
                logger.error $"  {cross} {dup}"

            logger.error ""

            async { return 1 } |> universalRunTests
        else

            let globalConfig = configurer TestConfig.Default
            let anyFocused = hasFocused tests
            let sw = UniversalStopwatch()
            let now = System.DateTime.Now
            let startAt = sprintf "%02d:%02d:%02d" now.Hour now.Minute now.Second

            let printDot result =
                match result with
                | TestResult.Passed r -> writeRaw (passedColor r "·")
                | TestResult.Failed _ -> writeRaw (red "x")
                | TestResult.Skipped _ -> writeRaw (dim "-")
                | TestResult.Pending _ -> writeRaw (yellow "*")

            async {
                let! results = execute anyFocused globalConfig printDot tests

                logger.info ""
                logger.info ""
                printResults logger results
                logger.info ""

                let passed =
                    results
                    |> List.sumBy (
                        function
                        | TestResult.Passed _ -> 1
                        | _ -> 0
                    )

                let failed =
                    results
                    |> List.sumBy (
                        function
                        | TestResult.Failed _ -> 1
                        | _ -> 0
                    )

                let skipped =
                    results
                    |> List.sumBy (
                        function
                        | TestResult.Skipped _ -> 1
                        | _ -> 0
                    )

                let pending =
                    results
                    |> List.sumBy (
                        function
                        | TestResult.Pending _ -> 1
                        | _ -> 0
                    )

                let total = passed + failed + skipped + pending

                let totalMs = sw.ElapsedMs()

                let durationStr =
                    if totalMs < 1000 then
                        $"{totalMs}ms"
                    else
                        $"%.2f{(float totalMs / 1000.0)}s".Replace(".00s", "s")

                let parts =
                    [
                        if failed > 0 then
                            red $"{failed} failed"
                        if isCI && anyFocused then
                            red "focused (not allowed in CI)"
                        if passed > 0 then
                            (green >> bold) $"{passed} passed"
                        if skipped > 0 then
                            dim $"{skipped} skipped"
                        if pending > 0 then
                            yellow $"{pending} pending"
                    ]
                    |> String.concat " | "

                let labelTests = dim ("Tests".PadLeft(9))
                let labelStartAt = dim ("Start at".PadLeft(9))
                let labelDuration = dim ("Duration".PadLeft(9))
                let totalStr = dim $"({total})"

                logger.info $"{labelTests}  {parts} {totalStr}"
                logger.info $"{labelStartAt}  {startAt}"
                logger.info $"{labelDuration}  {durationStr}"
                logger.info ""

                if isCI && anyFocused then
                    logger.warning (
                        red "CI: focused tests detected - ftest/ftestList must not be committed."
                    )

                    logger.warning ""

                let exitCode =
                    if failed <> 0 then
                        1
                    else if isCI && anyFocused then
                        1
                    else
                        0

                return exitCode
            }
            |> universalRunTests

    static member runTestsWith(configurer: TestConfig -> TestConfig, test: TestCase) =
        Runner.runTestsWith (configurer, [ test ])

    static member runTests(tests: TestCase list) = Runner.runTestsWith (id, tests)

    static member runTests(test: TestCase) = Runner.runTestsWith (id, [ test ])
