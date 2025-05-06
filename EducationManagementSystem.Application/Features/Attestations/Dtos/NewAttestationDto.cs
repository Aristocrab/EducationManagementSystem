using EducationManagementSystem.Core.Enums;

namespace EducationManagementSystem.Application.Features.Attestations.Dtos;

public class NewAttestationDto
{
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public AttestationResult Result { get; set; }
    public DateTime Date { get; set; }
}