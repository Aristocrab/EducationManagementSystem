using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Certificate : Entity
{
    public required Student Student { get; set; }
    public required Subject Subject { get; set; }
    public required string CourseTitle { get; set; }
    public required decimal CourseGrade { get; set; }
    public required string Issuer { get; set; }
    public required string Link { get; set; }
    public required DateTime IssuedAt { get; set; }
}