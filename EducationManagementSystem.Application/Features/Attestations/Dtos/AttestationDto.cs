using EducationManagementSystem.Core.Enums;

namespace EducationManagementSystem.Application.Features.Attestations.Dtos;

public class AttestationDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = default!;
    public string SubjectTitle { get; set; } = default!;
    public AttestationResult Result { get; set; }
    public DateTime Date { get; set; }
}