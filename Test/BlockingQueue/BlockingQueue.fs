module BlockingQueue

open System

type BlockingQueue<'a> () =
    let mutable items = []
    let locker = new Object()

    let enq (item : 'a) =
        lock locker (fun () ->
            items <- List.append items [item] )

    let deq () =
        let rec loop () =
            match items with
            | h :: t ->
                items <- t
                h
            | [] ->
                loop()
        lock locker (loop)

    member _.Count() = items.Length
    member _.Enqueue item = enq item
    member _.Dequeue () = deq ()
