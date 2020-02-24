module CreateList

let createList n m =
    let rec power n m =
        match m with
        | 0 -> 1
        | 1 -> n
        | _ -> n * power n (m - 1)

    let rec createRecursive list n m =
        if m = -1 then
            list
        else
            createRecursive (n :: list) (n / 2) (m - 1)

    if n < 0 || m < 0 then
        None
    else
        Some(createRecursive [] (power 2 (n + m)) m)