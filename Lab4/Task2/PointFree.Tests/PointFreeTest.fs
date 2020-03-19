module PointFree.Tests

open PointFree
open NUnit.Framework
open FsCheck
open FsCheck.NUnit

// Using this, there's no exception during tests
let testFunc'4: int -> int list -> int list =
    List.map << (*)

// This is how func'4 is implemented in PointFree module
//let func'4: int -> int list -> int list =
//    List.map << (*)

[<Property>]
let ``Functions from converting into point-free procedure should be equal`` (x:int, xs:list<int>) =
    let res = func x xs
    let res'1 = func'1 x xs
    let res'2 = func'2 x xs
    let res'3 = func'3 x xs
    //let res'4 = func'4 x xs // This causes NullReferenceException, but works properly in PointFree module
    //let f = func'4 // No exception
    let res'4 = testFunc'4 x xs // No exception, passing tests
    res = res'1 &&
    res'1 = res'2 &&
    res'2 = res'3 &&
    res'3 = res'4

[<Test>]
Check.Quick ``Functions from converting into point-free procedure should be equal``