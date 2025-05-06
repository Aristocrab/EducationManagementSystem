using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Exceptions;
using Throw;

namespace EducationManagementSystem.Application.Extensions;

public static class AuthThrowExtensions
{
    public static ref readonly Validatable<User> IfNotAdmin(this in Validatable<User> validatable)
    {
        validatable.Value.Role
            .ThrowIfNull(_ => new UnauthorizedException("Please log in"));
        
        if (validatable.Value.Role != Role.Admin)
        {
            throw new UnauthorizedException("You are not an admin");
        }

        return ref validatable;
    }
    
    public static ref readonly Validatable<User> IfNotAdminOrModerator(
        this in Validatable<User> validatable)
    {
        validatable.Value.Role
            .ThrowIfNull(_ => new UnauthorizedException("Please log in"));
        
        if (validatable.Value.Role == Role.Teacher)
        {
            throw new UnauthorizedException("You are not an admin or moderator");
        }

        return ref validatable;
    }
}