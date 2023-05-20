using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.IntegrationTests;

[TestFixture]
public class BaseTestFixture
{
    private IHost _host = null!;
    private IServiceScopeFactory _scopeFactory = null!;

    protected readonly Mock<IHotKeyTrigger> hotKeyTriggerMock = new();
    protected readonly Mock<IDisplayDeviceController> displayControllerMock = new();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _host = Host.CreateDefaultBuilder()
            .AddApplicationServices()
            .ConfigureServices(services =>
            {
                services.AddTransient(_ => hotKeyTriggerMock.Object);
                services.AddTransient(_ => displayControllerMock.Object);
            })
            .Build();

        _scopeFactory = _host.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [SetUp]
    public void SetUp()
    {
        hotKeyTriggerMock.Reset();
        displayControllerMock.Reset();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _host.Dispose();
    }

    protected async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    protected async Task SendAsync(IRequest request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }
}