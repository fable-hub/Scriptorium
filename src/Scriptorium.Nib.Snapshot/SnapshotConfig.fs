namespace Scriptorium.Nib

/// <summary>Configuration for snapshot assertions.</summary>
type SnapshotConfig =
    {
        /// <summary>Name of the directory that holds snapshot files, relative to the test file.</summary>
        /// <remarks>Default: <c>"__snapshots__"</c></remarks>
        DirectoryName: string

        /// <summary>File extension for snapshot files (including the dot).</summary>
        /// <remarks>Default: <c>".snap"</c></remarks>
        Extension: string

        /// <summary>Environment variable that triggers snapshot updates when set.</summary>
        /// <remarks>Default: <c>"UPDATE_SNAPSHOTS"</c></remarks>
        UpdateEnvironmentVariable: string

        /// <summary>
        /// When <c>true</c>, missing snapshots are created automatically and the test passes.
        /// When <c>false</c>, missing snapshots cause the test to fail.
        /// </summary>
        /// <remarks>
        /// Setting this to <c>false</c> in CI ensures every snapshot is committed
        /// and no new snapshots are silently generated on the build agent.
        /// Default: <c>true</c>
        /// </remarks>
        AutoCreate: bool
    }

    /// <summary>Default snapshot configuration.</summary>
    static member Default =
        {
            DirectoryName = "__snapshots__"
            Extension = ".snap"
            UpdateEnvironmentVariable = "UPDATE_SNAPSHOTS"
            AutoCreate = true
        }
