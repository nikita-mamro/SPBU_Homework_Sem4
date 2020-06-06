module SquareTest

open NUnit.Framework
open FsUnit
open SquarePrinter

let testData () =
    [
        0, []
        1, [["*"]]
        2, [["*"; "*"]; ["*"; "*"]]
        3, [["*"; "*"; "*"]; ["*"; " "; "*"]; ["*"; "*"; "*"]]
    ]
    |> Seq.map (fun (input, expected) -> TestCaseData(input, expected))

[<TestCaseSource("testData")>]
let ``Square should be found correctly`` (input, expected) =
    getSquare input |> should equal expected
