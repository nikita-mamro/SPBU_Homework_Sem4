module Computer

open OperatingSystem

type Computer (id, os : OS, isInfected : bool) =
    let mutable _isInfected = isInfected

    member this.Id = id
    member this.OS = os
    member this.IsInfected
        with get () = _isInfected
        and set (value) = _isInfected <- value
