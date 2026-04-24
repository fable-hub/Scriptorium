namespace Fable.Nib.Snapshot

open Fable.Quill
open Fable.Nib
open Fable.Nib.Assertion

[<AutoOpen>]
module SnapshotContextExtensions =

    type TestContext with

        /// <summary>
        /// Asserts that <c>value</c> matches a stored snapshot named after the current test,
        /// using the default serializer.
        /// </summary>
        /// <param name="config">Optional snapshot configuration. Uses <c>SnapshotConfig.Default</c> when omitted.</param>
        member ctx.snapshot(value: 'a, ?config: SnapshotConfig) =
            let name = ctx.Path |> String.concat " > "

            assertThat
                value
                (Snapshot.matchesWith (
                    Advanced.defaultSerialize,
                    name,
                    ?config = config,
                    ?testFilePath = Some ctx.FilePath
                ))

        /// <summary>
        /// Asserts that <c>value</c> matches a stored snapshot named after the current test,
        /// using a custom serializer.
        /// </summary>
        /// <param name="config">Optional snapshot configuration. Uses <c>SnapshotConfig.Default</c> when omitted.</param>
        member ctx.snapshotWith(serialize: 'a -> string, value: 'a, ?config: SnapshotConfig) =
            let name = ctx.Path |> String.concat " > "

            assertThat
                value
                (Snapshot.matchesWith (
                    serialize,
                    name,
                    ?config = config,
                    ?testFilePath = Some ctx.FilePath
                ))
