using Application.Common.Interfaces;
using Application.Common.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Background;

/// <summary>
/// Background service for listening to key presses in the background.
/// </summary>
public sealed class BackgroundHotKeyService : BackgroundService
{
    private readonly ILogger<BackgroundHotKeyService> _logger;
    private readonly ProfileOptions _profileOptions;
    private readonly IHotKeyTrigger _hotKeyTrigger;

    public BackgroundHotKeyService(
        ILogger<BackgroundHotKeyService> logger, 
        IOptions<ProfileOptions> profileOptions, 
        IHotKeyTrigger hotKeyTrigger
        )
    {
        _logger = logger;
        _hotKeyTrigger = hotKeyTrigger;
        if (profileOptions == null)
        {
            throw new ArgumentNullException(nameof(profileOptions));
        }
        _profileOptions = profileOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RegisterHotKeys(stoppingToken).ConfigureAwait(false);

        while (!stoppingToken.IsCancellationRequested)
        {
            var key = await _hotKeyTrigger.GetHotKeyPressAsync(stoppingToken).ConfigureAwait(false);
        }
        throw new NotImplementedException();
    }

    private async Task RegisterHotKeys(CancellationToken stoppingToken)
    {
        foreach (var profile in _profileOptions.Profiles)
        {
            await _hotKeyTrigger.RegisterHotKeyAsync(null!, stoppingToken).ConfigureAwait(false); // TODO: Hotkeys
        }
    }
}