using DeviceProfiles.Application.Features.Devices.Queries;

namespace DeviceProfiles.Application.IntegrationTests.Features.Devices.Queries;

public sealed class GetCurrentDeviceInformationQueryTests : BaseTestFixture
{
    [Test]
    public async Task Handle_InformationIsRetrieved_ReturnsCorrectResults()
    {
        displayControllerMock
            .Setup(m => m.GetCurrentDisplayInformationString(
                It.IsAny<CancellationToken>())
            )
            .ReturnsAsync("MockedDeviceInformation");

        var query = new GetCurrentDeviceInformationQuery();
        var result = await SendAsync(query);
        result.Should().Be("MockedDeviceInformation");
    }

    [Test]
    public async Task Handle_RetrievalFails_ThrowsException()
    {
        displayControllerMock
            .Setup(m => m.GetCurrentDisplayInformationString(
                It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new Exception("Something went wrong."));

        var query = new GetCurrentDeviceInformationQuery();
        var testAction = async() => await SendAsync(query);
        await testAction.Should().ThrowAsync<Exception>("Exception should be rethrown.");
    }
}