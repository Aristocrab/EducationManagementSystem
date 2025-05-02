using EducationManagementSystem.Core.Models.Base;
using EducationManagementSystem.Core.ValueTypes;

namespace EducationManagementSystem.Core.Models;

public sealed class Session : Entity
{
    public required Student Student { get; set; }
    public required Subject Subject { get; set; }
    public required Grade Grade { get; set; } 
    public required DateTime Date { get; set; }
}
