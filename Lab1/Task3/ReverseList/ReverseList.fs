module ReverseList

let reverseList list =
    let rec reverseRecursive acc = function
        | [] -> acc
        | head :: tail -> reverseRecursive (head :: acc) tail
    reverseRecursive [] list

printfn "%A" (reverseList [1; 2; 3; 4])