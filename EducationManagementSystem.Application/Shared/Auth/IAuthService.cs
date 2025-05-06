using EducationManagementSystem.Application.Features.Teachers.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Dtos;

namespace EducationManagementSystem.Application.Shared.Auth;

public interface IAuthService
{
    Task<TeacherDto> GetCurrentUser(Guid currentUserId);
    Task<string> Login(LoginDto loginDto);
}