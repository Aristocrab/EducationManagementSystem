namespace EducationManagementSystem.Application.Features.Payments.Models;

public class WebhookFopEvent
{
    public required string InvoiceId { get; set; }
    public required string Status { get; set; }
    public int Amount { get; set; }
    public required string Destination { get; set; }
    public DateTime CreatedDate { get; set; }
}
