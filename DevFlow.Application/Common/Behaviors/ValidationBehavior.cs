using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using MediatR;
using DevFlow.Application.Exceptions;
namespace DevFlow.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>where TRequest :notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var failures = validators.Select(v => v.Validate(context))
                                         .SelectMany(result => result.Errors)
                                         .Where(error => error != null)
                                         .Select(error => error.ErrorMessage)
                                         .ToList();
                if (failures.Any())
                {
                    throw new ValidationException(failures);
                }
            }
            return await next();
        }
    }
}
