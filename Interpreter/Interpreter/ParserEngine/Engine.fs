namespace ParserEngine
module Engine =
    open System
    open Types
    open CoreParser

    type Engine() = 
        member this.Visit_TerminalNode (node: CoreParser.Parser.Node) : Terminal =
            let tokenType = node.Token.constType
            let rawVal = node.Token.token
            let value : Terminal = match tokenType with
                                   | ConstTypes.number -> new Number(Double.Parse(rawVal)) :> Terminal
                                   | ConstTypes.boolean -> new Boolean(bool.Parse(rawVal)) :> Terminal
                                   | ConstTypes.str -> new String(rawVal) :> Terminal
            value

        member this.Visit_BinaryOp (node: CoreParser.Parser.Node) : Terminal =
            let op = node.Token.token
            let leftEval = this.Visit(node.Children().[0])
            let rightEval = this.Visit(node.Children().[1])

            let value = 
                match op with
                | "+" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() + rightEval.ToDouble()) :> Terminal
                | "-" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() - rightEval.ToDouble()) :> Terminal
                | "*" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() * rightEval.ToDouble()) :> Terminal
                | "/" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() / rightEval.ToDouble()) :> Terminal
                | "^" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Number(leftEval.ToDouble() ** rightEval.ToDouble()) :> Terminal
                | "<" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() < rightEval.ToDouble()) :> Terminal
                | ">" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() > rightEval.ToDouble()) :> Terminal
                | "<=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() <= rightEval.ToDouble()) :> Terminal
                | ">=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() >= rightEval.ToDouble()) :> Terminal
                | "==" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() = rightEval.ToDouble()) :> Terminal
                          | (:? Boolean, :? Boolean) -> new Boolean(leftEval.ToBool() = rightEval.ToBool()) :> Terminal
                          | (:? String, :? String) -> new Boolean(leftEval.ToString = rightEval.ToString) :> Terminal
                | "!=" -> match (leftEval, rightEval) with
                          | (:? Number, :? Number) -> new Boolean(leftEval.ToDouble() <> rightEval.ToDouble()) :> Terminal
                          | (:? Boolean, :? Boolean) -> new Boolean(leftEval.ToBool() <> rightEval.ToBool()) :> Terminal
                          | (:? String, :? String) -> new Boolean(leftEval.ToString <> rightEval.ToString) :> Terminal
            value


        member this.Visit (node: CoreParser.Parser.Node) : Terminal =
            let value = match node with
                        | :? Parser.Ops.TerminalNode -> this.Visit_TerminalNode(node)
                        | :? Parser.Ops.BinaryOp -> this.Visit_BinaryOp(node)
            value
