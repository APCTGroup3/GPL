﻿namespace ParserEngine
module Program =
    open System
    open Engine
    open System.Collections.Generic
    open CoreParser
    open CoreParser.Parser

    [<EntryPoint>]
    let rec main argv =
        let engine = new Engine()
        let lines = new List<string>()
        printfn("Enter expressions (type 'exit' to quit or 'stop' to finish entering statements): ")

        let rec start() =
            try
                let toparse = Console.ReadLine()
                if toparse = "exit" then
                    Environment.Exit 1
                elif toparse = "stop" then
                    try
                        let program = String.concat "" lines
                        let lexer = new CoreParser.Lexer(program)
                        lexer.Tokenise()
                        let parser = new CoreParser.Parser.Parser()
                        let node = parser.Parse(lexer.getTokenList())

                        let start = node

                        printfn "Input:\n %s" program
                        printfn "Result:\n %s" (engine.Run(start).ToStr())
                    finally
                        lines.Clear()
                    printfn "-----------------------------------------"
                    printfn("Enter expressions (type 'exit' to quit or 'stop' to finish entering statements): ")
                else 
                    lines.Add(toparse)
                    lines.Add("\n")

            with
                | _ as ex -> printfn "%s" (ex.Message)
            start()

        start()


