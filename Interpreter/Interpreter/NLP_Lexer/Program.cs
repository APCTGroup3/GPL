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

            // Test input
            string inp_a = "print add 4 and 5";

            string lex_chunk_a = nlpLexer.Tokenise(inp_a);

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
        }
    }
}
