namespace Fable.Parchment

/// Log levels in order from most important to least important.
[<RequireQualifiedAccess>]
type Severity =
    | Error
    | Warning
    | Info
    | Verbose
    | Debug
    | Silly

type Sink = Severity -> string -> unit

type Logger(sinks: Sink list, ?prefix: string) =
    let prefix = defaultArg prefix ""
    let mutable _sinks = sinks

    member val Level = Severity.Info with get, set

    // Routes an already-formatted message to sinks, bypassing prefix application.
    // Used by child loggers to avoid double-prefixing.
    member private this.Route(severity: Severity, msg: string) =
        if severity <= this.Level then
            _sinks |> List.iter (fun sink -> sink severity msg)

    member this.Log(severity: Severity, msg: string) =
        let formatted =
            if prefix = "" then
                msg
            else
                $"{prefix} {msg}"

        this.Route(severity, formatted)

    member _.Add(sink: Sink) = _sinks <- _sinks @ [ sink ]

    member _.Remove(sink: Sink) =
        _sinks <-
            _sinks
            |> List.filter (fun s -> not (LanguagePrimitives.PhysicalEquality s sink))

    member this.Child(name: string) =
        let childPrefix =
            if prefix = "" then
                $"[{name}]"
            else
                $"{prefix}[{name}]"

        Logger(
            [
                fun severity msg -> this.Route(severity, msg)
            ],
            childPrefix
        )

    member this.error(msg: string) = this.Log(Severity.Error, msg)

    member this.errorf(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Error, s)) fmt

    member this.warning(msg: string) = this.Log(Severity.Warning, msg)

    member this.warningf(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Warning, s)) fmt

    member this.info(msg: string) = this.Log(Severity.Info, msg)

    member this.infof(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Info, s)) fmt

    member this.verbose(msg: string) = this.Log(Severity.Verbose, msg)

    member this.verbosef(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Verbose, s)) fmt

    member this.debug(msg: string) = this.Log(Severity.Debug, msg)

    member this.debugf(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Debug, s)) fmt

    member this.silly(msg: string) = this.Log(Severity.Silly, msg)

    member this.sillyf(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Silly, s)) fmt

type Parchment =

    /// <summary>Create a <see cref="Logger" /> that fans out to all provided sinks.</summary>
    static member Create([<System.ParamArray>] sinks: Sink[]) : Logger = Logger(Array.toList sinks)
