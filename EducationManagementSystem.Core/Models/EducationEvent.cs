using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class EducationEvent : Entity
{
    public required EducationEventType EventType { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
}