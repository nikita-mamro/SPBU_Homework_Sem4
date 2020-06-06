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
            | h :: t -> "(" + h.ToString() + " " + (listToStr t) + ")"

        match list with
        | [] -> []
        | h :: t -> (listToStr h) :: (formatRes t)

    let rec getFinalRes (list: list<string>) =
        match list with
        | [] -> []
        | h :: t ->
            match t with
            | [] -> list
            | [x] -> list
            | head :: tail ->
                (h + " " + head) :: getFinalRes tail

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
        (fun parsed _ ->
            let res = parsed |> formatRes |> getFinalRes

            // ex: x z (y z)
            // res: [x z;(y z)]

            // here enters with zero length sometimes

            if (res.Length = 1) then
                Variable ((char)res.Head)
            else
                match (run exprParser res.Head, run exprParser res.Tail.Head) with
                | (Success (left, _, _), Success (right, _, _)) ->
                    Application (left, right)
                | _ ->
                    failwith "Error"
            )

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
            absParser
            appParParser
            appNoParParser
            varParser
        ]

let parse str =
    init()
    let parser = !eRef .>> eof

    match run parser str with
    | Success (res, _, _) -> res
    | Failure (e, _, _) -> failwith e


[<EntryPoint>]
let main argv =
    let testInput = "\x y z.x z (y z)"

    printfn "%A" <| parse testInput

    0 // return an integer exit code
