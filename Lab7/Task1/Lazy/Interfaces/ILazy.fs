module Interfaces

type ILazy<'a> =
    abstract member Get: unit -> 'a