namespace Scriptorium.Parchment

/// <summary>
/// Log levels ordered from most important to least important.
/// A logger's <see cref="Logger.Level" /> acts as a ceiling - messages with
/// a severity numerically greater than the level are silently dropped.
/// </summary>
[<RequireQualifiedAccess>]
type Severity =
    | Error
    | Warning
    | Info
    | Verbose
    | Debug
    | Silly

/// <summary>
/// A function that receives a <see cref="Severity" /> and a formatted message
/// and performs the actual output (console, file, network, etc.).
/// </summary>
type Sink = Severity -> string -> unit

/// <summary>
/// A logger that routes messages to one or more <see cref="Sink" /> functions.
/// Messages are prefixed and filtered by <see cref="Level" /> before being sent to sinks.
/// </summary>
/// <remarks>
/// Create instances via <see cref="Parchment.Create" /> rather than calling the constructor directly.
/// </remarks>
type Logger(sinks: Sink list, ?prefix: string) =
    let prefix = defaultArg prefix ""
    let mutable _sinks = sinks

    /// <summary>
    /// The maximum severity this logger will emit. Defaults to <see cref="Severity.Info" />.
    /// Messages with a higher (less important) severity are silently discarded.
    /// </summary>
    member val Level = Severity.Info with get, set

    // Routes an already-formatted message to sinks, bypassing prefix application.
    // Used by child loggers to avoid double-prefixing.
    member private this.Route(severity: Severity, msg: string) =
        if severity <= this.Level then
            _sinks |> List.iter (fun sink -> sink severity msg)

    /// <summary>
    /// Logs a message at the given severity, applying the logger's prefix if one is set.
    /// </summary>
    /// <param name="severity">The importance level of the message.</param>
    /// <param name="msg">The message text.</param>
    member this.Log(severity: Severity, msg: string) =
        let formatted =
            if prefix = "" then
                msg
            else
                $"{prefix} {msg}"

        this.Route(severity, formatted)

    /// <summary>
    /// Adds a new sink to the logger's output list.
    /// </summary>
    member _.Add(sink: Sink) = _sinks <- _sinks @ [ sink ]

    /// <summary>
    /// Removes a sink by physical (reference) equality.
    /// </summary>
    member _.Remove(sink: Sink) =
        _sinks <-
            _sinks
            |> List.filter (fun s -> not (LanguagePrimitives.PhysicalEquality s sink))

    /// <summary>
    /// Creates a child logger with an additional name prefix.
    /// Messages logged through the child are routed to this logger's sinks,
    /// so the child inherits all current and future sinks.
    /// </summary>
    /// <param name="name">The child name, appended in square brackets to the prefix chain.</param>
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

    /// <summary>
    /// Logs a message at <see cref="Severity.Error" />.
    /// </summary>
    member this.error(msg: string) = this.Log(Severity.Error, msg)

    /// <summary>
    /// Logs a formatted message at <see cref="Severity.Error" />.
    /// </summary>
    member this.errorf(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Error, s)) fmt

    /// <summary>
    /// Logs a message at <see cref="Severity.Warning" />.
    /// </summary>
    member this.warning(msg: string) = this.Log(Severity.Warning, msg)

    /// <summary>
    /// Logs a formatted message at <see cref="Severity.Warning" />.
    /// </summary>
    member this.warningf(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Warning, s)) fmt

    /// <summary>
    /// Logs a message at <see cref="Severity.Info" />.
    /// </summary>
    member this.info(msg: string) = this.Log(Severity.Info, msg)

    /// <summary>
    /// Logs a formatted message at <see cref="Severity.Info" />.
    /// </summary>
    member this.infof(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Info, s)) fmt

    /// <summary>
    /// Logs a message at <see cref="Severity.Verbose" />.
    /// </summary>
    member this.verbose(msg: string) = this.Log(Severity.Verbose, msg)

    /// <summary>
    /// Logs a formatted message at <see cref="Severity.Verbose" />.
    /// </summary>
    member this.verbosef(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Verbose, s)) fmt

    /// <summary>
    /// Logs a message at <see cref="Severity.Debug" />.
    /// </summary>
    member this.debug(msg: string) = this.Log(Severity.Debug, msg)

    /// <summary>
    /// Logs a formatted message at <see cref="Severity.Debug" />.
    /// </summary>
    member this.debugf(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Debug, s)) fmt

    /// <summary>
    /// Logs a message at <see cref="Severity.Silly" />.
    /// </summary>
    member this.silly(msg: string) = this.Log(Severity.Silly, msg)

    /// <summary>
    /// Logs a formatted message at <see cref="Severity.Silly" />.
    /// </summary>
    member this.sillyf(fmt) =
        Printf.kprintf (fun s -> this.Log(Severity.Silly, s)) fmt

/// <summary>
/// Static factory functions for creating <see cref="Logger" /> instances.
/// </summary>
type Parchment =

    /// <summary>
    /// Create a <see cref="Logger" /> that fans out to all provided sinks.
    /// </summary>
    /// <param name="sinks">One or more <see cref="Sink" /> functions to receive log output.</param>
    static member Create([<System.ParamArray>] sinks: Sink[]) : Logger = Logger(Array.toList sinks)
