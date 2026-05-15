(**
---
title: Overview
description: Playwright-powered DOM assertions and test helpers for end-to-end testing.
---
*)

(*** hide ***)

module NibDom.Overview

// Dummy values so the file compiles without a real browser.
let private page = Unchecked.defaultof<Glutinum.Playwright.Page>

(**

`Scriptorium.Nib.Browser` is the browser-testing layer for the Scriptorium family. It extends `Scriptorium.Nib`'s
composable assertions with Playwright-powered DOM checks and wraps `Scriptorium.Quill`'s test DSL with
helpers that manage browser lifecycle automatically.

:::caution
This package only works for Fable JavaScript target
:::

## Installation

```sh
dotnet add package Scriptorium.Nib.Browser
npm install -D playwright
```

Install the Playwright browser binaries (one-time setup):

```sh
npx playwright install chromium
```

## `DomAssertion`

`DomAssertion<'a>` is the async counterpart of Nib's `Assertion<'a>`. It returns a `Promise` so that each step can await Playwright operations.

Compose `DomAssertion` values with the `>>.` operator (note the dot after `>>`):

:::tip
You can read `>>.` as `and`
:::

*)

open Scriptorium.Nib.Browser
open Glutinum.Playwright

let myAssertion: DomAssertion<Locator> =
    toBeVisible >>. containText "Submit"

(**

## `assertLocator` — the entry point

Runs a `DomAssertion` chain against a locator. Rejects the promise if any assertion fails.

*)

let assertExample () =
    promise {
        do! assertLocator (page.locator "button") (
            toBeVisible >>. containText "Submit" >>. beEnabled
        )
    }
