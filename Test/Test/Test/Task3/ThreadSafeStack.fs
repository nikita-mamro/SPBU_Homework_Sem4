module ThreadSafeStack

open System

//Реализовать потокобезопасный стек. Стек должен иметь методы Push и TryPop,
//который возвращает Some <значение в вершине> или None, если стек пуст. Мутабельное состояние использовать можно.

let monitor = new Object()

type Stack<'a> =
    | Stack of 'a * Stack<'a>
    | Empty

    member s.Push e =
        lock monitor (fun() -> Stack(e, s))

    member s.TryPop =
        lock monitor (fun() ->
            match s with
            | Stack(e, _) -> Some(e)
            | Empty -> None)
