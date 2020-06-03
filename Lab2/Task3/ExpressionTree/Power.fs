module Power

// Calculates power of n with the exponent m
let rec power n m =
    if m < 0 then
        invalidArg "Exponent" "Negative exponent in the power function"
    else if m = 0 then
        1
    else if m % 2 = 0 then
        power (n * n) (m / 2)
    else
        n * power n (m - 1)