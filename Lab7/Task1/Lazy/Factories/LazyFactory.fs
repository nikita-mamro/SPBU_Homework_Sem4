module LazyFactory

type LazyFactory<'a> () =
    let mutable a = false

    member this.CreateLazy supplier =
        SingleThreadLazy.Lazy(supplier)

    member this.CreateThreadSafeLazy supplier =
        ThreadSafeLazy.Lazy(supplier)

    member this.CreateLockFreeLazy supplier =
        LockFreeLazy.Lazy(supplier)