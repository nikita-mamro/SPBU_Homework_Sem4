module Printer

// Prints formatted records
let print fileRecords bufferRecords =
    let printRecord = function
        | (name, number) -> printfn "%A - %A" name number

    printfn "\nФормат вывода: Имя - Номер"
    printfn "\nДанные из файла:\n"

    if (Seq.isEmpty fileRecords) then
        printfn "Нет данных"
    else
        fileRecords |> Seq.iter (printRecord)

    if not (Seq.isEmpty bufferRecords) then
        printfn "\nДанные из буффера:\n"
        bufferRecords |> Seq.iter (printRecord)

    printfn "\n"
