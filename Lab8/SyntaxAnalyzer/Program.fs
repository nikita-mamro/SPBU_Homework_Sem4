open System
open FParsec

open Interpreter

let exprParser, eRef = createParserForwardedToRef<LambdaTerm, unit>()

let str s = pstring s
let ws = spaces

let letParser =
    () // todo

let charParser =
    satisfy (fun c -> Char.IsLetterOrDigit(c) && c <> '\\' && c <> ' ')

let nameParser =
    charParser

let varParser =
    nameParser |>> Variable

let appParser =
    // todo
    pipe5
        (skipChar '(')
        exprParser
        (many1 <| skipChar ' ')
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
            appParser
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
    let testInput = "\x y.x"

    printfn "%A" <| parse testInput

    0 // return an integer exit code
