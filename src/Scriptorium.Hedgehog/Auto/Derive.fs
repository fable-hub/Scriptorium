namespace Scriptorium.Hedgehog

open System
open FSharp.Reflection
open Fable.Core
open Hedgehog
open Hedgehog.FSharp

/// <summary>Reflection-based automatic generator derivation for Hedgehog.</summary>
/// <remarks>
/// A drop-in alternative to Hedgehog's TypeShape-based <c>Gen.auto</c> / <c>Gen.autoWith</c>,
/// implemented with <c>FSharp.Reflection</c> instead of TypeShape. <c>Derive.gen&lt;'T&gt;</c>
/// produces a Hedgehog <c>Gen&lt;'T&gt;</c> usable anywhere a hand-written generator is.
///
/// Supported: the registered primitive types (see <c>DefaultGenerators</c>), tuples, records
/// (including anonymous), discriminated unions (including recursive ones, and <c>option</c>),
/// enums, <c>list</c>, 1-D arrays, <c>seq</c>, F# <c>Set</c> and <c>Map</c>. Register anything
/// else with <c>DeriveConfig.addGenerator</c>.
///
/// Recursion is bounded by the config's per-type depth: collections bottom out empty and unions
/// prefer non-recursive cases once the limit is reached; a type that can only recurse (no finite
/// value) raises a clear error.
///
/// Works on both .NET and Fable: the typed entry points (<c>gen</c>/<c>genWith</c>) are
/// <c>inline</c> so <c>typeof&lt;'T&gt;</c> resolves at the call site (Fable erases generics
/// otherwise) and delegate to a non-inline walker over the runtime <see cref="System.Type"/>.
/// </remarks>
[<RequireQualifiedAccess>]
module Derive =

    let private listDef = typedefof<obj list>
    let private seqDef = typedefof<seq<obj>>
    let private setDef = typedefof<Set<int>>
    let private mapDef = typedefof<Map<int, int>>

    let private isGenericOf (genericDef: Type) (t: Type) : bool =
        t.IsGenericType && t.GetGenericTypeDefinition() = genericDef

    // -- Reflective value construction ----------------------------------------

    let private makeTypedList (elementType: Type) (values: obj list) : obj =
        if Compiler.isDotnet then
            let listType = listDef.MakeGenericType [| elementType |]
            let cases = FSharpType.GetUnionCases listType
            let emptyCase = cases |> Array.find (fun c -> c.GetFields().Length = 0)
            let consCase = cases |> Array.find (fun c -> c.GetFields().Length = 2)
            let nil = FSharpValue.MakeUnion(emptyCase, [||])

            List.foldBack
                (fun x acc ->
                    FSharpValue.MakeUnion(
                        consCase,
                        [|
                            x
                            acc
                        |]
                    )
                )
                values
                nil
        else
            // Generics are erased under Fable, so the `obj list` already *is* a valid `'t list` at
            // runtime - no reflective cons/nil rebuild (F# list isn't a reflectable union there).
            box values

    let private makeTypedArray (elementType: Type) (values: obj list) : obj =
        #if FABLE_COMPILER
        // Generics are erased under Fable, so a plain JS array of the boxed values already *is*
        // the typed array - no reflective element-typed allocation is needed (or supported).
        // `Array.CreateInstance`/`SetValue` are not supported by Fable, so this branch stays a
        // compiler directive rather than a `Compiler.isDotnet` runtime check.
        box (List.toArray values)
        #else
            let arr = Array.CreateInstance(elementType, List.length values)
            values |> List.iteri (fun i v -> arr.SetValue(v, i))
            box arr
        #endif

    let private makeTypedSet (elementType: Type) (values: obj list) : obj =
        if Compiler.isDotnet then
            let setType = setDef.MakeGenericType [| elementType |]
            Activator.CreateInstance(setType, [| makeTypedList elementType values |])
        else
            // obj carries no `comparison` constraint, but Fable erases generics and compares
            // structurally at runtime, so we borrow an arbitrary comparable witness type to satisfy
            // the type-checker. The cast must be applied to the WHOLE list (a reference value -> a
            // genuine identity coercion under Fable). A per-element `unbox<int>` would int-coerce any
            // non-numeric value to 0 (a union case object becomes `0`, etc.), silently corrupting
            // Set/Map of user types. With the list-level cast the element values are untouched and
            // Fable's structural comparer keys the Set/Map by the real values.
            (box values |> unbox<int list>) |> Set.ofList |> box

    let private makeTypedMap (keyType: Type) (valueType: Type) (pairs: obj list) : obj =
        if Compiler.isDotnet then
            let tupleType =
                FSharpType.MakeTupleType
                    [|
                        keyType
                        valueType
                    |]

            let mapType =
                mapDef.MakeGenericType
                    [|
                        keyType
                        valueType
                    |]

            Activator.CreateInstance(mapType, [| makeTypedList tupleType pairs |])
        else
            // See makeTypedSet for why the cast is applied to the whole list.
            (box pairs |> unbox<(int * obj) list>) |> Map.ofList |> box

    let rec private build (config: DeriveConfig) (state: RecursionState) (t: Type) : Gen<obj> =
        let maxDepth = DeriveConfig.recursionDepth config

        match RecursionState.reconcile maxDepth t state with
        | None ->
            Gen.delay (fun () ->
                raise (
                    InvalidOperationException(
                        sprintf
                            "Derive: recursion depth limit %d exceeded for type '%s'. It has no finite value; register an explicit generator with DeriveConfig.addGenerator."
                            maxDepth
                            t.FullName
                    )
                )
            )
        | Some state ->
            match GeneratorCollection.tryFind t config.Generators with
            | Some gen -> gen
            | None -> buildStructural config state t

    and private buildStructural
        (config: DeriveConfig)
        (state: RecursionState)
        (t: Type)
        : Gen<obj>
        =
        if t = typeof<unit> then
            Gen.constant (box ())
        // `Type.GetArrayRank` is unsupported by Fable, so the 1-D restriction stays a directive.
#if FABLE_COMPILER
        elif t.IsArray then
            buildArray config state t
#else
        elif t.IsArray && t.GetArrayRank() = 1 then
            buildArray config state t
#endif
        elif isGenericOf listDef t then
            buildList config state t
        elif isGenericOf seqDef t then
            buildSeq config state t
        elif isGenericOf setDef t then
            buildSet config state t
        elif isGenericOf mapDef t then
            buildMap config state t
        elif FSharpType.IsUnion(t, true) then
            buildUnion config state t
        elif FSharpType.IsRecord(t, true) then
            buildRecord config state t
        elif FSharpType.IsTuple t then
            buildTuple config state t
        elif t.IsEnum then
            buildEnum t
        else
            raise (
                NotSupportedException(
                    sprintf
                        "Derive: no automatic generator for type '%s'. Register one with DeriveConfig.addGenerator."
                        t.FullName
                )
            )

    and private buildElements
        (config: DeriveConfig)
        (state: RecursionState)
        (elementType: Type)
        : Gen<obj list>
        =
        // Elements use the unchanged size (no per-descent scaling) - parity with Hedgehog's auto-gen,
        // which also only depth-limits per type. So deep heterogeneous nesting multiplies (~seqRange
        // per level) and is slow on this reflection backend; bound seqRange for such types. Depth-
        // scaled sizing (Gen.scale per level) was considered and deliberately deferred.
        if state.CanRecurse then
            build config state elementType |> Gen.list (DeriveConfig.seqRange config)
        else
            Gen.constant []

    and private buildList config state (t: Type) : Gen<obj> =
        let elementType = t.GetGenericArguments().[0]
        buildElements config state elementType |> Gen.map (makeTypedList elementType)

    and private buildSeq config state (t: Type) : Gen<obj> =
        // A typed list is a valid seq<'t>.
        let elementType = t.GetGenericArguments().[0]
        buildElements config state elementType |> Gen.map (makeTypedList elementType)

    and private buildArray config state (t: Type) : Gen<obj> =
        let elementType = t.GetElementType()
        buildElements config state elementType |> Gen.map (makeTypedArray elementType)

    and private buildSet config state (t: Type) : Gen<obj> =
        let elementType = t.GetGenericArguments().[0]
        buildElements config state elementType |> Gen.map (makeTypedSet elementType)

    and private buildMap config state (t: Type) : Gen<obj> =
        let args = t.GetGenericArguments()
        let keyType, valueType = args.[0], args.[1]

        let tupleType =
            FSharpType.MakeTupleType
                [|
                    keyType
                    valueType
                |]

        let pairGen =
            if state.CanRecurse then
                Gen.sequenceList
                    [
                        build config state keyType
                        build config state valueType
                    ]
                |> Gen.map (fun kv -> FSharpValue.MakeTuple(List.toArray kv, tupleType))
                |> Gen.list (DeriveConfig.seqRange config)
            else
                Gen.constant []

        pairGen |> Gen.map (makeTypedMap keyType valueType)

    and private buildRecord config state (t: Type) : Gen<obj> =
        FSharpType.GetRecordFields(t, true)
        |> Array.toList
        |> List.map (fun p -> build config state p.PropertyType)
        |> Gen.sequenceList
        |> Gen.map (fun values -> FSharpValue.MakeRecord(t, List.toArray values, true))

    and private buildTuple config state (t: Type) : Gen<obj> =
        FSharpType.GetTupleElements t
        |> Array.toList
        |> List.map (build config state)
        |> Gen.sequenceList
        |> Gen.map (fun values -> FSharpValue.MakeTuple(List.toArray values, t))

    and private buildUnion config state (t: Type) : Gen<obj> =
        let cases = FSharpType.GetUnionCases(t, true)

        // A case is non-recursive when none of its fields refer back to the union type.
        let isLeaf (c: UnionCaseInfo) =
            c.GetFields() |> Array.forall (fun p -> p.PropertyType.FullName <> t.FullName)

        let usableCases =
            if state.CanRecurse then
                cases
            else
                match cases |> Array.filter isLeaf with
                | [||] -> cases
                | leaves -> leaves

        let caseGen (c: UnionCaseInfo) =
            c.GetFields()
            |> Array.toList
            |> List.map (fun p -> build config state p.PropertyType)
            |> Gen.sequenceList
            |> Gen.map (fun values -> FSharpValue.MakeUnion(c, List.toArray values, true))

        usableCases |> Array.map caseGen |> Gen.choice

    and private buildEnum (t: Type) : Gen<obj> =
        let values = [ for v in Enum.GetValues t -> box v ]
        Gen.item values

    /// Derives a boxed generator for the given runtime <see cref="System.Type"/>. The typed
    /// entry points below delegate here after resolving <c>typeof&lt;'a&gt;</c> at the call site
    /// (an <c>inline</c> hop is what lets this work under Fable, where generics are erased).
    let genBoxedWith (config: DeriveConfig) (t: Type) : Gen<obj> =
        build config RecursionState.empty t

    /// Derives a generator for <c>'a</c> using the given configuration.
    let inline genWith<'a> (config: DeriveConfig) : Gen<'a> =
        genBoxedWith config typeof<'a> |> Gen.map (fun o -> unbox<'a> o)

    /// Derives a generator for <c>'a</c> using the default configuration.
    let inline gen<'a> : Gen<'a> = genWith<'a> DeriveConfig.defaults
