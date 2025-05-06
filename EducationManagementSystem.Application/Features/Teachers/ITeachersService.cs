using EducationManagementSystem.Application.Features.Teachers.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Enums;

namespace EducationManagementSystem.Application.Features.Teachers;

public interface ITeachersService
{
    Task<List<TeacherDto>> GetAllTeachers(User currentUser);
    Task<TeacherDto> GetTeacherById(Guid teacherId, User currentUser);
    Task RegisterTeacher(RegisterDto registerDto, User currentUser, Role role = Role.Teacher);
    Task EditTeacherWorkingHours(Guid teacherId, string workingHours, User currentUser);
    Task EditTeacherBalance(Guid teacherId, decimal balance, User currentUser);
    Task DeleteTeacher(Guid teacherId, User currentUser);
}