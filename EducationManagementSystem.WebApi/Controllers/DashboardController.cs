using EducationManagementSystem.Application.Features.Dashboard;
using EducationManagementSystem.Application.Features.Dashboard.Models;
using EducationManagementSystem.Application.Features.Lessons.Dtos;
using EducationManagementSystem.Application.Features.Payments;
using EducationManagementSystem.Application.Features.Payments.Models;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationManagementSystem.WebApi.Controllers;

[Route("api/dashboard")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]
public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;
    private readonly IPaymentsService _paymentsService;

    public DashboardController(IDashboardService dashboardService, IPaymentsService paymentsService)
    {
        _dashboardService = dashboardService;
        _paymentsService = paymentsService;
    }
    
    [HttpGet("lessons")]
    public async Task<List<LessonDto>> GetLessons()
    {
        return await _dashboardService.GetAllLessons();
    }
    
    [HttpGet("currentMonth")]
    public async Task<CurrentMonthData> GetCurrentMonthData()
    {
        return await _dashboardService.GetCurrentMonthIncome();
    }
    
    [HttpGet("monthly")]
    public async Task<List<MonthlyData>> GetMonthlyData()
    {
        return await _dashboardService.GetMonthlyData();
    }
    
    [HttpGet("daily")]
    public async Task<List<DailyData>> GetDailyData()
    {
        return await _dashboardService.GetDailyData();
    }
    
    [HttpGet("students")]
    public async Task<List<StudentData>> GetStudentsData()
    {
        return await _dashboardService.GetStudentsList();
    }
    
    [HttpGet("teachers")]
    public async Task<List<StudentData>> GetTeachersData()
    {
        return await _dashboardService.GetTeachersList();
    }
    
    [HttpGet("student/{studentId}/monthly")]
    public async Task<List<UserMontlyData>> GetStudentMonthlyData(Guid studentId)
    {
        return await _dashboardService.GetStudentMonthlyData(studentId);
    }
    
    [HttpGet("teacher/{teacherId}/monthly")]
    public async Task<List<UserMontlyData>> GetTeacherMonthlyData(Guid teacherId)
    {
        return await _dashboardService.GetTeacherMonthlyData(teacherId);
    }
    
    [HttpGet("payments")]
    public async Task<List<PaymentDto>> GetPayments()
    {
        return await _paymentsService.GetPayments(CurrentUser);
    }
}