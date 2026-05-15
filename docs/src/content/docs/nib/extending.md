---
title: Extending
description: How to extend Scriptorium.Nib with custom assertions and new assertion types.
---

`Scriptorium.Nib` is built around a single, small abstraction: the `Assertion<'a, 'b>` type.

Everything else — the built-in assertions, the combinators, and the packages that extend Nib — is built on top of it.

## The core abstraction

An assertion is a function that transforms an `AssertionState<'a>` into an `AssertionState<'b>`:

```fsharp
type Assertion<'a, 'b> = AssertionState<'a> -> AssertionState<'b>
```

The state carries four pieces of information:

| Field | Purpose |
| --- | --- |
| `Subject` | The current value under test |
| `Errors` | Accumulated failure messages |
| `Tags` | Active label stack for error prefixes |
| `Stopped` | Whether the chain has been force-stopped |

Assertions **never** throw mid-chain. They append to `Errors` and continue. Only `assertThat` throws, and only after the entire chain has run.

## Creating a custom assertion

The `assertion` function is the primitive building block. Give it a predicate and an error message formatter:

```fsharp
let isPositive: Assertion<int> =
    assertion
        (fun n -> n > 0)
        (fun n -> $"expected a positive number but got {n}")

assertThat 42 isPositive        // passes
assertThat -1 isPositive        // throws: expected a positive number but got -1
```

Custom assertions compose with built-in ones using `>>`:

```fsharp
let isValidEmail: Assertion<string> =
    assertion
        (fun s -> s.Contains("@") && s.Contains("."))
        (fun s -> $"'{s}' is not a valid email address")

assertThat "alice@example.com" (isNotEqualTo "" >> isValidEmail)
```

## Extending `TestContext`

You can extend `TestContext` with type extensions to provide domain-specific assertion helpers that automatically use contextual information like the test path and file location.

For example, `Scriptorium.Nib.Snapshot` extends `TestContext` with `snapshot` and `snapshotWith` methods that derive the snapshot name from `ctx.Path` and the file path from `ctx.FilePath`.

```fsharp
namespace Scriptorium.Nib.Snapshot

open Scriptorium.Quill
open Scriptorium.Nib
open Scriptorium.Nib.Assertion

[<AutoOpen>]
module SnapshotContextExtensions =

    type TestContext with

        member ctx.snapshot(value: 'a, ?config: SnapshotConfig) =
            let name = ctx.Path |> String.concat " > "

            assertThat
                value
                (Snapshot.matchesWith (
                    Advanced.defaultSerialize,
                    name,
                    ?config = config,
                    ?testFilePath = Some ctx.FilePath
                ))
```

This lets test authors write the shorter form:

```fsharp
test("user profile", fun t ->
    let data = {| Name = "Alice"; Age = 30 |}
    t.snapshot data
)
```

Instead of manually threading the snapshot name and file path:

```fsharp
test("user profile", fun t ->
    let data = {| Name = "Alice"; Age = 30 |}
    assertThat
        data
        (Snapshot.matches t.Name)
)
```

## Patterns for extension

| What you need | Approach | Example |
| --- | --- | --- |
| A new assertion on existing subjects | Use `assertion` | `isValidEmail` |
| A new assertion that needs external state (files, network) | Use `assertion` with side effects in the predicate | `Snapshot.matches` |
| Async assertions (Promise-based) | Define a new async assertion type, mirror Nib's operators | `DomAssertion<'a>` |
| Custom combinators | Manipulate `AssertionState` directly | `inside`, `focus`, `tag` |
| Nicer integration with `TestContext` | Extend `TestContext` with instance members | `t.snapshot data` |

The key insight is that `AssertionState` is public and unconstrained. You can read from it, write to it, and transform it however you need — as long as you respect the invariant that errors accumulate and `Stopped` short-circuits subsequent work.
