using System;
using System.Collections.Generic;

namespace Interpreter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string sourceCode = "hgtfsum gt";
            Lexer lexer = new Lexer(sourceCode);
            lexer.Tokenise();
            List<Token> tokenList = lexer.getTokenList();

            Console.WriteLine("\nNumber of tokens: " + tokenList.Count);

            Console.WriteLine("\n------------------------------\n\nTOKENS:\n");

            foreach (Token tok in tokenList)
            {
                Console.WriteLine(tok.token);
            }

            //Console.WriteLine("\n------------------------------\n\nAST:\n");

            //Parser.Parser parser = new Parser.Parser();
            //Parser.Node ast = parser.Parse(tokenList);
            //parser.PrintTree(ast);
        }
    }

}
