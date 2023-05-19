using Application.Common.Interfaces;
using Infrastructure.Environment.Windows.Services.Displays;
using Infrastructure.Environment.Windows.Services.Keyboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

/// <summary>
/// Extension methods for injecting required infrastructure components.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Add required Infrastructure Services to the program dependency injection container.
    /// </summary>
    public static IHostBuilder AddInfrastructureServices(this IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IHotKeyTrigger, KeyboardHotKeyService>(); // Singleton to track registrations and control disposing (unregister functionality).
            services.AddSingleton<IDisplayDeviceController, DisplayDeviceService>(); // TODO: lifetime scope should be figured out later.
        });
        return builder;
    }
}