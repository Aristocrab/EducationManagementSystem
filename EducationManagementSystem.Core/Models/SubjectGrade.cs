using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class SubjectGrade : Entity
{
    public required Student Student { get; set; }
    public required Subject Subject { get; set; }
    public required decimal Value { get; set; }
    public required DateTime IssuedAt { get; set; }
}