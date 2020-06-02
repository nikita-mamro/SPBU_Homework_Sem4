module BTreeMap

type Tree<'a> =
    | Node of Tree<'a> * 'a * Tree<'a>
    | Empty

// Returns a copy of a binary tree
// applying f to each tree element
let rec mapTree f = function
    | Node(l, d, r) -> Node(mapTree f l, f d, mapTree f r)
    | Empty -> Empty