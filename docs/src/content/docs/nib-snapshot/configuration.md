---
title: Configuration
description: SnapshotConfig fields, defaults, and CI behaviour.
---

`Fable.Nib.Snapshot` behaviour is controlled by `SnapshotConfig` — a plain record with sensible defaults.

## Fields

| Field | Type | Default | Description |
|---|---|---|---|
| `DirectoryName` | `string` | `"__snapshots__"` | Name of the directory that holds snapshot files, relative to the test file. |
| `Extension` | `string` | `".snap"` | File extension for snapshot files (including the dot). |
| `UpdateEnvironmentVariable` | `string` | `"UPDATE_SNAPSHOTS"` | Environment variable that triggers snapshot updates when set. |
| `AutoCreate` | `bool` | `true` | When `true`, missing snapshots are created automatically and the test passes. When `false`, missing snapshots cause the test to fail. |

## Overriding defaults

Use record update syntax to override only the fields you need:

```fsharp
let ciConfig =
    { SnapshotConfig.Default with
        AutoCreate = false
    }

assertThat myValue (Snapshot.matches ("my snapshot", config = ciConfig))
```

## CI behaviour

:::caution
Even when `AutoCreate` is `true`, snapshot creation is **automatically disabled in CI environments** (when the `CI` environment variable is set). This prevents uncommitted snapshots from being silently generated on build agents.
:::
