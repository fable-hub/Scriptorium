---
title: Overview
description: Snapshot testing for Scriptorium.Nib - lock in serialised output and catch regressions automatically.
---

`Scriptorium.Nib.Snapshot` adds snapshot testing on top of `Scriptorium.Nib`. On first run the snapshot is written to disk; on every subsequent run the value is serialised and compared against the stored file.

## Installation

```sh
dotnet add package Scriptorium.Nib.Snapshot
```

## How it works

1. **First run** - no snapshot exists yet, it is written and the test passes.
2. **Subsequent runs** - the value is serialised and diffed against the stored snapshot.
3. **Updating** - set `UPDATE_SNAPSHOTS=1` and run the tests. All snapshots are rewritten.

Snapshot files live in a `__snapshots__` directory next to the test file with a `.snap` extension.

## Usage with Scriptorium.Quill

The recommended approach is `ctx.snapshot` - it uses the full test path as the snapshot name automatically, so snapshot names are always unique and renaming a test is enough to regenerate its snapshot:

```fsharp
test ("user record", fun t ->
    t.snapshot { Name = "alice"; Age = 25 }
)

// Custom serialiser
test ("DU shape", fun t ->
    t.snapshotWith (sprintf "%A", Shape.Circle 5.0)
)
```

## Standalone usage

Without a `TestContext`, use `Snapshot.matches` directly:

```fsharp
test ("user snapshot", fun _ ->
    assertThat
        { Name = "alice"; Age = 25 }
        (Snapshot.matches "user snapshot")
)
```

The first run writes:

```
=== user snapshot ===
{
  "Name": "alice",
  "Age": 25
}
=== end-snapshot ===
```

`Snapshot.matchesWith` accepts a custom serialiser as the first argument:

```fsharp
assertThat (Shape.Circle 5.0) (Snapshot.matchesWith (sprintf "%A", "circle snapshot"))
```

## Updating snapshots

```sh
UPDATE_SNAPSHOTS=1 dotnet fable MyApp.Tests/ --runScript
```

All snapshot assertions update their stored value and pass. Commit the updated `.snap` files.

## Snapshot file format

Multiple snapshots from the same test file share one `.snap` file, keyed by name:

```text
=== My suite > user snapshot ===
{
  "Name": "alice",
  "Age": 25
}
=== end-snapshot ===

=== My suite > shape snapshot ===
{
  "tag": "Circle",
  "fields": [5.0]
}
=== end-snapshot ===
```
