module Interfaces

/// Interface of lazy object
type ILazy<'a> =
    abstract member Get: unit -> 'a