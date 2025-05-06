using EducationManagementSystem.Application.Features.Subjects.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;

namespace EducationManagementSystem.Application.Features.Subjects;

public interface ISubjectsService
{
    Task<IReadOnlyList<SubjectDto>> GetAllSubjects(User currentUser);
    Task<SubjectDto> GetSubjectById(Guid id);
    Task AddSubject(NewSubjectDto newSubjectDto, User currentUser);
    Task EditSubject(Guid subjectId, NewSubjectDto subjectDto, User currentUser);
    Task DeleteSubject(Guid subjectId, User currentUser);
}