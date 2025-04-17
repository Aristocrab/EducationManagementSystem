using EducationManagementSystem.Application.Features.Teachers;
using EducationManagementSystem.Application.Features.Teachers.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

[Route("teachers")]
public class TeachersController : BaseController
{
    private readonly ITeachersService _scheduleService;

    public TeachersController(ITeachersService scheduleService)
    {
        _scheduleService = scheduleService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Get all teachers")]
    public async Task<List<TeacherDto>> GetAllTeachers()
    {
        return await _scheduleService.GetAllTeachers(CurrentUser);
    }
    
    [HttpGet("withSchedule")]
    [SwaggerOperation(Summary = "Get all teachers with schedule")]
    public async Task<List<TeacherDto>> GetAllTeachersWithSchedule()
    {
        return await _scheduleService.GetAllTeachers(CurrentUser);
    }
    
    [HttpGet("{teacherId}")]
    [SwaggerOperation(Summary = "Get teacher by id")]
    public async Task<TeacherDto> GetTeacherById(Guid teacherId)
    {
        return await _scheduleService.GetTeacherById(teacherId, CurrentUser);
    }
}