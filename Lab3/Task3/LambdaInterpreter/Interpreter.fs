module Interpreter

type LambdaTerm =
    | Variable of char
    | Application of LambdaTerm * LambdaTerm
    | Abstraction of list<char> * LambdaTerm
    override this.ToString() =
        match this with
        | Variable name -> 
            name.ToString()
        | Application (f, arg) -> 
            sprintf "(%A %A)" f arg
        | Abstraction (parameters, body) -> 
            sprintf ("Л%A.%A") parameters body

let rec occurs name = function
    | Variable varName -> 
        varName = name
    | Application (f, arg) ->
        occurs name f || occurs name arg
    | Abstraction (parameters, body) ->
        List.contains name parameters || occurs name body

let rec occursFree name = function
    | Variable varName ->
        varName = name
    | Application (f, arg) ->
        occursFree name f || occursFree name arg
    | Abstraction (parameters, body) ->
        not (List.contains name parameters) && occursFree name body

// no double occurance support    
let alphaConvert oldName newName term =

    let rec convert oldName newName expression =
        match expression with
        | Variable name -> 
            if name = oldName then
                Variable(newName)
            else
                expression
        | Application (f, arg) ->
            Application(convert oldName newName f, 
                        convert oldName newName arg)
        | Abstraction(parameters, body) -> 
            Abstraction(parameters, convert oldName newName body)

    if (occurs newName term) then
        failwithf "%c already occurs" newName
    match term with
    | Abstraction(parameters, body) ->
        if not (List.contains oldName parameters) then
            failwithf "%c does not exist" oldName
            // need to update list of parameters
        Abstraction(parameters, convert oldName newName body)
    | _ -> failwith "Only abstraction can be alpha converted"
        
        

printfn "%A" <| Abstraction(['x'; 'y'], Application(Variable('x'), Variable('x'))).ToString()