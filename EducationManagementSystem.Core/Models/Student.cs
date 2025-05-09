using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Student : Entity
{
    public required string FullName { get; set; }

    public List<Lesson> Lessons { get; init; } = [];
    
    public required Group Group { get; set; }

    public List<SubjectGrade> Grades { get; set; } = [];

    public List<Attestation> Attestations { get; set; } = [];

    public List<Certificate> Certificates { get; set; } = [];

    public List<Subject> Subjects { get; set; } = [];
}