namespace Fable.Nib.Browser

open Fable.Core
open Fable.Core.JS
open Fable.Nib.Assertion
open Glutinum.Playwright

open type Glutinum.Playwright.Exports

// ---------------------------------------------------------------------------
// Async assertion type — mirrors Fable.Nib's Assertion<'a, 'b> but with Promise
// ---------------------------------------------------------------------------

type DomAssertion<'a, 'b> = AssertionState<'a> -> Promise<AssertionState<'b>>

type DomAssertion<'a> = DomAssertion<'a, 'a>

// ---------------------------------------------------------------------------
// Composition and entry point
// ---------------------------------------------------------------------------

[<AutoOpen>]
module DomAssertionOps =

    /// Sequential composition of two DomAssertions. Analogous to >> for Assertion<'a>.
    let (>>.) (a: DomAssertion<'a, 'b>) (b: DomAssertion<'b, 'c>) : DomAssertion<'a, 'c> =
        fun state ->
            promise {
                let! state' = a state
                return! b state'
            }

    /// Runs a DomAssertion chain against a locator and rejects the Promise if any assertion failed.
    let assertLocator (subject: Locator) (assertion: DomAssertion<Locator>) : Promise<unit> =
        let initial =
            {
                Subject = subject
                Errors = []
                Tags = []
                Stopped = false
            }

        promise {
            let! final = assertion initial

            match List.rev final.Errors with
            | [] -> ()
            | errors -> failwith (System.String.Join("\n", errors))
        }

// ---------------------------------------------------------------------------
// Composition helpers
// ---------------------------------------------------------------------------

[<AutoOpen>]
module DomAssertionCombinators =

    /// <summary>
    /// Inverts a DomAssertion — passes when <c>inner</c> fails, fails when <c>inner</c> passes.
    /// </summary>
    /// <remarks>
    /// Runs <c>inner</c> against a clean error slate. If <c>inner</c> accumulated no errors
    /// (it passed), <c>not'</c> injects a failure. If <c>inner</c> did fail, <c>not'</c>
    /// discards those errors and lets the outer chain continue cleanly.
    /// </remarks>
    let not' (inner: DomAssertion<Locator>) : DomAssertion<Locator> =
        fun state ->
            if state.Stopped then
                promise { return state }
            else
                promise {
                    let! innerResult = inner { state with Errors = [] }

                    if innerResult.Errors.IsEmpty then
                        return
                            { state with
                                Errors = "Expected assertion to fail but it passed" :: state.Errors
                            }
                    else
                        return state
                }

    /// Alias for <c>not'</c> — use when the apostrophe is inconvenient in a pipeline.
    let not_ = not'

// ---------------------------------------------------------------------------
// Locator assertions
// ---------------------------------------------------------------------------

[<AutoOpen>]
module DomAssertions =

    let private wrap
        (check: Locator -> Promise<unit>)
        : DomAssertion<Locator>
        =
        fun state ->
            if state.Stopped then
                promise { return state }
            else
                promise {
                    let mutable result = state

                    try
                        do! check state.Subject
                    with ex ->
                        result <- { state with Errors = ex.Message :: state.Errors }

                    return result
                }

    let containText (expected: string) : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toContainText(expected))

    let haveText (expected: string) : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toHaveText(expected))

    let toBeVisible : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toBeVisible())

    let beChecked : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toBeChecked())

    let haveValue (expected: string) : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toHaveValue(U2.Case1 expected))

    let beHidden : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toBeHidden())

    let beDisabled : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toBeDisabled())

    let beEnabled : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toBeEnabled())

    let beFocused : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toBeFocused())

    let beEmpty : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toBeEmpty())

    let beEditable : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toBeEditable())

    /// Asserts the locator resolves to exactly <c>expected</c> DOM nodes.
    let haveCount (expected: int) : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toHaveCount(float expected))

    /// Asserts the element has an attribute with the given name and value.
    let haveAttribute (name: string) (value: string) : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toHaveAttribute(name, U2.Case1 value))

    /// Asserts the element's <c>class</c> attribute exactly matches the given class string.
    let haveClass (expected: string) : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toHaveClass(U3.Case1 expected))

    /// Asserts the element's <c>class</c> list contains all classes in <c>expected</c>.
    let containClass (expected: string) : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toContainClass(U2.Case1 expected))

    /// Asserts the element has the given computed CSS property value.
    let haveCSS (name: string) (value: string) : DomAssertion<Locator> =
        wrap (fun locator -> expect(locator).toHaveCSS(name, U2.Case1 value))
