using System;
using CoreParser.Parser.AST;

namespace CoreParser
{
    public static class Test
    {
        public static Node GetStart()
        {
            var start = new FunctionNode(new Token { token = "Print" });
            var tok = new Token()
            {
                token = "5",
                tokenType = TokenTypes.constant,
                constType = ConstTypes.number
            };

            var p1 = new TerminalNode(tok);

            start.Parameters.Add(p1);

            return start;
        }
    }
}
