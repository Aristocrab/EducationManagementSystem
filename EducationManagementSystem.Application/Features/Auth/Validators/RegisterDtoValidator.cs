using EducationManagementSystem.Application.Features.Auth.Dtos;
using FluentValidation;

namespace EducationManagementSystem.Application.Features.Auth.Validators;

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