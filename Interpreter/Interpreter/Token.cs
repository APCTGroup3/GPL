namespace Interpreter{
    public enum TokenTypes
    {
        assignment = 0,
        statement = 1,
        identity = 2,
        op = 3, //Operator, but operator is a reserveword
        constant = 4,
        eof = 5,
        boolean = 6,
    }
    public class Token{
        public TokenTypes type;
        public int lineNumber;
        public string token;
    }
}