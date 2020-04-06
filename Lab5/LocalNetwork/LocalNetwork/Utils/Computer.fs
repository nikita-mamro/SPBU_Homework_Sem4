module Computer

type Computer(id, os, isInfected) =
    member this.Id = id
    member this.Os = os
    member this.IsInfected = isInfected