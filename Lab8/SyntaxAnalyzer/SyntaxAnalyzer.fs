module SyntaxAnalyzer

open Interpreter

open LambdaParser
open System.IO

type BetaAnalyzer () =
    member this.ParseString expression =
        let lambdaRes =
            expression
            |> Parser.Parse
            |> betaReduction

        lambdaRes.ToString()

    member this.ParseFile filename =
        0