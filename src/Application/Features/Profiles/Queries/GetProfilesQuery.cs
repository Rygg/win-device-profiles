using Application.Common.Options;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.Profiles.Queries;

public sealed record GetProfilesQuery : IRequest<DeviceProfile[]>;

public sealed class GetProfilesQueryHandler : IRequestHandler<GetProfilesQuery, DeviceProfile[]>
{
    private readonly DeviceProfile[] _deviceProfiles;

    public GetProfilesQueryHandler(IOptions<ProfileOptions> profileOptions)
    {
        ArgumentNullException.ThrowIfNull(profileOptions);
        
        _deviceProfiles = profileOptions.Value.Profiles
            .Select(p => p.ToDeviceProfile())
            .ToArray();
    }

    public Task<DeviceProfile[]> Handle(GetProfilesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_deviceProfiles);
    }
}