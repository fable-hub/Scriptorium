module MainTests

open Fable.Nib.Assertion
open Fable.Parchment

open type Fable.Quill.Test
open type Fable.Quill.Runner

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/// Build a capture sink and return it together with a function to read what was logged.
let private captureSink () =
    let captured = ResizeArray<Severity * string>()
    let sink : Sink = fun sev msg -> captured.Add(sev, msg)
    sink, (fun () -> List.ofSeq captured)

[<EntryPoint>]
let main _ =

    let tests =
        testList (
            "Parchment",
            [

                // ------------------------------------------------------------------
                // Severity routing
                // ------------------------------------------------------------------

                testList (
                    "severity routing",
                    [

                        test (
                            "error routes to Error severity",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.error "boom"
                                assertThat (read ()) (isEqualTo [ Severity.Error, "boom" ])
                        )

                        test (
                            "warning routes to Warning severity",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.warning "careful"
                                assertThat (read ()) (isEqualTo [ Severity.Warning, "careful" ])
                        )

                        test (
                            "info routes to Info severity",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.info "hello"
                                assertThat (read ()) (isEqualTo [ Severity.Info, "hello" ])
                        )

                        test (
                            "verbose routes to Verbose severity",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Silly
                                logger.verbose "details"
                                assertThat (read ()) (isEqualTo [ Severity.Verbose, "details" ])
                        )

                        test (
                            "debug routes to Debug severity",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Silly
                                logger.debug "breakpoint"
                                assertThat (read ()) (isEqualTo [ Severity.Debug, "breakpoint" ])
                        )

                        test (
                            "silly routes to Silly severity",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Silly
                                logger.silly "trace"
                                assertThat (read ()) (isEqualTo [ Severity.Silly, "trace" ])
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Printf formatting
                // ------------------------------------------------------------------

                testList (
                    "printf formatting",
                    [

                        test (
                            "infof formats string and int",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.infof "context: %s - %i" "my-data" 42
                                assertThat (read ()) (isEqualTo [ Severity.Info, "context: my-data - 42" ])
                        )

                        test (
                            "warningf formats correctly",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.warningf "value is %i" 99
                                assertThat (read ()) (isEqualTo [ Severity.Warning, "value is 99" ])
                        )

                        test (
                            "errorf formats correctly",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.errorf "failed with %s" "timeout"
                                assertThat (read ()) (isEqualTo [ Severity.Error, "failed with timeout" ])
                        )

                        test (
                            "verbosef formats correctly",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Silly
                                logger.verbosef "step %i of %i" 3 10
                                assertThat (read ()) (isEqualTo [ Severity.Verbose, "step 3 of 10" ])
                        )

                        test (
                            "debugf formats correctly",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Silly
                                logger.debugf "value=%b" true
                                assertThat (read ()) (isEqualTo [ Severity.Debug, "value=true" ])
                        )

                        test (
                            "sillyf formats correctly",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Silly
                                logger.sillyf "x=%f" 3.14
                                assertThat (read ()) (isEqualTo [ Severity.Silly, "x=3.140000" ])
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Multiple sinks
                // ------------------------------------------------------------------

                testList (
                    "multiple sinks",
                    [

                        test (
                            "all sinks receive the message",
                            fun _ ->
                                let sink1, read1 = captureSink ()
                                let sink2, read2 = captureSink ()
                                let logger = Parchment.Create(sink1, sink2)
                                logger.info "broadcast"
                                assertThat (read1 ()) (isEqualTo [ Severity.Info, "broadcast" ])
                                assertThat (read2 ()) (isEqualTo [ Severity.Info, "broadcast" ])
                        )

                        test (
                            "zero sinks is a no-op",
                            fun _ ->
                                let logger = Parchment.Create()
                                logger.info "ignored"
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Add / Remove
                // ------------------------------------------------------------------

                // ------------------------------------------------------------------
                // Level filtering
                // ------------------------------------------------------------------

                testList (
                    "Level",
                    [

                        test (
                            "default level is Info — Verbose/Debug/Silly are filtered",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.verbose "filtered"
                                logger.debug "filtered"
                                logger.silly "filtered"
                                logger.info "passes"
                                assertThat (read ()) (isEqualTo [ Severity.Info, "passes" ])
                        )

                        test (
                            "messages at the set level pass",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Info
                                logger.info "at level"
                                assertThat (read ()) (isEqualTo [ Severity.Info, "at level" ])
                        )

                        test (
                            "messages more important than the level pass",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Info
                                logger.error "critical"
                                logger.warning "careful"
                                assertThat (read ()) (isEqualTo [ Severity.Error, "critical"; Severity.Warning, "careful" ])
                        )

                        test (
                            "messages less important than the level are filtered",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Info
                                logger.verbose "filtered"
                                logger.debug "filtered"
                                logger.silly "filtered"
                                assertThat (read ()) (isEqualTo [])
                        )

                        test (
                            "level can be changed at runtime",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Error
                                logger.info "filtered"
                                logger.Level <- Severity.Info
                                logger.info "passes"
                                assertThat (read ()) (isEqualTo [ Severity.Info, "passes" ])
                        )

                        test (
                            "child level filters independently from parent",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Level <- Severity.Info
                                let child = logger.Child("db")
                                child.Level <- Severity.Error
                                child.info "filtered by child"
                                child.error "passes both"
                                assertThat (read ()) (isEqualTo [ Severity.Error, "[db] passes both" ])
                        )

                    ]
                )

                testList (
                    "Add / Remove",
                    [

                        test (
                            "Add makes new sink receive subsequent messages",
                            fun _ ->
                                let sink1, read1 = captureSink ()
                                let sink2, read2 = captureSink ()
                                let logger = Parchment.Create(sink1)
                                logger.info "before"
                                logger.Add(sink2)
                                logger.info "after"
                                assertThat (read1 ()) (isEqualTo [ Severity.Info, "before"; Severity.Info, "after" ])
                                assertThat (read2 ()) (isEqualTo [ Severity.Info, "after" ])
                        )

                        test (
                            "Remove stops sink from receiving messages",
                            fun _ ->
                                let sink1, read1 = captureSink ()
                                let sink2, read2 = captureSink ()
                                let logger = Parchment.Create(sink1, sink2)
                                logger.Remove(sink2)
                                logger.info "after"
                                assertThat (read1 ()) (isEqualTo [ Severity.Info, "after" ])
                                assertThat (read2 ()) (isEqualTo [])
                        )

                        test (
                            "Remove non-existent sink is a no-op",
                            fun _ ->
                                let sink, read = captureSink ()
                                let other, _ = captureSink ()
                                let logger = Parchment.Create(sink)
                                logger.Remove(other)
                                logger.info "still here"
                                assertThat (read ()) (isEqualTo [ Severity.Info, "still here" ])
                        )

                    ]
                )

                // ------------------------------------------------------------------
                // Child
                // ------------------------------------------------------------------

                testList (
                    "Child",
                    [

                        test (
                            "Child prefixes message with [prefix]",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                let child = logger.Child("db")
                                child.info "connected"
                                assertThat (read ()) (isEqualTo [ Severity.Info, "[db] connected" ])
                        )

                        test (
                            "nested Child accumulates prefixes",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                let db = logger.Child("db")
                                let query = db.Child("query")
                                query.info "SELECT *"
                                assertThat (read ()) (isEqualTo [ Severity.Info, "[db][query] SELECT *" ])
                        )

                        test (
                            "Child does not affect parent messages",
                            fun _ ->
                                let sink, read = captureSink ()
                                let logger = Parchment.Create(sink)
                                let child = logger.Child("db")
                                logger.info "root"
                                child.info "child"
                                assertThat
                                    (read ())
                                    (isEqualTo [ Severity.Info, "root"; Severity.Info, "[db] child" ])
                        )

                        test (
                            "sink added to parent after Child creation is visible to Child",
                            fun _ ->
                                let sink1, read1 = captureSink ()
                                let sink2, read2 = captureSink ()
                                let logger = Parchment.Create(sink1)
                                let child = logger.Child("db")
                                logger.Add(sink2)
                                child.info "hello"
                                assertThat (read1 ()) (isEqualTo [ Severity.Info, "[db] hello" ])
                                assertThat (read2 ()) (isEqualTo [ Severity.Info, "[db] hello" ])
                        )

                    ]
                )

            ]
        )

    runTests tests
