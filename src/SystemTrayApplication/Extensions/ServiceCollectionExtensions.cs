using Microsoft.Extensions.DependencyInjection;

namespace SystemTrayApplication.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging();
        return services;
    }

}