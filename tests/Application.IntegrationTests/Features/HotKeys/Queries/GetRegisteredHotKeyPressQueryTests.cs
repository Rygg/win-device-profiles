using DeviceProfiles.Application.Features.HotKeys.Queries;

namespace DeviceProfiles.Application.IntegrationTests.Features.HotKeys.Queries;

public sealed class GetRegisteredHotKeyPressQueryTests : BaseTestFixture
{
    [Test]
    public async Task Handle_HotKeyPressed_ReturnsCorrectProfile()
    {
        await PopulateDbWithTestProfiles();
        hotKeyTriggerMock
            .Setup(m => m.GetHotKeyPressAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestConfiguration.TestProfile2.HotKey!);

        var query = new GetRegisteredHotKeyPressQuery();
        var result = await SendAsync(query);
        result.Should().BeEquivalentTo(TestConfiguration.TestProfile2);
    }

    [Test]
    public async Task Handle_CancelledOperation_ThrowsException()
    {
        hotKeyTriggerMock
            .Setup(m => m.GetHotKeyPressAsync(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var query = new GetRegisteredHotKeyPressQuery();
        var testAction = async () => await SendAsync(query);
        await testAction.Should().ThrowAsync<OperationCanceledException>("Operation should be cancelled.");
    }
}