namespace CoreParser
{
    public enum TokenTypes
    {
        assignment = 0,
        statement = 1,
        identity = 2,
        op = 3, //Operator, but operator is a reserveword
        constant = 4,
        eof = 5,
        boolean = 6,
        newline = 7,
        keyword = 8 //if, else, for etc
    }

    public enum ConstTypes
    {
        boolean = 1,
        number = 2,
        str = 3,
        nullVal = 4
    }

    public class Token{
        public TokenTypes tokenType;
        public int lineNumber;
        public string token;
        public ConstTypes constType;
    }
}