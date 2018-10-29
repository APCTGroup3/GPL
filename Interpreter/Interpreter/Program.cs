using System;
using System.Collections.Generic;
//using Interpreter.Parser;

namespace Interpreter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string sourceCode = "47.3plus2";
            Lexer lexer = new Lexer(sourceCode);
            lexer.Tokenise();
            List<Token> tokenList = lexer.getTokenList();
            Console.WriteLine("Number of tokens: " + tokenList.Count);

            Parser.Parser parser = new Parser.Parser();
            Parser.Node ast = parser.Parse(tokenList);
            parser.PrintTree(ast);
        }
    }

}
