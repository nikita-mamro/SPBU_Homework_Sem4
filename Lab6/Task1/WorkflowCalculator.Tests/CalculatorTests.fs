module WorkflowCalculator.Tests

open NUnit.Framework
open FsUnit
open Calculator

[<Test>]
let ``Zero mantissas calculations should work properly`` () =
    let rounding = new CalculationBuilder(3)
    rounding {
        let! a = 2.0 / 12.0
        let! b = 3.5
        return a / b
    } |> should (equalWithin 0.001) 0.048

[<Test>]
let ``All non-zero mantissas calculations should work properly`` () =
    let rounding = new CalculationBuilder(4)
    rounding {
        let! a = 10.5
        let! b = 15.68
        let! c = a + b
        let! d = 17.123
        return c / d
    } |> should (equalWithin 0.0001) 1.5289

[<Test>]
let ``Division by zero should return infinity`` () =
    let rounding = new CalculationBuilder(12)
    rounding {
        let! a = 1.0
        let! b = 0.0
        return a / b
    } |> should equal infinity

[<Test>]
let ``Long calculations should work properly`` () =
    let rounding = new CalculationBuilder(6)
    rounding {
        let! a = 12.4243252
        let! b = 77.3
        let! c = 0.235
        let! d = a + b * c
        let! e = 123.0
        let! f = 3.14
        let! g = d + e / f
        let! h = 0.987
        return g * h
    } |> should (equalWithin 0.000001) 68.854895