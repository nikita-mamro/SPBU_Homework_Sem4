module OperatingSystem

open System

/// Class describing operation system with certain chance of infection
type OS (infectionProbability : float) =
    do
        if infectionProbability < 0.0 || (infectionProbability - 1.0) > (float)Single.Epsilon then
            invalidArg "infectionProbability" (sprintf "Infection probabilty must be from 0 to 1")

    member this.InfectionProbability = infectionProbability