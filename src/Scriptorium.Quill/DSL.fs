namespace Scriptorium.Quill


open System.Runtime.CompilerServices
open Scriptorium.Quill.Prelude

[<AutoOpen>]
module Configurers =

    let skipIf (condition: bool) (config: TestConfig) : TestConfig =
        if condition then
            { config with
                Skip = true
            }
        else
            config

    let skipIfJavaScript (config: TestConfig) : TestConfig =
        if currentPlatform = JavaScript then
            { config with
                Skip = true
            }
        else
            config

    let skipIfDotNet (config: TestConfig) : TestConfig =
        if currentPlatform = DotNet then
            { config with
                Skip = true
            }
        else
            config

    let skipIfPython (config: TestConfig) : TestConfig =
        if currentPlatform = Python then
            { config with
                Skip = true
            }
        else
            config

    let skipIfBeam (config: TestConfig) : TestConfig =
        if currentPlatform = Beam then
            { config with
                Skip = true
            }
        else
            config

    let timeout (ms: int) (config: TestConfig) : TestConfig =
        { config with
            TimeoutMs = Some ms
        }

    let noTimeout (config: TestConfig) : TestConfig =
        { config with
            TimeoutMs = None
        }

    let slowThreshold (ms: int) (config: TestConfig) : TestConfig =
        { config with
            SlowThresholdMs = ms
        }


type Test =

    static member test
        (
            name: string,
            body: TestContext -> unit,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.SyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Normal
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member test
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: TestContext -> unit,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.SyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Normal
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member xtest
        (
            name: string,
            body: TestContext -> unit,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.SyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Pending
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member xtest
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: TestContext -> unit,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.SyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Pending
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member ftest
        (
            name: string,
            body: TestContext -> unit,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.SyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Focused
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member ftest
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: TestContext -> unit,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.SyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Focused
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member testAsync
        (
            name: string,
            body: Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        TestCase.AsyncTest
            {
                Name = name
                Body = fun _ -> body
                Mark = TestMark.Normal
                FilePath = defaultArg filePath ""
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member testAsync
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        TestCase.AsyncTest
            {
                Name = name
                Body = fun _ -> body
                Mark = TestMark.Normal
                FilePath = defaultArg filePath ""
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member testAsync
        (
            name: string,
            body: TestContext -> Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.AsyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Normal
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member testAsync
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: TestContext -> Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.AsyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Normal
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member xtestAsync
        (
            name: string,
            body: Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        TestCase.AsyncTest
            {
                Name = name
                Body = fun _ -> body
                Mark = TestMark.Pending
                FilePath = defaultArg filePath ""
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member xtestAsync
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        TestCase.AsyncTest
            {
                Name = name
                Body = fun _ -> body
                Mark = TestMark.Pending
                FilePath = defaultArg filePath ""
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member xtestAsync
        (
            name: string,
            body: TestContext -> Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.AsyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Pending
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member xtestAsync
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: TestContext -> Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.AsyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Pending
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member ftestAsync
        (
            name: string,
            body: Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        TestCase.AsyncTest
            {
                Name = name
                Body = fun _ -> body
                Mark = TestMark.Focused
                FilePath = defaultArg filePath ""
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member ftestAsync
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        TestCase.AsyncTest
            {
                Name = name
                Body = fun _ -> body
                Mark = TestMark.Focused
                FilePath = defaultArg filePath ""
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member ftestAsync
        (
            name: string,
            body: TestContext -> Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.AsyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Focused
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member ftestAsync
        (
            name: string,
            configurer: TestConfig -> TestConfig,
            body: TestContext -> Async<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let fp = defaultArg filePath ""

        TestCase.AsyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Focused
                FilePath = fp
                LineNumber = defaultArg lineNumber 0
                Configurer = configurer
            }

    static member todo
        (name: string, [<CallerFilePath>] ?filePath: string, [<CallerLineNumber>] ?lineNumber: int)
        : TestCase
        =
        TestCase.SyncTest
            {
                Name = name
                Body = fun _ -> ()
                Mark = TestMark.Pending
                FilePath = defaultArg filePath ""
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

    static member testList(name, tests) =
        TestCase.TestList
            {
                Name = name
                Tests = tests
                Mark = TestMark.Normal
                Configurer = id
                IsSequential = false
            }

    static member testList(name, configurer, tests) =
        TestCase.TestList
            {
                Name = name
                Tests = tests
                Mark = TestMark.Normal
                Configurer = configurer
                IsSequential = false
            }

    static member xtestList(name, tests) =
        TestCase.TestList
            {
                Name = name
                Tests = tests
                Mark = TestMark.Pending
                Configurer = id
                IsSequential = false
            }

    static member xtestList(name, configurer, tests) =
        TestCase.TestList
            {
                Name = name
                Tests = tests
                Mark = TestMark.Pending
                Configurer = configurer
                IsSequential = false
            }

    static member ftestList(name, tests) =
        TestCase.TestList
            {
                Name = name
                Tests = tests
                Mark = TestMark.Focused
                Configurer = id
                IsSequential = false
            }

    static member ftestList(name, configurer, tests) =
        TestCase.TestList
            {
                Name = name
                Tests = tests
                Mark = TestMark.Focused
                Configurer = configurer
                IsSequential = false
            }

    static member testSequenced(name, tests) =
        TestCase.TestList
            {
                Name = name
                Tests = tests
                Mark = TestMark.Normal
                Configurer = id
                IsSequential = true
            }

    static member testSequenced(name, configurer, tests) =
        TestCase.TestList
            {
                Name = name
                Tests = tests
                Mark = TestMark.Normal
                Configurer = configurer
                IsSequential = true
            }
