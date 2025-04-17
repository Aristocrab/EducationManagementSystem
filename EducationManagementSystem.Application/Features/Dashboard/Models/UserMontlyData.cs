namespace EducationManagementSystem.Application.Features.Dashboard.Models;

public class UserMontlyData
{
    public required string MonthName { get; set; }
    public required int Lessons { get; set; }
    public required double FullIncome { get; set; }
    public required double SchoolIncome { get; set; }
    public required double TeacherIncome { get; set; }
}