module BlockingQueue.Tests

open BlockingQueue
open NUnit.Framework
open FsUnit
open System.Threading

[<Test>]
let ``Single thread single enqueue should work as expected`` =
    let queue = BlockingQueue<int>()
    queue.Enqueue 0
    queue.Count |> should equal 1
    queue.Dequeue |> should equal 0

[<Test>]
let ``Single thread multiple enqueue should work as expected`` =
    let queue = BlockingQueue<int>()
    queue.Enqueue -1
    queue.Enqueue 0
    queue.Enqueue 1
    queue.Count |> should equal 3
    queue.Dequeue |> should equal -1
    queue.Dequeue |> should equal 0
    queue.Dequeue |> should equal 1
    queue.Count |> should equal 3

[<Test>]
let ``Multi-thread enqueue should work as expected`` =
    let queue = BlockingQueue<int>()
    let threadA = Thread(fun () -> queue.Enqueue 1)
    let threadB = Thread(fun () -> queue.Enqueue 2)
    threadA.Start()
    threadB.Start()
    queue.Count |> should equal 2

[<Test>]
let ``Multi-thread dequeue should work as expected`` =
    let mutable x = 0
    let queue = BlockingQueue<int>()

    let enq =
        Thread.Sleep(1000)
        queue.Enqueue 1

    let threadA = Thread(fun () -> enq)
    let threadB = Thread(fun() -> x <- queue.Dequeue())

    threadA.Start()
    x |> should equal 0
    threadB.Start()
    x |> should equal 0

    Thread.Sleep(1500)
    x |> should equal 1