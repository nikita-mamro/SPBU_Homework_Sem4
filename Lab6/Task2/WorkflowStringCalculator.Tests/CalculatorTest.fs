module WorkflowStringCalculator.Tests

open NUnit.Framework
open FsUnit
open Calculator

[<Test>]
let ``Correct calculations should return result`` () =
    let calculate = new CalculationBuilder()
    calculate {
        let! a = "1"
        let! b = "2"
        let c = a + b
        let! d = "10"
        let! e = "12"
        let f = (e - d) * c
        let! g = "60"
        let h = g / f
        return h
    } |> should equal (Some(10))

[<Test>]
let ``Incorrect calculations should return value showing that there's no result`` () =
    let calculate = new CalculationBuilder()
    calculate {
        let! a = "1"
        let b = a * 2
        let! c = "Ú"
        let d = a + b + c
        return d
    } |> should equal None
