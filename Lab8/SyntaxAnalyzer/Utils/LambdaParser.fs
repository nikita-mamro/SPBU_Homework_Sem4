module LambdaParser

open System
open FParsec

open Interpreter

/// Parses expression to lambda term, should support naming
///
/// Example (not implemented yet):
///
/// Input:
/// let K = \x y. x
/// K K
/// Output:
/// Application(Abstraction('x', Abstraction('y', Variable('x'))), Abstraction('x', Abstraction('y', Variable('x'))))
///
/// Currently available: pass sth like "\x y z.x y" -> res Abs(x,Abs(y,Abs(z, App(var x, var y))))
/// Not abvailable:
///     "let" parsing logic
type Parser =
    static member Parse expression =
        let exprParser, eRef = createParserForwardedToRef<LambdaTerm, unit>()

        /// Some utils
        let str s = pstring s
        let ws = spaces

        /// Map to store named variables when parsing is implemented
        let mutable map = Map.empty

        /// Proceeds input like "let X = *some lambda*" and stores X's lambda term in variables map
        let letParser =
            map <- map.Add('X', Variable 'x')
            () // TODO

        /// Parsers for characters

        let charParser =
            satisfy (fun c -> Char.IsLetterOrDigit(c) && c <> '\\' && c <> ' ')

        let varParser =
            charParser |>> Variable

        /// Application parsing logic
        let appParenthesesParser =
            pipe5
                (skipChar '(')
                varParser
                (spaces)
                varParser
                (skipChar ')')
                (fun _ f _ arg _ ->
                    Application (f, arg))

        /// Parses applications like: A B (C D)  (at least should parse)
        let appNoParenthesesParser =
            let arrCharParser =
                pipe2
                    ws
                    charParser
                    (fun _ c -> [c])

            let rec multiCharParser =
                between (str "(" .>> ws) (str ")") (sepBy ((satisfy (fun c -> c <> '\\' && c <> ' ')) <|> charParser) (str " "))

            let betweenParser =
                between (ws) (ws) (sepBy (arrCharParser <|> multiCharParser) (str " "))

            /// Some utils, might have been a mistake to make them
            /// instead of looking for FParsec alternatives

            /// Gets list after running betweenParser and formats it to needed list of strings
            let rec formatParserRes list =
                let rec listToStr = function
                    | [] -> ""
                    | [x] -> x.ToString()
                    | h :: t -> "(" + h.ToString() + " " + (listToStr t) + ")"

                match list with
                | [] -> []
                | h :: t -> (listToStr h) :: (formatParserRes t)

            /// Gets list from formatRes and moves everything apart from last element to the begining
            /// That's how we use it to create application then
            let rec getAbstractionPair (list: list<string>) =
                match list with
                | [] -> []
                | h :: t ->
                    match t with
                    | [] -> list
                    | [x] -> list
                    | head :: tail ->
                        (h + " " + head) :: getAbstractionPair tail

            pipe2
                betweenParser
                ws
                (fun parsed _ ->
                    let res = parsed |> formatParserRes |> getAbstractionPair

                    // Example:
                    // input: "x z (y z)"
                    // res: ["x z";"(y z)"]
                    // then we should do App(parse("x z"), parse("(y z)"))

                    if (res.Length = 1) then
                        try
                            Variable ((char)res.Head)
                        with _ ->
                            match run appParenthesesParser res.Head with
                            | Success (res, _, _) ->
                                res
                            | _ ->
                                failwith "Invalid token"
                    else
                        match (run exprParser res.Head, run exprParser res.Tail.Head) with
                        | (Success (left, _, _), Success (right, _, _)) ->
                            Application (left, right)
                        | _ ->
                            failwith "Invalid token"
                    )
        /// End of application parser

        ///Abstraction arguments parsing logic
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
        /// End of abstraction parser

        /// Initializes the main parser
        let init () =
            eRef := choice
                [
                    absParser
                    appNoParenthesesParser
                ]

        /// Parses lambda term and returns needed value of 'LambdaTerm' type from Interpreter project
        let parse str =
            init()
            let parser = !eRef .>> eof

            match run parser str with
            | Success (res, _, _) -> res
            | Failure (e, _, _) -> failwith e

        parse expression