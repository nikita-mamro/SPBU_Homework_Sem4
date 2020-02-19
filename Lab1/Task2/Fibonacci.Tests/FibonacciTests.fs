module Fibonacci.Tests

open NUnit.Framework
open Fibonacci

[<TestCase (0, 0)>]
[<TestCase (1, 1)>]
[<TestCase (2, 1)>]
[<TestCase (3, 2)>]
[<TestCase (4, 3)>]
[<TestCase (5, 5)>]
let FibonacciTest (input, expected) =
    Assert.AreEqual(Some(expected), fibonacci input)

[<TestCase (-1)>]
[<TestCase (-1000000)>]
let FibonacciNoneTest (input) =
    Assert.AreEqual(None, fibonacci input)
