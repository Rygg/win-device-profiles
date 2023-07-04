using DeviceProfiles.Application.Common.Extensions;

namespace DeviceProfiles.Application.Common.Behaviors;

public sealed class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest,TResponse> where TRequest : notnull
{
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.UnhandledExceptionOccurred(e);
            throw;
        }
    }
}