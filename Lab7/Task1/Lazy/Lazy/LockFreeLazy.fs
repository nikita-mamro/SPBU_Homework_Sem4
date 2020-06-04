module LockFreeLazy

open Interfaces
open System.Threading

/// Type implementing ILazy interface
/// This implementation guarantees
/// correct behaviour in multithread mode
/// Lock-Free version of ThreadSafeLazy
type Lazy<'a> (supplier) =
    let mutable obj = None
    let mutable isCalculated = false

    interface ILazy<'a> with
        member this.Get () =
            if not isCalculated then
                let start = obj
                let res = Some (supplier())
                // Thinking over it
                obj <- Interlocked.CompareExchange(ref obj, res, start)
                isCalculated <- true

            obj.Value
