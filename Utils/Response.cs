using MiniBank.Enums;

namespace MiniBank.Utils
{
    internal class Response<T>
    {
        internal OperationStatus Status { get; set; }
        internal T? Data { get; set; }
        internal string? ErrorMessage { get; set; }

        internal void Deconstruct(out OperationStatus status, out T data, out string error)
        {
            status = Status;
            data = Data;
            error = ErrorMessage;
        }
    }
}