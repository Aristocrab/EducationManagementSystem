using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Attestation : Entity
{
    public required Student Student { get; set; }
    public required Subject Subject { get; set; }
    public AttestationResult Result { get; set; } = AttestationResult.NotSelected;
    public required DateTime Date { get; set; }
}