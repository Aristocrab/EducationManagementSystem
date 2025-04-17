using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class School : Entity
{
    public required decimal Balance { get; set; }
}