module Interpreter

/// Type describing lambda term
type LambdaTerm =
    | Variable of char
    | Application of LambdaTerm * LambdaTerm
    | Abstraction of char * LambdaTerm

    override this.ToString() =
        match this with
        | Variable name ->
            name.ToString()
        | Application (f, arg) ->
            ["("; f.ToString(); ") ("; arg.ToString(); ")"] |> Seq.fold (+) ""
        | Abstraction (parameter, body) ->
            ["Л"; parameter.ToString(); ".("; body.ToString(); ")"] |> Seq.fold (+) ""

/// Checks if there's such name in the term
let rec occurs name = function
    | Variable varName ->
        varName = name
    | Application (f, arg) ->
        occurs name f || occurs name arg
    | Abstraction (parameter, body) ->
        name = parameter || occurs name body

/// Checks if name is a free variable in the term
let rec occursFree name = function
    | Variable varName ->
        varName = name
    | Application (f, arg) ->
        occursFree name f || occursFree name arg
    | Abstraction (parameter, body) ->
        name <> parameter && occursFree name body

/// Executes alpha convertion for a term
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
        invalidArg "newName" "Error: New name occurs free in term"
    match term with
    | Abstraction(parameter, body) ->
        if oldName <> parameter then
            invalidArg "oldName" "Error: Old name does not exist"
        Abstraction(newName, convert oldName newName body)
    | _ -> invalidArg "term" "Error: Only abstraction can be alpha converted"

/// Executes substitution of argument into body
let rec substitute arg parameter body =

    let rec variables term =
        seq {
            match term with
            | Variable name ->
                 yield name
            | Application (f, arg) ->
                 yield! variables f
                 yield! variables arg
            | Abstraction (parameter, body) ->
                yield parameter
                yield! variables body
        }

    match body with
    | Variable name->
        if name = parameter then arg else body
    | Application (f, arg')->
         (Application (substitute arg parameter f, substitute arg parameter arg'))
    | Abstraction (parameter', body') ->
        if parameter' = parameter then
            body
        elif occursFree parameter' arg then
            let currentVariables = variables body
            let alphabet = seq {'a' .. 'z'}

            let c = alphabet |> Seq.filter(fun x -> not (Seq.contains x currentVariables)) |> Seq.tryHead
            if c = None then
                failwithf "Error: unable to proceed beta reduction"

            substitute arg parameter (alphaConvert parameter' c.Value body)
        else
            Abstraction (parameter', substitute arg parameter body')

/// Executes beta reduction
let rec betaReduction term =
    match term with
    | Application (Abstraction (parameter, body), arg) ->
        betaReduction <| substitute arg parameter body
    | Application (f, arg) ->
        Application(betaReduction f, betaReduction arg)
    | Abstraction (parameter, body) ->
        Abstraction(parameter, betaReduction body)
    | Variable _ ->
        term
