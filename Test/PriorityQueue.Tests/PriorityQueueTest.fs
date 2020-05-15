module PriorityQueue.Tests

open NUnit.Framework
open FsUnit
open PriorityQueue

/// Tested queue object
let mutable queue = new PriorityQueue<int>()

[<SetUp>]
let ``Initialize`` () =
    queue <- new PriorityQueue<int>()

///Set of tests for priority queue implementation

[<Test>]
let ``Count of empty queue should be zero`` () =
    queue.Count |> should equal 0

[<Test>]
let ``Count after single dequeue should be one`` () =
    queue.Enq 0 0
    queue.Count |> should equal 1

[<Test>]
let ``Count after multiple enqueue should be correct`` () =
    queue.Enq 0 0
    queue.Enq 0 0
    queue.Count |> should equal 2

[<Test>]
let ``Count after enqueue and dequeue should be zero`` () =
    queue.Enq 0 0
    queue.Deq() |> ignore
    queue.Count |> should equal 0

[<Test>]
let ``Count after multiple enqueue and dequeue should be correct`` () =
    queue.Enq 0 0
    queue.Enq 0 0
    queue.Enq 0 0
    queue.Enq 0 0
    queue.Deq() |> ignore
    queue.Count |> should equal 3

[<Test>]
let ``Dequeue from single-item queue should return correct value`` () =
    queue.Enq 3 1
    queue.Deq() |> should equal 3

[<Test>]
let ``Enqueue for items with same priority should work properly`` () =
    queue.Enq 1 0
    queue.Enq 2 0
    queue.Enq 0 0
    queue.Deq() |> should equal 1

[<Test>]
let ``Enqueue with priority should work properly`` () =
    queue.Enq -1 0
    queue.Enq 0 0
    queue.Enq 2 1
    queue.Deq() |> should equal 2

[<Test>]
let ``Queue with multiple priority values should work properly`` () =
    queue.Enq 0 -1
    queue.Enq 1 0
    queue.Enq 3 5
    queue.Enq 2 1
    queue.Enq -12 4
    queue.Enq 0 0
    queue.Deq() |> should equal 3
    queue.Deq() |> should equal -12
    queue.Deq() |> should equal 2
    queue.Deq() |> should equal 1
    queue.Deq() |> should equal 0
    queue.Deq() |> should equal 0