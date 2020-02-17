module  Factorial

let factorial n =
    let rec recursiveFactorial n acc =
        if n < 0 then
            None
        else if n = 0 || n = 1 then
            Some(acc)
        else
            recursiveFactorial (n - 1) (acc * n)

    recursiveFactorial n 1