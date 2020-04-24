open BlockingQueue

let x = BlockingQueue<int>()

x.Enqueue 1
x.Enqueue 2

x.Dequeue()
printfn "%A" <| x.Count()
printfn "%A" <| x.Dequeue()
printfn "%A" <| x.Count()