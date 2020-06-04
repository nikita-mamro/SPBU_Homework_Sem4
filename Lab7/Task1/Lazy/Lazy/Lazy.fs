module SingleThreadLazy

open Interfaces

/// Type implementing ILazy interface
/// This implementation does not guarantee
/// correct behaviour in multithread mode
type Lazy<'a> (supplier) =
    let mutable obj = None
    let mutable isCalculated = false

    interface ILazy<'a> with
        member this.Get () =
            match obj with
            | None ->
                obj <- Some (supplier())
                obj.Value
            | Some value ->
                value