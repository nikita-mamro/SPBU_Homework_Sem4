module Calculator

open System

// Workflow builder for float calculations
// with a certain accuracy
type CalculationBuilder(accuracy : int) =
    member this.Bind(x, f) =
         match f x with
            | (res : float) ->
                Math.Round (res, accuracy)
    member this.Return(x) =
        x