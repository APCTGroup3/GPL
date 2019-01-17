using System;
using System.Collections.Generic;
using CoreParser;
using ParserEngine;

namespace NLP_Lexer
{
    class MainClass
    {

        static string ACCESS_TOKEN = "7DOTRBXV6DLL22FQUJRKOMSCOEUL5XG5";

        public static void Main(string[] args)
        {

            NLP_Lexer.Lexer nlpLexer = new NLP_Lexer.Lexer(ACCESS_TOKEN);

            string inp_a = "print add 4 and 5";
            //string inp_b = "print b";

            string lex_chunk_a = nlpLexer.Tokenise(inp_a);
            //string lex_chunk_b = nlpLexer.Tokenise(inp_b);

            Console.WriteLine("Input = \n\t" + inp_a);

            Console.WriteLine(lex_chunk_a);
            try
            {
                CoreParser.Lexer lexer = new CoreParser.Lexer(lex_chunk_a);
                lexer.Tokenise();
                List<Token> tokens = lexer.getTokenList();
                CoreParser.Parser.Parser parser = new CoreParser.Parser.Parser();
                CoreParser.Parser.AST.Node ast = parser.Parse(tokens);
                Engine.Engine engine = new Engine.Engine();
                engine.Run(ast);
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }


            //Wit client = new Wit(accessToken: ACCESS_TOKEN);

            //var response = client.Message("30.6 times 600.9");

            //string lex_chunk = string.Empty;

            //if (response.Entities.ContainsKey("add"))
            //{
            //    lex_chunk += response.Entities["number"][0]["value"];
            //    lex_chunk += " + ";
            //    lex_chunk += response.Entities["number"][1]["value"];
            //}

            //if (response.Entities.ContainsKey("subtract"))
            //{
            //    // Look into intent //
            //    lex_chunk += response.Entities["number"][1]["value"];
            //    lex_chunk += " - ";
            //    lex_chunk += response.Entities["number"][0]["value"];
            //}

            //if (response.Entities.ContainsKey("divide"))
            //{
            //    lex_chunk += response.Entities["number"][0]["value"];
            //    lex_chunk += " / ";
            //    lex_chunk += response.Entities["number"][1]["value"];
            //}

            //if (response.Entities.ContainsKey("multiply"))
            //{
            //    lex_chunk += response.Entities["number"][0]["value"];
            //    lex_chunk += " * ";
            //    lex_chunk += response.Entities["number"][1]["value"];
            //}

            //CoreParser.Lexer lexer = new CoreParser.Lexer(lex_chunk);

            //lexer.Tokenise();
            //List<Token> tokenList = lexer.getTokenList();
            //Console.WriteLine("Number of tokens: " + tokenList.Count);

            //foreach(var token in tokenList)
            //{
            //    Console.WriteLine(token.token);
            //}
        }
    }
}
