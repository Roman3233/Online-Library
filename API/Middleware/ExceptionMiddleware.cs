using System.Net;
using Microsoft.AspNetCore.Mvc;

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
        catch (Exception ex)
        {
            
        }
    }

    
}
