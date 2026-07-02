# Python: fable-library gaps that compile but fail at runtime

These APIs produce **no Fable compile error** but fail when the generated Python runs. That makes
them easy to ship by accident — worth either a compile-time error or a library implementation.

## Reproduce

```bash
dotnet tool restore
dotnet fable --lang python -o out --noCache          # compiles with NO error
python -m venv .venv && .venv/bin/pip install fable-library
.venv/bin/python out/repro.py                         # fails at runtime
```

## 1. `System.Diagnostics.Stopwatch.StartNew` — missing library symbol

The call compiles to `from fable_library.diagnostics import start_new`, but
`fable_library.diagnostics` only exposes a `StopWatch` class (with `start()` / `stop()` /
`elapsed_milliseconds`) — there is no `start_new`. Result:

```
ImportError: cannot import name 'start_new' from 'fable_library.diagnostics'
```

## 2. `System.Text.Json` — silent throwing stub

`System.Text.Json` is unsupported, but instead of a Fable compile error the backend emits a
throwing stub. The generated `default_serialize`-style function becomes:

```python
def f(...):
    options: Any
    raise int32.ONE     # raise the integer 1
    raise int32.ONE
```

At runtime Python rejects it:

```
TypeError: exceptions must derive from BaseException
```

A compile-time "not supported" error (as other unsupported APIs get) would be far less surprising.

## Related (not in this minimal repro)

Reflection-based generic derivation (`FSharp.Reflection` walking records/unions/tuples to build
values, as in `Scriptorium.Hedgehog`'s `Derive`) does not work under fable-library-python: the
hand-written property API runs, but every auto-derivation test fails. Reproduce by running the
Scriptorium.Hedgehog test suite under `--lang python` (13 of 21 tests fail). This is why
`Scriptorium.Hedgehog` is excluded from the Python test matrix.

## Workarounds used in Scriptorium

`Stopwatch` -> `time.perf_counter()` via `emitPyExpr`; `System.Text.Json` -> the `json` module via
`emitPyExpr` with a `default` handler that unwraps Fable numeric wrappers and record objects. See
`src/Scriptorium.Quill/Prelude.fs` and `src/Scriptorium.Nib.Snapshot/Advanced.fs`.
