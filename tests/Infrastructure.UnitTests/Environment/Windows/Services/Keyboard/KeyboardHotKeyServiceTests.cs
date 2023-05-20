using Domain.Models;
using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Services.Keyboard;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.UnitTests.Environment.Windows.Services.Keyboard;

[TestFixture]
public sealed class KeyboardHotKeyServiceTests
{
    private readonly Mock<IHotKeyService> _hotKeyService = new();
    private readonly Mock<IWindowsHotKeyEventSender> _eventSender = new();
    private readonly ILogger<KeyboardHotKeyService> _logger = Mock.Of<ILogger<KeyboardHotKeyService>>();

    private KeyboardHotKeyService _sut = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _sut = new KeyboardHotKeyService(_logger, _eventSender.Object, _hotKeyService.Object);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _sut.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        _hotKeyService.Reset();
        _eventSender.Reset();
    }

    [Test]
    public async Task RegisterHotKeyAsync_InvalidHotKey_Foo()
    {
        var action = async () => await _sut.RegisterHotKeyAsync(new HotKeyCombination(), CancellationToken.None);
        await action.Should().ThrowAsync<InvalidOperationException>(); 
        Assert.Pass();
    }
}