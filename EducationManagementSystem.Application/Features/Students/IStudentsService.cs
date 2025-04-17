using EducationManagementSystem.Application.Features.Auth.Models;
using EducationManagementSystem.Application.Features.Students.Dtos;

namespace EducationManagementSystem.Application.Features.Students;

public interface IStudentsService
{
    Task<IReadOnlyList<StudentDto>> GetAllStudents(User currentUser);
    Task<StudentDto> GetStudentById(Guid id);
    Task AddStudent(NewStudentDto newStudent, User currentUser);
    Task EditStudent(Guid studentId, NewStudentDto student, User currentUser);
    Task DeleteStudent(Guid studentId, User currentUser);
}