module EvenCount.Tests

open NUnit.Framework
open FsCheck.NUnit
open EvenCount

let testData () = 
    [
        [], 0
        [0], 1
        [1], 0
        [-1], 0
        [-2], 1
        [0; 2; 4; 6; 8; 10], 6 
        [1; 3; 5; 7; 9; 11], 0 
        [0; 1; 3], 1
        [1; 0; 3], 1
        [1; 3; 0], 1
        [1; 2; 3], 1
        [0; 1; 2; 3; 4], 3
    ] |> List.map (fun (list, expected) -> TestCaseData(list, expected))

[<TestCaseSource("testData")>]
let MapEvenCountTest (list, expected) =
    Assert.AreEqual(expected, evenCountMap list)

[<TestCaseSource("testData")>]
let FilterEvenCountTest (list, expected) = 
    Assert.AreEqual(expected, evenCountFilter list)

[<TestCaseSource("testData")>]
let FoldEvenCountTest (list, expected) = 
    Assert.AreEqual(expected, evenCountFold list)

[<Property>]
let ``All implementations should return same value`` (xs:list<int>) = 
    let foldRes = evenCountFold xs
    let mapRes = evenCountMap xs
    let filterRes = evenCountFilter xs
    foldRes = mapRes && mapRes = filterRes