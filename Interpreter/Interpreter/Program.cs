using System;
using System.Collections.Generic;
//using Interpreter.Parser;

namespace Interpreter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string sourceCode = "(47.6^(4)*49)+59*43/(4-3)^2)";
            Lexer lexer = new Lexer(sourceCode);
            lexer.Tokenise();
            List<Token> tokenList = lexer.getTokenList();
            Console.WriteLine("Number of tokens: " + tokenList.Count);

        }
    }

}
