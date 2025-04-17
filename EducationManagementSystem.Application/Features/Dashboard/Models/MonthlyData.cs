namespace EducationManagementSystem.Application.Features.Dashboard.Models;

public record MonthlyData
{
    public required DateOnly Month { get; set; }
    public required string MonthName { get; set; }
    public required decimal Income { get; set; }
}