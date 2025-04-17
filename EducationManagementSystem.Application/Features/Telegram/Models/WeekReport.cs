namespace EducationManagementSystem.Application.Features.Telegram.Models;

internal record WeekReport
{
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public int Lessons { get; set; }
    public int CompletedLessons { get; set; }
    public decimal FullIncome { get; set; }
    public decimal SchoolIncome { get; set; }
}