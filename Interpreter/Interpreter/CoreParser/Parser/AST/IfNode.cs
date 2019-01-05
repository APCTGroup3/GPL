using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class IfNode : Node
    {
        public Node Condition { get; set; }
        public Node Statements { get; set; }
        public Node ElseStatements { get; set; }

        public IfNode(Token token, Node cond, Node statements) : base(token)
        {
            Condition = cond;
            Statements = statements;
            ElseStatements = new BlockNode(new Token()); // Blank list of statements, nothing is executed
        }

        public IfNode(Token token, Node cond, Node statements, Node elseStatements) : base(token)
        {
            Condition = cond;
            Statements = statements;
            ElseStatements = elseStatements; // Blank list of statements, nothing is executed
        }

        public override List<Node> Children()
        {
            return new List<Node>() { Condition, Statements };
        }
    }
}
