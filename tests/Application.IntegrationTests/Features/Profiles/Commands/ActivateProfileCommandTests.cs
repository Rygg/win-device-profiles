using Application.Features.Profiles.Commands;
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
}