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

    public Task<bool> ChangeDisplaySettings(DeviceProfile profile)
    {
        return Task.FromResult(false);
    }

    public Task<string> GetCurrentDisplayInformationString()
    {
        return Task.FromResult(string.Empty);
    }
}