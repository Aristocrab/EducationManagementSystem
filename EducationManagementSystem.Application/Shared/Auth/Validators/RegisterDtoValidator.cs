using EducationManagementSystem.Application.Shared.Auth.Dtos;
using FluentValidation;

namespace EducationManagementSystem.Application.Shared.Auth.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(3);
    }
}