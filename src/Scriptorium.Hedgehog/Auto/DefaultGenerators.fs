namespace Scriptorium.Hedgehog

open System
open Hedgehog
open Hedgehog.FSharp

/// The default registered generators for primitive / leaf .NET types. Structural types
/// (records, unions, tuples, collections, …) are derived by the walker, not registered here.
module internal DefaultGenerators =

    let private boxed (g: Gen<'a>) : Gen<obj> = g |> Gen.map box

    let private dateTimeRange =
        Range.constant (DateTime(2000, 1, 1)) (DateTime(2050, 12, 31))

    let collection: GeneratorCollection =
        GeneratorCollection.empty
        |> GeneratorCollection.add typeof<bool> (boxed Gen.bool)
        |> GeneratorCollection.add typeof<byte> (boxed (Gen.byte (Range.exponentialBounded ())))
        |> GeneratorCollection.add typeof<int16> (boxed (Gen.int16 (Range.exponentialBounded ())))
        |> GeneratorCollection.add typeof<uint16> (boxed (Gen.uint16 (Range.exponentialBounded ())))
        |> GeneratorCollection.add typeof<int> (boxed (Gen.int32 (Range.exponentialBounded ())))
        |> GeneratorCollection.add typeof<uint32> (boxed (Gen.uint32 (Range.exponentialBounded ())))
        |> GeneratorCollection.add typeof<int64> (boxed (Gen.int64 (Range.exponentialBounded ())))
        |> GeneratorCollection.add typeof<uint64> (boxed (Gen.uint64 (Range.exponentialBounded ())))
        |> GeneratorCollection.add typeof<single> (boxed (Gen.single (Range.constant -1e9f 1e9f)))
        |> GeneratorCollection.add typeof<double> (boxed (Gen.double (Range.constant -1e9 1e9)))
        |> GeneratorCollection.add typeof<decimal> (boxed (Gen.decimal (Range.constant -1e9m 1e9m)))
        |> GeneratorCollection.add typeof<char> (boxed Gen.alphaNum)
        |> GeneratorCollection.add typeof<string> (boxed (Gen.string (Range.linear 0 50) Gen.alphaNum))
        |> GeneratorCollection.add typeof<Guid> (boxed Gen.guid)
        |> GeneratorCollection.add typeof<DateTime> (boxed (Gen.dateTime dateTimeRange))
