# Scriptorium

F# libraries for building and testing F#/Fable applications.

## Packages

| Package | Description | NuGet |
|---|---|---|
| **Scriptorium.Ink** | ANSI colour and style for terminal output | [![NuGet](https://badgen.net/nuget/v/Scriptorium.Ink)](https://www.nuget.org/packages/Scriptorium.Ink) |
| **Scriptorium.Parchment** | Structured logging (`info`, `warn`, `error`) | [![NuGet](https://badgen.net/nuget/v/Scriptorium.Parchment)](https://www.nuget.org/packages/Scriptorium.Parchment) |
| **Scriptorium.Nib** | Fluent assertion library | [![NuGet](https://badgen.net/nuget/v/Scriptorium.Nib)](https://www.nuget.org/packages/Scriptorium.Nib) |
| **Scriptorium.Quill** | Test runner with coloured output and source links | [![NuGet](https://badgen.net/nuget/v/Scriptorium.Quill)](https://www.nuget.org/packages/Scriptorium.Quill) |
| **Scriptorium.Nib.Snapshot** | Snapshot assertions for Nib | [![NuGet](https://badgen.net/nuget/v/Scriptorium.Nib.Snapshot)](https://www.nuget.org/packages/Scriptorium.Nib.Snapshot) |
| **Scriptorium.Nib.Browser** | Browser tests via Playwright | [![NuGet](https://badgen.net/nuget/v/Scriptorium.Nib.Browser)](https://www.nuget.org/packages/Scriptorium.Nib.Browser) |

## Quick start

```fsharp
open Scriptorium.Nib
open Scriptorium.Quill.Runner
open type Scriptorium.Quill.Runner.Test

[<EntryPoint>]
let main _ =
    runTests [
        testList "My suite" [

            test("equality", fun () ->
                assertThat 42 (isEqualTo 42)
            )

            test("option chain", fun () ->
                assertThat (Some "hello") (Option.value >> isEqualTo "hello")
            )

            test("record fields with tags", fun () ->
                assertThat { Name = "alice"; Age = 25 } (
                    inside _.Age  (tag "age"  >> isGreaterOrEqual 18)
                    >> inside _.Name (tag "name" >> isNotEqualTo "")
                )
            )

        ]
    ]
```
