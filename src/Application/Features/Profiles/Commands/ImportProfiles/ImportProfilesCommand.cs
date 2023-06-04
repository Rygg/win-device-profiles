using Application.Common.Interfaces;
using Application.Common.Options;
using MediatR;

namespace Application.Features.Profiles.Commands.ImportProfiles;

public sealed record ImportProfilesCommand : IRequest
{
    public IEnumerable<DeviceProfileOptions> Profiles { get; init; } = Enumerable.Empty<DeviceProfileOptions>();
}

public sealed class ImportProfilesCommandHandler : IRequestHandler<ImportProfilesCommand>
{
    private readonly IDeviceProfilesDbContext _dbContext;

    public ImportProfilesCommandHandler(IDeviceProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ImportProfilesCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var profiles = request.Profiles.Select(p => p.ToDeviceProfile()).ToList();
        foreach (var profile in profiles)
        {
            _dbContext.DeviceProfiles.Add(profile);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false); // Save one-by-one to differentiate between record type value objects.
        }
    }
}