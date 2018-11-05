using System;
using System.Collections.Generic;

namespace CoreParser.Parser.Ops
{
    public class BinaryOp : Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }

        public BinaryOp(Token token)
        {
            Token = token;
        }
        public BinaryOp(Token token, Node left, Node right)
        {
            Token = token;
            Left = left;
            Right = right;
        }

        public override List<Node> Children()
        {
            return new List<Node>() { Left, Right };
        }
    }
}
