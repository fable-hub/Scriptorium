(**
---
title: Overview
description: Composable fluent assertions for F# and Fable - no runner dependency.
---
*)

(*** hide ***)

module Nib.Overview

(**

`Scriptorium.Nib` is the assertion layer for the Scriptorium family.

It works with any F# framework - or directly in application code.

## Installation

```sh
dotnet add package Scriptorium.Nib
```

## Usage

All assertions follows a consistent pattern:

1. Start with `assertThat`
2. Pass subject to validate
3. Chain of assertions

### Basic assertions

*)

open Scriptorium.Nib.Assertion

assertThat 42 (isEqualTo 42)
assertThat "hello" (isNotEqualTo "world")
assertThat true isTrue

(**

### Compositions

Assertions compose with `>>`

:::tip
You can read `>>` as `and`
:::

*)

assertThat 42 (isGreaterThan 0 >> isLessThan 100 >> isNotEqualTo 99)

(**

Every assertion in the chain runs - failures are collected and reported together rather than stopping on the first one.

### Nested assertions

Use `inside` and `tag` to drill into records and label failures:

*)

(*** hide ***)

type User =
    {
        Name: string
        Age: int
    }

(** *)

assertThat
    {
        Name = ""
        Age = 15
    }
    (inside _.Name (tag "name" >> isNotEqualTo "")
     >> inside _.Age (tag "age" >> isGreaterOrEqual 18))

// Throws:
//   [name] given "" should not be equal to ""
//   [age] given 15 should be greater than or equal to 18

(**

Collections, options, and results have dedicated assertions:

*)

assertThat
    [
        1
        2
        3
    ]
    (isNotEmpty >> hasSize 3 >> contain 2)

assertThat (Some "alice") (Option.value >> isNotEqualTo "")

assertThat (Ok 42: Result<int, string>) (Result.okValue >> isGreaterThan 0)
