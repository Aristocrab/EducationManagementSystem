using EducationManagementSystem.Application.Features.Students.Dtos;
using FluentValidation;

namespace EducationManagementSystem.Application.Features.Students.Validators;

public class StudentDtoValidator: AbstractValidator<NewStudentDto>
{
    public StudentDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(3);
    }
}