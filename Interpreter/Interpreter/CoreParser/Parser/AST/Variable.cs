using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
	public class Variable : Node
    {
        public Variable(Token token) : base(token) { }

        public override List<Node> Children()
        {
            return new List<Node>();
        }
    }
}
