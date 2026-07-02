namespace Scriptorium.Hedgehog

open System

/// <summary>Tracks how deep auto-generation has recursed, per type.</summary>
/// <remarks>
/// Recursion is limited **per type** (keyed by full name): two distinct types can each reach
/// the configured depth independently. <c>CanRecurse</c> goes false once the limit is reached so
/// that collections bottom out (empty) and unions prefer non-recursive cases instead of looping.
/// </remarks>
type internal RecursionState =
    {
        Depths: Map<string, int>
        CanRecurse: bool
    }

[<RequireQualifiedAccess>]
module internal RecursionState =

    let empty: RecursionState =
        {
            Depths = Map.empty
            CanRecurse = true
        }

    /// <summary>Reconciles the state for visiting <paramref name="t"/> under the given depth limit.</summary>
    /// <returns><c>None</c> when the type has already recursed past the limit (no finite value can be
    /// produced); otherwise the advanced state with that type's depth incremented.</returns>
    let reconcile (maxDepth: int) (t: Type) (state: RecursionState) : RecursionState option =
        let key = t.FullName
        let level = state.Depths |> Map.tryFind key |> Option.defaultValue 0

        if level > maxDepth then
            None
        else
            Some
                {
                    Depths = state.Depths |> Map.add key (level + 1)
                    CanRecurse = level < maxDepth
                }
