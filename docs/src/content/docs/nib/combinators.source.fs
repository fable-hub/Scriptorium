(***
---
title: Combinators
description: Structural combinators for navigating, labelling, and controlling assertion chains.
---
*)

(*** hide ***)

module Nib.Combinators

open Scriptorium.Nib.Assertion

type Author =
    {
        Name: string
        Age: int
    }

type Book =
    {
        Title: string
        Author: Author
    }

(**

Combinators are the building blocks that let you navigate into nested values, label failures, short-circuit on critical errors, and invert assertions. They work at the level of the chain itself, not the subject value.

## `assertion` — the primitive building block

Every built-in assertion is defined with this function. Give it a predicate and an error message formatter:

*)

let isPositive: Assertion<int> =
    assertion (fun n -> n > 0) (fun n -> $"expected a positive number but got {n}")

assertThat 42 isPositive // passes
assertThat -1 isPositive // throws: expected a positive number but got -1

(**

Custom assertions compose freely with built-in ones:

*)

let isValidEmail: Assertion<string> =
    assertion
        (fun s -> s.Contains("@") && s.Contains("."))
        (fun s -> $"'{s}' is not a valid email address")

assertThat "alice@example.com" (isNotEqualTo "" >> isValidEmail)

(**

## `inside` — assert on a projected value

Runs an inner chain on a projection of the subject, merges any failures back into the outer chain, then **restores the original subject**. Use this to assert on multiple fields without losing the outer context:

*)

assertThat
    {
        Name = "alice"
        Age = 25
    }
    (inside _.Name (isNotEqualTo "") >> inside _.Age (isGreaterOrEqual 18))

(**

`inside` nests to any depth:

*)

assertThat
    {
        Title = ""
        Author =
            {
                Name = ""
                Age = 15
            }
    }
    (inside _.Title (tag "title" >> isNotEqualTo "")
     >> inside
         _.Author
         (tag "author"
          >> inside _.Name (tag "name" >> isNotEqualTo "")
          >> inside _.Age (tag "age" >> isGreaterOrEqual 18)))
// Throws:
//   [title] given "" should not be equal to ""
//   [author.name] given "" should not be equal to ""
//   [author.age] given 15 should be greater than or equal to 18

(**

## `focus` — change the subject

Projects the current subject into a new value. All subsequent assertions in the chain work on the projected value. The original is discarded — use `inside` if you need to come back to it.

*)

assertThat (fun () -> failwith "boom") (throws >> focus _.Message >> isEqualTo "boom")

(**

## `tag` — label failures

Marks all following assertions with a named prefix. Tags nest — `tag "b"` inside a `tag "a"` scope produces `[a.b]`:

*)

assertThat 15 (tag "age" >> isGreaterOrEqual 18)
// throws: [age] given 15 should be greater than or equal to 18

assertThat
    {
        Name = ""
        Age = 15
    }
    (tag "user"
     >> inside _.Name (tag "name" >> isNotEqualTo "")
     >> inside _.Age (tag "age" >> isGreaterOrEqual 18))
// throws:
//   [user.name] given "" should not be equal to ""
//   [user.age] given 15 should be greater than or equal to 18

(**

## `forceError` — short-circuit on failure

Wraps an assertion so that if it fails, the rest of the chain is aborted. Use this when subsequent assertions would crash or be meaningless if the guarded one fails:

*)

(*** hide ***)

let myList = []
let expectedHead = 0

(*** show ***)

assertThat
    myList
    (forceError isNotEmpty // abort if empty — List.head would crash
     >> focus List.head
     >> isEqualTo expectedHead)

(**

`Option.value`, `Result.okValue`, and `DU.ofCase` all use `forceError` internally for the same reason.

## `not'` — invert an assertion

Passes when the inner assertion fails, fails when it passes:

*)

assertThat 1 (not' (isEqualTo 2)) // passes: 1 ≠ 2

assertThat 1 (not' (isEqualTo 1))
// throws: Expected assertion to fail but it passed

(**

`not_` is an alias when the apostrophe is inconvenient in a pipeline.

## `DU.ofCase` — discriminated union assertions

Checks a DU case and extracts its payload as the new subject. Aborts the chain if the case does not match.

Define one `ofCase` per case you care about, then compose freely:

*)

[<RequireQualifiedAccess>]
type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

let isCircle: Assertion<Shape, float> =
    DU.ofCase
        "Circle"
        (function
        | Shape.Circle r -> Some r
        | _ -> None
        )

let isRectangle: Assertion<Shape, float * float> =
    DU.ofCase
        "Rectangle"
        (function
        | Shape.Rectangle(w, h) -> Some(w, h)
        | _ -> None
        )

// Assert case and continue on the payload
assertThat (Shape.Circle 5.0) (isCircle >> isGreaterThan 0.0)

// Assert on both tuple fields
assertThat
    (Shape.Rectangle(4.0, 6.0))
    (isRectangle
     >> inside fst (tag "width" >> isGreaterThan 0.0)
     >> inside snd (tag "height" >> isGreaterThan 0.0))

// Wrong case produces a readable failure
assertThat (Shape.Rectangle(3.0, 4.0)) isCircle
// throws: expected Circle but got Rectangle (3, 4)
