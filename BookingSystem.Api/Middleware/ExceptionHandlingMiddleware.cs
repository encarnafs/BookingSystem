using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using BookingSystem.Application.Common.Exceptions; // si tienes NotFoundException, etc.
using FluentValidation;
using BookingSystem.Domain.Exceptions;  // para ValidationException

namespace BookingSystem.Api.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ExceptionHandlingMiddleware(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: ex.Message);

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (FluentValidation.ValidationException ex)
        {
            var errors = ex.Errors.Select(e => e.ErrorMessage).ToArray();

            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation error",
                detail: string.Join("; ", errors)
            );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (BookingSystem.Application.Common.Exceptions.ValidationException ex)
        {
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Application validation error",
                detail: ex.Message
            );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (UnauthorizedAccessException ex)
        {
                var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: ex.Message);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (DomainException ex)
        {
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status409Conflict,
                title: "Domain error",
                detail: ex.Message);

            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (ConflictException ex)
        {
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict error",
                detail: ex.Message
            );

            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (ForbiddenAccessException ex)
        {
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden",
                detail: ex.Message
            );

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (Exception ex)
        {
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected error",
                detail: ex.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
