using Vogen;

namespace EducationManagementSystem.Core.ValueTypes;

[ValueObject<decimal>]
public readonly partial struct Grade
{
    private static Validation Validate(decimal input)
    {
        if (input is < 0 or > 100)
            return Validation.Invalid("Grade must be between 0 and 100.");

        return Validation.Ok;
    }
}