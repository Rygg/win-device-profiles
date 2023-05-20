using Domain.Models;
using Infrastructure.Environment.Windows.Services.Keyboard;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitTests.Environment.Windows.Services.Keyboard;

public sealed class KeyboardHotKeyServiceTests : BaseTestFixture
{


    [Test]
    public async Task RegisterHotKeyAsync_InvalidHotKey_Foo()
    {
        var sut = host.Services.GetRequiredService<KeyboardHotKeyService>();
        var action = async () => await sut.RegisterHotKeyAsync(new HotKeyCombination(), CancellationToken.None);
        await action.Should().ThrowAsync<InvalidOperationException>(); 
        Assert.Pass();
    }
}