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
        new Computer(WindowsOS, false)
        new Computer(MacOS, false)
        new Computer(WindowsOS, false)
        new Computer(LinuxOS, false)
        new Computer(WindowsOS, false)
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

    let n = Network(cs, matr)

    n.PrintReport()


    0