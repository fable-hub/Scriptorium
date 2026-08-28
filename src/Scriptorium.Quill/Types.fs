namespace Scriptorium.Quill

type TargetPlatform =
    | JavaScript
    | DotNet
    | Python
    | Beam

type TestConfig =
    {
        Skip: bool
        TimeoutMs: int option
        SlowThresholdMs: int
    }

    static member Default =
        {
            Skip = false
            TimeoutMs = Some 5_000 // 5 second default timeout
            SlowThresholdMs = 300
        }

/// Context injected into test bodies that use the <c>fun t -&gt; ...</c> signature.
/// Carries the test's own metadata so helpers (e.g. snapshot) can use it
/// without the author having to repeat the test name.
type TestContext =
    {
        Name: string
        FilePath: string
        /// Full path from the root testList down to this test, e.g. ["Suite"; "Group"; "test name"].
        Path: string list
    }

[<RequireQualifiedAccess>]
type TestMark =
    | Normal
    | Pending
    | Focused

type TestDefinition<'body> =
    {
        Name: string
        Body: 'body
        Mark: TestMark
        FilePath: string
        LineNumber: int
        Configurer: TestConfig -> TestConfig
    }

type TestListDefinition =
    {
        Name: string
        Tests: TestCase list
        Mark: TestMark
        Configurer: TestConfig -> TestConfig
        IsSequential: bool
    }

and [<RequireQualifiedAccess>] TestCase =
    | SyncTest of TestDefinition<TestContext -> unit>
    | AsyncTest of TestDefinition<TestContext -> Async<unit>>
    | TestList of TestListDefinition

/// Why a test was sidelined for this run.
[<RequireQualifiedAccess>]
type SkipReason =
    /// The effective configuration asked for it (e.g. <c>skipIf</c>, <c>skipIfPython</c>).
    | Configured
    /// Focus mode is active and this test is neither focused nor under a focused list.
    | NotFocused

type SkippedResult =
    {
        Path: string list
        FilePath: string
        LineNumber: int
        Reason: SkipReason
    }

type PendingResult =
    {
        Path: string list
        FilePath: string
        LineNumber: int
    }

type PassedResult =
    {
        Path: string list
        FilePath: string
        LineNumber: int
        Duration: int
        SlowThresholdMs: int
    }

type FailedResult =
    {
        Path: string list
        Message: string
        /// Runtime type name of the exception, when the target can report one.
        ExceptionType: string option
        /// Stack trace of the exception, when the target records one.
        StackTrace: string option
        FilePath: string
        LineNumber: int
        Duration: int
        SlowThresholdMs: int
    }

[<RequireQualifiedAccess>]
type TestResult =
    | Passed of PassedResult
    | Failed of FailedResult
    | Skipped of SkippedResult // runtime/conditional skip (skipIf, focus mode)
    | Pending of PendingResult // author-marked (xtest, todo)

/// Everything a run produced: the individual results plus the run-level facts
/// a report writer needs.
type TestRunReport =
    {
        /// Results in declaration order.
        Results: TestResult list
        /// Wall-clock time the run started.
        StartTime: System.DateTime
        /// Total run duration in milliseconds.
        Duration: int
        PassedCount: int
        FailedCount: int
        SkippedCount: int
        PendingCount: int
        TotalCount: int
        /// Whether any test or list in the run was focused.
        AnyFocused: bool
    }

/// Observes a run
type Reporter =
    {
        /// Called as each result is produced, in completion order. On BEAM this runs inside the
        /// spawned job's process, so it can write output but cannot reach state owned by the run.
        OnResult: TestResult -> unit
        /// Called once when the run has finished, with the results in declaration order.
        OnRunComplete: TestRunReport -> unit
    }

    static member Default =
        {
            OnResult = ignore
            OnRunComplete = ignore
        }
