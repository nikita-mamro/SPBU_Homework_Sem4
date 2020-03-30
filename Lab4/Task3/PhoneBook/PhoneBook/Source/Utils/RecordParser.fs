module RecordParser

let parse (expression : string) =
    let rec listToString = function
    | [e] ->
        e.ToString()
    | h :: t ->
        h.ToString() + " " + listToString t
    | [] ->
        ""

    let listToPair = function
        | h :: t ->
            (listToString t, h)
        | [] ->
            invalidArg "list" "List is not convertable to pair"
    try
        expression.Split ' '
        |> Seq.toList
        |> listToPair
        |> Some
    with
        | ex -> printfn "Expression can't be parsed"; None