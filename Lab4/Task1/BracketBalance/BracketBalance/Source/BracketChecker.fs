module BracketBalanceChecker

open Stack

// Provides generic  method to check bracket balance in an expression
let checkGeneric (opening, closing, equals) =

    let rec checkCharListRec (stack : CharStack) = function
        | [] ->
            stack = Empty
        | h :: t ->
            if (equals h opening) then
                checkCharListRec (stack.Push h) t
            elif (equals h closing) then
                if not (stack = Empty) then
                    if (equals stack.Top opening) then
                        checkCharListRec (stack.Pop) t
                    else
                        false
                else
                    false
            else
                checkCharListRec stack t

    let checkString expression =
        checkCharListRec Empty (List.ofSeq expression)

    checkString
