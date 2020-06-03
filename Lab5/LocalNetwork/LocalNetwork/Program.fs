module Program

open System
open Computer
open OperatingSystem
open LocalNetwork

open System.Collections.Generic

[<EntryPoint>]
let main args =
    let WindowsOS = OS(0.5)
    let MacOS = OS(0.3)
    let LinuxOS = OS(0.2)

    let cs =
        [
        new Computer(Guid.NewGuid(), WindowsOS, false)
        new Computer(Guid.NewGuid(), MacOS, false)
        new Computer(Guid.NewGuid(), WindowsOS, false)
        new Computer(Guid.NewGuid(), LinuxOS, false)
        new Computer(Guid.NewGuid(), WindowsOS, false)
        ]

    let m =
        [
            [0;1;0;1;1]
            [1;0;1;0;1]
            [0;1;0;1;0]
            [1;0;1;0;1]
            [1;1;0;1;0]
        ]

    let matr = m |> List.map (fun list -> (list |> List.map (fun x -> Convert.ToBoolean(x))))

    new Network(cs, matr) |> ignore


    let q = new Queue<int>()
    q.Enqueue(2)
    q.Enqueue(3)
    q.Enqueue(4)

    printfn "%A" <| q.Dequeue()
    printfn "%A" <| q.Dequeue()
    printfn "%A" <| q.Dequeue()


    0