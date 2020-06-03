module LocalNetworkTest

open System

open NUnit.Framework
open FsUnit

open LocalNetwork
open Computer
open OperatingSystem

let invurnelableOS () = OS(0.0)

let overvulnerableOS () = OS(1.0)

/// Will use this just for tests
let getBoolMatrix (m : list<list<int>>) = m |> List.map (fun list -> (list |> List.map (fun x -> Convert.ToBoolean(x))))

let overvulnerableComputers () =
    [
        [
        Computer(Guid.NewGuid(), overvulnerableOS(), true)
        Computer(Guid.NewGuid(), overvulnerableOS(), false)
        Computer(Guid.NewGuid(), overvulnerableOS(), false)
        Computer(Guid.NewGuid(), overvulnerableOS(), false)
        Computer(Guid.NewGuid(), overvulnerableOS(), false)
        ]
    ] |> Seq.map (fun (computers) -> TestCaseData(computers))

let invulnerableComputers () =
    [
        [
            Computer(Guid.NewGuid(), invurnelableOS(), true)
            Computer(Guid.NewGuid(), invurnelableOS(), false)
            Computer(Guid.NewGuid(), invurnelableOS(), false)
        ], 1
    ] |> Seq.map (fun (computers, infected) -> TestCaseData(computers, infected))

[<TestCaseSource("overvulnerableComputers")>]
let ``Network should throw exception having wrong size matrix`` (computers) =
    (fun () -> Network(computers, []) |> ignore) |> should throw typeof<ArgumentException>

[<TestCaseSource("overvulnerableComputers")>]
let ``Network should throw exception having not square matrix`` (computers) =
    (fun () -> Network(computers, [[];[];[];[];[]]) |> ignore) |> should throw typeof<ArgumentException>

[<TestCaseSource("overvulnerableComputers")>]
let ``If probability is 1 infection should behave like bfs`` (computers) =
    let matrix =
        [
            [false; true; false; true; true]
            [true; false; true; false; true]
            [false; true; false; true; false]
            [true; false; true; false; true]
            [true; true; false; true; false]
        ]

    let net = Network(computers, matrix)

    // Only first computer is infected
    net.Computers |> List.map(fun x -> x.IsInfected) |> should equal [true; false; false; false; false]

    // First computer can infect neighbours
    net.Infect() |> should equal true
    // First computer infects connected computers, which are 2,4,5
    net.Computers |> List.map(fun x -> x.IsInfected) |> should equal [true; true; false; true; true]
    // Last computer can be infected
    net.Infect() |> should equal true
    // Last computer is infected
    net.Computers |> List.map(fun x -> x.IsInfected) |> should equal [true; true; true; true; true]
    // All computers are infected
    net.Infect() |> should equal false

[<TestCaseSource("invulnerableComputers")>]
let ``If probability is 0 infection should not happen`` (computers, infected) =
    let matrix =
        [
            [false; true; true]
            [true; false; false]
            [true; false; false]
        ]

    let net = Network(computers, matrix)

    net.Infect() |> should equal false

/// Computer can be infected only if it's connected with infected one,
/// it means that if there're no infected computers, there're no infections
[<Test>]
let ``If there're no infected computers infection should not happen`` () =
    let computers = [Computer(Guid.NewGuid(), OS(0.5), false); Computer(Guid.NewGuid(), OS(0.5), false)]

    let matrix =
        [
            [false; true]
            [true; false]
        ]

    let net = Network(computers, matrix)

    net.Infect() |> should equal false

[<TestCaseSource("overvulnerableComputers")>]
let ``No infections should happen if infected computers are isolated`` (computers) =
    let matrix =
        [
            [false; false; false; false; false]
            [false; false; true; false; true]
            [false; true; false; true; false]
            [false; false; true; false; true]
            [false; true; false; true; false]
        ]

    let net = Network(computers, matrix)

    net.Infect() |> should equal false