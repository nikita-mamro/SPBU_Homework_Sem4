module Calculator

open System

// Builder for workflow, proceeding
// calculations with integers, being
// proceeded as strings
type CalculationBuilder() =
    member this.Bind(x : string, f) =
        try
            x |> int |> f
        with :? FormatException ->
            None
    member this.Return(x) =
       Some x
