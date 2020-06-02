module PrimeSequence.Tests

open NUnit.Framework
open PrimeChecker
open PrimeSequence

let testIsPrimeData() =
    [
        -370248451, false
        -3, false
        0, false
        1, false
        2, true
        3, true
        4, false
        5, true
        6, false
        36, false
        100000000, false
        370248451, true
    ] |> Seq.map (fun (input, expected) -> TestCaseData(input, expected))

let testSequenceData() =
    [
        1, Some(2)
        5, Some(7)
        10000, Some(10007)
        10000000, Some(10000019)
        234725389, Some(234725399)
    ] |> Seq.map (fun (condition, expected) -> TestCaseData(condition, expected))

[<TestCaseSource("testIsPrimeData")>]
let IsPrimeTest (input, expected) =
    Assert.AreEqual(expected, isPrime input)

[<TestCaseSource("testSequenceData")>]
let SequenceTest(condition, expected) =
    let res = primeNumbers() |> Seq.tryPick (fun x -> if x > condition then Some(x) else None)
    Assert.AreEqual(expected, res)
