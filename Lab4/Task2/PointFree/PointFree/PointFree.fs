module PointFree

// Procedure of turning function
// into function in point-free style
let func x l = List.map (fun y -> y * x) l

let func'1 x: int list -> int list =
    List.map (fun y -> y * x)

let func'2 x: int list -> int list =
    List.map (fun y -> (*) x y)

let func'3 x: int list -> int list =
    List.map ((*) x)

let func'4: int -> int list -> int list =
    List.map << (*)
