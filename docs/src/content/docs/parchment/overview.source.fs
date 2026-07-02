(***
---
title: Overview
description: Structured logging with severity levels and a pluggable sink system.
---
*)

(*** hide ***)

module Parchment.Overview

let itemCount = 0

(**
`Scriptorium.Parchment` is the structured logging layer for the Scriptorium family. It exposes severity-based logging methods, a composable sink system, and child loggers with automatic prefixes.

## Installation

```sh
dotnet add package Scriptorium.Parchment
```

## Usage

Create a logger with one or more sinks, then call the severity method you need:

*)

open Scriptorium.Parchment
open Scriptorium.Parchment.Sinks

let logger = Parchment.Create(Universal.console ())

logger.info "Server started on port 3000"
logger.warning "Deprecated API called"
logger.error "Connection failed"
logger.infof "Processing %d items" itemCount

// Child loggers prefix every message automatically
let db = logger.Child("db")
db.info "connected" // → [db] connected
db.error "query failed" // → [db] query failed

(**

Six severity levels are available, from highest to lowest priority:

```text
error > warning > info > verbose > debug > silly
```

Each has a plain and a formatted (`f`-suffixed) variant.

## Filtering

Set `logger.Level` to suppress messages below a given severity. The default is `Info`.

*)
logger.Level <- Severity.Warning

logger.error "logged" // ✓ Error ≤ Warning
logger.warning "logged" // ✓ Warning ≤ Warning
logger.info "not logged" // ✗ Info > Warning

(**

## Child loggers

`logger.Child(prefix)` creates a logger that prepends `[prefix]` to every message:

*)

let db1 = logger.Child("db")
let cache = logger.Child("cache")

db1.info "connected" // → [db] connected
cache.warning "cache miss" // → [cache] cache miss

(**

Children can be nested - prefixes are concatenated in order:

*)

let db2 = logger.Child("db")
let query = db.Child("query")

query.info "SELECT *" // → [db][query] SELECT *
query.error "timeout" // → [db][query] timeout

(**

## Custom sinks

A `Sink` is a `Severity -> string -> unit` function, so any output target works:

*)

let messages = ResizeArray<string>()

let collectingSink: Sink = fun severity msg -> messages.Add($"[{severity}] {msg}")

let myLogger = Parchment.Create(collectingSink)

(**

Sinks can be added or removed at runtime with `logger.Add(sink)` and `logger.Remove(sink)`.

*)
