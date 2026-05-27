module Scriptorium.Nib.Browser.Test.Main

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open Scriptorium.Nib.Browser
open Glutinum.Playwright

open type Scriptorium.Nib.Browser.UserEvents
open type Scriptorium.Nib.Browser.BrowserTest
open type Scriptorium.Quill.Runner
open type Scriptorium.Quill.Test

[<EntryPoint>]
let main _ =

    let tests =
        testList (
            "Scriptorium.Nib.Browser",
            // Set a longer timeout for browser tests since GitHub Actions fails 1 times out of 2 with the default 5s timeout.
            timeout 10000,
            [

                testPage (
                    "a created div exists in the document",
                    fun page ->
                        promise {
                            do! page.setContent "<html><body><div>Hello</div></body></html>"
                            do! assertLocator (page.locator "div") toBeVisible
                        }
                )

                testPage (
                    "a div with Hello World text contains that text",
                    fun page ->
                        promise {
                            do! page.setContent "<html><body><div>Hello World</div></body></html>"
                            do! assertLocator (page.locator "div") (haveText "Hello World")
                        }
                )

                testList (
                    "Counter",
                    [

                        let counterHtml =
                            """
                            <html><body>
                                <script>let count = 0;</script>
                                <p id="count">0</p>
                                <button id="increment" onclick="document.getElementById('count').textContent = ++count">+</button>
                                <button id="decrement" onclick="document.getElementById('count').textContent = --count">-</button>
                            </body></html>
                            """

                        testPage (
                            "starts at zero",
                            fun page ->
                                promise {
                                    do! page.setContent counterHtml

                                    do!
                                        assertLocator
                                            (page.locator "#count")
                                            (toBeVisible >>. haveText "0")
                                }
                        )

                        testPage (
                            "increments when + is clicked",
                            fun page ->
                                promise {
                                    do! page.setContent counterHtml
                                    do! click (page.locator "#increment")
                                    do! assertLocator (page.locator "#count") (haveText "1")
                                }
                        )

                        testPage (
                            "decrements when - is clicked",
                            fun page ->
                                promise {
                                    do! page.setContent counterHtml
                                    do! click (page.locator "#decrement")
                                    do! assertLocator (page.locator "#count") (haveText "-1")
                                }
                        )

                        testPage (
                            "can increment multiple times",
                            fun page ->
                                promise {
                                    do! page.setContent counterHtml
                                    let incrementButton = page.locator "#increment"
                                    do! click incrementButton
                                    do! click incrementButton
                                    do! click incrementButton
                                    do! assertLocator (page.locator "#count") (haveText "3")
                                }
                        )

                    ]
                )

                testList (
                    "Locator.all",
                    [

                        testPage (
                            "returns one locator per matching element",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            """
                                            <html><body>
                                                <li class="item">Apple</li>
                                                <li class="item">Banana</li>
                                                // <li class="item">Cherry</li>
                                            </body></html>
                                            """

                                    let! items = page.locator(".item").all ()

                                    assertThat items.Count (isEqualTo 3)

                                    for item in items do
                                        do! assertLocator item toBeVisible
                                }
                        )

                        testPage (
                            "returns empty list when no elements match",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body></body></html>"
                                    let! items = page.locator(".nonexistent").all ()

                                    assertThat items.Count (isEqualTo 0)
                                }
                        )

                        testPage (
                            "each returned locator targets the correct element",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            """
                                            <html><body>
                                                <li class="item">Apple</li>
                                                <li class="item">Banana</li>
                                                <li class="item">Cherry</li>
                                            </body></html>
                                            """

                                    let! items = page.locator(".item").all ()

                                    let expected =
                                        [
                                            "Apple"
                                            "Banana"
                                            "Cherry"
                                        ]

                                    for item, text in Seq.zip items expected do
                                        do! assertLocator item (haveText text)
                                }
                        )

                    ]
                )

                testList (
                    "Locator.and_",
                    [

                        testPage (
                            "narrows selection to elements matching both locators",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            """
                                            <html><body>
                                                <button class="primary">Save</button>
                                                <button class="secondary">Cancel</button>
                                                <button class="primary danger">Delete</button>
                                            </body></html>
                                            """

                                    let primaryDanger =
                                        page.locator("button.primary").and_ (page.locator ".danger")

                                    do!
                                        assertLocator
                                            primaryDanger
                                            (toBeVisible >>. haveText "Delete")
                                }
                        )

                        testPage (
                            "does not match when intersection is empty",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            """
                                            <html><body>
                                                <button class="primary">Save</button>
                                                <button class="secondary">Cancel</button>
                                            </body></html>
                                            """

                                    let primaryAndSecondary =
                                        page
                                            .locator("button.primary")
                                            .and_ (page.locator ".secondary")

                                    let! count = primaryAndSecondary.count ()

                                    assertThat count (isEqualTo 0)
                                }
                        )

                    ]
                )

                testList (
                    "Locator.blur",
                    [

                        let blurHtml =
                            """
                            <html><body>
                                <input id="name" />
                                <p id="status">focused</p>
                                <script>
                                    document.getElementById('name').addEventListener('blur', function() {
                                        document.getElementById('status').textContent = 'blurred';
                                    });
                                </script>
                            </body></html>
                            """

                        testPage (
                            "fires the blur event on the element",
                            fun page ->
                                promise {
                                    do! page.setContent blurHtml
                                    let input = page.locator "#name"

                                    do! focus input
                                    do! blur input

                                    do! assertLocator (page.locator "#status") (haveText "blurred")
                                }
                        )

                        testPage (
                            "fires the blur event when a timeout is specified",
                            fun page ->
                                promise {
                                    do! page.setContent blurHtml
                                    let input = page.locator "#name"

                                    do! focus input
                                    do! input.blur (Locator.blur.options (500))

                                    do! assertLocator (page.locator "#status") (haveText "blurred")
                                }
                        )

                    ]
                )

                testList (
                    "UserEvents.hover",
                    [

                        let hoverHtml =
                            """
                            <html><body>
                                <div id="target">Hover me</div>
                                <p id="status">idle</p>
                                <script>
                                    document.getElementById('target').addEventListener('mouseover', function() {
                                        document.getElementById('status').textContent = 'hovered';
                                    });
                                </script>
                            </body></html>
                            """

                        testPage (
                            "fires mouseover on the element",
                            fun page ->
                                promise {
                                    do! page.setContent hoverHtml
                                    do! hover (page.locator "#target")
                                    do! assertLocator (page.locator "#status") (haveText "hovered")
                                }
                        )

                    ]
                )

                testList (
                    "UserEvents.selectOption",
                    [

                        let selectHtml =
                            """
                            <html><body>
                                <select id="fruit">
                                    <option value="apple">Apple</option>
                                    <option value="banana">Banana</option>
                                    <option value="cherry">Cherry</option>
                                </select>
                                <p id="status">none</p>
                                <script>
                                    document.getElementById('fruit').addEventListener('change', function() {
                                        document.getElementById('status').textContent = this.value;
                                    });
                                </script>
                            </body></html>
                            """

                        testPage (
                            "selects the given option by value",
                            fun page ->
                                promise {
                                    do! page.setContent selectHtml
                                    do! selectOption (page.locator "#fruit", "banana")
                                    do! assertLocator (page.locator "#status") (haveText "banana")
                                }
                        )

                        testPage (
                            "can select a different option",
                            fun page ->
                                promise {
                                    do! page.setContent selectHtml
                                    do! selectOption (page.locator "#fruit", "cherry")
                                    do! assertLocator (page.locator "#status") (haveText "cherry")
                                }
                        )

                    ]
                )

                testList (
                    "UserEvents.press",
                    [

                        let pressHtml =
                            """
                            <html><body>
                                <input id="field" />
                                <p id="status">none</p>
                                <script>
                                    document.getElementById('field').addEventListener('keydown', function(e) {
                                        document.getElementById('status').textContent = e.key;
                                    });
                                </script>
                            </body></html>
                            """

                        testPage (
                            "fires keydown with the given key",
                            fun page ->
                                promise {
                                    do! page.setContent pressHtml
                                    do! press (page.locator "#field", "Enter")
                                    do! assertLocator (page.locator "#status") (haveText "Enter")
                                }
                        )

                        testPage (
                            "fires keydown with a letter key",
                            fun page ->
                                promise {
                                    do! page.setContent pressHtml
                                    do! press (page.locator "#field", "a")
                                    do! assertLocator (page.locator "#status") (haveText "a")
                                }
                        )

                    ]
                )

                testList (
                    "UserEvents.highlight",
                    [

                        testPage (
                            "does not throw and element remains visible",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body><div id=\"target\">Hello</div></body></html>"
                                    let target = page.locator "#target"
                                    do! highlight target
                                    do! assertLocator target toBeVisible
                                }
                        )

                    ]
                )

                testList (
                    "UserEvents.fill",
                    [

                        testPage (
                            "sets the value of the input",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body><input id=\"field\" /></body></html>"
                                    do! fill (page.locator "#field", "Hello")
                                    do! assertLocator (page.locator "#field") (haveValue "Hello")
                                }
                        )

                        testPage (
                            "replaces an existing value",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><input id=\"field\" value=\"old\" /></body></html>"

                                    do! fill (page.locator "#field", "new")
                                    do! assertLocator (page.locator "#field") (haveValue "new")
                                }
                        )

                    ]
                )

                testList (
                    "check and uncheck",
                    [

                        let checkboxHtml =
                            """
                            <html><body>
                                <input type="checkbox" id="toggle" />
                                <p id="status">unchecked</p>
                                <script>
                                    document.getElementById('toggle').addEventListener('change', function() {
                                        document.getElementById('status').textContent = this.checked ? 'checked' : 'unchecked';
                                    });
                                </script>
                            </body></html>
                            """

                        testPage (
                            "check sets the checkbox to checked",
                            fun page ->
                                promise {
                                    do! page.setContent checkboxHtml
                                    do! check (page.locator "#toggle")
                                    do! assertLocator (page.locator "#status") (haveText "checked")
                                }
                        )

                        testPage (
                            "uncheck sets the checkbox to unchecked",
                            fun page ->
                                promise {
                                    do! page.setContent checkboxHtml
                                    let checkbox = page.locator "#toggle"
                                    do! check checkbox
                                    do! uncheck checkbox

                                    do!
                                        assertLocator
                                            (page.locator "#status")
                                            (haveText "unchecked")
                                }
                        )

                        testPage (
                            "can toggle multiple times",
                            fun page ->
                                promise {
                                    do! page.setContent checkboxHtml
                                    let checkbox = page.locator "#toggle"
                                    do! check checkbox
                                    do! uncheck checkbox
                                    do! check checkbox
                                    do! assertLocator (page.locator "#status") (haveText "checked")
                                }
                        )

                    ]
                )

                testList (
                    "beHidden",
                    [

                        testPage (
                            "passes when the element is not in the DOM",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body></body></html>"
                                    do! assertLocator (page.locator "#missing") beHidden
                                }
                        )

                        testPage (
                            "passes when the element has display:none",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><div id=\"el\" style=\"display:none\">hidden</div></body></html>"

                                    do! assertLocator (page.locator "#el") beHidden
                                }
                        )

                    ]
                )

                testList (
                    "beDisabled / beEnabled",
                    [

                        testPage (
                            "beDisabled passes for a disabled button",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><button id=\"btn\" disabled>Submit</button></body></html>"

                                    do! assertLocator (page.locator "#btn") beDisabled
                                }
                        )

                        testPage (
                            "beEnabled passes for a non-disabled button",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><button id=\"btn\">Submit</button></body></html>"

                                    do! assertLocator (page.locator "#btn") beEnabled
                                }
                        )

                    ]
                )

                testList (
                    "beFocused",
                    [

                        testPage (
                            "passes after the element receives focus",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><input id=\"field\" /></body></html>"

                                    let field = page.locator "#field"
                                    do! focus field
                                    do! assertLocator field beFocused
                                }
                        )

                    ]
                )

                testList (
                    "beEmpty",
                    [

                        testPage (
                            "passes for an empty input",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><input id=\"field\" /></body></html>"

                                    do! assertLocator (page.locator "#field") beEmpty
                                }
                        )

                        testPage (
                            "passes for a div with no text",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><div id=\"el\"></div></body></html>"

                                    do! assertLocator (page.locator "#el") beEmpty
                                }
                        )

                    ]
                )

                testList (
                    "beEditable",
                    [

                        testPage (
                            "passes for a regular input",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><input id=\"field\" /></body></html>"

                                    do! assertLocator (page.locator "#field") beEditable
                                }
                        )

                    ]
                )

                testList (
                    "not' / not_",
                    [

                        // Playwright assertions all use retry logic — they wait up to the
                        // configured timeout before giving up. A `not'` test that relies on
                        // a Playwright assertion failing would stall for the full timeout.
                        // We use a locally defined `alwaysFail` that returns an error state
                        // immediately to keep these tests fast.
                        let alwaysFail : DomAssertion<Locator> =
                            fun state -> promise { return { state with Errors = "always fails" :: state.Errors } }

                        testPage (
                            "not' passes when the inner assertion fails",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body><div id=\"el\">Hello</div></body></html>"
                                    do! assertLocator (page.locator "#el") (not' alwaysFail)
                                }
                        )

                        testPage (
                            "not' fails when the inner assertion passes",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body><div id=\"el\">Hello</div></body></html>"

                                    // toBeVisible passes immediately on a visible element → not' should inject a failure
                                    let mutable errorMsg = ""

                                    try
                                        do! assertLocator (page.locator "#el") (not' toBeVisible)
                                    with ex ->
                                        errorMsg <- ex.Message

                                    assertThat
                                        errorMsg
                                        (satisfy (fun m ->
                                            m.Contains "Expected assertion to fail but it passed"
                                        ))
                                }
                        )

                        testPage (
                            "not_ is an alias for not'",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body><div id=\"el\">Hello</div></body></html>"
                                    do! assertLocator (page.locator "#el") (not_ alwaysFail)
                                }
                        )

                        testPage (
                            "not' can be chained with other assertions",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body><div id=\"el\">Hello</div></body></html>"

                                    do!
                                        assertLocator
                                            (page.locator "#el")
                                            (not' alwaysFail >>. haveText "Hello")
                                }
                        )

                    ]
                )

                testList (
                    "haveCount",
                    [

                        testPage (
                            "passes when the locator matches the expected number of elements",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            """
                                            <html><body>
                                                <li class="item">A</li>
                                                <li class="item">B</li>
                                                <li class="item">C</li>
                                            </body></html>
                                            """

                                    do! assertLocator (page.locator ".item") (haveCount 3)
                                }
                        )

                        testPage (
                            "passes with zero when no elements match",
                            fun page ->
                                promise {
                                    do! page.setContent "<html><body></body></html>"
                                    do! assertLocator (page.locator ".item") (haveCount 0)
                                }
                        )

                    ]
                )

                testList (
                    "haveAttribute",
                    [

                        testPage (
                            "passes when the attribute matches the expected value",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><input id=\"field\" type=\"email\" /></body></html>"

                                    do!
                                        assertLocator
                                            (page.locator "#field")
                                            (haveAttribute "type" "email")
                                }
                        )

                        testPage (
                            "passes for a data attribute",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><div id=\"el\" data-status=\"active\"></div></body></html>"

                                    do!
                                        assertLocator
                                            (page.locator "#el")
                                            (haveAttribute "data-status" "active")
                                }
                        )

                    ]
                )

                testList (
                    "haveClass / containClass",
                    [

                        testPage (
                            "haveClass passes when the class attribute exactly matches",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><div id=\"el\" class=\"btn primary\"></div></body></html>"

                                    do!
                                        assertLocator (page.locator "#el") (haveClass "btn primary")
                                }
                        )

                        testPage (
                            "containClass passes when the element has the given class among others",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><div id=\"el\" class=\"btn primary active\"></div></body></html>"

                                    do!
                                        assertLocator (page.locator "#el") (containClass "primary")
                                }
                        )

                    ]
                )

                testList (
                    "haveCSS",
                    [

                        testPage (
                            "passes when the computed style matches",
                            fun page ->
                                promise {
                                    do!
                                        page.setContent
                                            "<html><body><div id=\"el\" style=\"color: red\"></div></body></html>"

                                    do! assertLocator (page.locator "#el") (haveCSS "color" "rgb(255, 0, 0)")
                                }
                        )

                    ]
                )

            ]
        )

    runTestsWith(slowThreshold 500, tests)
