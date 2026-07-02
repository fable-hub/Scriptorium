namespace Scriptorium.Nib.Snapshot

open Scriptorium.Nib
open Fable.Core.JsInterop

module Advanced =

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    let private fs: obj = importAll "fs"
    let private path: obj = importAll "path"
#endif

    let shouldUpdateSnapshots (config: SnapshotConfig) : bool =
        let envVar = config.UpdateEnvironmentVariable
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        emitJsExpr envVar "!!process.env[$0]"
#endif
#if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyExpr envVar "bool(__import__('os').environ.get($0))"
#endif
#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON)
        System.Environment.GetEnvironmentVariable(envVar)
        |> System.String.IsNullOrEmpty
        |> not
#endif

    let snapshotFilePath (config: SnapshotConfig) (testFilePath: string) : string =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        let dir = path?dirname (testFilePath)
        let baseName = path?basename (testFilePath)
        path?join (dir, config.DirectoryName, baseName + config.Extension)
#endif
#if FABLE_COMPILER_PYTHON
        let dir: string = Fable.Core.PyInterop.emitPyExpr testFilePath "__import__('os').path.dirname($0)"
        let baseName: string = Fable.Core.PyInterop.emitPyExpr testFilePath "__import__('os').path.basename($0)"
        dir + "/" + config.DirectoryName + "/" + baseName + config.Extension
#endif
#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON)
        let dir = System.IO.Path.GetDirectoryName(testFilePath)
        let fileName = System.IO.Path.GetFileName(testFilePath) + config.Extension
        System.IO.Path.Combine(dir, config.DirectoryName, fileName)
#endif

    let readFile (pathStr: string) : string option =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        try
            fs?readFileSync (pathStr, "utf8") |> Some
        with _ ->
            None
#else
        try
            System.IO.File.ReadAllText(pathStr) |> Some
        with _ ->
            None
#endif

    let writeFile (pathStr: string) (content: string) : unit =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        let dir = path?dirname (pathStr)

        if not (fs?existsSync (dir)) then
            fs?mkdirSync (
                dir,
                {|
                    recursive = true
                |}
            )

        fs?writeFileSync (pathStr, content, "utf8")
#endif
#if FABLE_COMPILER_PYTHON
        let dir: string = Fable.Core.PyInterop.emitPyExpr pathStr "__import__('os').path.dirname($0)"
        Fable.Core.PyInterop.emitPyStatement dir "__import__('os').makedirs($0, exist_ok=True)"
        System.IO.File.WriteAllText(pathStr, content)
#endif
#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON)
        let dir = System.IO.Path.GetDirectoryName(pathStr)

        if not (System.IO.Directory.Exists(dir)) then
            System.IO.Directory.CreateDirectory(dir) |> ignore

        System.IO.File.WriteAllText(pathStr, content)
#endif

    let defaultSerialize (value: 'a) : string =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        emitJsExpr value "JSON.stringify($0, null, 2)"
#endif
#if FABLE_COMPILER_PYTHON
        // System.Text.Json is not supported by fable-library-python, so serialize with the json
        // module. The `default` handler unwraps Fable's numeric wrappers (int32, ...) to plain
        // numbers and record objects (whether __dict__- or __slots__-based) to their fields, so the
        // output matches the JS `JSON.stringify` snapshots.
        Fable.Core.PyInterop.emitPyExpr
            value
            "__import__('json').dumps($0, indent=2, sort_keys=True, default=lambda o: int(o) if hasattr(o, '__int__') and not isinstance(o, (bool, int)) else (vars(o) if hasattr(o, '__dict__') else {s: getattr(o, s) for s in getattr(type(o), '__slots__', ())}))"
#endif
#if !(FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || FABLE_COMPILER_PYTHON)
        let options = System.Text.Json.JsonSerializerOptions(WriteIndented = true)
        System.Text.Json.JsonSerializer.Serialize(value, options)
#endif

    let parseSnapshotFile (content: string) : Map<string, string> =
        let rec parse lines snapshots name contentLines inContent =
            match lines with
            | [] -> snapshots
            | line :: rest ->
                if line = "=== end-snapshot ===" && inContent then
                    parse rest (Map.add name (String.concat "\n" (List.rev contentLines)) snapshots) "" [] false
                elif line.Length > 7 && line.StartsWith("=== ") && line.EndsWith(" ===") then
                    parse rest snapshots (line.Substring(4, line.Length - 8)) [] true
                elif inContent then
                    parse rest snapshots name (line :: contentLines) true
                else
                    parse rest snapshots name contentLines inContent

        parse (content.Split('\n') |> Array.toList) Map.empty "" [] false

    let formatSnapshotFile (snapshots: Map<string, string>) : string =
        snapshots
        |> Map.toSeq
        |> Seq.collect (fun (name, content) ->
            seq {
                $"=== {name} ==="

                if not (System.String.IsNullOrEmpty(content)) then
                    content

                "=== end-snapshot ==="
                ""
            }
        )
        |> String.concat "\n"

    let runSnapshot
        (config: SnapshotConfig)
        (snapshotName: string)
        (testFilePath: string)
        (update: bool)
        (serialize: 'a -> string)
        (value: 'a)
        : bool * string option
        =
        let snapPath = snapshotFilePath config testFilePath
        let actual = serialize value

        let snapshots =
            match readFile snapPath with
            | Some content -> parseSnapshotFile content
            | None -> Map.empty

        if update then
            let newSnapshots = Map.add snapshotName actual snapshots
            writeFile snapPath (formatSnapshotFile newSnapshots)
            true, None
        else
            match Map.tryFind snapshotName snapshots with
            | Some expected ->
                if actual = expected then
                    true, None
                else
                    false, Some expected
            | None ->
                if config.AutoCreate && not Scriptorium.Quill.Prelude.isCI then
                    let newSnapshots = Map.add snapshotName actual snapshots
                    writeFile snapPath (formatSnapshotFile newSnapshots)
                    true, None
                else
                    false, None
