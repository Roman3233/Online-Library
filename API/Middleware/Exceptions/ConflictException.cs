namespace API.Middleware.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
        StatusCode = 409;
    }

    public int StatusCode { get; }
}