using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    /* An AST Node for binary mathematical or logical operations,
     * for example 5 > 3.
     */
    public class BinaryOp : Node
    {
        //The left and right expressions either side of the operator
        public Node Left { get; set; }
        public Node Right { get; set; }

        //The token should be the operator for the current expression
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
