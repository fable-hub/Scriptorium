# TypeScript: overloaded generic members hash to colliding names

## Symptom

Compiling to TypeScript sometimes fails with:

```
error FABLE: Cannot have two module members with same name: Test_property_535CAFD9
error FABLE: Cannot have two module members with same name: Test_propertyWith_669CBA1C
```

The members are overloaded static members whose generic parameter erases to the same
runtime type. On a *correct* compile Fable disambiguates them with distinct hashes:

```
Test_property_743B3C03   // (name, Property<bool>, ...)
Test_property_Z404AA92B  // (name, Property<unit>, ...)
Test_property_Z32C4CE32  // (name, Gen<'a>, 'a -> unit, ...)
```

On a failing compile both `bool` / `unit` overloads collapse onto one hash
(`Test_property_535CAFD9`) and collide.

## It is non-deterministic

- Cold compile (`--noCache` into a clean output dir) *usually* succeeds.
- Cached / incremental compiles, or compiles where a sibling `fable_modules` from a JS
  build is present, *frequently* fail.
- The same command can pass and then fail on repeated runs.

## Reliable trigger: Scriptorium.Hedgehog

The reduced sample in this folder (`Repro.fs` — a `Test`-like type with 3 + 2 overloads and
`CallerFilePath`/`CallerLineNumber` optional args) compiles cleanly on its own and did **not**
reproduce the collision as a single project. The collision reproduces in the real, multi-project
setup:

```bash
# from the Scriptorium repo root
cd tests/Scriptorium.Hedgehog.Test
dotnet fable --runScript                 # JS build, leaves an in-place fable_modules
rm -rf obj/fable-ts
dotnet fable --lang typescript -o obj/fable-ts --noCache   # often fails; re-run to see it flip
```

The overloaded members live in `src/Scriptorium.Hedgehog/DSL.fs` (`Test.property` ×3,
`Test.propertyWith` ×2). This points at Fable's TS overload-name hashing depending on
non-deterministic state (hash-set ordering / cross-project resolution / cache) rather than
purely on the declared signatures.

## Reduced sample (does not currently reproduce alone)

```bash
dotnet tool restore
dotnet fable --lang typescript -o out --noCache
grep -oE "Api_make[A-Za-z0-9_]*" out/Repro.ts | sort | uniq -c
```

Included as a starting point for minimization — the goal is to make it fail deterministically.
