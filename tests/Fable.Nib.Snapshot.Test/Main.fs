module Fable.Nib.Snapshot.Test.Main

open Fable.Nib
open Fable.Nib.Assertion
open Fable.Nib.Snapshot

open type Fable.Quill.Test
open type Fable.Quill.Runner

open Fable.Quill

[<EntryPoint>]
let main _ =

    let tests =
        testSequenced (
            "Fable.Nib.Snapshot",
            [

                // TEMP: deliberate mismatch to inspect output
                // test("diff output demo", fun _ ->
                //     let data = {| Name = "Alicd"; Age = 30 |}
                //     assertThat data (Snapshot.matches "first-run")
                // )

                test (
                    "matches existing snapshot via TestContext",
                    fun (t: TestContext) ->
                        let data =
                            {|
                                Name = "Alice"
                                Age = 30
                            |}

                        t.snapshot data
                )

                test (
                    "matches existing snapshot",
                    fun _ ->
                        let data =
                            {|
                                Name = "Alice"
                                Age = 30
                            |}

                        assertThat data (Snapshot.matches "first-run")
                )

                test (
                    "custom serializer works",
                    fun _ ->
                        let serialize x = $"custom:{x}"
                        assertThat 42 (Snapshot.matchesWith (serialize, "custom-serializer"))
                )

                test (
                    "mismatch fails with diff",
                    fun _ ->
                        assertThat "expected value" (Snapshot.matches "mismatch-test")

                        assertThat
                            (fun () -> assertThat "actual value" (Snapshot.matches "mismatch-test"))
                            throws
                )

                test (
                    "AutoCreate=false fails on missing snapshot",
                    fun _ ->
                        let cfg =
                            { SnapshotConfig.Default with
                                AutoCreate = false
                            }

                        assertThat
                            (fun () -> assertThat 99 (Snapshot.matches ("never-created", cfg)))
                            (throwsWithMessage
                                "Snapshot 'never-created' does not exist. Run with UPDATE_SNAPSHOTS=1 to create it.")
                )

                test (
                    "custom DirectoryName stores snapshot in configured directory",
                    fun _ ->
                        let cfg =
                            { SnapshotConfig.Default with
                                DirectoryName = "__custom__"
                            }

                        let data =
                            {|
                                Name = "Bob"
                                Age = 25
                            |}

                        assertThat data (Snapshot.matches ("custom-dir", cfg))
                )

                test (
                    "custom Extension stores snapshot with configured extension",
                    fun _ ->
                        let cfg =
                            { SnapshotConfig.Default with
                                Extension = ".json"
                            }

                        assertThat
                            42
                            (Snapshot.matchesWith (
                                (fun _ -> """{ "value": 42 }"""),
                                "custom-ext",
                                cfg
                            ))
                )

            ]
        )

    runTests tests
