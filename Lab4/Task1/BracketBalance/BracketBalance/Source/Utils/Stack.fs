module Stack

// Implements immutable stack
type CharStack =
    | Stack of char * CharStack
    | Empty

    // Returns a stack after pushing an element
    member s.Push e = Stack(e, s)

    // Returns element from the top of the stack
    member s.Top =
        match s with
        | Stack(e, stack) -> e
        | Empty -> invalidOp "Top() error: Stack is empty"

    // Returns a stack after popping an element
    member s.Pop =
        match s with
        | Stack(e, stack) -> stack
        | Empty -> invalidOp "Pop() error: Stack is empty"
