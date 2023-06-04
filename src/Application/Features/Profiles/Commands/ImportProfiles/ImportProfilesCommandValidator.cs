using FluentValidation;

namespace Application.Features.Profiles.Commands.ImportProfiles;

public sealed class ImportProfilesCommandValidator : AbstractValidator<ImportProfilesCommand>
{
    public ImportProfilesCommandValidator()
    {
        RuleFor(c => c.Profiles).NotEmpty();
        RuleForEach(c => c.Profiles).Must(p => p.IsValid());
    }
}