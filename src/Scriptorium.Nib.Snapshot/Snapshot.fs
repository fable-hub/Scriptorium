namespace Scriptorium.Nib.Snapshot

open System.Runtime.CompilerServices

open Scriptorium.Nib.Assertion
open Scriptorium.Nib.Snapshot
open Scriptorium.Nib

/// <summary>Snapshot assertions for Scriptorium.Nib.</summary>
type Snapshot =

    /// <summary>
    /// Asserts that the subject matches a stored snapshot, using a custom serializer.
    /// </summary>
    /// <remarks>
    /// On first run or when the configured update environment variable is set,
    /// the snapshot is written and the test passes. On subsequent runs, the subject
    /// is serialized and compared against the stored snapshot.
    /// </remarks>
    /// <param name="serialize">Transforms the subject into a string representation.</param>
    /// <param name="snapshotName">Unique name for this snapshot within the test file.</param>
    /// <param name="config">Optional snapshot configuration. Uses <c>SnapshotConfig.Default</c> when omitted.</param>
    static member matchesWith
        (
            serialize: 'a -> string,
            snapshotName: string,
            ?config: SnapshotConfig,
            [<CallerFilePath>] ?testFilePath: string
        )
        : Assertion<'a>
        =
        let cfg = defaultArg config SnapshotConfig.Default
        let testFile = defaultArg testFilePath ""
        let update = Advanced.shouldUpdateSnapshots cfg

        assertion
            (fun value ->
                let passed, _ = Advanced.runSnapshot cfg snapshotName testFile update serialize value
                passed
            )
            (fun value ->
                let snapPath = Advanced.snapshotFilePath cfg testFile

                let snapshots =
                    match Advanced.readFile snapPath with
                    | Some content -> Advanced.parseSnapshotFile content
                    | None -> Map.empty

                match Map.tryFind snapshotName snapshots with
                | Some expected ->
                    let actual = serialize value
                    let diff = Diff.format expected actual
                    $"Snapshot '{snapshotName}' mismatch\n\nDiff:{diff}\n"
                | None ->
                    $"Snapshot '{snapshotName}' does not exist. Run with {cfg.UpdateEnvironmentVariable}=1 to create it."
            )

    /// <summary>
    /// Asserts that the subject matches a stored snapshot, using the default serializer.
    /// </summary>
    /// <remarks>
    /// On JavaScript, uses <c>JSON.stringify</c> with 2-space indentation.
    /// On .NET, uses <c>System.Text.Json</c> with indentation.
    /// </remarks>
    /// <param name="snapshotName">Unique name for this snapshot within the test file.</param>
    /// <param name="config">Optional snapshot configuration. Uses <c>SnapshotConfig.Default</c> when omitted.</param>
    static member matches
        (snapshotName: string, ?config: SnapshotConfig, [<CallerFilePath>] ?testFilePath: string)
        : Assertion<'a>
        =
        Snapshot.matchesWith (
            Advanced.defaultSerialize,
            snapshotName,
            ?config = config,
            ?testFilePath = testFilePath
        )
