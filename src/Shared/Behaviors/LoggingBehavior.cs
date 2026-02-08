using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace shared.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest,TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull , IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] Handle Request={Request}, response={Response}, RequestData={RequestData}",
                typeof(TRequest).Name, typeof(TResponse).Name, request);

            var timer = Stopwatch.StartNew();
            var response = await next();
            timer.Stop();
            if (timer.ElapsedMilliseconds > 3000)
            {
                logger.LogWarning("[PERFORMANCE] Handle Request={Request}, ElapsedTime={ElapsedTime}ms",
                    typeof(TRequest).Name, timer.ElapsedMilliseconds);
            }
            logger.LogInformation("[END] Handle Request={Request}, response={Response}, ElapsedTime={ElapsedTime}ms",
                typeof(TRequest).Name, typeof(TResponse).Name, timer.ElapsedMilliseconds);

            return response;
        }
    }
}
