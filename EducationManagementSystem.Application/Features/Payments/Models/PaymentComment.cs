namespace EducationManagementSystem.Application.Features.Payments.Models;

public class PaymentComment
{
    public required Guid StudentId { get; set; }
    public List<Guid>? Lessons { get; set; }
    public bool? All { get; set; }
}