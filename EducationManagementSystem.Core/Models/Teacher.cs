using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Teacher : Entity
{
    public Role Role { get; init; }
    
    public required string FullName { get; init; }
    public required string Username { get; init; }
    public required string PasswordHash { get; init; }
    public required string PasswordSalt { get; init; }
    
    public required DateTime RegisteredAt { get; init; }
    
    public List<Lesson> Lessons { get; init; } = [];
    public List<Subject> Subjects { get; init; } = [];
    public List<Group> Groups { get; init; } = [];
}