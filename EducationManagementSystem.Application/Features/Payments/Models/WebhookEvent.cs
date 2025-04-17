namespace EducationManagementSystem.Application.Features.Payments.Models;

public class WebhookEvent
{
    public required string Type { get; set; }
    public required WebhookData Data { get; set; }
}

public class WebhookData
{
    public required string Account { get; set; }
    public required StatementItem StatementItem { get; set; }
}

public class StatementItem
{
    public required string Id { get; set; }
    public required long Time { get; set; }
    public required string Description { get; set; }
    public string? Comment { get; set; }
    public required int Amount { get; set; }
    public required int CurrencyCode { get; set; }
    public required int Balance { get; set; }
}