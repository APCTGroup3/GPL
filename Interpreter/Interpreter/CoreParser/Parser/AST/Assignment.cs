using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class Assignment : Node
    {
        public Node ID { get; set; }
        public Node Value { get; set; }

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
