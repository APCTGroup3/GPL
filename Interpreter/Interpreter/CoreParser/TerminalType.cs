using System;
using System.Collections.Generic;
using System.Collections;
using CoreParser.Parser;

namespace CoreParser
{
    public abstract class Terminal
    {
        public string TypeName { get; private set; }
        public abstract string ToStr();
        public double ToDouble()
        {
            return Double.Parse(ToStr());
        }
        public bool ToBool()
        {
            return bool.Parse(ToStr());
        }

        public Terminal(string typename)
        {
            TypeName = typename;
        }
        public Arr ToArr()
        {
            return (Arr)this;
        }
    }

    public class Number : Terminal
    {
        public double Value { get; private set; }
        public Number(double value) : base("Number")
        {
            Value = value;
        }
        public override string ToStr()
        {
            return this.Value.ToString();
        }
    }

    public class Boolean : Terminal
    {
        public bool Value { get; private set; }
        public Boolean(bool value) : base("Boolean")
        {
            Value = value;
        }
        public override string ToStr()
        {
            return this.Value.ToString();
        }
    }

    public class Str : Terminal
    {
        public string Value { get; private set; }
        public Str(string value) : base("Str")
        {
            Value = value;
        }
        public override string ToStr()
        {
            return this.Value.ToString();
        }
    }

    public class Void : Terminal
    {
        public Void() : base("Void") {}
        public override string ToStr()
        {
            return "";
        }
    }

    public class Arr : Terminal
    {
        public ArrayList Elements { get; private set; }

        //Creates an empty array of size 0
        public Arr() : base("Array")
        {
            Elements = new ArrayList();
        }

        //Creates an array of a given size, populated with Void types
        public Arr(int size) : base("Array")
        {
            Elements = new ArrayList();
            for (var i = 0; i < size; i++)
            {
                Elements.Add(new Void());
            }
        }

        public void Add(int index, Terminal element)
        {
            if (index < 0)
            {
                throw new ParserException("Index cannot be negative");
            }
            if (index >= Elements.Count)
            {
                //Resize array
                int numToAdd = index + 1 - Elements.Count;
                for (int i = 0; i < numToAdd; i++)
                {
                    Elements.Add(new Void());
                }
            }
            Elements[index] = element;
        }

        public void Add(Number i, Terminal element)
        {
            int index = (int)i.ToDouble();
            Add(index, element);
        }
        public Terminal Get(int i)
        {
            if (i < 0)
            {
                throw new ParserException("Index cannot be negative");
            }
            else if (i >= Elements.Count)
            {
                throw new ParserException("Array index out of bounds");
            }
            return ((Terminal)Elements[i]);

        }
        public Terminal Get(Number n)
        {
            int i = (int)n.ToDouble();
            return Get(i);
        }
        public override string ToStr()
        {
            var str = "[";
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] is Str)
                {
                    str += "\"";
                    str += ((Terminal)Elements[i]).ToStr();
                    str += "\"";
                }
                else
                {
                    str += ((Terminal)Elements[i]).ToStr();
                }
                if (i < Elements.Count - 1)
                {
                    str += ", ";
                }
            }
            str += "]";
            return str;
        }

    }
}
