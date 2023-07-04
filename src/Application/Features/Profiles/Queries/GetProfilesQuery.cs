using DeviceProfiles.Application.Common.Interfaces;
using DeviceProfiles.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceProfiles.Application.Features.Profiles.Queries;

public sealed record GetProfilesQuery : IRequest<DeviceProfile[]>;

public sealed class GetProfilesQueryHandler : IRequestHandler<GetProfilesQuery, DeviceProfile[]>
{
    private readonly IDeviceProfilesDbContext _dbContext;
    
    public GetProfilesQueryHandler(IDeviceProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<DeviceProfile[]> Handle(GetProfilesQuery request, CancellationToken cancellationToken)
    {
        return _dbContext.DeviceProfiles.ToArrayAsync(cancellationToken);
    }
}