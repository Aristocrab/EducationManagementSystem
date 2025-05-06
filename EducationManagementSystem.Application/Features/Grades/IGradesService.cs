using EducationManagementSystem.Application.Features.Grades.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;

namespace EducationManagementSystem.Application.Features.Grades;

public interface IGradesService
{
    Task<IReadOnlyList<SubjectGradeDto>> GetAllGrades(User currentUser);
    Task<SubjectGradeDto> GetGradeById(Guid id);
    Task AddGrade(NewSubjectGradeDto newGradeDto, User currentUser);
    Task EditGrade(Guid gradeId, NewSubjectGradeDto gradeDto, User currentUser);
    Task DeleteGrade(Guid gradeId, User currentUser);
}