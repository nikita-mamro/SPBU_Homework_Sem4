module WebScraper

open WebScraperUtils

open System
open System.Net
open System.IO

/// Takes page's url and gets information abot each website
/// it has href link to, starting with 'http(s)://'
/// Returns list of pairs (link - html markup/None if can't reach link)
/// Returns None if base website can't be reached
let getHrefPagesInfo url =

    if not (isValidUrl url) then
        invalidArg "url" (sprintf "Passed link is not a valid url")

    let getDocumentAsync url =
        async {
            try
                let req = WebRequest.Create(Uri(url))
                use! resp = req.AsyncGetResponse()
                use stream = resp.GetResponseStream()
                use reader = new StreamReader(stream)
                let html = reader.ReadToEnd()
                return (url, Some html)
            with
                | _ ->
                    return (url, None)
        }

    let getAllHrefLinks url =
        let baseDocInfo =
            getDocumentAsync url
            |> Async.RunSynchronously

        match baseDocInfo with
        | (_, Some html) ->
            Some (getHrefPagesLinks html)
        | (_, None) ->
            None

    match getAllHrefLinks url with
    | Some links ->
        Some (links
        |> List.map getDocumentAsync
        |> Async.Parallel
        |> Async.RunSynchronously)
    | None ->
        None

/// Takes page's url and prints information abot each website
/// it has href link to, starting with 'http(s)://'
/// Information is printed in format: 'link - page size'
let printHrefsInfo url =

    let getInfo (reports: (string * string option) []) =
        if (reports.Length = 0) then
            printfn "No results found"

        for report in reports do
            match report with
            | (link, Some html) ->
                printfn "%s - %d symbols" link html.Length
            | (link, None) ->
                printfn "%s - can't reach page" link

    match getHrefPagesInfo url with
    | Some reports ->
        getInfo reports
    | None ->
        printfn "Can't reach %s" url
