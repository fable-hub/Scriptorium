(***
---
title: Overview
description: Pure ANSI colour and style for terminal output in F# and Fable.
---
*)

(*** hide ***)
module Ink.Overview

(**
import { Image } from "astro:assets";
import demoImage from "./assets/demo.png";

`Scriptorium.Ink` is the colour and style layer for the Scriptorium family. It wraps ANSI escape codes into a small set of composable functions - no dependencies, no configuration required.

## Installation

```sh
dotnet add package Scriptorium.Ink
```

## Usage

All functions take a `string` and return a `string`, so they work inline in format strings, with pipes, or composed with `>>`:

*)

open Scriptorium.Ink

let log text = printfn "%s\n" text

// Single function
green "I am successful" |> log

// Pipe composition
(bold >> underline) "Look how stylized I am!" |> log

// Inline in a format string
log $"""The task %s{bold "do the laundry"} was %s{green "successful"} and took %s{dim "5min"}"""

// Nested - outer style survives the inner colour reset
underline $"""Hello I am {red "red"} while the whole text is underlined"""
|> log

(**

<div style={{ display: "flex", justifyContent: "center" }}>
    <Image width="700px" src={demoImage} alt="Example output" />
</div>

There are foreground colours (`red`, `green`, `blue`, …), background colours (`bgRed`, `bgGreen`, …), and text styles (`bold`, `dim`, `italic`, `underline`, `strikethrough`). Both standard and bright variants are available for colours.

## NO_COLOR

`Scriptorium.Ink` follows the [no-color.org](https://no-color.org) convention. When `NO_COLOR` is set, colour functions return the string unchanged. Style functions (`bold`, `dim`, etc.) always apply regardless.
*)
