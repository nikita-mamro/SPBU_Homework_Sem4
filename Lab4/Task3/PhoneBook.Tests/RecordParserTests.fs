module RecordParserTests

open NUnit.Framework
open FsUnit
open RecordParser

[<Test>]
let ``Parsing valid expression with short expressions`` () =
    (parse "123 Jo").Value |> should equal ("Jo", "123")

[<Test>]
let ``Parsing valid expression with long expressions`` () =
    (parse "8-800-555-35-35 J Jo Joh Hohn").Value |> should equal ("J Jo Joh Hohn", "8-800-555-35-35")

[<Test>]
let ``Parsing invalid expression`` () =
    parse "" |> should equal None
