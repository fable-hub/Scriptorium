module Repro

open System.Runtime.CompilerServices

// Mirrors Scriptorium.Hedgehog DSL.fs `Test` type: overloaded static members whose
// generic parameter erases to the same runtime type, plus optional CallerInfo args.
type Prop<'a> = { Value: 'a }

type Api =
    static member make(name: string, p: Prop<bool>, [<CallerFilePath>] ?fp: string, [<CallerLineNumber>] ?ln: int) : string = name
    static member make(name: string, p: Prop<unit>, [<CallerFilePath>] ?fp: string, [<CallerLineNumber>] ?ln: int) : string = name
    static member make(name: string, g: 'a, assertion: 'a -> unit, [<CallerFilePath>] ?fp: string, [<CallerLineNumber>] ?ln: int) : string = name
    static member makeWith(name: string, cfg: int, p: Prop<bool>, [<CallerFilePath>] ?fp: string, [<CallerLineNumber>] ?ln: int) : string = name
    static member makeWith(name: string, cfg: int, p: Prop<unit>, [<CallerFilePath>] ?fp: string, [<CallerLineNumber>] ?ln: int) : string = name

[<EntryPoint>]
let main _ =
    printfn "%s" (Api.make("a", { Value = true }))
    printfn "%s" (Api.make("b", { Value = () }))
    printfn "%s" (Api.make("c", 1, fun _ -> ()))
    printfn "%s" (Api.makeWith("d", 500, { Value = true }))
    0
