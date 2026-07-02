# Fable TypeScript & Python reproductions

Issues found while adding TypeScript and Python targets to the Scriptorium ecosystem
(Fable `5.5.0`, Fable.Core `5.2.0`, fable-library `5.5.0`). All are Fable / fable-library bugs
or gaps, not issues in Scriptorium's own code — the ecosystem otherwise compiles and passes its
test suites on both targets (183 tests each across Ink, Nib, Nib.Snapshot, Parchment, Quill;
Hedgehog is excluded on both, see below).

Each subfolder is self-contained (`dotnet tool restore` to get the pinned Fable).

## 1. `ts-library-type-imported-as-value/` — fable-library-ts imports types as values

`AsyncBuilder.ts` (and others) do `import { ..., IDisposable } from "./Util.ts"` where
`IDisposable` / `IComparable` are `export interface` — pure types. Mixing a type into a value
import breaks any consumer that strips types syntactically rather than semantically:

- `node --experimental-strip-types` / Node's native `.ts` execution
- `tsc` / bundlers configured with `verbatimModuleSyntax: true`

Minimal, deterministic: a 3-line `Async` program is enough. Workaround: run the output through
`tsx`/esbuild or `tsc` without `verbatimModuleSyntax` (they erase the dangling type import).
Proper fix: emit `import type { IDisposable }` (or `import { type IDisposable }`) in fable-library-ts.

## 2. `ts-overload-name-collision/` — overloaded generic members collide (non-deterministic)

A type with overloaded members whose generic parameter erases to the same runtime type
(e.g. `member make(_, Prop<bool>)` and `member make(_, Prop<unit>)`) sometimes mangles both
overloads to the **same** name on the TS target:

```
error FABLE: Cannot have two module members with same name: Test_property_535CAFD9
```

It is **non-deterministic**: a cold compile usually disambiguates correctly (distinct hashes
`Test_property_743B3C03`, `Test_property_Z404AA92B`, …), while cached / incremental compiles
frequently collapse them to one hash and fail. It reproduces reliably in `Scriptorium.Hedgehog`
(`src/Scriptorium.Hedgehog/DSL.fs`, the `Test.property` / `Test.propertyWith` overloads) but did
**not** reproduce in a minimal single-project sample — it appears to need the larger cross-project
member set. See that folder's README for exact commands and the reduced sample.

Because of this, `Scriptorium.Hedgehog` is currently excluded from the TypeScript test matrix
(`build/Commands/Test.fs`).

## 3. `python-library-gaps/` — Python APIs that compile but fail at runtime

`System.Diagnostics.Stopwatch.StartNew` (missing `start_new` in `fable_library.diagnostics`) and
`System.Text.Json` (emits a throwing `raise int32.ONE` stub instead of a compile error) both pass
Fable compilation but crash the generated Python. Reflection-based generic derivation
(`Scriptorium.Hedgehog`'s `Derive`) also does not run under fable-library-python, so Hedgehog is
excluded from the Python test matrix too. See that folder's README.
