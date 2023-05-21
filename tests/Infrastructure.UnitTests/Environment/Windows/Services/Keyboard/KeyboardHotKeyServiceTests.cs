using Domain.Enums;
using Domain.Models;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
using Infrastructure.Environment.Windows.Services.Keyboard;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitTests.Environment.Windows.Services.Keyboard;

public sealed class KeyboardHotKeyServiceTests : BaseTestFixture
{
    [Test]
    public async Task RegisterHotKeyAsync_NullHotKey_ThrowsException()
    {
        var sut = host.Services.GetRequiredService<KeyboardHotKeyService>();
        var action = async () => await sut.RegisterHotKeyAsync(null!, CancellationToken.None);
        await action.Should().ThrowAsync<ArgumentNullException>();
        Assert.Pass();
    }

    [Test]
    public async Task RegisterHotKeyAsync_InvalidHotKey_ThrowsException()
    {
        var sut = host.Services.GetRequiredService<KeyboardHotKeyService>();
        var action = async () => await sut.RegisterHotKeyAsync(new HotKeyCombination(), CancellationToken.None);
        await action.Should().ThrowAsync<InvalidOperationException>(); 
        Assert.Pass();
    }

    public async Task RegisterHotKeyAsync_ValidHotKey_RegistersHotKey()
    {
        var key = new HotKeyCombination
        {
            Key = SupportedKeys.D1,
            Modifiers = SupportedKeyModifiers.Ctrl | SupportedKeyModifiers.Shift
        };
        var sut = host.Services.GetRequiredService<KeyboardHotKeyService>();
        var action = async () => await sut.RegisterHotKeyAsync(key, CancellationToken.None);
        await action.Should().NotThrowAsync();
        // TODO: Verify mock
        /*hotKeyService
            .Verify(m => m.RegisterHotKeyToHandle(
                It.IsAny<nint>(), 
                It.IsAny<int>(),
                It.Is<FsModifiers>(mod => mod.HasFlag(FsModifiers.MOD_CONTROL) && mod.HasFlag(FsModifiers.MOD_SHIFT))
                ));*/
    }
}