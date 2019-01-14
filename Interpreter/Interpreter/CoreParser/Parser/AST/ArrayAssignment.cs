using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class ArrayAssignment : Node
    {
        public Node ID { get; set; }
        public Node Value { get; set; }
        public Node Element { get; set; }

        public ArrayAssignment(Token token) : base(token) { }
        public ArrayAssignment(Token token, Node id, Node element, Node value) : base(token)
        {
            ID = id;
            Value = value;
            Element = element;
        }

        public override List<Node> Children()
        {
            return new List<Node>() { ID, Element, Value };
        }
    }
}
