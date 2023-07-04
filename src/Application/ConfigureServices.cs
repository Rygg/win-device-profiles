using System.Runtime.CompilerServices;
using DeviceProfiles.Application.Common.Behaviors;

[assembly: InternalsVisibleTo("Application.UnitTests")]
[assembly: InternalsVisibleTo("Application.IntegrationTests")]
namespace DeviceProfiles.Application;
/// <summary>
/// Extension methods for injecting required application components.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Add required Application Services to the program dependency injection container.
    /// </summary>
    public static IHostBuilder AddApplicationServices(this IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ConfigureServices((_, services) =>
        {
            services
                .AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly)
                .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(ConfigureServices).Assembly);
                    cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
                    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                });
        });
        return builder;
    }
}