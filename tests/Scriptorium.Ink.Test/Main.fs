module Scriptorium.Ink.Test.Main

open Scriptorium.Ink
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test
open type Scriptorium.Quill.Runner

let private foregroundColours =
    testList (
        "foreground colours",
        [
            test ("black", fun _ -> assertThat (black "text") (isEqualTo "\x1b[30mtext\x1b[39m"))
            test ("red", fun _ -> assertThat (red "text") (isEqualTo "\x1b[31mtext\x1b[39m"))
            test ("green", fun _ -> assertThat (green "text") (isEqualTo "\x1b[32mtext\x1b[39m"))
            test ("yellow", fun _ -> assertThat (yellow "text") (isEqualTo "\x1b[33mtext\x1b[39m"))
            test ("blue", fun _ -> assertThat (blue "text") (isEqualTo "\x1b[34mtext\x1b[39m"))
            test (
                "magenta",
                fun _ -> assertThat (magenta "text") (isEqualTo "\x1b[35mtext\x1b[39m")
            )
            test ("cyan", fun _ -> assertThat (cyan "text") (isEqualTo "\x1b[36mtext\x1b[39m"))
            test ("white", fun _ -> assertThat (white "text") (isEqualTo "\x1b[37mtext\x1b[39m"))
            test ("empty string", fun _ -> assertThat (red "") (isEqualTo "\x1b[31m\x1b[39m"))
        ]
    )

let private foregroundBrightColours =
    testList (
        "foreground bright colours",
        [
            test (
                "blackBright",
                fun _ -> assertThat (blackBright "text") (isEqualTo "\x1b[90mtext\x1b[39m")
            )
            test (
                "redBright",
                fun _ -> assertThat (redBright "text") (isEqualTo "\x1b[91mtext\x1b[39m")
            )
            test (
                "greenBright",
                fun _ -> assertThat (greenBright "text") (isEqualTo "\x1b[92mtext\x1b[39m")
            )
            test (
                "yellowBright",
                fun _ -> assertThat (yellowBright "text") (isEqualTo "\x1b[93mtext\x1b[39m")
            )
            test (
                "blueBright",
                fun _ -> assertThat (blueBright "text") (isEqualTo "\x1b[94mtext\x1b[39m")
            )
            test (
                "magentaBright",
                fun _ -> assertThat (magentaBright "text") (isEqualTo "\x1b[95mtext\x1b[39m")
            )
            test (
                "cyanBright",
                fun _ -> assertThat (cyanBright "text") (isEqualTo "\x1b[96mtext\x1b[39m")
            )
            test (
                "whiteBright",
                fun _ -> assertThat (whiteBright "text") (isEqualTo "\x1b[97mtext\x1b[39m")
            )
        ]
    )

let private backgroundColours =
    testList (
        "background colours",
        [
            test (
                "bgBlack",
                fun _ -> assertThat (bgBlack "text") (isEqualTo "\x1b[40mtext\x1b[49m")
            )
            test ("bgRed", fun _ -> assertThat (bgRed "text") (isEqualTo "\x1b[41mtext\x1b[49m"))
            test (
                "bgGreen",
                fun _ -> assertThat (bgGreen "text") (isEqualTo "\x1b[42mtext\x1b[49m")
            )
            test (
                "bgYellow",
                fun _ -> assertThat (bgYellow "text") (isEqualTo "\x1b[43mtext\x1b[49m")
            )
            test ("bgBlue", fun _ -> assertThat (bgBlue "text") (isEqualTo "\x1b[44mtext\x1b[49m"))
            test (
                "bgMagenta",
                fun _ -> assertThat (bgMagenta "text") (isEqualTo "\x1b[45mtext\x1b[49m")
            )
            test ("bgCyan", fun _ -> assertThat (bgCyan "text") (isEqualTo "\x1b[46mtext\x1b[49m"))
            test (
                "bgWhite",
                fun _ -> assertThat (bgWhite "text") (isEqualTo "\x1b[47mtext\x1b[49m")
            )
            test ("empty string", fun _ -> assertThat (bgRed "") (isEqualTo "\x1b[41m\x1b[49m"))
        ]
    )

let private backgroundBrightColours =
    testList (
        "background bright colours",
        [
            test (
                "bgBlackBright",
                fun _ -> assertThat (bgBlackBright "text") (isEqualTo "\x1b[100mtext\x1b[49m")
            )
            test (
                "bgRedBright",
                fun _ -> assertThat (bgRedBright "text") (isEqualTo "\x1b[101mtext\x1b[49m")
            )
            test (
                "bgGreenBright",
                fun _ -> assertThat (bgGreenBright "text") (isEqualTo "\x1b[102mtext\x1b[49m")
            )
            test (
                "bgYellowBright",
                fun _ -> assertThat (bgYellowBright "text") (isEqualTo "\x1b[103mtext\x1b[49m")
            )
            test (
                "bgBlueBright",
                fun _ -> assertThat (bgBlueBright "text") (isEqualTo "\x1b[104mtext\x1b[49m")
            )
            test (
                "bgMagentaBright",
                fun _ -> assertThat (bgMagentaBright "text") (isEqualTo "\x1b[105mtext\x1b[49m")
            )
            test (
                "bgCyanBright",
                fun _ -> assertThat (bgCyanBright "text") (isEqualTo "\x1b[106mtext\x1b[49m")
            )
            test (
                "bgWhiteBright",
                fun _ -> assertThat (bgWhiteBright "text") (isEqualTo "\x1b[107mtext\x1b[49m")
            )
        ]
    )

let private textStyles =
    testList (
        "text styles",
        [
            test ("bold", fun _ -> assertThat (bold "text") (isEqualTo "\x1b[1mtext\x1b[22m"))
            test ("dim", fun _ -> assertThat (dim "text") (isEqualTo "\x1b[2mtext\x1b[22m"))
            test ("italic", fun _ -> assertThat (italic "text") (isEqualTo "\x1b[3mtext\x1b[23m"))
            test (
                "underline",
                fun _ -> assertThat (underline "text") (isEqualTo "\x1b[4mtext\x1b[24m")
            )
            test (
                "strikethrough",
                fun _ -> assertThat (strikethrough "text") (isEqualTo "\x1b[9mtext\x1b[29m")
            )
            test ("empty string", fun _ -> assertThat (bold "") (isEqualTo "\x1b[1m\x1b[22m"))
        ]
    )

let private nesting =
    testList (
        "nesting",
        [
            // Style wrapping colour: the style's targeted close leaves the fg/bg reset
            // to the inner colour function - outer style survives.
            test (
                "style wrapping fg color",
                fun _ ->
                    assertThat
                        (underline $"""Hello {green "world"} foo""")
                        (isEqualTo "\x1b[4mHello \x1b[32mworld\x1b[39m foo\x1b[24m")
            )

            test (
                "style wrapping bg color",
                fun _ ->
                    assertThat
                        (bold $"""normal {bgRed "alert"} normal""")
                        (isEqualTo "\x1b[1mnormal \x1b[41malert\x1b[49m normal\x1b[22m")
            )

            // Color wrapping style: the style closes itself; outer color closes after.
            test (
                "fg color wrapping style",
                fun _ ->
                    assertThat (red (bold "text")) (isEqualTo "\x1b[31m\x1b[1mtext\x1b[22m\x1b[39m")
            )

            test (
                "bg color wrapping style",
                fun _ ->
                    assertThat
                        (bgBlue (italic "text"))
                        (isEqualTo "\x1b[44m\x1b[3mtext\x1b[23m\x1b[49m")
            )

            // bg color wrapping fg color: fg and bg use separate close codes,
            // so the outer bg survives the inner fg reset.
            test (
                "bg color wrapping fg color",
                fun _ ->
                    assertThat
                        (bgBlue $"""normal {red "red text"} normal""")
                        (isEqualTo "\x1b[44mnormal \x1b[31mred text\x1b[39m normal\x1b[49m")
            )

            // Multiple styles: each style uses its own close code, so they don't interfere.
            test (
                "bold wrapping italic",
                fun _ ->
                    assertThat
                        (bold (italic "text"))
                        (isEqualTo "\x1b[1m\x1b[3mtext\x1b[23m\x1b[22m")
            )

            // fg-inside-fg: ANSI has no colour stack. The inner \x1b[39m resets the
            // foreground to default - the outer red is gone for the trailing text.
            // "should still be red" is NOT red.
            test (
                "fg color wrapping fg color: inner close resets outer",
                fun _ ->
                    assertThat
                        (red $"""red {green "green"} should still be red""")
                        (isEqualTo "\x1b[31mred \x1b[32mgreen\x1b[39m should still be red\x1b[39m")
            )

            // Same limitation for bg-inside-bg.
            test (
                "bg color wrapping bg color: inner close resets outer",
                fun _ ->
                    assertThat
                        (bgRed $"""red bg {bgGreen "green bg"} should still be red bg""")
                        (isEqualTo
                            "\x1b[41mred bg \x1b[42mgreen bg\x1b[49m should still be red bg\x1b[49m")
            )
        ]
    )

let private composition =
    testList (
        "composition",
        [
            test (
                "pipe: red then bold",
                fun _ ->
                    assertThat
                        ("text" |> red |> bold)
                        (isEqualTo "\x1b[1m\x1b[31mtext\x1b[39m\x1b[22m")
            )

            test (
                "pipe preserves content",
                fun _ ->
                    assertThat
                        ("hello" |> red |> bold)
                        (isEqualTo "\x1b[1m\x1b[31mhello\x1b[39m\x1b[22m")
            )

            test (
                "function composition",
                fun _ ->
                    assertThat
                        ((green >> bold >> italic) "hello")
                        (isEqualTo "\x1b[3m\x1b[1m\x1b[32mhello\x1b[39m\x1b[22m\x1b[23m")
            )
        ]
    )

[<EntryPoint>]
let main _ =

    runTests
        [
            testList (
                "Scriptorium.Ink",
                [
                    foregroundColours
                    foregroundBrightColours
                    backgroundColours
                    backgroundBrightColours
                    textStyles
                    nesting
                    composition
                ]
            )
        ]
