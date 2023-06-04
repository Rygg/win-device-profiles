using Application.Features.HotKeys.Commands;
using Application.Features.HotKeys.Queries;
using Application.Features.Profiles.Queries;
using Microsoft.Extensions.Logging;
using TrayApplication.Components.Windows.Forms;
using TrayApplication.Components.Windows.Forms.TrayIcon;

namespace TrayApplication.UnitTests;

[TestFixture]
public sealed class DeviceProfilesApplicationContextTests
{
    private readonly Mock<ISender> _sender = new();
    private readonly TrayIconBuilder _trayIconBuilder = new(Mock.Of<ILogger<TrayIconBuilder>>());
    private readonly ILogger<DeviceProfilesApplicationContext> _logger = Mock.Of<ILogger<DeviceProfilesApplicationContext>>();

    private DeviceProfilesApplicationContext? _sut;

    [SetUp]
    public void Setup()
    {
        _sender.Reset();
    }

    [TearDown]
    public void TearDown()
    {
        _sut?.Dispose();
    }

    [Test]
    public void Created_RetrievesCurrentProfiles()
    {
        _sut = new DeviceProfilesApplicationContext(_sender.Object, _logger, _trayIconBuilder);
        _sender.Verify(m => m.Send(It.IsAny<GetProfilesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Created_StartsBackgroundLoop()
    {
        _sut = new DeviceProfilesApplicationContext(_sender.Object, _logger, _trayIconBuilder);
        _sender.Verify(m => m.Send(It.IsAny<RegisterHotKeysCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _sender.Verify(m => m.Send(It.IsAny<GetRegisteredHotKeyPressQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}