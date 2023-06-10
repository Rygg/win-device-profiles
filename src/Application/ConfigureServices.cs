using Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Application.UnitTests")]
[assembly: InternalsVisibleTo("Application.IntegrationTests")]
namespace Application;
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
        
        builder.ConfigureServices((context, services) =>
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