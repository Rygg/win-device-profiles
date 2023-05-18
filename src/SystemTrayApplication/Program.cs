using Application;
using Infrastructure;
using Microsoft.Extensions.Hosting;
using WindowsTrayApplication.Extensions;

using var host = Host.CreateDefaultBuilder(args)
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddTrayApplicationServices()
    .Build();

await host.RunAsync().ConfigureAwait(true);