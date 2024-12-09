using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;

namespace Unify.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = request.GetType().Name;

        try
        {
            _logger.LogInformation("Executing request {Request}", name);

            var result = await next();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Request {Request} processed successfully", name);
            }
            else
            {
                using (LogContext.PushProperty("Errors", result.Error, true))
                {
                    _logger.LogError("Request {Request} processed with error", name);
                }
            }


            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Request {Request} processing failed", name);

            throw;
        }
    }
}