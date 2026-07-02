---
title: Configuration
description: TestConfig, skipIf, timeout, slowThreshold, and how configuration is inherited.
---

`Scriptorium.Quill` lets you configure every test and list with a `TestConfig -> TestConfig` function. Configuration is inherited from the global level down through `testList` to individual tests, with each level able to override its parent.

```fs
open Scriptorium.Quill
```

## TestConfig

```fsharp
type TestConfig =
    {
        Skip: bool
        TimeoutMs: int option
        SlowThresholdMs: int
    }
```

| Field | Default | Description |
|---|---|---|
| `Skip` | `false` | When `true`, the test is skipped at runtime |
| `TimeoutMs` | `Some 5000` | Maximum allowed duration in ms. `None` = no timeout |
| `SlowThresholdMs` | `300` | Tests slower than this show their time in yellow |

You never construct `TestConfig` directly - use the built-in configurers and compose them with `>>`.

## Built-in configurers

### `skipIf`

```fsharp
skipIf (condition: bool) : TestConfig -> TestConfig
```

Skips the test if `condition` is `true`. The test body does not execute; the result is `Skipped`.

```fsharp
test ("conditional skip", skipIf (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)), fun _ ->
    // Only runs on non-Windows
    assertThat true isTrue
)
```

### `skipIfJavaScript`

```fsharp
skipIfJavaScript : TestConfig -> TestConfig
```

Skips when compiled to JavaScript (Fable).

```fsharp
test ("uses .NET DateTime", skipIfJavaScript, fun _ ->
    assertThat System.DateTime.Now.Year (isGreaterThan 2020)
)
```

### `skipIfDotNet`

```fsharp
skipIfDotNet : TestConfig -> TestConfig
```

Skips when running on .NET (i.e. not compiled by Fable).

```fsharp
test ("uses browser fetch", skipIfDotNet, fun _ ->
    assertThat window isNotNull
)
```

### `timeout`

```fsharp
timeout (ms: int) : TestConfig -> TestConfig
```

Sets a timeout for the test. Async tests that exceed the timeout fail with `"Test timed out after Nms"`. Synchronous tests are checked retroactively after the body returns.

```fsharp
testAsync ("slow network call", timeout 10_000, fun _ -> async {
    let! result = httpGetAsync "https://api.example.com/data"
    assertThat result.StatusCode (isEqualTo 200)
})
```

### `noTimeout`

```fsharp
noTimeout : TestConfig -> TestConfig
```

Removes the timeout entirely. The test can run as long as it needs.

```fsharp
testAsync ("very slow integration test", noTimeout, fun _ -> async {
    let! result = runFullPipelineAsync ()
    assertThat result.Success isTrue
})
```

### `slowThreshold`

```fsharp
slowThreshold (ms: int) : TestConfig -> TestConfig
```

Sets the threshold above which a passing test's duration is displayed in yellow. Default is 300 ms.

```fsharp
testList ("integration suite", slowThreshold 1000, [
    // Tests in this list are only highlighted as slow if > 1000ms
    test ("database roundtrip", fun _ -> ...)
])
```

## Composing configurers

Configurers are plain functions; compose them with `>>`:

```fsharp
test ("platform + timeout", skipIfJavaScript >> timeout 2000, fun _ ->
    // Only runs on .NET, with a 2s timeout
    runHeavyComputation ()
)
```

## Configuration inheritance

Configuration flows from global, list, test. Each level can override its parent's values.

```
Runner.runTestsWith(configurer, tests)          global
    └── testList(name, configurer, ...)         list override
            └── test(name, configurer, ...)     test override
```

When a child configurer is applied, it receives the configuration inherited from its parent. The final configuration for a test is:

```
testConfig = testConfigurer (listConfigurer (globalConfig))
```

### Example: timeout inheritance

```fsharp
Runner.runTestsWith(timeout 5000, [    // global: 5s
    testList ("group", timeout 2000, [ // list overrides: 2s
        testAsync ("fast", fun _ -> async { do! Async.Sleep 100 })  // inherits 2s - passes
        testAsync ("slow", timeout 10000, fun _ -> async {          // test overrides: 10s - passes
            do! Async.Sleep 5000
        })
        testAsync ("too slow", fun _ -> async {  // inherits 2s - fails after 2s
            do! Async.Sleep 3000
        })
    ])
])
```
