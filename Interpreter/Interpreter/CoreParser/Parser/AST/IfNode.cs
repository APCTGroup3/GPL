using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class IfNode : Node
    {
        public Node Condition { get; set; }
        public Node Statements { get; set; }

        public IfNode(Token token, Node cond, Node statements) : base(token)
        {
            Condition = cond;
            Statements = statements;
        }

        public override List<Node> Children()
        {
            return new List<Node>() { Condition, Statements };
        }
    }
}
