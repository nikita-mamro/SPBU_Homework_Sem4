module ListMin

//Task:
//Найти наименьший элемент в списке, не используя рекурсию и List.min.
//Конструкции императивного программирования использовать нельзя.

let min list =
    match list with
    | [] ->
        failwith "Empty list"
    | h :: t ->
        List.fold (fun x y -> if x < y then x else y) h list