namespace Scriptorium.Hedgehog

open Hedgehog
open Hedgehog.FSharp

/// <summary>Configuration for reflection-based auto-generation.</summary>
/// <remarks>
/// Mirrors Hedgehog's <c>IAutoGenConfig</c>: a collection of explicit generators, the size range
/// used for generated collections, and the per-type recursion depth limit. Named <c>DeriveConfig</c>
/// here only to coexist with Hedgehog's own (TypeShape) <c>AutoGenConfig</c> on .NET.
/// </remarks>
type DeriveConfig =
    internal
        {
            SeqRange: Range<int> option
            RecursionDepth: int option
            Generators: GeneratorCollection
        }

/// Functions for building and tweaking an <see cref="DeriveConfig"/>. Mirrors the upstream Hedgehog
/// <c>AutoGenConfig</c> module.
[<RequireQualifiedAccess>]
module DeriveConfig =

    // Exponential (not linear) so most generated collections stay small and only the largest
    // sizes approach the bound - keeps auto-derivation of nested types cheap on average. Matches
    // upstream Hedgehog's AutoGenConfig.
    let private defaultSeqRange = Range.exponential 0 50
    let private defaultRecursionDepth = 1

    /// A configuration with no generators and default sizing/recursion - a base to add only the
    /// generators you want.
    let empty: DeriveConfig =
        {
            SeqRange = None
            RecursionDepth = None
            Generators = GeneratorCollection.empty
        }

    /// The default configuration: generators for the common primitive types, collection sizes in
    /// <c>[0, 50]</c>, and a per-type recursion depth of 1.
    let defaults: DeriveConfig =
        {
            SeqRange = Some defaultSeqRange
            RecursionDepth = Some defaultRecursionDepth
            Generators = DefaultGenerators.collection
        }

    /// The configured collection-size range (or the default).
    let seqRange (config: DeriveConfig) : Range<int> =
        config.SeqRange |> Option.defaultValue defaultSeqRange

    let setSeqRange (range: Range<int>) (config: DeriveConfig) : DeriveConfig =
        { config with SeqRange = Some range }

    /// The configured per-type recursion depth (or the default).
    let recursionDepth (config: DeriveConfig) : int =
        config.RecursionDepth |> Option.defaultValue defaultRecursionDepth

    let setRecursionDepth (depth: int) (config: DeriveConfig) : DeriveConfig =
        { config with RecursionDepth = Some depth }

    /// Merges two configurations; settings and generators in <paramref name="extra"/> win.
    let merge (baseConfig: DeriveConfig) (extra: DeriveConfig) : DeriveConfig =
        {
            SeqRange = extra.SeqRange |> Option.orElse baseConfig.SeqRange
            RecursionDepth = extra.RecursionDepth |> Option.orElse baseConfig.RecursionDepth
            Generators = GeneratorCollection.merge baseConfig.Generators extra.Generators
        }

    /// Registers a boxed generator for the given runtime <see cref="System.Type"/>. The typed
    /// <see cref="addGenerator"/> delegates here after resolving <c>typeof&lt;'a&gt;</c> at the
    /// call site (the <c>inline</c> hop is what makes it work under Fable).
    let addGeneratorFor (t: System.Type) (gen: Gen<obj>) (config: DeriveConfig) : DeriveConfig =
        { config with
            Generators = config.Generators |> GeneratorCollection.add t gen
        }

    /// Registers an explicit generator for type <c>'a</c>, overriding structural derivation for it.
    let inline addGenerator (gen: Gen<'a>) (config: DeriveConfig) : DeriveConfig =
        addGeneratorFor typeof<'a> (gen |> Gen.map box) config

// `addGenerators` (the "marker type" pattern: harvest every Gen-returning static member of a type)
// is a neat idea, but it can't be supported on the Fable side without significant changes there:
// Fable has no runtime member enumeration, and it erases the generic args we'd need to key each
// generator by its type.
// Exposed on neither Fable nor .NET for now; kept here, self-contained, in case Fable ever gains the
// needed reflection. To revive: uncomment and restore `open System.Reflection`.
// #if !FABLE_COMPILER
//     /// Boxes a typed generator into a `Gen<obj>` so it can be invoked reflectively (via
//     /// MakeGenericMethod) when registering generators harvested from a marker type.
//     type private BoxHelper =
//         static member Box<'a>(g: Gen<'a>) : Gen<obj> = g |> Gen.map box
//
//     /// <summary>Registers every generator exposed as a static, parameterless member of type
//     /// <c>'a</c> returning a <c>Gen&lt;_&gt;</c> (the "marker type" pattern).</summary>
//     let addGenerators<'a> (config: DeriveConfig) : DeriveConfig =
//         let genericGenDef = typedefof<Gen<int>>
//         let markerType = typeof<'a>

//         let returnsGen (returnType: System.Type) =
//             returnType.IsGenericType
//             && returnType.GetGenericTypeDefinition() = genericGenDef

//         let boxMethod = typeof<BoxHelper>.GetMethod("Box")

//         let harvested =
//             [
//                 for m in markerType.GetMethods(BindingFlags.Static ||| BindingFlags.Public) do
//                     if m.GetParameters().Length = 0 && returnsGen m.ReturnType then
//                         yield m.ReturnType.GetGenericArguments().[0], m.Invoke(null, [||])

//                 for p in markerType.GetProperties(BindingFlags.Static ||| BindingFlags.Public) do
//                     if returnsGen p.PropertyType then
//                         yield p.PropertyType.GetGenericArguments().[0], p.GetValue(null)
//             ]

//         (config, harvested)
//         ||> List.fold (fun acc (elementType, genObj) ->
//             let boxedGen =
//                 boxMethod.MakeGenericMethod(elementType).Invoke(null, [| genObj |]) :?> Gen<obj>

//             { acc with
//                 Generators = acc.Generators |> GeneratorCollection.add elementType boxedGen
//             }
//         )
// #endif
