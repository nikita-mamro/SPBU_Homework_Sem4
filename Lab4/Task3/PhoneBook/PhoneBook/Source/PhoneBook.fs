module PhoneBook

open Printer
open FileIO
open System.IO

let root = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())))
let storagePath = root + "/Storage/storage.txt"
let bufferPath = root + "/Storage/buffer.txt"

// Adds a new record no unsaved records
let add (record : string * string) =
    write (seq {record}) bufferPath

// Saves changes to file
let save =
    let bufferRecords = read bufferPath
    write bufferRecords storagePath

// Read data from file as a
// sequence of pairs (name, phone)
let getSavedRecords =
    read storagePath

// Searches all the data
// for a phone / phones by a given name
let findPhone name =
    let bufferRecords = read bufferPath
    getSavedRecords
    |> Seq.append bufferRecords
    |> Seq.filter (fun x -> fst x = name)

// Console GUI support for findPhone()
let findAndPrintPhone name =
    let searchResults = findPhone name

    if Seq.isEmpty searchResults then
        printfn "No record found"
    else
        let printPhone = function
            (_, phone) -> printfn "%A" phone

        searchResults |> Seq.iter (printPhone)

// Searches all the data
// for a name / names by a given phone
let findName phone =
    let bufferRecords = read bufferPath
    getSavedRecords
    |> Seq.append bufferRecords
    |> Seq.filter (fun x -> snd x = phone)

// Console GUI support for findPhone()
let findAndPrintName phone =
    let searchResults = findName phone

    if Seq.isEmpty searchResults then
        printfn "No record found"
    else
        let printName = function
            (name, _) -> printfn "%A" name

        searchResults |> Seq.iter (printName)

// Prints all records saved in file
let printFileData =
    getSavedRecords |> print
