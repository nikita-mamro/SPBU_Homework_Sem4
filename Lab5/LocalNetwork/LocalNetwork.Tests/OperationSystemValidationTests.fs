module OperationSystemValidationTests

open NUnit.Framework
open FsUnit

open System

open OperatingSystem

[<Test>]
let ``Infection probability must be more or equal to 0`` () =
    (fun () -> new OS(-0.2) |> ignore) |> should throw typeof<ArgumentException>

[<Test>]
let ``Infection probability must be less or equal to 1`` () =
    (fun () -> new OS(1.03) |> ignore) |> should throw typeof<ArgumentException>
