using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Lesson : Entity
{
    public required Teacher Teacher { get; init; }
    public required Student Student { get; set; }
    public required Subject Subject { get; set; }
    public List<Group> Groups { get; set; } = [];
    public List<SelectedSubjectGroup> SelectedSubjectGroups { get; set; } = [];
    public bool IsSelectedSubject { get; set; }
    public required DateTime DateTime { get; set; }
    public required TimeSpan Duration { get; set; }
    public required string Description { get; set; }
    public Status Status { get; set; } = Status.Pending;
}