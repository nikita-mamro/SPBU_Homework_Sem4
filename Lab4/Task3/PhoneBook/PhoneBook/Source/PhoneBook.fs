module PhoneBook

open Printer
open RecordParser
open System.IO

// Adds a new record no unsaved records
let add record seq =
    Seq.append seq [record]

// Saves changes to file
let save seq storagePath =
    let write records path =
        let toWrite =
            records |> Seq.map(fun x -> match x with (name, phone) -> phone.ToString() + " " + name.ToString())
        File.AppendAllLines(path, toWrite)

    write seq storagePath
    Seq.empty

// Read data from file as a
// sequence of pairs (name, phone)
let getSavedRecords storagePath =
    let read path =
        File.ReadAllLines(path)
        |> Seq.map (fun (record : string) ->
            match parse record with
            | Some(pair) -> pair
            | None -> invalidOp "Ошибка при чтении из базы")

    read storagePath

// Searches all the data
// for a phone / phones by a given name
let findPhone (name : string) seq storagePath=
    getSavedRecords storagePath
    |> Seq.append seq
    |> Seq.filter (fun x -> ((fst x).ToLower().Contains (name.ToLower())))

// Console UI support for findPhone()
let findAndPrintPhone name seq storagePath=
    let searchResults = findPhone name seq storagePath

    if Seq.isEmpty searchResults then
        printfn "Запись не найдена\n"
    else
        let printPhone = function
            (name, phone) -> printfn "%A's phone - %A" name phone

        printfn "Результаты поиска:"
        searchResults |> Seq.iter (printPhone)
        printf "\n"

// Searches all the data
// for a name / names by a given phone
let findName (phone : string) seq storagePath =
    getSavedRecords storagePath
    |> Seq.append seq
    |> Seq.filter (fun x -> ((snd x).ToLower().Contains phone))

// Console UI support for findPhone()
let findAndPrintName phone seq storagePath =
    let searchResults = findName phone seq storagePath

    if Seq.isEmpty searchResults then
        printfn "Запись не найдена"
    else
        let printName = function
            (name, _) -> printfn "%A" name

        searchResults |> Seq.iter (printName)

// Prints all records
let printAll seq storagePath =
    let saved = getSavedRecords storagePath
    print saved seq

let printFromFile storagePath =
    let saved = getSavedRecords storagePath
    print saved Seq.empty