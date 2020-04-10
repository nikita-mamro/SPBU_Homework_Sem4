module ListMin

//Task:
//Найти наименьший элемент в списке, не используя рекурсию и List.min.
//Конструкции императивного программирования использовать нельзя.

let min = function
    | [] -> failwith "Empty list"
    | head :: tail ->
        let rec recMin currentMin = function
            | [] -> currentMin
            | h :: t ->
                if h < currentMin then
                    recMin h t
                else
                    recMin currentMin t
        recMin head tail
