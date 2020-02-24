module CreateList.Tests

open NUnit.Framework
open CreateList

let testData () =
    [
        0, 0, Some([1])
        0, 3, Some([1; 2; 4; 8])
        1, 3, Some([2; 4; 8; 16])
    ] 
    |> List.map (fun (n, m, expected) -> TestCaseData(n, m, expected))

[<TestCaseSource("testData")>]
let ReverseTests (n, m, expected) = 
    Assert.AreEqual(expected, createList n m)

[<Test>]
let ReverseNoneTests () =
    Assert.AreEqual(None, createList -1 0)
    Assert.AreEqual(None, createList 0 -1)