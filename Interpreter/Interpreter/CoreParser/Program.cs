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
            string sourceCode = Utils.ReadFile("test.txt");
            Lexer lexer = new Lexer(sourceCode);
            lexer.Tokenise();
            List<Token> tokenList = lexer.getTokenList();
            foreach (Token t in tokenList) {
                Console.WriteLine(t.token);
            }
            Console.WriteLine("Number of tokens: " + tokenList.Count);

            Parser.Parser parser = new Parser.Parser();
            Node ast = parser.Parse(tokenList);
            parser.PrintTree(ast);
        }
    }

}
