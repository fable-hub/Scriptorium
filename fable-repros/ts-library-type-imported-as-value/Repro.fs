module Repro
// Any Async use pulls in fable-library-ts AsyncBuilder.ts / Util.ts, which import the
// *types* IDisposable / IComparable (etc.) as values. Node's native type-stripping then
// fails on those modules. `Async.StartImmediate` is fully supported by Fable - the failure
// is purely in the generated library imports, not in this program.
[<EntryPoint>]
let main _ =
    Async.StartImmediate(async { printfn "42" })
    0
