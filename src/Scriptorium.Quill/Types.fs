namespace Scriptorium.Quill

type TargetPlatform =
    | JavaScript
    | DotNet
    | Python

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

type PassedResult =
    {
        Path: string list
        Duration: int
        SlowThresholdMs: int
    }

type FailedResult =
    {
        Path: string list
        Message: string
        FilePath: string
        LineNumber: int
        Duration: int
        SlowThresholdMs: int
    }

[<RequireQualifiedAccess>]
type TestResult =
    | Passed of PassedResult
    | Failed of FailedResult
    | Skipped of path: string list // runtime/conditional skip (skipIf, focus mode)
    | Pending of path: string list // author-marked (xtest, todo)
