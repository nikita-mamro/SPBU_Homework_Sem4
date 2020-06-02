module PrimeChecker

// Checks if number is prime
let isPrime n =
    if n <= 3 then
        n > 1
    else if n % 2 = 0 || n % 3 = 0 then
        false
    else
        let rec isPrimeRecursive n i =
            if i * i <= n then
                if n % i = 0 || n % (i + 2) = 0 then
                    false
                else
                    isPrimeRecursive n (i + 6)
            else
                true
        isPrimeRecursive n 5