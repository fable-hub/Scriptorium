namespace Scriptorium.Hedgehog

open System.Runtime.CompilerServices
open Hedgehog
open Hedgehog.FSharp
open Scriptorium.Quill

module private Internal =

    let inline mk (name: string) (filePath: string option) (lineNumber: int option) (body: TestContext -> unit) : TestCase =
        TestCase.SyncTest
            {
                Name = name
                Body = body
                Mark = TestMark.Normal
                FilePath = defaultArg filePath ""
                LineNumber = defaultArg lineNumber 0
                Configurer = id
            }

/// <summary>Bridges <a href="https://hedgehogqa.github.io/fsharp-hedgehog/">Hedgehog</a> properties
/// into the Scriptorium.Quill test runner.</summary>
/// <remarks>
/// Write properties with Hedgehog's manual API (<c>Gen</c>, <c>Range</c>, the <c>property { }</c>
/// computation expression) and run them as ordinary Quill tests. A falsification fails the test with
/// Hedgehog's rendered, minimal counterexample (including the <c>Property.recheck</c> data).
///
/// Hedgehog's own TypeShape-based automatic generation (<c>Gen.auto</c>) is intentionally not
/// surfaced here - it is not supported under Fable. For portable auto-derivation use this package's
/// <c>Derive.gen</c> / <c>Derive.genWith</c> (reflection-based, works on .NET and Fable).
/// </remarks>
type Test =

    /// Runs a boolean Hedgehog property as a Quill test (100 tests by default). This is the common
    /// case - a `property { ... return &lt;bool&gt; }` block has type <c>Property&lt;bool&gt;</c>.
    static member testProperty
        (
            name: string,
            prop: Property<bool>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Internal.mk name filePath lineNumber (fun _ -> Property.checkBool prop)

    /// Runs a unit Hedgehog property as a Quill test (100 tests by default). Use this for properties
    /// whose body asserts via exceptions and ends with <c>return ()</c>.
    static member testProperty
        (
            name: string,
            prop: Property<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Internal.mk name filePath lineNumber (fun _ -> Property.check prop)

    /// <summary>Generates values and checks them with a throwing assertion.</summary>
    /// <remarks>
    /// This is the bridge to Scriptorium.Nib: pass <c>fun x -&gt; assertThat x (...)</c> as the
    /// assertion. When the assertion throws, Hedgehog catches it, treats the case as a failure, and
    /// shrinks to a minimal counterexample - the assertion's message is shown in the report.
    /// </remarks>
    static member testProperty
        (
            name: string,
            gen: Gen<'a>,
            assertion: 'a -> unit,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        let prop =
            property {
                let! value = gen
                return (assertion value
                        true)
            }

        Internal.mk name filePath lineNumber (fun _ -> Property.checkBool prop)

    /// Runs a boolean Hedgehog property with an explicit Hedgehog <c>PropertyConfig</c> (e.g. a custom
    /// test count via <c>PropertyConfig.defaults |&gt; PropertyConfig.withTests 500&lt;tests&gt;</c>).
    static member testPropertyWith
        (
            name: string,
            config: IPropertyConfig,
            prop: Property<bool>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Internal.mk name filePath lineNumber (fun _ -> Property.checkBoolWith config prop)

    /// Runs a unit Hedgehog property with an explicit Hedgehog <c>PropertyConfig</c>.
    static member testPropertyWith
        (
            name: string,
            config: IPropertyConfig,
            prop: Property<unit>,
            [<CallerFilePath>] ?filePath: string,
            [<CallerLineNumber>] ?lineNumber: int
        )
        : TestCase
        =
        Internal.mk name filePath lineNumber (fun _ -> Property.checkWith config prop)
