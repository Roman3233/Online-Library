namespace API.Middleware.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
        StatusCode = 403;
    }
    public int StatusCode { get; }
}