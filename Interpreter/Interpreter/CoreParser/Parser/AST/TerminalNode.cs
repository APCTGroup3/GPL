using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class TerminalNode : Expression
    {
        public TerminalNode(Token token) : base(token) { }

        public override List<Node> Children()
        {
            return new List<Node>();
        }
    }
}
