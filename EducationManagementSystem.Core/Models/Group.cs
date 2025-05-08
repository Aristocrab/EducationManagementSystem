using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Group : Entity
{
    public required string GroupId { get; set; }
    public List<Student> Students { get; set; } = [];
}