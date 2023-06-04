using Application.Common.Options;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.Profiles.Queries;

public sealed record GetProfilesQuery : IRequest<DeviceProfile[]>;

public sealed class GetProfilesQueryHandler : IRequestHandler<GetProfilesQuery, DeviceProfile[]>
{
    private readonly DeviceProfile[] _deviceProfiles;

    public GetProfilesQueryHandler(IOptions<ProfileOptions> profileOptions)
    {
        if (profileOptions == null)
        {
            throw new ArgumentNullException(nameof(profileOptions));
        }
        _deviceProfiles = profileOptions.Value.Profiles
            .Select(p => p.ToDeviceProfile())
            .ToArray();
    }

    public Task<DeviceProfile[]> Handle(GetProfilesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_deviceProfiles);
    }
}