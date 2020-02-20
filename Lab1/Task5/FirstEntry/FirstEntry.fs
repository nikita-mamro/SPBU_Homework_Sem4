module FirstEntry

let firstEntry x list =
    let rec firstEntryRecursive list acc = 
        match list with
        | head :: tail ->
                if head = x then
                    Some(acc)
                else 
                    firstEntryRecursive tail (acc + 1)
        | [] -> None
    firstEntryRecursive list 0