using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class WhileNode : Node
    {
        public Node Condition { get; set; }
        public Node Block { get; set; }
        public WhileNode(Token token) : base(token) { }
        public WhileNode(Token token, Node condition, Node block) : base(token)
        {
            Condition = condition;
            Block = block;
        }

        public override List<Node> Children()
        {
            return new List<Node>() { Condition, Block };
        }
    }
}
