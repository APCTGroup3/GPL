using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class UnaryNode : Expression
    {
        public Node Child { get; set; }
        public UnaryNode(Token token) : base(token) { }
        public UnaryNode(Token token, Node child) : base(token)
        {
            Child = child;
        }

        public override List<Node> Children()
        {
            return new List<Node>() { Child };
        }
    }
}
