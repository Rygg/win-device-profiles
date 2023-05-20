using Application.Features.HotKeys.Commands;
using Domain.Models;

namespace Application.IntegrationTests.Features.HotKeys.Commands;

public sealed class RegisterHotKeysCommandTests : BaseTestFixture
{
    [Test]
    public async Task Handle_SuccessfulCase_RegistersHotKeys()
    {
        var command = new RegisterHotKeysCommand();
        var act = async () => await SendAsync(command);
        await act.Should().NotThrowAsync("Command should not throw an exception.");
        // TODO: Validate mock properly.
        hotKeyTriggerMock.Verify(m => m.RegisterHotKeyAsync(It.IsAny<HotKeyCombination>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        
    }
}