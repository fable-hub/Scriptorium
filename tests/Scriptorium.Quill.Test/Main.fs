module MainTests

open Scriptorium.Quill
open Scriptorium.Nib.Assertion

open type Scriptorium.Quill.Test
open type Scriptorium.Quill.Runner

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/// Run a list of TestCase trees and return the results without printing.
let private runToResultsWith (configurer: TestConfig -> TestConfig) (tests: TestCase list) : Async<TestResult list> =
    let anyFocused = Advanced.hasFocused tests
    Advanced.execute anyFocused (configurer TestConfig.Default) ignore tests

let private runToResults (tests: TestCase list) : Async<TestResult list> =
    runToResultsWith id tests

// ---------------------------------------------------------------------------
// Assertion helpers on TestResult
// ---------------------------------------------------------------------------

let private isPassed r =
    match r with
    | TestResult.Passed _ -> true
    | _ -> false

let private isFailed r =
    match r with
    | TestResult.Failed _ -> true
    | _ -> false

let private isSkipped r =
    match r with
    | TestResult.Skipped _ -> true
    | _ -> false

let private isPending r =
    match r with
    | TestResult.Pending _ -> true
    | _ -> false

let private passedCount rs =
    rs |> List.filter isPassed |> List.length

let private failedCount rs =
    rs |> List.filter isFailed |> List.length

let private skippedCount rs =
    rs |> List.filter isSkipped |> List.length

let private pathOf r =
    match r with
    | TestResult.Passed r -> List.rev r.Path
    | TestResult.Failed r -> List.rev r.Path
    | TestResult.Skipped p -> List.rev p
    | TestResult.Pending p -> List.rev p

let private slowThresholdOf r =
    match r with
    | TestResult.Passed r -> r.SlowThresholdMs
    | TestResult.Failed r -> r.SlowThresholdMs
    | TestResult.Skipped _ -> failwith "Skipped has no slowThreshold"
    | TestResult.Pending _ -> failwith "Pending has no slowThreshold"

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

[<EntryPoint>]
let main _ =

    let tests =
        testList (
            "Runner",
            [

                // ------------------------------------------------------------------
                // Basic pass / fail
                // ------------------------------------------------------------------

                testList (
                    "basic",
                    [

                        testAsync (
                            "passing test produces Passed result",
                            fun _ ->
                                async {
                                    let! results = runToResults [ test ("ok", fun _ -> ()) ]
                                    assertThat results (hasSize 1)
                                    assertThat (isPassed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "failing test produces Failed result",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults [ test ("bad", fun _ -> failwith "boom") ]

                                    assertThat results (hasSize 1)
                                    assertThat (isFailed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "multiple failing tests all produce Failed results",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                test ("bad1", fun _ -> failwith "x")
                                                test ("bad2", fun _ -> failwith "y")
                                                test ("ok", fun _ -> ())
                                            ]

                                    assertThat (failedCount results) (isEqualTo 2)
                                    assertThat (passedCount results) (isEqualTo 1)
                                }
                        )

                        testAsync (
                            "Failed result carries the exception message",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults [ test ("bad", fun _ -> failwith "boom") ]

                                    match results[0] with
                                    | TestResult.Failed r -> assertThat r.Message (isEqualTo "boom")
                                    | other -> failwithf "Expected Failed, got %A" other
                                }
                        )

                        testAsync (
                            "path is [listName; testName] for nested test",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [ testList ("MyList", [ test ("t", fun _ -> ()) ]) ]

                                    assertThat
                                        (pathOf results[0])
                                        (isEqualTo [ "MyList"; "t" ])
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // xtest — skip
                // ------------------------------------------------------------------

                testList (
                    "xtest",
                    [

                        testAsync (
                            "xtest produces Pending result",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults [ xtest ("skip me", fun _ -> ()) ]

                                    assertThat results (hasSize 1)
                                    assertThat (isPending results[0]) isTrue
                                }
                        )

                        testAsync (
                            "xtest body is never executed",
                            fun _ ->
                                async {
                                    let mutable ran = false

                                    let! results =
                                        runToResults [ xtest ("skip me", fun _ -> ran <- true) ]

                                    assertThat ran isFalse
                                    assertThat (isPending results[0]) isTrue
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // todo — placeholder
                // ------------------------------------------------------------------

                testList (
                    "todo",
                    [

                        testAsync (
                            "todo produces Pending result",
                            fun _ ->
                                async {
                                    let! results = runToResults [ todo "not implemented yet" ]

                                    assertThat results (hasSize 1)
                                    assertThat (isPending results[0]) isTrue
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // ftest — focused
                // ------------------------------------------------------------------

                testList (
                    "ftest",
                    [

                        testAsync (
                            "focused test runs, normal test is skipped",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                test ("normal", fun _ -> ())
                                                ftest ("focused", fun _ -> ())
                                            ]

                                    assertThat (passedCount results) (isEqualTo 1)
                                    assertThat (skippedCount results) (isEqualTo 1)

                                    assertThat
                                        (pathOf (results |> List.find isPassed))
                                        (isEqualTo [ "focused" ])

                                    assertThat
                                        (pathOf (results |> List.find isSkipped))
                                        (isEqualTo [ "normal" ])
                                }
                        )

                        testAsync (
                            "multiple focused tests all run",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                ftest ("f1", fun _ -> ())
                                                ftest ("f2", fun _ -> ())
                                                test ("n1", fun _ -> ())
                                            ]

                                    assertThat (passedCount results) (isEqualTo 2)
                                    assertThat (skippedCount results) (isEqualTo 1)
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // xtestList — skips all children
                // ------------------------------------------------------------------

                testList (
                    "xtestList",
                    [

                        testAsync (
                            "xtestList marks all children as Pending",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                xtestList (
                                                    "pending group",
                                                    [
                                                        test ("a", fun _ -> ())
                                                        test ("b", fun _ -> ())
                                                    ]
                                                )
                                            ]

                                    assertThat results (hasSize 2)
                                    assertThat (results |> List.filter isPending |> List.length) (isEqualTo 2)
                                }
                        )

                        testAsync (
                            "xtestList children bodies are never executed",
                            fun _ ->
                                async {
                                    let mutable count = 0

                                    let! _ =
                                        runToResults
                                            [
                                                xtestList (
                                                    "pending group",
                                                    [
                                                        test ("a", fun _ -> count <- count + 1)
                                                        test ("b", fun _ -> count <- count + 1)
                                                    ]
                                                )
                                            ]

                                    assertThat count (isEqualTo 0)
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // ftestList — focused list runs all children, others skipped
                // ------------------------------------------------------------------

                testList (
                    "ftestList",
                    [

                        testAsync (
                            "ftestList children run, sibling normal tests are skipped",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                ftestList (
                                                    "focused group",
                                                    [
                                                        test ("a", fun _ -> ())
                                                        test ("b", fun _ -> ())
                                                    ]
                                                )
                                                test ("outside", fun _ -> ())
                                            ]

                                    assertThat (passedCount results) (isEqualTo 2)
                                    assertThat (skippedCount results) (isEqualTo 1)
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // testSequenced — sequential execution
                // ------------------------------------------------------------------

                testList (
                    "testSequenced",
                    [

                        testAsync (
                            "children run one after another",
                            fun _ ->
                                async {
                                    let order = System.Collections.Generic.List<int>()

                                    let! _ =
                                        runToResults
                                            [
                                                testSequenced (
                                                    "seq",
                                                    [
                                                        // t1 sleeps before recording; if parallel t2 would fire first
                                                        testAsync (
                                                            "t1",
                                                            fun _ ->
                                                                async {
                                                                    do! Async.Sleep 50
                                                                    order.Add(1)
                                                                }
                                                        )
                                                        testAsync (
                                                            "t2",
                                                            fun _ ->
                                                                async {
                                                                    order.Add(2)
                                                                }
                                                        )
                                                    ]
                                                )
                                            ]

                                    assertThat (List.ofSeq order) (isEqualTo [ 1; 2 ])
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // CallerFilePath / CallerLineNumber
                // ------------------------------------------------------------------

                testList (
                    "caller info",
                    [

                        testAsync (
                            "Failed result carries non-empty filePath",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults [ test ("bad", fun _ -> failwith "x") ]

                                    match results[0] with
                                    | TestResult.Failed r -> assertThat (r.FilePath.Length > 0) isTrue
                                    | other -> failwithf "Expected Failed, got %A" other
                                }
                        )

                        testAsync (
                            "Failed result carries positive lineNumber",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults [ test ("bad", fun _ -> failwith "x") ]

                                    match results[0] with
                                    | TestResult.Failed r -> assertThat (r.LineNumber > 0) isTrue
                                    | other -> failwithf "Expected Failed, got %A" other
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Test Configuration (skipIf, timeout, etc.)
                // ------------------------------------------------------------------

                testList (
                    "TestConfig",
                    [

                        testAsync (
                            "skipIf(true) skips test via configurer",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                test ("skip me", skipIf true, fun _ -> ())
                                            ]

                                    assertThat results (hasSize 1)
                                    assertThat (isSkipped results[0]) isTrue
                                }
                        )

                        testAsync (
                            "skipIf(false) runs test",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                test ("run me", skipIf false, fun _ -> ())
                                            ]

                                    assertThat results (hasSize 1)
                                    assertThat (isPassed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "composed configurers work together",
                            fun _ ->
                                async {
                                    // skipIf(false) >> timeout 1000 should not skip and set timeout
                                    let! results =
                                        runToResults
                                            [
                                                test ("composed", skipIf false >> timeout 1000, fun _ -> ())
                                            ]

                                    assertThat results (hasSize 1)
                                    assertThat (isPassed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "timeout causes test to fail when exceeded",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                testAsync (
                                                    "slow test",
                                                    timeout 100,
                                                    fun _ -> async { do! Async.Sleep 500 }
                                                )
                                            ]

                                    assertThat results (hasSize 1)
                                    assertThat (isFailed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "timeout causes sync test to fail when exceeded",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                test (
                                                    "slow sync test",
                                                    timeout 1,
                                                    fun _ ->
                                                        // Busy-wait long enough to exceed the 1ms timeout.
                                                        // Uses DateTime so it works on both .NET and JavaScript.
                                                        let start = System.DateTime.UtcNow
                                                        while (System.DateTime.UtcNow - start).TotalMilliseconds < 50.0 do
                                                            ()
                                                )
                                            ]

                                    assertThat results (hasSize 1)
                                    assertThat (isFailed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "skipIfJavaScript skips on JavaScript, runs on .NET",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                test ("platform", skipIfJavaScript, fun _ -> ())
                                            ]

                                    assertThat results (hasSize 1)

                                    match Prelude.currentPlatform with
                                    | JavaScript -> assertThat (isSkipped results[0]) isTrue
                                    | DotNet -> assertThat (isPassed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "skipIfDotNet skips on .NET, runs on JavaScript",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                test ("platform", skipIfDotNet, fun _ -> ())
                                            ]

                                    assertThat results (hasSize 1)

                                    match Prelude.currentPlatform with
                                    | DotNet -> assertThat (isSkipped results[0]) isTrue
                                    | JavaScript -> assertThat (isPassed results[0]) isTrue
                                }
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Config inheritance (global → testList → test)
                // ------------------------------------------------------------------

                testList (
                    "Config inheritance",
                    [

                        testAsync (
                            "global slowThreshold is inherited by test",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResultsWith (slowThreshold 500) [ test ("t", fun _ -> ()) ]

                                    assertThat (slowThresholdOf results[0]) (isEqualTo 500)
                                }
                        )

                        testAsync (
                            "testList slowThreshold overrides global",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResultsWith
                                            (slowThreshold 500)
                                            [ testList ("list", slowThreshold 200, [ test ("t", fun _ -> ()) ]) ]

                                    assertThat (slowThresholdOf results[0]) (isEqualTo 200)
                                }
                        )

                        testAsync (
                            "test slowThreshold overrides testList",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResultsWith
                                            (slowThreshold 500)
                                            [
                                                testList (
                                                    "list",
                                                    slowThreshold 200,
                                                    [ test ("t", slowThreshold 100, fun _ -> ()) ]
                                                )
                                            ]

                                    assertThat (slowThresholdOf results[0]) (isEqualTo 100)
                                }
                        )

                        testAsync (
                            "global timeout is inherited by async test",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResultsWith
                                            (timeout 50)
                                            [ testAsync ("slow", fun _ -> async { do! Async.Sleep 500 }) ]

                                    assertThat (isFailed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "testList timeout overrides global",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResultsWith
                                            (timeout 50)
                                            [
                                                testList (
                                                    "list",
                                                    timeout 2000,
                                                    [ testAsync ("t", fun _ -> async { do! Async.Sleep 100 }) ]
                                                )
                                            ]

                                    assertThat (isPassed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "test timeout overrides testList",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                testList (
                                                    "list",
                                                    timeout 50,
                                                    [ testAsync ("t", timeout 2000, fun _ -> async { do! Async.Sleep 100 }) ]
                                                )
                                            ]

                                    assertThat (isPassed results[0]) isTrue
                                }
                        )

                        testAsync (
                            "testList Skip propagates to children",
                            fun _ ->
                                async {
                                    let! results =
                                        runToResults
                                            [
                                                testList (
                                                    "list",
                                                    skipIf true,
                                                    [
                                                        test ("a", fun _ -> ())
                                                        test ("b", fun _ -> ())
                                                    ]
                                                )
                                            ]

                                    assertThat (skippedCount results) (isEqualTo 2)
                                }
                        )

                    ]
                )

            ]
        )

    runTests tests
