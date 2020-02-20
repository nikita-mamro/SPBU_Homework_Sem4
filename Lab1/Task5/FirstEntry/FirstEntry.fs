module FirstEntry

let firstEntry x list =
    let rec firstEntryRecursive x list acc = 
        match list with
        | head :: tail ->
                if head = x then
                    Some(acc)
                else 
                    firstEntryRecursive x tail (acc + 1)
        | [] -> None
    firstEntryRecursive x list 0