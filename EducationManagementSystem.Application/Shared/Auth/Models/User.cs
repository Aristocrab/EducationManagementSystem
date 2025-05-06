using EducationManagementSystem.Core.Enums;

namespace EducationManagementSystem.Application.Shared.Auth.Models;

public record User(Guid Id, Role? Role)
{
    public static User Admin => new(Guid.NewGuid(), Core.Enums.Role.Admin);
}