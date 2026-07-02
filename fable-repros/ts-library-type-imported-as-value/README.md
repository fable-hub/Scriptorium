# fable-library-ts imports interface types as values

## Reproduce

```bash
dotnet tool restore
dotnet fable --lang typescript -o out --noCache
node --experimental-strip-types out/Repro.ts
```

## Expected

Prints `42`.

## Actual

```
SyntaxError: The requested module './Util.ts' does not provide an export named 'IComparable'
```

(Depending on the reachable library graph the missing name may be `IDisposable`,
`IComparable`, etc. — all are `export interface` types in `Util.ts`.)

## Cause

`out/fable_modules/fable-library-ts.*/AsyncBuilder.ts` begins with:

```ts
import { Exception, ensureErrorOrException, IDisposable } from "./Util.ts";
```

`IDisposable` (and the others) are `export interface` — pure types with no runtime
value. Node's native type-stripping and `tsc --verbatimModuleSyntax` only erase
imports written as `import type` / `import { type X }`, so a type in a *value* import
position is treated as a missing runtime export.

## Workaround

Run the generated TypeScript through `tsx` / esbuild, or `tsc` without
`verbatimModuleSyntax` — they erase the unused type import. (Scriptorium's build uses
`tsx`.)

## Fix

Emit type-only imports for interface types in fable-library-ts, e.g.
`import { Exception, ensureErrorOrException, type IDisposable } from "./Util.ts";`.
