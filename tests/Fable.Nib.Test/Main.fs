module Fable.Nib.Test.Main

open Fable.Nib.Assertion
open type Fable.Quill.Test
open type Fable.Quill.Runner

// ---------------------------------------------------------------------------
// Sample types
// ---------------------------------------------------------------------------

type User =
    {
        Name: string
        Age: int
    }

type Page =
    {
        Link: string
    }

type Post =
    {
        Title: string
        Content: string
        Author: User
        Page: Page
    }

[<RequireQualifiedAccess>]
type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float
    | Triangle of base': float * height: float

// ---------------------------------------------------------------------------
// Reusable DU assertions
// ---------------------------------------------------------------------------

let isCircle: Assertion<Shape, float> =
    DU.ofCase
        "Circle"
        (fun s ->
            match s with
            | Shape.Circle r -> Some r
            | _ -> None
        )

let isRectangle: Assertion<Shape, float * float> =
    DU.ofCase
        "Rectangle"
        (fun s ->
            match s with
            | Shape.Rectangle(w, h) -> Some(w, h)
            | _ -> None
        )

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

[<EntryPoint>]
let main _ =

    let tests =
        testList (
            "Assert",
            [

                // ------------------------------------------------------------------
                // Primitives
                // ------------------------------------------------------------------

                testList (
                    "primitives",
                    [

                        test (
                            "isEqualTo passes for equal values",
                            fun _ -> assertThat 42 (isEqualTo 42)
                        )

                        test (
                            "isNotEqualTo passes for different values",
                            fun _ -> assertThat "hello" (isNotEqualTo "world")
                        )

                        test ("isGreaterThan passes", fun _ -> assertThat 42 (isGreaterThan 40))

                        test ("isLessThan passes", fun _ -> assertThat 42 (isLessThan 50))

                        test (
                            "chained primitive assertions pass",
                            fun _ ->
                                assertThat 42 (isEqualTo 42 >> isGreaterThan 40 >> isLessThan 50)
                        )

                        test (
                            "isGreaterOrEqual passes on boundary",
                            fun _ -> assertThat 18 (isGreaterOrEqual 18)
                        )

                        test (
                            "isLessOrEqual passes on boundary",
                            fun _ -> assertThat 10 (isLessOrEqual 10)
                        )

                        test (
                            "satisfy passes when predicate is true",
                            fun _ -> assertThat 42 (satisfy (fun n -> n % 2 = 0))
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Boolean
                // ------------------------------------------------------------------

                testList (
                    "boolean",
                    [

                        test ("isTrue passes for true", fun _ -> assertThat true isTrue)

                        test ("isFalse passes for false", fun _ -> assertThat false isFalse)

                        test ("isNotFalse passes for true", fun _ -> assertThat true isNotFalse)

                        test ("isNotTrue passes for false", fun _ -> assertThat false isNotTrue)

                        test (
                            "isTrue fails for false with expected message",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat false isTrue)
                                    (throwsWithMessage "given false should be true")
                        )

                        test (
                            "isFalse fails for true with expected message",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat true isFalse)
                                    (throwsWithMessage "given true should be false")
                        )

                        test (
                            "isNotTrue fails for true with expected message",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat true isNotTrue)
                                    (throwsWithMessage "given true should not be true")
                        )

                        test (
                            "isNotFalse fails for false with expected message",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat false isNotFalse)
                                    (throwsWithMessage "given false should not be false")
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Null
                // ------------------------------------------------------------------

                testList (
                    "null",
                    [

                        test (
                            "isNull passes for null string",
                            fun _ -> assertThat (null: string) isNull
                        )

                        test (
                            "isNotNull passes for non-null string",
                            fun _ -> assertThat "hello" isNotNull
                        )

                        test (
                            "isNull fails for non-null value",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat "hello" isNull)
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m -> m.Contains "should be null"))
                        )

                        test (
                            "isNotNull fails for null value",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat (null: string) isNotNull)
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m -> m.Contains "should not be null"))
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Thunk
                // ------------------------------------------------------------------

                testList (
                    "thunk",
                    [

                        test (
                            "doesNotThrow passes for non-throwing thunk",
                            fun _ -> assertThat (fun _ -> ()) doesNotThrow
                        )

                        test (
                            "doesNotThrow passes for computation that returns a value",
                            fun _ -> assertThat (fun _ -> ignore (1 + 1)) doesNotThrow
                        )

                        test (
                            "throws passes and shifts subject to exception",
                            fun _ ->
                                assertThat
                                    (fun _ -> failwith "boom")
                                    (throws >> focus _.Message >> isEqualTo "boom")
                        )

                        test (
                            "throws fails when thunk does not throw",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat (fun _ -> ()) throws)
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m -> m.Contains "should have thrown"))
                        )

                        test (
                            "doesNotThrow fails when thunk throws",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat (fun _ -> failwith "oops") doesNotThrow)
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m -> m.Contains "should not have thrown"))
                        )

                        test (
                            "throwsWithMessage passes when message matches",
                            fun _ ->
                                assertThat (fun _ -> failwith "boom") (throwsWithMessage "boom")
                        )

                        test (
                            "throwsWithMessage allows further chaining after message check",
                            fun _ ->
                                assertThat
                                    (fun _ -> failwith "boom")
                                    (throwsWithMessage "boom" >> focus _.Message >> isEqualTo "boom")
                        )

                        test (
                            "throwsWithMessage fails when thunk does not throw",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat (fun _ -> ()) (throwsWithMessage "boom"))
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m -> m.Contains "should have thrown"))
                        )

                        test (
                            "throwsWithMessage fails when message is wrong",
                            fun _ ->
                                assertThat
                                    (fun _ ->
                                        assertThat
                                            (fun _ -> failwith "oops")
                                            (throwsWithMessage "boom")
                                    )
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m ->
                                         m.Contains "\"boom\"" && m.Contains "\"oops\""
                                     ))
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Collections
                // ------------------------------------------------------------------

                testList (
                    "collections",
                    [

                        test (
                            "hasSize passes for correct length",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (hasSize 3)
                        )

                        test (
                            "isEmpty passes for empty list",
                            fun _ -> assertThat ([]: int list) isEmpty
                        )

                        test (
                            "isNotEmpty passes for non-empty list",
                            fun _ -> assertThat [ 1 ] isNotEmpty
                        )

                        test (
                            "contain passes when element present",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (contain 3)
                        )

                        test (
                            "notContain passes when element absent",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (notContain 99)
                        )

                        test (
                            "startWith passes for correct prefix",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (startWith
                                        [
                                            1
                                            2
                                        ])
                        )

                        test (
                            "haveSameSizeAs passes for equal-length lists",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (haveSameSizeAs
                                        [
                                            4
                                            5
                                            6
                                        ])
                        )

                        test (
                            "beSameAs passes for identical lists",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (beSameAs
                                        [
                                            1
                                            2
                                            3
                                        ])
                        )

                        test (
                            "haveSameElements passes regardless of order",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (haveSameElements
                                        [
                                            3
                                            1
                                            2
                                        ])
                        )

                        test (
                            "chained collection assertions pass",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (hasSize 3
                                     >> isNotEmpty
                                     >> startWith
                                         [
                                             1
                                             2
                                         ]
                                     >> contain 3
                                     >> haveSameElements
                                         [
                                             3
                                             1
                                             2
                                         ])
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Option
                // ------------------------------------------------------------------

                testList (
                    "option",
                    [

                        test (
                            "Option.isSome passes for Some",
                            fun _ -> assertThat (Some 42) Option.isSome
                        )

                        test (
                            "Option.isNone passes for None",
                            fun _ -> assertThat (None: int option) Option.isNone
                        )

                        test (
                            "Option.value unwraps and allows further assertions",
                            fun _ -> assertThat (Some 42) (Option.value >> isEqualTo 42)
                        )

                        test (
                            "Option.value stops chain on None",
                            fun _ ->
                                let mutable ran = false

                                assertThat
                                    (fun _ ->
                                        assertThat
                                            (None: int option)
                                            (Option.value
                                             >> satisfy (fun _ ->
                                                 ran <- true
                                                 true
                                             ))
                                    )
                                    throws

                                assertThat ran isFalse
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Result
                // ------------------------------------------------------------------

                testList (
                    "result",
                    [

                        test (
                            "Result.isOk passes for Ok",
                            fun _ -> assertThat (Ok 10: Result<int, string>) Result.isOk
                        )

                        test (
                            "Result.isError passes for Error",
                            fun _ -> assertThat (Error "oops": Result<int, string>) Result.isError
                        )

                        test (
                            "Result.okValue unwraps Ok and allows further assertions",
                            fun _ ->
                                assertThat
                                    (Ok 10: Result<int, string>)
                                    (Result.okValue >> isEqualTo 10)
                        )

                        test (
                            "Result.errorValue unwraps Error and allows further assertions",
                            fun _ ->
                                assertThat
                                    (Error "oops": Result<int, string>)
                                    (Result.errorValue >> isEqualTo "oops")
                        )

                        test (
                            "Result.okValue stops chain on Error",
                            fun _ ->
                                let mutable ran = false

                                assertThat
                                    (fun _ ->
                                        assertThat
                                            (Error "oops": Result<int, string>)
                                            (Result.okValue
                                             >> satisfy (fun _ ->
                                                 ran <- true
                                                 true
                                             ))
                                    )
                                    throws

                                assertThat ran isFalse
                        )

                        test (
                            "Result.errorValue stops chain on Ok",
                            fun _ ->
                                let mutable ran = false

                                assertThat
                                    (fun _ ->
                                        assertThat
                                            (Ok 1: Result<int, string>)
                                            (Result.errorValue
                                             >> satisfy (fun _ ->
                                                 ran <- true
                                                 true
                                             ))
                                    )
                                    throws

                                assertThat ran isFalse
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Discriminated Union
                // ------------------------------------------------------------------

                testList (
                    "DU",
                    [

                        test (
                            "DU.ofCase passes and extracts payload for correct case",
                            fun _ ->
                                assertThat
                                    (Shape.Circle 5.0)
                                    (isCircle >> isGreaterThan 0.0 >> isLessOrEqual 10.0)
                        )

                        test (
                            "DU.ofCase extracts tuple payload and allows inside chaining",
                            fun _ ->
                                assertThat
                                    (Shape.Rectangle(4.0, 6.0))
                                    (isRectangle
                                     >> inside fst (tag "width" >> isGreaterThan 0.0)
                                     >> inside
                                         snd
                                         (tag "height" >> isGreaterThan 0.0 >> isLessOrEqual 10.0))
                        )

                        test (
                            "DU.ofCase fails with readable message for wrong case",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat (Shape.Rectangle(3.0, 4.0)) isCircle)
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m -> m.Contains "expected Circle"))
                        )

                        test (
                            "DU.ofCase stops chain on wrong case",
                            fun _ ->
                                let mutable ran = false

                                assertThat
                                    (fun _ ->
                                        assertThat
                                            (Shape.Rectangle(1.0, 2.0))
                                            (isCircle
                                             >> satisfy (fun _ ->
                                                 ran <- true
                                                 true
                                             ))
                                    )
                                    throws

                                assertThat ran isFalse
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Short-circuit (forceError)
                // ------------------------------------------------------------------

                testList (
                    "short-circuit",
                    [

                        test (
                            "forceError stops chain when assertion fails",
                            fun _ ->
                                let mutable ran = false

                                assertThat
                                    (fun _ ->
                                        assertThat
                                            0
                                            (forceError (isGreaterThan 10)
                                             >> satisfy (fun _ ->
                                                 ran <- true
                                                 true
                                             ))
                                    )
                                    throws

                                assertThat ran isFalse
                        )

                        test (
                            "forceError does not stop chain when assertion passes",
                            fun _ ->
                                let mutable ran = false

                                assertThat
                                    42
                                    (forceError (isGreaterThan 0)
                                     >> satisfy (fun _ ->
                                         ran <- true
                                         true
                                     ))

                                assertThat ran isTrue
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Record / nested inside + tag
                // ------------------------------------------------------------------

                testList (
                    "record",
                    [

                        test (
                            "inside + tag on a simple record passes",
                            fun _ ->
                                assertThat
                                    {
                                        Name = "alice"
                                        Age = 25
                                    }
                                    (inside
                                        _.Age
                                        (tag "age" >> isGreaterOrEqual 18 >> isLessThan 65)
                                     >> inside _.Name (tag "name" >> isNotEqualTo ""))
                        )

                        test (
                            "nested inside + tag on Post produces tagged failure messages",
                            fun _ ->
                                assertThat
                                    (fun _ ->
                                        assertThat
                                            {
                                                Title = ""
                                                Content = ""
                                                Author =
                                                    {
                                                        Name = ""
                                                        Age = 15
                                                    }
                                                Page =
                                                    {
                                                        Link = "http://x"
                                                    }
                                            }
                                            (tag "post"
                                             >> inside _.Title (tag "title" >> isNotEqualTo "")
                                             >> inside _.Content (tag "content" >> isNotEqualTo "")
                                             >> inside
                                                 (fun p -> p.Author)
                                                 (tag "author"
                                                  >> inside
                                                      (fun u -> u.Name)
                                                      (tag "name" >> isNotEqualTo "")
                                                  >> inside
                                                      (fun u -> u.Age)
                                                      (tag "age" >> isGreaterOrEqual 18))
                                             >> inside
                                                 (fun p -> p.Page)
                                                 (tag "page"
                                                  >> inside
                                                      (fun pg -> pg.Link)
                                                      (tag "link"
                                                       >> satisfy (fun l ->
                                                           l.StartsWith "https://"
                                                       ))))
                                    )
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m ->
                                         m.Contains "[post.title]"
                                         && m.Contains "[post.content]"
                                         && m.Contains "[post.author.name]"
                                         && m.Contains "[post.author.age]"
                                         && m.Contains "[post.page.link]"
                                     ))
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Direct equality on complex types
                // ------------------------------------------------------------------

                testList (
                    "direct equality",
                    [

                        test (
                            "isEqualTo passes for equal records",
                            fun _ ->
                                assertThat
                                    {
                                        Name = "alice"
                                        Age = 25
                                    }
                                    (isEqualTo
                                        {
                                            Name = "alice"
                                            Age = 25
                                        })
                        )

                        test (
                            "isNotEqualTo passes for different records",
                            fun _ ->
                                assertThat
                                    {
                                        Name = "alice"
                                        Age = 25
                                    }
                                    (isNotEqualTo
                                        {
                                            Name = "bob"
                                            Age = 30
                                        })
                        )

                        test (
                            "isEqualTo passes for equal lists",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (isEqualTo
                                        [
                                            1
                                            2
                                            3
                                        ])
                        )

                        test (
                            "isNotEqualTo passes for different lists",
                            fun _ ->
                                assertThat
                                    [
                                        1
                                        2
                                        3
                                    ]
                                    (isNotEqualTo
                                        [
                                            1
                                            2
                                            4
                                        ])
                        )

                        test (
                            "isEqualTo passes for equal DU values",
                            fun _ -> assertThat (Shape.Circle 5.0) (isEqualTo (Shape.Circle 5.0))
                        )

                        test (
                            "isNotEqualTo passes for different DU cases",
                            fun _ ->
                                assertThat
                                    (Shape.Circle 5.0)
                                    (isNotEqualTo (Shape.Rectangle(4.0, 6.0)))
                        )

                    ]
                )

                testList (
                    "not'",
                    [

                        test (
                            "passes when the inner assertion fails",
                            fun _ -> assertThat 1 (not' (isEqualTo 2))
                        )

                        test (
                            "fails when the inner assertion passes",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat 1 (not' (isEqualTo 1)))
                                    (throwsWithMessage "Expected assertion to fail but it passed")
                        )

                        test (
                            "not_ is an alias for not'",
                            fun _ -> assertThat 1 (not_ (isEqualTo 2))
                        )

                        test (
                            "can be chained with other assertions",
                            fun _ -> assertThat 1 (isGreaterThan 0 >> not' (isEqualTo 2))
                        )

                        test (
                            "collects outer errors independently of inner result",
                            fun _ ->
                                assertThat
                                    (fun _ -> assertThat 1 (not' (isEqualTo 1) >> isEqualTo 2))
                                    (throws
                                     >> focus _.Message
                                     >> satisfy (fun m ->
                                         m.Contains "Expected assertion to fail but it passed"
                                         && m.Contains "given 1 should be equal to 2"
                                     ))
                        )

                    ]
                )

            ]
        )

    runTests tests
