using FluentValidation;

namespace Application.Features.Profiles.Commands;

public sealed class ActivateProfileCommandValidator : AbstractValidator<ActivateProfileCommand>
{
    public ActivateProfileCommandValidator()
    {
        RuleFor(c => c.ProfileId).NotEmpty();
    }
}