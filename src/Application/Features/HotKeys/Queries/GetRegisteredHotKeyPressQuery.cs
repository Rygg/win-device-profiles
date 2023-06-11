using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        var profile = await _dbContext.DeviceProfiles
            .SingleAsync(p => 
                    p.HotKey != null && 
                    p.HotKey.Key == key.Key && 
                    p.HotKey.Modifiers == key.Modifiers,
                cancellationToken)
            .ConfigureAwait(false);
        return profile;
    }
}