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
        new Computer(Guid.NewGuid(), overvulnerableOS(), false);
        new Computer(Guid.NewGuid(), overvulnerableOS(), false);
        new Computer(Guid.NewGuid(), overvulnerableOS(), false);
        new Computer(Guid.NewGuid(), overvulnerableOS(), false);
        new Computer(Guid.NewGuid(), overvulnerableOS(), false)
        ]
    ] |> Seq.map (fun (computers) -> TestCaseData(computers))

let invulnerableComputers () =
    [
        [
            new Computer(Guid.NewGuid(), invurnelableOS(), false);
            new Computer(Guid.NewGuid(), invurnelableOS(), false);
        ]
    ] |> Seq.map (fun (computers) -> TestCaseData(computers))

[<TestCaseSource("overvulnerableComputers")>]
let ``Network should throw exception having wrong size matrix`` (computers) =
    (fun () -> Network(computers, []) |> ignore) |> should throw typeof<ArgumentException>

[<TestCaseSource("overvulnerableComputers")>]
let ``Network should throw exception having not square matrix`` (computers) =
    (fun () -> Network(computers, [[];[];[];[];[]]) |> ignore) |> should throw typeof<ArgumentException>

[<TestCaseSource("overvulnerableComputers")>]
let ``If probability is 1 infection should behave like bfs`` () =
    0 |> should equal 1

[<TestCaseSource("invulnerableComputers")>]
let ``If probability is 0 infection should not happen`` () =
    0 |> should equal 1