using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Domain.Exceptions;

namespace VotingOnIdeas.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, errors) = exception switch
        {
            NotFoundException ex => (StatusCodes.Status404NotFound, ex.Message, (IDictionary<string, string[]>?)null),
            UnauthorizedException ex => (StatusCodes.Status403Forbidden, ex.Message, null),
            ConflictException ex => (StatusCodes.Status409Conflict, ex.Message, null),
            ValidationException ex => (StatusCodes.Status422UnprocessableEntity, "Validation failed.", (IDictionary<string, string[]>?)ex.Errors),
            DomainException ex => (StatusCodes.Status400BadRequest, ex.Message, null),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null),
        };

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = context.Request.Path,
        };

        if (errors is not null)
            problem.Extensions["errors"] = errors;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}
