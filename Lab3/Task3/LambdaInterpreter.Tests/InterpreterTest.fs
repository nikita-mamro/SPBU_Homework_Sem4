module LambdaInterpreter.Tests

open NUnit.Framework
open FsUnit
open Interpreter

/// Sets of data for tests
let occursTestData () =
    [
        'a', Variable('a'), true,  true
        'a', Variable('x'), false, false
        'x', Application(Variable('a'), Variable('x')), true, true
        'z', Application(Variable('a'), Variable('x')), false, false
        'x', Abstraction('x', Application(Variable('a'), Variable('x'))), true, false
    ] |> Seq.map (fun (name, term, expected, expectedFree) -> TestCaseData(name, term, expected, expectedFree))

let alphaExceptionTestData  () =
    [
        'a', 'b', Variable('a')
        'x', 't', Application(Variable('x'), Variable('y'))
        'x', 'a', Abstraction('x', Application(Variable('x'), Variable('a')))
        'y', 'b', Abstraction('x', Application(Variable('x'), Variable('a')))
    ] |> Seq.map (fun (oldName, newName, term) -> TestCaseData(oldName, newName, term))

let alphaConvertionTestData () =
    [
        'x', 'y', Abstraction('x', Application(Variable('x'), Variable('a'))),
        Abstraction('y', Application(Variable('y'), Variable('a')))

        'y', 'a', Abstraction('y', Abstraction('x', Variable('x'))),
        Abstraction('a', Abstraction('x', Variable('x')))

        'x', 'y', Abstraction('x', Variable('x')),
        Abstraction('y', Variable('y'))
    ] |> Seq.map (fun (oldName, newName, term, expected) -> TestCaseData(oldName, newName, term,expected))

/// Checks if terms are same
let rec termsAreSame t1 t2 =
    match (t1, t2) with
    | (Variable(name1), Variable(name2)) ->
        name1 = name2
    | (Abstraction(parameter1, body1), Abstraction(parameter2, body2)) ->
        parameter1 = parameter2 && (termsAreSame body1 body2)
    | (Application(f1, arg1), Application(f2, arg2)) ->
        (termsAreSame f1 f2) && (termsAreSame arg1 arg2)
    | _ -> false

/// Tests for lambda term interpreter
[<TestCaseSource("occursTestData")>]
let ``occcurs() and occursFree() checks should return correct value`` (name, term, expected, expectedFree) =
    occurs name term |> should equal expected
    occursFree name term |> should equal expectedFree

[<TestCaseSource("alphaConvertionTestData")>]
let ``Alpha convertion should return correct term`` (oldName, newName, term, expected) =
    termsAreSame (alphaConvert oldName newName term) expected |> should equal true

[<TestCaseSource("alphaExceptionTestData")>]
let ``Alpha convertion throws exception for invalid arguments`` (oldName, newName, term) =
    (fun () -> alphaConvert oldName newName term |> ignore) |> should throw typeof<System.ArgumentException>

[<Test>]
let ``Beta reduction should return correct term`` () =
    let term = Application(Abstraction('c', Application(Variable('c'), Variable('b'))), Abstraction('a', Variable('a')))
    term |> betaReduction |> should equal (Variable 'b')
