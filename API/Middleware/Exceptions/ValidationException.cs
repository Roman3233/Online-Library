namespace API.Middleware.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
        StatusCode = 400;
    }

    public int StatusCode { get; }
}