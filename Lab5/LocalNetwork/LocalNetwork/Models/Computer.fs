module Computer

open OperatingSystem

/// Class describing computer
type Computer (id, os : OS, isInfected : bool) =
    let mutable isInfected = isInfected

    member this.Id = id
    member this.OS = os
    member this.IsInfected
        with get () = isInfected
        and set (value) = isInfected <- value
