namespace rec Glutinum.Playwright

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AllowNullLiteral>]
[<Interface>]
type Page =
    /// <summary>
    /// Adds a <c>&lt;link rel="stylesheet"></c> tag into the page with the desired url or a <c>&lt;style type="text/css"></c> tag with the
    /// content. Returns the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member addStyleTag: ?options: Page.addStyleTag.options -> JS.Promise<ElementHandle>
    /// <summary>
    /// Clears all stored console messages from this page. Subsequent calls to
    /// [page.consoleMessages([options])](https://playwright.dev/docs/api/class-page#page-console-messages) will only
    /// return messages logged after the clear.
    /// </summary>
    abstract member clearConsoleMessages: unit -> JS.Promise<unit>
    /// <summary>
    /// Clears all stored page errors from this page. Subsequent calls to
    /// [page.pageErrors([options])](https://playwright.dev/docs/api/class-page#page-page-errors) will only return errors
    /// thrown after the clear.
    /// </summary>
    abstract member clearPageErrors: unit -> JS.Promise<unit>
    /// <summary>
    /// If [<c>runBeforeUnload</c>](https://playwright.dev/docs/api/class-page#page-close-option-run-before-unload) is <c>false</c>,
    /// does not run any unload handlers and waits for the page to be closed. If
    /// [<c>runBeforeUnload</c>](https://playwright.dev/docs/api/class-page#page-close-option-run-before-unload) is <c>true</c> the
    /// method will run unload handlers, but will **not** wait for the page to close.
    ///
    /// By default, <c>page.close()</c> **does not** run <c>beforeunload</c> handlers.
    ///
    /// **NOTE** if [<c>runBeforeUnload</c>](https://playwright.dev/docs/api/class-page#page-close-option-run-before-unload) is
    /// passed as true, a <c>beforeunload</c> dialog might be summoned and should be handled manually via
    /// [page.on('dialog')](https://playwright.dev/docs/api/class-page#page-event-dialog) event.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member close: ?options: Page.close.options -> JS.Promise<unit>
    /// <summary>
    /// Pauses script execution. Playwright will stop executing the script and wait for the user to either press the
    /// 'Resume' button in the page overlay or to call <c>playwright.resume()</c> in the DevTools console.
    ///
    /// User can inspect selectors or perform manual steps while paused. Resume will continue running the original script
    /// from the place it was paused.
    ///
    /// **NOTE** This method requires Playwright to be started in a headed mode, with a falsy
    /// [<c>headless</c>](https://playwright.dev/docs/api/class-browsertype#browser-type-launch-option-headless) option.
    /// </summary>
    abstract member pause: unit -> JS.Promise<unit>
    /// <summary>
    /// Resumes script execution. Throws if the debugger is not paused.
    /// </summary>
    abstract member resume: unit -> JS.Promise<unit>
    /// <summary>
    /// This method internally calls [document.write()](https://developer.mozilla.org/en-US/docs/Web/API/Document/write),
    /// inheriting all its specific characteristics and behaviors.
    /// </summary>
    /// <param name="html">
    /// HTML markup to assign to the page.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member setContent: html: string * ?options: Page.setContent.options -> JS.Promise<unit>
    /// <summary>
    /// The method returns an element locator that can be used to perform actions on this page / frame. Locator is resolved
    /// to the element immediately before performing an action, so a series of actions on the same locator can in fact be
    /// performed on different DOM elements. That would happen if the DOM structure between those actions has changed.
    ///
    /// [Learn more about locators](https://playwright.dev/docs/locators).
    /// </summary>
    /// <param name="selector">
    /// A selector to use when resolving DOM element.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member locator: selector: string * ?options: Page.locator.options -> Locator


module Page =

    module locator =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?has: Locator,
                ?hasNot: Locator,
                ?hasNotText: U2<string, RegExp>,
                ?hasText: U2<string, RegExp>
            ) =

            member val has : Locator option = nativeOnly with get, set
            member val hasNot : Locator option = nativeOnly with get, set
            member val hasNotText : U2<string, RegExp> option = nativeOnly with get, set
            member val hasText : U2<string, RegExp> option = nativeOnly with get, set

    module setContent =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float,
                ?waitUntil: Page.setContent.options.waitUntil
            ) =

            member val timeout : float option = nativeOnly with get, set
            member val waitUntil : Page.setContent.options.waitUntil option = nativeOnly with get, set

        module options =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type waitUntil =
                | load
                | domcontentloaded
                | networkidle
                | commit

    module addStyleTag =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?content: string,
                ?path: string,
                ?url: string
            ) =

            member val content : string option = nativeOnly with get, set
            member val path : string option = nativeOnly with get, set
            member val url : string option = nativeOnly with get, set

    module close =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?reason: string,
                ?runBeforeUnload: bool
            ) =

            member val reason : string option = nativeOnly with get, set
            member val runBeforeUnload : bool option = nativeOnly with get, set


[<AllowNullLiteral>]
[<Interface>]
type Locator =
    /// <summary>
    /// Returns a human-readable representation of the locator, using the
    /// [locator.description()](https://playwright.dev/docs/api/class-locator#locator-description) if one exists;
    /// otherwise, it generates a string based on the locator's selector.
    /// </summary>
    abstract member toString: unit -> string
    /// <summary>
    /// When the locator points to a list of elements, this returns an array of locators, pointing to their respective
    /// elements.
    ///
    /// **NOTE** [locator.all()](https://playwright.dev/docs/api/class-locator#locator-all) does not wait for elements to
    /// match the locator, and instead immediately returns whatever is present in the page.
    ///
    /// When the list of elements changes dynamically,
    /// [locator.all()](https://playwright.dev/docs/api/class-locator#locator-all) will produce unpredictable and flaky
    /// results.
    ///
    /// When the list of elements is stable, but loaded dynamically, wait for the full list to finish loading before
    /// calling [locator.all()](https://playwright.dev/docs/api/class-locator#locator-all).
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// for (const li of await page.getByRole('listitem').all())
    ///   await li.click();
    /// </code>
    /// </summary>
    abstract member all: unit -> JS.Promise<ResizeArray<Locator>>
    /// <summary>
    /// Returns an array of <c>node.innerText</c> values for all matching nodes.
    ///
    /// **NOTE** If you need to assert text on the page, prefer
    /// [expect(locator).toHaveText(expected[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-text)
    /// with
    /// [<c>useInnerText</c>](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-text-option-use-inner-text)
    /// option to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const texts = await page.getByRole('link').allInnerTexts();
    /// </code>
    /// </summary>
    abstract member allInnerTexts: unit -> JS.Promise<ResizeArray<string>>
    /// <summary>
    /// Returns an array of <c>node.textContent</c> values for all matching nodes.
    ///
    /// **NOTE** If you need to assert text on the page, prefer
    /// [expect(locator).toHaveText(expected[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-text)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const texts = await page.getByRole('link').allTextContents();
    /// </code>
    /// </summary>
    abstract member allTextContents: unit -> JS.Promise<ResizeArray<string>>
    /// <summary>
    /// Creates a locator that matches both this locator and the argument locator.
    ///
    /// **Usage**
    ///
    /// The following example finds a button with a specific title.
    ///
    /// <code lang="js">
    /// const button = page.getByRole('button').and(page.getByTitle('Subscribe'));
    /// </code>
    /// </summary>
    /// <param name="locator">
    /// Additional locator to match.
    /// </param>
    abstract member ``and``: locator: Locator -> Locator
    /// <summary>
    /// Creates a locator that matches both this locator and the argument locator.
    ///
    /// **Usage**
    ///
    /// The following example finds a button with a specific title.
    ///
    /// <code lang="js">
    /// const button = page.getByRole('button').and(page.getByTitle('Subscribe'));
    /// </code>
    /// </summary>
    /// <param name="locator">
    /// Additional locator to match.
    /// </param>
    [<Emit("$0.and($1)")>]
    abstract member and_: locator: Locator -> Locator
    /// <summary>
    /// Captures the aria snapshot of the given element. Read more about [aria snapshots](https://playwright.dev/docs/aria-snapshots) and
    /// [expect(locator).toMatchAriaSnapshot(expected[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-match-aria-snapshot)
    /// for the corresponding assertion.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('link').ariaSnapshot();
    /// </code>
    ///
    /// **Details**
    ///
    /// This method captures the aria snapshot of the given element. The snapshot is a string that represents the state of
    /// the element and its children. The snapshot can be used to assert the state of the element in the test, or to
    /// compare it to state in the future.
    ///
    /// The ARIA snapshot is represented using [YAML](https://yaml.org/spec/1.2.2/) markup language:
    /// - The keys of the objects are the roles and optional accessible names of the elements.
    /// - The values are either text content or an array of child elements.
    /// - Generic static text can be represented with the <c>text</c> key.
    ///
    /// Below is the HTML markup and the respective ARIA snapshot:
    ///
    /// <code lang="html">
    /// &lt;ul aria-label="Links">
    ///   &lt;li>&lt;a href="/">Home&lt;/a>&lt;/li>
    ///   &lt;li>&lt;a href="/about">About&lt;/a>&lt;/li>
    /// &lt;ul>
    /// </code>
    ///
    /// <code lang="yml">
    /// - list "Links":
    ///   - listitem:
    ///     - link "Home"
    ///   - listitem:
    ///     - link "About"
    /// </code>
    ///
    /// An AI-optimized snapshot, controlled by
    /// [<c>mode</c>](https://playwright.dev/docs/api/class-locator#locator-aria-snapshot-option-mode), is different from a
    /// default snapshot:
    /// 1. Includes element references <c>[ref=e2]</c>. 2. Does not wait for an element matching the locator, and throws when
    ///    no elements match. 3. Includes snapshots of <c>&lt;iframe></c>s inside the target.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member ariaSnapshot: ?options: Locator.ariaSnapshot.options -> JS.Promise<string>
    /// <summary>
    /// Calls [blur](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/blur) on the element.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member blur: ?options: Locator.blur.options -> JS.Promise<unit>
    /// <summary>
    /// This method returns the bounding box of the element matching the locator, or <c>null</c> if the element is not visible.
    /// The bounding box is calculated relative to the main frame viewport - which is usually the same as the browser
    /// window.
    ///
    /// **Details**
    ///
    /// Scrolling affects the returned bounding box, similarly to
    /// [Element.getBoundingClientRect](https://developer.mozilla.org/en-US/docs/Web/API/Element/getBoundingClientRect).
    /// That means <c>x</c> and/or <c>y</c> may be negative.
    ///
    /// Elements from child frames return the bounding box relative to the main frame, unlike the
    /// [Element.getBoundingClientRect](https://developer.mozilla.org/en-US/docs/Web/API/Element/getBoundingClientRect).
    ///
    /// Assuming the page is static, it is safe to use bounding box coordinates to perform input. For example, the
    /// following snippet should click the center of the element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const box = await page.getByRole('button').boundingBox();
    /// await page.mouse.click(box.x + box.width / 2, box.y + box.height / 2);
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member boundingBox: ?options: Locator.boundingBox.options -> JS.Promise<Locator.boundingBox option>
    /// <summary>
    /// Ensure that checkbox or radio element is checked.
    ///
    /// **Details**
    ///
    /// Performs the following steps:
    /// 1. Ensure that element is a checkbox or a radio input. If not, this method throws. If the element is already
    ///    checked, this method returns immediately.
    /// 1. Wait for [actionability](https://playwright.dev/docs/actionability) checks on the element, unless
    ///    [<c>force</c>](https://playwright.dev/docs/api/class-locator#locator-check-option-force) option is set.
    /// 1. Scroll the element into view if needed.
    /// 1. Use [page.mouse](https://playwright.dev/docs/api/class-page#page-mouse) to click in the center of the
    ///    element.
    /// 1. Ensure that the element is now checked. If not, this method throws.
    ///
    /// If the element is detached from the DOM at any moment during the action, this method throws.
    ///
    /// When all steps combined have not finished during the specified
    /// [<c>timeout</c>](https://playwright.dev/docs/api/class-locator#locator-check-option-timeout), this method throws a
    /// [TimeoutError](https://playwright.dev/docs/api/class-timeouterror). Passing zero timeout disables this.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('checkbox').check();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member check: ?options: Locator.check.options -> JS.Promise<unit>
    /// <summary>
    /// Clear the input field.
    ///
    /// **Details**
    ///
    /// This method waits for [actionability](https://playwright.dev/docs/actionability) checks, focuses the element, clears it and triggers an
    /// <c>input</c> event after clearing.
    ///
    /// If the target element is not an <c>&lt;input></c>, <c>&lt;textarea></c> or <c>[contenteditable]</c> element, this method throws an
    /// error. However, if the element is inside the <c>&lt;label></c> element that has an associated
    /// [control](https://developer.mozilla.org/en-US/docs/Web/API/HTMLLabelElement/control), the control will be cleared
    /// instead.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('textbox').clear();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member clear: ?options: Locator.clear.options -> JS.Promise<unit>
    /// <summary>
    /// Click an element.
    ///
    /// **Details**
    ///
    /// This method clicks the element by performing the following steps:
    /// 1. Wait for [actionability](https://playwright.dev/docs/actionability) checks on the element, unless
    ///    [<c>force</c>](https://playwright.dev/docs/api/class-locator#locator-click-option-force) option is set.
    /// 1. Scroll the element into view if needed.
    /// 1. Use [page.mouse](https://playwright.dev/docs/api/class-page#page-mouse) to click in the center of the
    ///    element, or the specified
    ///    [<c>position</c>](https://playwright.dev/docs/api/class-locator#locator-click-option-position).
    /// 1. Wait for initiated navigations to either succeed or fail, unless
    ///    [<c>noWaitAfter</c>](https://playwright.dev/docs/api/class-locator#locator-click-option-no-wait-after) option is
    ///    set.
    ///
    /// If the element is detached from the DOM at any moment during the action, this method throws.
    ///
    /// When all steps combined have not finished during the specified
    /// [<c>timeout</c>](https://playwright.dev/docs/api/class-locator#locator-click-option-timeout), this method throws a
    /// [TimeoutError](https://playwright.dev/docs/api/class-timeouterror). Passing zero timeout disables this.
    ///
    /// **Usage**
    ///
    /// Click a button:
    ///
    /// <code lang="js">
    /// await page.getByRole('button').click();
    /// </code>
    ///
    /// Shift-right-click at a specific position on a canvas:
    ///
    /// <code lang="js">
    /// await page.locator('canvas').click({
    ///   button: 'right',
    ///   modifiers: ['Shift'],
    ///   position: { x: 23, y: 32 },
    /// });
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member click: ?options: Locator.click.options -> JS.Promise<unit>
    /// <summary>
    /// Returns a [FrameLocator](https://playwright.dev/docs/api/class-framelocator) object pointing to the same <c>iframe</c>
    /// as this locator.
    ///
    /// Useful when you have a [Locator](https://playwright.dev/docs/api/class-locator) object obtained somewhere, and
    /// later on would like to interact with the content inside the frame.
    ///
    /// For a reverse operation, use
    /// [frameLocator.owner()](https://playwright.dev/docs/api/class-framelocator#frame-locator-owner).
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('iframe[name="embedded"]');
    /// // ...
    /// const frameLocator = locator.contentFrame();
    /// await frameLocator.getByRole('button').click();
    /// </code>
    /// </summary>
    abstract member contentFrame: unit -> FrameLocator
    /// <summary>
    /// Returns the number of elements matching the locator.
    ///
    /// **NOTE** If you need to assert the number of elements on the page, prefer
    /// [expect(locator).toHaveCount(count[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-count)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const count = await page.getByRole('listitem').count();
    /// </code>
    /// </summary>
    abstract member count: unit -> JS.Promise<float>
    /// <summary>
    /// Double-click an element.
    ///
    /// **Details**
    ///
    /// This method double clicks the element by performing the following steps:
    /// 1. Wait for [actionability](https://playwright.dev/docs/actionability) checks on the element, unless
    ///    [<c>force</c>](https://playwright.dev/docs/api/class-locator#locator-dblclick-option-force) option is set.
    /// 1. Scroll the element into view if needed.
    /// 1. Use [page.mouse](https://playwright.dev/docs/api/class-page#page-mouse) to double click in the center of the
    ///    element, or the specified
    ///    [<c>position</c>](https://playwright.dev/docs/api/class-locator#locator-dblclick-option-position).
    ///
    /// If the element is detached from the DOM at any moment during the action, this method throws.
    ///
    /// When all steps combined have not finished during the specified
    /// [<c>timeout</c>](https://playwright.dev/docs/api/class-locator#locator-dblclick-option-timeout), this method throws a
    /// [TimeoutError](https://playwright.dev/docs/api/class-timeouterror). Passing zero timeout disables this.
    ///
    /// **NOTE** <c>element.dblclick()</c> dispatches two <c>click</c> events and a single <c>dblclick</c> event.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member dblclick: ?options: Locator.dblclick.options -> JS.Promise<unit>
    /// <summary>
    /// Describes the locator, description is used in the trace viewer and reports. Returns the locator pointing to the
    /// same element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const button = page.getByTestId('btn-sub').describe('Subscribe button');
    /// await button.click();
    /// </code>
    /// </summary>
    /// <param name="description">
    /// Locator description.
    /// </param>
    abstract member describe: description: string -> Locator
    /// <summary>
    /// Returns locator description previously set with
    /// [locator.describe(description)](https://playwright.dev/docs/api/class-locator#locator-describe). Returns <c>null</c> if
    /// no custom description has been set. Prefer
    /// [locator.toString()](https://playwright.dev/docs/api/class-locator#locator-to-string) for a human-readable
    /// representation, as it uses the description when available.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const button = page.getByRole('button').describe('Subscribe button');
    /// console.log(button.description()); // "Subscribe button"
    ///
    /// const input = page.getByRole('textbox');
    /// console.log(input.description()); // null
    /// </code>
    /// </summary>
    abstract member description: unit -> string option
    /// <summary>
    /// Programmatically dispatch an event on the matching element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await locator.dispatchEvent('click');
    /// </code>
    ///
    /// **Details**
    ///
    /// The snippet above dispatches the <c>click</c> event on the element. Regardless of the visibility state of the element,
    /// <c>click</c> is dispatched. This is equivalent to calling
    /// [element.click()](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/click).
    ///
    /// Under the hood, it creates an instance of an event based on the given
    /// [<c>type</c>](https://playwright.dev/docs/api/class-locator#locator-dispatch-event-option-type), initializes it with
    /// [<c>eventInit</c>](https://playwright.dev/docs/api/class-locator#locator-dispatch-event-option-event-init) properties
    /// and dispatches it on the element. Events are <c>composed</c>, <c>cancelable</c> and bubble by default.
    ///
    /// Since [<c>eventInit</c>](https://playwright.dev/docs/api/class-locator#locator-dispatch-event-option-event-init) is
    /// event-specific, please refer to the events documentation for the lists of initial properties:
    /// - [DeviceMotionEvent](https://developer.mozilla.org/en-US/docs/Web/API/DeviceMotionEvent/DeviceMotionEvent)
    /// - [DeviceOrientationEvent](https://developer.mozilla.org/en-US/docs/Web/API/DeviceOrientationEvent/DeviceOrientationEvent)
    /// - [DragEvent](https://developer.mozilla.org/en-US/docs/Web/API/DragEvent/DragEvent)
    /// - [Event](https://developer.mozilla.org/en-US/docs/Web/API/Event/Event)
    /// - [FocusEvent](https://developer.mozilla.org/en-US/docs/Web/API/FocusEvent/FocusEvent)
    /// - [KeyboardEvent](https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/KeyboardEvent)
    /// - [MouseEvent](https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent/MouseEvent)
    /// - [PointerEvent](https://developer.mozilla.org/en-US/docs/Web/API/PointerEvent/PointerEvent)
    /// - [TouchEvent](https://developer.mozilla.org/en-US/docs/Web/API/TouchEvent/TouchEvent)
    /// - [WheelEvent](https://developer.mozilla.org/en-US/docs/Web/API/WheelEvent/WheelEvent)
    ///
    /// You can also specify [JSHandle](https://playwright.dev/docs/api/class-jshandle) as the property value if you want
    /// live objects to be passed into the event:
    ///
    /// <code lang="js">
    /// const dataTransfer = await page.evaluateHandle(() => new DataTransfer());
    /// await locator.dispatchEvent('dragstart', { dataTransfer });
    /// </code>
    /// </summary>
    /// <param name="type">
    /// DOM event type: <c>"click"</c>, <c>"dragstart"</c>, etc.
    /// </param>
    /// <param name="eventInit">
    /// Optional event-specific initialization properties.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member dispatchEvent: ``type``: string * ?eventInit: EvaluationArgument * ?options: Locator.dispatchEvent.options -> JS.Promise<unit>
    /// <summary>
    /// Drag the source element towards the target element and drop it.
    ///
    /// **Details**
    ///
    /// This method drags the locator to another target locator or target position. It will first move to the source
    /// element, perform a <c>mousedown</c>, then move to the target element or position and perform a <c>mouseup</c>.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const source = page.locator('#source');
    /// const target = page.locator('#target');
    ///
    /// await source.dragTo(target);
    /// // or specify exact positions relative to the top-left corners of the elements:
    /// await source.dragTo(target, {
    ///   sourcePosition: { x: 34, y: 7 },
    ///   targetPosition: { x: 10, y: 20 },
    /// });
    /// </code>
    /// </summary>
    /// <param name="target">
    /// Locator of the element to drag to.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member dragTo: target: Locator * ?options: Locator.dragTo.options -> JS.Promise<unit>
    /// <summary>
    /// **NOTE** Always prefer using [Locator](https://playwright.dev/docs/api/class-locator)s and web assertions over
    /// [ElementHandle](https://playwright.dev/docs/api/class-elementhandle)s because latter are inherently racy.
    ///
    /// Resolves given locator to all matching DOM elements. If there are no matching elements, returns an empty list.
    /// </summary>
    abstract member elementHandles: unit -> JS.Promise<ResizeArray<ElementHandle>>
    /// <summary>
    /// Set a value to the input field.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('textbox').fill('example value');
    /// </code>
    ///
    /// **Details**
    ///
    /// This method waits for [actionability](https://playwright.dev/docs/actionability) checks, focuses the element, fills it and triggers an
    /// <c>input</c> event after filling. Note that you can pass an empty string to clear the input field.
    ///
    /// If the target element is not an <c>&lt;input></c>, <c>&lt;textarea></c> or <c>[contenteditable]</c> element, this method throws an
    /// error. However, if the element is inside the <c>&lt;label></c> element that has an associated
    /// [control](https://developer.mozilla.org/en-US/docs/Web/API/HTMLLabelElement/control), the control will be filled
    /// instead.
    ///
    /// To send fine-grained keyboard events, use
    /// [locator.pressSequentially(text[, options])](https://playwright.dev/docs/api/class-locator#locator-press-sequentially).
    /// </summary>
    /// <param name="value">
    /// Value to set for the <c>&lt;input></c>, <c>&lt;textarea></c> or <c>[contenteditable]</c> element.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member fill: value: string * ?options: Locator.fill.options -> JS.Promise<unit>
    /// <summary>
    /// This method narrows existing locator according to the options, for example filters by text. It can be chained to
    /// filter multiple times.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const rowLocator = page.locator('tr');
    /// // ...
    /// await rowLocator
    ///     .filter({ hasText: 'text in column 1' })
    ///     .filter({ has: page.getByRole('button', { name: 'column 2 button' }) })
    ///     .screenshot();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member filter: ?options: Locator.filter.options -> Locator
    /// <summary>
    /// Returns locator to the first matching element.
    /// </summary>
    abstract member first: unit -> Locator
    /// <summary>
    /// Calls [focus](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus) on the matching element.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member focus: ?options: Locator.focus.options -> JS.Promise<unit>
    /// <summary>
    /// When working with iframes, you can create a frame locator that will enter the iframe and allow locating elements in
    /// that iframe:
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.frameLocator('iframe').getByText('Submit');
    /// await locator.click();
    /// </code>
    /// </summary>
    /// <param name="selector">
    /// A selector to use when resolving DOM element.
    /// </param>
    abstract member frameLocator: selector: string -> FrameLocator
    /// <summary>
    /// Returns the matching element's attribute value.
    ///
    /// **NOTE** If you need to assert an element's attribute, prefer
    /// [expect(locator).toHaveAttribute(name, value[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-attribute)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    /// </summary>
    /// <param name="name">
    /// Attribute name to get the value for.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member getAttribute: name: string * ?options: Locator.getAttribute.options -> JS.Promise<string option>
    /// <summary>
    /// Allows locating elements by their alt text.
    ///
    /// **Usage**
    ///
    /// For example, this method will find the image by alt text "Playwright logo":
    ///
    /// <code lang="html">
    /// &lt;img alt='Playwright logo'>
    /// </code>
    ///
    /// <code lang="js">
    /// await page.getByAltText('Playwright logo').click();
    /// </code>
    /// </summary>
    /// <param name="text">
    /// Text to locate the element for.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member getByAltText: text: U2<string, RegExp> * ?options: Locator.getByAltText.options -> Locator
    /// <summary>
    /// Allows locating input elements by the text of the associated <c>&lt;label></c> or <c>aria-labelledby</c> element, or by the
    /// <c>aria-label</c> attribute.
    ///
    /// **Usage**
    ///
    /// For example, this method will find inputs by label "Username" and "Password" in the following DOM:
    ///
    /// <code lang="html">
    /// &lt;input aria-label="Username">
    /// &lt;label for="password-input">Password:&lt;/label>
    /// &lt;input id="password-input">
    /// </code>
    ///
    /// <code lang="js">
    /// await page.getByLabel('Username').fill('john');
    /// await page.getByLabel('Password').fill('secret');
    /// </code>
    /// </summary>
    /// <param name="text">
    /// Text to locate the element for.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member getByLabel: text: U2<string, RegExp> * ?options: Locator.getByLabel.options -> Locator
    /// <summary>
    /// Allows locating input elements by the placeholder text.
    ///
    /// **Usage**
    ///
    /// For example, consider the following DOM structure.
    ///
    /// <code lang="html">
    /// &lt;input type="email" placeholder="name@example.com" />
    /// </code>
    ///
    /// You can fill the input after locating it by the placeholder text:
    ///
    /// <code lang="js">
    /// await page
    ///     .getByPlaceholder('name@example.com')
    ///     .fill('playwright@microsoft.com');
    /// </code>
    /// </summary>
    /// <param name="text">
    /// Text to locate the element for.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member getByPlaceholder: text: U2<string, RegExp> * ?options: Locator.getByPlaceholder.options -> Locator
    /// <summary>
    /// Allows locating elements by their [ARIA role](https://www.w3.org/TR/wai-aria-1.2/#roles),
    /// [ARIA attributes](https://www.w3.org/TR/wai-aria-1.2/#aria-attributes) and
    /// [accessible name](https://w3c.github.io/accname/#dfn-accessible-name).
    ///
    /// **Usage**
    ///
    /// Consider the following DOM structure.
    ///
    /// <code lang="html">
    /// &lt;h3>Sign up&lt;/h3>
    /// &lt;label>
    ///   &lt;input type="checkbox" /> Subscribe
    /// &lt;/label>
    /// &lt;br/>
    /// &lt;button>Submit&lt;/button>
    /// </code>
    ///
    /// You can locate each element by its implicit role:
    ///
    /// <code lang="js">
    /// await expect(page.getByRole('heading', { name: 'Sign up' })).toBeVisible();
    ///
    /// await page.getByRole('checkbox', { name: 'Subscribe' }).check();
    ///
    /// await page.getByRole('button', { name: /submit/i }).click();
    /// </code>
    ///
    /// **Details**
    ///
    /// Role selector **does not replace** accessibility audits and conformance tests, but rather gives early feedback
    /// about the ARIA guidelines.
    ///
    /// Many html elements have an implicitly [defined role](https://w3c.github.io/html-aam/#html-element-role-mappings)
    /// that is recognized by the role selector. You can find all the
    /// [supported roles here](https://www.w3.org/TR/wai-aria-1.2/#role_definitions). ARIA guidelines **do not recommend**
    /// duplicating implicit roles and attributes by setting <c>role</c> and/or <c>aria-*</c> attributes to default values.
    /// </summary>
    /// <param name="role">
    /// Required aria role.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member getByRole: role: Locator.getByRole.role * ?options: Locator.getByRole.options -> Locator
    /// <summary>
    /// Locate element by the test id.
    ///
    /// **Usage**
    ///
    /// Consider the following DOM structure.
    ///
    /// <code lang="html">
    /// &lt;button data-testid="directions">Itinéraire&lt;/button>
    /// </code>
    ///
    /// You can locate the element by its test id:
    ///
    /// <code lang="js">
    /// await page.getByTestId('directions').click();
    /// </code>
    ///
    /// **Details**
    ///
    /// By default, the <c>data-testid</c> attribute is used as a test id. Use
    /// [selectors.setTestIdAttribute(attributeName)](https://playwright.dev/docs/api/class-selectors#selectors-set-test-id-attribute)
    /// to configure a different test id attribute if necessary.
    ///
    /// <c></c>`js
    /// // Set custom test id attribute from
    /// </summary>
    /// <param name="testId">
    /// Id to locate the element by.
    /// </param>
    abstract member getByTestId: testId: U2<string, RegExp> -> Locator
    /// <summary>
    /// Allows locating elements that contain given text.
    ///
    /// See also [locator.filter([options])](https://playwright.dev/docs/api/class-locator#locator-filter) that allows to
    /// match by another criteria, like an accessible role, and then filter by the text content.
    ///
    /// **Usage**
    ///
    /// Consider the following DOM structure:
    ///
    /// <code lang="html">
    /// &lt;div>Hello &lt;span>world&lt;/span>&lt;/div>
    /// &lt;div>Hello&lt;/div>
    /// </code>
    ///
    /// You can locate by text substring, exact string, or a regular expression:
    ///
    /// <code lang="js">
    /// // Matches &lt;span>
    /// page.getByText('world');
    ///
    /// // Matches first &lt;div>
    /// page.getByText('Hello world');
    ///
    /// // Matches second &lt;div>
    /// page.getByText('Hello', { exact: true });
    ///
    /// // Matches both &lt;div>s
    /// page.getByText(/Hello/);
    ///
    /// // Matches second &lt;div>
    /// page.getByText(/^hello$/i);
    /// </code>
    ///
    /// **Details**
    ///
    /// Matching by text always normalizes whitespace, even with exact match. For example, it turns multiple spaces into
    /// one, turns line breaks into spaces and ignores leading and trailing whitespace.
    ///
    /// Input elements of the type <c>button</c> and <c>submit</c> are matched by their <c>value</c> instead of the text content. For
    /// example, locating by text <c>"Log in"</c> matches <c>&lt;input type=button value="Log in"></c>.
    /// </summary>
    /// <param name="text">
    /// Text to locate the element for.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member getByText: text: U2<string, RegExp> * ?options: Locator.getByText.options -> Locator
    /// <summary>
    /// Allows locating elements by their title attribute.
    ///
    /// **Usage**
    ///
    /// Consider the following DOM structure.
    ///
    /// <code lang="html">
    /// &lt;span title='Issues count'>25 issues&lt;/span>
    /// </code>
    ///
    /// You can check the issues count after locating it by the title text:
    ///
    /// <code lang="js">
    /// await expect(page.getByTitle('Issues count')).toHaveText('25 issues');
    /// </code>
    /// </summary>
    /// <param name="text">
    /// Text to locate the element for.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member getByTitle: text: U2<string, RegExp> * ?options: Locator.getByTitle.options -> Locator
    /// <summary>
    /// Highlight the corresponding element(s) on the screen. Useful for debugging, don't commit the code that uses
    /// [locator.highlight()](https://playwright.dev/docs/api/class-locator#locator-highlight).
    /// </summary>
    abstract member highlight: unit -> JS.Promise<unit>
    /// <summary>
    /// Hover over the matching element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('link').hover();
    /// </code>
    ///
    /// **Details**
    ///
    /// This method hovers over the element by performing the following steps:
    /// 1. Wait for [actionability](https://playwright.dev/docs/actionability) checks on the element, unless
    ///    [<c>force</c>](https://playwright.dev/docs/api/class-locator#locator-hover-option-force) option is set.
    /// 1. Scroll the element into view if needed.
    /// 1. Use [page.mouse](https://playwright.dev/docs/api/class-page#page-mouse) to hover over the center of the
    ///    element, or the specified
    ///    [<c>position</c>](https://playwright.dev/docs/api/class-locator#locator-hover-option-position).
    ///
    /// If the element is detached from the DOM at any moment during the action, this method throws.
    ///
    /// When all steps combined have not finished during the specified
    /// [<c>timeout</c>](https://playwright.dev/docs/api/class-locator#locator-hover-option-timeout), this method throws a
    /// [TimeoutError](https://playwright.dev/docs/api/class-timeouterror). Passing zero timeout disables this.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member hover: ?options: Locator.hover.options -> JS.Promise<unit>
    /// <summary>
    /// Returns the [<c>element.innerHTML</c>](https://developer.mozilla.org/en-US/docs/Web/API/Element/innerHTML).
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member innerHTML: ?options: Locator.innerHTML.options -> JS.Promise<string>
    /// <summary>
    /// Returns the [<c>element.innerText</c>](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/innerText).
    ///
    /// **NOTE** If you need to assert text on the page, prefer
    /// [expect(locator).toHaveText(expected[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-text)
    /// with
    /// [<c>useInnerText</c>](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-text-option-use-inner-text)
    /// option to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member innerText: ?options: Locator.innerText.options -> JS.Promise<string>
    /// <summary>
    /// Returns the value for the matching <c>&lt;input></c> or <c>&lt;textarea></c> or <c>&lt;select></c> element.
    ///
    /// **NOTE** If you need to assert input value, prefer
    /// [expect(locator).toHaveValue(value[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-value)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const value = await page.getByRole('textbox').inputValue();
    /// </code>
    ///
    /// **Details**
    ///
    /// Throws elements that are not an input, textarea or a select. However, if the element is inside the <c>&lt;label></c>
    /// element that has an associated
    /// [control](https://developer.mozilla.org/en-US/docs/Web/API/HTMLLabelElement/control), returns the value of the
    /// control.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member inputValue: ?options: Locator.inputValue.options -> JS.Promise<string>
    /// <summary>
    /// Returns whether the element is checked. Throws if the element is not a checkbox or radio input.
    ///
    /// **NOTE** If you need to assert that checkbox is checked, prefer
    /// [expect(locator).toBeChecked([options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-be-checked)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const checked = await page.getByRole('checkbox').isChecked();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member isChecked: ?options: Locator.isChecked.options -> JS.Promise<bool>
    /// <summary>
    /// Returns whether the element is disabled, the opposite of [enabled](https://playwright.dev/docs/actionability#enabled).
    ///
    /// **NOTE** If you need to assert that an element is disabled, prefer
    /// [expect(locator).toBeDisabled([options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-be-disabled)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const disabled = await page.getByRole('button').isDisabled();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member isDisabled: ?options: Locator.isDisabled.options -> JS.Promise<bool>
    /// <summary>
    /// Returns whether the element is [editable](https://playwright.dev/docs/actionability#editable). If the target element is not an <c>&lt;input></c>,
    /// <c>&lt;textarea></c>, <c>&lt;select></c>, <c>[contenteditable]</c> and does not have a role allowing <c>[aria-readonly]</c>, this method
    /// throws an error.
    ///
    /// **NOTE** If you need to assert that an element is editable, prefer
    /// [expect(locator).toBeEditable([options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-be-editable)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const editable = await page.getByRole('textbox').isEditable();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member isEditable: ?options: Locator.isEditable.options -> JS.Promise<bool>
    /// <summary>
    /// Returns whether the element is [enabled](https://playwright.dev/docs/actionability#enabled).
    ///
    /// **NOTE** If you need to assert that an element is enabled, prefer
    /// [expect(locator).toBeEnabled([options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-be-enabled)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const enabled = await page.getByRole('button').isEnabled();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member isEnabled: ?options: Locator.isEnabled.options -> JS.Promise<bool>
    /// <summary>
    /// Returns whether the element is hidden, the opposite of [visible](https://playwright.dev/docs/actionability#visible).
    ///
    /// **NOTE** If you need to assert that element is hidden, prefer
    /// [expect(locator).toBeHidden([options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-be-hidden)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const hidden = await page.getByRole('button').isHidden();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member isHidden: ?options: Locator.isHidden.options -> JS.Promise<bool>
    /// <summary>
    /// Returns whether the element is [visible](https://playwright.dev/docs/actionability#visible).
    ///
    /// **NOTE** If you need to assert that element is visible, prefer
    /// [expect(locator).toBeVisible([options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-be-visible)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const visible = await page.getByRole('button').isVisible();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member isVisible: ?options: Locator.isVisible.options -> JS.Promise<bool>
    /// <summary>
    /// Returns locator to the last matching element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const banana = await page.getByRole('listitem').last();
    /// </code>
    /// </summary>
    abstract member last: unit -> Locator
    /// <summary>
    /// The method finds an element matching the specified selector in the locator's subtree. It also accepts filter
    /// options, similar to [locator.filter([options])](https://playwright.dev/docs/api/class-locator#locator-filter)
    /// method.
    ///
    /// [Learn more about locators](https://playwright.dev/docs/locators).
    /// </summary>
    /// <param name="selectorOrLocator">
    /// A selector or locator to use when resolving DOM element.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member locator: selectorOrLocator: string * ?options: Locator.locator.options -> Locator
    abstract member locator: selectorOrLocator: Locator * ?options: Locator.locator.options -> Locator
    /// <summary>
    /// Returns a new locator that uses best practices for referencing the matched element, prioritizing test ids, aria
    /// roles, and other user-facing attributes over CSS selectors. This is useful for converting implementation-detail
    /// selectors into more resilient, human-readable locators.
    /// </summary>
    abstract member normalize: unit -> JS.Promise<Locator>
    /// <summary>
    /// Returns locator to the n-th matching element. It's zero based, <c>nth(0)</c> selects the first element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const banana = await page.getByRole('listitem').nth(2);
    /// </code>
    /// </summary>
    /// <param name="index">
    ///
    /// </param>
    abstract member nth: index: float -> Locator
    /// <summary>
    /// Creates a locator matching all elements that match one or both of the two locators.
    ///
    /// Note that when both locators match something, the resulting locator will have multiple matches, potentially causing
    /// a [locator strictness](https://playwright.dev/docs/locators#strictness) violation.
    ///
    /// **Usage**
    ///
    /// Consider a scenario where you'd like to click on a "New email" button, but sometimes a security settings dialog
    /// shows up instead. In this case, you can wait for either a "New email" button, or a dialog and act accordingly.
    ///
    /// **NOTE** If both "New email" button and security dialog appear on screen, the "or" locator will match both of them,
    /// possibly throwing the ["strict mode violation" error](https://playwright.dev/docs/locators#strictness). In this case, you can use
    /// [locator.first()](https://playwright.dev/docs/api/class-locator#locator-first) to only match one of them.
    ///
    /// <code lang="js">
    /// const newEmail = page.getByRole('button', { name: 'New' });
    /// const dialog = page.getByText('Confirm security settings');
    /// await expect(newEmail.or(dialog).first()).toBeVisible();
    /// if (await dialog.isVisible())
    ///   await page.getByRole('button', { name: 'Dismiss' }).click();
    /// await newEmail.click();
    /// </code>
    /// </summary>
    /// <param name="locator">
    /// Alternative locator to match.
    /// </param>
    abstract member ``or``: locator: Locator -> Locator
    /// <summary>
    /// A page this locator belongs to.
    /// </summary>
    abstract member page: unit -> Page
    /// <summary>
    /// Focuses the matching element and presses a combination of the keys.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('textbox').press('Backspace');
    /// </code>
    ///
    /// **Details**
    ///
    /// Focuses the element, and then uses
    /// [keyboard.down(key)](https://playwright.dev/docs/api/class-keyboard#keyboard-down) and
    /// [keyboard.up(key)](https://playwright.dev/docs/api/class-keyboard#keyboard-up).
    ///
    /// [<c>key</c>](https://playwright.dev/docs/api/class-locator#locator-press-option-key) can specify the intended
    /// [keyboardEvent.key](https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/key) value or a single character
    /// to generate the text for. A superset of the
    /// [<c>key</c>](https://playwright.dev/docs/api/class-locator#locator-press-option-key) values can be found
    /// [here](https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/key/Key_Values). Examples of the keys are:
    ///
    /// <c>F1</c> - <c>F12</c>, <c>Digit0</c>- <c>Digit9</c>, <c>KeyA</c>- <c>KeyZ</c>, <c>Backquote</c>, <c>Minus</c>, <c>Equal</c>, <c>Backslash</c>, <c>Backspace</c>, <c>Tab</c>,
    /// <c>Delete</c>, <c>Escape</c>, <c>ArrowDown</c>, <c>End</c>, <c>Enter</c>, <c>Home</c>, <c>Insert</c>, <c>PageDown</c>, <c>PageUp</c>, <c>ArrowRight</c>, <c>ArrowUp</c>,
    /// etc.
    ///
    /// Following modification shortcuts are also supported: <c>Shift</c>, <c>Control</c>, <c>Alt</c>, <c>Meta</c>, <c>ShiftLeft</c>,
    /// <c>ControlOrMeta</c>. <c>ControlOrMeta</c> resolves to <c>Control</c> on Windows and Linux and to <c>Meta</c> on macOS.
    ///
    /// Holding down <c>Shift</c> will type the text that corresponds to the
    /// [<c>key</c>](https://playwright.dev/docs/api/class-locator#locator-press-option-key) in the upper case.
    ///
    /// If [<c>key</c>](https://playwright.dev/docs/api/class-locator#locator-press-option-key) is a single character, it is
    /// case-sensitive, so the values <c>a</c> and <c>A</c> will generate different respective texts.
    ///
    /// Shortcuts such as <c>key: "Control+o"</c>, <c>key: "Control++</c> or <c>key: "Control+Shift+T"</c> are supported as well. When
    /// specified with the modifier, modifier is pressed and being held while the subsequent key is being pressed.
    /// </summary>
    /// <param name="key">
    /// Name of the key to press or a character to generate, such as <c>ArrowLeft</c> or <c>a</c>.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member press: key: string * ?options: Locator.press.options -> JS.Promise<unit>
    /// <summary>
    /// **NOTE** In most cases, you should use
    /// [locator.fill(value[, options])](https://playwright.dev/docs/api/class-locator#locator-fill) instead. You only need
    /// to press keys one by one if there is special keyboard handling on the page.
    ///
    /// Focuses the element, and then sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the
    /// text.
    ///
    /// To press a special key, like <c>Control</c> or <c>ArrowDown</c>, use
    /// [locator.press(key[, options])](https://playwright.dev/docs/api/class-locator#locator-press).
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await locator.pressSequentially('Hello'); // Types instantly
    /// await locator.pressSequentially('World', { delay: 100 }); // Types slower, like a user
    /// </code>
    ///
    /// An example of typing into a text field and then submitting the form:
    ///
    /// <code lang="js">
    /// const locator = page.getByLabel('Password');
    /// await locator.pressSequentially('my password');
    /// await locator.press('Enter');
    /// </code>
    /// </summary>
    /// <param name="text">
    /// String of characters to sequentially press into a focused element.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member pressSequentially: text: string * ?options: Locator.pressSequentially.options -> JS.Promise<unit>
    /// <summary>
    /// Take a screenshot of the element matching the locator.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('link').screenshot();
    /// </code>
    ///
    /// Disable animations and save screenshot to a file:
    ///
    /// <code lang="js">
    /// await page.getByRole('link').screenshot({ animations: 'disabled', path: 'link.png' });
    /// </code>
    ///
    /// **Details**
    ///
    /// This method captures a screenshot of the page, clipped to the size and position of a particular element matching
    /// the locator. If the element is covered by other elements, it will not be actually visible on the screenshot. If the
    /// element is a scrollable container, only the currently scrolled content will be visible on the screenshot.
    ///
    /// This method waits for the [actionability](https://playwright.dev/docs/actionability) checks, then scrolls element into view before taking
    /// a screenshot. If the element is detached from DOM, the method throws an error.
    ///
    /// Returns the buffer with the captured screenshot.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member screenshot: ?options: LocatorScreenshotOptions -> JS.Promise<Buffer>
    /// <summary>
    /// This method waits for [actionability](https://playwright.dev/docs/actionability) checks, then tries to scroll element into view, unless
    /// it is completely visible as defined by
    /// [IntersectionObserver](https://developer.mozilla.org/en-US/docs/Web/API/Intersection_Observer_API)'s <c>ratio</c>.
    ///
    /// See [scrolling](https://playwright.dev/docs/input#scrolling) for alternative ways to scroll.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member scrollIntoViewIfNeeded: ?options: Locator.scrollIntoViewIfNeeded.options -> JS.Promise<unit>
    /// <summary>
    /// Selects option or options in <c>&lt;select></c>.
    ///
    /// **Details**
    ///
    /// This method waits for [actionability](https://playwright.dev/docs/actionability) checks, waits until all specified options are present in
    /// the <c>&lt;select></c> element and selects these options.
    ///
    /// If the target element is not a <c>&lt;select></c> element, this method throws an error. However, if the element is inside
    /// the <c>&lt;label></c> element that has an associated
    /// [control](https://developer.mozilla.org/en-US/docs/Web/API/HTMLLabelElement/control), the control will be used
    /// instead.
    ///
    /// Returns the array of option values that have been successfully selected.
    ///
    /// Triggers a <c>change</c> and <c>input</c> event once all the provided options have been selected.
    ///
    /// **Usage**
    ///
    /// <code lang="html">
    /// &lt;select multiple>
    ///   &lt;option value="red">Red&lt;/option>
    ///   &lt;option value="green">Green&lt;/option>
    ///   &lt;option value="blue">Blue&lt;/option>
    /// &lt;/select>
    /// </code>
    ///
    /// <code lang="js">
    /// // single selection matching the value or label
    /// element.selectOption('blue');
    ///
    /// // single selection matching the label
    /// element.selectOption({ label: 'Blue' });
    ///
    /// // multiple selection for red, green and blue options
    /// element.selectOption(['red', 'green', 'blue']);
    /// </code>
    /// </summary>
    /// <param name="values">
    /// Options to select. If the <c>&lt;select></c> has the <c>multiple</c> attribute, all matching options are selected, otherwise
    /// only the first option matching one of the passed options is selected. String values are matching both values and
    /// labels. Option is considered matching if all specified properties match.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member selectOption: values: obj * ?options: Locator.selectOption.options -> JS.Promise<ResizeArray<string>>
    abstract member selectOption: values: U6<string, obj, ReadonlyArray<string>, Locator.selectOption.values.U6.Case4, ReadonlyArray<ElementHandle>, ReadonlyArray<Locator.selectOption.values.U6.Case6>> option * ?options: Locator.selectOption.options -> JS.Promise<ResizeArray<string>>
    abstract member selectOption: [<ParamArray>] values: string * ?options: Locator.selectOption.options -> JS.Promise<ResizeArray<string>>
    /// <summary>
    /// This method waits for [actionability](https://playwright.dev/docs/actionability) checks, then focuses the element and selects all its
    /// text content.
    ///
    /// If the element is inside the <c>&lt;label></c> element that has an associated
    /// [control](https://developer.mozilla.org/en-US/docs/Web/API/HTMLLabelElement/control), focuses and selects text in
    /// the control instead.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member selectText: ?options: Locator.selectText.options -> JS.Promise<unit>
    /// <summary>
    /// Set the state of a checkbox or a radio element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('checkbox').setChecked(true);
    /// </code>
    ///
    /// **Details**
    ///
    /// This method checks or unchecks an element by performing the following steps:
    /// 1. Ensure that matched element is a checkbox or a radio input. If not, this method throws.
    /// 1. If the element already has the right checked state, this method returns immediately.
    /// 1. Wait for [actionability](https://playwright.dev/docs/actionability) checks on the matched element, unless
    ///    [<c>force</c>](https://playwright.dev/docs/api/class-locator#locator-set-checked-option-force) option is set. If
    ///    the element is detached during the checks, the whole action is retried.
    /// 1. Scroll the element into view if needed.
    /// 1. Use [page.mouse](https://playwright.dev/docs/api/class-page#page-mouse) to click in the center of the
    ///    element.
    /// 1. Ensure that the element is now checked or unchecked. If not, this method throws.
    ///
    /// When all steps combined have not finished during the specified
    /// [<c>timeout</c>](https://playwright.dev/docs/api/class-locator#locator-set-checked-option-timeout), this method throws a
    /// [TimeoutError](https://playwright.dev/docs/api/class-timeouterror). Passing zero timeout disables this.
    /// </summary>
    /// <param name="checked">
    /// Whether to check or uncheck the checkbox.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member setChecked: ``checked``: bool * ?options: Locator.setChecked.options -> JS.Promise<unit>
    /// <summary>
    /// Upload file or multiple files into <c>&lt;input type=file></c>. For inputs with a <c>[webkitdirectory]</c> attribute, only a
    /// single directory path is supported.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// // Select one file
    /// await page.getByLabel('Upload file').setInputFiles(path.join(__dirname, 'myfile.pdf'));
    ///
    /// // Select multiple files
    /// await page.getByLabel('Upload files').setInputFiles([
    ///   path.join(__dirname, 'file1.txt'),
    ///   path.join(__dirname, 'file2.txt'),
    /// ]);
    ///
    /// // Select a directory
    /// await page.getByLabel('Upload directory').setInputFiles(path.join(__dirname, 'mydir'));
    ///
    /// // Remove all the selected files
    /// await page.getByLabel('Upload file').setInputFiles([]);
    ///
    /// // Upload buffer from memory
    /// await page.getByLabel('Upload file').setInputFiles({
    ///   name: 'file.txt',
    ///   mimeType: 'text/plain',
    ///   buffer: Buffer.from('this is test')
    /// });
    /// </code>
    ///
    /// **Details**
    ///
    /// Sets the value of the file input to these file paths or files. If some of the <c>filePaths</c> are relative paths, then
    /// they are resolved relative to the current working directory. For empty array, clears the selected files.
    ///
    /// This method expects [Locator](https://playwright.dev/docs/api/class-locator) to point to an
    /// [input element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input). However, if the element is inside
    /// the <c>&lt;label></c> element that has an associated
    /// [control](https://developer.mozilla.org/en-US/docs/Web/API/HTMLLabelElement/control), targets the control instead.
    /// </summary>
    /// <param name="files">
    ///
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member setInputFiles: files: U4<string, ReadonlyArray<string>, Locator.setInputFiles.files.U4.Case3, ReadonlyArray<Locator.setInputFiles.files.U4.Case4>> * ?options: Locator.setInputFiles.options -> JS.Promise<unit>
    /// <summary>
    /// Perform a tap gesture on the element matching the locator. For examples of emulating other gestures by manually
    /// dispatching touch events, see the [emulating legacy touch events](https://playwright.dev/docs/touch-events) page.
    ///
    /// **Details**
    ///
    /// This method taps the element by performing the following steps:
    /// 1. Wait for [actionability](https://playwright.dev/docs/actionability) checks on the element, unless
    ///    [<c>force</c>](https://playwright.dev/docs/api/class-locator#locator-tap-option-force) option is set.
    /// 1. Scroll the element into view if needed.
    /// 1. Use [page.touchscreen](https://playwright.dev/docs/api/class-page#page-touchscreen) to tap the center of the
    ///    element, or the specified
    ///    [<c>position</c>](https://playwright.dev/docs/api/class-locator#locator-tap-option-position).
    ///
    /// If the element is detached from the DOM at any moment during the action, this method throws.
    ///
    /// When all steps combined have not finished during the specified
    /// [<c>timeout</c>](https://playwright.dev/docs/api/class-locator#locator-tap-option-timeout), this method throws a
    /// [TimeoutError](https://playwright.dev/docs/api/class-timeouterror). Passing zero timeout disables this.
    ///
    /// **NOTE** <c>element.tap()</c> requires that the <c>hasTouch</c> option of the browser context be set to true.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member tap: ?options: Locator.tap.options -> JS.Promise<unit>
    /// <summary>
    /// Returns the [<c>node.textContent</c>](https://developer.mozilla.org/en-US/docs/Web/API/Node/textContent).
    ///
    /// **NOTE** If you need to assert text on the page, prefer
    /// [expect(locator).toHaveText(expected[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-have-text)
    /// to avoid flakiness. See [assertions guide](https://playwright.dev/docs/test-assertions) for more details.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member textContent: ?options: Locator.textContent.options -> JS.Promise<string option>
    /// <summary>
    /// Focuses the element, and then sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the
    /// text.
    ///
    /// To press a special key, like <c>Control</c> or <c>ArrowDown</c>, use
    /// [locator.press(key[, options])](https://playwright.dev/docs/api/class-locator#locator-press).
    ///
    /// **Usage**
    /// </summary>
    /// <param name="text">
    /// A text to type into a focused element.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    [<Obsolete("""In most cases, you should use
[locator.fill(value[, options])](https://playwright.dev/docs/api/class-locator#locator-fill) instead. You only need
to press keys one by one if there is special keyboard handling on the page - in this case use
[locator.pressSequentially(text[, options])](https://playwright.dev/docs/api/class-locator#locator-press-sequentially).""")>]
    abstract member ``type``: text: string * ?options: Locator.``type``.options -> JS.Promise<unit>
    /// <summary>
    /// Ensure that checkbox or radio element is unchecked.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await page.getByRole('checkbox').uncheck();
    /// </code>
    ///
    /// **Details**
    ///
    /// This method unchecks the element by performing the following steps:
    /// 1. Ensure that element is a checkbox or a radio input. If not, this method throws. If the element is already
    ///    unchecked, this method returns immediately.
    /// 1. Wait for [actionability](https://playwright.dev/docs/actionability) checks on the element, unless
    ///    [<c>force</c>](https://playwright.dev/docs/api/class-locator#locator-uncheck-option-force) option is set.
    /// 1. Scroll the element into view if needed.
    /// 1. Use [page.mouse](https://playwright.dev/docs/api/class-page#page-mouse) to click in the center of the
    ///    element.
    /// 1. Ensure that the element is now unchecked. If not, this method throws.
    ///
    /// If the element is detached from the DOM at any moment during the action, this method throws.
    ///
    /// When all steps combined have not finished during the specified
    /// [<c>timeout</c>](https://playwright.dev/docs/api/class-locator#locator-uncheck-option-timeout), this method throws a
    /// [TimeoutError](https://playwright.dev/docs/api/class-timeouterror). Passing zero timeout disables this.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member uncheck: ?options: Locator.uncheck.options -> JS.Promise<unit>
    /// <summary>
    /// Returns when element specified by locator satisfies the
    /// [<c>state</c>](https://playwright.dev/docs/api/class-locator#locator-wait-for-option-state) option.
    ///
    /// If target element already satisfies the condition, the method returns immediately. Otherwise, waits for up to
    /// [<c>timeout</c>](https://playwright.dev/docs/api/class-locator#locator-wait-for-option-timeout) milliseconds until the
    /// condition is met.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const orderSent = page.locator('#order-sent');
    /// await orderSent.waitFor();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member waitFor: ?options: Locator.waitFor.options -> JS.Promise<unit>

module Locator =

    [<Global>]
    [<AllowNullLiteral>]
    type boundingBox
        [<ParamObject; Emit("$0")>]
        (
            x: float,
            y: float,
            width: float,
            height: float
        ) =

        member val x : float = nativeOnly with get, set
        member val y : float = nativeOnly with get, set
        member val width : float = nativeOnly with get, set
        member val height : float = nativeOnly with get, set

    module ariaSnapshot =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?depth: float,
                ?mode: Locator.ariaSnapshot.options.mode,
                ?timeout: float
            ) =

            member val depth : float option = nativeOnly with get, set
            member val mode : Locator.ariaSnapshot.options.mode option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

        module options =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type mode =
                | ai
                | ``default``

    module blur =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module boundingBox =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module check =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?noWaitAfter: bool,
                ?position: Locator.check.options.position,
                ?timeout: float,
                ?trial: bool
            ) =

            member val force : bool option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val position : Locator.check.options.position option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val trial : bool option = nativeOnly with get, set

        module options =

            [<Global>]
            [<AllowNullLiteral>]
            type position
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

    module clear =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?noWaitAfter: bool,
                ?timeout: float
            ) =

            member val force : bool option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module click =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?button: Locator.click.options.button,
                ?clickCount: float,
                ?delay: float,
                ?force: bool,
                ?modifiers: ResizeArray<Locator.click.options.modifiers>,
                ?noWaitAfter: bool,
                ?position: Locator.click.options.position,
                ?steps: float,
                ?timeout: float,
                ?trial: bool
            ) =

            member val button : Locator.click.options.button option = nativeOnly with get, set
            member val clickCount : float option = nativeOnly with get, set
            member val delay : float option = nativeOnly with get, set
            member val force : bool option = nativeOnly with get, set
            member val modifiers : ResizeArray<Locator.click.options.modifiers> option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val position : Locator.click.options.position option = nativeOnly with get, set
            member val steps : float option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val trial : bool option = nativeOnly with get, set

        module options =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type button =
                | left
                | right
                | middle

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type modifiers =
                | Alt
                | Control
                | ControlOrMeta
                | Meta
                | Shift

            [<Global>]
            [<AllowNullLiteral>]
            type position
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

    module dblclick =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?button: Locator.dblclick.options.button,
                ?delay: float,
                ?force: bool,
                ?modifiers: ResizeArray<Locator.dblclick.options.modifiers>,
                ?noWaitAfter: bool,
                ?position: Locator.dblclick.options.position,
                ?steps: float,
                ?timeout: float,
                ?trial: bool
            ) =

            member val button : Locator.dblclick.options.button option = nativeOnly with get, set
            member val delay : float option = nativeOnly with get, set
            member val force : bool option = nativeOnly with get, set
            member val modifiers : ResizeArray<Locator.dblclick.options.modifiers> option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val position : Locator.dblclick.options.position option = nativeOnly with get, set
            member val steps : float option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val trial : bool option = nativeOnly with get, set

        module options =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type button =
                | left
                | right
                | middle

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type modifiers =
                | Alt
                | Control
                | ControlOrMeta
                | Meta
                | Shift

            [<Global>]
            [<AllowNullLiteral>]
            type position
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

    module dispatchEvent =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module dragTo =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?noWaitAfter: bool,
                ?sourcePosition: Locator.dragTo.options.sourcePosition,
                ?steps: float,
                ?targetPosition: Locator.dragTo.options.targetPosition,
                ?timeout: float,
                ?trial: bool
            ) =

            member val force : bool option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val sourcePosition : Locator.dragTo.options.sourcePosition option = nativeOnly with get, set
            member val steps : float option = nativeOnly with get, set
            member val targetPosition : Locator.dragTo.options.targetPosition option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val trial : bool option = nativeOnly with get, set

        module options =

            [<Global>]
            [<AllowNullLiteral>]
            type sourcePosition
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

            [<Global>]
            [<AllowNullLiteral>]
            type targetPosition
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

    module fill =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?noWaitAfter: bool,
                ?timeout: float
            ) =

            member val force : bool option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module filter =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?has: Locator,
                ?hasNot: Locator,
                ?hasNotText: U2<string, RegExp>,
                ?hasText: U2<string, RegExp>,
                ?visible: bool
            ) =

            member val has : Locator option = nativeOnly with get, set
            member val hasNot : Locator option = nativeOnly with get, set
            member val hasNotText : U2<string, RegExp> option = nativeOnly with get, set
            member val hasText : U2<string, RegExp> option = nativeOnly with get, set
            member val visible : bool option = nativeOnly with get, set

    module focus =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module getAttribute =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module getByAltText =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?exact: bool
            ) =

            member val exact : bool option = nativeOnly with get, set

    module getByLabel =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?exact: bool
            ) =

            member val exact : bool option = nativeOnly with get, set

    module getByPlaceholder =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?exact: bool
            ) =

            member val exact : bool option = nativeOnly with get, set

    module getByRole =

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.None)>]
        type role =
            | alert
            | alertdialog
            | application
            | article
            | banner
            | blockquote
            | button
            | caption
            | cell
            | checkbox
            | code
            | columnheader
            | combobox
            | complementary
            | contentinfo
            | definition
            | deletion
            | dialog
            | directory
            | document
            | emphasis
            | feed
            | figure
            | form
            | generic
            | grid
            | gridcell
            | group
            | heading
            | img
            | insertion
            | link
            | list
            | listbox
            | listitem
            | log
            | main
            | marquee
            | math
            | meter
            | menu
            | menubar
            | menuitem
            | menuitemcheckbox
            | menuitemradio
            | navigation
            | none
            | note
            | option
            | paragraph
            | presentation
            | progressbar
            | radio
            | radiogroup
            | region
            | row
            | rowgroup
            | rowheader
            | scrollbar
            | search
            | searchbox
            | separator
            | slider
            | spinbutton
            | status
            | strong
            | subscript
            | superscript
            | switch
            | tab
            | table
            | tablist
            | tabpanel
            | term
            | textbox
            | time
            | timer
            | toolbar
            | tooltip
            | tree
            | treegrid
            | treeitem

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?``checked``: bool,
                ?disabled: bool,
                ?exact: bool,
                ?expanded: bool,
                ?includeHidden: bool,
                ?level: float,
                ?name: U2<string, RegExp>,
                ?pressed: bool,
                ?selected: bool
            ) =

            member val ``checked`` : bool option = nativeOnly with get, set
            member val disabled : bool option = nativeOnly with get, set
            member val exact : bool option = nativeOnly with get, set
            member val expanded : bool option = nativeOnly with get, set
            member val includeHidden : bool option = nativeOnly with get, set
            member val level : float option = nativeOnly with get, set
            member val name : U2<string, RegExp> option = nativeOnly with get, set
            member val pressed : bool option = nativeOnly with get, set
            member val selected : bool option = nativeOnly with get, set

    module getByText =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?exact: bool
            ) =

            member val exact : bool option = nativeOnly with get, set

    module getByTitle =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?exact: bool
            ) =

            member val exact : bool option = nativeOnly with get, set

    module hover =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?modifiers: ResizeArray<Locator.hover.options.modifiers>,
                ?noWaitAfter: bool,
                ?position: Locator.hover.options.position,
                ?timeout: float,
                ?trial: bool
            ) =

            member val force : bool option = nativeOnly with get, set
            member val modifiers : ResizeArray<Locator.hover.options.modifiers> option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val position : Locator.hover.options.position option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val trial : bool option = nativeOnly with get, set

        module options =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type modifiers =
                | Alt
                | Control
                | ControlOrMeta
                | Meta
                | Shift

            [<Global>]
            [<AllowNullLiteral>]
            type position
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

    module innerHTML =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module innerText =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module inputValue =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module isChecked =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module isDisabled =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module isEditable =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module isEnabled =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module isHidden =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module isVisible =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module locator =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?has: Locator,
                ?hasNot: Locator,
                ?hasNotText: U2<string, RegExp>,
                ?hasText: U2<string, RegExp>
            ) =

            member val has : Locator option = nativeOnly with get, set
            member val hasNot : Locator option = nativeOnly with get, set
            member val hasNotText : U2<string, RegExp> option = nativeOnly with get, set
            member val hasText : U2<string, RegExp> option = nativeOnly with get, set

    module press =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?delay: float,
                ?noWaitAfter: bool,
                ?timeout: float
            ) =

            member val delay : float option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module pressSequentially =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?delay: float,
                ?noWaitAfter: bool,
                ?timeout: float
            ) =

            member val delay : float option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module scrollIntoViewIfNeeded =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module selectOption =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?noWaitAfter: bool,
                ?timeout: float
            ) =

            member val force : bool option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

        module values =

            module U6 =

                [<Global>]
                [<AllowNullLiteral>]
                type Case4
                    [<ParamObject; Emit("$0")>]
                    (
                        ?value: string,
                        ?label: string,
                        ?index: float
                    ) =

                    member val value : string option = nativeOnly with get, set
                    member val label : string option = nativeOnly with get, set
                    member val index : float option = nativeOnly with get, set

                [<Global>]
                [<AllowNullLiteral>]
                type Case6
                    [<ParamObject; Emit("$0")>]
                    (
                        ?value: string,
                        ?label: string,
                        ?index: float
                    ) =

                    member val value : string option = nativeOnly with get, set
                    member val label : string option = nativeOnly with get, set
                    member val index : float option = nativeOnly with get, set

    module selectText =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?timeout: float
            ) =

            member val force : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module setChecked =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?noWaitAfter: bool,
                ?position: Locator.setChecked.options.position,
                ?timeout: float,
                ?trial: bool
            ) =

            member val force : bool option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val position : Locator.setChecked.options.position option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val trial : bool option = nativeOnly with get, set

        module options =

            [<Global>]
            [<AllowNullLiteral>]
            type position
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

    module setInputFiles =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?noWaitAfter: bool,
                ?timeout: float
            ) =

            member val noWaitAfter : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

        module files =

            module U4 =

                [<Global>]
                [<AllowNullLiteral>]
                type Case3
                    [<ParamObject; Emit("$0")>]
                    (
                        name: string,
                        mimeType: string,
                        buffer: Buffer
                    ) =

                    member val name : string = nativeOnly with get, set
                    member val mimeType : string = nativeOnly with get, set
                    member val buffer : Buffer = nativeOnly with get, set

                [<Global>]
                [<AllowNullLiteral>]
                type Case4
                    [<ParamObject; Emit("$0")>]
                    (
                        name: string,
                        mimeType: string,
                        buffer: Buffer
                    ) =

                    member val name : string = nativeOnly with get, set
                    member val mimeType : string = nativeOnly with get, set
                    member val buffer : Buffer = nativeOnly with get, set

    module tap =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?modifiers: ResizeArray<Locator.tap.options.modifiers>,
                ?noWaitAfter: bool,
                ?position: Locator.tap.options.position,
                ?timeout: float,
                ?trial: bool
            ) =

            member val force : bool option = nativeOnly with get, set
            member val modifiers : ResizeArray<Locator.tap.options.modifiers> option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val position : Locator.tap.options.position option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val trial : bool option = nativeOnly with get, set

        module options =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type modifiers =
                | Alt
                | Control
                | ControlOrMeta
                | Meta
                | Shift

            [<Global>]
            [<AllowNullLiteral>]
            type position
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

    module textContent =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module ``type`` =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?delay: float,
                ?noWaitAfter: bool,
                ?timeout: float
            ) =

            member val delay : float option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module uncheck =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?force: bool,
                ?noWaitAfter: bool,
                ?position: Locator.uncheck.options.position,
                ?timeout: float,
                ?trial: bool
            ) =

            member val force : bool option = nativeOnly with get, set
            member val noWaitAfter : bool option = nativeOnly with get, set
            member val position : Locator.uncheck.options.position option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val trial : bool option = nativeOnly with get, set

        module options =

            [<Global>]
            [<AllowNullLiteral>]
            type position
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

    module waitFor =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?state: Locator.waitFor.options.state,
                ?timeout: float
            ) =

            member val state : Locator.waitFor.options.state option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

        module options =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type state =
                | attached
                | detached
                | visible
                | hidden
