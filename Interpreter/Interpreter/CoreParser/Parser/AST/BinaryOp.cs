using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class BinaryOp : Expression
    {
        public Node Left { get; set; }
        public Node Right { get; set; }

        public BinaryOp(Token token) : base(token) { }
        public BinaryOp(Token token, Node left, Node right) : base(token)
        {
            Left = left;
            Right = right;
        }

        public override List<Node> Children()
        {
            return new List<Node>() { Left, Right };
        }
    }
}
