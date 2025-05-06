using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Enums;


namespace EducationManagementSystem.Tests.Shared;

public abstract class ServiceTests
{ 
    protected readonly User Admin = new(Guid.NewGuid(), Role.Admin);
    protected readonly User Teacher = new(Guid.NewGuid(), Role.Teacher);
}