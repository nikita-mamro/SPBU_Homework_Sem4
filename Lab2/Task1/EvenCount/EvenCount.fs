module EvenCount

// Function counting even numbers in a list using map() 
let evenCountMap list = 
    list |> Seq.map (fun x -> abs(x + 1) % 2) |> Seq.sum

// Function counting even numbers in a list using filter()
let evenCountFilter list =
    list |> Seq.filter (fun x -> x % 2 = 0) |> Seq.length

// Function counting even numbers in a list using fold()
let evenCountFold list = 
    list |> Seq.fold (fun acc x -> acc + abs(x + 1) % 2) 0