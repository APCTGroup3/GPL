using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class BlockNode : Node
    {
        public List<Node> Statements { get; set; }
        public BlockNode(Token token) : base(token) 
        {
            Statements = new List<Node>();
        }

        public BlockNode(Token token, Node[] statements) : base(token)
        {
            Statements = new List<Node>(statements);
        }

        public BlockNode(Token token, IEnumerable<Node> statements) : base(token)
        {
            Statements = new List<Node>(statements);
        }

        public override List<Node> Children()
        {
            return Statements;
        }
    }
}
