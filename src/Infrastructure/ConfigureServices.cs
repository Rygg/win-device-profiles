using Application.Common.Interfaces;
using Infrastructure.Environment.Windows.Services.Keyboard;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Extension methods for injecting required infrastructure components.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Add required Infrastructure Services to the program dependency injection container.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IHotKeyTrigger, KeyboardHotKeyService>(); // Singleton to track registrations and control disposing.
        
        return services;
    }
}