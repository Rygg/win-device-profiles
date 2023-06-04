using Application.Features.Profiles.Commands.ImportProfiles;
using Domain.Entities;

namespace Application.IntegrationTests.Features.Profiles.Commands;

public sealed class ImportProfilesCommandTests : BaseTestFixture
{

    [Test]
    public async Task Handle_ValidCommand_ImportsProfiles()
    {
        var profiles = GetProfilesFromConfiguration();
        var command = new ImportProfilesCommand
        {
            Profiles = profiles
        };
        var act = async () => await SendAsync(command);
        await act.Should().NotThrowAsync();

        var profile1 = await FindAsync<DeviceProfile>(1);
        var profile2 = await FindAsync<DeviceProfile>(2);
        var profile3 = await FindAsync<DeviceProfile>(3);

        profile1.Should().NotBeNull();
        profile2.Should().NotBeNull();
        profile3.Should().NotBeNull();

        var configProfile1 = profiles.First(p => p.Id == 1).ToDeviceProfile();
        var configProfile2 = profiles.First(p => p.Id == 2).ToDeviceProfile();
        var configProfile3 = profiles.First(p => p.Id == 3).ToDeviceProfile();

        profile1.Should().BeEquivalentTo(configProfile1);
        profile2.Should().BeEquivalentTo(configProfile2);
        profile3.Should().BeEquivalentTo(configProfile3);
    }
}