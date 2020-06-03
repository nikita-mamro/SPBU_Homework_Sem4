module LocalNetwork

open Computer
open System.Collections.Generic

/// Represents local network in which computers can infect each other
type Network (computers: list<Computer>, matrix: list<list<bool>>) =
    /// First of all, checking if matrix's format is correct
    do
        if computers.Length <> matrix.Length then
            invalidArg "matrix" (sprintf "Matrix's size was incorrect")

        if computers.Length > 0 then
            let rec checkMatrixRec (matrix: list<list<bool>>) =
                match matrix with
                | [] ->
                    true
                | h :: t ->
                    if h.Length <> computers.Length then
                        invalidArg "matrix" (sprintf "Matrix format is incorrect, pass square matrix")
                    checkMatrixRec t

            checkMatrixRec matrix |> ignore


    let mutable _computers = computers

    /// Property to access info about computers directly
    member this.Computers = _computers

    /// Performs one iteration of network infection
    member this.Infect () =
        0

    /// Performs certain amount of iterations, starting with certain infected computer
    member this.StartInfection iterationsCount =
        for _ in 1 .. iterationsCount do
            this.Infect()
            this.PrintReport()

    /// Prints info about computers in console
    member this.PrintReport () =
        _computers |>
        List.iter (fun item -> printfn "id: %A\nInfection probability: %A\nIs infected: %A\n" item.Id item.OS.InfectionProbability item.IsInfected)

