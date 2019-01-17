using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    /* Represents the assignment of a value to a given variable */
    public class Assignment : Node
    {
        public Node ID { get; set; } // Should be a Str terminal
        public Node Value { get; set; } //Should evaluate to a Terminal

        public Assignment(Token token) : base(token){}
        public Assignment(Token token, Node id, Node value) : base(token)
        {
            ID = id;
            Value = value;
        }

        public override List<Node> Children()
        {
            return new List<Node>() { ID, Value };
        }
    }
}
