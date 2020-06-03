module Computer

open System

open OperatingSystem

type Computer (os : OS, isInfected) =
    let mutable _isInfected = isInfected

    member this.Id = Guid.NewGuid()
    member this.OS = os
    member this.IsInfected
        with get () = _isInfected
        and set (value) = _isInfected <- value
