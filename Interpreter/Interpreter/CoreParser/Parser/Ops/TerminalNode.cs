using System;
using System.Collections.Generic;

namespace CoreParser.Parser.Ops
{
    public class TerminalNode : Node
    {
        public TerminalNode(Token token)
        {
            Token = token;
        }

        public override List<Node> Children()
        {
            return new List<Node>();
        }
    }
}
