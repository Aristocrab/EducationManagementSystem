namespace EducationManagementSystem.Application.Features.Telegram.Models;

internal record MonthReport
{
    public DateOnly MonthStart { get; set; }
    public DateOnly MonthEnd { get; set; }
    public required string MonthName { get; set; }
    public int Lessons { get; set; }
    public int CompletedLessons { get; set; }
    public decimal FullIncome { get; set; }
    public decimal SchoolIncome { get; set; }
    public required int ActiveStudents { get; set; }
    public required int ActiveTeachers { get; set; }
}