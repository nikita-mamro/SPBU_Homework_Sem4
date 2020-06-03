module Calculator

open System

// Workflow builder for float calculations
// with a certain accuracy
type CalculationBuilder(accuracy : int) =
    member this.Bind(x : float, f : float -> float) =
        Math.Round (f x, accuracy)
    member this.Return(x : float) =
        Math.Round (x, accuracy)
