using Application.Common.Interfaces;
using Application.Common.Options;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.HotKeys.Commands;

public sealed record RegisterHotKeysCommand : IRequest;

public sealed class RegisterHotKeysCommandHandler : IRequestHandler<RegisterHotKeysCommand>
{
    private readonly DeviceProfile[] _deviceProfiles;
    private readonly IHotKeyTrigger _hotKeyTrigger;

    public RegisterHotKeysCommandHandler(
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

    public async Task Handle(RegisterHotKeysCommand request, CancellationToken cancellationToken)
    {
        foreach (var profile in _deviceProfiles)
        {
            if (profile.HotKey != null)
            {
                await _hotKeyTrigger.RegisterHotKeyAsync(profile.HotKey, cancellationToken).ConfigureAwait(true);
            }
        }
    }
}