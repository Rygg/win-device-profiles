using DeviceProfiles.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeviceProfiles.Application.Features.HotKeys.Commands;

public sealed record RegisterHotKeysCommand : IRequest;

public sealed class RegisterHotKeysCommandHandler : IRequestHandler<RegisterHotKeysCommand>
{
    private readonly IDeviceProfilesDbContext _dbContext;
    private readonly IHotKeyTrigger _hotKeyTrigger;

    public RegisterHotKeysCommandHandler(
        IDeviceProfilesDbContext dbContext,
        IHotKeyTrigger hotKeyTrigger
        )
    {
        _dbContext = dbContext;
        _hotKeyTrigger = hotKeyTrigger;
    }

    public async Task Handle(RegisterHotKeysCommand request, CancellationToken cancellationToken)
    {
        var profiles = await _dbContext.DeviceProfiles.ToArrayAsync(cancellationToken).ConfigureAwait(false);
        foreach (var profile in profiles)
        {
            if (profile.HotKey != null)
            {
                await _hotKeyTrigger.RegisterHotKeyAsync(profile.HotKey, cancellationToken).ConfigureAwait(true);
            }
        }
    }
}