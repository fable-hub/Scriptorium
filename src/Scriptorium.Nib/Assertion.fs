namespace Scriptorium.Nib

open Scriptorium.Ink

module Assertion =

    // ---------------------------------------------------------------------------
    // Core types
    // ---------------------------------------------------------------------------

    /// State threaded through an assertion chain.
    type AssertionState<'a> =
        {
            /// The current subject under test.
            Subject: 'a
            /// All accumulated failure messages (never thrown mid-chain).
            Errors: string list
            /// Active tag stack, e.g. ["foo"; "age"] → prefix "[foo.age]".
            Tags: string list
            /// When true (set by forceError) no further assertions in the chain run.
            Stopped: bool
        }

    /// <summary>An assertion that transforms the subject from <c>'a</c> to <c>'b</c>.</summary>
    /// <remarks>
    /// Use <c>Assertion&lt;'a&gt;</c> (= <c>Assertion&lt;'a,'a&gt;</c>) for assertions that keep the same subject type.
    /// Use <c>Assertion&lt;'a,'b&gt;</c> when <c>focus</c> changes the subject type mid-chain.
    /// </remarks>
    type Assertion<'a, 'b> = AssertionState<'a> -> AssertionState<'b>

    /// Shorthand for an assertion that does not change the subject type.
    type Assertion<'a> = Assertion<'a, 'a>

    // ---------------------------------------------------------------------------
    // Internal helpers
    // ---------------------------------------------------------------------------

    [<AutoOpen>]
    module private Internal =

        let tagPrefix (tags: string list) =
            match tags with
            | [] -> ""
            | ts -> "[" + System.String.Join(".", List.rev ts) + "] "

        let addError (msg: string) (state: AssertionState<'a>) : AssertionState<'a> =
            { state with
                Errors = (tagPrefix state.Tags + msg) :: state.Errors
            }

        let mapSubject (f: 'a -> 'b) (state: AssertionState<'a>) : AssertionState<'b> =
            {
                Subject = f state.Subject
                Errors = state.Errors
                Tags = state.Tags
                Stopped = state.Stopped
            }

    // ---------------------------------------------------------------------------
    // assertThat — runs the chain and throws on any failure
    // ---------------------------------------------------------------------------

    /// <summary>Runs an assertion chain against the given subject.</summary>
    /// <remarks>
    /// All failures in the chain are collected before throwing — a single run
    /// always reports every failing assertion, not just the first one.
    /// Throws <c>exn</c> with all error messages joined by newlines.
    /// </remarks>
    /// <param name="subject">The value under test.</param>
    /// <param name="assertion">The assertion chain to execute.</param>
    let assertThat (subject: 'a) (assertion: Assertion<'a, 'b>) : unit =
        let initial =
            {
                Subject = subject
                Errors = []
                Tags = []
                Stopped = false
            }

        let final = assertion initial

        match List.rev final.Errors with
        | [] -> ()
        | errors -> failwith (System.String.Join("\n", errors))

    // ---------------------------------------------------------------------------
    // Building block
    // ---------------------------------------------------------------------------

    /// <summary>The primitive building block for custom assertions.</summary>
    /// <remarks>
    /// - If the predicate returns <c>false</c> the error message is appended and the chain continues.
    /// - If the subject is stopped (via <c>forceError</c>) the assertion is skipped.
    /// </remarks>
    /// <param name="predicate">Returns <c>true</c> when the assertion passes.</param>
    /// <param name="message">Formats the error message from the subject when the assertion fails.</param>
    let assertion (predicate: 'a -> bool) (message: 'a -> string) : Assertion<'a> =
        fun state ->
            if state.Stopped then
                state
            elif predicate state.Subject then
                state
            else
                addError (message state.Subject) state

    // ---------------------------------------------------------------------------
    // Structural combinators
    // ---------------------------------------------------------------------------

    /// <summary>Changes the subject under test by applying a projection.</summary>
    /// <remarks>The original subject is discarded. Use <c>inside</c> to preserve it.</remarks>
    /// <param name="projection">Transforms the current subject into a new one.</param>
    let focus (projection: 'a -> 'b) : Assertion<'a, 'b> =
        fun state ->
            if state.Stopped then
                {
                    Subject = Unchecked.defaultof<'b>
                    Errors = state.Errors
                    Tags = state.Tags
                    Stopped = true
                }
            else
                mapSubject projection state

    /// <summary>
    /// Runs a sub-assertion on a projected value while preserving the original subject for
    /// assertions that follow in the outer chain.
    /// </summary>
    /// <param name="projection">Extracts the inner value to assert against.</param>
    /// <param name="inner">The assertion chain to run on the projected value.</param>
    let inside (projection: 'a -> 'b) (inner: Assertion<'b, 'c>) : Assertion<'a> =
        fun state ->
            if state.Stopped then
                state
            else
                let innerInitial = mapSubject projection state
                let innerFinal = inner innerInitial
                // Merge errors back; restore original subject.
                { state with
                    Errors = innerFinal.Errors
                }

    /// <summary>Marks all following assertions in the chain with a named prefix.</summary>
    /// <remarks>
    /// Tags nest: applying <c>tag "b"</c> inside a <c>tag "a"</c> scope produces <c>[a.b]</c>.
    /// The tag stays active for all subsequent assertions in the chain.
    /// </remarks>
    /// <param name="name">The tag label to prepend to failure messages.</param>
    let tag (name: string) : Assertion<'a> =
        fun state ->
            { state with
                Tags = name :: state.Tags
            }

    /// <summary>
    /// Wraps an assertion so that, if it fails, the rest of the chain is aborted.
    /// </summary>
    /// <remarks>
    /// Useful when subsequent assertions would be meaningless if a prior one fails
    /// (e.g. <c>Option.extracting</c> wraps <c>isSome</c> with <c>forceError</c> to avoid
    /// a <c>None.Value</c> crash in the follow-up <c>focus</c>).
    /// </remarks>
    /// <param name="inner">The assertion to force-stop the chain on failure.</param>
    let forceError (inner: Assertion<'a>) : Assertion<'a> =
        fun state ->
            let result = inner state

            if result.Errors.Length > state.Errors.Length then
                { result with
                    Stopped = true
                }
            else
                result

    /// <summary>
    /// Inverts an assertion — passes when <c>inner</c> fails, fails when <c>inner</c> passes.
    /// </summary>
    /// <remarks>
    /// Runs <c>inner</c> against a clean error slate. If <c>inner</c> accumulated no errors
    /// (it passed), <c>not'</c> injects a failure. If <c>inner</c> did fail, <c>not'</c>
    /// discards those errors and lets the outer chain continue cleanly.
    /// </remarks>
    let not' (inner: Assertion<'a>) : Assertion<'a> =
        fun state ->
            if state.Stopped then
                state
            else
                let innerResult =
                    inner
                        { state with
                            Errors = []
                        }

                if innerResult.Errors.IsEmpty then
                    addError "Expected assertion to fail but it passed" state
                else
                    state

    /// Alias for <c>not'</c> — use when the apostrophe is inconvenient in a pipeline.
    let not_ = not'

    // ---------------------------------------------------------------------------
    // Primitive assertions
    // ---------------------------------------------------------------------------

    /// Asserts the subject equals the expected value.
    let isEqualTo (expected: 'a) : Assertion<'a> =
        assertion
            (fun a -> a = expected)
            (fun a ->
                let diff = Diff.format (sprintf "%A" expected) (sprintf "%A" a)
                $"given %A{a} should be equal to %A{expected}\n\nDiff:{diff}\n"
            )

    /// Asserts the subject does not equal the expected value.
    let isNotEqualTo (expected: 'a) : Assertion<'a> =
        assertion (fun a -> a <> expected) (fun a -> $"given %A{a} should not be equal to %A{expected}")

    /// Asserts the subject is greater than the threshold.
    let isGreaterThan (threshold: 'a) : Assertion<'a> when 'a: comparison =
        assertion
            (fun a -> a > threshold)
            (fun a -> $"given %A{a} should be greater than %A{threshold}")

    /// Asserts the subject is greater than or equal to the threshold.
    let isGreaterOrEqual (threshold: 'a) : Assertion<'a> when 'a: comparison =
        assertion
            (fun a -> a >= threshold)
            (fun a -> $"given %A{a} should be greater than or equal to %A{threshold}")

    /// Asserts the subject is less than the threshold.
    let isLessThan (threshold: 'a) : Assertion<'a> when 'a: comparison =
        assertion (fun a -> a < threshold) (fun a -> $"given %A{a} should be less than %A{threshold}")

    /// Asserts the subject is less than or equal to the threshold.
    let isLessOrEqual (threshold: 'a) : Assertion<'a> when 'a: comparison =
        assertion
            (fun a -> a <= threshold)
            (fun a -> $"given %A{a} should be less than or equal to %A{threshold}")

    /// Asserts the subject satisfies an arbitrary predicate.
    let satisfy (predicate: 'a -> bool) : Assertion<'a> =
        assertion predicate (fun a -> $"given %A{a} should satisfy the predicate")

    // ---------------------------------------------------------------------------
    // Boolean assertions
    // ---------------------------------------------------------------------------

    /// Asserts the subject is true.
    let isTrue: Assertion<bool> = assertion id (fun _ -> "given false should be true")

    /// Asserts the subject is false.
    let isFalse: Assertion<bool> = assertion not (fun _ -> "given true should be false")

    /// Asserts the subject is not true.
    let isNotTrue: Assertion<bool> =
        assertion not (fun _ -> "given true should not be true")

    /// Asserts the subject is not false.
    let isNotFalse: Assertion<bool> =
        assertion id (fun _ -> "given false should not be false")

    // ---------------------------------------------------------------------------
    // Null assertions
    // ---------------------------------------------------------------------------

    /// Asserts the subject is null.
    let isNull<'a when 'a: null> : Assertion<'a> =
        assertion Operators.isNull (fun _ -> "given value should be null")

    /// Asserts the subject is not null.
    let isNotNull<'a when 'a: null> : Assertion<'a> =
        assertion (Operators.isNull >> not) (fun _ -> "given null should not be null")

    // ---------------------------------------------------------------------------
    // Thunk assertions
    // ---------------------------------------------------------------------------

    /// <summary>Asserts the thunk throws an exception.</summary>
    /// <remarks>Aborts the chain if no exception is raised (via <c>forceError</c>).</remarks>
    /// <returns>An <c>Assertion&lt;unit -&gt; unit, exn&gt;</c> that shifts the subject to the caught exception.</returns>
    let throws: Assertion<unit -> unit, exn> =
        let check: Assertion<unit -> unit> =
            assertion
                (fun f ->
                    try
                        f ()
                        false
                    with _ ->
                        true
                )
                (fun _ -> "given thunk should have thrown but did not")

        forceError check
        >> focus (fun f ->
            try
                f ()
                Unchecked.defaultof<exn>
            with ex ->
                ex
        )

    /// <summary>Asserts the thunk throws an exception whose message equals <c>expected</c>.</summary>
    /// <remarks>Aborts the chain if no exception is raised (via <c>forceError</c>).
    /// The subject shifts to the caught exception, allowing further chaining.</remarks>
    /// <param name="expected">The exact message the exception must carry.</param>
    let throwsWithMessage (expected: string) : Assertion<unit -> unit, exn> =
        throws
        >> forceError (
            assertion
                (fun ex -> ex.Message = expected)
                (fun ex ->
                    let diff =
                        Scriptorium.Nib.Diff.format (sprintf "%A" expected) (sprintf "%A" ex.Message)

                    $"expected exception message \"{expected}\" but got \"{ex.Message}\"\n\nDiff:{diff}\n"
                )
        )

    /// <summary>Asserts the thunk does not throw any exception.</summary>
    let doesNotThrow: Assertion<unit -> unit> =
        assertion
            (fun f ->
                try
                    f ()
                    true
                with _ ->
                    false
            )
            (fun _ -> "given thunk should not have thrown but did")

    // ---------------------------------------------------------------------------
    // Collection assertions  (all work on 'a list)
    // ---------------------------------------------------------------------------

    /// Asserts the list has exactly the given number of elements.
    let hasSize (expected: int) : Assertion<'a list> =
        assertion
            (fun xs -> xs.Length = expected)
            (fun xs -> $"given list of size {xs.Length} should have size {expected}")

    /// Asserts the list is empty.
    let isEmpty<'a> : Assertion<'a list> =
        assertion
            (fun xs -> xs.IsEmpty)
            (fun xs -> $"given list of size {xs.Length} should be empty")

    /// Asserts the list is not empty.
    let isNotEmpty<'a> : Assertion<'a list> =
        assertion (fun xs -> not xs.IsEmpty) (fun _ -> "given list should not be empty")

    /// Asserts the list contains the given element.
    let contain (expected: 'a) : Assertion<'a list> =
        assertion
            (fun xs -> List.contains expected xs)
            (fun _ -> $"given list should contain %A{expected}")

    /// Asserts the list does not contain the given element.
    let notContain (expected: 'a) : Assertion<'a list> =
        assertion
            (fun xs -> not (List.contains expected xs))
            (fun _ -> $"given list should not contain %A{expected}")

    /// Asserts the list starts with the given prefix.
    let startWith (prefix: 'a list) : Assertion<'a list> =
        assertion
            (fun xs ->
                let rec loop xs prefix =
                    match xs, prefix with
                    | _, [] -> true
                    | x :: xs', p :: ps' when x = p -> loop xs' ps'
                    | _ -> false

                loop xs prefix
            )
            (fun xs ->
                let diff =
                    Scriptorium.Nib.Diff.format
                        (sprintf "%A" prefix)
                        (sprintf "%A" (xs |> List.truncate prefix.Length))

                $"given %A{xs} should start with %A{prefix}\n\nDiff:{diff}\n"
            )

    /// Asserts the list has the same length as another list.
    let haveSameSizeAs (other: 'a list) : Assertion<'a list> =
        assertion
            (fun xs -> xs.Length = other.Length)
            (fun xs ->
                $"given list of size {xs.Length} should have same size as list of size {other.Length}"
            )

    /// Asserts the list contains the same elements in the same order.
    let beSameAs (expected: 'a list) : Assertion<'a list> =
        assertion
            (fun xs -> xs = expected)
            (fun xs ->
                let diff = Scriptorium.Nib.Diff.format (sprintf "%A" expected) (sprintf "%A" xs)
                $"given %A{xs} should be same as %A{expected}\n\nDiff:{diff}\n"
            )

    /// Asserts the list contains the same elements in any order.
    let haveSameElements (expected: 'a list) : Assertion<'a list> =
        assertion
            (fun xs -> List.sort xs = List.sort expected)
            (fun xs ->
                let diff =
                    Scriptorium.Nib.Diff.format
                        (sprintf "%A" (List.sort expected))
                        (sprintf "%A" (List.sort xs))

                $"given %A{xs} should have same elements as %A{expected}\n\nDiff:{diff}\n"
            )

    // ---------------------------------------------------------------------------
    // Option assertions
    // ---------------------------------------------------------------------------

    [<RequireQualifiedAccess>]
    module Option =

        /// Asserts the option is None.
        let isNone<'a> : Assertion<'a option> =
            assertion (fun x -> Option.isNone x) (fun x -> $"given %A{x} should be None")

        /// Asserts the option is Some.
        let isSome<'a> : Assertion<'a option> =
            assertion (fun x -> Option.isSome x) (fun _ -> "given None should be Some")

        /// <summary>Asserts the option is Some and extracts the inner value as the new subject.</summary>
        /// <remarks>Aborts the chain if the value is None (via <c>forceError</c>).</remarks>
        let value<'a> : Assertion<'a option, 'a> = forceError isSome >> focus Option.get

    // ---------------------------------------------------------------------------
    // Result assertions
    // ---------------------------------------------------------------------------

    [<RequireQualifiedAccess>]
    module Result =

        /// Asserts the result is Ok.
        let isOk<'a, 'e> : Assertion<Result<'a, 'e>> =
            assertion
                (fun r ->
                    match r with
                    | Ok _ -> true
                    | Error _ -> false
                )
                (fun r -> $"given %A{r} should be Ok")

        /// Asserts the result is Error.
        let isError<'a, 'e> : Assertion<Result<'a, 'e>> =
            assertion
                (fun r ->
                    match r with
                    | Ok _ -> false
                    | Error _ -> true
                )
                (fun r -> $"given %A{r} should be Error")

        /// <summary>Asserts the result is Ok and extracts the inner value as the new subject.</summary>
        /// <remarks>Aborts the chain if the result is Error (via <c>forceError</c>).</remarks>
        let okValue<'a, 'e> : Assertion<Result<'a, 'e>, 'a> =
            forceError isOk
            >> focus (fun r ->
                match r with
                | Ok v -> v
                | Error _ -> Unchecked.defaultof<'a>
            )

        /// <summary>Asserts the result is Error and extracts the inner error as the new subject.</summary>
        /// <remarks>Aborts the chain if the result is Ok (via <c>forceError</c>).</remarks>
        let errorValue<'a, 'e> : Assertion<Result<'a, 'e>, 'e> =
            forceError isError
            >> focus (fun r ->
                match r with
                | Error e -> e
                | Ok _ -> Unchecked.defaultof<'e>
            )

    // ---------------------------------------------------------------------------
    // Discriminated union assertions
    // ---------------------------------------------------------------------------

    [<RequireQualifiedAccess>]
    module DU =

        /// <summary>
        /// Builds a reusable assertion that checks a DU case and extracts its payload as the new subject.
        /// </summary>
        /// <remarks>
        /// - Aborts the chain if <c>tryExtract</c> returns <c>None</c> (via <c>forceError</c>).
        /// - Pass the case label as a plain string for readable failure messages.
        /// </remarks>
        /// <param name="label">The case name used in the failure message, e.g. <c>"Circle"</c>.</param>
        /// <param name="tryExtract">
        /// Returns <c>Some payload</c> when the subject matches the expected case,
        /// <c>None</c> otherwise.
        /// </param>
        /// <returns>
        /// An <c>Assertion&lt;'a,'b&gt;</c> that passes when the case matches and shifts the subject
        /// to the extracted <c>'b</c> payload.
        /// </returns>
        let ofCase (label: string) (tryExtract: 'a -> 'b option) : Assertion<'a, 'b> =
            forceError (
                assertion
                    (fun a -> (tryExtract a).IsSome)
                    (fun a -> $"expected {label} but got {a}")
            )
            >> focus (fun a -> (tryExtract a).Value)
