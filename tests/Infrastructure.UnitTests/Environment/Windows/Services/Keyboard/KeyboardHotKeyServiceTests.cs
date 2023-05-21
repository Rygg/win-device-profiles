using System.ComponentModel;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
using Infrastructure.Environment.Windows.Services.Keyboard;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.UnitTests.Environment.Windows.Services.Keyboard;

[TestFixture]
public sealed class KeyboardHotKeyServiceTests
{
    private readonly Mock<IHotKeyService> _hotKeyService = new();
    private readonly Mock<IWindowsHotKeyEventSender> _eventSender = new();
    private readonly Mock<ILogger<KeyboardHotKeyService>> _logger = new();

    [SetUp]
    public void Setup()
    {
        _hotKeyService.Reset();
        _eventSender.Reset();
    }

    [Test]
    public async Task RegisterHotKeyAsync_NullHotKey_ThrowsException()
    {
        var sut = new KeyboardHotKeyService(_logger.Object, _eventSender.Object, _hotKeyService.Object);
        var action = async () => await sut.RegisterHotKeyAsync(null!, CancellationToken.None);
        await action.Should().ThrowAsync<ArgumentNullException>();
        Assert.Pass();
    }

    [Test]
    public async Task RegisterHotKeyAsync_InvalidHotKey_ThrowsException()
    {
        var sut = new KeyboardHotKeyService(_logger.Object, _eventSender.Object, _hotKeyService.Object);
        var action = async () => await sut.RegisterHotKeyAsync(new HotKeyCombination(), CancellationToken.None);
        await action.Should().ThrowAsync<InvalidOperationException>(); 
        Assert.Pass();
    }

    [Test]
    public async Task RegisterHotKeyAsync_ApiFailure_ThrowsException()
    {
        _hotKeyService
            .Setup(m => m.RegisterHotKeyToHandle(It.IsAny<nint>(), It.IsAny<int>(), It.IsAny<FsModifiers>(), It.IsAny<uint>()))
            .Returns(false);

        var hotkey = new HotKeyCombination
        {
            Key = SupportedKeys.D1,
            Modifiers = SupportedKeyModifiers.Ctrl | SupportedKeyModifiers.Shift
        };
        var sut = new KeyboardHotKeyService(_logger.Object, _eventSender.Object, _hotKeyService.Object);
        var action = async () => await sut.RegisterHotKeyAsync(hotkey, CancellationToken.None);
        await action.Should().ThrowAsync<Win32Exception>();

        _hotKeyService
            .Verify(m => m.RegisterHotKeyToHandle(
                    It.IsAny<nint>(),
                    It.IsAny<int>(),
                    It.Is<FsModifiers>(mod =>
                        mod.HasFlag(FsModifiers.MOD_CONTROL)
                        && mod.HasFlag(FsModifiers.MOD_SHIFT)
                        && !mod.HasFlag(FsModifiers.MOD_ALT)
                        && !mod.HasFlag(FsModifiers.MOD_WIN)
                        && !mod.HasFlag(FsModifiers.MOD_NOREPEAT)),
                    It.Is<uint>(key => key == (uint)SupportedKeys.D1)
                ),
                Times.Once);
    }

    [Test]
    public async Task RegisterHotKeyAsync_ValidHotKey_RegistersHotKey()
    {
        _hotKeyService
            .Setup(m => m.RegisterHotKeyToHandle(It.IsAny<nint>(), It.IsAny<int>(), It.IsAny<FsModifiers>(), It.IsAny<uint>()))
            .Returns(true);

        var hotkey = new HotKeyCombination
        {
            Key = SupportedKeys.D1,
            Modifiers = SupportedKeyModifiers.Ctrl | SupportedKeyModifiers.Shift
        };

        var sut = new KeyboardHotKeyService(_logger.Object, _eventSender.Object, _hotKeyService.Object);
        var action = async () => await sut.RegisterHotKeyAsync(hotkey, CancellationToken.None);
        await action.Should().NotThrowAsync();

        _hotKeyService
            .Verify(m => m.RegisterHotKeyToHandle(
                    It.IsAny<nint>(),
                    It.IsAny<int>(),
                    It.Is<FsModifiers>(mod =>
                        mod.HasFlag(FsModifiers.MOD_CONTROL)
                        && mod.HasFlag(FsModifiers.MOD_SHIFT)
                        && !mod.HasFlag(FsModifiers.MOD_ALT)
                        && !mod.HasFlag(FsModifiers.MOD_WIN)
                        && !mod.HasFlag(FsModifiers.MOD_NOREPEAT)),
                    It.Is<uint>(key => key == (uint)SupportedKeys.D1)
                ),
                Times.Once);
    }

    [Test]
    public async Task RegisterHotKeyAsync_KeyIsAlreadyRegistered_DoesNotReRegister()
    {
        _hotKeyService
            .Setup(m => m.RegisterHotKeyToHandle(It.IsAny<nint>(), It.IsAny<int>(), It.IsAny<FsModifiers>(), It.IsAny<uint>()))
            .Returns(true);

        var hotkey = new HotKeyCombination
        {
            Key = SupportedKeys.D1,
            Modifiers = SupportedKeyModifiers.Ctrl | SupportedKeyModifiers.Shift
        };
        var sut = new KeyboardHotKeyService(_logger.Object, _eventSender.Object, _hotKeyService.Object);
        var preAction = async () => await sut.RegisterHotKeyAsync(hotkey, CancellationToken.None);
        await preAction.Should().NotThrowAsync();

        _hotKeyService.Invocations.Clear();
        var testAction = async () => await sut.RegisterHotKeyAsync(hotkey, CancellationToken.None);
        await testAction.Should().NotThrowAsync();

        _hotKeyService
            .Verify(m => m.RegisterHotKeyToHandle(
                    It.IsAny<nint>(),
                    It.IsAny<int>(),
                    It.Is<FsModifiers>(mod =>
                        mod.HasFlag(FsModifiers.MOD_CONTROL)
                        && mod.HasFlag(FsModifiers.MOD_SHIFT)),
                    It.Is<uint>(key => key == (uint)SupportedKeys.D1)
                ),
                Times.Never);
    }
}