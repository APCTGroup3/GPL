using System;
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
}
