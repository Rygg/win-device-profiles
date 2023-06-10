using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Profiles.Commands.ActivateProfile;

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
    private readonly IDeviceProfilesDbContext _dbContext;
    private readonly IDisplayDeviceController _displayDeviceController;
    private readonly ILogger<SetProfileCommandHandler> _logger;

    public SetProfileCommandHandler(
        IDeviceProfilesDbContext dbContext,
        IDisplayDeviceController displayDeviceController,
        ILogger<SetProfileCommandHandler> logger
        )
    
    {
        _displayDeviceController = displayDeviceController;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(ActivateProfileCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var clickedProfile = 
            await _dbContext.DeviceProfiles.FirstOrDefaultAsync(dp => dp.Id == request.ProfileId, cancellationToken).ConfigureAwait(false)
            ?? throw new ArgumentException(nameof(request.ProfileId), $"Profile with identifier {request.ProfileId} not found from the configuration"); ; // Get the clicked profile.

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