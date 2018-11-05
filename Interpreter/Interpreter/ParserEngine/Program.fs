open System
open CoreParser




let interpret (node: CoreParser.Parser.Node) =
    let rec interpret_res (n1: CoreParser.Parser.Node) = 
        let tokentype = n1.Token.tokenType
        let tokenval = n1.Token.token
        let res = if tokentype = TokenTypes.constant then System.Single.Parse(tokenval)
                  else match tokenval with
                       | "-" -> if (n1.Children().Count = 1) then (interpret_res(n1.Children().[0]) * -1.0F) else (interpret_res(n1.Children().[0]) - interpret_res(n1.Children().[1]))
                       | "+" -> interpret_res(n1.Children().[0]) + interpret_res(n1.Children().[1])
                       | "*" -> interpret_res(n1.Children().[0]) * interpret_res(n1.Children().[1])
                       | "/" -> interpret_res(n1.Children().[0]) / interpret_res(n1.Children().[1])
                       | "^" -> interpret_res(n1.Children().[0]) ** interpret_res(n1.Children().[1])
        res
    interpret_res(node)




[<EntryPoint>]
let main argv =
    let toparse = "((5+4)/3)^2"
    let lexer = new CoreParser.Lexer(toparse)
    lexer.Tokenise()
    let parser = new CoreParser.Parser.Parser()
    let node = parser.Parse(lexer.getTokenList())
    printfn "Input: %s" toparse
    printfn "Result: %f" (interpret(node))
    1

