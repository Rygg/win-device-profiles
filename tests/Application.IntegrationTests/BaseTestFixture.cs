using Application.Common.Interfaces;
using Application.Features.Profiles.Commands.Common;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Application.IntegrationTests;

[TestFixture]
public class BaseTestFixture
{
    private IHost _host = null!;
    private IServiceScopeFactory _scopeFactory = null!;
    protected IConfiguration configuration = null!;

    private static readonly SqliteConnectionStringBuilder ConnectionStringBuilder = new("Data Source=tests.db")
    {
        Mode = SqliteOpenMode.ReadWriteCreate
    };

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
                services
                    .AddDbContext<DeviceProfilesDbContext>(options =>
                        options.UseSqlite(ConnectionStringBuilder.ToString(),
                            sqlBuilder => sqlBuilder.MigrationsAssembly(typeof(DeviceProfilesDbContext).Assembly.FullName)));
                services.AddScoped<DeviceProfilesDbContextInitializer>();
                services.AddScoped<IDeviceProfilesDbContext>(provider => provider.GetRequiredService<DeviceProfilesDbContext>());
            })
            .Build();

        configuration = _host.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _host.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _host.Dispose();
    }

    [SetUp]
    public async Task SetUp()
    {
        hotKeyTriggerMock.Reset();
        displayControllerMock.Reset();

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DeviceProfilesDbContext>();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DeviceProfilesDbContextInitializer>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbInitializer.InitializeAsync(CancellationToken.None);
    }

    protected async Task<ProfilesFileDto> GetTestProfilesFromFile()
    {
        var result = await File.ReadAllTextAsync("testProfiles.json");
        return JsonSerializer.Deserialize<ProfilesFileDto>(result, JsonSerializerOptions.Default)
               ?? throw new InvalidOperationException("Test Profiles could not be parsed.");
        
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

    protected async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DeviceProfilesDbContext>();
        return await context.FindAsync<TEntity>(keyValues);
    }
}