using System;
namespace EngineLibrary
{
    public class EngineException : Exception
    {
        public EngineException()
        {
        }

        public EngineException(string message)
            : base(message)
        {
        }

        public EngineException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
