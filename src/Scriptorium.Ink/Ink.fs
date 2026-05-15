module Scriptorium.Ink

open Fable.Core
open Fable.Core.JsInterop

// ---------------------------------------------------------------------------
// Color support detection
// ---------------------------------------------------------------------------

/// Colors are disabled when the NO_COLOR environment variable is set,
/// following the no-color.org convention.
let private colorsEnabled: bool =
#if FABLE_COMPILER_JAVASCRIPT
    emitJsExpr
        ()
        "!((typeof process !== 'undefined' && process.env['NO_COLOR'] != null) || typeof NO_COLOR !== 'undefined')"
#endif

#if !FABLE_COMPILER_JAVASCRIPT
    System.Environment.GetEnvironmentVariable("NO_COLOR") |> isNull
#endif

// ---------------------------------------------------------------------------
// Raw codes
// ---------------------------------------------------------------------------

[<Literal>]
let private ESC = "\x1b["

// Foreground colors close with \x1b[39m (default fg) rather than \x1b[0m
// (reset all) so that outer styles applied around an inner color are not
// cancelled by the inner close code.
let private wrapFgColor (openCode: string) (s: string) =
    if colorsEnabled then
        $"%s{ESC}%s{openCode}m%s{s}%s{ESC}39m"
    else
        s

// Background colors close with \x1b[49m (default bg) for the same reason.
let private wrapBgColor (openCode: string) (s: string) =
    if colorsEnabled then
        $"%s{ESC}%s{openCode}m%s{s}%s{ESC}49m"
    else
        s

// Each style uses its own targeted close code so it only turns itself off.
let private wrapStyle (openCode: string) (closeCode: string) (s: string) =
    $"%s{ESC}%s{openCode}m%s{s}%s{ESC}%s{closeCode}m"

// ---------------------------------------------------------------------------
// Colours — foreground
// ---------------------------------------------------------------------------

let black (s: string) = wrapFgColor "30" s
let red (s: string) = wrapFgColor "31" s
let green (s: string) = wrapFgColor "32" s
let yellow (s: string) = wrapFgColor "33" s
let blue (s: string) = wrapFgColor "34" s
let magenta (s: string) = wrapFgColor "35" s
let cyan (s: string) = wrapFgColor "36" s
let white (s: string) = wrapFgColor "37" s

// Bright variants

let blackBright (s: string) = wrapFgColor "90" s
let redBright (s: string) = wrapFgColor "91" s
let greenBright (s: string) = wrapFgColor "92" s
let yellowBright (s: string) = wrapFgColor "93" s
let blueBright (s: string) = wrapFgColor "94" s
let magentaBright (s: string) = wrapFgColor "95" s
let cyanBright (s: string) = wrapFgColor "96" s
let whiteBright (s: string) = wrapFgColor "97" s

// ---------------------------------------------------------------------------
// Colours — background
// ---------------------------------------------------------------------------

let bgBlack (s: string) = wrapBgColor "40" s
let bgRed (s: string) = wrapBgColor "41" s
let bgGreen (s: string) = wrapBgColor "42" s
let bgYellow (s: string) = wrapBgColor "43" s
let bgBlue (s: string) = wrapBgColor "44" s
let bgMagenta (s: string) = wrapBgColor "45" s
let bgCyan (s: string) = wrapBgColor "46" s
let bgWhite (s: string) = wrapBgColor "47" s

// Bright variants

let bgBlackBright (s: string) = wrapBgColor "100" s
let bgRedBright (s: string) = wrapBgColor "101" s
let bgGreenBright (s: string) = wrapBgColor "102" s
let bgYellowBright (s: string) = wrapBgColor "103" s
let bgBlueBright (s: string) = wrapBgColor "104" s
let bgMagentaBright (s: string) = wrapBgColor "105" s
let bgCyanBright (s: string) = wrapBgColor "106" s
let bgWhiteBright (s: string) = wrapBgColor "107" s

// ---------------------------------------------------------------------------
// Styles
// ---------------------------------------------------------------------------

let bold (s: string) = wrapStyle "1" "22" s
let dim (s: string) = wrapStyle "2" "22" s
let italic (s: string) = wrapStyle "3" "23" s
let underline (s: string) = wrapStyle "4" "24" s
let strikethrough (s: string) = wrapStyle "9" "29" s

(**

This is a test test

**strong**

*)
