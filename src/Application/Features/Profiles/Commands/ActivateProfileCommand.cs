using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Options;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Profiles.Commands;

public sealed record ActivateProfileCommand : IRequest
{
    /// <summary>
    /// Identifier of the profile in the configuration.
    /// </summary>
    public int ProfileId { get; init; }
}

/// <summary>
/// Implementation for the CommandHandler processing the commands.
/// </summary>
public sealed class SetProfileCommandHandler : IRequestHandler<ActivateProfileCommand>
{
    private readonly IDisplayDeviceController _displayDeviceController;
    private readonly ILogger<SetProfileCommandHandler> _logger;
    private readonly DeviceProfile[] _deviceProfiles;

    public SetProfileCommandHandler(
        IDisplayDeviceController displayDeviceController, 
        ILogger<SetProfileCommandHandler> logger,
        IOptions<ProfileOptions> profileOptions
        )
    {
        if (profileOptions == null)
        {
            throw new ArgumentNullException(nameof(profileOptions));
        }

        _displayDeviceController = displayDeviceController;
        _logger = logger;
        _deviceProfiles = profileOptions.Value.Profiles
            .Select(p => p.ToDeviceProfile())
            .ToArray();
    }

    public async Task Handle(ActivateProfileCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var clickedProfile = _deviceProfiles
            .FirstOrDefault(p => p.Id == request.ProfileId) ?? 
                             throw new ArgumentException(nameof(request.ProfileId), $"Profile with identifier {request.ProfileId} not found from the configuration"); ; // Get the clicked profile.
        

        if (await _displayDeviceController.ChangeDisplaySettings(clickedProfile, cancellationToken).ConfigureAwait(false))
        {
            _logger.DisplayProfileChanged();
        }
        else
        {
            _logger.DisplayProfileCouldNotBeChanged();
        }

        // TODO: SoundDevice controller?
    }
}