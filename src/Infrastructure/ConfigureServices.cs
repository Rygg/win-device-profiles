using Application.Common.Interfaces;
using Infrastructure.Environment.Windows.Services.Displays;
using Infrastructure.Environment.Windows.Services.Keyboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using Infrastructure.Environment.Windows.Common.User32;
using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Infrastructure.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
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
            services.AddTransient<IHotKeyService, HotKeyService>(); // WIN32 Native API Wrapper.
            services.AddTransient<IDisplayService, DisplayService>(); // WIN32 Native API Wrapper.

            // Actual services for application functionality:
            services.AddSingleton<IHotKeyTrigger, KeyboardHotKeyService>(); // Singleton to track registrations and control disposing (unregister functionality).
            services.AddTransient<IDisplayDeviceController, DisplayDeviceService>();

            // Database:
            services.AddDatabaseServices();
        });
        return builder;
    }

    /// <summary>
    /// Add all database related services to the container.
    /// </summary>
    private static void AddDatabaseServices(this IServiceCollection services)
    {
        const string databaseName = "profiles.db";

        var connectionStringBuilder = new SqliteConnectionStringBuilder($"Data Source={databaseName}")
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        services.AddDbContext<DeviceProfilesDbContext>(options =>
        {
            options.UseSqlite(connectionStringBuilder.ToString(), builder =>
            {
                builder.MigrationsAssembly(typeof(DeviceProfilesDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<IDeviceProfilesDbContext>(provider => provider.GetRequiredService<DeviceProfilesDbContext>());
        services.AddScoped<DeviceProfilesDbContextInitializer>();
    }
}