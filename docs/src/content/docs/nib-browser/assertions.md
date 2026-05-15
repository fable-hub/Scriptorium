---
title: Assertions Reference
description: Complete list of built-in DOM assertion functions in Scriptorium.Nib.Browser.
---

```fsharp
open Scriptorium.Nib.Browser
```

All DOM assertions wrap Playwright's `LocatorAssertions` (`expect(locator).*`). They operate on a `Locator` subject.

## Visibility and state

| Assertion | Playwright equivalent |
|---|---|
| `toBeVisible` | `expect(l).toBeVisible()` |
| `beHidden` | `expect(l).toBeHidden()` |
| `beEnabled` | `expect(l).toBeEnabled()` |
| `beDisabled` | `expect(l).toBeDisabled()` |
| `beFocused` | `expect(l).toBeFocused()` |
| `beChecked` | `expect(l).toBeChecked()` |
| `beEmpty` | `expect(l).toBeEmpty()` |
| `beEditable` | `expect(l).toBeEditable()` |

## Text content

| Assertion | Description |
|---|---|
| `containText (expected: string)` | The element contains `expected` anywhere in its text |
| `haveText (expected: string)` | The element's full text matches `expected` exactly |

## Attributes and CSS

| Assertion | Description |
|---|---|
| `haveValue (expected: string)` | The input's value equals `expected` |
| `haveAttribute (name: string) (value: string)` | The element has the given attribute with the given value |
| `haveClass (expected: string)` | The element's class list contains exactly the classes in `expected` |
| `containClass (expected: string)` | The element's class list contains all classes in `expected` |
| `haveCSS (name: string) (value: string)` | The element has the given computed CSS property |
| `haveCount (expected: int)` | The locator resolves to exactly `expected` DOM nodes |

## Negation

Use `not'` (or `not_`) to invert any DOM assertion:

```fsharp
promise {
    do! assertLocator (page.locator "button") (
        not' beDisabled >>. toBeVisible
    )
}
```
