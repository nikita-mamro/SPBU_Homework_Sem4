module Stack.Tests

open NUnit.Framework
open FsUnit
open Stack

// Test data set
let multiplePushCases () =
    [
        ['a'; 'b']
        ['a'; 'b'; 'c'; 'd'; 'e'; 'f']
    ] |> Seq.map (fun (elements) -> TestCaseData(elements))

// Checks if stacks are equal
let stacksAreEqual s1 s2 =
    match (s1, s2) with
    | (Empty, Empty) ->
        true
    | (Stack(e1, stack1), Stack(e2, stack2)) when e1 = e2 ->
        stack1 = stack2
    | (_, _) -> false

// Tests for CharStack
[<Test>]
let ``Single push should not return empty stack`` () =
    (Empty.Push 'x') = Empty |> should equal false

[<Test>]
let ``Single push should return expected stack`` () =
    stacksAreEqual (Empty.Push 'x') (Stack('x', Empty)) |> should equal true

[<TestCaseSource("multiplePushCases")>]
let ``Multiple push should not return empty stack`` (elements) =
    let rec pushAll (s : CharStack) = function
        | h :: t ->
            pushAll (s.Push h) t
        | _ -> s
    (pushAll Empty elements) = Empty |> should equal false

[<Test>]
let ``Multiple push should return expected`` () =
    stacksAreEqual ((Empty.Push 'x').Push 'y') (Stack('y', Stack('x', Empty))) |> should equal true

[<Test>]
let ``Top should return correct element`` () =
    Stack('x', Stack('y', Stack('z', Empty))).Top |> should equal 'x'

[<Test>]
let ``Top should throw exception when stack is empty`` () =
    (fun () -> Empty.Top |> ignore) |> should throw typeof<System.InvalidOperationException>

[<Test>]
let ``Single element Pop test`` () =
    (Stack('x', Empty).Pop) = Empty |> should equal true

[<Test>]
let ``Multiple elements Pop test`` () =
    stacksAreEqual (Stack('x', Stack('y', Stack('z', Empty))).Pop.Pop) (Stack('z', Empty)) |> should equal true

[<Test>]
let ``Pop should throw exception when stack is empty`` () =
    (fun () -> Empty.Pop |> ignore) |> should throw typeof<System.InvalidOperationException>
