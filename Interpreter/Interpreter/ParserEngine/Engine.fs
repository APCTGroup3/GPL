namespace ParserEngine
module Engine =
    open System
    open Context
    open System.Collections.Generic
    open System.Collections
    open CoreParser.Parser.AST
    open CoreParser

    type Engine() = 
        let mutable Stack : Collections.Generic.Stack<Context> = new Collections.Generic.Stack<Context>() //Variable stack
        do Stack.Push(new Context())

        member this.Visit_Terminal (node: Node) : Terminal =
            let tokenType = node.Token.constType
            let rawVal = node.Token.token
            let value : Terminal = match tokenType with
                                   | ConstTypes.number -> new Number(Double.Parse(rawVal)) :> Terminal
                                   | ConstTypes.boolean -> new Boolean(bool.Parse(rawVal)) :> Terminal
                                   | ConstTypes.str -> new Str(rawVal) :> Terminal
                                   | _ -> failwith "Unknown token type"
            value

        member this.Visit_UnaryOp (node: Node) : Terminal =
            let op = node.Token.token
            let eval = this.Visit(node.Children().[0])
            let value = match op with
                        | "-" -> match eval with
                                 | :? Number -> new Number(-(eval.ToDouble())) :> Terminal
                                 | :? Boolean -> new Boolean(not (eval.ToBool())) :> Terminal
                                 | _ -> failwithf "Operator %s cannot be applied to %s" op eval.TypeName
                        | "+" -> match eval with 
                                 | :? Number -> eval
                                 | :? Boolean -> eval
                                 | _ -> failwithf "Operator %s cannot be applied to %s" op eval.TypeName
                        | _ -> failwithf "Unknown unary operator %s" op
            value


        member this.Visit_BinaryOp (node: Node) : Terminal =
            let op = node.Token.token
            let leftEval = this.Visit(node.Children().[0])
            let rightEval = this.Visit(node.Children().[1])

            let RaiseOpError(op: String, type1: String, type2: String) =
                failwithf "Operator %s is not compatible with types (%s, %s)" op type1 type2

            let value = 
                match op with
                | "+"  -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() + rightEval.ToDouble()) :> Terminal
                          | (:? Str, _) -> new Str(String.concat "" [leftEval.ToStr(); rightEval.ToStr()]) :> Terminal
                          | ( _, :? Str) -> new Str(String.concat "" [leftEval.ToStr(); rightEval.ToStr()]) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | "-" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() - rightEval.ToDouble()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | "*" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() * rightEval.ToDouble()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | "/" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() / rightEval.ToDouble()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | "^" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() ** rightEval.ToDouble()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | "<" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() < rightEval.ToDouble()) :> Terminal
                          | _ -> raise (Exception(""))
                | ">" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() > rightEval.ToDouble()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | "<=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() <= rightEval.ToDouble()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | ">=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() >= rightEval.ToDouble()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | "==" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() = rightEval.ToDouble()) :> Terminal
                          | (:? Boolean, :? Boolean) -> new Boolean(leftEval.ToBool() = rightEval.ToBool()) :> Terminal
                          | (:? Str, :? Str) -> new Boolean(leftEval.ToStr() = rightEval.ToStr()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | "!=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() <> rightEval.ToDouble()) :> Terminal
                          | (:? Boolean, :? Boolean) -> new Boolean(leftEval.ToBool() <> rightEval.ToBool()) :> Terminal
                          | (:? Str, :? Str) -> new Boolean(leftEval.ToStr() <> rightEval.ToStr()) :> Terminal
                          | _ -> RaiseOpError(op, leftEval.TypeName, rightEval.TypeName)
                | _ -> failwithf "Unknown operator %s" op
            value

        member this.Visit_Assign (node: Assignment) =
            let id = node.ID.Token.token
            let value = this.Visit(node.Value)
            Stack.Peek().Store(id, value)
            let res = new Void()
            res :> Terminal

        member this.Visit_AssignArrayElement (node: ArrayAssignment) =
            let id = node.ID.Token.token;
            let element = this.Visit(node.Element)
            let fullId = id + "." + element.ToStr()
            let value = this.Visit(node.Value)
            Stack.Peek().Delete(id) |> ignore
            Stack.Peek().Store(fullId, value)
            new Void() :> Terminal

        member this.Visit_Variable (node: Node) =
            let id = node.Token.token
            let value = Stack.Peek().Get(id)
            value

        member this.Visit_ArrayElement (node: ArrayElement) =
            let id = node.Token.token;
            let element = this.Visit(node.Element);
            let fullId = id + "." + element.ToStr()
            let value = Stack.Peek().Get(fullId)
            value

        member this.Visit_If(node: IfNode) =
            let cond = node.Condition
            let statements = node.Statements
            let elseStatements = node.ElseStatements
            let res = match this.Visit(cond).ToBool() with
                      | true -> this.Visit(statements)
                      | false -> this.Visit(elseStatements)
            res

        member this.Visit_Function(node: FunctionNode) =
            let nodeParams = node.Parameters
            let parameters = Array.init nodeParams.Count (fun i -> this.Visit(nodeParams.[i]))
            let name = node.Token.token
            let result = Parser.StandardLibrary.StandardLibrary.Run(name, parameters)
            result

        member this.Visit_While(node:WhileNode) =
            let block = node.Block
            let condition = node.Condition
            while this.Visit(condition).ToBool() do
                this.Visit(block) |> ignore
            new Void() :> Terminal

        member this.Visit_Block(node:BlockNode) =
            let statements = node.Statements
            for statement in statements do
                this.Visit(statement) |> ignore
            new Void() :> Terminal

        member this.Visit (node: Node) : Terminal =
            let value = match node with
                        | :? TerminalNode -> this.Visit_Terminal(node)
                        | :? BinaryOp -> this.Visit_BinaryOp(node)
                        | :? UnaryNode -> this.Visit_UnaryOp(node)
                        | :? Assignment -> this.Visit_Assign(node:?>Assignment)
                        | :? ArrayAssignment -> this.Visit_AssignArrayElement(node:?>ArrayAssignment)
                        | :? Variable -> this.Visit_Variable(node)
                        | :? ArrayElement -> this.Visit_ArrayElement(node:?>ArrayElement)
                        | :? IfNode -> this.Visit_If(node:?>IfNode)
                        | :? FunctionNode -> this.Visit_Function(node:?>FunctionNode)
                        | :? WhileNode -> this.Visit_While(node:?>WhileNode)
                        | :? BlockNode -> this.Visit_Block(node:?>BlockNode)
                        | _ -> failwithf "Parser Error"
            value

        member this.Run (node: Node) : Terminal =
            this.Visit (node)