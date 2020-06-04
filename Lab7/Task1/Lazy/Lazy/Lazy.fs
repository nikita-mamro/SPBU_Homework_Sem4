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
            if not isCalculated then
                obj <- Some (supplier())
                isCalculated <- true

            obj.Value