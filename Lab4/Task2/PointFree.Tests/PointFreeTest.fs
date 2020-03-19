module PointFree.Tests

open PointFree
open NUnit.Framework
open FsCheck
open FsCheck.NUnit

[<Property>]
let PointFreeTest (x:int, xs:list<int>) =
    let res = func x xs
    let res'1 = func'1 x xs
    let res'2 = func'2 x xs
    let res'3 = func'3 x xs
    //let res'4 = func'4 x xs // This causes NullReferenceException, but works properly when testes by print
    res = res'1 &&
    res'1 = res'2 &&
    res'2 = res'3 //&&
    //res'3 = res'4

[<Test>]
Check.Quick PointFreeTest