namespace ParserEngine
module Program =
    open System
    open Engine



    [<EntryPoint>]
    let rec main argv =
        try
            printfn("Enter expression (or type 'exit' to quit): ")
            let toparse = Console.ReadLine()
            if toparse = "exit" then
                Environment.Exit 1
            else 
                let lexer = new CoreParser.Lexer(toparse)
                lexer.Tokenise()
                let parser = new CoreParser.Parser.Parser()
                let node = parser.Parse(lexer.getTokenList())
                let engine = new Engine()
                printfn "Input: %s" toparse
                printfn "Result: %s" (engine.Visit(node)).ToString
                printfn "-----------------------------------------"
        with
            | _ as ex -> printfn "%s" (ex.Message)
        main(null)

