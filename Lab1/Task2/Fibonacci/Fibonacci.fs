module Fibonacci

let fibonacci n =
    let rec calculateFibonacci n i x y = 
        if n < 0 then
            None
        else if i = n then
            Some(y)
        else
            calculateFibonacci n (i + 1) (x + y) x
    calculateFibonacci n 0 1 0

        
        
