using Application.Features.HotKeys.Commands;
using Application.Features.HotKeys.Queries;
using Application.Features.Profiles.Queries;
using Microsoft.Extensions.Logging;
using TrayApplication.Components.Interfaces;
using TrayApplication.Components.Windows.Forms.Context;

namespace TrayApplication.UnitTests;

[TestFixture]
public sealed class DeviceProfilesApplicationContextTests
{
    private readonly Mock<IRequestSender> _sender = new();
    private readonly ITrayIconProvider _trayIconProvider = Mock.Of<ITrayIconProvider>();
    private readonly IApplicationCancellationTokenSource _cts = Mock.Of<IApplicationCancellationTokenSource>();
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
        _sut = new DeviceProfilesApplicationContext(_sender.Object,_trayIconProvider, _cts, _logger);
        _sender.Verify(m => m.SendAsync(It.IsAny<GetProfilesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Created_StartsBackgroundLoop()
    {
        _sut = new DeviceProfilesApplicationContext(_sender.Object, _trayIconProvider, _cts, _logger);
        _sender.Verify(m => m.SendAsync(It.IsAny<RegisterHotKeysCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _sender.Verify(m => m.SendAsync(It.IsAny<GetRegisteredHotKeyPressQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}