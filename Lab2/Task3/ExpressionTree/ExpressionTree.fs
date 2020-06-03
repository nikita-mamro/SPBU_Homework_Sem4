module ExpressionTree

open Power

// Type for integer expressions in prefix notation
// Supports Sum(+), Sub(-), Mul(*), Div(/), Pow(^), Mod(%)
type Expression =
    | Number of int
    | Sum of Expression * Expression
    | Sub of Expression * Expression
    | Mul of Expression * Expression
    | Div of Expression * Expression
    | Pow of Expression * Expression
    | Mod of Expression * Expression

// Calculates the result of expression in prefix notation
let rec calculate (e: Expression) =
    match e with
        | Number(x) -> x
        | Sum(e1, e2) -> calculate e1 + calculate e2
        | Sub(e1, e2) -> calculate e1 - calculate e2
        | Mul(e1, e2) -> calculate e1 * calculate e2
        | Div(e1, e2) -> calculate e1 / calculate e2
        | Pow(e1, e2) -> power (calculate e1) (calculate e2)
        | Mod(e1, e2) -> calculate e1 % calculate e2
