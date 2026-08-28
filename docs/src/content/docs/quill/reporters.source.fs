(***
---
title: Reporters
description: Observing a run to build custom output such as JUnit XML for CI.
---
*)

(*** hide ***)

module Quill.CustomReporters

open System
open System.Globalization
open Scriptorium.Quill

(**

A reporter observes a run. `Scriptorium.Quill` uses one for its own terminal output, so the
dots, the result tree, and the summary you normally see are produced through exactly the same
hooks available to you - there is no privileged built-in path.

Register your own when you need the results somewhere other than the terminal: a JUnit XML file
for CI, a JSON report, or a service message protocol.

## The `Reporter` type

```fsharp
type Reporter =
    {
        OnResult: TestResult -> unit
        OnRunComplete: TestRunReport -> unit
    }
```

| Hook | When it runs |
| --- | --- |
| `OnResult` | As each result is produced, in completion order |
| `OnRunComplete` | Once, after every test has finished, with results in declaration order |

Build one from `Reporter.Default` and override only the hooks you need - the rest stay no-ops:

*)

let countingReporter =
    let mutable failures = 0

    { Reporter.Default with
        OnResult =
            fun result ->
                match result with
                | TestResult.Failed _ -> failures <- failures + 1
                | _ -> ()
    }

(**

## Registering reporters

Pass a reporter list to `runTests` or `runTestsWith`:

```fsharp
[<EntryPoint>]
let main _ =
    Runner.runTests(
        [ Reporters.console; countingReporter ],
        [ testList ("My suite", [ (* ... *) ]) ]
    )
```

The list **replaces** the default rather than adding to it, so include `Reporters.console`
whenever you still want terminal output. Reporters run in the order given.

| Call | Reporters used |
| --- | --- |
| `Runner.runTests(tests)` | `[ Reporters.console ]` |
| `Runner.runTests(reporters, tests)` | the list you pass |
| `Runner.runTestsWith(configurer, tests)` | `[ Reporters.console ]` |
| `Runner.runTestsWith(configurer, reporters, tests)` | the list you pass |

## What a run reports

`OnRunComplete` receives everything the run produced:

```fsharp
type TestRunReport =
    {
        Results: TestResult list
        StartTime: DateTime
        Duration: int
        PassedCount: int
        FailedCount: int
        SkippedCount: int
        PendingCount: int
        TotalCount: int
        AnyFocused: bool
    }
```

`Duration` is in milliseconds and `Results` are in declaration order, so a report writer can
emit them in the order the suite was written rather than the order tests happened to finish.

Each result carries what a report format needs:

| Case | Carries |
| --- | --- |
| `Passed` | `Path`, `FilePath`, `LineNumber`, `Duration`, `SlowThresholdMs` |
| `Failed` | the above plus `Message`, `ExceptionType`, `StackTrace` |
| `Skipped` | `Path`, `FilePath`, `LineNumber`, `Reason` |
| `Pending` | `Path`, `FilePath`, `LineNumber` |

`Path` runs leaf-first, so `List.rev` gives you root-to-leaf.

`ExceptionType` and `StackTrace` are `string option` because not every runtime can supply them.
On .NET and Python both are available; on JavaScript only the type name is, since
fable-library's `Exception` is deliberately not derived from `Error`; on the BEAM neither is,
because an F# exception is lowered to a bare map and the stacktrace lives on the catch clause.

## Example: a JUnit XML reporter

JUnit XML is the format most CI systems read - GitLab, Azure DevOps, Jenkins, CircleCI, and the
common GitHub Actions reporting actions all accept it.

*)

let private escape (value: string) =
    value
        .Replace("&", "&amp;")
        .Replace("<", "&lt;")
        .Replace(">", "&gt;")
        .Replace("\"", "&quot;")

/// JUnit splits a test's identity into a dotted `classname` and a `name`.
let private classNameAndName (path: string list) =
    match List.rev path with
    | [] -> "", ""
    | full ->
        let name = List.last full
        let className = full |> List.take (full.Length - 1) |> String.concat "."
        className, name

let private seconds (ms: int) =
    (float ms / 1000.0).ToString("0.000", CultureInfo.InvariantCulture)

let private testCaseXml (result: TestResult) =
    let path, duration, body =
        match result with
        | TestResult.Passed r -> r.Path, r.Duration, ""
        | TestResult.Failed r ->
            let kind = defaultArg r.ExceptionType "Exception"
            let detail = defaultArg r.StackTrace r.Message

            let failure =
                $"""<failure message="%s{escape r.Message}" type="%s{escape kind}">%s{escape detail}</failure>"""

            r.Path, r.Duration, failure
        | TestResult.Skipped r ->
            let reason =
                match r.Reason with
                | SkipReason.Configured -> "skipped by configuration"
                | SkipReason.NotFocused -> "not focused"

            r.Path, 0, $"""<skipped message="%s{reason}" />"""
        | TestResult.Pending r -> r.Path, 0, """<skipped message="pending" />"""

    let className, name = classNameAndName path

    $"""  <testcase classname="%s{escape className}" name="%s{escape name}" time="%s{seconds duration}">%s{body}</testcase>"""

let toJUnitXml (report: TestRunReport) =
    let header =
        $"""<testsuite name="Scriptorium" tests="%d{report.TotalCount}" failures="%d{report.FailedCount}" errors="0" skipped="%d{report.SkippedCount + report.PendingCount}" time="%s{seconds report.Duration}" timestamp="%s{report.StartTime.ToString("o", CultureInfo.InvariantCulture)}">"""

    [
        """<?xml version="1.0" encoding="UTF-8"?>"""
        header
        yield! report.Results |> List.map testCaseXml
        "</testsuite>"
    ]
    |> String.concat Environment.NewLine

(**

Writing it out is then a one-hook reporter:

*)

let junitReporter (path: string) =
    { Reporter.Default with
        OnRunComplete = fun report -> IO.File.WriteAllText(path, toJUnitXml report)
    }

(**

```fsharp
[<EntryPoint>]
let main _ =
    Runner.runTests(
        [ Reporters.console; junitReporter "test-results.xml" ],
        tests
    )
```

`System.IO.File.WriteAllText` works on .NET and on the Python target. On JavaScript you need
the platform's own API - `fs.writeFileSync` - the way `Scriptorium.Nib.Snapshot` does it.

## Choosing a hook

Use `OnRunComplete` for anything that needs the whole run: file formats, summaries, uploads. It
runs once, in the process that started the run, with results in declaration order.

Use `OnResult` only for streaming output that must appear while the run is still going - a
progress indicator, or a CI service-message protocol that expects one line per test.

:::caution
On the BEAM, `OnResult` runs inside the spawned job's own process. It can write output, but it
cannot reach state owned by the run, so accumulating results into a list or counter there will
not work. Collect from `OnRunComplete` instead, which always runs in the parent.
:::

## What the runner still handles

Two things are the runner's responsibility and happen whatever reporters you register, so you
never need to reimplement them:

- **Duplicate test paths** abort the run with an error before any test executes.
- **The CI focused-tests guard** - when `CI` is set and the suite contains `ftest` or
  `ftestList`, the run exits non-zero and prints why.

Dropping `Reporters.console` therefore cannot silently disable either check.

*)
