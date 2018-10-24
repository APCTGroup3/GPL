using System;
using System.Collections.Generic;

namespace Interpreter.Parser
{
    public class UnaryNode : Node
    {
        public Node Child { get; set; }
        public UnaryNode(Token token)
        {
            Token = token;
        }
        public UnaryNode(Token token, Node child)
        {
            Token = token;
            Child = child;
        }

        public override List<Node> Children()
        {
            return new List<Node>() { Child };
        }
    }
}
