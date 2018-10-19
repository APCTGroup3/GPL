namespace Interpreter{
    enum tokenTypes
    {
        assignment = 0,
        conditional = 1,
        identity = 2,
        expression = 3,
    }
    class Token{
        public tokenTypes type;
        public int lineNumber;
        public string token;

    }
}