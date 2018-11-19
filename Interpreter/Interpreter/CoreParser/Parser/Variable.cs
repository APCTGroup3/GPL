using System;
using System.Collections.Generic;

namespace CoreParser.Parser
{
	public class Variable : Node
    {
        public Variable(Token token)
        {
            Token = token;
        }

        public override List<Node> Children()
        {
            return new List<Node>();
        }
    }
}
