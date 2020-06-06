open LambdaParser

[<EntryPoint>]
let main argv =
    let testInput = "\x y z.x z x"

    printfn "%A" <| Parser.Parse testInput

    0 // return an integer exit code
