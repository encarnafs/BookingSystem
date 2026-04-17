using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using BookingSystem.Application.Common.Exceptions; // si tienes NotFoundException, etc.

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
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (ValidationException)
        {
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation error",
                detail: "One or more validation errors occurred.");

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
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
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
