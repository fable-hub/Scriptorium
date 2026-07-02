module Repro
// Two fable-library-python gaps. Both COMPILE without a Fable error but fail at runtime.
[<EntryPoint>]
let main _ =
    // (1) System.Diagnostics.Stopwatch.StartNew -> generates `from fable_library.diagnostics
    //     import start_new`, but that name does not exist in fable-library-python -> ImportError
    //     at import time.
    let sw = System.Diagnostics.Stopwatch.StartNew()
    printfn "elapsed=%d" sw.ElapsedMilliseconds

    // (2) System.Text.Json is not supported: instead of a compile error, Fable emits a throwing
    //     stub `raise int32.ONE` (raise the integer 1), which Python rejects at runtime with
    //     "exceptions must derive from BaseException".
    let opts = System.Text.Json.JsonSerializerOptions(WriteIndented = true)
    printfn "%s" (System.Text.Json.JsonSerializer.Serialize({| A = 1 |}, opts))
    0
