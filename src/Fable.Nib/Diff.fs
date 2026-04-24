namespace Fable.Nib

open Fable.Ink

module Diff =

    // ---------------------------------------------------------------------------
    // Generic LCS-based diff engine
    //
    // Fable does not support Array2D, so the DP table is a flat 1D array
    // with manual (i, j) → i*(n+1)+j indexing.
    // ---------------------------------------------------------------------------

    [<NoComparison; NoEquality>]
    type private Op<'a> =
        | Keep of 'a
        | OnlyLeft of 'a // only in expected
        | OnlyRight of 'a // only in actual

    let private computeOps<'a when 'a: equality> (left: 'a[]) (right: 'a[]) : Op<'a> list =
        let m = left.Length
        let n = right.Length
        let cols = n + 1
        let dp = Array.create ((m + 1) * cols) 0

        for i in 1..m do
            for j in 1..n do
                if left.[i - 1] = right.[j - 1] then
                    dp.[i * cols + j] <- dp.[(i - 1) * cols + (j - 1)] + 1
                else
                    dp.[i * cols + j] <- max dp.[(i - 1) * cols + j] dp.[i * cols + (j - 1)]

        let result = ResizeArray()
        let mutable i = m
        let mutable j = n

        while i > 0 || j > 0 do
            if i > 0 && j > 0 && left.[i - 1] = right.[j - 1] then
                result.Add(Keep left.[i - 1])
                i <- i - 1
                j <- j - 1
            elif j > 0 && (i = 0 || dp.[i * cols + (j - 1)] >= dp.[(i - 1) * cols + j]) then
                result.Add(OnlyRight right.[j - 1])
                j <- j - 1
            else
                result.Add(OnlyLeft left.[i - 1])
                i <- i - 1

        result |> Seq.rev |> Seq.toList

    // ---------------------------------------------------------------------------
    // Character-level diff: accumulates consecutive same-type ops into strings
    // ---------------------------------------------------------------------------

    let private charDiff (expected: string) (actual: string) : Op<string> list =
        computeOps (expected.ToCharArray()) (actual.ToCharArray())
        |> List.fold
            (fun acc op ->
                let toStr =
                    function
                    | Keep c -> Keep(string c)
                    | OnlyLeft c -> OnlyLeft(string c)
                    | OnlyRight c -> OnlyRight(string c)

                match acc, op with
                | Keep a :: rest, Keep b -> Keep(a + string b) :: rest
                | OnlyLeft a :: rest, OnlyLeft b -> OnlyLeft(a + string b) :: rest
                | OnlyRight a :: rest, OnlyRight b -> OnlyRight(a + string b) :: rest
                | _ -> toStr op :: acc
            )
            []
        |> List.rev

    // ---------------------------------------------------------------------------
    // Rendering
    //
    // Two levels of visual emphasis, inspired by Meld:
    //   - Line level  : entire +/- lines are colored green/red
    //   - Char level  : differing chars within a paired line are bold + bright
    // ---------------------------------------------------------------------------

    // Expected side of a paired char diff:
    //   context chars  → plain (no distraction)
    //   expected-only  → bright green foreground
    let private renderExpected (ops: Op<string> list) : string =
        ops
        |> List.map (
            function
            | Keep s -> s
            | OnlyLeft s -> green s
            | OnlyRight _ -> ""
        )
        |> String.concat ""

    // Actual side of a paired char diff:
    //   context chars  → plain
    //   actual-only    → bright red foreground
    let private renderActual (ops: Op<string> list) : string =
        ops
        |> List.map (
            function
            | Keep s -> s
            | OnlyLeft _ -> ""
            | OnlyRight s -> red s
        )
        |> String.concat ""

    // Render one hunk of changed lines.
    // Pairs up expected-only and actual-only lines for char-level highlighting.
    // Paired lines: green/red prefix + plain context + bg-highlighted diff chars.
    // Surplus lines: prefix + whole content highlighted in bg color.
    let private renderHunk (leftLines: string list) (rightLines: string list) : string list =
        let pairs = min leftLines.Length rightLines.Length

        let paired =
            [
                for i in 0 .. pairs - 1 do
                    let ops = charDiff leftLines.[i] rightLines.[i]
                    yield green "+ " + renderExpected ops
                    yield red "- " + renderActual ops
            ]

        let extraLeft = leftLines |> List.skip pairs |> List.map (fun l -> green ("+ " + l))

        let extraRight = rightLines |> List.skip pairs |> List.map (fun l -> red ("- " + l))

        paired @ extraLeft @ extraRight

    // Renders a line-level diff op list into a single string with newlines.
    let private renderLineDiff (ops: Op<string> list) : string =
        let lines = ResizeArray<string>()
        let arr = ops |> List.toArray
        let mutable i = 0

        while i < arr.Length do
            match arr.[i] with
            | Keep line ->
                lines.Add(dim ("  " + line))
                i <- i + 1
            | _ ->
                let leftLines = ResizeArray<string>()
                let rightLines = ResizeArray<string>()

                while i < arr.Length
                      && (
                          match arr.[i] with
                          | Keep _ -> false
                          | _ -> true
                      ) do
                    match arr.[i] with
                    | OnlyLeft l -> leftLines.Add(l)
                    | OnlyRight r -> rightLines.Add(r)
                    | Keep _ -> ()

                    i <- i + 1

                renderHunk (leftLines |> Seq.toList) (rightLines |> Seq.toList)
                |> List.iter lines.Add

        lines |> String.concat "\n"

    /// Maximum string length for which char-level diff is computed.
    /// Beyond this falls back to plain two-line display.
    let private maxDiffLength = 500

    /// <summary>
    /// Formats a colored diff between <c>expectedStr</c> and <c>actualStr</c>.
    /// </summary>
    /// <remarks>
    /// Two modes:
    ///
    /// - Single-line, within length limit → char-level diff: context in green/red,
    ///   differing chars in bold bright color.
    /// - Single-line, too long → plain two-line display.
    /// - Multi-line → line-level LCS diff with char-level highlights on paired changed lines.
    ///   Unchanged lines are dimmed for context.
    /// </remarks>
    let format (expectedStr: string) (actualStr: string) : string =
        let isMultiline = expectedStr.Contains("\n") || actualStr.Contains("\n")

        if isMultiline then
            let ops = computeOps (expectedStr.Split('\n')) (actualStr.Split('\n'))
            "\n" + renderLineDiff ops

        elif expectedStr.Length > maxDiffLength || actualStr.Length > maxDiffLength then
            let expLabel = green "Expected"
            let actLabel = red "Actual"
            $"\n  {expLabel}: {expectedStr}\n  {actLabel}: {actualStr}"

        else
            let ops = charDiff expectedStr actualStr
            let expLabel = green "+"
            let actLabel = red "-"
            $"\n  {expLabel} {renderExpected ops}\n  {actLabel} {renderActual ops}"
