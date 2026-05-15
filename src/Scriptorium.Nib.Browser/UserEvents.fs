namespace Scriptorium.Nib.Browser

open System
open Fable.Core.JS
open Glutinum.Playwright

type UserEvents =

    /// Clicks the element matched by the locator.
    static member click(locator: Locator) : Promise<unit> = locator.click ()

    /// Focuses the element matched by the locator.
    static member focus(locator: Locator, ?options) = locator.focus (?options = options)

    /// Hover over the matching element.
    static member hover(locator: Locator, ?options) = locator.hover (?options = options)

    /// Removes focus from the element matched by the locator.
    static member blur(locator: Locator) : Promise<unit> = locator.blur ()

    /// Ensure that checkbox or radio element is checked.
    static member check(locator: Locator, ?options) = locator.check (?options = options)

    /// Ensure that checkbox or radio element is unchecked.
    static member uncheck(locator: Locator, ?options) = locator.uncheck (?options = options)

    /// Selects option or options in <c><select></c>.
    static member selectOption(locator: Locator, [<ParamArray>] values: string, ?options) : Promise<unit> =
        promise { let! _ = locator.selectOption (values, ?options = options) in () }

    /// Focuses the matching element and presses a combination of the keys.
    static member press(locator: Locator, key: string, ?options) =
        locator.press (key, ?options = options)

    /// <summary>
    /// Highlight the corresponding element(s) on the screen. Useful for debugging, don't commit the code that uses
    /// [locator.highlight()](https://playwright.dev/docs/api/class-locator#locator-highlight).
    /// </summary>
    static member highlight(locator: Locator) = locator.highlight ()

    /// Set a value to the input field.
    static member fill(locator: Locator, value: string, ?options) =
        locator.fill (value, ?options = options)
