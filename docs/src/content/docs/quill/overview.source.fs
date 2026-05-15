(***
---
title: Overview
description: The test runner that holds the Scriptorium family together.
---
*)

(*** hide ***)

module Quill.Overview

let computeAsync _ = async { return 42 }

(**

import { Tabs, TabItem } from '@astrojs/starlight/components';
import { Steps } from '@astrojs/starlight/components';

`Scriptorium.Quill` is the test runner for the Scriptorium family.

It provides a DSL for defining sync and async tests, a focus / pending / skip system,
per-test configuration, and a pretty-printer that renders results with colour, indentation,
and clickable source links in terminal output.

Scriptorium.Quill aims to support all F#/Fable runtimes:

| Runtime    | Supports |
| ---------- | :------: |
| .NET       |    ✅    |
| JavaScript |    ✅    |
| Python     |    ✅    |
| Rust       |   🚧    |

## Installation

```sh
dotnet add package Scriptorium.Quill
```

`Scriptorium.Quill` depends on `Scriptorium.Ink`, `Scriptorium.Parchment`, and `Scriptorium.Nib`. A single package reference covers the full stack.

## Quick start

import { Image } from "astro:assets";
import outputExample from "./assets/output-example.png";

<Steps>


1. Set up your tests

    ```fs

    open Scriptorium.Nib.Assertion

    open type Scriptorium.Quill.Test
    open type Scriptorium.Quill.Runner

    [<EntryPoint>]
    let main _ =
        runTests [
            testList ("My suite", [

                test ("basic equality", fun _ ->
                    assertThat 42 (isEqualTo 42)
                )

                testAsync ("async operation", fun _ -> async {
                    let! result = computeAsync ()
                    assertThat result (isEqualTo 42)
                })

                // ...

            ])
        ]
    ```

2. Use the corresponding command based on your runtime:

    |  Runtime   | Command                              |
    | :--------: | :----------------------------------- |
    |    .NET    | `dotnet run`                         |
    | JavaScript | `dotnet fable --runScript`           |
    |   Python   | `dotnet fable --lang py --runScript` |

3. You should see an output looking like that

    <div style={{ display: "flex", justifyContent: "center" }}>
    <Image width="500px" src={outputExample} alt="Example output" />
    </div>

</Steps>

*)
