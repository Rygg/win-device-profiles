using Application.Common.Options;
using Application.Features.Background;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        builder.ConfigureServices((context, services) =>
        {
            services
                .AddApplicationOptions(context)
                .AddHostedService<BackgroundHotKeyService>();
        });
        return builder;
    }

    /// <summary>
    /// Private helper method for adding application layer options.
    /// </summary>
    private static IServiceCollection AddApplicationOptions(this IServiceCollection services, HostBuilderContext builder)
    {
        services
            .AddOptions<ProfileOptions>()
            .Bind(builder.Configuration.GetSection(ProfileOptions.RootSectionName))
            .Validate(ProfileOptions.Validate);
        
        return services;
    }
}