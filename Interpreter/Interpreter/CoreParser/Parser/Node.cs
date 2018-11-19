﻿using System;
using System.Collections.Generic;

namespace CoreParser.Parser
{
    public abstract class Node
    {
        public Token Token { get; protected set; }

        public abstract List<Node> Children();
        public bool HasChildren()
        {
            return Children().Count > 0;
        }
    }
}