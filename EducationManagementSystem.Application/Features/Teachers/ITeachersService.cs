using EducationManagementSystem.Application.Features.Auth.Dtos;
using EducationManagementSystem.Application.Features.Auth.Models;
using EducationManagementSystem.Application.Features.Teachers.Dtos;
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