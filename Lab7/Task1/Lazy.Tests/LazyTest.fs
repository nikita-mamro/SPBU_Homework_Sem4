module Lazy.Tests

open Interfaces
open LazyFactory

open System.Threading

open NUnit.Framework
open FsUnit

/// Tests for single threaded lazy

[<Test>]
let ``Single thread lazy should contain needed value`` () =
    let value = "some value"
    let lazyObj = LazyFactory.CreateLazy(fun () -> value)
    lazyObj.Get() |> should equal value

[<Test>]
let ``Single thread lazy should keep correct value`` () =
    let value = Some 123
    let lazyObj = LazyFactory.CreateLazy(fun () -> value)
    lazyObj.Get() |> should equal value
    lazyObj.Get() |> should equal value

[<Test>]
let ``Single threaded calculations should be really lazy`` () =
    let mutable value = 0

    let supplier () =
        value <- value + 3
        value

    let lazyObj = LazyFactory.CreateLazy(supplier)
    lazyObj.Get() |> should equal 3

/// Tests for multithreading scenarios

let TestMultithreaded (lazyObj : ILazy<'a>, expected) =

    let resetEvent = new ManualResetEvent(false)

    let threadInit () =
        resetEvent.WaitOne() |> ignore
        for i in 0 .. 5 do
            lazyObj.Get() |> should equal expected

    let threads = Array.init 10 (fun _ -> Thread(threadInit))

    for t in threads do
        t.Start()

    resetEvent.Set() |> ignore

    for t in threads do
        t.Join()

[<Test>]
let ``Thread safe lazy should work properly`` () =
    let mutable value = 0

    let supplier () =
        value <- value + 3
        value

    TestMultithreaded (LazyFactory.CreateThreadSafeLazy(supplier), 3)
    value |> should equal 3

[<Test>]
let ``Lock free lazy should work properly`` () =
    let mutable value = 123

    let supplier () =
        value <- value - 100
        value

    TestMultithreaded (LazyFactory.CreateThreadSafeLazy(supplier), 23)
    value |> should equal 23
