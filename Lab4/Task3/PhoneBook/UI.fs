module UI

open System
open System.IO
open PhoneBook

let root = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())))
let storagePath = root + "/Storage/storage.txt"

let printMenu ()=
    printfn "1. Выйти"
    printfn "2. Добавить запись (имя и телефон)"
    printfn "3. Найти телефон по имени"
    printfn "4. Найти имя по телефону"
    printfn "5. Вывести всё текущее содержимое базы"
    printfn "6. Сохранить текущие данные в файл"
    printfn "7. Считать данные из файла"

// List of handlers for different menu item

// Command "1" (exit app) handler
let exitCommand () =
    Environment.Exit(0)

// Command "2" (add new record) handler
let addCommand seq =
    printfn "Добавление записи..."
    printf "Введите имя: "
    let name = Console.ReadLine()
    printf "Введите номер: "
    let phone = Console.ReadLine()
    add (name, phone) seq

// Command "3" (search phone number by name) handler
let findPhoneCommand seq =
    printfn "Поиск телефона по имени..."
    printf "Введите имя: "
    let name = Console.ReadLine()
    findAndPrintPhone name seq storagePath

// Command "4" (search name by phone number) handler
let findNameCommand seq =
    printfn "Поиск имени по телефону..."
    printf "Введите телефон: "
    let phone = Console.ReadLine()
    findAndPrintName phone seq storagePath

// Command "5" (print all data) handler
let printCommand seq =
    Console.Clear()
    printAll seq storagePath

// Command "6" (save from buffer to file) handler
let saveCommand seq =
    save seq storagePath

// Command "7" (get data from file) handler
let printFromFileCommand () =
    printFromFile storagePath

let initializeUI =
    let rec loop seq =
        printMenu()
        let choice = Console.ReadLine()
        match choice with
        | "1" ->
            exitCommand
        | "2" ->
            loop (addCommand seq)
        | "3" ->
            findPhoneCommand seq
            loop seq
        | "4" ->
            findNameCommand seq
            loop seq
        | "5" ->
            printCommand seq
            loop seq
        | "6" ->
            loop (saveCommand seq)
        | "7" ->
            printFromFileCommand()
            loop seq
        | _ ->
            printfn "Выберите команду от 1 до 7"
            loop seq

    loop Seq.empty
