using Application.Features.Profiles.Commands.Common;
using Application.Features.Profiles.Commands.ImportProfiles;
using Domain.Entities;

namespace Application.IntegrationTests.Features.Profiles.Commands;

public sealed class ImportProfilesCommandTests : BaseTestFixture
{
    [Test]
    public async Task Handle_DefaultCommand_ThrowsValidationException()
    {
        var command = new ImportProfilesCommand();
        var act = async () => await SendAsync(command);
        await act.Should().ThrowExactlyAsync<ValidationException>();
    }

    [Test]
    public async Task Handle_InvalidProfilesInCommand_ThrowsValidationException()
    {
        var command = new ImportProfilesCommand
        {
            ProfileFile = new ProfilesFileDto() // This should not be valid profiles. 
        };
        var act = async () => await SendAsync(command);
        await act.Should().ThrowExactlyAsync<ValidationException>();
    }

    [Test]
    public async Task Handle_EmptyProfilesInCommand_ThrowsValidationException()
    {
        var command = new ImportProfilesCommand
        {
            ProfileFile = new ProfilesFileDto()
            {
                Profiles = Array.Empty<DeviceProfileDto>() // This should not be valid profiles. 
            } 
        };
        var act = async () => await SendAsync(command);
        await act.Should().ThrowExactlyAsync<ValidationException>();
    }

    [Test]
    public async Task Handle_ValidCommand_ImportsProfiles()
    {
        var file = await GetTestProfilesFromFile();
        var command = new ImportProfilesCommand
        {
            ProfileFile = file
        };
        var act = async () => await SendAsync(command);
        await act.Should().NotThrowAsync();

        var profile1 = await FindAsync<DeviceProfile>(1);
        var profile2 = await FindAsync<DeviceProfile>(2);
        var profile3 = await FindAsync<DeviceProfile>(3);

        profile1.Should().NotBeNull();
        profile2.Should().NotBeNull();
        profile3.Should().NotBeNull();

        var configProfile1 = file.Profiles.First(p => p.Id == 1).ToDeviceProfile();
        var configProfile2 = file.Profiles.First(p => p.Id == 2).ToDeviceProfile();
        var configProfile3 = file.Profiles.First(p => p.Id == 3).ToDeviceProfile();

        profile1.Should().BeEquivalentTo(configProfile1);
        profile2.Should().BeEquivalentTo(configProfile2);
        profile3.Should().BeEquivalentTo(configProfile3);
    }
}