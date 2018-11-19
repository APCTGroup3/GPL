namespace ParserEngine
module Engine =
    open System
    open Types
    open Context
    open System.Collections.Generic
    open System.Collections
    open CoreParser
    open CoreParser.Parser

    type Engine() = 
        let mutable Stack : Collections.Generic.Stack<Context> = new Collections.Generic.Stack<Context>() //Variable stack
        do Stack.Push(new Context())

        member this.Visit_TerminalNode (node: CoreParser.Parser.Node) : Terminal =
            let tokenType = node.Token.constType
            let rawVal = node.Token.token
            let value : Terminal = match tokenType with
                                   | ConstTypes.number -> new Number(Double.Parse(rawVal)) :> Terminal
                                   | ConstTypes.boolean -> new Boolean(bool.Parse(rawVal)) :> Terminal
                                   | ConstTypes.str -> new Str(rawVal) :> Terminal
            value

        member this.Visit_UnaryOp (node: CoreParser.Parser.Node) : Terminal =
            let op = node.Token.token
            let eval = this.Visit(node.Children().[0])
            let value = match op with
                        | "-" -> match eval with
                                 | :? Number -> new Number(-(eval.ToDouble())) :> Terminal
                                 | :? Boolean -> new Boolean(not (eval.ToBool())) :> Terminal
                        | "+" -> match eval with 
                                 | :? Number -> eval
                                 | :? Boolean -> eval
            value


        member this.Visit_BinaryOp (node: CoreParser.Parser.Node) : Terminal =
            let op = node.Token.token
            let leftEval = this.Visit(node.Children().[0])
            let rightEval = this.Visit(node.Children().[1])

            let value = 
                match op with
                | "+" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() + rightEval.ToDouble()) :> Terminal
                          | (:? Str, _) -> new Str(String.concat "" [leftEval.ToString; rightEval.ToString]) :> Terminal
                          | ( _, :? Str) -> new Str(String.concat "" [leftEval.ToString; rightEval.ToString]) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | "-" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() - rightEval.ToDouble()) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | "*" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() * rightEval.ToDouble()) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | "/" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() / rightEval.ToDouble()) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | "^" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() ** rightEval.ToDouble()) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | "<" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() < rightEval.ToDouble()) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | ">" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() > rightEval.ToDouble()) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | "<=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() <= rightEval.ToDouble()) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | ">=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() >= rightEval.ToDouble()) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | "==" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() = rightEval.ToDouble()) :> Terminal
                          | (:? Boolean, :? Boolean) -> new Boolean(leftEval.ToBool() = rightEval.ToBool()) :> Terminal
                          | (:? Str, :? Str) -> new Boolean(leftEval.ToString = rightEval.ToString) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | "!=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() <> rightEval.ToDouble()) :> Terminal
                          | (:? Boolean, :? Boolean) -> new Boolean(leftEval.ToBool() <> rightEval.ToBool()) :> Terminal
                          | (:? Str, :? Str) -> new Boolean(leftEval.ToString <> rightEval.ToString) :> Terminal
                          | _ -> failwithf "Operator %s is not compatible with types (%s, %s)" op leftEval.TypeName rightEval.TypeName
                | _ -> failwithf "Unknown operator %s" op
            value

        member this.Visit_Assign (node: CoreParser.Parser.Assignment) =
            let id = node.ID.Token.token
            let value = this.Visit(node.Value)
            Stack.Peek().Store(id, value)
            let res = new Void()
            res :> Terminal

        member this.Visit_Variable (node: CoreParser.Parser.Node) =
            let id = node.Token.token
            let value = Stack.Peek().Get(id)
            value

        member this.Visit (node: CoreParser.Parser.Node) : Terminal =
            let value = match node with
                        | :? Parser.Ops.TerminalNode -> this.Visit_TerminalNode(node)
                        | :? Parser.Ops.BinaryOp -> this.Visit_BinaryOp(node)
                        | :? Parser.UnaryNode -> this.Visit_UnaryOp(node)
                        | :? Parser.Assignment -> this.Visit_Assign(node:?>Assignment)
                        | :? Parser.Variable -> this.Visit_Variable(node)
                        | _ -> failwithf "Parser Error"
            value

        member this.Run (node: CoreParser.Parser.Node) : Terminal =
            this.Visit (node)