using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    /* Represents a Unary expression such as -4 or !True */
    public class UnaryNode : Node
    {
        public Node Child { get; set; } //The operand
        //Token should be the operator
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
