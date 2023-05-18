using Infrastructure.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using WindowsTrayApplication.Components.HotKeys;

namespace WindowsTrayApplication.Extensions;

/// <summary>
/// Extension methods to support dependency injections.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the logging for the application.
    /// </summary>
    internal static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        return services.AddLogging();
    }

    /// <summary>
    /// Add the required services for Windows Tray Application functionality.
    /// </summary>
    internal static IServiceCollection AddWindowsTrayApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IHotKeyEventSender, KeyboardHotKeyHandle>(); // Add keyboard hot key handle as singleton.
        return services;
    }

}