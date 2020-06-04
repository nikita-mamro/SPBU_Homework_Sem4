module LazyFactory

/// Factory generating lazy objects
type LazyFactory<'a> () =
    member this.CreateLazy supplier =
        SingleThreadLazy.Lazy(supplier)

    member this.CreateThreadSafeLazy supplier =
        ThreadSafeLazy.Lazy(supplier)

    member this.CreateLockFreeLazy supplier =
        LockFreeLazy.Lazy(supplier)