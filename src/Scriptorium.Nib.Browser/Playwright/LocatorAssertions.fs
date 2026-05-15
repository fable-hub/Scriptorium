namespace rec Glutinum.Playwright

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AllowNullLiteral>]
[<Interface>]
type LocatorAssertions =
    /// <summary>
    /// Ensures that [Locator](https://playwright.dev/docs/api/class-locator) points to an element that is
    /// [connected](https://developer.mozilla.org/en-US/docs/Web/API/Node/isConnected) to a Document or a ShadowRoot.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await expect(page.getByText('Hidden text')).toBeAttached();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeAttached: ?options: LocatorAssertions.toBeAttached.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to a checked input.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByLabel('Subscribe to newsletter');
    /// await expect(locator).toBeChecked();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeChecked: ?options: LocatorAssertions.toBeChecked.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to a disabled element. Element is
    /// disabled if it has "disabled" attribute or is disabled via
    /// ['aria-disabled'](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Attributes/aria-disabled). Note
    /// that only native control elements such as HTML <c>button</c>, <c>input</c>, <c>select</c>, <c>textarea</c>, <c>option</c>, <c>optgroup</c> can be
    /// disabled by setting "disabled" attribute. "disabled" attribute on other elements is ignored by the browser.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('button.submit');
    /// await expect(locator).toBeDisabled();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeDisabled: ?options: LocatorAssertions.toBeDisabled.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an editable element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('textbox');
    /// await expect(locator).toBeEditable();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeEditable: ?options: LocatorAssertions.toBeEditable.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an empty editable element or to a
    /// DOM node that has no text.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('div.warning');
    /// await expect(locator).toBeEmpty();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeEmpty: ?options: LocatorAssertions.toBeEmpty.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an enabled element.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('button.submit');
    /// await expect(locator).toBeEnabled();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeEnabled: ?options: LocatorAssertions.toBeEnabled.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to a focused DOM node.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('textbox');
    /// await expect(locator).toBeFocused();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeFocused: ?options: LocatorAssertions.toBeFocused.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures that [Locator](https://playwright.dev/docs/api/class-locator) either does not resolve to any DOM node, or
    /// resolves to a [non-visible](https://playwright.dev/docs/actionability#visible) one.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('.my-element');
    /// await expect(locator).toBeHidden();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeHidden: ?options: LocatorAssertions.toBeHidden.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element that intersects viewport,
    /// according to the
    /// [intersection observer API](https://developer.mozilla.org/en-US/docs/Web/API/Intersection_Observer_API).
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('button');
    /// // Make sure at least some part of element intersects viewport.
    /// await expect(locator).toBeInViewport();
    /// // Make sure element is fully outside of viewport.
    /// await expect(locator).not.toBeInViewport();
    /// // Make sure that at least half of the element intersects viewport.
    /// await expect(locator).toBeInViewport({ ratio: 0.5 });
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeInViewport: ?options: LocatorAssertions.toBeInViewport.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures that [Locator](https://playwright.dev/docs/api/class-locator) points to an attached and
    /// [visible](https://playwright.dev/docs/actionability#visible) DOM node.
    ///
    /// To check that at least one element from the list is visible, use
    /// [locator.first()](https://playwright.dev/docs/api/class-locator#locator-first).
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// // A specific element is visible.
    /// await expect(page.getByText('Welcome')).toBeVisible();
    ///
    /// // At least one item in the list is visible.
    /// await expect(page.getByTestId('todo-item').first()).toBeVisible();
    ///
    /// // At least one of the two elements is visible, possibly both.
    /// await expect(
    ///     page.getByRole('button', { name: 'Sign in' })
    ///         .or(page.getByRole('button', { name: 'Sign up' }))
    ///         .first()
    /// ).toBeVisible();
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toBeVisible: ?options: LocatorAssertions.toBeVisible.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with given CSS classes.
    /// All classes from the asserted value, separated by spaces, must be present in the
    /// [Element.classList](https://developer.mozilla.org/en-US/docs/Web/API/Element/classList) in any order.
    ///
    /// **Usage**
    ///
    /// <code lang="html">
    /// &lt;div class='middle selected row' id='component'>&lt;/div>
    /// </code>
    ///
    /// <code lang="js">
    /// const locator = page.locator('#component');
    /// await expect(locator).toContainClass('middle selected row');
    /// await expect(locator).toContainClass('selected');
    /// await expect(locator).toContainClass('row middle');
    /// </code>
    ///
    /// When an array is passed, the method asserts that the list of elements located matches the corresponding list of
    /// expected class lists. Each element's class attribute is matched against the corresponding class in the array:
    ///
    /// <code lang="html">
    /// &lt;div class='list'>
    ///   &lt;div class='component inactive'>&lt;/div>
    ///   &lt;div class='component active'>&lt;/div>
    ///   &lt;div class='component inactive'>&lt;/div>
    /// &lt;/div>
    /// </code>
    ///
    /// <code lang="js">
    /// const locator = page.locator('.list > .component');
    /// await expect(locator).toContainClass(['inactive', 'active', 'inactive']);
    /// </code>
    /// </summary>
    /// <param name="expected">
    /// A string containing expected class names, separated by spaces, or a list of such strings to assert multiple
    /// elements.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toContainClass: expected: U2<string, ReadonlyArray<string>> * ?options: LocatorAssertions.toContainClass.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element that contains the given
    /// text. All nested elements will be considered when computing the text content of the element. You can use regular
    /// expressions for the value as well.
    ///
    /// **Details**
    ///
    /// When <c>expected</c> parameter is a string, Playwright will normalize whitespaces and line breaks both in the actual
    /// text and in the expected string before matching. When regular expression is used, the actual text is matched as is.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('.title');
    /// await expect(locator).toContainText('substring');
    /// await expect(locator).toContainText(/\d messages/);
    /// </code>
    ///
    /// If you pass an array as an expected value, the expectations are:
    /// 1. Locator resolves to a list of elements.
    /// 1. Elements from a **subset** of this list contain text from the expected array, respectively.
    /// 1. The matching subset of elements has the same order as the expected array.
    /// 1. Each text value from the expected array is matched by some element from the list.
    ///
    /// For example, consider the following list:
    ///
    /// <code lang="html">
    /// &lt;ul>
    ///   &lt;li>Item Text 1&lt;/li>
    ///   &lt;li>Item Text 2&lt;/li>
    ///   &lt;li>Item Text 3&lt;/li>
    /// &lt;/ul>
    /// </code>
    ///
    /// Let's see how we can use the assertion:
    ///
    /// <code lang="js">
    /// // ✓ Contains the right items in the right order
    /// await expect(page.locator('ul > li')).toContainText(['Text 1', 'Text 3']);
    ///
    /// // ✖ Wrong order
    /// await expect(page.locator('ul > li')).toContainText(['Text 3', 'Text 2']);
    ///
    /// // ✖ No item contains this text
    /// await expect(page.locator('ul > li')).toContainText(['Some 33']);
    ///
    /// // ✖ Locator points to the outer list element, not to the list items
    /// await expect(page.locator('ul')).toContainText(['Text 3']);
    /// </code>
    /// </summary>
    /// <param name="expected">
    /// Expected substring or RegExp or a list of those.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toContainText: [<ParamArray>] expected: string * ?options: LocatorAssertions.toContainText.options -> JS.Promise<unit>
    abstract member toContainText: [<ParamArray>] expected: RegExp * ?options: LocatorAssertions.toContainText.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with a given
    /// [accessible description](https://w3c.github.io/accname/#dfn-accessible-description).
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByTestId('save-button');
    /// await expect(locator).toHaveAccessibleDescription('Save results to disk');
    /// </code>
    /// </summary>
    /// <param name="description">
    /// Expected accessible description.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveAccessibleDescription: description: U2<string, RegExp> * ?options: LocatorAssertions.toHaveAccessibleDescription.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with a given
    /// [aria errormessage](https://w3c.github.io/aria/#aria-errormessage).
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByTestId('username-input');
    /// await expect(locator).toHaveAccessibleErrorMessage('Username is required.');
    /// </code>
    /// </summary>
    /// <param name="errorMessage">
    /// Expected accessible error message.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveAccessibleErrorMessage: errorMessage: U2<string, RegExp> * ?options: LocatorAssertions.toHaveAccessibleErrorMessage.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with a given
    /// [accessible name](https://w3c.github.io/accname/#dfn-accessible-name).
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByTestId('save-button');
    /// await expect(locator).toHaveAccessibleName('Save to disk');
    /// </code>
    /// </summary>
    /// <param name="name">
    /// Expected accessible name.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveAccessibleName: name: U2<string, RegExp> * ?options: LocatorAssertions.toHaveAccessibleName.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with given attribute.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('input');
    /// await expect(locator).toHaveAttribute('type', 'text');
    /// </code>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with given attribute. The
    /// method will assert attribute presence.
    ///
    /// <code lang="js">
    /// const locator = page.locator('input');
    /// // Assert attribute existence.
    /// await expect(locator).toHaveAttribute('disabled');
    /// await expect(locator).not.toHaveAttribute('open');
    /// </code>
    /// </summary>
    /// <param name="name">
    /// Attribute name.
    /// </param>
    /// <param name="value">
    /// Expected attribute value.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveAttribute: name: string * value: U2<string, RegExp> * ?options: LocatorAssertions.toHaveAttribute.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with given attribute.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('input');
    /// await expect(locator).toHaveAttribute('type', 'text');
    /// </code>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with given attribute. The
    /// method will assert attribute presence.
    ///
    /// <code lang="js">
    /// const locator = page.locator('input');
    /// // Assert attribute existence.
    /// await expect(locator).toHaveAttribute('disabled');
    /// await expect(locator).not.toHaveAttribute('open');
    /// </code>
    /// </summary>
    /// <param name="name">
    /// Attribute name.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveAttribute: name: string * ?options: LocatorAssertions.toHaveAttribute.options_1 -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with given CSS classes.
    /// When a string is provided, it must fully match the element's <c>class</c> attribute. To match individual classes use
    /// [expect(locator).toContainClass(expected[, options])](https://playwright.dev/docs/api/class-locatorassertions#locator-assertions-to-contain-class).
    ///
    /// **Usage**
    ///
    /// <code lang="html">
    /// &lt;div class='middle selected row' id='component'>&lt;/div>
    /// </code>
    ///
    /// <code lang="js">
    /// const locator = page.locator('#component');
    /// await expect(locator).toHaveClass('middle selected row');
    /// await expect(locator).toHaveClass(/(^|\s)selected(\s|$)/);
    /// </code>
    ///
    /// When an array is passed, the method asserts that the list of elements located matches the corresponding list of
    /// expected class values. Each element's class attribute is matched against the corresponding string or regular
    /// expression in the array:
    ///
    /// <code lang="js">
    /// const locator = page.locator('.list > .component');
    /// await expect(locator).toHaveClass(['component', 'component selected', 'component']);
    /// </code>
    /// </summary>
    /// <param name="expected">
    /// Expected class or RegExp or a list of those.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveClass: expected: U3<string, RegExp, ReadonlyArray<U2<string, RegExp>>> * ?options: LocatorAssertions.toHaveClass.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) resolves to an exact number of DOM nodes.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const list = page.locator('list > .component');
    /// await expect(list).toHaveCount(3);
    /// </code>
    /// </summary>
    /// <param name="count">
    /// Expected count.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveCount: count: float * ?options: LocatorAssertions.toHaveCount.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) resolves to an element with the given computed
    /// CSS style.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('button');
    /// await expect(locator).toHaveCSS('display', 'flex');
    /// </code>
    /// </summary>
    /// <param name="name">
    /// CSS property name.
    /// </param>
    /// <param name="value">
    /// CSS property value.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveCSS: name: string * value: U2<string, RegExp> * ?options: LocatorAssertions.toHaveCSS.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with the given DOM Node
    /// ID.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('textbox');
    /// await expect(locator).toHaveId('lastname');
    /// </code>
    /// </summary>
    /// <param name="id">
    /// Element id.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveId: id: U2<string, RegExp> * ?options: LocatorAssertions.toHaveId.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with given JavaScript
    /// property. Note that this property can be of a primitive type as well as a plain serializable JavaScript object.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('.component');
    /// await expect(locator).toHaveJSProperty('loaded', true);
    /// </code>
    /// </summary>
    /// <param name="name">
    /// Property name.
    /// </param>
    /// <param name="value">
    /// Property value.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveJSProperty: name: string * value: obj * ?options: LocatorAssertions.toHaveJSProperty.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with a given
    /// [ARIA role](https://www.w3.org/TR/wai-aria-1.2/#roles).
    ///
    /// Note that role is matched as a string, disregarding the ARIA role hierarchy. For example, asserting  a superclass
    /// role <c>"checkbox"</c> on an element with a subclass role <c>"switch"</c> will fail.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByTestId('save-button');
    /// await expect(locator).toHaveRole('button');
    /// </code>
    /// </summary>
    /// <param name="role">
    /// Required aria role.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveRole: role: LocatorAssertions.toHaveRole.role * ?options: LocatorAssertions.toHaveRole.options -> JS.Promise<unit>
    /// <summary>
    /// This function will wait until two consecutive locator screenshots yield the same result, and then compare the last
    /// screenshot with the expectation.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('button');
    /// await expect(locator).toHaveScreenshot('image.png');
    /// </code>
    ///
    /// Note that screenshot assertions only work with Playwright test runner.
    /// This function will wait until two consecutive locator screenshots yield the same result, and then compare the last
    /// screenshot with the expectation.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('button');
    /// await expect(locator).toHaveScreenshot();
    /// </code>
    ///
    /// Note that screenshot assertions only work with Playwright test runner.
    /// </summary>
    /// <param name="name">
    /// Snapshot name.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveScreenshot: name: U2<string, ReadonlyArray<string>> * ?options: LocatorAssertions.toHaveScreenshot.options -> JS.Promise<unit>
    /// <summary>
    /// This function will wait until two consecutive locator screenshots yield the same result, and then compare the last
    /// screenshot with the expectation.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('button');
    /// await expect(locator).toHaveScreenshot('image.png');
    /// </code>
    ///
    /// Note that screenshot assertions only work with Playwright test runner.
    /// This function will wait until two consecutive locator screenshots yield the same result, and then compare the last
    /// screenshot with the expectation.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.getByRole('button');
    /// await expect(locator).toHaveScreenshot();
    /// </code>
    ///
    /// Note that screenshot assertions only work with Playwright test runner.
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveScreenshot: ?options: LocatorAssertions.toHaveScreenshot.options_1 -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with the given text. All
    /// nested elements will be considered when computing the text content of the element. You can use regular expressions
    /// for the value as well.
    ///
    /// **Details**
    ///
    /// When <c>expected</c> parameter is a string, Playwright will normalize whitespaces and line breaks both in the actual
    /// text and in the expected string before matching. When regular expression is used, the actual text is matched as is.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('.title');
    /// await expect(locator).toHaveText(/Welcome, Test User/);
    /// await expect(locator).toHaveText(/Welcome, .*\/);
    /// </code>
    ///
    /// If you pass an array as an expected value, the expectations are:
    /// 1. Locator resolves to a list of elements.
    /// 1. The number of elements equals the number of expected values in the array.
    /// 1. Elements from the list have text matching expected array values, one by one, in order.
    ///
    /// For example, consider the following list:
    ///
    /// <code lang="html">
    /// &lt;ul>
    ///   &lt;li>Text 1&lt;/li>
    ///   &lt;li>Text 2&lt;/li>
    ///   &lt;li>Text 3&lt;/li>
    /// &lt;/ul>
    /// </code>
    ///
    /// Let's see how we can use the assertion:
    ///
    /// <code lang="js">
    /// // ✓ Has the right items in the right order
    /// await expect(page.locator('ul > li')).toHaveText(['Text 1', 'Text 2', 'Text 3']);
    ///
    /// // ✖ Wrong order
    /// await expect(page.locator('ul > li')).toHaveText(['Text 3', 'Text 2', 'Text 1']);
    ///
    /// // ✖ Last item does not match
    /// await expect(page.locator('ul > li')).toHaveText(['Text 1', 'Text 2', 'Text']);
    ///
    /// // ✖ Locator points to the outer list element, not to the list items
    /// await expect(page.locator('ul')).toHaveText(['Text 1', 'Text 2', 'Text 3']);
    /// </code>
    /// </summary>
    /// <param name="expected">
    /// Expected string or RegExp or a list of those.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveText: [<ParamArray>] expected: string * ?options: LocatorAssertions.toHaveText.options -> JS.Promise<unit>
    abstract member toHaveText: [<ParamArray>] expected: RegExp * ?options: LocatorAssertions.toHaveText.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to an element with the given input
    /// value. You can use regular expressions for the value as well.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// const locator = page.locator('input[type=number]');
    /// await expect(locator).toHaveValue(/[0-9]/);
    /// </code>
    /// </summary>
    /// <param name="value">
    /// Expected value.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveValue: value: U2<string, RegExp> * ?options: LocatorAssertions.toHaveValue.options -> JS.Promise<unit>
    /// <summary>
    /// Ensures the [Locator](https://playwright.dev/docs/api/class-locator) points to multi-select/combobox (i.e. a
    /// <c>select</c> with the <c>multiple</c> attribute) and the specified values are selected.
    ///
    /// **Usage**
    ///
    /// For example, given the following element:
    ///
    /// <code lang="html">
    /// &lt;select id="favorite-colors" multiple>
    ///   &lt;option value="R">Red&lt;/option>
    ///   &lt;option value="G">Green&lt;/option>
    ///   &lt;option value="B">Blue&lt;/option>
    /// &lt;/select>
    /// </code>
    ///
    /// <code lang="js">
    /// const locator = page.locator('id=favorite-colors');
    /// await locator.selectOption(['R', 'G']);
    /// await expect(locator).toHaveValues([/R/, /G/]);
    /// </code>
    /// </summary>
    /// <param name="values">
    /// Expected options currently selected.
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toHaveValues: values: ReadonlyArray<U2<string, RegExp>> * ?options: LocatorAssertions.toHaveValues.options -> JS.Promise<unit>
    /// <summary>
    /// Asserts that the target element matches the given [accessibility snapshot](https://playwright.dev/docs/aria-snapshots).
    ///
    /// **Usage**
    ///
    /// <c></c><c>js
    /// await page.goto('https://demo.playwright.dev/todomvc/');
    /// await expect(page.locator('body')).toMatchAriaSnapshot(</c>
    ///   - heading "todos"
    ///   - textbox "What needs to be done?"
    /// <c>);
    /// </c><c></c>
    /// Asserts that the target element matches the given [accessibility snapshot](https://playwright.dev/docs/aria-snapshots).
    ///
    /// Snapshot is stored in a separate <c>.aria.yml</c> file in a location configured by
    /// <c>expect.toMatchAriaSnapshot.pathTemplate</c> and/or <c>snapshotPathTemplate</c> properties in the configuration file.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await expect(page.locator('body')).toMatchAriaSnapshot();
    /// await expect(page.locator('body')).toMatchAriaSnapshot({ name: 'body.aria.yml' });
    /// </code>
    /// </summary>
    /// <param name="expected">
    ///
    /// </param>
    /// <param name="options">
    ///
    /// </param>
    abstract member toMatchAriaSnapshot: expected: string * ?options: LocatorAssertions.toMatchAriaSnapshot.options -> JS.Promise<unit>
    /// <summary>
    /// Asserts that the target element matches the given [accessibility snapshot](https://playwright.dev/docs/aria-snapshots).
    ///
    /// **Usage**
    ///
    /// <c></c><c>js
    /// await page.goto('https://demo.playwright.dev/todomvc/');
    /// await expect(page.locator('body')).toMatchAriaSnapshot(</c>
    ///   - heading "todos"
    ///   - textbox "What needs to be done?"
    /// <c>);
    /// </c><c></c>
    /// Asserts that the target element matches the given [accessibility snapshot](https://playwright.dev/docs/aria-snapshots).
    ///
    /// Snapshot is stored in a separate <c>.aria.yml</c> file in a location configured by
    /// <c>expect.toMatchAriaSnapshot.pathTemplate</c> and/or <c>snapshotPathTemplate</c> properties in the configuration file.
    ///
    /// **Usage**
    ///
    /// <code lang="js">
    /// await expect(page.locator('body')).toMatchAriaSnapshot();
    /// await expect(page.locator('body')).toMatchAriaSnapshot({ name: 'body.aria.yml' });
    /// </code>
    /// </summary>
    /// <param name="options">
    ///
    /// </param>
    abstract member toMatchAriaSnapshot: ?options: LocatorAssertions.toMatchAriaSnapshot.options_1 -> JS.Promise<unit>
    /// <summary>
    /// Makes the assertion check for the opposite condition.
    ///
    /// **Usage**
    ///
    /// For example, this code tests that the Locator doesn't contain text <c>"error"</c>:
    ///
    /// <code lang="js">
    /// await expect(locator).not.toContainText('error');
    /// </code>
    /// </summary>
    abstract member not: LocatorAssertions with get, set

module LocatorAssertions =

    module toBeAttached =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?attached: bool,
                ?timeout: float
            ) =

            member val attached : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toBeChecked =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?``checked``: bool,
                ?indeterminate: bool,
                ?timeout: float
            ) =

            member val ``checked`` : bool option = nativeOnly with get, set
            member val indeterminate : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toBeDisabled =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toBeEditable =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?editable: bool,
                ?timeout: float
            ) =

            member val editable : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toBeEmpty =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toBeEnabled =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?enabled: bool,
                ?timeout: float
            ) =

            member val enabled : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toBeFocused =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toBeHidden =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toBeInViewport =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?ratio: float,
                ?timeout: float
            ) =

            member val ratio : float option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toBeVisible =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float,
                ?visible: bool
            ) =

            member val timeout : float option = nativeOnly with get, set
            member val visible : bool option = nativeOnly with get, set

    module toContainClass =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toContainText =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?ignoreCase: bool,
                ?timeout: float,
                ?useInnerText: bool
            ) =

            member val ignoreCase : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val useInnerText : bool option = nativeOnly with get, set

    module toHaveAccessibleDescription =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?ignoreCase: bool,
                ?timeout: float
            ) =

            member val ignoreCase : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toHaveAccessibleErrorMessage =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?ignoreCase: bool,
                ?timeout: float
            ) =

            member val ignoreCase : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toHaveAccessibleName =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?ignoreCase: bool,
                ?timeout: float
            ) =

            member val ignoreCase : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toHaveAttribute =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?ignoreCase: bool,
                ?timeout: float
            ) =

            member val ignoreCase : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

        [<Global>]
        [<AllowNullLiteral>]
        type options_1
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toHaveClass =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toHaveCount =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toHaveCSS =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toHaveId =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toHaveJSProperty =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toHaveRole =

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
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toHaveScreenshot =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?animations: LocatorAssertions.toHaveScreenshot.options.animations,
                ?caret: LocatorAssertions.toHaveScreenshot.options.caret,
                ?mask: ResizeArray<Locator>,
                ?maskColor: string,
                ?maxDiffPixelRatio: float,
                ?maxDiffPixels: float,
                ?omitBackground: bool,
                ?scale: LocatorAssertions.toHaveScreenshot.options.scale,
                ?stylePath: U2<string, ResizeArray<string>>,
                ?threshold: float,
                ?timeout: float
            ) =

            member val animations : LocatorAssertions.toHaveScreenshot.options.animations option = nativeOnly with get, set
            member val caret : LocatorAssertions.toHaveScreenshot.options.caret option = nativeOnly with get, set
            member val mask : ResizeArray<Locator> option = nativeOnly with get, set
            member val maskColor : string option = nativeOnly with get, set
            member val maxDiffPixelRatio : float option = nativeOnly with get, set
            member val maxDiffPixels : float option = nativeOnly with get, set
            member val omitBackground : bool option = nativeOnly with get, set
            member val scale : LocatorAssertions.toHaveScreenshot.options.scale option = nativeOnly with get, set
            member val stylePath : U2<string, ResizeArray<string>> option = nativeOnly with get, set
            member val threshold : float option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

        module options =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type animations =
                | disabled
                | allow

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type caret =
                | hide
                | initial

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type scale =
                | css
                | device

        [<Global>]
        [<AllowNullLiteral>]
        type options_1
            [<ParamObject; Emit("$0")>]
            (
                ?animations: LocatorAssertions.toHaveScreenshot.options.animations,
                ?caret: LocatorAssertions.toHaveScreenshot.options.caret,
                ?mask: ResizeArray<Locator>,
                ?maskColor: string,
                ?maxDiffPixelRatio: float,
                ?maxDiffPixels: float,
                ?omitBackground: bool,
                ?scale: LocatorAssertions.toHaveScreenshot.options.scale,
                ?stylePath: U2<string, ResizeArray<string>>,
                ?threshold: float,
                ?timeout: float
            ) =

            member val animations : LocatorAssertions.toHaveScreenshot.options.animations option = nativeOnly with get, set
            member val caret : LocatorAssertions.toHaveScreenshot.options.caret option = nativeOnly with get, set
            member val mask : ResizeArray<Locator> option = nativeOnly with get, set
            member val maskColor : string option = nativeOnly with get, set
            member val maxDiffPixelRatio : float option = nativeOnly with get, set
            member val maxDiffPixels : float option = nativeOnly with get, set
            member val omitBackground : bool option = nativeOnly with get, set
            member val scale : LocatorAssertions.toHaveScreenshot.options.scale option = nativeOnly with get, set
            member val stylePath : U2<string, ResizeArray<string>> option = nativeOnly with get, set
            member val threshold : float option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set

    module toHaveText =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?ignoreCase: bool,
                ?timeout: float,
                ?useInnerText: bool
            ) =

            member val ignoreCase : bool option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
            member val useInnerText : bool option = nativeOnly with get, set

    module toHaveValue =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toHaveValues =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

    module toMatchAriaSnapshot =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?timeout: float
            ) =

            member val timeout : float option = nativeOnly with get, set

        [<Global>]
        [<AllowNullLiteral>]
        type options_1
            [<ParamObject; Emit("$0")>]
            (
                ?name: string,
                ?timeout: float
            ) =

            member val name : string option = nativeOnly with get, set
            member val timeout : float option = nativeOnly with get, set
