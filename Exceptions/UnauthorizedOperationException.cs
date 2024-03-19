namespace MiniBank.Exceptions
{
    internal class UnauthorizedOperationException : Exception
    {
        public UnauthorizedOperationException()
        {
        }

        public UnauthorizedOperationException(string message)
            : base(message)
        {
        }

        public UnauthorizedOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}