using System;
using System.Collections.Generic;
//using Interpreter.Parser;

namespace Interpreter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string sourceCode = "((4-65.3^2)/149*4*4-(12.4^(7-5))^12)";
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
