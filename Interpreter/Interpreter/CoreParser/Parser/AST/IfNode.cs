using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    /* Represents an If or an If Else statement, consisting of a boolean condition, a block of statements to execute if the condition is true,
     * and an optional block of statements to execute if false.
     * 
     * If statements are represented in the same way internally as if/else statements, only with a default empty else block 
     */


    public class IfNode : Node
    {
        public Node Condition { get; set; } //Should evaluate to a Boolean
        public Node Statements { get; set; } //To execute if condition is true
        public Node ElseStatements { get; set; } //Executes if condition is false

        public IfNode(Token token, Node cond, Node statements) : base(token)
        {
            Condition = cond;
            Statements = statements;
            ElseStatements = new BlockNode(new Token() { token = "" }); // Blank list of statements, nothing is executed
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
