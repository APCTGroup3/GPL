namespace ParserEngine
module Program =
    open System
    open Engine

    [<EntryPoint>]
    let rec main argv =
        let engine = new Engine()

        let rec start() =
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
                    printfn "Input: %s" toparse
                    printfn "Result: %s" (engine.Run(node)).ToString
                    printfn "-----------------------------------------"
            with
                | _ as ex -> printfn "%s" (ex.Message)
            start()

        start()


