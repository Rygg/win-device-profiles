using Application;
using Infrastructure;
using Microsoft.Extensions.Hosting;
using WindowsTrayApplication.Extensions;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .ConfigureLogging()
            .AddWindowsTrayApplicationServices()
            .AddInfrastructureServices()
            .AddApplicationServices();
    })
    .Build();

await host.RunAsync();