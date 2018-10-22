using System;
using System.Collections.Generic;

namespace Interpreter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string sourceCode = "if x > y then a + b";
            Lexer lexer = new Lexer(sourceCode);
            lexer.Tokenise();
            List<Token> tokenList = lexer.getTokenList();
            Console.WriteLine("Number of tokens: " + tokenList.Count);
        }
    }
}
