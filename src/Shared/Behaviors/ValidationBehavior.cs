using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Behaviors
{
    public class ValidationBehavior<TRequest,TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest,TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validationContext = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));
            var validationFailures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (validationFailures.Any())
            {
                throw new ValidationException(validationFailures);
            }

            return await next();
        }
    }
}
