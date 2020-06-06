module SyntaxAnalyzer.Tests

open NUnit.Framework
open FsUnit

open Interpreter
open LambdaParser

[<Test>]
let ``dummy test`` () =
    Parser.Parse "\x y z.x y" |> should equal (Abstraction('x', Abstraction('y', Abstraction('z', Application(Variable('x'), Variable('y'))))))