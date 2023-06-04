using Application.Common.Interfaces;
using Application.Common.Options;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.HotKeys.Queries;

public sealed record GetRegisteredHotKeyPressQuery : IRequest<DeviceProfile>;

public sealed class GetRegisteredHotKeyPressQueryHandler : IRequestHandler<GetRegisteredHotKeyPressQuery, DeviceProfile>
{
    private readonly DeviceProfile[] _deviceProfiles;
    private readonly IHotKeyTrigger _hotKeyTrigger;

    public GetRegisteredHotKeyPressQueryHandler(
        IOptions<ProfileOptions> profileOptions,
        IHotKeyTrigger hotKeyTrigger
        )
    {
        ArgumentNullException.ThrowIfNull(profileOptions);

        _hotKeyTrigger = hotKeyTrigger;
        _deviceProfiles = profileOptions.Value.Profiles
            .Select(p => p.ToDeviceProfile())
            .ToArray();
    }

    public async Task<DeviceProfile> Handle(GetRegisteredHotKeyPressQuery request, CancellationToken cancellationToken)
    {
        var key = await _hotKeyTrigger.GetHotKeyPressAsync(cancellationToken).ConfigureAwait(false);
        var profile = GetDeviceProfileFromKeyTrigger(key);
        return profile;
    }

    private DeviceProfile GetDeviceProfileFromKeyTrigger(HotKeyCombination hotKeyTrigger)
    {
        return _deviceProfiles
            .Single(p => p.HotKey != null && p.HotKey == hotKeyTrigger);
    }
}