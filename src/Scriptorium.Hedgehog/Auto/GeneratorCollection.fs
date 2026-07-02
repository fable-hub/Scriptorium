namespace Scriptorium.Hedgehog

open System
open Hedgehog

/// <summary>A registry of explicit generators keyed by target type.</summary>
/// <remarks>
/// The auto-generator consults this collection (by the target type's full name) before falling
/// back to structural derivation, so a registered generator always wins for its type.
/// </remarks>
type GeneratorCollection =
    internal
        {
            Generators: Map<string, Gen<obj>>
        }

[<RequireQualifiedAccess>]
module internal GeneratorCollection =

    let empty: GeneratorCollection = { Generators = Map.empty }

    let add (t: Type) (gen: Gen<obj>) (collection: GeneratorCollection) : GeneratorCollection =
        { Generators = collection.Generators |> Map.add t.FullName gen }

    let tryFind (t: Type) (collection: GeneratorCollection) : Gen<obj> option =
        collection.Generators |> Map.tryFind t.FullName

    /// Merges two collections; entries in <paramref name="extra"/> override those in
    /// <paramref name="baseCollection"/>.
    let merge (baseCollection: GeneratorCollection) (extra: GeneratorCollection) : GeneratorCollection =
        let merged =
            (baseCollection.Generators, extra.Generators)
            ||> Map.fold (fun acc key value -> Map.add key value acc)

        { Generators = merged }
