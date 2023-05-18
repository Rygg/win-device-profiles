using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WindowsTrayApplication.Components;
using WindowsTrayApplication.Extensions;

namespace WindowsTrayApplication;

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

        System.Windows.Forms.Application.Run(host.Services.GetRequiredService<DeviceProfilesApplicationContext>());
    }
}
