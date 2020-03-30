module Printer

// Prints formatted records
let print records =
    let printRecord = function
        | (name, number) -> printfn "%A - %A" name number

    printfn "Name - Phone number"
    records |> Seq.iter (printRecord)