open System
open FParsec

open Interpreter

let exprParser, eRef = createParserForwardedToRef<LambdaTerm, unit>()

let str s = pstring s
let ws = spaces

let mutable map = Map.empty.Add(' ',' ')

let letParser =
    () //

let charParser =
    satisfy (fun c -> Char.IsLetterOrDigit(c) && c <> '\\' && c <> ' ')

let nameParser =
    charParser

let varParser =
    nameParser |>> Variable

// Parses applications like: A B (C D)
let appNoParParser =
    let rec formatRes list =
        let rec listToStr = function
            | [] -> ""
            | [x] -> x.ToString()
            | h :: t -> h.ToString() + " " + (listToStr t)

        match list with
        | [] -> []
        | h :: t -> (listToStr h) :: (formatRes t)

    let arrCharParser =
        pipe2
            ws
            charParser
            (fun _ c -> [c])

    let rec multiCharParser =
        between (str "(" .>> ws) (str ")") (sepBy ((satisfy (fun c -> c <> '\\' && c <> ' ')) <|> charParser) (str " "))

    let betweenParser =
        between (ws) (ws) (sepBy (arrCharParser <|> multiCharParser) (str " "))

    pipe2
        betweenParser
        ws
        (fun parser _ ->
            Variable 'x')

let appParParser =
    pipe5
        (skipChar '(')
        exprParser
        (spaces)
        exprParser
        (skipChar ')')
        (fun _ f _ arg _ ->
            Application (f, arg))

let namesParser =
    between (str "\\" .>> ws) (str ".") (sepBy charParser (str " "))

let absParser =
    pipe4
        ws
        namesParser
        ws
        exprParser
        (fun _ parameters _ body ->
            let rec absRec (list: list<char>) =
                if list.IsEmpty then
                    failwith "Invalid token"
                if list.Length = 1 then
                    Abstraction (list.Head, body)
                else
                    Abstraction (list.Head, absRec list.Tail)
            absRec parameters)

let init () =
    eRef := choice
        [
            varParser
            appParParser
            absParser
        ]

let parse str =
    init()
    let parser = !eRef .>> eof

    match run parser str with
    | Success (res, _, _) -> res
    | Failure (e, _, _) -> failwith e


[<EntryPoint>]
let main argv =
    let testInput = "\x y.(x y)"

    printfn "%A" <| parse testInput

    let x = "(a b) c d"

    let rec formatRes list =
        let rec listToStr = function
            | [] -> ""
            | [x] -> x.ToString()
            | h :: t -> h.ToString() + " " + (listToStr t)

        match list with
        | [] -> []
        | h :: t -> (listToStr h) :: (formatRes t)

    let newCharParser =
        pipe2
            ws
            charParser
            (fun _ c -> [c])

    let rec multiCharParser =
        between (str "(" .>> ws) (str ")") (sepBy ((satisfy (fun c -> c <> '\\' && c <> ' ')) <|> charParser) (str " "))

    let betweenParser =
        between (ws) (ws) (sepBy (newCharParser <|> multiCharParser) (str " "))

    let btwn = choice [betweenParser]

    match run btwn x with
    | Success (res, _, _) ->
        printfn "%A" <| formatRes res
    | Failure (e, _, _) ->
        failwith e

    0 // return an integer exit code
