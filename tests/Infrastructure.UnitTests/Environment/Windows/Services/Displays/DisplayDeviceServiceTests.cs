using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Services.Displays;
using Microsoft.Extensions.Logging;

namespace Infrastructure.UnitTests.Environment.Windows.Services.Displays;

[TestFixture]
public sealed class DisplayDeviceServiceTests
{
    private readonly Mock<IDisplayService> _displayService = new();
    private readonly Mock<ILogger<DisplayDeviceService>> _logger = new();

    [Test]
    public void Foo()
    {
        var sut = new DisplayDeviceService(_logger.Object, _displayService.Object);
        var str = sut.GetCurrentDisplayInformationString(CancellationToken.None);
        Assert.Pass();
    }
}