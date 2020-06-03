module BracketBalanceChecker

/// Provides generic  method to check bracket balance in an expression
let checkGeneric bracketPairs =

    let isOpening bracket =
        bracketPairs |> List.unzip |> fst |> List.contains bracket

    let isClosing bracket =
        bracketPairs |> List.unzip |> snd |> List.contains bracket

    let isPair left right =
        bracketPairs |> List.contains (left, right)

    let rec checkExpressionRec (list : list<'a>) = function
        | [] ->
            list.IsEmpty
        | h :: t ->
            if (isOpening h) then
                checkExpressionRec (h :: list) t
            elif (isClosing h) then
                match list with
                | [] ->
                    false
                | top :: tail ->
                    if (isPair top h) then
                        checkExpressionRec tail t
                    else
                        false
            else
                checkExpressionRec list t

    let checkExpression expression =
        checkExpressionRec [] (List.ofSeq expression)

    checkExpression