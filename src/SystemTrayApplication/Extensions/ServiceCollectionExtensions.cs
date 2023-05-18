using Infrastructure.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WindowsTrayApplication.Components.HotKeys;

namespace WindowsTrayApplication.Extensions;

/// <summary>
/// Extension methods to support dependency injections.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method for adding the required windows application services.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    internal static IHostBuilder AddTrayApplicationServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<IWindowsHotKeyEventSender, KeyboardHotKeyHandle>(); // Add keyboard hot key handle as singleton.
            services.ConfigureLogging();
        });
        return hostBuilder;
    }

    /// <summary>
    /// Configures the logging for the application.
    /// </summary>
    internal static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        return services.AddLogging();
    }
}