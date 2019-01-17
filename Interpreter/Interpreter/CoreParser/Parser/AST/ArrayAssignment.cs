using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    /* A Node representing the assignment of a value to an element within an array variable */
    public class ArrayAssignment : Node
    {
        public Node ID { get; set; } //The name of the array variable. Should evaluate to a Boolean
        public Node Value { get; set; } //The value to set the element to. Should evaluate to a Terminal
        public Node Element { get; set; } //The index of the element. Should evaluate to a Number

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
