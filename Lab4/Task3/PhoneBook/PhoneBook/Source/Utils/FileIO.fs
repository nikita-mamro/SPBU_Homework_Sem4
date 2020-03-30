module FileIO

open RecordParser
open System.IO

let write records path =
    let toWrite =
        records |> Seq.map(fun x -> match x with (name, phone) -> phone.ToString() + " " + name.ToString())
    File.AppendAllLines (path, toWrite)

let read path =
    File.ReadLines(path)
    |> Seq.map (fun (record : string) ->
        match parse record with
        | Some(pair) -> pair
        | None -> invalidOp "Error occured reading the phonebook")
