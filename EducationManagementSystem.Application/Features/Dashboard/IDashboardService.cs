using EducationManagementSystem.Application.Features.Dashboard.Models;
using EducationManagementSystem.Application.Features.Lessons.Dtos;

namespace EducationManagementSystem.Application.Features.Dashboard;

public interface IDashboardService
{
    Task<List<LessonDto>> GetAllLessons();
    Task<CurrentMonthData> GetCurrentMonthIncome();
    Task<List<DailyData>> GetDailyData();
    Task<List<MonthlyData>> GetMonthlyData();
    Task<List<StudentData>> GetStudentsList();
    Task<List<StudentData>> GetTeachersList();
    Task<List<UserMontlyData>> GetStudentMonthlyData(Guid studentId);
    Task<List<UserMontlyData>> GetTeacherMonthlyData(Guid teacherId);
}