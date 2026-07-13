using Microsoft.AspNetCore.Mvc;
using API.Middleware.Exceptions;
namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IProblemDetailsService problemDetailsService)    
    {
        _next = next;
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogInformation("Resource not found: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, ex.StatusCode, "Not Found", ex.Message);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("Validation failed: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, ex.StatusCode, "Validation Error", ex.Message);
        }
        catch (ForbiddenException ex)
        {
            _logger.LogInformation("Forbidden: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, ex.StatusCode, "Forbidden", ex.Message);
        }
        catch (ConflictException ex)
        {
            _logger.LogInformation("Conflict: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, ex.StatusCode, "Conflict", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await WriteProblemDetailsAsync(context, 500, "Internal Server Error", "Something went wrong.");
        }
    }

    private async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string detail)
    {
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions = { { "traceId", context.TraceIdentifier } }
            }
        });
    }

    

    
}
