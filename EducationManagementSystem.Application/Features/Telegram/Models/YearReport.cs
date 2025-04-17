namespace EducationManagementSystem.Application.Features.Telegram.Models;

internal record YearReport
{
    public required DateOnly YearStart { get; set; }
    public required DateOnly YearEnd { get; set; }
    public required int Lessons { get; set; }
    public required int CompletedLessons { get; set; }
    public required int CanceledLessons { get; set; }
    public required decimal FullIncome { get; set; }
    public required decimal SchoolIncome { get; set; }
    public required int ActiveStudents { get; set; }
    public required int ActiveTeachers { get; set; }
}