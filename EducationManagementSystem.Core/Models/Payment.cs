using EducationManagementSystem.Core.Models.Base;

namespace EducationManagementSystem.Core.Models;

public sealed class Payment : Entity
{
    public required string Account { get; set; }
    public required decimal AccountAmount { get; set; }
    public required string MonobankPaymentId { get; set; }
    public required string Description { get; set; }
    public required string? Comment { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required DateTime Time { get; set; }
    public bool IsConfirmed { get; set; }
    public bool AutoConfirmed { get; set; }
    public Student? Student { get; set; }
}