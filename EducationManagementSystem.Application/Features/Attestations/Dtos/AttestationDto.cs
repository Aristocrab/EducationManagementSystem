using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Features.Subjects.Dtos;
using EducationManagementSystem.Core.Enums;

namespace EducationManagementSystem.Application.Features.Attestations.Dtos;

public class AttestationDto
{
    public Guid Id { get; set; }
    public required StudentDto Student { get; set; } 
    public required SubjectDto Subject { get; set; }
    public AttestationResult Result { get; set; }
    public DateTime Date { get; set; }
}