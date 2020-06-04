module LazyFactory

open Interfaces

/// Factory generating lazy objects
type LazyFactory<'a> =
    static member CreateLazy supplier =
        SingleThreadLazy.Lazy(supplier) :> ILazy<'a>

    static member CreateThreadSafeLazy supplier =
        ThreadSafeLazy.Lazy(supplier) :> ILazy<'a>

    static member CreateLockFreeLazy supplier =
        LockFreeLazy.Lazy(supplier) :> ILazy<'a>
