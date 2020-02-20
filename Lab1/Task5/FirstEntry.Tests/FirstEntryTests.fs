module FirstEntry.Tests

open NUnit.Framework
open FirstEntry

let testData () = 
    [
        0, [0], Some(0)
        0, [0; 1; 2], Some(0)
        0, [1; 0; 2], Some(1)
        0, [1; 2; 0], Some(2)
    ] |> List.map (fun (element, list, expected) -> TestCaseData(element, list, expected))

[<TestCaseSource("testData")>]
let FirstEntryTest (element, list, expected) =
    Assert.AreEqual(expected, firstEntry element list)

[<Test>]
let FirstEntryNoneTest () =
    Assert.AreEqual(None, firstEntry 0 [])
    Assert.AreEqual(None, firstEntry 0 [1; 2; 3])
