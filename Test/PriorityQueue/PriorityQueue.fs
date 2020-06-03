module PriorityQueue

/// Priority queue structure
/// Stores pairs (value, priority)
type PriorityQueue<'a>() =
    /// List of stored pairs
    let mutable items = []

    /// Enqueues item with given priority
    member this.Enq (value : 'a) priority =
        let rec insert before after =
            match after with
            | [] ->
                before @ [(value, priority)]
            | (v, p) :: t ->
                if p < priority then
                    before @ [(value, priority)] @ after
                else
                    insert (before @ [(v, p)]) t

        items <- insert [] items

    /// Dequeues item with highest priority
    member this.Deq () =
        match items with
        | [] ->
            invalidOp("Can't dequeue from empty queue.")
        | h :: t ->
            items <- t
            fst h

    member this.Count = items.Length
