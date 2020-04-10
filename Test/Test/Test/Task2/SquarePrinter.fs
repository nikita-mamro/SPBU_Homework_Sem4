module SquarePrinter

//Описать функцию, которая для данного числа n печатает квадрат из * со стороной n. Например, для n = 4 надо напечатать:
//****
//*  *
//*  *
//****
//Конструкции императивного программирования использовать нельзя.

let getSquare n =
    let rec getLine (list : list<list<string>>) numberOfLine =

        let rec getFull list acc =
            if acc < n then
                getFull (List.append list ["*"]) (acc + 1)
            else
                list

        let rec getEmpty list acc =
            if acc < n then
                if acc = 0 || acc = n - 1 then
                    getEmpty (List.append list ["*"]) (acc + 1)
                else
                    getEmpty (List.append list [" "]) (acc + 1)
            else
                list

        if numberOfLine < n then
            if (numberOfLine = 0 || (numberOfLine = n - 1)) then
                getLine ((getFull [] 0) :: list) (numberOfLine + 1)
            else
                getLine ((getEmpty [] 0) :: list) (numberOfLine + 1)
        else
            list

    getLine [] 0

let printSquare n =
    let rec printSquareRec = function
        | h :: t ->
            printfn "%A" h
            printSquareRec t
        | _ ->
            printfn
    printSquareRec (getSquare n)

printSquare 3 |> ignore
