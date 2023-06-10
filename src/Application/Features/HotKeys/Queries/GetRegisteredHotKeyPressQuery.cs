using Application.Common.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.HotKeys.Queries;

public sealed record GetRegisteredHotKeyPressQuery : IRequest<DeviceProfile>;

public sealed class GetRegisteredHotKeyPressQueryHandler : IRequestHandler<GetRegisteredHotKeyPressQuery, DeviceProfile>
{
    private readonly IDeviceProfilesDbContext _dbContext; 
    private readonly IHotKeyTrigger _hotKeyTrigger;

    public GetRegisteredHotKeyPressQueryHandler(
        IDeviceProfilesDbContext dbContext,
        IHotKeyTrigger hotKeyTrigger
        )
    {
        _dbContext = dbContext;
        _hotKeyTrigger = hotKeyTrigger;
    }

    public async Task<DeviceProfile> Handle(GetRegisteredHotKeyPressQuery request, CancellationToken cancellationToken)
    {
        var key = await _hotKeyTrigger.GetHotKeyPressAsync(cancellationToken).ConfigureAwait(false);
        var profile = GetDeviceProfileFromKeyTrigger(key);
        return profile;
    }

    private DeviceProfile GetDeviceProfileFromKeyTrigger(HotKeyCombination hotKeyTrigger)
    {
        return _dbContext.DeviceProfiles
            .Single(p => p.HotKey != null && p.HotKey == hotKeyTrigger);
    }
}