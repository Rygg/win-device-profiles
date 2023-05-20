using Infrastructure.Environment.Windows.Services.Displays;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitTests.Environment.Windows.Services.Displays;

public sealed class DisplayDeviceServiceTests : BaseTestFixture
{
    [Test]
    public void Foo()
    {
        var sut = host.Services.GetRequiredService<DisplayDeviceService>();
        var str = sut.GetCurrentDisplayInformationString(CancellationToken.None);
        Assert.Pass();
    }
}