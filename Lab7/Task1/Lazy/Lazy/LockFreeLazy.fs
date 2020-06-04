module LockFreeLazy

open Interfaces

open System.Threading

/// Type implementing ILazy interface
/// This implementation guarantees
/// correct behaviour in multithread mode
/// Lock-Free version of ThreadSafeLazy
type Lazy<'a> (supplier) =
    let mutable obj = None

    interface ILazy<'a> with
        member this.Get () =
            while obj.IsNone do
                let desiredVal = Some (supplier())
                Interlocked.CompareExchange(ref obj, desiredVal, None) |> ignore
            obj.Value
