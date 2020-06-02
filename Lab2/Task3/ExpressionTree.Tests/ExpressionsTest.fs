module ExpressionTree.Tests

open NUnit.Framework
open FsUnit
open Power
open ExpressionTree

let testExceptionData() =
    [
        -100000000, -999999999
        -100000000, -1
        -1, -99999999
        -1, -1
        0, -99999999
        0, -1
        1, -99999999
        1, -1
        1000000000, -99999999
        1000000000, -1
    ] |> Seq.map (fun (number, exponent) -> TestCaseData(number, exponent))

let testPowerData() =
    [
        0, 0, 1
        0, 1, 0
        0, 123, 0
        1, 0, 1
        1, 1, 1
        100, 0, 1
        100, 1, 100
        2, 5, 32
        -3, 3, -27
        -4, 2, 16
    ] |> Seq.map (fun (number, exponent, expected) -> TestCaseData(number, exponent, expected))

let testExpressionTreeData() =
    [
        Number(0), 0
        Number(-1), -1
        Number(1), 1
        Sum(Number(1), Number(0)), 1
        Sum(Number(1), Number(2)), 3
        Sum(Number(-1), Number(2)), 1
        Sum(Number(1), Number(-2)), -1
        Sum(Number(-1), Number(-2)), -3
        Sum(Number(7777777), Number(-7777777)), 0
        Sub(Number(7), Number(3)), 4
        Sub(Number(2), Number(4)), -2
        Sub(Number(1), Number(0)), 1
        Sub(Number(0), Number(1)), -1
        Sub(Number(0), Number(-1)), 1
        Sub(Number(-1), Number(-2)), 1
        Sub(Number(99999999), Number(99999999)), 0
        Mul(Number(0), Number(0)), 0
        Mul(Number(0), Number(99999999)), 0
        Mul(Number(9999999), Number(0)), 0
        Mul(Number(2), Number(512)), 1024
        Mul(Number(2), Number(-512)), -1024
        Mul(Number(2), Number(1)), 2
        Div(Number(2), Number(2)), 1
        Div(Number(2), Number(-2)), -1
        Div(Number(23235235), Number(5)), 4647047
        Div(Number(0), Number(5)), 0
        Mod(Number(1), Number(2)), 1
        Mod(Number(4), Number(2)), 0
        Pow(Number(0), Number(0)), 1
        Pow(Number(2), Number(0)), 1
        Pow(Number(5), Number(3)), 125
        Sum(Sum(Number(1), Number(2)), Sum(Number(3), Number(4))), 10
        Sub(Sub(Number(1), Number(2)), Sub(Number(4), Number(3))), -2
        Mul(Mul(Number(1), Number(2)), Mul(Number(3), Number(4))), 24
        Div(Sum(Number(2), Number(1)), Div(Number(8), Number(4))), 1
        Mod(Mod(Number(1), Number(2)), Mod(Number(3), Number(4))), 1
        Pow(Pow(Number(2), Number(2)), Pow(Number(3), Number(2))), 262144
        Sum(Pow(Sum(Number(1), Number(2)), Sub(Number(6), Number(3))), Mul(Div(Number(9), Number(3)), Mod(Number(3), Number(2)))), 30
    ] |> Seq.map (fun (tree, expected) -> TestCaseData(tree, expected))

[<TestCaseSource("testExceptionData")>]
let ``Power with negative exponent should throw exception`` (number, exponent) =
    (fun () -> power number exponent |> ignore) |> should throw typeof<System.ArgumentException>

[<TestCaseSource("testPowerData")>]
let ``Power function should return correct values`` (number, exponent, expected) =
    power number exponent |> should equal expected

[<TestCaseSource("testExpressionTreeData")>]
let ``Tree should calculate expression correctly`` (tree, expected) =
    calculate tree |> should equal expected