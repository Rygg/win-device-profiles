﻿using DeviceProfiles.Infrastructure.Interfaces;
using DeviceProfiles.TrayApplication.Components.Interfaces;
using DeviceProfiles.TrayApplication.Components.Windows.Forms.Context;
using DeviceProfiles.TrayApplication.Components.Windows.Forms.HotKeys;
using DeviceProfiles.TrayApplication.Components.Windows.Forms.TrayIcon;
using Serilog;
using Serilog.Events;

namespace DeviceProfiles.TrayApplication.Extensions;

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
                services.AddSingleton<ITrayIconProvider, ApplicationTrayIconProvider>(); // Add tray icon provider as a singleton.
                services.AddSingleton<IApplicationCancellationTokenSource, DeviceProfilesCancellationTokenSource>(); // Add global cts as a singleton.
                services.AddSingleton<IRequestSender, ScopedRequestSender>(); // Add scoped request sender as a singleton.
                services.AddSingleton<DeviceProfilesApplicationContext>(); // Add application context as a singleton.
            });
    }

    /// <summary>
    /// Configures the logging for the application.
    /// </summary>
    private static IHostBuilder ConfigureLogging(this IHostBuilder builder)
    {
        const string logFile = "Logs\\DeviceProfiles.log";
        const string logTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}][{SourceContext:l}]: {Message:lj}{NewLine}{Exception}";
        const RollingInterval logInterval = RollingInterval.Day;

        return builder.UseSerilog((hostingContext, _, loggingConfiguration) =>
        {
            loggingConfiguration
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: logFile,
                    outputTemplate: logTemplate,
                    formatProvider: CultureInfo.InvariantCulture,
                    retainedFileCountLimit: 30,
                    rollingInterval: logInterval
                );

            var logLevelBlock = hostingContext.Configuration.GetSection("LogLevel");
            if(Enum.TryParse(logLevelBlock.Value, true, out LogEventLevel logLevel))
            {
                loggingConfiguration.MinimumLevel.Is(logLevel);
            }
            else
            {
                loggingConfiguration.MinimumLevel.Error();
            }
        });
    }
}