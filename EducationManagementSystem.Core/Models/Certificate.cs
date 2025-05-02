using EducationManagementSystem.Core.Models.Base;
using EducationManagementSystem.Core.ValueTypes;

namespace EducationManagementSystem.Core.Models;

public sealed class Certificate : Entity
{
    public required Student Student { get; set; }
    public required Subject Subject { get; set; }
    public required string CourseTitle { get; set; }
    public required Grade CourseGrade { get; set; }
    public required Grade MaxGrade { get; set; } = Grade.From(100);
    public required string Issuer { get; set; }
    public required string Link { get; set; }
    public required DateTime IssuedAt { get; set; }
}