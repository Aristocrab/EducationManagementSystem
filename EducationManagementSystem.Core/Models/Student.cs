using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Student : Entity
{
    public required string FullName { get; set; }

    public string? MessengerLink { get; set; }
    
    public List<string> Languages { get; set; } = [];
    public List<Lesson> Lessons { get; init; } = [];
}