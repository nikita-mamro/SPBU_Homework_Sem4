module WebScraperUtils

open System.Text.RegularExpressions

/// Checks if passed string is a valid url
let isValidUrl link =
    let pattern = "^((http\:\/\/|https\:\/\/|ftp\:\/\/)|(www.))(([a-zA-Z0-9\.-]+\.[a-zA-Z]{2,4})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(\/[a-zA-Z0-9%:\/-_\?\.'~]*)?$"
    Regex.Match(link, pattern).Success

/// Takes html markup and returns all links, which
/// can be reached via href, starting with 'http(s)://'
let getHrefPagesLinks html =
    let hrefPattern = @"href\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>\S+))"
    let regex = Regex(hrefPattern, RegexOptions.IgnoreCase)

    regex.Matches(html)
    |> Seq.map (fun (m: Match) -> m.Groups.[1].Value)
    |> Seq.filter (fun (href) -> isValidUrl href)
    |> Seq.map (fun (href) -> href.TrimEnd('/'))
    |> Seq.toList