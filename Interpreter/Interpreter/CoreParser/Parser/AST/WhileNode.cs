using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    /*
     * A Node defining a while loop, containing a condition Node which should evaluate to a boolean,
     * and a block of statements to execute while the condition is true.
     */    
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
