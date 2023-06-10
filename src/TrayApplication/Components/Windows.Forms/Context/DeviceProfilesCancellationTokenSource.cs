using Microsoft.Extensions.Logging;
using TrayApplication.Components.Interfaces;
using TrayApplication.Extensions;

namespace TrayApplication.Components.Windows.Forms.Context;

/// <summary>
/// Implementation for serving a common CancellationToken across the application.
/// </summary>
public sealed class DeviceProfilesCancellationTokenSource : IApplicationCancellationTokenSource
{
    private readonly ILogger<DeviceProfilesCancellationTokenSource> _logger;
    private static readonly CancellationTokenSource GlobalCts = new();

    public DeviceProfilesCancellationTokenSource(ILogger<DeviceProfilesCancellationTokenSource> logger)
    {
        _logger = logger;
    }

    public CancellationToken Token { get; } = GlobalCts.Token;

    public void Cancel()
    {
        GlobalCts.Cancel();
        _logger.GlobalTokenCancelled();
    }

    public void Dispose()
    {
        GlobalCts.Dispose();
    }
}