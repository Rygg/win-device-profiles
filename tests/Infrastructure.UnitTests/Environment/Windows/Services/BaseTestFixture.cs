using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Services.Displays;
using Infrastructure.Environment.Windows.Services.Keyboard;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.UnitTests.Environment.Windows.Services;

[TestFixture]
public class BaseTestFixture
{
    protected IHost host = null!;

    internal readonly Mock<IHotKeyService> hotKeyService = new();
    internal readonly Mock<IDisplayService> displayService = new();
    internal readonly Mock<IWindowsHotKeyEventSender> eventSender = new();
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddTransient<KeyboardHotKeyService>();
                services.AddTransient<DisplayDeviceService>();
                services.AddTransient(_ => hotKeyService.Object);
                services.AddTransient(_ => displayService.Object);
                services.AddTransient(_ => eventSender.Object);
            })
            .Build();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        host.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        hotKeyService.Reset();
        eventSender.Reset();
    }
}