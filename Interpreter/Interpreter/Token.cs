namespace Interpreter{
    public enum TokenTypes
    {
        assignment = 0,
        conditional = 1,
        identity = 2,
        expression = 3,
        eof = 4,
    }
    class Token{
        public TokenTypes type;
        public int lineNumber;
        public string token;

    }
}