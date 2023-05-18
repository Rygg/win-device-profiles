using Application.Common.Interfaces;
using Application.Common.Options;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.HotKeys.Queries;

public sealed record GetHotKeyPressProfileChangeQuery : IRequest<DeviceProfile>;

public sealed class GetHotKeyPressProfileChangeHandler : IRequestHandler<GetHotKeyPressProfileChangeQuery, DeviceProfile>
{
    private readonly DeviceProfile[] _deviceProfiles;
    private readonly IHotKeyTrigger _hotKeyTrigger;

    public GetHotKeyPressProfileChangeHandler(
        IOptions<ProfileOptions> profileOptions,
        IHotKeyTrigger hotKeyTrigger
        )
    {
        if (profileOptions == null)
        {
            throw new ArgumentNullException(nameof(profileOptions));
        }

        _hotKeyTrigger = hotKeyTrigger;
        _deviceProfiles = profileOptions.Value.Profiles
            .Select(p => p.ToDeviceProfile())
            .ToArray();
    }

    public async Task<DeviceProfile> Handle(GetHotKeyPressProfileChangeQuery request, CancellationToken cancellationToken)
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