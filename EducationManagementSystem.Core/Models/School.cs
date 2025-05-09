using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class School : Entity
{
    public string SchoolName { get; set; } = string.Empty;
}