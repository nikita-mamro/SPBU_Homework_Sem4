module PrimeSequence

open PrimeChecker

// Generates sequence of prime numbers
let primeNumbers() =
    Seq.initInfinite (fun i -> i + 2)
    |> Seq.filter isPrime