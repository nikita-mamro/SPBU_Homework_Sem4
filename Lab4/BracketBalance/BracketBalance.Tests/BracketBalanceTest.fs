module BracketBalance.Tests

open NUnit.Framework
open FsUnit
open BracketBalanceChecker

// Test data set
let balanceCheckCases () =
    [
        "()()()", true
        "((x)(xx(xx)x)x()x()x)", true
        "(", false
        "()()()(", false
        "(()(())", false
    ] |> Seq.map (fun (expression, expected) -> TestCaseData(expression, expected))

// Generates datasets for other types of brackets
let generateTestDataSets expression expected =
    let convertExpression opening closing expression =
        String.map(fun x -> if x = '(' then opening elif x = ')' then closing else x) expression

    seq {
        yield (checkGeneric ('(', ')', (=)), expression, expected)
        yield (checkGeneric ('[', ']', (=)), convertExpression '[' ']' expression, expected)
        yield (checkGeneric ('{', '}', (=)), convertExpression '{' '}' expression, expected)
        yield (checkGeneric ('<', '>', (=)), convertExpression '<' '>' expression, expected)
    }

// Tests for bracket balance checker
[<TestCaseSource("balanceCheckCases")>]
let ``Checker should correctly check balance in expression`` (expression, expected) =
    let launchTest dataSet =
        match dataSet with
        | (check, expr, expectedResult) ->
            check expr |> should equal expectedResult

    Seq.iter launchTest (generateTestDataSets expression expected)
