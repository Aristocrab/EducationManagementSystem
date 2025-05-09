using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class SelectedSubjectGroup : Entity
{
    public required Subject Subject { get; set; }
    public int MaxStudents { get; set; } = 100;
    public List<Student> Students { get; set; } = [];
}