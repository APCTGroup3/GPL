open System
open CoreParser


let interpretExpression (node: CoreParser.Parser.Node) =
    let rec interpret_res (n1: CoreParser.Parser.Node) = 
        let tokentype = n1.Token.tokenType
        let tokenval = n1.Token.token
        let res = if tokentype = TokenTypes.constant then System.Double.Parse(tokenval)
                  else match tokenval with
                       | "-" -> if (n1.Children().Count = 1) then (interpret_res(n1.Children().[0]) * -1.0) else (interpret_res(n1.Children().[0]) - interpret_res(n1.Children().[1]))
                       | "+" -> interpret_res(n1.Children().[0]) + interpret_res(n1.Children().[1])
                       | "*" -> interpret_res(n1.Children().[0]) * interpret_res(n1.Children().[1])
                       | "/" -> interpret_res(n1.Children().[0]) / interpret_res(n1.Children().[1])
                       | "^" -> interpret_res(n1.Children().[0]) ** interpret_res(n1.Children().[1])
        res
    interpret_res(node)




[<EntryPoint>]
let rec main argv =
    try
        printfn("Enter expression: ")
        let toparse = Console.ReadLine()
        if toparse = "exit" then
            Environment.Exit 1
        else 
            let lexer = new CoreParser.Lexer(toparse)
            lexer.Tokenise()
            let parser = new CoreParser.Parser.Parser()
            let node = parser.Parse(lexer.getTokenList())
            printfn "Input: %s" toparse
            printfn "Result: %f" (interpretExpression(node))
            printfn "-----------------------------------------"
    with
        | _ as ex -> printfn "%s" (ex.Message)
    main(null)

