using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class AllowedCertificate : Entity
{
    public required string Name { get; set; }
    public required Subject Subject { get; set; }
}