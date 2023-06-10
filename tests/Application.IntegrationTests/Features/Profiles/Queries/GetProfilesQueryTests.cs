using Application.Features.Profiles.Queries;

namespace Application.IntegrationTests.Features.Profiles.Queries;

public sealed class GetProfilesQueryTests : BaseTestFixture
{
    [Test]
    public async Task Handle_ReturnsCorrectProfiles()
    {
        await PopulateDbWithTestProfiles();
        var query = new GetProfilesQuery();
        var result = await SendAsync(query);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(TestConfiguration.TestProfiles);
    }
}