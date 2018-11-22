using System;
using System.Collections.Generic;
using CoreParser.Parser.AST;
//using Interpreter.Parser;

namespace CoreParser
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string sourceCode = "4.5";
            Lexer lexer = new Lexer(sourceCode);
            lexer.Tokenise();
            List<Token> tokenList = lexer.getTokenList();
            Console.WriteLine("Number of tokens: " + tokenList.Count);

            Parser.Parser parser = new Parser.Parser();
            Parser.AST.Node ast = parser.Parse(tokenList);
            parser.PrintTree(ast);
        }
    }

}
