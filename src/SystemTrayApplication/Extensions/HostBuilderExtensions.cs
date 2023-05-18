using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrayApplication.Components.Windows.Forms;
using TrayApplication.Components.Windows.Forms.HotKeys;
using TrayApplication.Components.Windows.Forms.TrayIcon;

namespace TrayApplication.Extensions;

/// <summary>
/// Extension methods to support dependency injections.
/// </summary>
internal static class HostBuilderExtensions
{
    /// <summary>
    /// Extension method for adding the required windows application services.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    internal static IHostBuilder AddTrayApplicationServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder
            .ConfigureLogging()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IWindowsHotKeyEventSender, KeyboardHotKeyHandle>(); // Add keyboard hot key handle as singleton.
                services.AddSingleton<TrayIconBuilder>();
                services.AddSingleton<DeviceProfilesApplicationContext>();
            });
    }

    /// <summary>
    /// Configures the logging for the application.
    /// </summary>
    private static IHostBuilder ConfigureLogging(this IHostBuilder builder)
    {
        return builder.ConfigureLogging((context,options) =>
        {
            options.AddConfiguration(context.Configuration);
        });
    }
}