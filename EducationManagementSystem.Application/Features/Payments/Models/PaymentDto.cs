using EducationManagementSystem.Application.Features.Students.Dtos;

namespace EducationManagementSystem.Application.Features.Payments.Models;

public class PaymentDto
{
    public required Guid Id { get; set; }
    public required string MonobankPaymentId { get; set; }
    public required string Account { get; set; }
    public required string Name { get; set; }
    public required string? Comment { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required DateTime Time { get; set; }
    public bool IsConfirmed { get; set; }
    public bool AutoConfirmed { get; set; }
    public StudentDto? Student { get; set; }
    public bool AmountWarning { get; set; }
    
    public required List<StudentDto> SupposedStudents { get; set; }
}