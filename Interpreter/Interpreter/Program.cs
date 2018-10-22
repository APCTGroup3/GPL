using System;
using System.Collections.Generic;

namespace Interpreter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string sourceCode = "if x > -4 then a+b else 8.8";
            Lexer lexer = new Lexer(sourceCode);
            lexer.Tokenise();
            List<Token> tokenList = lexer.getTokenList();
            Console.WriteLine("Number of tokens: " + tokenList.Count);
        }
    }
}
