module BracketBalance.Tests

open NUnit.Framework
open FsUnit
open BracketBalanceChecker

/// Test data sets

let balanceCheckCases () =
    [
        "()()()", true
        "((x)(xx(xx)x)x()x()x)", true
        "(", false
        "()()()(", false
        "(()(())", false
    ] |> Seq.map (fun (expression, expected) -> TestCaseData(expression, expected))

let balanceCheckDifferentTypesCases () =
    [
        "", true
        "<{", false
        "[}", false
        "{<[()]>}", true
        "[()]", true
        "[)(]", false
        "<[>]", false
    ] |> Seq.map (fun (expression, expected) -> TestCaseData(expression, expected))

/// Generates single bracket type datasets for other types of brackets
let generateTestDataSets expression expected =
    let convertExpression opening closing expression =
        String.map(fun x -> if x = '(' then opening elif x = ')' then closing else x) expression

    seq {
        yield (checkGeneric ([('(', ')')]), expression, expected)
        yield (checkGeneric ([('[', ']')]), convertExpression '[' ']' expression, expected)
        yield (checkGeneric ([('{', '}')]), convertExpression '{' '}' expression, expected)
        yield (checkGeneric ([('<', '>')]), convertExpression '<' '>' expression, expected)
    }

/// Tests for bracket balance checker

[<TestCaseSource("balanceCheckCases")>]
let ``Checker should correctly check balance in expression`` (expression, expected) =
    let launchTest dataSet =
        match dataSet with
        | (check, expr, expectedResult) ->
            check expr |> should equal expectedResult

    Seq.iter launchTest (generateTestDataSets expression expected)

[<TestCaseSource("balanceCheckDifferentTypesCases")>]
let ``Checker should work correctly for expressions with different types of brackets`` (expression, expected) =
    checkGeneric ([('(',')'); ('[',']'); ('{','}'); ('<','>')]) expression |> should equal expected
