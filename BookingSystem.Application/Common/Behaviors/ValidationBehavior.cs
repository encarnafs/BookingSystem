using BookingSystem.Application.Auth.Responses;
using FluentValidation;
using MediatR;

namespace BookingSystem.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            // En lugar de lanzar excepción, devolvemos una respuesta controlada
            var responseType = typeof(TResponse);
            if (responseType == typeof(AuthResponse))
            {
                var response = new AuthResponse
                {
                    Success = false,
                    Message = string.Join("; ", failures.Select(f => f.ErrorMessage))
                };

                return (TResponse)(object)response;
            }

            // Si no es AuthResponse, puedes devolver null o manejarlo según tu caso
            return default!;
        }

        return await next(cancellationToken);
    }

}
