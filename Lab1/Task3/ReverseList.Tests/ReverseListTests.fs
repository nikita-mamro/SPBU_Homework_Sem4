module ReverseList.Tests

open NUnit.Framework
open ReverseList

let sourceLists () =
    [
        [], []
        [1], [1]
        [1; 2], [2; 1]
        [2; 4; 6; 3], [3; 6; 4; 2]
    ] |> List.map (fun (input, expected) -> TestCaseData(input, expected))

[<TestCaseSource("sourceLists")>]
let ReverseTests (input, expected) = 
    Assert.AreEqual(expected, reverseList input)