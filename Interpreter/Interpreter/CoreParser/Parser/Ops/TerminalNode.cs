using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST.Ops
{
    public class TerminalNode : Node
    {
        public TerminalNode(Token token) : base(token)
        {
            Token = token;
        }

        public override List<Node> Children()
        {
            return new List<Node>();
        }
    }
}
