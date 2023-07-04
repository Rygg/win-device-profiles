namespace DeviceProfiles.Application.Features.Profiles.Commands.ActivateProfile;

public sealed class ActivateProfileCommandValidator : AbstractValidator<ActivateProfileCommand>
{
    public ActivateProfileCommandValidator()
    {
        RuleFor(c => c.ProfileId).NotEmpty();
    }
}