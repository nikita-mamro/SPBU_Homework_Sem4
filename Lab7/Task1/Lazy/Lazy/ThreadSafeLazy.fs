module ThreadSafeLazy

open System

open Interfaces

/// Type implementing ILazy interface
/// This implementation guarantees
/// correct behaviour in multithread mode
type Lazy<'a> (supplier) =
    let lockObj = Object()
    let mutable obj = None

    [<VolatileField>]
    let mutable isCalculated = false

    interface ILazy<'a> with
        member this.Get () =
            if not isCalculated then
                lock lockObj (fun () ->
                    if not isCalculated then
                        obj <- Some (supplier())
                        isCalculated <- true
                        )
            obj.Value
