using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    //Represents a block (series) of statements that should be executed sequentially
    public class BlockNode : Node
    {
        public List<Node> Statements { get; set; } //The statements to execute, in the order they should be executed
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
