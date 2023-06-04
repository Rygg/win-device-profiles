using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrayApplication.Components.Windows.Forms;
using TrayApplication.Extensions;

namespace TrayApplication;

internal static class Program
{
    /// <summary>
    /// The program starting point.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .AddApplicationServices()
            .AddInfrastructureServices()
            .AddTrayApplicationServices()
            .Build();

        using (var scope = host.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<DeviceProfilesDbContextInitializer>();
            initializer.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        System.Windows.Forms.Application.Run(host.Services.GetRequiredService<DeviceProfilesApplicationContext>());
    }
}
