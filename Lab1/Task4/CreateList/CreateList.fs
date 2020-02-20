module CreateList

let createList n m =
    let rec power n m =
        match m with
        | 0 -> 1
        | 1 -> n
        | 2 -> n * n
        | _ -> (power n (m / 2)) * (power n (m / 2)) * (power n (m % 2))
    
    let rec createRecursive list n m =
        if m = -1 then
            list
        else
            createRecursive (power 2 (n + m) :: list) n (m - 1)

    if n < 0 || m < 0 then
        None
    else
        Some(createRecursive [] n m)