module PriorityQueue

/// Priority queue structure
/// Stores pairs (value, priority)
type PriorityQueue<'a>() =
    /// List of stored pairs
    let mutable list = []

    /// Enqueues item with given priority
    member this.Enq value priority =
        ""

    /// Dequeues item with highest priority
    member this.Deq () =
        match list with
        | [] ->
            invalidOp("Can't dequeue from empty queue.")
        | h :: t ->
            list <- t
            h