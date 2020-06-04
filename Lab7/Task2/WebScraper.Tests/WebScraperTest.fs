module WebScraper.Tests

open System

open WebScraper
open WebScraperUtils

open NUnit.Framework
open FsUnit

/// Some links in invalid format to check validation exceptions
let invalidLinks () =
    [
        "foo"
        "foo.com"
        "www.foo.123"
        "http://https://foo.com"
        "http://http://foo.com"
    ] |> Seq.map (fun (link) -> TestCaseData(link))

/// Some of hwproj course href links
let hwprojHrefs () =
    [
        [
            "http://fsharp.org"
            "http://fsharpforfunandprofit.com"
            "https://dotnetfiddle.net"
            "https://www.papeeria.com"
        ]
    ] |> Seq.map (fun (links) -> TestCaseData(links))

/// Tests for web scraper

[<TestCaseSource("invalidLinks")>]
let ``Invalid links shouldn't pass validation`` (link) =
    isValidUrl link |> should equal false

[<TestCaseSource("invalidLinks")>]
let ``Passing invalid links should throw exception`` (link) =
    (fun () -> getHrefPagesInfo link |> ignore) |> should throw typeof<ArgumentException>

[<Test>]
let ``Scraping markup with some valid hrefs should recognize links`` () =
    let markup = "<!DOCTYPE html><html><body><p>Some links</p>"
               + "<a href=\"https://www.website.com\">This is a valid link</a>"
               + "<a href=\"press\">Some text</a>"
               + "<a href=\"http://www.somesite.ru\">This is a valid link</a>"
               + "</body></html>"

    let links = getHrefPagesLinks markup

    links |> should equal ["https://www.website.com"; "http://www.somesite.ru"]

[<Test>]
let ``Scraping markup without valid hrefs shouldn't return any links`` () =
    let markup = "<!DOCTYPE html><html><body><p>Some links</p>"
               + "<a href=\"link\">This is a valid link</a>"
               + "<a href=\"press\">Some text</a>"
               + "<a href=\"pressagain\">This is a valid link</a>"
               + "</body></html>"

    getHrefPagesLinks markup |> should be Empty

[<TestCaseSource("hwprojHrefs")>]
let ``Scraping real website with some valid hrefs should as expected`` (links) =
    match getHrefPagesInfo "http://hwproj.me/courses/34" with
    | Some hrefs ->
        for link in links do
            hrefs |> Seq.map (fun (href, _) -> href) |> Seq.contains link |> should equal true
    | None ->
        ()
