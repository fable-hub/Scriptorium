---
title: Platform Support
description: Which Scriptorium libraries run on which Fable target.
---

Scriptorium targets every runtime Fable can compile to. Not every library supports every target yet - some depend on APIs (reflection, filesystem, DOM) that haven't been mapped to a given target.

This table reflects the project's own test matrix, which is exercised in CI on every change:

| Library | .NET | JavaScript | TypeScript | Python | Beam (Erlang) |
|---|:---:|:---:|:---:|:---:|:---:|
| [`Scriptorium.Ink`](/Scriptorium/ink/overview/) | ✅ | ✅ | ✅ | ✅ | ✅ |
| [`Scriptorium.Parchment`](/Scriptorium/parchment/overview/) | ✅ | ✅ | ✅ | ✅ | ✅ |
| [`Scriptorium.Nib`](/Scriptorium/nib/overview/) | ✅ | ✅ | ✅ | ✅ | ✅ |
| [`Scriptorium.Quill`](/Scriptorium/quill/overview/) | ✅ | ✅ | ✅ | ✅ | ✅ |
| [`Scriptorium.Hedgehog`](/Scriptorium/hedgehog/overview/) | ✅ | ✅ | ✅ | ✅ | ✅ |
| [`Scriptorium.Nib.Snapshot`](/Scriptorium/nib-snapshot/overview/) | ✅ | ✅ | ✅ | ✅ | ❌ |
| [`Scriptorium.Nib.Browser`](/Scriptorium/nib-browser/overview/) | ❌ | ✅ | ✅ | ❌ | ❌ |

## Notes

- **TypeScript** compiles through the same pipeline as JavaScript and runs on the JavaScript runtime - it's listed separately here because it's tested as its own Fable target.
- **`Scriptorium.Nib.Browser`** wraps Playwright's Node.js API directly, so it's JavaScript-only by design.
- **`Scriptorium.Nib.Snapshot`** doesn't support Beam yet - it's deferred pending heavy filesystem I/O.

If a library doesn't list a target here, treat it as unsupported: it isn't compiled or tested against that target, and behaviour there is undefined.
