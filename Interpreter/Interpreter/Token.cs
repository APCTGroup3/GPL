namespace Interpreter{
    public enum TokenTypes
    {
        assignment = 0,
        statement = 1,
        identity = 2,
        expression = 3, //Operator, but operator is a reserveword
        eof = 4,
    }
    class Token{
        public TokenTypes type;
        public int lineNumber;
        public string token;

    }
}