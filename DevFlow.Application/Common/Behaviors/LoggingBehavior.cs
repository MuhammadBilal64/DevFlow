using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevFlow.Application.Common.Behaviors
{
    public  class LoggingBehavior<TRequest,TResponse>:IPipelineBehavior<TRequest,TResponse>  where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest,TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest,TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)

        {
            var requestName=typeof(TRequest).Name;
            var startTime=DateTime.UtcNow;
            _logger.LogInformation("Handling request {requestName} at time {startTime}", requestName, startTime);
            try
            {
                var response= await next();
                var endTime=DateTime.UtcNow;
                var duration = endTime - startTime;
                _logger.LogInformation("Handled request {requestName} successfully in {duration}ms", requestName, duration.TotalMilliseconds);
                return response;

            }catch(Exception ex)
            {
                var endTime= DateTime.UtcNow;
                var duration= endTime - startTime;
                _logger.LogError(ex,"Request {requestName} failed after {duration}ms", requestName, duration.TotalMilliseconds);
                throw;
            }

        }

    }
}
