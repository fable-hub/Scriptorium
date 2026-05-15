---
title: Assertions Reference
description: Complete list of built-in assertion functions in Scriptorium.Nib.
---

```fsharp
open Scriptorium.Nib.Assertion
```

## Primitive

| Assertion | Passes when |
|---|---|
| `isEqualTo expected` | subject equals `expected` (structural equality, shows diff on failure) |
| `isNotEqualTo expected` | subject does not equal `expected` |
| `isGreaterThan threshold` | subject > threshold |
| `isGreaterOrEqual threshold` | subject ≥ threshold |
| `isLessThan threshold` | subject < threshold |
| `isLessOrEqual threshold` | subject ≤ threshold |
| `satisfy predicate` | predicate returns `true` |

```fsharp
assertThat 42    (isEqualTo 42)
assertThat 42    (isGreaterThan 0 >> isLessThan 100)
assertThat 18    (isGreaterOrEqual 18)   // boundary — passes
assertThat "hi"  (satisfy (fun s -> s.Length > 0))
```

## Boolean

| Assertion | Passes when |
|---|---|
| `isTrue` | `true` |
| `isFalse` | `false` |
| `isNotTrue` | `false` |
| `isNotFalse` | `true` |

## Null

| Assertion | Passes when |
|---|---|
| `isNull` | subject is `null` |
| `isNotNull` | subject is not `null` |

## Thunk (`unit -> unit`)

| Assertion | Passes when |
|---|---|
| `throws` | thunk throws; shifts subject to the caught exception |
| `throwsWithMessage msg` | thunk throws with exactly `msg`; shifts subject to exception |
| `doesNotThrow` | thunk completes without throwing |

```fsharp
assertThat (fun () -> failwith "boom") (throwsWithMessage "boom")

// Continue asserting on the exception itself
assertThat (fun () -> failwith "boom") (
    throws
    >> focus _.Message
    >> isEqualTo "boom"
)

assertThat (fun () -> ignore (1 + 1)) doesNotThrow
```

## Collections (`'a list`)

| Assertion | Passes when |
|---|---|
| `hasSize n` | list has exactly `n` elements |
| `isEmpty` | list is empty |
| `isNotEmpty` | list has at least one element |
| `contain x` | list contains `x` |
| `notContain x` | list does not contain `x` |
| `startWith prefix` | list begins with `prefix` (shows diff on failure) |
| `haveSameSizeAs other` | same length as `other` |
| `beSameAs expected` | same elements in same order (shows diff) |
| `haveSameElements expected` | same elements in any order (sorts both before comparing) |

```fsharp
assertThat [1; 2; 3] (
    isNotEmpty
    >> hasSize 3
    >> contain 2
    >> startWith [1]
    >> haveSameElements [3; 1; 2]
)
```

Use `inside` to drill into elements:

```fsharp
assertThat [{ Name = "alice"; Age = 25 }] (
    hasSize 1
    >> inside List.head (
        inside _.Name (tag "name" >> isNotEqualTo "")
        >> inside _.Age (tag "age" >> isGreaterOrEqual 18)
    )
)
```

## Option

| Assertion | Passes when |
|---|---|
| `Option.isSome` | option is `Some` |
| `Option.isNone` | option is `None` |
| `Option.value` | option is `Some`; shifts subject to the inner value (aborts if `None`) |

```fsharp
assertThat (Some 42)             (Option.value >> isEqualTo 42)
assertThat (None : int option)   Option.isNone
```

## Result

| Assertion | Passes when |
|---|---|
| `Result.isOk` | result is `Ok` |
| `Result.isError` | result is `Error` |
| `Result.okValue` | result is `Ok`; shifts subject to the inner value (aborts if `Error`) |
| `Result.errorValue` | result is `Error`; shifts subject to the error (aborts if `Ok`) |

```fsharp
assertThat (Ok 42 : Result<int, string>)      (Result.okValue >> isEqualTo 42)
assertThat (Error "oops" : Result<int, string>) (Result.errorValue >> isEqualTo "oops")
```
