using System.Net;
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
            await _problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = ex.StatusCode,
                    Title = "Not Found",
                    Detail = ex.Message,
                    Instance = context.Request.Path,
                    Extensions = { { "traceId", context.TraceIdentifier } }
                }
            });
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("Validation failed: {Message}", ex.Message);
            await _problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = ex.StatusCode,
                    Title = "Validation Error",
                    Detail = ex.Message,
                    Instance = context.Request.Path,
                    Extensions = { { "traceId", context.TraceIdentifier } }
                }
            });
        }
        catch (ForbiddenException ex)
        {
            _logger.LogInformation("Forbidden: {Message}", ex.Message);
            await _problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = ex.StatusCode,
                    Title = "Forbidden",
                    Detail = ex.Message,
                    Instance = context.Request.Path,
                    Extensions = { { "traceId", context.TraceIdentifier } }
                }
            });
        }
        catch (ConflictException ex)
        {
            _logger.LogInformation("Conflict: {Message}", ex.Message);
            await _problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = ex.StatusCode,
                    Title = "Conflict",
                    Detail = ex.Message,
                    Instance = context.Request.Path,
                    Extensions = { { "traceId", context.TraceIdentifier } }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await _problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Instance = context.Request.Path,
                    Extensions = { { "traceId", context.TraceIdentifier } }
                }
            });
        }
    }

    
}
