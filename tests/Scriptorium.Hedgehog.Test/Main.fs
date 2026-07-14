module Scriptorium.Hedgehog.Test.Main

open Hedgehog
open Hedgehog.FSharp
open Scriptorium.Nib.Assertion
open Scriptorium.Hedgehog
open type Scriptorium.Quill.Test
open type Scriptorium.Quill.Runner

// Types exercised by the Derive (auto-generation) tests.
type Suit =
    | Hearts
    | Spades
    | Clubs
    | Diamonds

type Card =
    {
        Suit: Suit
        Rank: int
        Tags: string list
    }

[<RequireQualifiedAccess>]
type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

[<RequireQualifiedAccess>]
type Tree =
    | Leaf of int
    | Branch of Tree * Tree

let rec treeDepth (t: Tree) =
    match t with
    | Tree.Leaf _ -> 1
    | Tree.Branch (l, r) -> 1 + max (treeDepth l) (treeDepth r)

type UserId = UserId of int

let tests =
    testList (
        "Scriptorium.Hedgehog",
        [
            // The Hedgehog manual API (Gen / Range / property { }) driven through the Quill runner.
            Test.testProperty (
                "List.rev is involutive (property CE)",
                property {
                    let! xs = Gen.list (Range.linear 0 100) (Gen.int32 (Range.constant 0 1000))
                    return List.rev (List.rev xs) = xs
                }
            )

            Test.testProperty (
                "addition is commutative",
                property {
                    let! a = Gen.int32 (Range.linear -1000 1000)
                    let! b = Gen.int32 (Range.linear -1000 1000)
                    return a + b = b + a
                }
            )

            // The Nib bridge: generate values, assert with Scriptorium.Nib's assertThat.
            Test.testProperty (
                "doubling equals adding to itself (Nib assertThat)",
                Gen.int32 (Range.linear -1000 1000),
                fun n -> assertThat (n * 2) (isEqualTo (n + n))
            )

            // An explicit Hedgehog PropertyConfig (custom test count).
            Test.testPropertyWith (
                "runs with an explicit config (500 tests)",
                PropertyConfig.defaults |> PropertyConfig.withTests 500<tests>,
                property {
                    let! n = Gen.int32 (Range.constant 0 100)
                    return n >= 0
                }
            )

            // Failure paths: confirm a falsified property fails the test (Property.check throws).
            test (
                "a falsifiable property fails the test",
                fun _ ->
                    let prop =
                        property {
                            let! n = Gen.int32 (Range.linear 0 1000)
                            return n < 10
                        }

                    let threw =
                        try
                            Property.checkBool prop
                            false
                        with _ ->
                            true

                    assertThat threw isTrue
            )

            test (
                "a failing Nib assertion inside a property fails the test",
                fun _ ->
                    let prop =
                        property {
                            let! n = Gen.int32 (Range.constant 50 100)
                            return (assertThat n (isLessThan 10)
                                    true)
                        }

                    let threw =
                        try
                            Property.checkBool prop
                            false
                        with _ ->
                            true

                    assertThat threw isTrue
            )

            testList (
                "Derive (reflection-based auto-gen)",
                [
                    Test.testProperty (
                        "derives records with primitives and collections",
                        property {
                            let! card = Derive.gen<Card>
                            return card.Rank = card.Rank && card.Tags.Length >= 0
                        }
                    )

                    Test.testProperty (
                        "derives discriminated unions",
                        property {
                            let! shape = Derive.gen<Shape>

                            return
                                match shape with
                                | Shape.Circle r -> r = r
                                | Shape.Rectangle (w, h) -> w = w && h = h
                        }
                    )

                    Test.testProperty (
                        "derives enums",
                        property {
                            let! suit = Derive.gen<Suit>
                            return List.contains suit [ Hearts; Spades; Clubs; Diamonds ]
                        }
                    )

                    Test.testProperty (
                        "derives tuples and options",
                        property {
                            let! (n, maybe) = Derive.gen<int * string option>

                            return
                                (n = n)
                                && (match maybe with
                                    | Some s -> s.Length >= 0
                                    | None -> true)
                        }
                    )

                    Test.testProperty (
                        "derives F# Set and Map",
                        property {
                            let! set = Derive.gen<Set<int>>
                            let! map = Derive.gen<Map<string, int>>
                            return set.Count >= 0 && map.Count >= 0
                        }
                    )

                    // Collections of NON-primitive element types: the element type recurses back
                    // through the full walker, so any user type should compose under list/array/
                    // seq/Set/Map.
                    Test.testProperty (
                        "derives a list of a user record",
                        property {
                            let! cards = Derive.gen<Card list>
                            return cards |> List.forall (fun c -> c.Rank = c.Rank && c.Tags.Length >= 0)
                        }
                    )

                    Test.testProperty (
                        "derives an array",
                        property {
                            let! arr = Derive.gen<int array>
                            return arr.Length >= 0
                        }
                    )

                    Test.testProperty (
                        "derives a seq",
                        property {
                            let! xs = Derive.gen<seq<int>>
                            return Seq.length xs >= 0
                        }
                    )

                    Test.testProperty (
                        "derives Set and Map of user types",
                        property {
                            // Card's nested Tags list makes each map entry a full record derivation,
                            // so bound this one test (same reasoning as the nested-collection test below).
                            let config = DeriveConfig.defaults |> DeriveConfig.setSeqRange (Range.linear 0 5)
                            let! suits = Derive.genWith<Set<Suit>> config
                            let! cards = Derive.genWith<Map<string, Card>> config

                            return
                                suits |> Set.forall (fun s -> List.contains s [ Hearts; Spades; Clubs; Diamonds ])
                                && cards |> Map.forall (fun _ c -> c.Tags.Length >= 0)
                        }
                    )

                    Test.testProperty (
                        "derives a nested collection (Map of user key to list of records)",
                        property {
                            // Exponential sizing keeps the shallow cases fast on the default, but a
                            // 3-level nest still hits ~50^3 at the largest size, so bound this one
                            // test until depth-scaled sizing (Option B) is decided.
                            let config = DeriveConfig.defaults |> DeriveConfig.setSeqRange (Range.linear 0 5)
                            let! m = Derive.genWith<Map<Suit, Card list>> config
                            return m |> Map.forall (fun _ cards -> cards |> List.forall (fun c -> c.Rank = c.Rank))
                        }
                    )

                    Test.testProperty (
                        "derives a terminating generator for recursive unions",
                        property {
                            let! tree = Derive.gen<Tree>
                            return treeDepth tree >= 1
                        }
                    )

                    Test.testProperty (
                        "honours a deeper recursion depth via autoWith",
                        property {
                            let config =
                                DeriveConfig.defaults |> DeriveConfig.setRecursionDepth 3

                            let! tree = Derive.genWith<Tree> config
                            return treeDepth tree >= 1
                        }
                    )

                    Test.testProperty (
                        "addGenerator overrides the derived generator",
                        property {
                            let config =
                                DeriveConfig.defaults
                                |> DeriveConfig.addGenerator (Gen.constant (UserId 999))

                            let! id = Derive.genWith<UserId> config
                            return id = UserId 999
                        }
                    )

                    Test.testProperty (
                        "addGenerator can compose another generator (varied UserId)",
                        property {
                            // The registered generator is itself built from an int generator, so
                            // every draw is a different UserId rather than a single fixed value.
                            let config =
                                DeriveConfig.defaults
                                |> DeriveConfig.addGenerator (Gen.int32 (Range.linear 1 100) |> Gen.map UserId)

                            let! id = Derive.genWith<UserId> config
                            let (UserId n) = id
                            return n >= 1 && n <= 100
                        }
                    )

                    test (
                        "an unsupported type fails with a clear error",
                        fun _ ->
                            // The walker raises eagerly when constructing the generator.
                            let message =
                                try
                                    Derive.gen<System.Text.StringBuilder> |> ignore
                                    "no exception was raised"
                                with ex ->
                                    ex.Message

                            assertThat
                                message
                                (isEqualTo
                                    "Derive: no automatic generator for type 'System.Text.StringBuilder'. Register one with DeriveConfig.addGenerator.")
                    )
                ]
            )
        ]
    )

[<EntryPoint>]
let main _ =
   runTests tests
