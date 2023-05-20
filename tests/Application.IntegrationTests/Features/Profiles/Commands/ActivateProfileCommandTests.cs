using Application.Features.Profiles.Commands;
using Domain.Models;
using FluentValidation;

namespace Application.IntegrationTests.Features.Profiles.Commands;

public sealed class ActivateProfileCommandTests : BaseTestFixture
{
    [Test]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var command = new ActivateProfileCommand();
        var act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<ValidationException>("Empty command should not be allowed.");
    }

    [Test]
    public async Task Handle_ProfileIsNotFound_ThrowsArgumentException()
    {
        var command = new ActivateProfileCommand
        {
            ProfileId = 9876,
        };
        var act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<ArgumentException>("Profile should not be found.");
    }

    [Test]
    public async Task Handle_ValidCommand_ActivatesCorrectProfile()
    {
        var command = new ActivateProfileCommand
        {
            ProfileId = 1,
        };
        var act = async () => await SendAsync(command);
        await act.Should().NotThrowAsync("Command should be successful.");

        displayControllerMock
            .Verify(m => m.ChangeDisplaySettings(
                    It.Is<DeviceProfile>(dp =>
                        dp.Id == 1
                        && dp.Name == TestConfiguration.TestProfile1.Name
                        && dp.HotKey == TestConfiguration.TestProfile1.HotKey
                        && dp.DisplaySettings.SequenceEqual(TestConfiguration.TestProfile1.DisplaySettings)
                    ),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once);

        var command2 = new ActivateProfileCommand
        {
            ProfileId = 3,
        };
        act = async () => await SendAsync(command2);
        await act.Should().NotThrowAsync("Command should be successful.");

        displayControllerMock
            .Verify(m => m.ChangeDisplaySettings(
                    It.Is<DeviceProfile>(dp =>
                        dp.Id == 1
                        && dp.Name == TestConfiguration.TestProfile1.Name
                        && dp.HotKey == TestConfiguration.TestProfile1.HotKey
                        && dp.DisplaySettings.SequenceEqual(TestConfiguration.TestProfile1.DisplaySettings)
                    ),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once);

        displayControllerMock.Invocations.Should().HaveCount(2);
    }

    [Test]
    public async Task Handle_OperationFails_ThrowsException()
    {
        displayControllerMock.Setup(m => m.ChangeDisplaySettings(
            It.IsAny<DeviceProfile>(),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception("Something went wrong"));

        var command = new ActivateProfileCommand
        {
            ProfileId = 1,
        };
        var act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<Exception>("Profile should not be found.");
    }
}