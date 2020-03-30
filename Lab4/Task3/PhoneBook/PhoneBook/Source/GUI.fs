module GUI

open System
open PhoneBook

let printMenu =
    printfn "1. Выйти"
    printfn "2. Добавить запись (имя и телефон)"
    printfn "3. Найти телефон по имени"
    printfn "4. Найти имя по телефону"
    printfn "5. Вывести всё текущее содержимое базы"
    printfn "6. Сохранить текущие данные в файл"
    printfn "7. Считать данные из файла"

let exitCommand =
    Environment.Exit

let addCommand =
    Console.Clear
    printfn "Добавление записи..."
    printfn "Введите имя:"
    let name = Console.ReadLine()
    printfn "Введите номер:"
    let phone = Console.ReadLine()

let proceedChoice = function
    | "1" -> exitCommand
    | "2" -> exitCommand
    | "3" -> exitCommand
    | "4" -> exitCommand
    | "5" -> exitCommand
    | "6" -> exitCommand
    | "7" -> exitCommand
    | _ -> exitCommand

let runGUI =
    let rec loop =
        printMenu
        let choice = Console.ReadLine()
        match choice with
        | "1" ->
            exitCommand
        | "2" ->
            exitCommand
        | "3" ->
            exitCommand
        | "4" ->
            exitCommand
        | "5" ->
            exitCommand
        | "6" ->
            exitCommand
        | "7" ->
            exitCommand
        | _ ->
            exitCommand
    loop
