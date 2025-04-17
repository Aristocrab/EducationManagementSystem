namespace EducationManagementSystem.Application.Features.Dashboard.Models;

public class DailyData
{
    public DateOnly Date { get; set; }
    public int Completed { get; set; }
    public int Canceled { get; set; }
}