---
title: Why Scriptorium
description: The philosophy behind the Scriptorium family of libraries.
---

## The need for a cross-platform testing experience

Since its inception, the Fable ecosystem has been about bringing the power of F# to other runtimes. But for years, testing Fable applications meant maintaining compatibility layers between .NET testing tools and JavaScript testing frameworks.

Scriptorium is built to change that. It provides a family of focused libraries for testing Fable applications, all written in F#, designed to run on any Fable-supported platform with a consistent experience - the same API whether you target Node.js, the browser, or plain .NET.

## A single metaphor, many responsibilities

The Scriptorium libraries are organised around one metaphor: **the act of writing**.

In a medieval scriptorium, the scribe sits down with several tools - each with a single, well-defined purpose:

- **Ink** is the colour and style. It doesn't write; it only makes the marks visible.
- **Parchment** is the page that receives the writing. It structures the output.
- **Nib** is the sharp point that makes the mark. It is where truth is tested.
- **Quill** is the instrument that holds everything together and carries the hand across the page.

This is exactly how the libraries are designed. Each one does one thing, does it well, and composes cleanly with the others:

| Library | Responsibility |
|---|---|
| `Scriptorium.Ink` | Colour and style for terminal output |
| `Scriptorium.Parchment` | Structured logging that dips into Ink |
| `Scriptorium.Nib` | Composable assertions, no runner dependency |
| `Scriptorium.Quill` | Test runner that holds Nib, Ink, and Parchment together |
| `Scriptorium.Nib.Snapshot` | Snapshot testing on top of Nib |
| `Scriptorium.Nib.Browser` | Playwright-powered DOM assertions |

## Design principles

### Every failure is collected before reporting

`Scriptorium.Nib` never throws on the first failing assertion. An assertion chain continues to the end, collecting every failure. Only when `assertThat` returns does it throw - with a single exception carrying every message.

This means one failing test shows you *all* the things that are wrong, not just the first. No more re-running after each fix.

### Composability over magic

Assertions are plain F# functions of type `AssertionState<'a> -> AssertionState<'b>`. They compose with `>>`. There is no reflection, no attribute magic, no framework coupling.

You can define a reusable assertion for your own domain types, compose it with built-in assertions, and use it in any test framework that can call a function.

### Independent layers

All libraries are independently usable. You can:
- Use `Scriptorium.Nib` assertions inside Expecto, Jest, or any other test framework.
- Use `Scriptorium.Ink` to colour any string output in an application.
- Use `Scriptorium.Parchment` for structured logging without any test dependency.

Nothing forces you to use the full stack. Start with what you need.

### Respected conventions

- `NO_COLOR` - `Scriptorium.Ink` honours the [no-color.org](https://no-color.org) convention. Set `NO_COLOR=1` to strip all ANSI codes from output.
- `CI` - `Scriptorium.Quill` detects the `CI` environment variable and refuses to exit with code 0 if focused tests (`ftest`) are committed to the repository.
- `UPDATE_SNAPSHOTS` - `Scriptorium.Nib.Snapshot` uses this variable to trigger snapshot updates.
