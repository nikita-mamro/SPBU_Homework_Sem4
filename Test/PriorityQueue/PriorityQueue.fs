module PriorityQueue

/// Priority queue structure
/// Stores pairs (value, priority)
type PriorityQueue<'a>() =
    /// List of stored pairs
    let mutable list = []

    /// Enqueues item with given priority
    member this.Enq value priority =
        let rec insert before after =
            match after with
            | [] ->
                [(value, priority)]
            | (v, p) :: t ->
                if p < priority then
                    before @ [(value, priority)] @ after
                else
                    insert (before @ [(v, p)]) t
        list <- insert [] list

    /// Dequeues item with highest priority
    member this.Deq () =
        match list with
        | [] ->
            invalidOp("Can't dequeue from empty queue.")
        | h :: t ->
            list <- t
            fst h

    member this.Count = list.Length