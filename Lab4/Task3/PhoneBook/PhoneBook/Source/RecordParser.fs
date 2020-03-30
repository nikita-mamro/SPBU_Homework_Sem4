module RecordParser

let parse (expression : string) =
    let rec listToString = function
    | [e] ->
        e.ToString()
    | h :: t ->
        h.ToString() + " " + listToString t
    | [] ->
        invalidArg "list" "Список не подходит по формату"

    let listToPair = function
        | h :: t ->
            (listToString t, h)
        | [] ->
            invalidArg "list" "Список не подходит по формату"
    try
        expression.Split ' '
        |> Seq.toList
        |> listToPair
        |> Some
    with
        | ex -> printfn "Выражение не подходит по формату"; None