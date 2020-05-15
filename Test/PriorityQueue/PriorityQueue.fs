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
                if p > priority then
                    insert (before @ [(v, p)]) after
                else
                    before @ [(value, priority)] @ after
        list <- insert [] list

    /// Dequeues item with highest priority
    member this.Deq () =
        match list with
        | [] ->
            invalidOp("Can't dequeue from empty queue.")
        | h :: t ->
            list <- t
            h

    member this.Count = list.Length