using EducationManagementSystem.Application.Features.Teachers;
using EducationManagementSystem.Application.Features.Teachers.Dtos;
using EducationManagementSystem.Application.Shared.Auth;
using EducationManagementSystem.Application.Shared.Auth.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using EducationManagementSystem.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly ITeachersService _teachersService;

    public AuthController(IAuthService authService, ITeachersService teachersService)
    {
        _authService = authService;
        _teachersService = teachersService;
    }
    
    [HttpGet("currentUser")]
    [SwaggerOperation(Summary = "Get current user via jwt")]
    public async Task<TeacherDto> GetCurrentUser()
    {
        return await _authService.GetCurrentUser(CurrentUser.Id);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login")]
    public async Task<string> Login(LoginDto loginDto)
    {
        return await _authService.Login(loginDto);
    }
    
    [HttpPost("registerTeacher")]
    [SwaggerOperation(Summary = "Register a new teacher")]
    public async Task RegisterTeacher(RegisterDto registerDto)
    {
        await _teachersService.RegisterTeacher(registerDto, CurrentUser);
    }
    
    [HttpPost("registerModerator")]
    [SwaggerOperation(Summary = "Register a new moderator")]
    public async Task RegisterModerator(RegisterDto registerDto)
    {
        await _teachersService.RegisterTeacher(registerDto, CurrentUser, Role.Moderator);
    }
    
    [HttpPut("editWorkingHours/{teacherId}")]
    [SwaggerOperation(Summary = "Edit teacher's working hours")]
    public async Task EditTeacherWorkingHours(Guid teacherId, string workingHours = "")
    {
        await _teachersService.EditTeacherWorkingHours(teacherId, workingHours, CurrentUser);
    }
    
    [HttpPut("editTeacherBalance/{teacherId}")]
    [SwaggerOperation(Summary = "Edit teacher's balance")]
    public async Task EditTeacherBalance(Guid teacherId, decimal balance)
    {
        await _teachersService.EditTeacherBalance(teacherId, balance, CurrentUser);
    }
    
    [HttpDelete("deleteTeacher/{teacherId}")]
    [SwaggerOperation(Summary = "Delete a teacher")]
    public async Task DeleteTeacher(Guid teacherId)
    {
        await _teachersService.DeleteTeacher(teacherId, CurrentUser);
    }
}