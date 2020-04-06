module OperationSystem

type OS(name, infectionProbability) =
    member this.Name = name
    member this.InfectionProbability = infectionProbability