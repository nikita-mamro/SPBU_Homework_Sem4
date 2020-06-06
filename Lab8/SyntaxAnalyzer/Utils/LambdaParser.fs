module LambdaParser

open Interpreter

open FParsec

/// Parses expression to lambda term
///
/// Example:
///
/// Input:
/// let K = \x y. x
/// K K
/// Output:
/// Application(Abstraction('x', Abstraction('y', Variable('x'))), Abstraction('x', Abstraction('y', Variable('x'))))

type Parser =
    static member Parse expression =
        let e, eRef = createParserForwardedToRef<LambdaTerm, unit>()

        let init () =
            eRef := choice
                [

                ]

        init()
        let parser = !eRef .>> eof

        match run parser expression with
        | Success (res, _, _) ->
            res |> ignore // TODO REMOVE ignore
        | Failure (e, _, _) ->
            failwith e |> ignore // TODO REMOVE ignore

        Variable 'x'