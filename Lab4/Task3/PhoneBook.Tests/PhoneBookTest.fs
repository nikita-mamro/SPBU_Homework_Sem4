module PhoneBook.Tests

open NUnit.Framework
open FsUnit
open System.IO
open PhoneBook

let root () = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())))
let storagePath () = root() + "/Storage/storage.txt"

let data() = getSavedRecords <| storagePath()

let records ()= seq {
    yield ("Angel Estrada", "202-555-0163")
    yield ("Justin Guzman", "202-555-0119")
    yield ("Grady Welch", "202-555-0131")
    yield ("Jeannie Robertson", "202-555-0195")
}

[<SetUp>]
let Setup () =
    File.WriteAllLines(storagePath(), seq {
        yield "202-555-0163 Angel Estrada"
        yield "202-555-0119 Justin Guzman"
        yield "202-555-0131 Grady Welch"
        yield "202-555-0195 Jeannie Robertson"
    })

[<Test>]
let ``Reading from file should be correct`` () =
    let expected = records()
    data() |> should equal expected

[<Test>]
let ``Saving empty buffer shouldn't change the file`` () =
    let expected = records()
    storagePath() |> save Seq.empty |> should equal Seq.empty
    data() |> should equal expected

[<Test>]
let ``Adding without saving shouldn't change the file`` () =
    let expected = records()
    add ("xxx", "xxx") Seq.empty |> should equal (Seq.singleton ("xxx", "xxx"))
    data() |> should equal expected

[<Test>]
let ``Saving not empty buffer should change the file`` () =
    let expected = Seq.append (records()) (Seq.singleton  ("xxx", "xxx"))
    storagePath() |> save (add ("xxx", "xxx") Seq.empty) |> ignore
    data() |> should equal expected

[<Test>]
let ``Find phone by name which exists in file`` () =
    findPhone "Jeannie" Seq.empty (storagePath()) |> Seq.map (snd) |> should contain "202-555-0195"

[<Test>]
let ``Find phone by name which exists in buffer`` () =
    findPhone "uwu" (Seq.singleton ("uwu", "uwu")) (storagePath()) |> Seq.map (snd) |> should contain "uwu"

[<Test>]
let ``Find phone by name which doesn't exist`` () =
    findPhone "ololo" Seq.empty (storagePath()) |> should be Empty

[<Test>]
let ``Find name by phone which exists in file`` () =
    findName "202-555-0131" Seq.empty (storagePath()) |> Seq.map (fst) |> should contain "Grady Welch"

[<Test>]
let ``Find name by phone which exists in buffer`` () =
    findName <| "uwu" <| (Seq.singleton ("uwu", "uwu")) <| storagePath() |> Seq.map (fst) |> should contain "uwu"

[<Test>]
let ``Find name by phone which doesn't exist`` () =
    findName "ololo" Seq.empty (storagePath()) |> should be Empty