using EducationManagementSystem.Core.Models.Base;
using EducationManagementSystem.Core.ValueTypes;

namespace EducationManagementSystem.Core.Models;

public sealed class Group : Entity
{
    public required GroupId GroupId { get; set; }
    public List<Student> Students { get; set; } = [];
}