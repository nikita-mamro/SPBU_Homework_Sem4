module ListMinTest

open NUnit.Framework
open FsUnit
open ListMin

let testData () =
    [
        [1], 1
        [0], 0
        [-1], -1
        [1; 2; 10], 1
        [-10; -2; -1], -10
        [-100; -10000], -10000
        [123; 123; 0; 0], 0
    ]
    |> Seq.map (fun (input, expected) -> TestCaseData(input, expected))

[<TestCaseSource("testData")>]
let ``Minimum should be found correctly`` (input, expected) =
    min input |> should equal expected


[<Test>]
let ``Empty list should cause exception`` () =
    (fun () -> min [] |> ignore) |> should throw typeof<System.Exception>