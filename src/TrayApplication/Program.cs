using DeviceProfiles.Application;
using DeviceProfiles.Infrastructure;
using DeviceProfiles.Infrastructure.Persistence;
using DeviceProfiles.TrayApplication.Components.Windows.Forms.Context;
using DeviceProfiles.TrayApplication.Extensions;

namespace DeviceProfiles.TrayApplication;

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

        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.Run(host.Services.GetRequiredService<DeviceProfilesApplicationContext>());
    }
}
