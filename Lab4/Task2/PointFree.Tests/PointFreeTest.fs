module PointFree.Tests

open PointFree
open NUnit.Framework
open FsCheck

// Tests for equality of all functions from point-free convertion procedure steps
[<Test>]
let ``Check func'1`` () =
    Check.Quick (fun x xs -> (func x xs) = (func'1 x xs))

[<Test>]
let ``Check func'2`` () =
    Check.Quick (fun x xs -> (func x xs) = (func'2 x xs))
                                                 
[<Test>]                                         
let ``Check func'3`` () =                           
    Check.Quick (fun x xs -> (func x xs) = (func'3 x xs))
                                                 
[<Test>]                                         
let ``Check func'4`` () =                           
    Check.Quick (fun x xs -> (func x xs) = (func'4 x xs))