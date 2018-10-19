using System;

namespace Interpreter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string sourceCode = "if x > y then a + b";
            Lexer lexer = new Lexer(sourceCode);
            lexer.Tokenise();
        }
    }
}
