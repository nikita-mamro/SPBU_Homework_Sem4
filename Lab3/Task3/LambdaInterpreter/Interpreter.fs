module Interpreter

type VariablesSet = char

type LambdaTerm =
    | Variable of VariablesSet
    | Application of LambdaTerm * LambdaTerm
    | Abstraction of VariablesSet * LambdaTerm

    override this.ToString() = 
        match this with
        | Variable name ->
            name.ToString()
        | Application (f, arg) ->
            ["("; f.ToString(); " "; arg.ToString(); ")"] |> Seq.fold (+) ""
        | Abstraction (parameter, body) ->
            ["Л"; parameter.ToString(); "."; body.ToString()] |> Seq.fold (+) ""

let rec occurs name = function
    | Variable varName -> 
        varName = name
    | Application (f, arg) ->
        occurs name f || occurs name arg
    | Abstraction (parameter, body) ->
        name = parameter || occurs name body

let rec occursFree name = function
    | Variable varName ->
        varName = name
    | Application (f, arg) ->
        occursFree name f || occursFree name arg
    | Abstraction (parameter, body) ->
        name <> parameter && occursFree name body

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
        | Abstraction(parameter, body) ->
            if parameter = oldName then
                Abstraction(newName, convert oldName newName body)
            else
                Abstraction(parameter, convert oldName newName body)

    if (occursFree newName term) then
        failwithf "Error: New name %c occurs free in term" newName
    match term with
    | Abstraction(parameter, body) ->
        if oldName <> parameter then
            failwithf "Error: Old name %c does not exist" oldName
        Abstraction(newName, convert oldName newName body)
    | _ -> failwith "Error: Only abstraction can be alpha converted"
        
let rec substitute arg parameter body = 
    match body with
    | Variable name->
        if name = parameter then arg else body
    | Application (f, arg')->
         Application (substitute arg parameter f, substitute arg parameter arg')
    | Abstraction (parameter', body') -> 
        if parameter' = parameter then 
            body
        elif occursFree parameter' arg then
            // todo
            alphaConvert parameter' 'Ы' body'
        else
            Abstraction (parameter', substitute arg parameter body')

let betaReduction = function
    | Application (Abstraction (parameter, body), arg) ->
        substitute arg parameter body
    | expression -> failwithf "Error: %A is not a beta redex" expression

printfn "%A" <| (betaReduction (Application(Abstraction('x', Variable('x')), Abstraction('x', Variable('x'))))).ToString()