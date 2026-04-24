# Fable.Scriptorium

F# libraries for building and testing F#/Fable applications.

## Packages

| Package | Description | NuGet |
|---|---|---|
| **Fable.Ink** | ANSI colour and style for terminal output | [![NuGet](https://badgen.net/nuget/v/Fable.Ink)](https://www.nuget.org/packages/Fable.Ink) |
| **Fable.Parchment** | Structured logging (`info`, `warn`, `error`) | [![NuGet](https://badgen.net/nuget/v/Fable.Parchment)](https://www.nuget.org/packages/Fable.Parchment) |
| **Fable.Nib** | Fluent assertion library | [![NuGet](https://badgen.net/nuget/v/Fable.Nib)](https://www.nuget.org/packages/Fable.Nib) |
| **Fable.Quill** | Test runner with coloured output and source links | [![NuGet](https://badgen.net/nuget/v/Fable.Quill)](https://www.nuget.org/packages/Fable.Quill) |
| **Fable.Nib.Snapshot** | Snapshot assertions for Nib | [![NuGet](https://badgen.net/nuget/v/Fable.Nib.Snapshot)](https://www.nuget.org/packages/Fable.Nib.Snapshot) |
| **Fable.Nib.Browser** | Browser tests via Playwright | [![NuGet](https://badgen.net/nuget/v/Fable.Nib.Browser)](https://www.nuget.org/packages/Fable.Nib.Browser) |


## Quick start

```fsharp
open Fable.Nib
open Fable.Quill.Runner
open type Fable.Quill.Runner.Test

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
