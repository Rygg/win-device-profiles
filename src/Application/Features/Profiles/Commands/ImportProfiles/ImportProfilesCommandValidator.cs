namespace DeviceProfiles.Application.Features.Profiles.Commands.ImportProfiles;

public sealed class ImportProfilesCommandValidator : AbstractValidator<ImportProfilesCommand>
{
    public ImportProfilesCommandValidator()
    {
        RuleFor(c => c.ProfileFile).NotEmpty();
        RuleFor(c => c.ProfileFile).Must(pf => pf.Validate());
        RuleForEach(c => c.ProfileFile.Profiles).Must(p => p.IsValid());
    }
}