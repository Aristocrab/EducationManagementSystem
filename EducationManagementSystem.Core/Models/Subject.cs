using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Subject : Entity
{
    public required string Title { get; set; }
    public List<Teacher> Teachers { get; set; } = [];
    public List<Student> Students { get; set; } = [];
    public decimal CertificateMaxGrade { get; set; } = 100;
    public int MaxCertificates { get; set; } = 1; // todo
}
