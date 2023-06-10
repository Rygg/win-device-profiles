using Application.Common.Interfaces;
using Application.Features.Profiles.Commands.Common;
using MediatR;

namespace Application.Features.Profiles.Commands.ImportProfiles;

/// <summary>
/// This command overrides all current profiles in the database with the imported ones.
/// </summary>
public sealed record ImportProfilesCommand : IRequest
{
    public ProfilesFileDto ProfileFile { get; init; } = new();
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

        var profiles = request.ProfileFile.Profiles.Select(p => p.ToDeviceProfile()).ToList();
        _dbContext.DeviceProfiles.RemoveRange(_dbContext.DeviceProfiles); // Clear current database tables. This should be fine as there is not too many profiles.
        await _dbContext.DeviceProfiles.AddRangeAsync(profiles, cancellationToken).ConfigureAwait(true);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(true);
    }
}