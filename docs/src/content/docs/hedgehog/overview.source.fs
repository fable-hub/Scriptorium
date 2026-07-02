(***
---
title: Overview
description: Run Hedgehog property-based tests inside the Scriptorium.Quill runner.
---
*)

(*** hide ***)

module Hedgehog.Overview

open Hedgehog
open Hedgehog.FSharp
open Scriptorium.Nib.Assertion
open Scriptorium.Hedgehog
open type Scriptorium.Quill.Test

(**

`Scriptorium.Hedgehog` bridges the [Hedgehog](https://hedgehogqa.github.io/fsharp-hedgehog/)
property-based testing library into the [Scriptorium.Quill](/Scriptorium/quill/overview) runner.
You write properties with Hedgehog's normal API - `Gen`, `Range`, and the `property { }`
computation expression - and run them as ordinary Quill tests.

When a property is falsified, the test fails with Hedgehog's rendered, minimal counterexample.

## Installation

```sh
dotnet add package Scriptorium.Hedgehog
```

## Writing a property

`Test.property` turns a Hedgehog `Property<bool>` into a Quill `TestCase`. A `property { ... }` block
that ends with `return <bool>` is the common case - it runs 100 generated cases by default:

*)

let reverseRoundtrips =
    Test.property (
        "List.rev is involutive",
        property {
            let! xs = Gen.list (Range.linear 0 100) (Gen.int32 (Range.constant 0 1000))
            return List.rev (List.rev xs) = xs
        }
    )

(**

The block can draw as many generators as it needs with `let!`:

*)

let additionCommutes =
    Test.property (
        "addition is commutative",
        property {
            let! a = Gen.int32 (Range.linear -1000 1000)
            let! b = Gen.int32 (Range.linear -1000 1000)
            return a + b = b + a
        }
    )

(**

## Asserting with Nib

Instead of returning a `bool`, you can pass `Test.property` a generator and an assertion function.
Any `Scriptorium.Nib` assertion works: when it throws, Hedgehog records a failure, shrinks to a
minimal counterexample, and shows the assertion's message.

*)

let doublingMatches =
    Test.property (
        "doubling equals adding to itself",
        Gen.int32 (Range.linear -1000 1000),
        fun n -> assertThat (n * 2) (isEqualTo (n + n))
    )

(**

A unit property (`property { ... }` whose body asserts by throwing and ends with `return ()`) works
through the same `Test.property` overload set:

```fsharp
Test.property (
    "reverse preserves length",
    property {
        let! xs = Gen.list (Range.linear 0 50) (Gen.int32 (Range.constant 0 100))
        assertThat (List.rev xs |> List.length) (isEqualTo xs.Length)
        return ()
    }
)
```

## Custom configuration

`Test.propertyWith` accepts a Hedgehog `PropertyConfig` - for example to change the number of
generated cases:

*)

let manyTests =
    Test.propertyWith (
        "holds across 500 cases",
        PropertyConfig.defaults |> PropertyConfig.withTests 500<tests>,
        property {
            let! n = Gen.int32 (Range.constant 0 100)
            return n >= 0
        }
    )

(**

## Automatic generation

Hedgehog's built-in auto-generation (`Gen.auto<'T>`) doesn't run under Fable - it relies on
reflection APIs Fable doesn't provide.

`Scriptorium.Hedgehog` ships a portable alternative, `Derive.gen<'T>`, which builds a generator
with `FSharp.Reflection` - the reflection Fable *does* support - so you can draw a whole value with `let!`:

*)

type Point =
    {
        X: int
        Y: int
    }

let swapIsInvolutive =
    Test.property (
        "swapping a point's coordinates twice restores it",
        property {
            let! p = Derive.gen<Point>

            let swap pt =
                {
                    X = pt.Y
                    Y = pt.X
                }

            return swap (swap p) = p
        }
    )

(**

`Derive` handles primitives, tuples, records (including anonymous), discriminated unions (including
recursive ones and `option`), enums, `list`, arrays, `seq`, and F# `Set`/`Map` - on both .NET and
Fable.

For a type it doesn't recognise, `Derive` raises an error telling you to register a generator. Do
that with `DeriveConfig.addGenerator`, then pass the config to `Derive.genWith`:

*)

type Link =
    {
        Url: System.Uri
        Label: string
    }

let linkConfig =
    DeriveConfig.defaults
    |> DeriveConfig.addGenerator (
        [
            "https://example.com"
            "https://fable.io"
        ]
        |> Gen.item
        |> Gen.map (fun url -> System.Uri url)
    )

let everyLinkIsHttps =
    Test.property (
        "derived links use https",
        property {
            let! link = Derive.genWith<Link> linkConfig
            return link.Url.Scheme = "https"
        }
    )

(**

A few things to keep in mind:

- `Derive` is slower than a hand-written generator, since it works by reflection.
- Recursive types are bounded by a per-type depth (default `1`), so generation always terminates -
  raise it with `DeriveConfig.setRecursionDepth`.
- Deeply nested collections can grow large; cap their element counts with `DeriveConfig.setSeqRange`.

*)
