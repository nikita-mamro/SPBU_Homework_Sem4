module Facrtorial.Tests

open NUnit.Framework
open Factorial

[<TestCase (0, 1)>]
[<TestCase (1, 1)>]
[<TestCase (2, 2)>]
[<TestCase (5, 120)>]
[<TestCase (10, 3628800)>]
let FactorialTest (input, expected) =
    Assert.AreEqual(Some(expected), factorial input)

[<TestCase (-1)>]
[<TestCase (-10000000)>]
let FactorialNoneTest (input) = 
    Assert.AreEqual(None, factorial input)
