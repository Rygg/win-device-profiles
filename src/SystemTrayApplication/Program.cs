using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddLogging()
            // TODO: Add tray.
            .AddInfrastructureServices()
            .AddApplicationServices();
    })
    .Build();

await host.RunAsync();