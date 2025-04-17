using EducationManagementSystem.Application.Features.Auth.Dtos;
using EducationManagementSystem.Application.Features.Teachers.Dtos;

namespace EducationManagementSystem.Application.Features.Auth;

public interface IAuthService
{
    Task<TeacherDto> GetCurrentUser(Guid currentUserId);
    Task<string> Login(LoginDto loginDto);
}