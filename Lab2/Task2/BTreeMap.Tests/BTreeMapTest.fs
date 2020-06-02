module BTreeMap.Tests

open NUnit.Framework
open BTreeMap

let sourceDataInt() =
    [
        Empty,
        (fun x -> x * x * x),
        Empty

        Node(Empty, 1, Empty), 
        (fun x -> x + 1), 
        Node(Empty, 2, Empty)

        Node(Node(Empty, 0, Empty), 1, Node(Empty, 2, Empty)), 
        (fun x -> x * 2), 
        Node(Node(Empty, 0, Empty), 2, Node(Empty, 4, Empty))

        Node(Node(Node(Node(Empty, 0, Empty), 1, Empty), 2, Empty), 3, Empty),
        (fun x -> x + 2),
        Node(Node(Node(Node(Empty, 2, Empty), 3, Empty), 4, Empty), 5, Empty)    
    ] |> Seq.map (fun (input, expr, expected) -> TestCaseData(input, expr, expected))

let sourceDataString () =
    [
        Node(Node(Empty, "a", Empty), "b", Node(Empty, "c", Empty)), 
        (fun x -> "a"), 
        Node(Node(Empty, "a", Empty), "a", Node(Empty, "a", Empty))

        Node(Node(Node(Node(Empty, "123", Empty), "2345", Empty), "34567", Empty), "456789", Empty),
        (fun x -> (String.length x).ToString()),
        Node(Node(Node(Node(Empty, "3", Empty), "4", Empty), "5", Empty), "6", Empty) 
    ] |> Seq.map (fun (input, expr, expected) -> TestCaseData(input, expr, expected))

// Function to check if 2 trees are equal
let rec areTreesEqual t1 t2 =
    match (t1, t2) with
    | Node(l1, d1, r1), Node(l2, d2, r2) when d1 = d2 ->
        areTreesEqual l1 l2 && areTreesEqual r1 r2
    | Empty, Empty -> true
    | _ -> false

[<TestCaseSource("sourceDataInt")>]
[<TestCaseSource("sourceDataString")>]
let TreeMapTests (input, expr, expected) = 
    Assert.IsTrue(areTreesEqual (mapTree expr input) expected)