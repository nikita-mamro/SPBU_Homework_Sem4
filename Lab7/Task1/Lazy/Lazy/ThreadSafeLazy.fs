module ThreadSafeLazy

open System

open Interfaces

/// Type implementing ILazy interface
/// This implementation guarantees
/// correct behaviour in multithread mode
type Lazy<'a> (supplier) =
    let lockObj = Object()
    let mutable obj = None

    interface ILazy<'a> with
        member this.Get () =
            match obj with
            | None ->
                lock lockObj (fun () ->
                    if obj.IsNone then
                        obj <- Some (supplier())
                        )
                obj.Value
            | Some value ->
                value
