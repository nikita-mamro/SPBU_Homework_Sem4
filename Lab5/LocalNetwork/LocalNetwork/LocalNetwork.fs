module LocalNetwork

open Computer

open System

/// Represents local network in which computers can infect each other
type Network (computers: list<Computer>, matrix: list<list<bool>>) =
    /// First of all, checking if matrix's format is correct
    do
        if computers.Length <> matrix.Length then
            invalidArg "matrix" (sprintf "Matrix's size is incorrect")

        if computers.Length > 0 then
            let rec checkMatrixRec (matrix: list<list<bool>>) =
                match matrix with
                | [] ->
                    true
                | h :: t ->
                    if h.Length <> computers.Length then
                        invalidArg "matrix" (sprintf "Matrix format is incorrect, pass square matrix as parameter")
                    checkMatrixRec t

            checkMatrixRec matrix |> ignore

    let mutable _computers = computers

    /// Property to access info about computers directly
    member this.Computers = _computers

    /// Performs one iteration of network infection
    /// Returns true if some computers can be infected, false otherwise
    /// If some can be infected, all infected try to infect connected computers
    member this.Infect () =
        if not (_computers |> List.filter (fun x -> not x.IsInfected) |> List.isEmpty) then

            let mutable neighboursToInfect = 0
            let mutable allAreInvurnelable = true

            // Function to try to infect neighbours
            let tryToInfectNeighbours index =
                for i in 0 .. _computers.Length - 1 do
                    if matrix.[index].[i] then
                        neighboursToInfect <- neighboursToInfect + 1

                        if (_computers.[i].OS.InfectionProbability <> 1.0) then
                            allAreInvurnelable <- false

                        // Checking if neighbour is going to be infected
                        if (not _computers.[i].IsInfected && _computers.[i].OS.InfectionProbability > Random().NextDouble()) then
                            _computers.[i].IsInfected <- true

            let infected = _computers |> List.filter (fun x -> x.IsInfected)

            for pc in infected do
                _computers |> List.findIndex (fun x -> x.Id = pc.Id) |> tryToInfectNeighbours

            neighboursToInfect <> 0 && allAreInvurnelable
        else
            false

    /// Prints info about computers in console
    member this.PrintReport () =
        _computers |>
        List.iter (fun item -> printfn "id: %A\nInfection probability: %A\nIs infected: %A\n" item.Id item.OS.InfectionProbability item.IsInfected)

