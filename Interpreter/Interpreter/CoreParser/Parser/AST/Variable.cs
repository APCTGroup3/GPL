using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    //Node representing the retrieval of a variable stored in memory
	public class Variable : Node
    {
        //The token should be the name of the variable, a Str terminal
        public Variable(Token token) : base(token) { }

        public override List<Node> Children()
        {
            return new List<Node>();
        }
    }
}
