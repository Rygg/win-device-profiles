using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Environment.Windows.Services.Display;

public sealed class DisplayDeviceService : IDisplayDeviceController
{
    private readonly ILogger<DisplayDeviceService> _logger;

    public DisplayDeviceService(ILogger<DisplayDeviceService> logger)
    {
        _logger = logger;
    }

    public Task<bool> ChangeDisplaySettings(DeviceProfile profile, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }

    public Task<string> GetCurrentDisplayInformationString(CancellationToken cancellationToken)
    {
        return Task.FromResult("TODO");
    }
}